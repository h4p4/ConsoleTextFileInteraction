using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleTextFileInteraction{
    public class Program
    {
        static void Main(String[] args)
        {
            SetEncoding();
            App.Run();
        }
        static void SetEncoding(){
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.OutputEncoding = Encoding.GetEncoding(866);
                Console.InputEncoding = Encoding.GetEncoding(866);
            }
        }
    }
}