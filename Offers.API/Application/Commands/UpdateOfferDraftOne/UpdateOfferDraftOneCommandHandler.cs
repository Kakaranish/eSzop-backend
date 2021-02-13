﻿using Common.EventBus;
using Common.EventBus.IntegrationEvents;
using Common.Extensions;
using Common.Types;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Offers.API.Application.Types;
using Offers.API.DataAccess.Repositories;
using Offers.API.Domain;
using Offers.API.Services;
using Offers.API.Services.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Offers.API.Application.Commands.UpdateOfferDraftOne
{
    public class UpdateOfferDraftOneCommandHandler : IRequestHandler<UpdateOfferDraftOneCommand>
    {
        private readonly ILogger<UpdateOfferDraftOneCommandHandler> _logger;
        private readonly IOfferRepository _offerRepository;
        private readonly HttpContext _httpContext;
        private readonly IEventBus _eventBus;
        private readonly IImageStorage _imageStorage;

        public UpdateOfferDraftOneCommandHandler(ILogger<UpdateOfferDraftOneCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor, IOfferRepository offerRepository, IEventBus eventBus,
            IImageStorage imageStorage)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContext = httpContextAccessor.HttpContext ??
                           throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));
            _offerRepository = offerRepository ?? throw new ArgumentNullException(nameof(offerRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _imageStorage = imageStorage ?? throw new ArgumentNullException(nameof(imageStorage));
        }

        public async Task<Unit> Handle(UpdateOfferDraftOneCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContext.User.Claims.ToTokenPayload().UserClaims.Id;
            var offer = await _offerRepository.GetByIdAsync(Guid.Parse(request.OfferId));
            if (offer is null || offer.OwnerId != userId)
            {
                var msg = $"Offer {request.OfferId} not found";
                _logger.LogError(msg);
                throw new OffersDomainException(msg);
            }

            var @event = new OfferChangedIntegrationEvent
            {
                OfferId = offer.Id,
                Name = new ChangeState<string>(offer.Name, request.Name),
                Description = new ChangeState<string>(offer.Description, request.Description),
                Price = new ChangeState<decimal?>(offer.Price, request.Price),
                AvailableStock = new ChangeState<int?>(offer.AvailableStock, request.AvailableStock)
            };

            var keyValueInfos = ExtractKeyValueInfos(request);
            offer.SetKeyValueInfos(keyValueInfos);

            await ProcessOfferImages(request, offer);

            if (@event.Name.Changed) offer.SetName(request.Name);
            if (@event.Description.Changed) offer.SetDescription(request.Description);
            if (@event.Price.Changed) offer.SetPrice(request.Price.Value);

            _offerRepository.Update(offer);
            var shouldEventBePublished = await _offerRepository.UnitOfWork
                .SaveChangesAndDispatchDomainEventsAsync(cancellationToken);

            if (shouldEventBePublished)
            {
                await _eventBus.PublishAsync(@event);
                _logger.LogInformation($"Published {nameof(OfferChangedIntegrationEvent)} integration event");
            }

            return await Unit.Task;
        }

        private async Task ProcessOfferImages(UpdateOfferDraftOneCommand request, Offer offer)
        {
            var imagesMetadataDict = ExtractImagesMetadata(request);

            var imagesToRemove = offer.Images.Where(x => !imagesMetadataDict.ContainsKey(x.Id.ToString())).ToList();
            foreach (var imageToRemove in imagesToRemove)
            {
                await _imageStorage.DeleteAsync(imageToRemove.Filename);
                offer.RemoveImage(imageToRemove);
            }

            var currentOfferImages = new List<ImageInfo>();
            ImageInfo mainImage = null;

            foreach (var offerImage in offer.Images)
            {
                var imageMetadata = imagesMetadataDict[offerImage.Id.ToString()];
                if (!imageMetadata.IsRemote)
                    throw new OffersDomainException($"Invalid metadata entry - '{nameof(imageMetadata.IsRemote)}' should be true");

                offerImage.SetIsMain(imageMetadata.IsMain);
                if (offerImage.IsMain)
                {
                    offerImage.SetSortId(0);
                    mainImage = offerImage;
                }
                else
                {
                    offerImage.SetSortId(imageMetadata.SortId);
                    currentOfferImages.Add(offerImage);
                }

                imagesMetadataDict.Remove(offerImage.Id.ToString());
            }

            offer.ClearImages();

            if (imagesMetadataDict.Any(x => x.Value.IsRemote))
                throw new OffersDomainException("Uploaded image cannot be marked as isRemote");

            var uploadedImages = new List<ImageInfo>();

            if (request.Images != null)
            {
                foreach (var requestImageFile in request.Images)
                {
                    var id = Path.GetFileNameWithoutExtension(requestImageFile.FileName);
                    var metadata = imagesMetadataDict[id];

                    var uploadedImage = (await _imageStorage.UploadAsync(requestImageFile)).ToImageInfo();
                    uploadedImage.SetIsMain(metadata.IsMain);

                    if (uploadedImage.IsMain)
                    {
                        uploadedImage.SetSortId(0);
                        mainImage = uploadedImage;
                    }
                    else
                    {
                        uploadedImage.SetSortId(metadata.SortId);
                        uploadedImages.Add(uploadedImage);
                    }
                }
            }

            offer.AddImage(mainImage);
            foreach (var (offerImage, index) in
                new List<ImageInfo>(currentOfferImages).Concat(uploadedImages).OrderBy(x => x.SortId).WithIndex(1))
            {
                offerImage.SetSortId(index);
                offer.AddImage(offerImage);
            }
        }

        private static Dictionary<string, ImageMetadata> ExtractImagesMetadata(UpdateOfferDraftOneCommand request)
        {
            var imagesMetadataList = JsonConvert.DeserializeObject<IList<ImageMetadata>>(request.ImagesMetadata);
            var metadataDict = imagesMetadataList.ToDictionary(x => x.ImageId);

            if (imagesMetadataList == null)
                throw new OffersDomainException("Invalid images metadata");
            if (imagesMetadataList.Count == 0)
                throw new OffersDomainException("Min number of images is 1");
            if (imagesMetadataList.Count > 5)
                throw new OffersDomainException("Max number of images is 5");

            var mainImages = imagesMetadataList.Where(x => x.IsMain).ToList();
            if (mainImages.Count == 0)
                throw new OffersDomainException("No main image indicated");
            if (mainImages.Count > 1)
                throw new OffersDomainException("Possible only 1 main image");

            if (request.Images != null)
            {
                var imagesIdList = request.Images.Select(img => Path.GetFileNameWithoutExtension(img.FileName));
                if (imagesIdList.Any(id => !metadataDict.ContainsKey(id)))
                    throw new OffersDomainException("Invalid images metadata");
            }

            return metadataDict;
        }

        private static IList<KeyValueInfo> ExtractKeyValueInfos(UpdateOfferDraftOneCommand request)
        {
            if (request.KeyValueInfos == null) return null;

            var extractKeyValueInfos = JsonConvert.DeserializeObject<IList<KeyValueInfo>>(request.KeyValueInfos)
                                       ?? throw new OffersDomainException($"'{request.KeyValueInfos}' is not parsable");

            return extractKeyValueInfos;
        }
    }
}