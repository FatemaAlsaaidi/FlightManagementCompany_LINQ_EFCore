





namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface IFlightCrewRepo
    {
        void AddFlightCrew(Models.FlightCrew flightCrew);
        void DeleteFlightCrew(Models.FlightCrew flightCrew);
        IEnumerable<Models.FlightCrew> GetAllFlightCrews();
        Models.FlightCrew GetFlightCrewById(int CrewMemberId, int FlightId);
        List<Models.FlightCrew> GetFlightCrewsByCrewId(int crewId);
        List<Models.FlightCrew> GetFlightCrewsByFlightId(int flightId);
        void UpdateFlightCrew(Models.FlightCrew flightCrew);
    }
}