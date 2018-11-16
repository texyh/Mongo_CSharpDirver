using System;
using System.Collections.Generic;
using System.Text;

namespace Csharp_MongoDB.Entities
{
    public class Rental
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int NumberOfRooms { get; set; }

        public Address Address { get; set; }

        public int Price { get; set; }

        public string ZipCode { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }

        public string State { get; set; }
        
        public string Country { get; set; }

        public override string ToString()
        {
            return $"{Street}, {State}, {Country}";
        }
    }
}
