using System;
using System.Globalization; // Para decimal.TryParse y DateTime.TryParse
using System.Linq;
using System.IO; // Asegúrate de tener esto para Path y File
using System.Linq;
using System.Collections.Generic; // Para List<string>

namespace CinemaSolution.Service
{
    public class DatabaseHandler
    {
        private readonly string PathDataBase;
        public DatabaseHandler()
        {
            //It is validated if the necessary folder for methods exists, if it does not create and keep the path
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string databaseFolderPath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\database"));
            Directory.CreateDirectory(databaseFolderPath);
            PathDataBase = databaseFolderPath;
        }

        //methods
        public string EnsureFileExists(string fileName)
        {
            string filePath = Path.Combine(PathDataBase, fileName);
            // 3. Crea el archivo si no existe (para garantizar que exista)
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            return filePath;
        }


        //Read a complete file and return each line in an array.
        public string[] ReadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("Invalid file name.");
                return [];
            }
            string filePath = EnsureFileExists(fileName);
            string[] dataFile = File.ReadAllLines(filePath);

            if (dataFile.Length == 0)
            {
                throw new InvalidOperationException($"The file {fileName} is empty.");
            }
            return dataFile;
        }


        public bool WriteAllFile(string fileName, List<string> lines)
        {
            try
            {
                string filePath = EnsureFileExists(fileName);
                File.WriteAllLines(filePath, lines);
                Console.WriteLine("Database modified.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error modifying file, {ex.Message}");
                return false;
            }


        }

        //Write a new Line in the file.
        public void WriteFile(string fileName, string newLine)
        {
            string filePath = EnsureFileExists(fileName);
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.Write(newLine);
            }
            Console.WriteLine("Database modified.");
        }

        public bool DeleteLine(string lineToDelete, string fileName)
        {

            //obtengo todo el archivo
            string[] ArrayLines = ReadFile(fileName);
            List<string> ListLines = ArrayLines.ToList();

            // 2. Filtra la línea que quieres eliminar
            // Usamos StringComparison.Ordinal para una comparación de cadenas precisa y eficiente
            List<string> updatedLines = ListLines.Where(line => !line.Equals(lineToDelete, StringComparison.Ordinal)).ToList();

            return WriteAllFile(fileName, updatedLines);

        }

        public void ValidateRecord(Type[] ValuesTypeExpected, string[] FieldsOfRecord, string nameFile)
        {
            //If the length of the list is different, there are missing fields.
            if (ValuesTypeExpected.Length != FieldsOfRecord.Length)
            {
                throw new FormatException($"Invalid field count in record{nameFile}. Expected {ValuesTypeExpected.Length}, got {FieldsOfRecord.Length}.");
            }

            //Complex or structured types such as arrays (e.g., ["a", 1]) or objects (e.g., { key: "value" }) are not supported.
            // All values must be plain strings that can be parsed into the expected simple types.
            for (int i = 0; i < ValuesTypeExpected.Length; i++)
            {
                System.Type typeValue = ValuesTypeExpected[i];
                string valueField = FieldsOfRecord[i];
                Console.WriteLine(typeValue);
                Console.WriteLine(valueField);

                switch (Type.GetTypeCode(typeValue))
                {
                    case System.TypeCode.Int32:
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
                        // string siempre es válido, no se necesita validación de formato
                        break;

                    default: // Para cualquier otro tipo que no sea uno de los esperados
                        throw new NotSupportedException($"Unsupported type for validation: {typeValue.Name}. Supported: int, string, bool. The unparsed line is {string.Join("|", FieldsOfRecord)}");
                }
            }
        }
    }
};


/*
try
            {
                File.WriteAllLines(filePath, lines); // This is the actual file operation
                message = "File written successfully.";
                return true; // Operation succeeded
            }
            catch (Exception ex) // Catching a general exception for demonstration
            {
                // Log the exception details (ex.Message, ex.StackTrace) in a real app
                message = $"Error writing to file '{fileName}': {ex.Message}";
                return false; // Operation failed
            }

*/