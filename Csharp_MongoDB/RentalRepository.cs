using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csharp_MongoDB.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace Csharp_MongoDB
{
    public class RentalRepository
    {
        private readonly DbContext _dbContext;

        public RentalRepository()
        {
            _dbContext = new DbContext();
        }

        public async Task<IEnumerable<Rental>> GetAll()
        {
            return await _dbContext.Rentals.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<IEnumerable<Rental>> Search(RentalSearchCriteria criteria)
        {
            var filterDefinition = Builders<Rental>.Filter.Empty;

            if (criteria.NumberOfRooms.HasValue)
            {
                filterDefinition = Builders<Rental>.Filter
                    .Where(x => x.NumberOfRooms >= criteria.NumberOfRooms.Value);
            }

            if (criteria.Price.HasValue)
            {
                filterDefinition &= Builders<Rental>.Filter.Where(x => x.Price >= criteria.Price.Value);
            }

            //return await _dbContext.Rentals.Find(new BsonDocument
            //{
            //    {property, value }
            //}).ToListAsync();
            var rental = await _dbContext.Rentals
                .Find(filterDefinition)
                //.Sort(Builders<Rental>.Sort.Ascending(x => x.Price))
                .SortBy(x => x.Price)
                .ThenByDescending(r => r.NumberOfRooms)
                .ToListAsync();

            return rental;
        }

        public async Task Create(Rental rental)
        {
            await _dbContext.Rentals.InsertOneAsync(rental);
        }

        public async Task<Rental> Get(string id)
        {
            return await _dbContext.Rentals
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task Replace(Rental rental)
        {
            await _dbContext.Rentals.ReplaceOneAsync(x => x.Id == rental.Id, rental);
        }

        public async Task Update(Rental rental)
        {
            _dbContext.Rentals
                .UpdateOne(x => x.Id == rental.Id,
                    Builders<Rental>.Update.Set(x => x.Price, rental.Price)
                        .Set(x => x.NumberOfRooms, rental.NumberOfRooms));
        }

        public async Task Upsert(Rental rental)
        {
            var options = new UpdateOptions
            {
                IsUpsert = true
            };

            await _dbContext.Rentals.ReplaceOneAsync(r => r.Id == rental.Id, rental, options);
        }

        public async Task Delete(string id)
        {
           await _dbContext.Rentals.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<IList<RentalViewModel>> GetSummary()
        {
            return await _dbContext.Rentals
                .Find(new BsonDocument())
                .Project(x => new RentalViewModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    NumberOfRooms = x.NumberOfRooms,
                    Price = x.Price
                }).ToListAsync();
        }

        public async Task<IList<RentalViewModel>> SearchByLinq()
        {
            return  await _dbContext.Rentals
                .AsQueryable()
                .Select(x => new RentalViewModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    NumberOfRooms = x.NumberOfRooms,
                    Price = x.Price

                }).OrderBy(x => x.Price)
                .ThenByDescending(x => x.NumberOfRooms)
                .ToListAsync();
        }

        public async Task<IEnumerable> GetDistribution()
        {
            var dist = await _dbContext.Rentals.Aggregate()
                .Project(x => new {x.Price, PriceRange = (double)x.Price - ((double)x.Price % 500)})
                .Group(x => x.PriceRange, g => new {g.Key, Count = g.Count()})
                .SortByDescending(x => x.Key)
                .ToListAsync();

            return dist;
        }

        public async Task<IEnumerable> GetDistributionByLinq()
        {
            var dist = await _dbContext.Rentals.AsQueryable()
                .Select(x => new { x.Price, PriceRange = (double)x.Price - ((double)x.Price % 500) })
                .GroupBy(x => x.PriceRange)
                .Select(x => new {x.Key, Count = x.Count()})
                .OrderByDescending(x => x.Key)
                .ToListAsync();

            return dist;
        }

        public async Task<IEnumerable> GetRentalWithZipCodeWithoutLookup()
        {
            var rentals = await _dbContext.Rentals.Find(new BsonDocument()).ToListAsync();
            var zipIds = rentals.Select(x => x.ZipCode).Distinct().ToList();

            var zips = _dbContext.Zips.Find(x => zipIds.Contains(x.Id))
                .ToList()
                .ToDictionary(x => x.Id);

            return rentals.Select(x => new
            {
                Rental = rentals,
                ZipCode = x.ZipCode != null ? zips.FirstOrDefault(z => z.Key == x.ZipCode).Value : null
            });
        }

        public async Task<IEnumerable<RentalZipCode>> GetRentalWithZipCodeWithLookup()
        {
            var rentals = await _dbContext.Rentals
                .Aggregate()
                .Lookup<Rental, ZipCode, RentalZipCode>(_dbContext.Zips, r => r.ZipCode, z => z.Id, d => d.ZipCodes)
                .ToListAsync();

            return rentals;
        }

        public async Task<ObjectId> UploadFile(byte[] file)
        {
            var optios = new GridFSUploadOptions
            {
                Metadata = new BsonDocument("contentType", "jpg")
            };
            return await _dbContext.RentalBucket.UploadFromBytesAsync("rental", file, optios);
        }
    }
}
