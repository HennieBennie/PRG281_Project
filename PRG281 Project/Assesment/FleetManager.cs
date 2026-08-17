using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assesment
{

    public delegate void FleetLogHandler(string message);


    // ============================================================
    // CUSTOM EXCEPTIONS
    // ============================================================

    public class VehicleNotFoundException : Exception
    {
        public VehicleNotFoundException(int vehicleID)
            : base($"Vehicle with ID {vehicleID} was not found.")
        {
        }
    }


    public class DuplicateVehicleException : Exception
    {
        public DuplicateVehicleException(int vehicleID)
            : base($"A vehicle with ID {vehicleID} already exists.")
        {
        }
    }


    public class InvalidVehicleOperationException : Exception
    {
        public InvalidVehicleOperationException(string message)
            : base(message)
        {
        }
    }


    // ============================================================
    // FLEET MANAGER
    // ============================================================

    public class FleetManager
    {
        public event FleetLogHandler LogEntryAdded;

        private readonly object fleetLock = new object();

        private Thread monitorThread;
        private CancellationTokenSource monitorCts;
        private bool monitorRunning = false;

        // Encapsulation:
        // The fleet list cannot be accessed directly from Program.
        private readonly List<Vehicle> fleet;


        // ========================================================
        // CONSTRUCTOR
        // ========================================================

        public FleetManager()
        {


            fleet = new List<Vehicle>();

            // ----------------------------------------------------
            // Initial Trucks
            // ----------------------------------------------------

            fleet.Add(
                new Truck(
                    1,
                    "CDS791MP",
                    "Volvo FL"));

            fleet.Add(
                new Truck(
                    2,
                    "CF10VDGP",
                    "Nissan UD"));

            fleet.Add(
                new Truck(
                    3,
                    "BTN020GP",
                    "MAN TGS"));


            // ----------------------------------------------------
            // Initial Vans
            // ----------------------------------------------------

            fleet.Add(
                new Van(
                    4,
                    "BB12CDZN",
                    "Volkswagen Caddy"));

            fleet.Add(
                new Van(
                    5,
                    "BR30RJGP",
                    "Ford Transit"));

            fleet.Add(
                new Van(
                    6,
                    "HDJ392GP",
                    "Peugot Partner"));


            // ----------------------------------------------------
            // Initial Bikes
            // ----------------------------------------------------

            fleet.Add(
                new Bike(
                    7,
                    "JC72YMGP",
                    "Vespa"));

            fleet.Add(
                new Bike(
                    8,
                    "HBP713FS",
                    "Velocity 150"));

            fleet.Add(
                new Bike(
                    9,
                    "KVD496MP",
                    "Pacer 200"));

            foreach (Vehicle vehicle in fleet)
            {
                SubscribeToVehicleEvents(vehicle);
            }
        }

        private void SubscribeToVehicleEvents(Vehicle vehicle)
        {
            vehicle.FuelLow += Vehicle_FuelLow;
            vehicle.StatusChanged += Vehicle_StatusChanged;
            if (vehicle is IMaintainable maintainable)
                maintainable.MaintenanceRequired += Vehicle_MaintenanceRequired;
        }

        private void UnsubscribeFromVehicleEvents(Vehicle vehicle)
        {
            vehicle.FuelLow -= Vehicle_FuelLow;
            vehicle.StatusChanged -= Vehicle_StatusChanged;
            if (vehicle is IMaintainable maintainable)
                maintainable.MaintenanceRequired -= Vehicle_MaintenanceRequired;
        }

        private void Vehicle_FuelLow(object sender, VehicleEventArgs e) => RaiseLog($"[FUEL ALERT] {e.Message}");
        private void Vehicle_StatusChanged(object sender, VehicleEventArgs e) => RaiseLog($"[STATUS] {e.Message}");
        private void Vehicle_MaintenanceRequired(object sender, VehicleEventArgs e) => RaiseLog($"[MAINTENANCE ALERT] {e.Message}");

        private void RaiseLog(string message) => LogEntryAdded?.Invoke(message);




        // ========================================================
        // CREATE
        // ========================================================

        public void AddVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                throw new ArgumentNullException(
                    nameof(vehicle),
                    "Vehicle cannot be null.");
            }


            // Prevent duplicate IDs
            if (fleet.Any(
                v => v.VehicleID == vehicle.VehicleID))
            {
                throw new DuplicateVehicleException(
                    vehicle.VehicleID);
            }


            // Prevent duplicate registration numbers
            if (fleet.Any(
                v => v.RegistrationNumber.Equals(
                    vehicle.RegistrationNumber,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidVehicleOperationException(
                    $"Registration number " +
                    $"{vehicle.RegistrationNumber} already exists.");
            }


            fleet.Add(vehicle);
            SubscribeToVehicleEvents(vehicle);

            Console.WriteLine(
                $"{vehicle.VehicleType} added to the fleet successfully.");
        }


        // ========================================================
        // READ
        // ========================================================

        public Vehicle FindVehicle(int vehicleID)
        {
            return fleet.FirstOrDefault(
                v => v.VehicleID == vehicleID);
        }


        public List<Vehicle> GetAllVehicles()
        {
            // Return a copy so the original list remains protected.
            return new List<Vehicle>(fleet);
        }


        public int GetFleetCount()
        {
            return fleet.Count;
        }


        // ========================================================
        // DISPLAY ALL VEHICLES
        // ========================================================

        public void DisplayFleet()
        {
            Console.WriteLine();

            Console.WriteLine(
                "==============================================");

            Console.WriteLine(
                "              CURRENT FLEET");

            Console.WriteLine(
                "==============================================");


            if (fleet.Count == 0)
            {
                Console.WriteLine(
                    "The fleet is currently empty.");

                Console.WriteLine(
                    "==============================================");

                return;
            }


            foreach (Vehicle vehicle in fleet)
            {
                vehicle.DisplayInfo();

                Console.WriteLine(
                    "----------------------------------------------");
            }


            Console.WriteLine(
                $"Total vehicles: {fleet.Count}");

            Console.WriteLine(
                "==============================================");
        }


        // ========================================================
        // DISPLAY ONE VEHICLE
        // ========================================================

        public void DisplayVehicle(int vehicleID)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                throw new VehicleNotFoundException(
                    vehicleID);
            }


            Console.WriteLine();

            Console.WriteLine(
                "==============================================");

            Console.WriteLine(
                "              VEHICLE DETAILS");

            Console.WriteLine(
                "==============================================");


            vehicle.DisplayInfo();


            Console.WriteLine(
                $"Vehicle ID: {vehicle.VehicleID}");

            Console.WriteLine(
                $"Vehicle Type: {vehicle.VehicleType}");

            Console.WriteLine(
                "==============================================");
        }


        // ========================================================
        // UPDATE VEHICLE
        // ========================================================

        public void EditVehicle(
            int vehicleID,
            string registrationNumber,
            string vehicleName)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                throw new VehicleNotFoundException(
                    vehicleID);
            }


            if (string.IsNullOrWhiteSpace(
                registrationNumber))
            {
                throw new InvalidVehicleOperationException(
                    "Registration number cannot be empty.");
            }


            if (string.IsNullOrWhiteSpace(
                vehicleName))
            {
                throw new InvalidVehicleOperationException(
                    "Vehicle name cannot be empty.");
            }


            // Check that another vehicle isn't already
            // using the new registration number.
            bool duplicateRegistration =
                fleet.Any(
                    v =>
                        v.VehicleID != vehicleID &&
                        v.RegistrationNumber.Equals(
                            registrationNumber.Trim(),
                            StringComparison.OrdinalIgnoreCase));


            if (duplicateRegistration)
            {
                throw new InvalidVehicleOperationException(
                    $"Registration number " +
                    $"{registrationNumber} already exists.");
            }

            //
            vehicle.UpdateRegistration(registrationNumber);

            vehicle.UpdateName(vehicleName);
            //
        }


        // ========================================================
        // UPDATE FUEL
        // ========================================================

        public void UpdateVehicleFuel(
            int vehicleID,
            int fuelLevel)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                throw new VehicleNotFoundException(
                    vehicleID);
            }


            if (fuelLevel < 0 || fuelLevel > 100)
            {
                throw new InvalidVehicleOperationException(
                    "Fuel level must be between 0 and 100.");
            }


            vehicle.FuelLevel = fuelLevel;
        }


        // ========================================================
        // UPDATE LOCATION
        // ========================================================

        public void UpdateVehicleLocation(
            int vehicleID,
            string location)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                throw new VehicleNotFoundException(
                    vehicleID);
            }


            if (string.IsNullOrWhiteSpace(location))
            {
                throw new InvalidVehicleOperationException(
                    "Location cannot be empty.");
            }


            // Only Truck and Van implement ITrackable.
            if (vehicle is ITrackable trackable)
            {
                trackable.UpdateLocation(
                    location.Trim());
            }
            else
            {
                throw new InvalidVehicleOperationException(
                    $"{vehicle.VehicleType} does not support " +
                    "location tracking.");
            }
        }


        // ========================================================
        // DELETE
        // ========================================================

        public bool RemoveVehicle(int vehicleID)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                return false;
            }

            UnsubscribeFromVehicleEvents(vehicle);
            fleet.Remove(vehicle);

            return true;
        }


        // ========================================================
        // DISPATCH
        // ========================================================

        public void DispatchVehicle(int vehicleID)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                throw new VehicleNotFoundException(
                    vehicleID);
            }


            try
            {
                // Polymorphism happens here.
                //
                // Truck, Van and Bike can have different
                // CanDispatch behaviour.
                vehicle.Dispatch();


                Console.WriteLine(
                    $"{vehicle.RegistrationNumber} " +
                    "dispatched successfully.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(
                    $"Dispatch failed: {ex.Message}");
            }
        }


        // ========================================================
        // RETURN VEHICLE
        // ========================================================

        public void ReturnVehicle(int vehicleID)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                throw new VehicleNotFoundException(
                    vehicleID);
            }


            vehicle.ReturnToDepot();


            Console.WriteLine(
                $"{vehicle.RegistrationNumber} " +
                "returned to the depot.");
        }


        // ========================================================
        // REFUEL
        // ========================================================

        public void RefuelVehicle(int vehicleID)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                throw new VehicleNotFoundException(
                    vehicleID);
            }


            vehicle.Refuel();


            Console.WriteLine(
                $"{vehicle.RegistrationNumber} " +
                "has been refuelled to 100%.");
        }


        // ========================================================
        // MAINTENANCE CHECK
        // ========================================================

        public void CheckMaintenance()
        {
            bool maintenanceRequired = false;


            Console.WriteLine();

            Console.WriteLine(
                "========== MAINTENANCE CHECK ==========");


            foreach (Vehicle vehicle in fleet)
            {
                if (vehicle is IMaintainable maintainable)
                {
                    if (maintainable.NeedsMaintenance())
                    {
                        Console.WriteLine(
                            $"{vehicle.RegistrationNumber} " +
                            $"requires maintenance.");

                        maintenanceRequired = true;
                    }
                }
            }


            if (!maintenanceRequired)
            {
                Console.WriteLine(
                    "No vehicles currently require maintenance.");
            }
        }


        // ========================================================
        // SERVICE VEHICLE
        // ========================================================

        public void ServiceVehicle(int vehicleID)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                throw new VehicleNotFoundException(
                    vehicleID);
            }


            if (vehicle is IMaintainable maintainable)
            {
                maintainable.ServiceVehicles();


                Console.WriteLine(
                    $"{vehicle.RegistrationNumber} " +
                    "has been serviced.");
            }
            else
            {
                throw new InvalidVehicleOperationException(
                    $"{vehicle.VehicleType} does not support maintenance.");
            }
        }


        // ========================================================
        // ADD MILEAGE
        // ========================================================

        public void AddVehicleMileage(
            int vehicleID,
            int kilometres)
        {
            Vehicle vehicle = FindVehicle(vehicleID);


            if (vehicle == null)
            {
                throw new VehicleNotFoundException(
                    vehicleID);
            }


            if (kilometres <= 0)
            {
                throw new InvalidVehicleOperationException(
                    "Mileage must be greater than zero.");
            }


            if (vehicle is IMaintainable maintainable)
            {
                maintainable.AddMileage(
                    kilometres);
            }
            else
            {
                throw new InvalidVehicleOperationException(
                    $"{vehicle.VehicleType} does not support " +
                    "mileage tracking.");
            }
        }


        // ========================================================
        // SEARCH BY TYPE
        // LINQ BONUS FEATURE
        // ========================================================

        public List<Vehicle> FindByType(
            string vehicleType)
        {
            if (string.IsNullOrWhiteSpace(
                vehicleType))
            {
                throw new InvalidVehicleOperationException(
                    "Vehicle type cannot be empty.");
            }


            return fleet
                .Where(
                    v =>
                        v.VehicleType.Equals(
                            vehicleType.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                .ToList();
        }


        // ========================================================
        // FIND AVAILABLE VEHICLES
        // ========================================================

        public List<Vehicle> FindAvailableVehicles()
        {
            return fleet
                .Where(
                    v => v.IsAvailable)
                .ToList();
        }


        // ========================================================
        // DISPLAY AVAILABLE VEHICLES
        // ========================================================

        public void DisplayAvailableVehicles()
        {
            List<Vehicle> availableVehicles =
                FindAvailableVehicles();


            Console.WriteLine();

            Console.WriteLine(
                "========== AVAILABLE VEHICLES ==========");


            if (availableVehicles.Count == 0)
            {
                Console.WriteLine(
                    "No vehicles are currently available.");

                return;
            }


            foreach (Vehicle vehicle in availableVehicles)
            {
                Console.WriteLine(
                    $"ID: {vehicle.VehicleID} | " +
                    $"Type: {vehicle.VehicleType} | " +
                    $"Name: {vehicle.VehicleName} | " +
                    $"Reg: {vehicle.RegistrationNumber} | " +
                    $"Fuel: {vehicle.FuelLevel}%");
            }


            Console.WriteLine(
                $"Total available vehicles: " +
                $"{availableVehicles.Count}");
        }

        // ========================================================
        // THREADING - BACKGROUND FLEET MONITOR
        // ========================================================

        public void StartFleetMonitor(int intervalSeconds = 5)
        {
            if (monitorRunning)
            {
                RaiseLog("Fleet monitor is already running.");
                return;
            }

            if (intervalSeconds <= 0)
            {
                intervalSeconds = 5;
            }

            monitorCts = new CancellationTokenSource();
            monitorRunning = true;

            monitorThread = new Thread(
                () => MonitorLoop(intervalSeconds, monitorCts.Token))
            {
                IsBackground = true,
                Name = "FleetMonitorThread"
            };

            monitorThread.Start();

            RaiseLog(
                $"Fleet monitor started (checking every " +
                $"{intervalSeconds}s on thread " +
                $"{monitorThread.ManagedThreadId}).");
        }


        public void StopFleetMonitor()
        {
            if (!monitorRunning)
            {
                RaiseLog("Fleet monitor is not running.");
                return;
            }

            monitorCts.Cancel();
            monitorThread.Join();

            monitorRunning = false;

            RaiseLog("Fleet monitor stopped.");
        }


        public bool IsMonitorRunning()
        {
            return monitorRunning;
        }


        private void MonitorLoop(int intervalSeconds, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                List<Vehicle> snapshot;

                lock (fleetLock)
                {
                    snapshot = new List<Vehicle>(fleet);
                }

                foreach (Vehicle vehicle in snapshot)
                {
                    if (vehicle.FuelLevel < 20)
                    {
                        RaiseLog(
                            $"[MONITOR] {vehicle.RegistrationNumber} " +
                            $"fuel at {vehicle.FuelLevel}%.");
                    }

                    if (vehicle is IMaintainable maintainable &&
                        maintainable.NeedsMaintenance())
                    {
                        RaiseLog(
                            $"[MONITOR] {vehicle.RegistrationNumber} " +
                            "needs maintenance.");
                    }
                }

                token.WaitHandle.WaitOne(
                    TimeSpan.FromSeconds(intervalSeconds));
            }
        }


        // ========================================================
        // THREADING - CONCURRENT REFUEL (Task-based)
        // ========================================================
        public void RefuelAllVehiclesConcurrently()
        {
            List<Vehicle> snapshot;

            lock (fleetLock)
            {
                snapshot = new List<Vehicle>(fleet);
            }

            if (snapshot.Count == 0)
            {
                RaiseLog("Fleet is empty - nothing to refuel.");
                return;
            }

            RaiseLog(
                $"Starting concurrent refuel of " +
                $"{snapshot.Count} vehicle(s)...");

            List<Task> tasks = new List<Task>();

            foreach (Vehicle vehicle in snapshot)
            {
                Vehicle current = vehicle;

                Task task = Task.Run(() =>
                {
                    // Simulate refuelling taking some real time.
                    Thread.Sleep(200);

                    current.Refuel();

                    lock (fleetLock)
                    {
                        Console.WriteLine(
                            $"{current.RegistrationNumber} refuelled " +
                            $"on thread " +
                            $"{Thread.CurrentThread.ManagedThreadId}.");
                    }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            RaiseLog("Concurrent refuel completed for the whole fleet.");
        }
    }

}
