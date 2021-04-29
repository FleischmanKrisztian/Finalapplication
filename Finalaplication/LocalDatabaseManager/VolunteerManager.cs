﻿using Finalaplication.App_Start;
using Finalaplication.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Finalaplication.LocalDatabaseManager
{
    public class VolunteerManager
    {
        private MongoDBContextLocal dBContextLocal = new MongoDBContextLocal();
        ModifiedDocumentManager modifiedDocumentManager = new ModifiedDocumentManager();

        internal void AddVolunteerToDB(Volunteer volunteer)
        {
            IMongoCollection<Volunteer> volunteercollection = dBContextLocal.DatabaseLocal.GetCollection<Volunteer>("Volunteers");
            try
            {
                modifiedDocumentManager.AddIDtoString(volunteer._id);
                volunteercollection.InsertOne(volunteer);
            }
            catch
            {
                Console.WriteLine("There was an error adding Volunteer!");
            }
        }

        internal Volunteer GetOneVolunteer(string id)
        {
            IMongoCollection<Volunteer> volunteercollection = dBContextLocal.DatabaseLocal.GetCollection<Volunteer>("Volunteers");
            var filter = Builders<Volunteer>.Filter.Eq("_id", id);
            Volunteer volunteer = volunteercollection.Find(filter).FirstOrDefault();
            return volunteer;
        }

        internal List<Volunteer> GetListOfVolunteers()
        {
            IMongoCollection<Volunteer> volunteercollection = dBContextLocal.DatabaseLocal.GetCollection<Volunteer>("Volunteers");
            List<Volunteer> volunteers = volunteercollection.AsQueryable().ToList();
            return volunteers;
        }

        internal void UpdateAVolunteer(Volunteer volunteertopdate, string id)
        {
            IMongoCollection<Volunteer> volunteercollection = dBContextLocal.DatabaseLocal.GetCollection<Volunteer>("Volunteers");
            var filter = Builders<Volunteer>.Filter.Eq("_id",id);
            volunteertopdate._id = id;
            modifiedDocumentManager.AddIDtoString(volunteertopdate._id);
            volunteercollection.FindOneAndReplace(filter, volunteertopdate);
        }

        internal void DeleteAVolunteer(string id)
        {
            IMongoCollection<Volunteer> volunteercollection = dBContextLocal.DatabaseLocal.GetCollection<Volunteer>("Volunteers");
            modifiedDocumentManager.AddIDtoDeletionString(id);
            volunteercollection.DeleteOne(Builders<Volunteer>.Filter.Eq("_id", id));
        }
    }
}