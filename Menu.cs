using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace TestSimulator;

public static class Menu
{
    private static int _itemCount = 0;
    private static string[] _items = new string[_itemCount];
    private static bool _welcomed = false;

    public static void Welcome()
    {
        Console.WriteLine(Color.GREEN + "Welcome to " + Color.YELLOW + "Student Test Simulator" + Color.GREEN
            + ", where you can go through\ntests, view your statistics etc... You can use commands\n" + Color.YELLOW + "back" + Color.GREEN
            + " and " + Color.YELLOW + "exit" + Color.GREEN + " or shortcuts " + Color.YELLOW + "b" + Color.GREEN + " and " + Color.YELLOW
            + "e" + Color.GREEN + "! Enjoy the tests!" + Color.RESET);
    }

    public static bool FilterByCategory(TestCategory category)
    {
        int testCount = 0;

        foreach (var user in UserFile.Users)
        {
            if (user.Tests == null || user.Tests.Count == 0)
                continue;

            foreach (var test in user.Tests)
                if (test.Category == category)
                    test.Statistics(++testCount + ". " + user.Username);
        }

        if (testCount == 0)
            Console.WriteLine(Color.RED + "\nNo test found!" + Color.RESET);

        return Account();
    }

    public static bool PrintAccount()
    {
        string username = AdminChoose();
        int testCount = 0;

        foreach (var user in UserFile.Users)
        {
            if (user.Tests == null || user.Tests.Count == 0)
                continue;

            foreach (var test in user.Tests)
                if (test.Username == username)
                    test.Statistics("Test " + ++testCount);
        }

        if (testCount == 0)
            Console.WriteLine(Color.RED + "\nNo test found!" + Color.RESET);

        return Account();
    }

    public static bool PrintAllAccounts()
    {
        int testCount = 0;
        foreach (var user in UserFile.Users)
        {
            if (user.Tests == null || user.Tests.Count == 0)
                continue;
            foreach (var test in user.Tests)
            {
                test.Statistics(user.Username);
                testCount++;
            }
        }

        if (testCount == 0)
            Console.WriteLine(Color.RED + "\nNo test found!" + Color.RESET);

        return Account();
    }

    public static bool FilterStatistics()
    {
        _itemCount = 5;
        _items = new string[_itemCount];

        _items[0] = "Filter by Programming category";
        _items[1] = "Filter by Calisthenics category";
        _items[2] = "Print Account";
        _items[3] = "Print All Accounts";
        _items[4] = "Back";

        Print();
        int item = Choose();

        switch (item)
        {
            case 1:
                return FilterByCategory(TestCategory.Programming);
            case 2:
                return FilterByCategory(TestCategory.Calisthenics);
            case 3:
                return PrintAccount();
            case 4:
                return PrintAllAccounts();
        }

        return Account();
    }

    public static bool ViewStatistics(string username = "")
    {
        var user = new User();
        if (username == "")
            user = User.Find(User.CurrentUser);
        else
            user = User.Find(username);

        if (user.Tests == null || user.Tests.Count == 0)
        {
            Console.WriteLine(Color.RED + "\nEmpty tests!" + Color.RESET);
            return false;
        }

        Console.WriteLine(Color.GREEN + "Your statistics:" + Color.RESET);
        for (int i = 0; i < user.Tests.Count; i++)
            user.Tests[i].Statistics("Test " + (i + 1));

        return Account();
    }

    public static bool StartTest()
    {
        string username;
        if (User.CurrentUser == "Admin")
            username = AdminChoose();
        else
            username = User.CurrentUser;

        if (username == "")
            return Account();

        Console.WriteLine(Color.GREEN + "Choose a category:" + Color.RESET);
        _itemCount = 3;
        _items = new string[_itemCount];
        _items[0] = TestCategory.Programming.ToString();
        _items[1] = TestCategory.Calisthenics.ToString();
        _items[2] = "Back to Previous Menu";
        Print();

        int chosenItem = Choose();
        if (chosenItem < 1 || chosenItem > 3)
            return StartTest();
        else if (chosenItem == 3)
            return Account();

        var test = new Test(username);
        test.SetTemplate(chosenItem);
        test.Start();
        test.CalcGrade();
        test.Statistics("Test results");
        TestFile.Add(test);

        foreach (var user in UserFile.Users)
            if (user.Username == username)
                user.AddTest(test);

        return Account();
    }

    public static bool DeleteAccount()
    {
        string username;
        if (User.CurrentUser == "Admin")
            username = AdminChoose();
        else
            username = User.CurrentUser;

        var usersCopy = new List<User>();
        foreach (var user in UserFile.Users)
            if (user.Username != username)
                usersCopy.Add(user);

        UserFile.Users.Clear();
        foreach (var user in usersCopy)
            User.AddWithoutHash(user);

        UserFile.Serialize();
        User.CurrentUser = "";
        TestFile.Remove(username);

        Console.WriteLine(Color.GREEN + "\nSuccessful delete!\n" + Color.RESET);
        return true;
    }

    public static bool SignOut()
    {
        Console.WriteLine(Color.GREEN + "\nSigning out...\n" + Color.RESET);
        User.CurrentUser = "";
        return true;
    }

