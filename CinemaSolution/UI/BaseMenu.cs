
using System.Globalization; 
namespace CinemaSolution.UI
{
    public abstract class BaseMenu
    {
        static protected DateTime InputCorrectDateFormat()
        {
            DateTime newDate;
            while (true)
            {
                Console.Write("the correct format is like: 2026-12-24 15:00): ");
                Console.Write(" >>>> ");
                var input = Console.ReadLine() ?? "";

                if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var dateValid))
                {
                    newDate = dateValid;
                    break;
                }
                else
                {
                    SendMessageError("The date has an incorrect format");
                }
            }
            return newDate;
        }
        static protected decimal InputDecimal()
        {
            decimal value;
            while (true)
            {
                Console.WriteLine("Enter price (e.g., 4500.00) :");
                var input = Console.ReadLine() ?? "";
                if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    if (value >= 0) break;
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error: Price cannot be negative.");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Invalid price format. Please use numbers and a dot for decimals (e.g., 4500.00).");
                    Console.ResetColor();
                }
            }
            return value;
        }

       static protected void SendInstruction(List<string> instructions)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var message in instructions)
            {
                Console.WriteLine(message);
            }
            Console.ResetColor();
        }

        static protected void SendSuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine();
            Console.ResetColor();
        }

        static protected void SendMessageError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        static protected int InputNaturalNumbers(int max)
        {
            var flag = false;
            var option = 0;
            while (!flag)
            {
                var input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out option) && option > 0 && option <= max) return option;
                else
                {
                    SendMessageError($"Oops! That option isn't valid. Please try again by entering a number. It must be a whole number between 1 and {max}, inclusive.");
                    Console.Write(" >>>> ");
                }
            }
            return option;
        }

        static public bool GetUserOption()
        {
            Console.WriteLine("Press 1 to continue, or 2 to go back.");
            var input = InputNaturalNumbers(2);
            if (input == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        } 

       static public void GetUserMenuChoice()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Press 1 to return to the Main Menu, or 0 to exit.");
                Console.ResetColor();
                Console.Write(" >>>> ");
                var input = Console.ReadLine() ?? "";
                Console.ResetColor();

                if (int.TryParse(input, out var choice))
                {
                    if (choice == 0)
                    {
                        Console.WriteLine("Exiting application. Goodbye!");
                        Environment.Exit(0); 
                    }
                    else if (choice == 1)
                    {
                        Console.WriteLine("Returning to the Main Menu...");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter 1 to return to the Main Menu, or 0 to exit.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
        }
    }
}