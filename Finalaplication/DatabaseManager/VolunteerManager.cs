﻿using Finalaplication.App_Start;
using Finalaplication.Models;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Finalaplication.LocalDatabaseManager
{
    public class VolunteerManager
    {
        private MongoDBContext dBContext;
        private ModifiedDocumentManager modifiedDocumentManager = new ModifiedDocumentManager();

        public VolunteerManager(string SERVER_NAME, int SERVER_PORT, string DATABASE_NAME)
        {
            dBContext = new MongoDBContext(SERVER_NAME, SERVER_PORT, DATABASE_NAME);
        }

        internal void AddVolunteerToDB(Volunteer volunteer)
        {
            IMongoCollection<Volunteer> volunteercollection = dBContext.Database.GetCollection<Volunteer>("Volunteers");
            modifiedDocumentManager.AddIDtoString(volunteer._id);
            volunteercollection.InsertOne(volunteer);
        }

        internal Volunteer GetOneVolunteer(string id)
        {
            IMongoCollection<Volunteer> volunteercollection = dBContext.Database.GetCollection<Volunteer>("Volunteers");
            var filter = Builders<Volunteer>.Filter.Eq("_id", id);
            Volunteer volunteer = volunteercollection.Find(filter).FirstOrDefault();
            return volunteer;
        }

        internal List<Volunteer> GetListOfVolunteers()
        {
            IMongoCollection<Volunteer> volunteercollection = dBContext.Database.GetCollection<Volunteer>("Volunteers");
            List<Volunteer> volunteers = volunteercollection.AsQueryable().ToList();
            return volunteers;
        }

        internal void UpdateAVolunteer(Volunteer volunteertopdate, string id)
        {
            IMongoCollection<Volunteer> volunteercollection = dBContext.Database.GetCollection<Volunteer>("Volunteers");
            var filter = Builders<Volunteer>.Filter.Eq("_id", id);
            volunteertopdate._id = id;
            modifiedDocumentManager.AddIDtoString(volunteertopdate._id);
            volunteercollection.FindOneAndReplace(filter, volunteertopdate);
        }

        internal void DeleteAVolunteer(string id)
        {
            IMongoCollection<Volunteer> volunteercollection = dBContext.Database.GetCollection<Volunteer>("Volunteers");
            modifiedDocumentManager.AddIDtoDeletionString(id);
            volunteercollection.DeleteOne(Builders<Volunteer>.Filter.Eq("_id", id));
        }
    }
}