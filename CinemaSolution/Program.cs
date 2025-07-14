using System;
using CinemaSolution.Service;
using CinemaSolution.UI;
using System.Globalization;

namespace CinemaSolution
{
    internal class Program
    {
        static void Main()
        {
            // Set the default culture and UI culture for the current thread to invariant culture
            // Ensures consistent formatting (e.g., dates, numbers) regardless of system locale
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;           
 
            // Instantiate FileService to handle the "database" operations
            DatabaseHandler databaseHandler = new DatabaseHandler();

            // Initialize all services responsible for the business logic of the models
            MovieService movieService = new MovieService(databaseHandler);
            ScreeningService screeningService = new ScreeningService(databaseHandler);

            // Instantiate the main menu and pass the required service instances
            MainMenu menu = new MainMenu(screeningService, movieService);
            menu.InitialOptions();

        }
    }
}

