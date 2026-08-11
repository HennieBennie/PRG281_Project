using System;
using System.Collections.Generic;
//COPY AND PASTE YOUR CODE INTO YOUR OWN PROJECT ON VISUAL STUDIO, OTHERWISE IT WILL NOT RUN

public abstract class Vehicle
{
    public int VehicleID { get; set; }
    public string RegistrationNumber { get; set; }
    public string VehicleName { get; set; }
    public string VehicleType { get; protected set; }
    public bool IsAvailable { get; private set; } = true;
    private int fuelLevel { get; set; }

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
        }
    }
    //
    protected Vehicle( int id, string regNo, string name)
    {
        if (id <= 0)
            throw new ArgumentException("Vehicle ID must be positive.", nameof(id));
        if (string.IsNullOrWhiteSpace(regNo))
            throw new ArgumentException("Registration number is required.", nameof(regNo));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vehicle name is required.", nameof(name));
        //

        VehicleID = id;
        RegistrationNumber = regNo;
        VehicleName = name;
        IsAvailable = true;
        FuelLevel = 100;
    }
    public abstract void DisplayInfo();

   

    public virtual void Refuel()
    {
        FuelLevel = 100;
    }
    //
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
            throw new InvalidOperationException($"{RegistrationNumber} is already dispatched.");    
        }
        if (!CanDispatch(out string reason))
        {
            throw new InvalidOperationException($"{RegistrationNumber} cannot be dispatched: {reason}");
        }

        IsAvailable = false;
    }
    public void ReturnToDepot()
    {
        IsAvailable = true;
    }//
}
public interface ITrackable
{
    string CurrentLocation{ get; set; }
    void UpdateLocation(string location);

}
public interface IMaintainable
{
    int MileageSinceService { get; set; }
    int ServiceIntervalKm {  get; set; }
    bool NeedsMaintenance();
    void ServiceVehicles();
    void AddMileage(int kilometres);
}

public class Truck : Vehicle, ITrackable, IMaintainable
{

    public Truck(int id, string regNo, string name) : base(id, regNo, name)
    {
        VehicleType = "Truck";
        ServiceIntervalKm = 10000;  // 
        MileageSinceService = 0;
    }

    public string CurrentLocation{ get; set; } = "Depot";
    public int MileageSinceService { get; set; }
    public int ServiceIntervalKm { get; set; }
    public void UpdateLocation(string location)
    {
        CurrentLocation = location;
    }
    public bool NeedsMaintenance()
    {
        return MileageSinceService >= ServiceIntervalKm;
    }
    public void ServiceVehicles()
    {
        MileageSinceService = 0;
    }
    public void AddMileage(int kilometres)
    {
        if (kilometres > 0)
        {
            MileageSinceService += kilometres;
        }
    }
    //
    protected override bool CanDispatch(out string reason)
    {
        if (!base.CanDispatch(out reason))
            return false;

        if (NeedsMaintenance())
        {
            reason = "overdue for maintenance";
            return false;
        }
        return true;
    }
    //
    public override void DisplayInfo()
    {
        Console.WriteLine($"Truck: {VehicleName}");
        Console.WriteLine($"Registration: {RegistrationNumber}");
        Console.WriteLine($"Fuel: {FuelLevel}%");
        Console.WriteLine($"Location: {CurrentLocation}");
        Console.WriteLine($"Available: {IsAvailable}");
    }
}
public class Van : Vehicle, IMaintainable, ITrackable
{
   
    
    public Van(int id, string regNo, string name) : base(id, regNo, name)
    {
        VehicleType = "Van";
        ServiceIntervalKm = 15000;  // 
        MileageSinceService = 0;
    }

