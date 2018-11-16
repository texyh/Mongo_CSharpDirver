using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Csharp_MongoDB.Entities;

namespace Csharp_MongoDB
{
    public static class seed
    {
        private static DbContext _context = new DbContext();

        public static async Task AddRentals()
        {
            await _context.Rentals.InsertManyAsync(Rentals);
        }

        private static List<Rental> Rentals => new List<Rental>()
        {
            new Rental
            {
                Id = Guid.NewGuid().ToString(),
                Address = new Address
                {
                    Country = "Nigeria",
                    State = "Lagos",
                    Street = "Adenekan"
                },

                Description = "A cool three storey building with nice finishing",
                NumberOfRooms = 3,
                Price = 750000,
                ZipCode = "01001"
            },
            new Rental
            {
                Id = Guid.NewGuid().ToString(),

                Address = new Address
                {
                    Country = "Nigeria",
                    State = "Lagos",
                    Street = "Adenekan"
                },

                Description = "A bungalow with furnished rooms and living area",
                NumberOfRooms = 3,
                Price = 70000,
                ZipCode = "01008"
            },

            new Rental
            {
                Id = Guid.NewGuid().ToString(),

                Address = new Address
                {
                    Country = "Nigeria",
                    State = "Lagos",
                    Street = "Adenekan"
                },

                Description = "A duplex with state fo the art finishing",
                NumberOfRooms = 3,
                Price = 1000000,
                ZipCode = "01010"
            }

        };
    }
}
