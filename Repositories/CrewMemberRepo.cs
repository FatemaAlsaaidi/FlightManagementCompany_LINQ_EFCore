using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models; // Ensure this namespace matches your CrewMember model's namespace

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class CrewMemberRepo
    {
        // database injection
        private readonly FlightDatabaseContext _context;
        public CrewMemberRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // Get all crew members
        public IEnumerable<CrewMember> GetAllCrewMembers()
        {
            return _context.CrewMembers;
        }

        // Get crew member by ID
        public CrewMember GetCrewMemberById(int id)
        {
            return _context.CrewMembers.FirstOrDefault(cm => cm.CrewId == id);
        }

        // Add a new crew member
        public void AddCrewMember(CrewMember crewMember)
        {
            if (crewMember == null)
            {
                throw new ArgumentNullException(nameof(crewMember), "Crew member cannot be null");
            }
            _context.CrewMembers.Add(crewMember);
            _context.SaveChanges();
        }

        // Update an existing crew member
        public void UpdateCrewMember(CrewMember crewMember)
        {
            
            _context.CrewMembers.Update(crewMember);
            _context.SaveChanges();
        }

        // Delete a crew member
        public void DeleteCrewMember(CrewMember crewMember)
        {
            
            _context.CrewMembers.Remove(crewMember);
            _context.SaveChanges();
        }

    }
}
