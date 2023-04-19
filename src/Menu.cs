namespace ConsoleTextFileInteraction{
    public class Menu {

        
        List<MenuItem> _menuItems;
        public Menu(params MenuItem [] menuItems){
            _menuItems = new(menuItems);

        }
        public void Show(){
            foreach(var item in _menuItems)
                System.Console.WriteLine(item.ToFormattedMenuItemString());
        }
        public MenuItem ElementAt(int position) =>
        _menuItems.Where(x => x.Id == position).FirstOrDefault() ?? 
        throw new Exception("No menu item at provided position index.");
    }
}