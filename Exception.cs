namespace TestSimulator;

public abstract class ExceptionBase : Exception
{
    public delegate bool PreviousMenu();

    protected abstract string Message { get; set; }
    protected abstract bool IsError { get; set; }

    public void Get(PreviousMenu prevMenu = null)
    {
        if (IsError)
        {
            Console.WriteLine(Color.RED + "\nError: " + Color.RESET + Message + Color.RESET);
            Environment.Exit(-1);
        }

        Console.WriteLine(Color.RED + "\n" + Message + "\n\nUse commands " + Color.YELLOW + "back" + Color.RED + " or " + Color.YELLOW
            + "exit" + Color.RED + " or enter " + Color.YELLOW + "something else" + Color.RED + " to try again!" + Color.CYAN);
        switch (Console.ReadLine())
        {
            case "back" or "b" when prevMenu != null:
                prevMenu();
                break;
            case "exit" or "e":
                Menu.Exit();
                break;
        }
    }
}

class MenuException : ExceptionBase
{
    protected override string Message { get; set; }
    protected override bool IsError { get; set; }

    public MenuException(string message, bool error = false)
    {
        Message = message;
        IsError = error;
    }
}

class UserException : ExceptionBase
{
    protected override string Message { get; set; }
    protected override bool IsError { get; set; }

    public UserException(string message, bool error = false)
    {
        Message = message;
        IsError = error;
    }
}

class TestException : ExceptionBase
{
    protected override string Message { get; set; }
    protected override bool IsError { get; set; }

    public TestException(string message, bool error = false)
    {
        Message = message;
        IsError = error;
    }
}
