using System.Runtime.InteropServices;

namespace ConsoleTextFileInteraction
{
    public class App
    {
        private const string FILE_FOLDER = "txt";
        private const string FILE_FORMAT = ".txt";
        private static List<FileInfo> _files = new();
        private static Menu menu = new Menu(
                new MenuItem("Создать файл", () => CreateFile()),
                new MenuItem("Открыть файл", () => ReadFile()),
                new MenuItem("Удалить файл", () => DeleteFile())
            );
        static App(){
            // _files.Add(new FileInfo());
        }
        public static void Run()
        {
            ConsoleKey pressedKey;
            do
            {
                Console.Clear();
                menu.Show();
                pressedKey = Console.ReadKey(false).Key;
                HandleInput(pressedKey);
            } while (pressedKey != ConsoleKey.Escape);
        }
        private static void HandleInput(ConsoleKey inputToHandle)
        {
            const int OFFSET = 48;
            Console.Clear();
            menu.Show();
            menu.ElementAt(GetIndexFromKey(inputToHandle)).InvokeMethod();
            int GetIndexFromKey(ConsoleKey key) => (int)key - OFFSET;
        }
        private static void CreateFile()
        {
            FileStream file;
            string? errorMessage;
            Console.Write("Введите название текстового файла: ");
            string fileName = Console.ReadLine() ?? "no_name";
            string pathAndName = GetFullFilePathWithExtension(fileName);
            if (File.Exists(pathAndName))
            {
                WriteLineAndHold($"Файл {pathAndName} уже существует!", 2);
                return;
            }
            if (!TryCreateFile(pathAndName, out file, out errorMessage))
            {
                WriteLineAndHold("Не удалось добавить файл: " + file?.Name + "\nОшибка:\n" + errorMessage, 4);
                return;
            }
            WriteLineAndHold("Успешно был создан файл: " + file.Name, 1.5);
        }
        private static bool TryCreateFile(string pathAndName, out FileStream file, out string? errorMessage)
        {
            errorMessage = null;
            try
            {
                file = File.Create(pathAndName);
            }
            catch (Exception ex)
            {
                file = null;
                errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        private static void ReadFile()
        {
            throw new NotImplementedException();
        }

        private static void DeleteFile()
        {
            throw new NotImplementedException();
        }

        private static string GetFullFilePathWithExtension(string fileName) =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
         FILE_FOLDER + "\\" + fileName + FILE_FORMAT :
         FILE_FOLDER + "/" + fileName + FILE_FORMAT;
        private static void WriteLineAndHold(string message, double secondsToHold)
        {
            Console.WriteLine(message);
            Thread.Sleep(Convert.ToInt32(secondsToHold * 1000));
        }
    }
}