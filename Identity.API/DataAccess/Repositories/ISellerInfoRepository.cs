﻿using Common.DataAccess;
using Identity.API.Domain;
using System;
using System.Threading.Tasks;

namespace Identity.API.DataAccess.Repositories
{
    public interface ISellerInfoRepository : IDomainRepository<SellerInfo>
    {
        Task<SellerInfo> GetByIdAsync(Guid id);
        Task<SellerInfo> GetByUserIdAsync(Guid userId);
        void Add(SellerInfo sellerInfo);
        void Update(SellerInfo sellerInfo);
    }
}
