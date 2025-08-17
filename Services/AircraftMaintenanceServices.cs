using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models;
using FlightManagementCompany_LINQ_EFCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace FlightManagementCompany_LINQ_EFCore.Services
{
    public class AircraftMaintenanceServices
    {
        // injection repositry
        private readonly AircraftMaintenanceRepo _AircraftMaintenanceRepo;
        public AircraftMaintenanceServices(AircraftMaintenanceRepo aircraftMaintenanceRepo)
        {
            _AircraftMaintenanceRepo = aircraftMaintenanceRepo;
        }

        // 1. Get All AircraftMaintenance
        public void GetAllAircraftMaintenance()
        {
            if (_AircraftMaintenanceRepo == null)
            {
                throw new Exception("There is no AirCraft MAintence Yet");
                return;
            }
            else 
            {
                _AircraftMaintenanceRepo.GetAllAircrafMaintenances().ToList().ForEach(aircraftMaintenance =>
                {
                    aircraftMaintenance.MaintenanceId = aircraftMaintenance.MaintenanceId;
                    aircraftMaintenance.MaintenanceDate = aircraftMaintenance.MaintenanceDate;
                    aircraftMaintenance.Type = aircraftMaintenance.Type;
                    aircraftMaintenance.Aircraft.AircraftId = aircraftMaintenance.AircraftId;
                    aircraftMaintenance.Aircraft.Model = aircraftMaintenance.Aircraft.Model;
                    aircraftMaintenance.Note = aircraftMaintenance.Note;
                });
            }
        }

        // 2. Get AircraftMaintenance by AircraftID
        public void GetArtcraftMaintenanceByAircraftID(int AircraftID)
        {
            var AircraftAM = _AircraftMaintenanceRepo.GetMaintenancesByAircraftId(AircraftID);
            if (AircraftAM == null)
            {
                throw new Exception($"There is no Artcraft Maintenance for this {AircraftID} Aircraft ");
                return;

            }
            else
            {
                AircraftAM.ForEach(Am =>
                {
                    Am.AircraftId = AircraftID;
                    Am.MaintenanceId = Am.MaintenanceId;
                    Am.MaintenanceDate = Am.MaintenanceDate;
                    Am.Type = Am.Type;
                    Am.Note = Am.Note;

                });
            }            


        }

        // 3. DeleteAircraftMaintenance

        public void DeleteAircraftMaintenance(int  AMid) 
        {

            var AM = _AircraftMaintenanceRepo.GetAircraftMaintenanceById(AMid);
            if (AM == null)
            {
                throw new Exception($"There is no Artcraft Maintenance for this {AMid} Aircraft ");
                return;

            }
            else
            {
                _AircraftMaintenanceRepo.DeleteAircraftMaintenance(AM);
                Console.WriteLine("Delete Successfully");
            }

        }

        // Update AircraftMaintenance
        public void UpdateAircraftMaintenanceType(int maintenanceId, string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Need type of maintance", nameof(type));

            var am = _AircraftMaintenanceRepo.GetAircraftMaintenanceById(maintenanceId);
            if (am == null)
                throw new KeyNotFoundException($"There is no Aircraft Maintenance with {maintenanceId} ID.");

            am.Type = type.Trim(); //just change this attribute 

            _AircraftMaintenanceRepo.UpdateAircraftMaintenance(am); 
            Console.WriteLine("Update succesfully");
        }

    }
}
