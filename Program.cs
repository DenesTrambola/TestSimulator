namespace TestSimulator;

public class Program
{
    static void Main(string[] args)
    {
        Menu.EnableVirtualTerminalProcessing();

        UserFile.Load();
        TestFile.Load();

        Menu.Welcome();
        Menu.MainMenu();
    }
}
