﻿using Common.DataAccess;
using Common.Extensions;
using Common.Types;
using Microsoft.EntityFrameworkCore;
using Orders.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.API.DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _appDbContext;

        public IUnitOfWork UnitOfWork => _appDbContext;

        public OrderRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task<Order> GetByIdAsync(Guid orderId)
        {
            return await _appDbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        }

        public async Task<IList<Order>> GetAllStartedOrdersByOfferId(Guid offerId)
        {
            return await _appDbContext.Orders.Where(x =>
                    x.OrderState == OrderState.Started && x.OrderItems.Any(order => order.OfferId == offerId))
                .ToListAsync();
        }

        public async Task<bool> GetOfferHasAnyOrders(Guid offerId)
        {
            return await _appDbContext.OrderItems.AnyAsync(x => x.OfferId == offerId);
        }

        public async Task<Pagination<Order>> GetAllByBuyerIdAsync(Guid userId, BasicPaginationFilter filter)
        {
            var orders = _appDbContext.Orders
                .AsQueryable()
                .Where(x => x.BuyerId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.OrderItems);
            var pageDetails = new PageCriteria(filter.PageIndex, filter.PageSize);

            return await orders.PaginateAsync(pageDetails);
        }

        public async Task<Pagination<Order>> GetAllBySellerIdAsync(Guid userId, BasicPaginationFilter filter)
        {
            var orders = _appDbContext.Orders
                .AsQueryable()
                .Where(x => x.SellerId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.OrderItems);
            var pageDetails = new PageCriteria(filter.PageIndex, filter.PageSize);

            return await orders.PaginateAsync(pageDetails);
        }

        public void Add(Order offer)
        {
            _appDbContext.Orders.Add(offer);
        }

        public void Update(Order order)
        {
            _appDbContext.Update(order);
        }
    }
}
