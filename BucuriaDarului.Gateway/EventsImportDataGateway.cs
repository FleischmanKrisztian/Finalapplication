﻿using BucuriaDarului.Core;
using BucuriaDarului.Core.Gateways;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace BucuriaDarului.Gateway
{
    public class EventsImportDataGateway : IEventsImportDataGateway
    {
        MongoDBGateway dBContext = new MongoDBGateway();
        internal static string SERVER_NAME_LOCAL = Environment.GetEnvironmentVariable("volmongo_server");
        internal static int SERVER_PORT_LOCAL = int.Parse(Environment.GetEnvironmentVariable("volmongo_port"));
        internal static string DATABASE_NAME_LOCAL = Environment.GetEnvironmentVariable("volmongo_databasename");

        public void Insert(List<Event> events)
        {
            dBContext.ConnectToDB(SERVER_NAME_LOCAL, SERVER_PORT_LOCAL, DATABASE_NAME_LOCAL);
            IMongoCollection<Event> eventcollection = dBContext.Database.GetCollection<Event>("Events");
            eventcollection.InsertMany(events);
            ModifiedIDGateway modifiedIDGateway = new ModifiedIDGateway();
            foreach (var eve in events)
            {
                modifiedIDGateway.AddIDtoModifications(eve._id);
            }
        }
    }
}