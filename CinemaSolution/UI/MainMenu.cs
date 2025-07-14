using CinemaSolution.Service;

namespace CinemaSolution.UI
{
    public class MainMenu : BaseMenu
    {

        private readonly ScreeningService _screeningService;
        private readonly MovieService _movieService;

        //constructor 
        public MainMenu(ScreeningService screeningService, MovieService movieService)
        {
            _screeningService = screeningService;
            _movieService = movieService;
        }
        
        //creacion de instancia 
        ScreeningMenu screeningMenu = new ScreeningMenu();
        //mhetods
        public void InitialOptions()
        {
            Console.Clear();
            //show options to user
            string[] options = ["Enter the number for your choice:","1 - Create New Screening.", "2 - Delete Screening.", "3 - Modify Screening.", "4 - Exit."];
            Console.Write(" >>>> ");

            SendInstruction(options);
            // Validate that the selected option is within the allowed range, including the maximum value
            int option = InputNaturalNumbers(4);

            switch (option)
            {
                case 1:
                    Console.Clear();
                    screeningMenu.CreateScreening(_movieService, _screeningService);
                    break;
                case 2:
                    Console.Clear();
                    screeningMenu.DeleteScreening(_movieService, _screeningService);
                    break;
                case 3:
                    Console.Clear();
                    screeningMenu.DeleteScreening(_movieService, _screeningService);
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
                default:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("");
                    break;
            }
        }
    }
}