namespace CinemaSolution.Service
{
    public class DatabaseHandler
    {
        private readonly string PathDataBase;
        public DatabaseHandler()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var databaseFolderPath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\database"));
            Directory.CreateDirectory(databaseFolderPath);
            PathDataBase = databaseFolderPath;
        }

        public string EnsureFileExists(string fileName)
        {
            var filePath = Path.Combine(PathDataBase, fileName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            return filePath;
        }

        public List<string> ReadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new InvalidOperationException($"fileName in  DatabaseHandler.ReadFile() doesn't exist.");

            var filePath = EnsureFileExists(fileName);
            var dataFile = File.ReadAllLines(filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line)) 
            .ToArray();

            if (dataFile.Length == 0 && fileName == "Movies.txt") throw new InvalidOperationException($"The file {fileName} is empty.");

            return [.. dataFile];
        }


        public bool WriteAllFile(string fileName, List<string> lines)
        {
            try
            {
                var filePath = EnsureFileExists(fileName);
                File.WriteAllLines(filePath, lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim()));
                
                Console.WriteLine("Database modified.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error modifying file, {ex.Message}");
                return false;
            }


        }

        public bool DeleteLine(string lineToDelete, string fileName)
        {

            var arrayLines = ReadFile(fileName);
            var listLines = arrayLines
            .Where(line => !string.IsNullOrWhiteSpace(line) &&  !line.Equals(lineToDelete, StringComparison.Ordinal))
            .ToList();
            return WriteAllFile(fileName, listLines);
        }

        public void WriteFile(string fileName, string newLine)
        {
            var filePath = EnsureFileExists(fileName);
            File.AppendAllLines(filePath, [newLine]); 
            Console.WriteLine("Database modified.");
        }

       
        public void ValidateRecord(List<Type> valuesTypeExpected, List<string> fieldsOfRecord, string nameFile)
        {
            if (valuesTypeExpected.Count != fieldsOfRecord.Count) throw new FormatException($"Invalid field count in record {nameFile}. Expected {valuesTypeExpected.Count}, got {fieldsOfRecord.Count}.");

            for (var i = 0; i < valuesTypeExpected.Count; i++)
            {
                var typeValue = valuesTypeExpected[i];
                var valueField = fieldsOfRecord[i];

                switch (Type.GetTypeCode(typeValue))
                {
                    case TypeCode.Int32:
                        if (!int.TryParse(valueField, out _))
                            throw new FormatException($"Field {i} must be an integer. Got: '{valueField}'");
                        break;
                    case TypeCode.Decimal:
                        if (!decimal.TryParse(valueField, out _))
                            throw new FormatException($"Field {i} must be a decimal. Got: '{valueField}'");
                        break;
                    case TypeCode.DateTime:
                        if (!DateTime.TryParse(valueField, out _))
                            throw new FormatException($"Field {i} must be a date/time. Got: '{valueField}'");
                        break;
                    case TypeCode.Boolean:
                        if (!bool.TryParse(valueField, out _))
                            throw new FormatException($"Field {i} must be true or false. Got: '{valueField}'");
                        break;
                    case TypeCode.String:
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported type for validation: {typeValue.Name}. Supported: int, string, bool. The unparsed line is {string.Join("|", fieldsOfRecord)}");
                }
            }
        }
    }
};

