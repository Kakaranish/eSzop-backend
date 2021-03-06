﻿using System;

namespace Common.EventBus.IntegrationEvents
{
    public class OfferBecameUnavailableIntegrationEvent : IntegrationEvent
    {
        public Guid OfferId { get; init; }
        public UnavailabilityTrigger Trigger { get; init; }
    }
}
