using CinemaSolution.Models;
using CinemaSolution.Service;
using System.Globalization;
using System.Reflection;
namespace CinemaSolution.UI
{
    public class ScreeningMenu : BaseMenu
    {
        public DateTime[] GetSchedules(string[] movie, ScreeningService screeningService)
        {
            DateTime StartScreening;
            DateTime FinishScreening;
            while (true)
            {
                // This function returns an array containing the start and end times of the screening.
                DateTime now = DateTime.Now;
                Console.WriteLine("Enter the date and start time");
                StartScreening = InputCorrectDateFormat();
                if (StartScreening < now) // Validates that the date is not in the past.
                {
                    Console.WriteLine("This date is not valid because it is in the past.");
                }

                //I calculate the finishing time based on the sum of the start and the duration.
                int minutes = int.Parse(movie[2]);
                int IdDirector = int.Parse(movie[3]);
                bool IsInternational = bool.Parse(movie[4]);
                TimeSpan duration = TimeSpan.FromMinutes(minutes);
                FinishScreening = StartScreening.Add(duration);

                try
                {
                    //hacer las validaciones aqui mismo.
                    bool isValid = screeningService.ValidateInstance(StartScreening, FinishScreening, IdDirector, IsInternational);
                    if (isValid)
                    {
                        return new DateTime[] { StartScreening, FinishScreening };
                    }
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("There are no screening uploaded. Please contact the administrator.");
                    Environment.Exit(1);
                }
                catch (Exception ex)
                {
                    SendMessageError("An unexpected error occurred");
                    Console.WriteLine(ex.Message);
                    Environment.Exit(1);
                }

            }
        }

        public string[] CaptureMovie(MovieService movieService)
        {

            while (true)
            {
                Console.Write("(Press 0 to exit) Enter the name of the movie (include accents and punctuation if needed): ");

                string InputName = Console.ReadLine() ?? "";
                if (InputName == "0")
                {
                    Environment.Exit(0);
                }

                try
                {
                    if (movieService.SearcMovieByName(InputName, out string[] movie))
                    {
                        return movie;
                    }
                    SendMessageError("Movie not found. Please try again.");
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("There are no movies uploaded. Please contact the administrator.");
                    Environment.Exit(1);
                }
                catch (Exception ex)
                {
                    SendMessageError("An unexpected error occurred");
                    Console.WriteLine(ex.Message);
                    Environment.Exit(1);
                }
            }


        }


        public void CreateScreening(MovieService movieService, ScreeningService screeningService)
        {
            bool createdSuccefuly = false;
            while (!createdSuccefuly)
            {
                // Creates a Screening instance by capturing and validating user input from the console.
                // - Validating that the movie name exists
                // - Ensuring screening times are not in the past
                // - Collecting all necessary attributes with the correct data types
                // to successfully instantiate the Screening object
                string[] instruction = ["Enter the necessary information for the new screening."];
                SendInstruction(instruction);

                //get an array with the movie data
                string[] Movie = CaptureMovie(movieService);

                //get an available date and time for the film screening
                DateTime[] Schedules = GetSchedules(Movie, screeningService);


                //"DatabaseHandler.ValidateRecord" returns the data validating that a .ToParce() can be performed without errors.
                DateTime StartTime = Schedules[0];
                DateTime EndTime = Schedules[1];
                bool IsInternational = bool.Parse(Movie[4]);
                int IdDirector = int.Parse(Movie[3]);

                int IdMovie = int.Parse(Movie[0]);
                int Id = screeningService.GetNewId();
                decimal Price = InputDecimal();

                //"newScreening" is a new instance of model Screening
                Screening newScreening = new Screening(Id, Price, StartTime, EndTime, IdMovie, IdDirector, IsInternational);
                screeningService.AddNewScreening(newScreening);
                SendSuccessMessage("The screening was created successfully.");

            }

        }

        public void DeleteScreening(MovieService movieService, ScreeningService screeningService)
        {
            string[] FoundScreening;
            while (true)
            {
                Console.WriteLine("Please provide the exact date and start time of the screening to be deleted");
                DateTime NewDate = InputCorrectDateFormat();
                //get the correct screening to deleate
                if (screeningService.GetScreeningByDate(NewDate, out FoundScreening))
                {
                    int IdMovieFound = int.Parse(FoundScreening[0]);

                    string[] FoundMovie;
                    if (movieService.GetMovieById(IdMovieFound, out FoundMovie))
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Is the following screening the one you wish to delete?");
                        Console.ResetColor();
                        Console.WriteLine($"Screening Start: {FoundScreening[2]}");
                        Console.WriteLine($"Screening End: {FoundScreening[3]}");
                        Console.WriteLine($"Movie Name: {FoundMovie[1]}");
                        Console.WriteLine($"Ticket Price: {FoundScreening[1]}");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Enter 1 for Yes, 2 for No");
                        Console.ResetColor();

                        int option = InputNaturalNumbers(2);
                        if (option == 1) break;
                        Console.Clear();
                    }
                }
                else
                {
                    Console.WriteLine($"No screening was found at the specified date and time, please try another one.");
                }
            }
            screeningService.DeleteScreening(FoundScreening);

        }

        public void ModifyScreening(MovieService movieService, ScreeningService screeningService)
        {
        
            string[] FoundScreening;
            while (true)
            {
                Console.WriteLine("Please provide the exact date and start time of the screening to be modify");
                DateTime NewDate = InputCorrectDateFormat();
                //get the correct screening to deleate
                if (screeningService.GetScreeningByDate(NewDate, out FoundScreening))
                {
                    int IdMovieFound = int.Parse(FoundScreening[0]);

                    string[] FoundMovie;
                    if (movieService.GetMovieById(IdMovieFound, out FoundMovie))
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Is the following screening the one you wish to delete?");
                        Console.ResetColor();
                        Console.WriteLine($"Screening Start: {FoundScreening[2]}");
                        Console.WriteLine($"Screening End: {FoundScreening[3]}");
                        Console.WriteLine($"Movie Name: {FoundMovie[1]}");
                        Console.WriteLine($"Ticket Price: {FoundScreening[1]}");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Enter 1 for Yes, 2 for No");
                        Console.ResetColor();

                        int option = InputNaturalNumbers(2);
                        if (option == 1) break;
                        Console.Clear();
                    }
                }
                else
                {
                    Console.WriteLine($"No screening was found at the specified date and time, please try another one.");
                }
            }
            screeningService.DeleteScreening(FoundScreening);

        }
    }
    
}

