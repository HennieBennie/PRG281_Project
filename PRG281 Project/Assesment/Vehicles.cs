using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assesment
{
    public class VehicleEventArgs : EventArgs
    {
        public Vehicle Vehicle { get; }
        public string Message { get; }

        public VehicleEventArgs(Vehicle vehicle, string message)
        {
            Vehicle = vehicle;
            Message = message;
        }
    }

    public abstract class Vehicle
    {
        //
        public int VehicleID { get; private set; }

       
        public string RegistrationNumber { get; private set; }

        public string VehicleName { get; private set; }
        //

        public string VehicleType { get; protected set; }

        public bool IsAvailable { get; private set; } = true;

        private int fuelLevel { get; set; }

        private bool fuelLowAlertRaised = false;
        public int FuelLevel
        {
            get => fuelLevel;

            set
            {
                if (value < 0)
                {
                    fuelLevel = 0;
                }
                else if (value > 100)
                {
                    fuelLevel = 100;
                }
                else
                {
                    fuelLevel = value;
                }

                if (fuelLevel < 20 && !fuelLowAlertRaised)
                {
                    fuelLowAlertRaised = true;
                    OnFuelLow();
                }
                else if (fuelLevel >= 20)
                {
                    fuelLowAlertRaised = false;
                }
            }
        }

        public event EventHandler<VehicleEventArgs> FuelLow;
        public event EventHandler<VehicleEventArgs> StatusChanged;

        protected virtual void OnFuelLow()
        {
            FuelLow?.Invoke(this, new VehicleEventArgs(this, $"{RegistrationNumber} fuel is low ({FuelLevel}%)."));
        }

        protected virtual void OnStatusChanged(string message)
        {
            StatusChanged?.Invoke(this, new VehicleEventArgs(this, message));
        }

        // ========================================================
        // CONSTRUCTOR
        // ========================================================

        protected Vehicle(int id, string regNo, string name)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Vehicle ID must be positive.",
                    nameof(id));
            }

            if (string.IsNullOrWhiteSpace(regNo))
            {
                throw new ArgumentException(
                    "Registration number is required.",
                    nameof(regNo));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Vehicle name is required.",
                    nameof(name));
            }

            VehicleID = id;
            RegistrationNumber = regNo.Trim();
            VehicleName = name.Trim();

            IsAvailable = true;
            FuelLevel = 100;
        }


        // ========================================================
        // ABSTRACTION
        // ========================================================

        public abstract void DisplayInfo();


        // ========================================================
        // REFUELLING
        // ========================================================

        public virtual void Refuel()
        {
            FuelLevel = 100;
        }
        //========================================================
        //UPDATE REGISTRATION
        //========================================================

        //
        public void UpdateRegistration(string registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
                throw new ArgumentException("Registration number is required.", nameof(registrationNumber));

            RegistrationNumber = registrationNumber.Trim();
        }

        //========================================================
        //UPDATE NAME
        //========================================================

        //////
        public void UpdateName(string vehicleName)
        {
            if (string.IsNullOrWhiteSpace(vehicleName))
                throw new ArgumentException("Vehicle name is required.", nameof(vehicleName));

            VehicleName = vehicleName.Trim();
        }
        ////////
        // ========================================================
        // DISPATCH RULE
        // ========================================================

        protected virtual bool CanDispatch(out string reason)
        {
            if (FuelLevel < 20)
            {
                reason = "insufficient fuel";
                return false;
            }

            reason = string.Empty;
            return true;
        }


        public void Dispatch()
        {
            if (!IsAvailable)
            {
                throw new InvalidOperationException(
                    $"{RegistrationNumber} is already dispatched.");
            }

            if (!CanDispatch(out string reason))
            {
                throw new InvalidOperationException(
                    $"{RegistrationNumber} cannot be dispatched: {reason}");
            }

            IsAvailable = false;
            OnStatusChanged($"{RegistrationNumber} has been dispatched.");
        }


        public void ReturnToDepot()
        {
            IsAvailable = true;
            OnStatusChanged($"{RegistrationNumber} has returned to the depot.");
        }
    }


    // ============================================================
    // INTERFACES
    // ============================================================

    public interface ITrackable
    {
        string CurrentLocation { get; set; }

        void UpdateLocation(string location);
    }


    public interface IMaintainable
    {
        int MileageSinceService { get; set; }

        int ServiceIntervalKm { get; set; }

        bool NeedsMaintenance();

        void ServiceVehicles();

        void AddMileage(int kilometres);

        event EventHandler<VehicleEventArgs> MaintenanceRequired;
    }


    // ============================================================
    // TRUCK
    // ============================================================

    public class Truck : Vehicle, ITrackable, IMaintainable
    {

        public event EventHandler<VehicleEventArgs> MaintenanceRequired;   
        private bool maintenanceAlertRaised = false;

        public Truck(
            int id,
            string regNo,
            string name)
            : base(id, regNo, name)
        {
            VehicleType = "Truck";

            ServiceIntervalKm = 10000;

            MileageSinceService = 0;
        }


        public string CurrentLocation { get; set; } = "Depot";

        public int MileageSinceService { get; set; }

        public int ServiceIntervalKm { get; set; }


        public void UpdateLocation(string location)
        {
            if (!string.IsNullOrWhiteSpace(location))
            {
                CurrentLocation = location.Trim();
            }
        }


        public bool NeedsMaintenance()
        {
            return MileageSinceService >= ServiceIntervalKm;
        }


        public void ServiceVehicles()
        {
            MileageSinceService = 0;
            maintenanceAlertRaised = false;
        }


        public void AddMileage(int kilometres)
        {
            if (kilometres > 0)
            {
                MileageSinceService += kilometres;

                if (NeedsMaintenance() && !maintenanceAlertRaised)   
                {
                    maintenanceAlertRaised = true;
                    MaintenanceRequired?.Invoke(this, new VehicleEventArgs(this,
                        $"{RegistrationNumber} is now overdue for maintenance."));
                }
            }
        }


        // ========================================================
        // POLYMORPHISM
        // ========================================================

        protected override bool CanDispatch(out string reason)
        {
            if (!base.CanDispatch(out reason))
            {
                return false;
            }

            if (NeedsMaintenance())
            {
                reason = "overdue for maintenance";
                return false;
            }

            return true;
        }


        public override void DisplayInfo()
        {
            Console.WriteLine($"Truck: {VehicleName}");
            Console.WriteLine(
                $"Registration: {RegistrationNumber}");
            Console.WriteLine($"Fuel: {FuelLevel}%");
            Console.WriteLine($"Location: {CurrentLocation}");
            Console.WriteLine($"Available: {IsAvailable}");
            Console.WriteLine(
                $"Mileage Since Service: {MileageSinceService} km");
            Console.WriteLine(
                $"Service Interval: {ServiceIntervalKm} km");
        }
    }


    // ============================================================
    // VAN
    // ============================================================

    public class Van : Vehicle, IMaintainable, ITrackable
    {
        public event EventHandler<VehicleEventArgs> MaintenanceRequired;   // <- added
        private bool maintenanceAlertRaised = false;

        public Van(
            int id,
            string regNo,
            string name)
            : base(id, regNo, name)
        {
            VehicleType = "Van";

            ServiceIntervalKm = 15000;

            MileageSinceService = 0;
        }


        public string CurrentLocation { get; set; } = "Depot";

        public int MileageSinceService { get; set; }

        public int ServiceIntervalKm { get; set; }


        public void UpdateLocation(string location)
        {
            if (!string.IsNullOrWhiteSpace(location))
            {
                CurrentLocation = location.Trim();
            }
        }


        public bool NeedsMaintenance()
        {
            return MileageSinceService >= ServiceIntervalKm;
        }


        public void ServiceVehicles()
        {
            MileageSinceService = 0;
            maintenanceAlertRaised = false;
        }


        public void AddMileage(int kilometres)
        {
            if (kilometres > 0)
            {
                MileageSinceService += kilometres;

                if (NeedsMaintenance() && !maintenanceAlertRaised)   // <- added block
                {
                    maintenanceAlertRaised = true;
                    MaintenanceRequired?.Invoke(this, new VehicleEventArgs(this,
                        $"{RegistrationNumber} is now overdue for maintenance."));
                }
            }
        }


        public override void DisplayInfo()
        {
            Console.WriteLine($"Van: {VehicleName}");
            Console.WriteLine(
                $"Registration: {RegistrationNumber}");
            Console.WriteLine($"Fuel: {FuelLevel}%");
            Console.WriteLine($"Location: {CurrentLocation}");
            Console.WriteLine($"Available: {IsAvailable}");
            Console.WriteLine(
                $"Mileage Since Service: {MileageSinceService} km");
            Console.WriteLine(
                $"Service Interval: {ServiceIntervalKm} km");
        }
    }


    // ============================================================
    // BIKE
    // ============================================================

    public class Bike : Vehicle, IMaintainable
    {

        public event EventHandler<VehicleEventArgs> MaintenanceRequired;   
        private bool maintenanceAlertRaised = false;

        public Bike(
            int id,
            string regNo,
            string name)
            : base(id, regNo, name)
        {
            VehicleType = "Bike";

            ServiceIntervalKm = 5000;

            MileageSinceService = 0;
        }


        public int MileageSinceService { get; set; }

        public int ServiceIntervalKm { get; set; }


        public bool NeedsMaintenance()
        {
            return MileageSinceService >= ServiceIntervalKm;
        }


        public void ServiceVehicles()
        {
            MileageSinceService = 0;
            maintenanceAlertRaised = false;
        }


        public void AddMileage(int kilometres)
        {
            if (kilometres > 0)
            {
                MileageSinceService += kilometres;

                if (NeedsMaintenance() && !maintenanceAlertRaised)   
                {
                    maintenanceAlertRaised = true;
                    MaintenanceRequired?.Invoke(this, new VehicleEventArgs(this,
                        $"{RegistrationNumber} is now overdue for maintenance."));
                }
            }
        }


        // ========================================================
        // BIKE-SPECIFIC DISPATCH RULE
        // ========================================================

        protected override bool CanDispatch(out string reason)
        {
            if (FuelLevel < 5)
            {
                reason = "insufficient fuel";
                return false;
            }

            reason = string.Empty;
            return true;
        }


        public override void DisplayInfo()
        {
            Console.WriteLine($"Bike: {VehicleName}");
            Console.WriteLine(
                $"Registration: {RegistrationNumber}");
            Console.WriteLine($"Fuel: {FuelLevel}%");
            Console.WriteLine(
                $"Mileage Since Service: {MileageSinceService} km");
            Console.WriteLine(
                $"Service Interval: {ServiceIntervalKm} km");
            Console.WriteLine($"Available: {IsAvailable}");
        }
    }


}