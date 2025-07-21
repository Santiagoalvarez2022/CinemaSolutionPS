using CinemaSolution.Service;
using CinemaSolution.UI;
using System.Globalization;

namespace CinemaSolution
{
    internal class Program
    {
        static void Main()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;       

            DatabaseHandler databaseHandler = new DatabaseHandler();
            MovieService movieService = new MovieService(databaseHandler);
            ScreeningService screeningService = new ScreeningService(databaseHandler);

            MainMenu menu = new MainMenu(screeningService, movieService);
            menu.InitialOptions();

        }
    }
}

