
//esta funcion contendra metodos que heredaran los demas menus, estos deben ser reutilzables.
using System.Globalization; // Add this at the top of your file
namespace CinemaSolution.UI
{
    //al ser abstrac no se pueden crear instancias
    public abstract class BaseMenu
    {
        DateTime NewDate;
        protected DateTime InputCorrectDateFormat()
        {
            while (true)
            {
                Console.Write("the correct format is like: 2026-12-24 15:00): ");
                Console.Write(" >>>> ");
                string input = Console.ReadLine() ?? "";

                if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime dateValid))
                {
                    NewDate = dateValid;
                    break;
                }
                else
                {
                    SendMessageError("The date has an incorrect format");
                }
            }
            return NewDate;
        }

        protected decimal InputDecimal()
        {
            decimal value;
            while (true)
            {
                Console.WriteLine("enter price (e.g., 4500.00) :");

                string input = Console.ReadLine() ?? "";

                 if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    if (value >= 0) // Prices are usually non-negative
                    {
                        break;
                    }
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

        protected void SendInstruction(string[] instructions)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (string message in instructions)
            {
                Console.WriteLine(message);
            }
            Console.ResetColor();
        }

        protected void SendSuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine();
            Console.ResetColor();
        }

        protected void SendMessageError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        protected int InputNaturalNumbers(int max)
        {
            bool flag = false;
            int option = 0;
            while (!flag)
            {
                string input = Console.ReadLine() ?? "";
                //intento parsear el valor recibido por consola a un numero entero , ademas tiene que ser positivo y menor al max de opciones 
                if (int.TryParse(input, out option) && option > 0 && option <= max)
                {
                    return option;
                }
                else
                {
                    SendMessageError($"Oops! That option isn't valid. Please try again by entering a number. It must be a whole number between 1 and {max}, inclusive.");
                    Console.Write(" >>>> ");
                }
            }
            return option;
        }

        public bool GetUserOption()
        {
            Console.WriteLine("Press 1 to continue, or 2 to go back.");
            int input = InputNaturalNumbers(2);
            if (input == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        } 

       public void GetUserMenuChoice()
        {
            int choice;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Press 1 to return to the Main Menu, or 0 to exit.");
                Console.ResetColor();
                Console.Write(" >>>> ");

                string input = Console.ReadLine();
                Console.ResetColor();

                if (int.TryParse(input, out choice))
                {
                    if (choice == 0)
                    {
                        Console.WriteLine("Exiting application. Goodbye!");
                        Environment.Exit(0); // Termina la aplicación
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