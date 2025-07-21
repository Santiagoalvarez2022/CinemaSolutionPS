namespace CinemaSolution.Service
{
    public class MovieService
    {
        private readonly DatabaseHandler _databaseHandler;

        public static readonly List<Type> ExpectedRecordTypes = [
            typeof(int),       // ID
            typeof(string),   // Name
            typeof(int),  // Duration
            typeof(int),       // IdDirector 
            typeof(bool)       // IsInternational
        ];


        public MovieService(DatabaseHandler databaseHandler)
        {
            _databaseHandler = databaseHandler;
        }
        public bool SearcMovieById(int idSelected, out List<string> movie)
        {
            movie = [""];
            var allMovies = _databaseHandler.ReadFile("Movies.txt");

            foreach (var item in allMovies)
            {
                var dataMovie = item.Split("|").ToList();

                _databaseHandler.ValidateRecord(ExpectedRecordTypes, dataMovie, "Movies.txt");

                var idMovie = int.Parse(dataMovie[0]);

                if (idMovie == idSelected)
                {
                    movie = dataMovie;
                    return true;
                }
            }

            return false;
        }



        public bool GetMovieById(int id, out List<string> movie)
        {
            movie = [""];
            var allMovies = _databaseHandler.ReadFile("Movies.txt");
            foreach (var movieLine in allMovies)
            {
                var dataMovie = movieLine.Split("|").ToList();
                var idMovieField = dataMovie[0].Trim();
                if (int.TryParse(idMovieField, out var idMovie))
                {
                    if (id == idMovie)
                    {
                        movie = dataMovie;
                        return true;
                    }
                }

            }
            return false;
        }

        public void ShowMovies()
        {
            var allMovies = _databaseHandler.ReadFile("Movies.txt");
            Console.WriteLine("ID - NAME - NATIONAL/INTERNATIONAL");

            foreach (var movieLine in allMovies)
            {
                var dataMovie = movieLine.Split("|").ToList();
                var nation = dataMovie[4].Trim() == "true" ? "INTERNATIONAL" : "NATIONAL";
                Console.WriteLine($"{dataMovie[0]} - {dataMovie[1]} - {nation}");
            }

        }
        
    }
};


