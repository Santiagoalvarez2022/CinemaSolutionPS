using CinemaSolution.Models;
using System.Globalization;

namespace CinemaSolution.Service
{
    public class ScreeningService
    {
        private readonly DatabaseHandler _databaseHandler;
        public static readonly List<Type> ExpectedRecordTypes = [
            typeof(int),       // Id
            typeof(decimal),   // Price
            typeof(DateTime),  // StartScreening
            typeof(DateTime),  // FinishScreening
            typeof(int),       // IdMovie
            typeof(int),      // IdDirector
            typeof(bool)       // IsInternational
        ];
        public ScreeningService(DatabaseHandler databaseHandler)
        {
            _databaseHandler = databaseHandler;
        }

        public int GetNewId()
        {
            var id = 1;
            var filePath = _databaseHandler.EnsureFileExists("Screening.txt");
            var dataFile = File.ReadAllLines(filePath).ToList();

            if (dataFile.Count == 0) return id;

            foreach (var item in dataFile)
            {
                var screeningFields = item.Split('|').ToList();
                _databaseHandler.ValidateRecord(ExpectedRecordTypes, screeningFields, "Screening.txt");
                var current = int.Parse(screeningFields[0]);
                if (current > id)
                {
                    id = current;
                }
            }
            return ++id ;

        }

        private static bool IsScheduleAvailable(DateTime startScreening, DateTime finishScreening, List<string> screeningLines)
        {
            var dateToFind = startScreening.ToString("yyyy-MM-dd");
            foreach (var screeningLine in screeningLines)
            {
                var screeningFields = screeningLine.Split('|').ToList();
                var dateScreening = screeningFields[2].Trim();

                if (dateScreening.Contains(dateToFind))
                {
                    var start = DateTime.Parse(screeningFields[2]);
                    var end = DateTime.Parse(screeningFields[3]);

                    var overlap = startScreening < end && finishScreening > start;

                    if (overlap)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Sorry, the time slot you've chosen is already taken by another movie.");
                        Console.ResetColor();
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsDirectorDailyLimitReached(DateTime date, int idDirector, List<string> screeningLines)
        {
            var dateToFind = date.ToString("yyyy-MM-dd");
            var count = 0;
            foreach (var screeningLine in screeningLines)
            {
                var dataScreening = screeningLine.Split('|');
                var dateScreening = dataScreening[2].Trim();
                var screeningDirector = int.Parse(dataScreening[5]);
                if (dateScreening.Contains(dateToFind) && screeningDirector == idDirector)
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

        private bool IsInternationalLimitReached(DateTime newDate, List<string> screeningLines)
        {
            var dateToFind = newDate.ToString("yyyy-MM-dd");
            var count = 0;
            foreach (var screeningLine in screeningLines)
            {
                var screeningFields = screeningLine.Split('|');
                var dateScreening = screeningFields[2].Trim();
                var isInternationalF = screeningFields[6].Trim();
                var isInternational = bool.Parse(isInternationalF);
                if (dateScreening.Contains(dateToFind) && isInternational)
                {
                    count++;
                }
            }
            if (count < 8) return false;

            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry, the limit of international film screenings per day has been reached. Try a different date");
                Console.ResetColor();
                return true;
            }
        }

        public bool ValidateInstance(DateTime startScreening, DateTime finishScreening, int idDirector, bool isInternational, List<string> screeningLines)
        {
            if (screeningLines.Count == 0) return true;

            foreach (var item in screeningLines)
            {
                var screeningFields = item.Split('|').ToList();

                _databaseHandler.ValidateRecord(ExpectedRecordTypes, screeningFields, "Screening.txt");

            }
            if (IsDirectorDailyLimitReached(startScreening, idDirector, screeningLines)) return false;

            if (isInternational && IsInternationalLimitReached(startScreening, screeningLines)) return false;

            return IsScheduleAvailable(startScreening, finishScreening, screeningLines);
        }



        public bool AddNewScreening(Screening newScreening)
        {
            _databaseHandler.WriteFile("Screening.txt", newScreening.ToString());
            return true;
        }

        public bool GetScreeningById(int idSelected, out List<string> foundScreening)
        {
            var screeningLines = _databaseHandler.ReadFile("Screening.txt");
            foundScreening = [""];
            foreach (var ScreeningLine in screeningLines)
            {
                var screeningFields = ScreeningLine.Split('|').ToList();
                _databaseHandler.ValidateRecord(ExpectedRecordTypes, screeningFields, "Screening.txt");
                var idField = screeningFields[0].Trim();
                var idScreening = int.Parse(idField);
                if (idSelected == idScreening)
                {
                    foundScreening = screeningFields;
                    return true;
                }
            }
            return false;
        }
        public bool GetScreeningByDate(DateTime startTime, out List<string> foundScreening)
        {

            var screeningLines = _databaseHandler.ReadFile("Screening.txt");
            foundScreening = [""];
            var dateToFind = startTime.ToString("yyyy-MM-dd HH:mm");
            foreach (var screeningLine in screeningLines)
            {
                var screeningFields = screeningLine.Split('|').ToList();
                _databaseHandler.ValidateRecord(ExpectedRecordTypes, screeningFields, "Screening.txt");

                var startScreening = screeningFields[2].Trim();
                if (startScreening == dateToFind)
                {
                    foundScreening = screeningFields;
                    return true;
                }
            }
            return false;
        }

        public void DeleteScreening(List<string> foundScreening)
        {
            var lineToDelete = string.Join("|", foundScreening);
            var succedDelete = _databaseHandler.DeleteLine(lineToDelete, "Screening.txt");
            if (succedDelete)
            {
                Console.WriteLine("Movie screening successfully removed.");
            }

        }

        public List<string> GetAllScreening()
        {
            var screeningLines = _databaseHandler.ReadFile("Screening.txt");
            return [.. screeningLines];
        }

        public bool ShowScreening(MovieService movieService)
        {
            var allScreening = GetAllScreening();
            var linesWithParsedTime = new List<( List<string> originalLine, DateTime startTime)>();
            Console.WriteLine("ID - DAY - START TIME - END TIME - PRICE - MOVIE - NATIONAL/INTERNATIONAL ");

            if (allScreening.Count == 0) return false;

            foreach (var line in allScreening)
            {
                var fieldsScreening = line.Split("|");
                var startTimeString = fieldsScreening[2];
                var movieIdString = fieldsScreening[4];
                var movieId = int.Parse(movieIdString);
                if (movieService.GetMovieById(movieId, out var fieldsMovie))
                {
                    if (DateTime.TryParseExact(startTimeString, "yyyy-MM-dd HH:mm",CultureInfo.InvariantCulture,DateTimeStyles.None, out var startTime))
                    {
                        var fields = fieldsScreening.Concat(fieldsMovie).ToList();
                        linesWithParsedTime.Add((fields, startTime));
                    }
                }

            }
            var sortedParsedLines = linesWithParsedTime.OrderBy(item => item.startTime).Select(item => item.originalLine).ToList();
            foreach (var fields in sortedParsedLines)
            {
                var isInternational = fields[11] == "true" ? "INTERNATIONAL" : "NATIONAL";
                Console.WriteLine($"{fields[0]} - {fields[2].Substring(0, 10)} - {fields[2].Substring(11, 5)} - {fields[3].Substring(11, 5)} - {fields[1]} - {fields[8]} - {isInternational}");
            }
            return true;                            
        }

    }
}
