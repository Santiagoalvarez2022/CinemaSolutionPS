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
                    //ver camel case
                    Console.WriteLine("The film found is :");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(item);
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



/*
1,El Secreto del Espejo,115,1,True
2,La Sombra Olvidada,98,2,False
3,Viaje al Corazón Estelar,142,3,True
4,Crónicas de la Ciudad Oculta,105,4,False
5,El Último Aliento del Dragón,170,5,True
6,Noches de Neblina,90,6,True
7,El Enigma del Reloj de Arena,130,7,False
8,Los Guardianes del Bosque,85,8,True
9,Canto de Sirenas,110,9,False
10,La Leyenda del Despertar,155,10,True
11,Ecos en el Vacío,100,11,True
12,El Arte de Volar,125,12,False
13,Susurros del Pasado,95,13,True
14,La Danza de las Luciérnagas,108,14,True
15,Conexión Astral,135,15,False

*/