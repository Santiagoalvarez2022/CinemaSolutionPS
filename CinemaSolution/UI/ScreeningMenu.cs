using CinemaSolution.Models;
using CinemaSolution.Service;
using System.Globalization;
using System.Reflection;
namespace CinemaSolution.UI
{
    public class ScreeningMenu : BaseMenu
    {
        public DateTime[] GetSchedules(string[] movie, ScreeningService screeningService, List<string> Records)
        {
            DateTime StartScreening;
            DateTime FinishScreening;
            while (true)
            {

                // This function returns an array containing the start and end times of the screening.
                DateTime now = DateTime.Now;
                string[] instructions = [" 2 - Enter the date and start time"];
                SendInstruction(instructions);

                StartScreening = InputCorrectDateFormat();

                if (StartScreening < now) // Validates that the date is not in the past.
                {
                    SendMessageError("This date is not valid because it is in the past, try again.");
                    continue;
                }

                //I calculate the finishing time based on the sum of the start and the duration.
                int minutes = int.Parse(movie[2]);
                int IdDirector = int.Parse(movie[3]);
                bool IsInternational = bool.Parse(movie[4]);
                TimeSpan duration = TimeSpan.FromMinutes(minutes);
                FinishScreening = StartScreening.Add(duration);

                try
                {
                    bool isValid = screeningService.ValidateInstance(StartScreening, FinishScreening, IdDirector, IsInternational, Records);

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
                SendInstruction([" 1 - To start, first select the movies's id."]);
                Console.WriteLine("Movies List : ");
                movieService.ShowMovies();
                Console.WriteLine();
                Console.Write(" >>>> ");
                int idSelected = InputNaturalNumbers(16);
                try
                {
                    if (movieService.SearcMovieById(idSelected, out string[] movie))
                    {
                        SendSuccessMessage($"The film found is : {movie[1]} ");
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
                try
                {
                    // Creates a Screening instance by capturing and validating user input from the console.
                    // - Validating that the movie name exists
                    // - Ensuring screening times are not in the past
                    // - Collecting all necessary attributes with the correct data types
                    // to successfully instantiate the Screening object
                    string[] instruction = ["Enter the necessary information for the new screening."];
                    SendInstruction(instruction);
                    Console.Write(" >>>> ");
                    //get an array with the movie data
                    string[] Movie = CaptureMovie(movieService);

                    List<string> allRecords = screeningService.GetAllScreening();

                    if (GetUserOption())
                    {
                        break;
                    }
                    //get an available date and time for the film screening
                    DateTime[] Schedules = GetSchedules(Movie, screeningService, allRecords);


                    //"DatabaseHandler.ValidateRecord" returns the data validating that a .ToParce() can be performed without errors.
                    DateTime StartTime = Schedules[0];
                    DateTime EndTime = Schedules[1];
                    bool IsInternational = bool.Parse(Movie[4]);
                    int IdDirector = int.Parse(Movie[3]);

                    int IdMovie = int.Parse(Movie[0]);
                    int Id = screeningService.GetNewId();

                    SendSuccessMessage($"Date: {StartTime.ToString("yyyy-MM-dd")} | Start: {StartTime.ToString("HH:mm")} | End: {EndTime.ToString("HH:mm")} ");

                    if (GetUserOption())
                    {
                        break;
                    }
                    SendInstruction(["3 - Enter the price of the ticket"]);
                    decimal Price = InputDecimal();
                    Console.Write(" >>>> ");


                    //"newScreening" is a new instance of model Screening
                    Screening newScreening = new Screening(Id, Price, StartTime, EndTime, IdMovie, IdDirector, IsInternational);

                    bool hasCreated = screeningService.AddNewScreening(newScreening);
                    if (hasCreated)
                    {
                        SendSuccessMessage("The screening was created successfully.");
                        createdSuccefuly = hasCreated;
                        break;
                    }
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("There are no movies uploaded. Please contact the administrator.");
                    Environment.Exit(1);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            GetUserMenuChoice();
        }

        public void DeleteScreening(MovieService movieService, ScreeningService screeningService)
        {
            bool deleteSuccefuly = false;
            while (!deleteSuccefuly)
            {
                try
                {
                    Console.WriteLine("Screenings List sorted by date : ");
                    bool areThereScreening = screeningService.ShowScreening(movieService);
                    if (!areThereScreening)
                    {
                        SendInstruction(["There are no registered functions yet, go back to the beginning and create the first one."]);
                        break;
                    }
                    string[][] dataScreening = CaptureScreening(screeningService, movieService);
                    string[] FoundScreening = dataScreening[0];
                    string[] FoundMovie = dataScreening[1];

                    SendSuccessMessage($@"
                        Screening Start: {FoundScreening[2]}
                        Screening End: {FoundScreening[3]}
                        Movie Name: {FoundMovie[1]}
                        Ticket Price: {FoundScreening[1]}
                    ");

                    SendInstruction(["Is this screening the one you wish to delete?", "Enter 1 for Yes, 2 to find other, 3 to cancel."]);
                    Console.Write(" >>>> ");

                    int option = InputNaturalNumbers(3);
                    // si es 1, hago la modificacion, si es 2 busco una nueva, si es 3 salgo y voy al menu.
                    if (option == 3)
                    {
                        break;
                    }

                    if (option == 2)
                    {
                        Console.Clear();
                        Console.WriteLine("You selected find another screening");
                        continue;
                    }

                    screeningService.DeleteScreening(FoundScreening);
                    deleteSuccefuly = true;
                    break;
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("There are no movies uploaded. Please contact the administrator.");
                    Environment.Exit(1);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            GetUserMenuChoice();
        }

        public string[][] CaptureScreening(ScreeningService screeningService, MovieService movieService)
        {
            string[] FoundScreening;
            string[] FoundMovie;
            while (true)
            {

                SendInstruction([" 1 - Enter the screening's id."]);
                Console.Write(" >>>> ");

                string inputId = Console.ReadLine() ?? "";
                int idSelected;

                try
                {
                    if (int.TryParse(inputId, out idSelected))
                    {
                        if (screeningService.GetScreeningById(idSelected, out FoundScreening))
                        {
                            int IdMovieFound = int.Parse(FoundScreening[4]);

                            if (movieService.GetMovieById(IdMovieFound, out FoundMovie))
                            {
                                return [FoundScreening, FoundMovie];
                            }

                        }
                        else
                        {
                            SendMessageError("No screenign found with that ID, please try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter a number from the screening list.");
                    }

                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("There are no movies uploaded. Please contact the administrator.");
                    Environment.Exit(1);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
        public void ModifyScreening(MovieService movieService, ScreeningService screeningService)
        {
            bool modifySuccefuly = false;
            while (!modifySuccefuly)
            {
                try
                {
                    Console.WriteLine("Screenings List sorted by date : ");
                    bool areThereScreening = screeningService.ShowScreening(movieService);
                    if (!areThereScreening)
                    {
                        SendInstruction(["There are no registered functions yet, go back to the beginning and create the first one."]);
                        break;
                    }

                    string[][] dataScreening = CaptureScreening(screeningService, movieService);

                    string[] FoundScreening = dataScreening[0];
                    string[] FoundMovie = dataScreening[1];

                    SendSuccessMessage($@"
                        Screening Start: {FoundScreening[2]}
                        Screening End: {FoundScreening[3]}
                        Movie Name: {FoundMovie[1]}
                        Ticket Price: {FoundScreening[1]}
                    ");

                    SendInstruction(["Is this screening the one you wish to modify?", "Enter 1 for Yes, 2 to find other, 3 to cancel. "]);
                    Console.Write(" >>>> ");

                    int option = InputNaturalNumbers(3);
                    // si es 1, hago la modificacion, si es 2 busco una nueva, si es 3 salgo y voy al menu.
                    if (option == 3)
                    {
                        break;
                    }
                    if (option == 2)
                    {
                        Console.Clear();
                        Console.WriteLine("You selected find another screening");
                        continue;
                    }

                    SendInstruction(["Great, now we're going to ask you for the new information to complete the modification.", "Below you will see the list of all the movies in the cinema, please select one of them."]);


                    //get a new movie
                    string[] Movie = CaptureMovie(movieService);

                    if (GetUserOption())
                    {
                        break;
                    }
                    //cerate a string of oldData
                    string screningToModify = string.Join('|', FoundScreening);
                    //get all data in db to compare 
                    List<string> ListRecords = screeningService.GetAllScreening();
                    ListRecords.Remove(screningToModify);


                    SendInstruction(["Good! Now we need the new date and start time."]);
                    //get an available date and time for the film screening
                    DateTime[] Schedules = GetSchedules(Movie, screeningService, ListRecords);


                    //"DatabaseHandler.ValidateRecord" returns the data validating that a .ToParce() can be performed without errors.
                    DateTime StartTime = Schedules[0];
                    DateTime EndTime = Schedules[1];
                    bool IsInternational = bool.Parse(Movie[4]);
                    int IdDirector = int.Parse(Movie[3]);

                    int IdMovie = int.Parse(Movie[0]);
                    int Id = int.Parse(FoundScreening[0]);
                    SendSuccessMessage($"Date: {StartTime.ToString("yyyy-MM-dd")} | Start: {StartTime.ToString("HH:mm")} | End: {EndTime.ToString("HH:mm")} ");

                    if (GetUserOption())
                    {
                        break;
                    }

                    SendInstruction(["3 - Enter the price of the ticket"]);
                    decimal Price = InputDecimal();
                    Console.Write(" >>>> ");

                    //"newScreening" is a new instance of model Screening
                    Screening newScreening = new Screening(Id, Price, StartTime, EndTime, IdMovie, IdDirector, IsInternational);

                    //delete all record
                    screeningService.DeleteScreening(FoundScreening);

                    //add the new one with same id
                    bool hasCreated = screeningService.AddNewScreening(newScreening);
                    if (hasCreated)
                    {
                        SendSuccessMessage("The screening was modified successfully.");
                        modifySuccefuly = hasCreated;
                        break;
                    }

                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("There are no movies uploaded. Please contact the administrator.");
                    Environment.Exit(1);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            GetUserMenuChoice();
        }
        public void SeeScreenings(MovieService movieService,ScreeningService screeningService)
        {
            Console.WriteLine("Screenings List sorted by date : ");
            bool areThereScreening = screeningService.ShowScreening(movieService);
            if (!areThereScreening)
            {
                SendInstruction(["There are no registered functions yet, go back to the beginning and create the first one."]);
            }
            GetUserMenuChoice();
        }
    }
   
}

