using CinemaSolution.Models;

namespace CinemaSolution.Service
{
    public class ScreeningService
    {
        private readonly DatabaseHandler _databaseHandler;
        public static readonly Type[] ExpectedRecordTypes = new Type[]
        {
            typeof(int),       // Id
            typeof(decimal),   // Price
            typeof(DateTime),  // StartScreening
            typeof(DateTime),  // FinishScreening
            typeof(int),       // IdMovie
            typeof(int),  // IdDirector
            typeof(bool)       // IsInternational
        };
        public ScreeningService(DatabaseHandler databaseHandler)
        {
            _databaseHandler = databaseHandler;
        }

        public int GetNewId()
        {
            int id = 1;
            string filePath = _databaseHandler.EnsureFileExists("Screening.txt");


            string[] dataFile = File.ReadAllLines(filePath);

           
            if (dataFile.Length == 0)
            {
                return id;
            }

            foreach (var item in dataFile)
            {
                string[] ScreeningFields = item.Split('|');
                _databaseHandler.ValidateRecord(ExpectedRecordTypes, ScreeningFields, "Screening.txt");
                int current = int.Parse(ScreeningFields[0]);
                if (current > id)
                {
                    id = current;
                }
            }

            return id + 1;

        }

        private bool IsScheduleAvailable(DateTime StartScreening, DateTime FinishScreening, List<string> ScreeningLines)
        {

            string DateToFind = StartScreening.ToString("yyyy-MM-dd");
            foreach (var ScreeningLine in ScreeningLines)
            {
                string[] ScreeningFields = ScreeningLine.Split('|');
                string DateScreening = ScreeningFields[2].Trim();

                if (DateScreening.Contains(DateToFind))
                {
                    //solo si el dia coincide voy a parser el string
                    //datos de la funcion ya guardada
                    DateTime start = DateTime.Parse(ScreeningFields[2]);
                    DateTime end = DateTime.Parse(ScreeningFields[3]);

                    bool overlap = StartScreening < end && FinishScreening > start;

                    if (overlap)
                    {
                        //si se pisan los horarios overlap => true entonces 
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Sorry, the time slot you've chosen is already taken by another movie.");
                        Console.ResetColor();
                        return false;
                    }
                }
            }

            //se puede crear la funcion.
            return true;
        }

        private bool IsDirectorDailyLimitReached(DateTime date, int idDirector, List<string> ScreeningLines)
        {
            string DateToFind = date.ToString("yyyy-MM-dd");

            int count = 0;
            foreach (var ScreeningLine in ScreeningLines)
            {
                string[] DataScreening = ScreeningLine.Split('|');

                //Validates the fields of each line, sending exceptions if there is an error.

                string DateScreening = DataScreening[2].Trim();
                int ScreeningDirector = int.Parse(DataScreening[5]);

                if (DateScreening.Contains(DateToFind) && ScreeningDirector == idDirector)
                {
                    count++;
                }
            }

            if (count < 10)
            {
                return false;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry, the director of this film reached the limit of daily movie screenings. Try a different date");
                Console.ResetColor();
                return true;
            }
        }

        private bool IsInternationalLimitReached(DateTime newDate, List<string> ScreeningLines)
        {
            string DateToFind = newDate.ToString("yyyy-MM-dd");

            //vamos a ver cuantas funciones hay
            //ver en ese dia cuantas de ellas son nacionales, y cuales no.
            int count = 0;
            foreach (var ScreeningLine in ScreeningLines)
            {
                string[] ScreeningFields = ScreeningLine.Split('|');




                //date field                
                string DateScreening = ScreeningFields[2].Trim();
                //isInternational field
                string IsInternationalF = ScreeningFields[6].Trim();
                bool IsInternational = bool.Parse(IsInternationalF);


                if (DateScreening.Contains(DateToFind) && IsInternational)
                {
                    count++;
                }
            }

            if (count < 8)
            {
                return false;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry, the limit of international film screenings per day has been reached. Try a different date");
                Console.ResetColor();
                return true;
            }
        }

        public bool ValidateInstance(DateTime StartScreening, DateTime FinishScreening, int IdDirector, bool IsInternational, List<string> ScreeningLines)
        {
            if (ScreeningLines.Count == 0)
            {
                return true;
            }
            //valid that each record has not errors.
            foreach (string item in ScreeningLines)
            {
                string[] ScreeningFields = item.Split('|');

                _databaseHandler.ValidateRecord(ExpectedRecordTypes, ScreeningFields, "Screening.txt");

            }

            // - There may be a maximum of 10 screenings per day per director.
            // - International films have a maximum of 8 screenings assigned.

            if (IsDirectorDailyLimitReached(StartScreening, IdDirector, ScreeningLines))
            {
                return false;
            }

            if (IsInternational && IsInternationalLimitReached(StartScreening, ScreeningLines))
            {
                return false;
            }
            return IsScheduleAvailable(StartScreening, FinishScreening, ScreeningLines);
        }



        public void AddNewScreening(Screening NewScreening)
        {
            _databaseHandler.WriteFile("Screening.txt", NewScreening.ToString());

        }

        public bool GetScreeningByDate(DateTime StartTime, out string[] FoundScreening)
        {

            string[] ScreeningLines = _databaseHandler.ReadFile("Screening.txt");
            FoundScreening = [""];
            string DateToFind = StartTime.ToString("yyyy-MM-dd HH:mm");
         

            foreach (var ScreeningLine in ScreeningLines)
            {

                string[] ScreeningFields = ScreeningLine.Split('|');
                _databaseHandler.ValidateRecord(ExpectedRecordTypes, ScreeningFields, "Screening.txt");

                string StartScreening = ScreeningFields[2].Trim();
                if (StartScreening == DateToFind)
                {
                    FoundScreening = ScreeningFields;
                    return true;

                }
            }
            return false;
        }

        public void DeleteScreening(string[] foundScreening)
        {
            string lineToDelete = string.Join("|", foundScreening);
            bool succedDelete = _databaseHandler.DeleteLine(lineToDelete, "Screening.txt");
            if (succedDelete)
            {
                Console.WriteLine("Movie screening successfully removed.");
            }

        }

        public List<string> GetAllScreening()
        {
            string[] ScreeningLines = _databaseHandler.ReadFile("Screening.txt");


            return ScreeningLines.ToList(); 
        }
    }
}
