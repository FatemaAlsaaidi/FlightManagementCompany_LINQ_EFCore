using Microsoft.EntityFrameworkCore;
using FlightManagementCompany_LINQ_EFCore.SeedData;


namespace FlightManagementCompany_LINQ_EFCore
{
    internal class Program
    {
        static async Task Main()
        {
            using var db = new FlightDatabaseContext();

            // Make sure schema is up to date
            await db.Database.MigrateAsync();

            // Seed
            await FlightManagementCompany_LINQ_EFCore.SeedData.SeedData.SeedAsync(db);

            Console.WriteLine("Seed done ");
        }
    }
}
