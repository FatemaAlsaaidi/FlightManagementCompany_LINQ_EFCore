using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class AircraftRepo
    {
        // inject the DbContext
        private readonly FlightDatabaseContext _context;
        public AircraftRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // 1. GetAll()
        public List<Aircraft> GetAll()
        {
            return _context.Aircraft.ToList();
        }

        // 2. GetById()
        public Aircraft GetById(int id)
        {
            return _context.Aircraft.Find(id);
        }

        // 3. Add()
        public void Add(Aircraft aircraft)
        {
            _context.Aircraft.Add(aircraft);
            _context.SaveChanges();
        }

        // 4. Update()
        public void Update(Aircraft aircraft)
        {
            _context.Aircraft.Update(aircraft);
            _context.SaveChanges();

        }
        // 5. Delete()
        public void Delete(Aircraft aircraft)
        {
            _context.Aircraft.Remove(aircraft);
            _context.SaveChanges();
        }


    }
}
