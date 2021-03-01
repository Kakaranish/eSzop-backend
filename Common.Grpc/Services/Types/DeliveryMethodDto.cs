﻿using System.Runtime.Serialization;

namespace Common.Grpc.Services.Types
{
    [DataContract]
    public class DeliveryMethodDto
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public decimal Price { get; set; }
    }
}
