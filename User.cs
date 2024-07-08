using System.Text.Json.Serialization;

namespace TestSimulator;

class User
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public static string CurrentUser { get; set; } = "";
    [JsonIgnore]
    public List<Test> Tests { get; private set; }

    public User() { }
    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public void AddTest(Test test)
    {
        if (Tests == null)
            Tests = new List<Test>();

        Tests.Add(test);
        UserFile.Serialize();
    }

    public void AddWithHash() => AddWithHash(Username, Password);
    public static void AddWithHash(User user) => AddWithHash(user.Username, user.Password);
    public static void AddWithHash(string username, string password)
    {
        if (Contains(username))
            throw new UserException("Username already exists!");

        UserFile.Users.Add(new User(username, UserFile.ComputeHash(password)));
    }

    public void AddWithoutHash() => AddWithoutHash(Username, Password);
    public static void AddWithoutHash(User user) => AddWithoutHash(user.Username, user.Password);
    public static void AddWithoutHash(string username, string password)
    {
        if (Contains(username))
            throw new UserException("Username already exists!");

        UserFile.Users.Add(new User(username, password));
    }

    public void Remove() => UserFile.Users.Remove(this);
    public static void Remove(string username, string password) => Remove(new User(username, password));
    public static void Remove(User userToRemove)
    {
        Clear();
        foreach (var user in UserFile.Users)
        {
            if (user.Username == userToRemove.Username)
                continue;
            UserFile.Users.Add(user);
        }
    }

    public static void Clear() => UserFile.Users.Clear();

    public bool Contains() => UserFile.Users.Contains(this);
    public static bool Contains(User user) => Contains(user.Username, user.Password);
    public static bool Contains(string username, string password)
    {

        foreach (var user in UserFile.Users)
        {
            if (user.Username == username && UserFile.VerifyHash(password, user.Password))
                return true;
        }
        return false;
    }
    public static bool Contains(string username) => UserFile.Users.Any(u => u.Username == username);

    public static bool operator ==(User user1, User user2) => user1.Username == user2.Username && user1.Password == user2.Password;
    public static bool operator !=(User user1, User user2) => user1.Username != user2.Username || user1.Password != user2.Password;

    public User? Find() => UserFile.Users.Find(u => u == this);
    public static User? Find(User user) => UserFile.Users.Find(u => u == user);
    public static User? Find(string username) => UserFile.Users.Find(u => u.Username == username);
    public static User? Find(string username, string password) => UserFile.Users.Find(u => u == new User(username, password));
}
