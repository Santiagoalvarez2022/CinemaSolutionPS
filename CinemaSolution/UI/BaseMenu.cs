
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
                Console.Write("the correct format is: yyyy-MM-dd HH:mm): ");
                Console.Write(" >>>> ");
                string input = Console.ReadLine() ?? "";
                if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime dateValid))
                {
                    NewDate = dateValid;
                    break;
                }
            }
            return NewDate;
        }

        protected decimal InputDecimal()
        {
            decimal value;
            while (true)
            {
                Console.WriteLine("Enter price (e.g., 4500.00) :");
                string input = Console.ReadLine() ?? "";

                if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
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
    }
}