﻿using Finalaplication.App_Start;
using Finalaplication.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finalaplication.CommonDatabaseManager
{
    public class BeneficiaryManager
    {
        private MongoDBContextCommon dbContextCommon = new MongoDBContextCommon();

        internal List<Beneficiary> GetListOfBeneficiaries()
        {
            IMongoCollection<Beneficiary> beneficiarycollection = dbContextCommon.DatabaseCommon.GetCollection<Beneficiary>("Beneficiaries");
            List<Beneficiary> beneficiaries = beneficiarycollection.AsQueryable().ToList();
            return beneficiaries;
        }

        internal void AddBeneficiaryToDB(Beneficiary beneficiary)
        {
            IMongoCollection<Beneficiary> beneficiarycollection = dbContextCommon.DatabaseCommon.GetCollection<Beneficiary>("Beneficiaries");
            try
            {
                beneficiary._id = Guid.NewGuid().ToString();
                beneficiarycollection.InsertOne(beneficiary);
            }
            catch
            {
                Console.WriteLine("There was an error adding Beneficiary!");
            }
        }

        internal Beneficiary GetOneBeneficiary(string id)
        {
            IMongoCollection<Beneficiary> beneficiarycollection = dbContextCommon.DatabaseCommon.GetCollection<Beneficiary>("Beneficiaries");
            var filter = Builders<Beneficiary>.Filter.Eq("_id", ObjectId.Parse(id));
            Beneficiary beneficiary = beneficiarycollection.Find(filter).FirstOrDefault();
            return beneficiary;
        }


        internal void UpdateABeneficiary(Beneficiary beneficiarytopdate, string id)
        {
            IMongoCollection<Beneficiary> Beneficiarycollection = dbContextCommon.DatabaseCommon.GetCollection<Beneficiary>("Beneficiaries");
            var filter = Builders<Beneficiary>.Filter.Eq("_id", ObjectId.Parse(id));
            beneficiarytopdate._id = id;
            Beneficiarycollection.FindOneAndReplace(filter, beneficiarytopdate);
        }

        internal void DeleteBeneficiary(string id)
        {
            IMongoCollection<Beneficiary> beneficiarycollection = dbContextCommon.DatabaseCommon.GetCollection<Beneficiary>("Beneficiaries");
            beneficiarycollection.DeleteOne(Builders<Beneficiary>.Filter.Eq("_id", ObjectId.Parse(id)));
        }

    }
}
