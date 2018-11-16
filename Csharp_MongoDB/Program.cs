using System;
using System.IO;
using System.Reflection;
using Csharp_MongoDB.Entities;
using MongoDB.Bson;

namespace Csharp_MongoDB
{
    class Program
    {
        static void Main(string[] args)
        {
            //seed.AddRentals().GetAwaiter().GetResult();
            var repository = new RentalRepository();
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "rental01.jpg");
            var file = System.IO.File.ReadAllBytes(path);
            //Console.WriteLine(repository.GetAll().GetAwaiter().GetResult().ToJson());
            Console.WriteLine(repository.UploadFile(file).GetAwaiter().GetResult().ToJson());

            Console.Read();
        }
    }
}
 