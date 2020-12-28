﻿using Common.Types;
using Microsoft.EntityFrameworkCore;
using Offers.API.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.API.DataAccess.Repositories
{
    public class OfferRepository : IOfferRepository
    {
        private readonly AppDbContext _appDbContext;

        public IUnitOfWork UnitOfWork => _appDbContext;

        public OfferRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task AddAsync(Offer offer)
        {
            await _appDbContext.Offers.AddAsync(offer);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<Offer> GetByIdAsync(Guid offerId)
        {
            return await _appDbContext.Offers.FirstOrDefaultAsync(x => x.Id == offerId);
        }

        public async Task<IList<Offer>> GetAllAsync()
        {
            return await _appDbContext.Offers.ToListAsync();
        }
    }
}