    public static bool ChangePassword()
    {
        string username;
        if (User.CurrentUser == "Admin")
            username = AdminChoose();
        else
            username = User.CurrentUser;

        if (username == "")
            return Account();

        var foundUser = User.Find(username);

        Console.Write(Color.YELLOW + "\nEnter " + Color.WHITE + "old" + Color.YELLOW + " password: " + Color.CYAN);
        if (!UserFile.VerifyHash(EnterPassword(), foundUser.Password))
        {
            Console.WriteLine(Color.RED + "\nInvalid password! Try again!" + Color.RESET);
            return ChangePassword();
        }

        Console.Write(Color.YELLOW + "Enter " + Color.WHITE + "new" + Color.YELLOW + " password: " + Color.CYAN);
        string newPassword = EnterPassword();
        if (UserFile.VerifyHash(newPassword, foundUser.Password))
        {
            Console.WriteLine(Color.GREEN + "\nPassword is still the same!\n" + Color.RESET);
            return false;
        }

        foreach (var user in UserFile.Users)
            if (user == foundUser)
            {
                user.Password = UserFile.ComputeHash(newPassword);
                UserFile.Serialize();
                break;
            }

        Console.WriteLine(Color.GREEN + "\nSuccessful password change!\n" + Color.RESET);
        return true;
    }

    public static bool Account()
    {
        _itemCount = 5;
        _items = new string[_itemCount];

        _items[0] = "Start new test";
        _items[1] = "View statistics";
        _items[2] = "Change password";
        _items[3] = "Delete account";
        _items[4] = "Sign out";

        Print();
        int item = Choose();

        switch (item)
        {
            case 1:
                return StartTest();
            case 2:
                return User.CurrentUser == "Admin" ? FilterStatistics() : ViewStatistics();
            case 3:
                return ChangePassword();
            case 4:
                return DeleteAccount();
            case 5:
                return SignOut();
        }

        return Account();
    }

    public static string EnterPassword()
    {
        var password = new StringBuilder();
        ConsoleKeyInfo keyInfo;

        while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (keyInfo.Key != ConsoleKey.Backspace)
            {
                password.Append(keyInfo.KeyChar);
                Console.Write("*");
            }
        }

        Console.WriteLine();
        return password.ToString();
    }

    public static bool Login()
    {
        try
        {
            Console.Write(Color.YELLOW + "Username: " + Color.CYAN);
            string username = Console.ReadLine();

            if (!User.Contains(username))
                throw new UserException("Invalid username!");

            Console.Write(Color.YELLOW + "Password: " + Color.CYAN);
            string password = EnterPassword();

            if (!User.Contains(username, password))
                throw new UserException("Invalid password!");

            User.CurrentUser = username;
        }
        catch (UserException exc)
        {
            exc.Get(MainMenu);
            return Login();
        }

        Console.WriteLine(Color.GREEN + "\nSuccessful login!" + Color.RESET);
        return Account();
    }

    public static bool Register()
    {
        try
        {
            Console.Write(Color.YELLOW + "Username: " + Color.CYAN);
            string username = Console.ReadLine();

            if (User.Contains(username))
                throw new UserException("Username already exists!");

            Console.Write(Color.YELLOW + "Password: " + Color.CYAN);
            string password = EnterPassword();
            Console.Write(Color.YELLOW + "Repeat password: " + Color.CYAN);
            string passwordRepeat = EnterPassword();

            if (password != passwordRepeat)
                throw new UserException("Passwords are not matching!");

            User.AddWithHash(username, password);
            UserFile.Serialize();
        }
        catch (UserException exc)
        {
            exc.Get(MainMenu);
            return Register();
        }

        Console.WriteLine(Color.GREEN + "\nSuccessful register!" + Color.RESET);
        return true;
    }

    public static bool MainMenu()
    {
        int chosenItem;

        if (UserFile.Users.Count > 1)
            _welcomed = true;

        if (!_welcomed)
        {
            _welcomed = true;
            chosenItem = 2;
        }
        else
        {
            _itemCount = 3;
            _items = new string[_itemCount];

            _items[0] = "Login";
            _items[1] = "Register";
            _items[2] = "Exit";

            Print();
            chosenItem = Choose();
        }

        switch (chosenItem)
        {
            case 1:
                Console.WriteLine(Color.WHITE + "\nLogin..." + Color.RESET);
                Login();
                break;
            case 2:
                Console.WriteLine(Color.WHITE + "\nRegister..." + Color.RESET);
                Register();
                break;
            case 3:
                Exit();
                break;
        }

        return MainMenu();
    }

    public static void Print()
    {
        if (_itemCount == 0)
            throw new MenuException("Empty menu exception!", true);

        Console.WriteLine();
        for (int i = 0; i < _itemCount; i++)
            Console.WriteLine(Color.RED + (i + 1) + ". " + Color.YELLOW + _items[i] + Color.RESET);
    }

    private static string AdminChoose()
    {
        _itemCount = UserFile.Users.Count;
        _items = new string[_itemCount];

        for (int i = 0; i < _itemCount; i++)
        {
            if (i == _itemCount - 1)
                _items[i] = "Back to Previous Menu";
            else
                _items[i] = UserFile.Users[i + 1].Username;
        }

        Print();
        int userNum = Choose();

        if (userNum < 1 || userNum > _itemCount)
            return AdminChoose();

        return userNum == _itemCount ? "" : UserFile.Users[userNum].Username;
    }

    public static int Choose()
    {
        Console.Write(Color.CYAN);
        int item = 0;

        if (!int.TryParse(Console.ReadLine(), out item) || item < 1 || item > _itemCount)
            Console.WriteLine(Color.RED + "\nYou need to choose from the given menu items!" + Color.RESET);

        return item;
    }

    [DoesNotReturn]
    public static void Exit()
    {
        Console.WriteLine(Color.GREEN + "\nClosing application..." + Color.RESET);
        Thread.Sleep(700);
        Environment.Exit(0);
    }


    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    public static void EnableVirtualTerminalProcessing()
    {
        var handle = GetStdHandle(STD_OUTPUT_HANDLE);
        GetConsoleMode(handle, out uint mode);
        mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        SetConsoleMode(handle, mode);
    }
}
