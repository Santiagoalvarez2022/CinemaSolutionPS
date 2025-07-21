using CinemaSolution.Service;
using CinemaSolution.Models;
namespace CinemaSolution.UI
{
    public class MainMenu : BaseMenu
    {
        private ScreeningService _screeningService { get; set; }
        private MovieService _movieService { get; set; }
        public MainMenu(ScreeningService screeningService, MovieService movieService)
        {
            _screeningService = screeningService;
            _movieService = movieService;
        }
        
        public List<DateTime> GetSchedules(List<string> movie,  List<string> records)
        {
            while (true)
            {
                var now = DateTime.Now;
                SendInstruction([" 2 - Enter the date and start time"]);

                var startScreening = InputCorrectDateFormat();

                if (startScreening < now) 
                {
                    SendMessageError("This date is not valid because it is in the past, try again.");
                    continue;
                }

                var minutes = int.Parse(movie[2]);
                var idDirector = int.Parse(movie[3]);
                var isInternational = bool.Parse(movie[4]);
                var duration = TimeSpan.FromMinutes(minutes);
                var finishScreening = startScreening.Add(duration);

                try
                {
                    var isValid = _screeningService.ValidateInstance(startScreening, finishScreening, idDirector, isInternational, records);
                    if (isValid) return [startScreening, finishScreening];
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

        public  List<string> CaptureMovie()
        {
            while (true)
            {
                SendInstruction([" 1 - To start, first select the movies's id."]);
                Console.WriteLine("Movies List : ");
                _movieService.ShowMovies();
                Console.WriteLine();
                Console.Write(" >>>> ");
                var idSelected = InputNaturalNumbers(16);
                try
                {
                    if (_movieService.SearcMovieById(idSelected, out List<string> movie))
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

        public void SeeScreenings()
        {
            Console.WriteLine("Screenings List sorted by date : ");
            var areThereScreening = _screeningService.ShowScreening(_movieService);
            if (!areThereScreening)
            {
                SendInstruction(["There are no registered functions yet, go back to the beginning and create the first one."]);
            }
            GetUserMenuChoice();
        }


        public void CreateScreening()
        {
            var createdSuccefuly = false;
            while (!createdSuccefuly)
            {
                try
                {
                    SendInstruction(["Enter the necessary information for the new screening."]);
                    Console.Write(" >>>> ");
                    var movie = CaptureMovie();

                    var allRecords = _screeningService.GetAllScreening();

                    if (GetUserOption()) break;

                    var schedules = GetSchedules(movie,  allRecords);
                    var startTime = schedules[0];
                    var endTime = schedules[1];
                    var isInternational = bool.Parse(movie[4]);
                    var idDirector = int.Parse(movie[3]);
                    var idMovie = int.Parse(movie[0]);
                    var id = _screeningService.GetNewId();

                    SendSuccessMessage($"Date: {startTime:yyyy-MM-dd} | Start: {startTime:HH:mm} | End: {endTime:HH:mm}");

                    if (GetUserOption()) break;

                    SendInstruction(["3 - Enter the price of the ticket"]);
                    var Price = InputDecimal();
                    Console.Write(" >>>> ");

                    var newScreening = new Screening(id, Price, startTime, endTime, idMovie, idDirector, isInternational);

                    var hasCreated = _screeningService.AddNewScreening(newScreening);
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw ;
                }
            }
            GetUserMenuChoice();
        }
        public void DeleteScreening()
        {
            var deleteSuccefuly = false;
            while (!deleteSuccefuly)
            {
                try
                {
                    Console.WriteLine("Screenings List sorted by date : ");
                    var areThereScreening = _screeningService.ShowScreening(_movieService);
                    if (!areThereScreening)
                    {
                        SendInstruction(["There are no registered functions yet, go back to the beginning and create the first one."]);
                        break;
                    }
                    var dataScreening = CaptureScreening();
                    var foundScreening = dataScreening[0];
                    var foundMovie = dataScreening[1];

                    SendSuccessMessage($@"
                        Screening Start: {foundScreening[2]}
                        Screening End: {foundScreening[3]}
                        Movie Name: {foundMovie[1]}
                        Ticket Price: {foundScreening[1]}
                    ");
                    SendInstruction(["Is this screening the one you wish to delete?", "Enter 1 for Yes, 2 to find other, 3 to cancel."]);
                    Console.Write(" >>>> ");

                    var option = InputNaturalNumbers(3);

                    if (option == 3) break;

                    if (option == 2)
                    {
                        Console.Clear();
                        Console.WriteLine("You selected find another screening");
                        continue;
                    }

                    _screeningService.DeleteScreening(foundScreening);
                    deleteSuccefuly = true;
                    break;
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("There are no movies uploaded. Please contact the administrator.");
                    Environment.Exit(1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            GetUserMenuChoice();
        }

        public List<List<string>> CaptureScreening()
        {
            List<string> foundScreening;
            List<string> foundMovie;
            while (true)
            {
                SendInstruction([" 1 - Enter the screening's id."]);
                Console.Write(" >>>> ");
                var inputId = Console.ReadLine() ?? "";
                try
                {
                    if (int.TryParse(inputId, out var idSelected))
                    {
                        if (_screeningService.GetScreeningById(idSelected, out foundScreening))
                        {
                            var idMovieFound = int.Parse(foundScreening[4]);
                            if (_movieService.GetMovieById(idMovieFound, out foundMovie))
                            {
                                return [foundScreening, foundMovie];
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
        public void ModifyScreening()
        {
            var modifySuccefuly = false;
            while (!modifySuccefuly)
            {
                try
                {
                    Console.WriteLine("Screenings List sorted by date : ");
                    var areThereScreening = _screeningService.ShowScreening(_movieService);
                    if (!areThereScreening)
                    {
                        SendInstruction(["There are no registered functions yet, go back to the beginning and create the first one."]);
                        break;
                    }
                    var dataScreening = CaptureScreening();
                    var foundScreening = dataScreening[0];
                    var foundMovie = dataScreening[1];

                    SendSuccessMessage($@"
                        Screening Start: {foundScreening[2]}
                        Screening End: {foundScreening[3]}
                        Movie Name: {foundMovie[1]}
                        Ticket Price: {foundScreening[1]}
                    ");

                    SendInstruction(["Is this screening the one you wish to modify?", "Enter 1 for Yes, 2 to find other, 3 to cancel. "]);
                    Console.Write(" >>>> ");

                    var option = InputNaturalNumbers(3);
                    if (option == 3) break;
                    if (option == 2)
                    {
                        Console.Clear();
                        Console.WriteLine("You selected find another screening");
                        continue;
                    }

                    SendInstruction(["Great, now we're going to ask you for the new information to complete the modification.", "Below you will see the list of all the movies in the cinema, please select one of them."]);

                    var movie = CaptureMovie();

                    if (GetUserOption())
                    {
                        break;
                    }
                    var screningToModify = string.Join('|', foundScreening);
                    List<string> listRecords = _screeningService.GetAllScreening();
                    listRecords.Remove(screningToModify);

                    SendInstruction(["Good! Now we need the new date and start time."]);
                    var schedules = GetSchedules(movie, listRecords);

                    var startTime = schedules[0];
                    var endTime = schedules[1];
                    var isInternational = bool.Parse(movie[4]);
                    var idDirector = int.Parse(movie[3]);
                    var idMovie = int.Parse(movie[0]);
                    var id = int.Parse(foundScreening[0]);

                    SendSuccessMessage($"Date: {startTime:yyyy-MM-dd} | Start: {startTime:HH:mm} | End: {endTime:HH:mm} ");

                    if (GetUserOption()) break;

                    SendInstruction(["3 - Enter the price of the ticket"]);
                    var Price = InputDecimal();
                    Console.Write(" >>>> ");

                    var newScreening = new Screening(id, Price, startTime, endTime, idMovie, idDirector, isInternational);

                    _screeningService.DeleteScreening(foundScreening);

                    var hasCreated = _screeningService.AddNewScreening(newScreening);
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            GetUserMenuChoice();
        }

        public void InitialOptions()
        {
            var flag = false;
            while (!flag)
            {
                SendInstruction(["Enter the number for your choice:", "1 - See all Screenings", "2 - Create New Screening.", "3 - Modify Screening.", "4 - Delete Screening.", "5 - Exit."]);
                Console.Write(" >>>> ");
                var option = InputNaturalNumbers(5);

                switch (option)
                {
                    case 1:
                        SeeScreenings();
                        break;
                    case 2:
                        CreateScreening();
                        break;
                    case 3:
                        ModifyScreening();
                        break;
                    case 4:
                        DeleteScreening();
                        break;
                    case 5:
                        Console.WriteLine("Exiting application. Goodbye!");
                        flag = true;
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}