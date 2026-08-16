using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assesment
{
    public static class ConsoleInput
    {
        public static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);

                string input = Console.ReadLine();

                if (int.TryParse(input, out int value))
                {
                    return value;
                }

                Console.WriteLine(
                    "Invalid input. Please enter a whole number.");
            }
        }


        public static int ReadPositiveInt(string message)
        {
            while (true)
            {
                int value = ReadInt(message);

                if (value > 0)
                {
                    return value;
                }

                Console.WriteLine(
                    "Value must be greater than zero.");
            }
        }


        public static int ReadFuelLevel(string message)
        {
            while (true)
            {
                int fuel = ReadInt(message);

                if (fuel >= 0 && fuel <= 100)
                {
                    return fuel;
                }

                Console.WriteLine(
                    "Fuel level must be between 0 and 100.");
            }
        }


        public static string ReadRequiredString(string message)
        {
            while (true)
            {
                Console.Write(message);

                string input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine(
                    "Input cannot be empty.");
            }
        }


        public static string ReadOptionalString(string message)
        {
            Console.Write(message);

            string input = Console.ReadLine();

            return input?.Trim() ?? "";
        }


        public static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
