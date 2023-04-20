using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ConsoleTextFileInteraction
{
    public class App
    {
        private const int KEY_OFFSET = 48;
        private const string FILES_FOLDER = "notes";
        private const string FILE_FORMAT = ".txt";
        private static List<string> _fileNames;
        private static Menu menu = new Menu(
                new MenuItem("Создать файл", () => CreateFile()),
                new MenuItem("Открыть файл", () => ReadFile()),
                new MenuItem("Удалить файл", () => DeleteFile())
            );

        static App()
        {
            _fileNames = new(Directory.GetFiles(FILES_FOLDER));
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
            catch (Exception) { }
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
                WriteLineAndHold($"Файл {pathAndName} уже существует!");
                return;
            }
            if (!TryCreateFile(pathAndName, out file, out errorMessage))
            {
                WriteLineAndHold("Не удалось добавить файл: " + file?.Name + "\nОшибка:\n" + errorMessage);
                return;
            }
            WriteLineAndHold("Успешно был создан файл: " + file.Name);
        }
        private static bool TryCreateFile(string pathAndName, out FileStream file, out string? errorMessage)
        {
            errorMessage = null;
            try
            {
                file = File.Create(pathAndName);
                file.Close();
            }
            catch (Exception ex)
            {
                file = null;
                errorMessage = ex.Message;
                return false;
            }
            _fileNames = new(Directory.GetFiles(FILES_FOLDER));

            return true;
        }

        private static void ReadFile()
        {
            string? errorMessage, content;
            Console.WriteLine("\nВыберите файл для чтения:");
            var fileToRead = PrintFiles();
            if (fileToRead == null) return;
            if (!TryReadFile(fileToRead, out content, out errorMessage))
            {
                WriteLineAndHold("Не удалось прочитать файл: " + fileToRead + "\nОшибка:\n" + errorMessage);
                return;
            }
            Console.WriteLine("\nСодержимое файла " + fileToRead + ":\n\n" + content);

            // костыль убрать(не уберу)
            if (Console.ReadKey().Key == ConsoleKey.Backspace)
            {
                HandleInput(ConsoleKey.D2);
            }

        }
        private static bool TryReadFile(string fileToRead, out string? content, out string? errorMessage)
        {
            errorMessage = null;
            try
            {
                content = GetContent();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                content = String.Empty;
                return false;
            }
            return true;
            string GetContent()
            {
                string fileContent = String.Empty;
                using (var fs = new FileStream(fileToRead, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    fileContent = sr.ReadToEnd();
                }
                return fileContent;
            }
        }

        private static void DeleteFile()
        {
            string? errorMessage;
            Console.WriteLine("\nВыберите файл для для удаления:");
            string? fileToDelete = PrintFiles();
            if (fileToDelete == null) return;
            if (!TryDeleteFile(fileToDelete, out errorMessage))
            {
                WriteLineAndHold("Не удалось удалить файл: " + fileToDelete + "\nОшибка:\n" + errorMessage);
                return;
            }
            System.Console.WriteLine("Успешно был удалён файл: " + fileToDelete);

            // тот же самый костыль убери(не уберу)
            if (Console.ReadKey().Key == ConsoleKey.Backspace)
            {
                HandleInput(ConsoleKey.D3);
            }
        }
        private static bool TryDeleteFile(string fileToDelete, out string? errorMessage)
        {
            errorMessage = null;
            try
            {
                File.Delete(fileToDelete);
                _fileNames = new(Directory.GetFiles(FILES_FOLDER));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        private static string? PrintFiles(int selectedFileIndex = 0)
        {
            ShowMenuAndFiles(selectedFileIndex);
            string? chosenFile;
            do
            {
                chosenFile = HandleChoose();
            } while (chosenFile == String.Empty);
            return chosenFile;

            void ShowMenuAndFiles(int selectedIndex)
            {
                Console.Clear();
                menu.Show();
                Console.WriteLine();
                int numeration = 0;
                foreach (string file in _fileNames)
                {
                    numeration++;
                    if (_fileNames.ElementAt(selectedIndex) == file)
                    {
                        Console.WriteLine(">> " + numeration.ToString() + ". " + file + " <<");
                        continue;
                    }
                    Console.WriteLine(numeration.ToString() + ". " + file);
                }
            }
            string? HandleChoose()
            {
                ConsoleKey pressedKey = Console.ReadKey(false).Key;
                if (pressedKey == ConsoleKey.Escape || pressedKey == ConsoleKey.Backspace) return null;
                if (pressedKey == ConsoleKey.Enter) return _fileNames.ElementAt(selectedFileIndex);
                HandleInput(pressedKey);
                return String.Empty;
            }
            void HandleInput(ConsoleKey input)
            {
                switch (input)
                {
                    case ConsoleKey.UpArrow: ShowMenuAndFiles(IsValidIndex(--selectedFileIndex) ? selectedFileIndex : ++selectedFileIndex); break;
                    case ConsoleKey.DownArrow: ShowMenuAndFiles(IsValidIndex(++selectedFileIndex) ? selectedFileIndex : --selectedFileIndex); break;
                    default: ShowMenuAndFiles(selectedFileIndex); break;
                }
            }
        }
       
        private static bool IsValidIndex(int indx) => (indx >= 0 && indx <= _fileNames.Count - 1);
        private static int GetIndexFromKey(ConsoleKey key) => (int)key - KEY_OFFSET;
        private static string GetFullFilePathWithExtension(string fileName) => GetFullFilePathWithoutExtension(fileName) + FILE_FORMAT;
        private static string GetFullFilePathWithoutExtension(string fileName) => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? FILES_FOLDER + "\\" + fileName : FILES_FOLDER + "/" + fileName;
        private static void WriteLineAndHold(string message, double secondsToHold = Double.NaN)
        {
            Console.WriteLine(message);
            if (Double.IsNaN(secondsToHold))
            {
                Console.ReadKey();
                return;
            }
            Thread.Sleep(Convert.ToInt32(secondsToHold * 1000));
        }
    }
}