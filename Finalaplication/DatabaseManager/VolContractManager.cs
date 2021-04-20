﻿using Finalaplication.App_Start;
using Finalaplication.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Finalaplication.LocalDatabaseManager
{
    public class VolContractManager
    {
        private MongoDBContext dBContext;

        public VolContractManager(string SERVER_NAME_LOCAL, int SERVER_PORT_LOCAL, string DATABASE_NAME_LOCAL)
        {
            dBContext = new MongoDBContext(SERVER_NAME_LOCAL, SERVER_PORT_LOCAL, DATABASE_NAME_LOCAL);
        }

        internal void DeleteAVolunteersContracts(string id)
        {
            IMongoCollection<Volcontract> contractcollection = dBContext.Database.GetCollection<Volcontract>("Contracts");
            contractcollection.DeleteMany(Builders<Volcontract>.Filter.Eq("OwnerID", id));
        }

        internal void DeleteAVolContract(string id)
        {
            IMongoCollection<Volcontract> contractcollection = dBContext.Database.GetCollection<Volcontract>("Contracts");
            contractcollection.DeleteOne(Builders<Volcontract>.Filter.Eq("_id", id));
        }

        internal void AddVolunteerContractToDB(Volcontract contract)
        {
            IMongoCollection<Volcontract> volcontractcollection = dBContext.Database.GetCollection<Volcontract>("Contracts");
            try
            {
                volcontractcollection.InsertOne(contract);
            }
            catch
            {
                Console.WriteLine("There was an error adding Sponsor");
            }
        }

        internal Volcontract GetVolunteerContract(string id)
        {
            IMongoCollection<Volcontract> volcontractcollection = dBContext.Database.GetCollection<Volcontract>("Contracts");
            var filter = Builders<Volcontract>.Filter.Eq("_id", id);
            Volcontract returnVolContract = volcontractcollection.Find(filter).FirstOrDefault();
            return returnVolContract;
        }

        internal List<Volcontract> GetListOfVolunteersContracts()
        {
            IMongoCollection<Volcontract> volcontractcollection = dBContext.Database.GetCollection<Volcontract>("Contracts");
            List<Volcontract> contracts = volcontractcollection.AsQueryable().ToList();
            return contracts;
        }

        internal void UpdateVolunteerContract(FilterDefinition<Volcontract> filter, UpdateDefinition<Volcontract> contract_toupdate)
        {
            IMongoCollection<Volcontract> volcontractcollection = dBContext.Database.GetCollection<Volcontract>("Contracts");
            volcontractcollection.UpdateOne(filter, contract_toupdate);
        }

        internal void UpdateVolunteerContract(Volcontract contractupdate, string id)
        {
            IMongoCollection<Volcontract> volcontractcollection = dBContext.Database.GetCollection<Volcontract>("Contracts");
            var filter = Builders<Volcontract>.Filter.Eq("_id", id);
            contractupdate._id = id;
            volcontractcollection.FindOneAndReplace(filter, contractupdate);
        }
    }
}