using System;
using System.Collections.Generic;

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




class Program
{
    static void Main(string[] args)
    {
        //
        // This Main is a smoke test: it walks through every public method on
        // Vehicle/FleetManager, plus the edge cases most likely to break,
        // so you can see the whole hierarchy actually working before demo day.
        // Delete or trim this once your team's menu system replaces it.

        Section("1. Initial fleet");
        FleetManager manager = new FleetManager();
        manager.DisplayFleet();

        Section("2. Constructor validation (encapsulation check)");
        TryCreate(() => new Truck(0, "ABC123GP", "Bad ID"));       // id <= 0
        TryCreate(() => new Van(10, "", "Blank Reg"));              // empty reg
        TryCreate(() => new Bike(11, "XYZ789GP", "   "));           // blank name
        TryCreate(() => new Truck(12, "NEW001GP", "Scania R"));     // valid — should succeed

        Section("3. AddVehicle + FindVehicle");
        manager.AddVehicle(new Truck(12, "NEW001GP", "Scania R"));
        Vehicle found = manager.FindVehicle(12);
        Console.WriteLine(found != null ? $"Found: {found.RegistrationNumber}" : "Not found (unexpected!)");
        Vehicle missing = manager.FindVehicle(999);
        Console.WriteLine(missing == null ? "Vehicle 999 correctly not found." : "Unexpected match!");

        Section("4. ITrackable — works on Truck/Van, correctly skipped on Bike");
        manager.UpdateVehicleLocation(1, "N1 Highway");             // Truck: should update
        manager.UpdateVehicleLocation(7, "Somewhere");              // Bike: silently does nothing (no ITrackable)
        manager.FindVehicle(1).DisplayInfo();                       // location should show "N1 Highway"
        manager.FindVehicle(7).DisplayInfo();                       // no Location line at all — Bike doesn't implement ITrackable

        Section("5. Mileage + maintenance detection (IMaintainable)");
        manager.AddVehicleMileage(1, 10500);  // pushes Truck 1 over its 10 000km interval
        manager.AddVehicleMileage(4, 15200);  // pushes Van 4 over its 15 000km interval
        manager.AddVehicleMileage(7, 200);    // nowhere near Bike's 5 000km interval
        manager.CheckMaintenance();           // should list Truck 1 and Van 4 only

        Section("6. Dispatch — polymorphism proof");
        manager.DispatchVehicle(1);   // FAILS: Truck overdue for maintenance (overridden rule)
        manager.ServiceVehicle(1);    // reset its mileage
        manager.DispatchVehicle(1);   // SUCCEEDS now that maintenance is done
        manager.DispatchVehicle(1);   // FAILS: already dispatched (base rule)
        manager.FindVehicle(1).ReturnToDepot();
        manager.DispatchVehicle(1);   // SUCCEEDS again after returning to depot

        Console.WriteLine();
        manager.FindVehicle(7).FuelLevel = 3;  // drain the bike below its 5% threshold
        manager.DispatchVehicle(7);            // FAILS: Bike's lower fuel threshold (overridden rule)
        manager.FindVehicle(7).Refuel();
        manager.DispatchVehicle(7);            // SUCCEEDS after refuelling

        Section("7. RemoveVehicle");
        bool removed = manager.RemoveVehicle(9);
        bool removedAgain = manager.RemoveVehicle(9); // already gone
        Console.WriteLine($"First removal of vehicle 9: {removed}");
        Console.WriteLine($"Second removal of vehicle 9: {removedAgain}");

        Section("8. Final fleet state");
        manager.DisplayFleet();
    }

    // Small helper so constructor-validation failures print cleanly instead
    // of crashing the whole smoke test.
    static void TryCreate(Func<Vehicle> factory)
    {
        try
        {
            Vehicle v = factory();
            Console.WriteLine($"Created OK: {v.RegistrationNumber}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Rejected as expected: {ex.Message}");
        }
    }

    static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
    }
    //

}
