# PRG281_Project
# PRG281_Project — Smart Fleet Manager

A console-based C# fleet management system built for PRG281. It simulates a small delivery fleet — trucks, vans, bikes — with dispatching, fuel and mileage tracking, maintenance alerts, and a background thread that keeps watching the fleet while you're doing other things in the menu.

## What it does

- Add, view, edit and remove vehicles (trucks, vans, bikes)
- Dispatch a vehicle and bring it back to the depot
- Refuel one vehicle, or refuel the whole fleet at once (this one runs every refuel as a separate Task in parallel, not a loop)
- Track mileage and flag vehicles overdue for a service
- Update a truck or van's location — bikes don't have GPS fitted, so that option isn't available for them
- Search the fleet by vehicle type, or list only the vehicles that are currently available
- Run a background monitor thread that periodically checks for low fuel and overdue maintenance and logs whatever it finds

## Project structure

```
PRG281 Project/Assesment/
├── Vehicles.cs       Vehicle base class, Truck/Van/Bike, ITrackable, IMaintainable
├── FleetManager.cs   Owns the fleet list; all CRUD, dispatch, fuel and maintenance
│                      logic; custom exceptions; the background monitor thread
├── ConsoleInput.cs   Input helpers — read int, read string, validate, pause
├── Program.cs        The menu loop and everything that talks to the console
└── Assesment.csproj  .NET Framework 4.7.2 console project
```

## OOP concepts, and where to find them

`Vehicle` is an abstract base class. It defines `DisplayInfo()` without implementing it, and leaves each subclass to decide what's actually worth printing for that vehicle type — that's the abstraction. `Truck`, `Van` and `Bike` all inherit from it and get dispatch, refuelling and fuel-tracking for free (inheritance), but each one overrides `CanDispatch()` with its own rule: a truck refuses to go out if it's overdue for a service, and a bike uses a lower fuel threshold than a truck or van because it burns so much less fuel per trip. That's polymorphism doing actual work, not just existing for the sake of it.

Two interfaces split up the optional stuff. `ITrackable` covers location updates, and `IMaintainable` covers mileage and servicing. Trucks and vans implement both; bikes only implement `IMaintainable`, since there's no tracker fitted to them. Try to update a bike's location through the menu and `FleetManager` throws an `InvalidVehicleOperationException` instead of pretending it worked.

Events tie the whole thing together. Every `Vehicle` can raise `FuelLow` and `StatusChanged`, and anything implementing `IMaintainable` also raises `MaintenanceRequired`. `FleetManager` subscribes to all three on every vehicle and re-broadcasts them through its own `LogEntryAdded` event, which `Program.cs` listens to and prints. Three custom exceptions (`VehicleNotFoundException`, `DuplicateVehicleException`, `InvalidVehicleOperationException`) get caught in the menu loop so a bad vehicle ID or a duplicate registration number doesn't take the whole app down.

For threading: `StartFleetMonitor` spins up a dedicated background thread that polls the fleet on a timer, and `RefuelAllVehiclesConcurrently` fires off one `Task` per vehicle and waits for all of them with `Task.WaitAll`. Both touch the shared fleet list, so both go through the same lock. LINQ shows up in `FindByType` and `FindAvailableVehicles`, which filter with `Where`/`FirstOrDefault` instead of a manual foreach.

## Building and running

This is a classic (non-SDK) .NET Framework 4.7.2 console project, built with Visual Studio on Windows:

1. Open `PRG281 Project/Assesment/Assesment.slnx` in Visual Studio.
2. Build — Debug or Release both work.
3. Run. The main menu comes up with 9 vehicles already seeded (3 trucks, 3 vans, 3 bikes).

It should also run under Mono on Mac or Linux, but that's not what it was built or tested against.

## Using the menu

Check option 1 (View All Vehicles) first if you need a vehicle ID — most other options just ask for one and tell you plainly if it doesn't exist.

A couple of things that aren't obvious from the menu text alone:

- **Dispatch** checks fuel and maintenance before letting a vehicle go out. A truck under 20% fuel, or overdue for a service, gets refused with the reason printed.
- **Set Vehicle Fuel Level** (option 10) lets you push a vehicle's fuel down directly instead of waiting for it to actually run out — the fastest way to see the low-fuel alert and the fuel-based dispatch block fire.
- **Start Background Fleet Monitor** (option 16) keeps running after you're back at the menu, so it can log alerts while you keep working. Option 17 stops it, and exiting the app (option 0) stops it for you if you forget.

## Notes from putting this together

A few things needed tying up before this felt done rather than just working:

- `FleetManager.AddVehicle` was mutating the fleet list outside any lock, while the background monitor thread reads it under `lock (fleetLock)`. Add and Remove now take the same lock, so a vehicle can't be added or removed mid-snapshot.
- `UpdateVehicleFuel` on `FleetManager` and `ReadFuelLevel` on `ConsoleInput` both existed but neither was ever called from the menu — there was no way to set an arbitrary fuel level from the console at all. That's now menu option 10.
- The "invalid option" message still said "please select 0-14" even though the menu had grown to 17 options. Fixed, and renumbered to make room for the new fuel option.

## Known limitations

- Nothing is saved to disk. Restart the app and you're back to the 9 seeded vehicles.
- Location is just a string — there's no map or actual coordinates behind it.
- No automated tests. Everything's been checked by hand, running through the console menu.
