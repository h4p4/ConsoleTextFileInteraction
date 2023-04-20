using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ConsoleTextFileInteraction
{
    public class App
    {
        private const int OFFSET = 48;
        private const string FILE_FOLDER = "txt";
        private const string FILE_FORMAT = ".txt";
        private static List<FileStream> _files = new();
        private static Menu menu = new Menu(
                new MenuItem("Создать файл", () => CreateFile()),
                new MenuItem("Открыть файл", () => ReadFile()),
                new MenuItem("Удалить файл", () => DeleteFile())
            );
        static App()
        {
            var filenames = new List<string>(Directory.GetFiles(FILE_FOLDER));
            List<FileStream> files = new();
            foreach (var fName in filenames)
                files.Add(new FileStream(fName, FileMode.Open));
            _files.AddRange(files);
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
            Console.Clear();
            menu.Show();
            try
            {
                menu.ElementAt(GetIndexFromKey(inputToHandle)).InvokeMethod();
            }
            catch (Exception ex) { }
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
            _files.Add(file);
            return true;
        }

        private static void ReadFile()
        {
            Console.WriteLine("\nВыберите файл для чтения:");
            var readFile = PrintFiles(0);
            if (readFile == null) return;
            using (var reader = new StreamReader(readFile.Name))
            {
                var fileContent = reader.ReadToEnd();
                Console.WriteLine(fileContent);
                Console.ReadKey();
            }
        }

        private static void DeleteFile()
        {
            Console.WriteLine("\nВыберите файл для для удаления:");
            var readFile = PrintFiles(0);
            if (readFile == null) return;
            File.Delete(readFile.Name);
            _files.Remove(_files.Where(x => x.Name == readFile.Name).First());
        }

        private static string GetFullFilePathWithExtension(string fileName) =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
         FILE_FOLDER + "\\" + fileName + FILE_FORMAT :
         FILE_FOLDER + "/" + fileName + FILE_FORMAT;
        private static string GetFullFilePathWithoutExtension(string fileName) =>
       RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
        FILE_FOLDER + "\\" + fileName :
        FILE_FOLDER + "/" + fileName;

        private static void WriteLineAndHold(string message, double secondsToHold)
        {
            Console.WriteLine(message);
            Thread.Sleep(Convert.ToInt32(secondsToHold * 1000));
        }
        private static FileStream PrintFiles(int selectedFileIndex)
        {
            Console.Clear();
            menu.Show();
            int index = 0;
            foreach (var file in _files)
            {
                index++;
                if (_files.ElementAt(selectedFileIndex) == file)
                {
                    Console.WriteLine(">> " + index.ToString() + ". " + file.Name + " <<");
                    continue;
                }
                Console.WriteLine(index.ToString() + ". " + file.Name);
            }
            var chosenFile = HandleChoose();
            return chosenFile;

            FileStream HandleChoose()
            {
                ConsoleKey pressedKey = Console.ReadKey(false).Key;
                if (pressedKey != ConsoleKey.Enter) HandleInput();
                return _files.ElementAt(selectedFileIndex);


                void HandleInput()
                {
                    switch (pressedKey)
                    {
                        case ConsoleKey.UpArrow: PrintFiles(IsValidIndex(--selectedFileIndex) ? selectedFileIndex : ++selectedFileIndex); break;
                        case ConsoleKey.DownArrow: PrintFiles(IsValidIndex(++selectedFileIndex) ? selectedFileIndex : --selectedFileIndex); break;
                        case ConsoleKey.Escape: break;
                        default: HandleInput(); break;
                    }
                }
            }
            bool IsValidIndex(int indx) => (indx >= 0 && indx <= _files.Count - 1);
        }

        private static int GetIndexFromKey(ConsoleKey key) => (int)key - OFFSET;
    }
}