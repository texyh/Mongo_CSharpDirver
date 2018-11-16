using System;
using System.Collections.Generic;
using System.Text;
using Csharp_MongoDB.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.GridFS;

namespace Csharp_MongoDB
{
    public class DbContext
    {
        private  readonly IMongoDatabase _database;
        public DbContext()
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl("mongodb://localhost"));
            settings.ClusterConfigurator = builder => builder.Subscribe<CommandStartedEvent>(e =>
            {
                //Console.WriteLine(e);
            });
            var client = new MongoClient(settings);
            _database = client.GetDatabase("RentalDB");
        }

        public IMongoCollection<Rental> Rentals => _database.GetCollection<Rental>("rentals");

        public IMongoCollection<ZipCode> Zips => _database.GetCollection<ZipCode>("zips");

        public GridFSBucket RentalBucket => new GridFSBucket(_database, new GridFSBucketOptions
        {
            BucketName = "rentalBucket"
        });

        public BsonDocument BuildInfo => _database.RunCommand<BsonDocument>(new BsonDocument("buildinfo", 1));
    }
}
