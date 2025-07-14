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
            string lastLine = dataFile.Last();
            string lastId = lastLine.Split("|")[0].Trim();
            if (int.TryParse(lastId, out id))
            {
                return id + 1;
            }
            return id;

        }

        private bool IsScheduleAvailable(DateTime StartScreening, DateTime FinishScreening)
        {
            string[] ScreeningLines = _databaseHandler.ReadFile("Screening.txt");

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

        private bool IsDirectorDailyLimitReached(DateTime date, int idDirector)
        {
            string DateToFind = date.ToString("yyyy-MM-dd");

            string[] ScreeningLines = _databaseHandler.ReadFile("Screening.txt");
            int count = 0;
            foreach (var ScreeningLine in ScreeningLines)
            {
                string[] DataScreening = ScreeningLine.Split('|');

                //Validates the fields of each line, sending exceptions if there is an error.
                _databaseHandler.ValidateRecord(ExpectedRecordTypes, DataScreening, "Screening.txt");

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

        private bool IsInternationalLimitReached(DateTime newDate)
        {
            string DateToFind = newDate.ToString("yyyy-MM-dd");
            string[] ScreeningLines = _databaseHandler.ReadFile("Screening.txt");

            //vamos a ver cuantas funciones hay
            //ver en ese dia cuantas de ellas son nacionales, y cuales no.
            int count = 0;
            foreach (var ScreeningLine in ScreeningLines)
            {
                string[] ScreeningFields = ScreeningLine.Split('|');

                _databaseHandler.ValidateRecord(ExpectedRecordTypes, ScreeningFields, "Screening.txt");



                //date field                
                string DateScreening = ScreeningFields[2].Trim();
                //isInternational field
                string IsInternationalF = ScreeningFields[6].Trim();
                bool IsInternational = bool.Parse(IsInternationalF);


                if (DateScreening.Contains(DateToFind) && IsInternational)
                {
                    Console.WriteLine(ScreeningLine);
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

        public bool ValidateInstance(DateTime StartScreening, DateTime FinishScreening, int IdDirector, bool IsInternational)
        {
            // - There may be a maximum of 10 screenings per day per director.
            // - International films have a maximum of 8 screenings assigned.

            if (IsDirectorDailyLimitReached(StartScreening, IdDirector))
            {
                return false;
            }

            if (IsInternational && IsInternationalLimitReached(StartScreening))
            {
                return false;
            }
            return IsScheduleAvailable(StartScreening, FinishScreening);
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

        public void ModifyScreening()
        {
            
        }
    }
}
