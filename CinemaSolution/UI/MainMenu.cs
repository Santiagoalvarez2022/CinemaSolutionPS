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
            bool flag = false;
            while (!flag)
            {
                //show options to user
                string[] options = ["Enter the number for your choice:","1 - See all Screenings","2 - Create New Screening.",  "3 - Modify Screening.", "4 - Delete Screening.", "5 - Exit."];
                SendInstruction(options);
                Console.Write(" >>>> ");
                // Validate that the selected option is within the allowed range, including the maximum value
                int option = InputNaturalNumbers(5);

                switch (option)
                {
                    case 1:
                        screeningMenu.SeeScreenings(_movieService, _screeningService);
                        break;
                    case 2:
                        screeningMenu.CreateScreening(_movieService, _screeningService);
                        break;
                    case 3:
                        screeningMenu.ModifyScreening(_movieService, _screeningService);
                        break;
                    case 4:
                        screeningMenu.DeleteScreening(_movieService, _screeningService);
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