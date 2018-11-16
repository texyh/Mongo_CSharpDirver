using System;
using System.Collections.Generic;
using System.Text;

namespace Csharp_MongoDB
{
    public class RentalViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int NumberOfRooms { get; set; }
        
        public decimal Price { get; set; }
    }
}
