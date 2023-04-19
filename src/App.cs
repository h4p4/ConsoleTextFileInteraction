namespace ConsoleTextFileInteraction
{
    public class App
    {
        private static Menu menu = new Menu(
                new MenuItem("Создать файл", () => CreateFile()),
                new MenuItem("Открыть файл", () => ReadFile()),
                new MenuItem("Удалить файл", () => DeleteFile())
            );
        public static void Run()
        {
            ConsoleKey pressedKey;
            do
            {
                Console.Clear();
                menu.Show();
                pressedKey = Console.ReadKey().Key;
                HandleInput(pressedKey);
            } while (pressedKey != ConsoleKey.Escape);
        }
        private static void HandleInput(ConsoleKey inputToHandle)
        {
            switch (inputToHandle
            )
            {
                case ConsoleKey.D1:
                menu.ElementAt(1).InvokeMethod();
                    break;
                case ConsoleKey.D2:
                menu.ElementAt(2).InvokeMethod();
                    break;
                case ConsoleKey.D3:
                menu.ElementAt(3).InvokeMethod();
                    break;
                case ConsoleKey.D4:
                menu.ElementAt(4).InvokeMethod();
                    break;

            }
        }
        private static void CreateFile()
        {
            throw new NotImplementedException();
        }

        private static void ReadFile()
        {
            throw new NotImplementedException();
        }

        private static void DeleteFile()
        {
            throw new NotImplementedException();
        }

    }
}