    public string CurrentLocation { get; set; } = "Depot";
    public int MileageSinceService { get; set; }
    public int ServiceIntervalKm { get; set; }
    public void UpdateLocation(string location)
    {
        CurrentLocation = location;
    }
    public bool NeedsMaintenance()
    {
        return MileageSinceService >= ServiceIntervalKm;
    }
    public void ServiceVehicles()
    {
        MileageSinceService = 0;
    }
    public void AddMileage(int kilometres)
    {
        if (kilometres > 0)
        {
            MileageSinceService += kilometres;
        }
    }
    public override void DisplayInfo()
    {
        Console.WriteLine($"Van: {VehicleName}");
        Console.WriteLine($"Registration: {RegistrationNumber}");
        Console.WriteLine($"Fuel: {FuelLevel}%");
        Console.WriteLine($"Location: {CurrentLocation}");
    }
}
public class Bike : Vehicle, IMaintainable//
{
    public Bike(int id, string regNo, string name) : base(id, regNo, name)
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
    }
    public void AddMileage(int kilometres)
    {
        if (kilometres > 0)
        {
            MileageSinceService += kilometres;
        }
    }
    //
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
    //
    public override void DisplayInfo()
    {
        Console.WriteLine($"Bike: {VehicleName}");
        Console.WriteLine($"Registration: {RegistrationNumber}");
        Console.WriteLine($"Fuel: {FuelLevel}%");
        
    }
}



    public class FleetManager
    {
        private List<Vehicle> fleet = new List<Vehicle>();
        public FleetManager()
        {
            fleet.Add(new Truck(1, "CDS791MP", "Volvo FL"));
            fleet.Add(new Truck(2, "CF10VDGP", "Nissan UD"));
            fleet.Add(new Truck(3, "BTN020GP", "MAN TGS"));
            fleet.Add(new Van(4, "BB12CDZN", "Volkswagen Caddy"));
            fleet.Add(new Van(5, "BR30RJGP", "Ford Transit"));
            fleet.Add(new Van(6, "HDJ392GP", "Peugot Partner"));
            fleet.Add(new Bike(7, "JC72YMGP", "Vespa"));
            fleet.Add(new Bike(8, "HBP713FS", "Velocity 150"));
            fleet.Add(new Bike(9, "KVD496MP", "Pacer 200"));
        }
        public void AddVehicle(Vehicle vehicle)
        {
            fleet.Add(vehicle);
        }

        public void DisplayFleet()
        {
            Console.WriteLine("Current fleet:");

            foreach (Vehicle vehicle in fleet)
            {
                vehicle.DisplayInfo();
                Console.WriteLine("-------------------------");
            }
        }
        public Vehicle FindVehicle(int vehicleID)
        {
            foreach (Vehicle vehicle in fleet)
            {
                if (vehicle.VehicleID == vehicleID)
                {
                    return vehicle;
                }
            }

            return null;
        }
        public bool RemoveVehicle(int vehicleID)
        {
            Vehicle vehicle = FindVehicle(vehicleID);

            if (vehicle == null)
            {
                return false;
            }

            fleet.Remove(vehicle);
            return true;
        }
        public void UpdateVehicleLocation(int vehicleID, string location)
        {
            Vehicle vehicle = FindVehicle(vehicleID);

            if (vehicle is ITrackable trackable)
            {
                trackable.UpdateLocation(location);
            }
        }
        public void CheckMaintenance()
        {
            foreach (Vehicle vehicle in fleet)
            {
                if (vehicle is IMaintainable maintainable)
                {
                    if (maintainable.NeedsMaintenance())
                    {
                        Console.WriteLine(
                            $"{vehicle.RegistrationNumber} requires maintenance!");
                    }
                }
            }
        }
        public void ServiceVehicle(int vehicleID)
        {
            Vehicle vehicle = FindVehicle(vehicleID);

            if (vehicle is IMaintainable maintainable)
            {
                maintainable.ServiceVehicles();
                Console.WriteLine(
                    $"{vehicle.RegistrationNumber} has been serviced.");
            }
        }
        public void AddVehicleMileage(int vehicleID, int kilometres)
        {
                Vehicle vehicle = FindVehicle(vehicleID);

                if (vehicle is IMaintainable maintainable)
                {
                    maintainable.AddMileage(kilometres);
                }
        }
    //
        public void DispatchVehicle(int vehicleID)
        {
            Vehicle vehicle = FindVehicle(vehicleID);
            if (vehicle == null)
            {
            Console.WriteLine("Vehicle not found.");
            return;
            }

            try
            {
            vehicle.Dispatch();
            Console.WriteLine($"{vehicle.RegistrationNumber} dispatched.");
            }
            catch (InvalidOperationException ex)
            {
                //Enter error message here
                Console.WriteLine($"Dispatch failed: {ex.Message}");
            }
        //
    }
}
