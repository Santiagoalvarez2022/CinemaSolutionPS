using CinemaSolution.Models;
namespace CinemaSolution.Service
{
    public class MovieService
    {
        private readonly DatabaseHandler _databaseHandler;

        public static readonly Type[] ExpectedRecordTypes = new Type[]
        {
            typeof(int),       // ID
            typeof(string),   // Name
            typeof(int),  // Duration
            typeof(int),       // IdDirector
            typeof(bool)       // IsInternational
        };


        public MovieService(DatabaseHandler databaseHandler)
        {
            _databaseHandler = databaseHandler;
        }
        //validar si el nombre corresponde a una pelicula, si se encuentra devuelvo true => Movie intancia, si no false. 
        public bool SearcMovieByName(string name, out string[] movie)
        {
            movie = [""];
            string[] AllMovies = _databaseHandler.ReadFile("Movies.txt");
            //realizo la busqueda, si esta envio el nombre si no false 
            foreach (var item in AllMovies)
            {
                string[] DataMovie = item.Split("|");

                //Validates the fields of each line, sending exceptions if there is an error.
                _databaseHandler.ValidateRecord(ExpectedRecordTypes, DataMovie, "Movies.txt");

                string MovieName = DataMovie[1].Trim();

                //como deben ser mis 
                if (MovieName.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("The film found is :");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(MovieName);
                    Console.ResetColor();
                    movie = DataMovie;
                    return true;
                }
            }

            return false;
        }



        public bool GetMovieById(int id, out string[] movie)
        {
            movie = [""];
            string[] AllMovies = _databaseHandler.ReadFile("Movies.txt");

            foreach (var movieLine in AllMovies)
            {
                string[] DataMovie = movieLine.Split("|");

                string IdMovieField = DataMovie[0].Trim();
                int IdMovie = 0;
                if (int.TryParse(IdMovieField, out IdMovie))
                {
                    if (id == IdMovie)
                    {
                        movie = DataMovie;
                        return true;
                    }
                }

            }
            //lanzar error si no lo encuentro.

            return false;

        }
    }
};


