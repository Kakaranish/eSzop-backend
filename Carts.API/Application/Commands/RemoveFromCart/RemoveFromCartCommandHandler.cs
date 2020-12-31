﻿using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Carts.API.DataAccess.Repositories;
using Carts.API.Domain;
using Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Carts.API.Application.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand>
    {
        private readonly ILogger<RemoveFromCartCommandHandler> _logger;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly HttpContext _httpContext;

        public RemoveFromCartCommandHandler(ILogger<RemoveFromCartCommandHandler> logger, 
            ICartItemRepository cartItemRepository, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cartItemRepository = cartItemRepository ?? throw new ArgumentNullException(nameof(cartItemRepository));
            _httpContext = httpContextAccessor.HttpContext ??
                           throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));
        }

        public async Task<Unit> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContext.User.Claims.ToTokenPayload().UserClaims.Id;
            var cartItemId = Guid.Parse(request.CartItemId);
            var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);

            if (cartItem is null || userId != cartItem.SellerId)
            {
                throw new CartsDomainException($"Cart item {cartItemId} not found");
            }

            _cartItemRepository.Remove(cartItem);
            await _cartItemRepository.UnitOfWork.SaveChangesAndDispatchDomainEventsAsync(cancellationToken);

            _logger.LogInformation($"Removed cart item {cartItem.Id} from cart {cartItem.CartId}");
            
            return await Unit.Task;
        }
    }
}