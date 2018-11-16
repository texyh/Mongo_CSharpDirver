using System;
using System.Collections.Generic;
using System.Text;
using Csharp_MongoDB.Entities;

namespace Csharp_MongoDB
{
    public class RentalZipCode : Rental
    {
        public ZipCode[] ZipCodes { get; set; }
    }
}
