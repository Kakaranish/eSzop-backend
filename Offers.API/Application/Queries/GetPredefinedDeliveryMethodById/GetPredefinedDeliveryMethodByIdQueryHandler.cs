﻿using MediatR;
using Offers.API.Application.Dto;
using Offers.API.DataAccess.Repositories;
using Offers.API.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Offers.API.Application.Queries.GetPredefinedDeliveryMethodById
{
    public class GetPredefinedDeliveryMethodByIdQueryHandler :
        IRequestHandler<GetPredefinedDeliveryMethodByIdQuery, PredefinedDeliveryMethodDto>
    {
        private readonly IPredefinedDeliveryMethodRepository _deliveryMethodRepository;

        public GetPredefinedDeliveryMethodByIdQueryHandler(IPredefinedDeliveryMethodRepository deliveryMethodRepository)
        {
            _deliveryMethodRepository = deliveryMethodRepository ??
                                        throw new ArgumentNullException(nameof(deliveryMethodRepository));
        }

        public async Task<PredefinedDeliveryMethodDto> Handle(GetPredefinedDeliveryMethodByIdQuery request,
            CancellationToken cancellationToken)
        {
            var deliveryMethodId = Guid.Parse(request.DeliveryMethodId);
            var deliveryMethod = await _deliveryMethodRepository.GetById(deliveryMethodId);

            return deliveryMethod.ToDto();
        }
    }
}
