﻿using Finalaplication.App_Start;
using Finalaplication.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using VolCommon;

namespace Finalaplication.DatabaseManager
{
    public class VolContractManager
    {
        private MongoDBContextLocal dbcontext = new MongoDBContextLocal();

        internal void DeleteAVolunteersContracts(string id)
        {
            IMongoCollection<Volcontract> contractcollection = dbcontext.DatabaseLocal.GetCollection<Volcontract>("Contracts");
            contractcollection.DeleteMany(Builders<Volcontract>.Filter.Eq("OwnerID", id));
        }

        internal void DeleteAVolContract(string id)
        {
            IMongoCollection<Volcontract> contractcollection = dbcontext.DatabaseLocal.GetCollection<Volcontract>("Contracts");
            contractcollection.DeleteOne(Builders<Volcontract>.Filter.Eq("_id", ObjectId.Parse(id)));
        }

        internal void AddVolunteerContractToDB(Volcontract contract)
        {
            IMongoCollection<Volcontract> volcontractcollection = dbcontext.DatabaseLocal.GetCollection<Volcontract>("Contracts");
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
            IMongoCollection<Volcontract> volcontractcollection = dbcontext.DatabaseLocal.GetCollection<Volcontract>("Contracts");
            var filter = Builders<Volcontract>.Filter.Eq("_id", ObjectId.Parse(id));
            Volcontract returnVolContract = volcontractcollection.Find(filter).FirstOrDefault();
            return returnVolContract;
        }
        internal List<Volcontract> GetListOfVolunteersContracts()
        {
            IMongoCollection<Volcontract> volcontractcollection = dbcontext.DatabaseLocal.GetCollection<Volcontract>("Contracts");
            List<Volcontract> contracts = volcontractcollection.AsQueryable().ToList();
            return contracts;
        }

        internal void UpdateVolunteerContract(FilterDefinition<Volcontract> filter, UpdateDefinition<Volcontract> contract_toupdate)
        {
            IMongoCollection<Volcontract> volcontractcollection = dbcontext.DatabaseLocal.GetCollection<Volcontract>("Contracts");
            volcontractcollection.UpdateOne(filter, contract_toupdate);
        }
    }
}