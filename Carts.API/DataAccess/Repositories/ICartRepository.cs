﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Carts.API.Domain;
using Common.Types;

namespace Carts.API.DataAccess.Repositories
{
    public interface ICartRepository : IDomainRepository<Cart>
    {
        Task<IList<Cart>> GetAllAsync();
        Task<Cart> GetByIdAsync(Guid id);
        Task<Cart> GetOrCreateByUserIdAsync(Guid userId);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
    }
}