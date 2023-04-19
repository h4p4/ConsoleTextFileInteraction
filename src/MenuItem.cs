namespace ConsoleTextFileInteraction
{
    public class MenuItem
    {
        private static int _count = 1;
        private int _id;
        private string _itemText;
        private Delegate _command;
        public int Id => _id;

        public MenuItem(string itemText, Delegate command)
        {
            _itemText = itemText;
            _command = command;
            _id = _count;
            _count++;
        }
        public object? InvokeMethod(params object[] args) =>
        _command.DynamicInvoke(args);
        public string ToFormattedMenuItemString() => _id.ToString() + ". " + _itemText;
    }
}