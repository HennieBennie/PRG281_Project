using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assesment
{
    class Program
    {
        private static FleetManager manager = new FleetManager();

        static void Main(string[] args)
        {
            Console.Title = "Fleet Manager - Smart Operations Console";

            manager.LogEntryAdded += OnFleetLogEntry;

            bool running = true;

            while (running)
            {
                DisplayMainMenu();

                int choice = ConsoleInput.ReadInt("Select an option: ");

                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case 1:
                            ViewFleet();
                            break;

                        case 2:
                            ViewVehicle();
                            break;

                        case 3:
                            AddVehicle();
                            break;

                        case 4:
                            EditVehicle();
                            break;

                        case 5:
                            RemoveVehicle();
                            break;

                        case 6:
                            DispatchVehicle();
                            break;

                        case 7:
                            ReturnVehicle();
                            break;

                        case 8:
                            RefuelVehicle();
                            break;

                        case 9:
                            UpdateLocation();
                            break;

                        case 10:
                            AddMileage();
                            break;

                        case 11:
                            CheckMaintenance();
                            break;

                        case 12:
                            ServiceVehicle();
                            break;

                        case 13:
                            ViewAvailableVehicles();
                            break;

                        case 14:
                            SearchByType();
                            break;

                        case 15: 
                            StartFleetMonitor(); 
                            break;

                        case 16: 
                            StopFleetMonitor(); 
                            break;

                        case 17: 
                            RefuelAllConcurrently(); 
                            break;

                        case 0:
                            running = false;
                            if (manager.IsMonitorRunning())   // <- added
                                manager.StopFleetMonitor();
                            Console.WriteLine(
                                "Exiting Fleet Manager. Goodbye!");
                            break;

                        default:
                            Console.WriteLine(
                                "Invalid menu option. Please select 0-14.");
                            break;
                    }
                }
                catch (VehicleNotFoundException ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
                catch (DuplicateVehicleException ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
                catch (InvalidVehicleOperationException ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"INPUT ERROR: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Unexpected error: {ex.Message}");
                }
                finally
                {
                    if (running)
                    {
                        ConsoleInput.Pause();
                    }
                }
            }
        }


        // ========================================================
        // MAIN MENU
        // ========================================================

        static void DisplayMainMenu()
        {
            Console.Clear();

            Console.WriteLine("==============================================");
            Console.WriteLine("          SMART FLEET MANAGER");
            Console.WriteLine("        SMART OPERATIONS CONSOLE");
            Console.WriteLine("==============================================");
            Console.WriteLine(
                $"Vehicles in fleet: {manager.GetFleetCount()}");
            Console.WriteLine("----------------------------------------------");

            Console.WriteLine("1.  View All Vehicles");
            Console.WriteLine("2.  View Vehicle Details");
            Console.WriteLine("3.  Add Vehicle");
            Console.WriteLine("4.  Edit Vehicle");
            Console.WriteLine("5.  Remove Vehicle");

            Console.WriteLine("----------------------------------------------");

            Console.WriteLine("6.  Dispatch Vehicle");
            Console.WriteLine("7.  Return Vehicle to Depot");
            Console.WriteLine("8.  Refuel Vehicle");
            Console.WriteLine("9.  Update Vehicle Location");

            Console.WriteLine("----------------------------------------------");

            Console.WriteLine("10. Add Mileage");
            Console.WriteLine("11. Check Maintenance");
            Console.WriteLine("12. Service Vehicle");

            Console.WriteLine("----------------------------------------------");

            Console.WriteLine("13. View Available Vehicles");
            Console.WriteLine("14. Search Vehicles by Type");

            Console.WriteLine("----------------------------------------------");

            Console.WriteLine(
                $"15. Start Background Fleet Monitor" +
                $"{(manager.IsMonitorRunning() ? " (running)" : "")}");
            Console.WriteLine("16. Stop Background Fleet Monitor");
            Console.WriteLine("17. Refuel Entire Fleet Concurrently");

            Console.WriteLine("----------------------------------------------");

            Console.WriteLine("0.  Exit");

            Console.WriteLine("==============================================");
        }


        // ========================================================
        // VIEW
        // ========================================================

        static void ViewFleet()
        {
            manager.DisplayFleet();
        }


        static void ViewVehicle()
        {
            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID: ");

            manager.DisplayVehicle(id);
        }


        // ========================================================
        // CREATE
        // ========================================================

        static void AddVehicle()
        {
            Console.WriteLine("========== ADD VEHICLE ==========");

            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID: ");

            string registration =
                ConsoleInput.ReadRequiredString(
                    "Enter registration number: ");

            string name =
                ConsoleInput.ReadRequiredString(
                    "Enter vehicle name: ");

            Console.WriteLine();
            Console.WriteLine("Select vehicle type:");
            Console.WriteLine("1. Truck");
            Console.WriteLine("2. Van");
            Console.WriteLine("3. Bike");

            int type = ConsoleInput.ReadInt(
                "Select type: ");

            Vehicle vehicle;

            switch (type)
            {
                case 1:
                    vehicle = new Truck(
                        id,
                        registration,
                        name);
                    break;

                case 2:
                    vehicle = new Van(
                        id,
                        registration,
                        name);
                    break;

                case 3:
                    vehicle = new Bike(
                        id,
                        registration,
                        name);
                    break;

                default:
                    throw new InvalidVehicleOperationException(
                        "Invalid vehicle type selected.");
            }

            manager.AddVehicle(vehicle);

            Console.WriteLine();
            Console.WriteLine(
                $"{vehicle.VehicleType} added successfully.");

            Console.WriteLine(
                $"Vehicle ID: {vehicle.VehicleID}");

            Console.WriteLine(
                $"Registration: {vehicle.RegistrationNumber}");
        }


        // ========================================================
        // UPDATE
        // ========================================================

        static void EditVehicle()
        {
            Console.WriteLine("========== EDIT VEHICLE ==========");

            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID: ");

            Vehicle vehicle = manager.FindVehicle(id);

            if (vehicle == null)
            {
                throw new VehicleNotFoundException(id);
            }

            Console.WriteLine(
                $"Current registration: " +
                $"{vehicle.RegistrationNumber}");

            Console.WriteLine(
                $"Current name: {vehicle.VehicleName}");

            string registration =
                ConsoleInput.ReadRequiredString(
                    "Enter new registration number: ");

            string name =
                ConsoleInput.ReadRequiredString(
                    "Enter new vehicle name: ");

            manager.EditVehicle(
                id,
                registration,
                name);

            Console.WriteLine(
                "Vehicle updated successfully.");
        }


        static void UpdateLocation()
        {
            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID: ");

            string location =
                ConsoleInput.ReadRequiredString(
                    "Enter new location: ");

            manager.UpdateVehicleLocation(
                id,
                location);

            Console.WriteLine(
                "Vehicle location updated successfully.");
        }


        // ========================================================
        // DELETE
        // ========================================================

        static void RemoveVehicle()
        {
            Console.WriteLine(
                "========== REMOVE VEHICLE ==========");

            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID: ");

            Vehicle vehicle = manager.FindVehicle(id);

            if (vehicle == null)
            {
                throw new VehicleNotFoundException(id);
            }

            Console.WriteLine(
                $"Vehicle selected: {vehicle.VehicleName}");

            Console.WriteLine(
                $"Registration: {vehicle.RegistrationNumber}");

            Console.Write(
                "Are you sure you want to remove " +
                "this vehicle? (Y/N): ");

            string confirmation =
                Console.ReadLine()?.Trim().ToUpper();

            if (confirmation == "Y")
            {
                bool removed =
                    manager.RemoveVehicle(id);

                if (removed)
                {
                    Console.WriteLine(
                        "Vehicle removed successfully.");
                }
            }
            else
            {
                Console.WriteLine(
                    "Vehicle removal cancelled.");
            }
        }


        // ========================================================
        // DISPATCH
        // ========================================================

        static void DispatchVehicle()
        {
            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID to dispatch: ");

            manager.DispatchVehicle(id);
        }


        static void ReturnVehicle()
        {
            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID to return: ");

            manager.ReturnVehicle(id);
        }


        // ========================================================
        // FUEL
        // ========================================================

        static void RefuelVehicle()
        {
            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID: ");

            manager.RefuelVehicle(id);
        }


        // ========================================================
        // MILEAGE
        // ========================================================

        static void AddMileage()
        {
            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID: ");

            int kilometres =
                ConsoleInput.ReadPositiveInt(
                    "Enter kilometres travelled: ");

            manager.AddVehicleMileage(
                id,
                kilometres);

            Console.WriteLine(
                "Mileage added successfully.");
        }


        // ========================================================
        // MAINTENANCE
        // ========================================================

        static void CheckMaintenance()
        {
            manager.CheckMaintenance();
        }


        static void ServiceVehicle()
        {
            int id = ConsoleInput.ReadPositiveInt(
                "Enter vehicle ID: ");

            manager.ServiceVehicle(id);
        }


        // ========================================================
        // SEARCH
        // ========================================================

        static void ViewAvailableVehicles()
        {
            manager.DisplayAvailableVehicles();
        }


        static void SearchByType()
        {
            Console.WriteLine(
                "========== SEARCH BY TYPE ==========");

            Console.WriteLine("1. Truck");
            Console.WriteLine("2. Van");
            Console.WriteLine("3. Bike");

            int choice = ConsoleInput.ReadInt(
                "Select type: ");

            string type;

            switch (choice)
            {
                case 1:
                    type = "Truck";
                    break;

                case 2:
                    type = "Van";
                    break;

                case 3:
                    type = "Bike";
                    break;

                default:
                    throw new InvalidVehicleOperationException(
                        "Invalid vehicle type.");
            }

            List<Vehicle> results =
                manager.FindByType(type);

            Console.WriteLine();

            Console.WriteLine(
                $"========== {type.ToUpper()} VEHICLES ==========");

            if (results.Count == 0)
            {
                Console.WriteLine(
                    "No vehicles of this type were found.");

                return;
            }

            foreach (Vehicle vehicle in results)
            {
                Console.WriteLine(
                    $"ID: {vehicle.VehicleID} | " +
                    $"Registration: " +
                    $"{vehicle.RegistrationNumber} | " +
                    $"Name: {vehicle.VehicleName} | " +
                    $"Available: {vehicle.IsAvailable}");
            }

            Console.WriteLine(
                $"Total {type}s: {results.Count}");
        }

        // ========================================================
        // EVENTS & DELEGATES - LOG HANDLER
        // ========================================================

        static void OnFleetLogEntry(string message)
        {
            ConsoleColor previous = Console.ForegroundColor;

            Console.WriteLine($"** {message} **");

            Console.ForegroundColor = previous;
        }


        // ========================================================
        // THREADING
        // ========================================================

        static void StartFleetMonitor()
        {
            Console.WriteLine(
                "========== FLEET MONITOR ==========");

            int interval = ConsoleInput.ReadPositiveInt(
                "Check interval in seconds: ");

            manager.StartFleetMonitor(interval);
        }


        static void StopFleetMonitor()
        {
            manager.StopFleetMonitor();
        }


        static void RefuelAllConcurrently()
        {
            Console.WriteLine(
                "========== CONCURRENT REFUEL ==========");

            manager.RefuelAllVehiclesConcurrently();
        }
    }
}


