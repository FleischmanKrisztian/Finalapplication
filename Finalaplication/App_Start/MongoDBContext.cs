﻿using Finalaplication.Common;
using Finalaplication.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Finalaplication.App_Start
{
    public class MongoDBContext
    {
        public bool nointernet = false;
        public IMongoDatabase database;
        private MongoDBContextOffline dbcontextoffline;
        private IMongoCollection<Settings> settingcollection;

        /// <summary>
        /// Creates a new Mongo client and returns the connection.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="dbName"></param>
        /// <param name="portNum"></param>
        /// <returns></returns>
        private IMongoDatabase getDatabaseForAddressDbNameAndPort(string address, string dbName, int portNum)
        {
            MongoClient mongoClient = null;

            if (portNum == 0)
            {
                MongoUrl onlineUrl = new MongoUrl(address);
                mongoClient = new MongoClient(onlineUrl);
            }
            else
            {
                var clientSettings = new MongoClientSettings
                {
                    Server = new MongoServerAddress(address),
                    ClusterConfigurator = builder =>
                    {
                        builder.ConfigureCluster(settings => settings.With(serverSelectionTimeout: TimeSpan.FromSeconds(2)));
                    }
                };
                mongoClient = new MongoClient(clientSettings);
            }

            return mongoClient.GetDatabase(dbName);
        }

        /// <summary>
        /// Creates a MongoDb Client and retrieves the Database.
        ///
        /// </summary>
        /// <param name="envVarNameServer"></param>
        /// <param name="envVarDbName"></param>
        /// <param name="envVarNamePort"></param>
        /// <returns></returns>
        private IMongoDatabase getDatabaseForEnvironmentVars(
            string envVarNameServer,
            string envVarDbName,
            string envVarNamePort)
        {
            // Offline mode considered secondary
            //string envServerAddress = "172.17.0.2";
            //string envDatabaseName = "BucuriaDaruluiOffline";
            //int numServerPort = 27017;
            string envServerAddress = Environment.GetEnvironmentVariable(envVarNameServer);
            string envServerPort = Environment.GetEnvironmentVariable(envVarNamePort);
            int numServerPort = Convert.ToInt32(envServerPort);
            string envDatabaseName = Environment.GetEnvironmentVariable(envVarDbName);

            return getDatabaseForAddressDbNameAndPort(envServerAddress, envDatabaseName, numServerPort);
        }

        public MongoDBContext()
        {
            try
            {
                // TODO (Augustin Preda, 2019-10-23): is this still required (separate offline context?)
                dbcontextoffline = new MongoDBContextOffline();
                settingcollection = dbcontextoffline.databaseoffline.GetCollection<Settings>("Settings");
                var totalCount = settingcollection.CountDocuments(new BsonDocument());

                //In case there is no settings saved it initializes one with these default values.
                if (totalCount == 0)
                {
                    Settings sett = new Settings
                    {
                        Env = VolMongoConstants.CONNECTION_MODE_OFFLINE,
                        Lang = "English",
                        Quantity = 15
                    };
                    settingcollection.InsertOne(sett);
                    nointernet = true;
                }

                Settings set = settingcollection.AsQueryable<Settings>().SingleOrDefault();

                bool useOnline = (set.Env == VolMongoConstants.CONNECTION_MODE_ONLINE);
                try
                {
                    if (useOnline)
                    {
                        database = getDatabaseForEnvironmentVars(
                            VolMongoConstants.SERVER_NAME_MAIN,
                            VolMongoConstants.DATABASE_NAME_MAIN,
                            "");
                    }
                    else
                    {
                        // Offline mode considered secondary
                        database = getDatabaseForEnvironmentVars(
                            VolMongoConstants.SERVER_NAME_SECONDARY,
                            VolMongoConstants.DATABASE_NAME_SECONDARY,
                            VolMongoConstants.SERVER_PORT_SECONDARY);
                            nointernet = true;
                    }
                }
                catch (Exception)
                {
                    //In case there is no internet it changes the environment to offline so it will not try to connect a second time.
                    set.Env = VolMongoConstants.CONNECTION_MODE_OFFLINE;
                    settingcollection.ReplaceOne(y => y.Env.Contains("i"), set);
                    var client = new MongoClient();
                    database = client.GetDatabase("BucuriaDaruluiOffline");
                    nointernet = true; 
                }
            }
            catch
            {
                // Write online/offline setting
                Settings sett = new Settings();
                try
                {
                    Settings set = settingcollection.AsQueryable().FirstOrDefault();
                    sett.settingID = set.settingID;
                    sett.Env = VolMongoConstants.CONNECTION_MODE_OFFLINE;
                    sett.Lang = set.Lang;
                    sett.Quantity = set.Quantity;
                    settingcollection.ReplaceOne(y => y.Env.Contains("i"), sett);
                    var client = new MongoClient();
                    database = client.GetDatabase("BucuriaDaruluiOffline");
                }
                catch
                {
                }
            }
        }
    }
}
