﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using VolCommon;

namespace Finalaplication.Models
{
    public class Event : EventBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string EventID { get; set; }
    }
}
