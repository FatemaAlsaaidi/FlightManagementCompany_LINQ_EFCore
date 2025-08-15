using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface ICrewMemberRepo
    {
        void AddCrewMember(CrewMember crewMember);
        void DeleteCrewMember(CrewMember crewMember);
        IEnumerable<CrewMember> GetAllCrewMembers();
        IEnumerable<CrewMember> GetAvailableCrew(DateTime dep);
        IEnumerable<CrewMember> GetCrewByRole(string role);
        CrewMember GetCrewMemberById(int id);
        void UpdateCrewMember(CrewMember crewMember);
    }
}