//COPY AND PASTE CODE INTO OWN PROJECT
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
