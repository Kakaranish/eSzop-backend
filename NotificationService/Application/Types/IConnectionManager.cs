﻿using System;
using System.Collections.Generic;

namespace NotificationService.Application.Types
{
    public interface IConnectionManager
    {
        void Add(Guid userId, string connectionId);
        IEnumerable<string> Get(Guid userId);
        void Remove(Guid userId, string connectionId);
    }
}