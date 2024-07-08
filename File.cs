using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestSimulator;

static class UserFile
{
    private static readonly List<User> _users = new();
    private const string _filename = "users.json";
    private static readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _filename);

    public static List<User> Users { get { return _users; } }

    public static void Serialize()
    {
        var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        string json = JsonSerializer.Serialize(_users, options);
        File.WriteAllText(_path, json);
    }

    private static void Deserialize()
    {
        var content = File.ReadAllText(_path);
        if (!string.IsNullOrEmpty(content))
            try
            {
                List<User> loadedUsers = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_path));
                foreach (var user in loadedUsers)
                    User.AddWithoutHash(user);
            }
            catch (JsonException) { }
    }

    public static void Load()
    {
        if (!File.Exists(_path))
            File.Create(_path).Close();
        else
            Deserialize();

        if ((_users.Count != 0 && _users[0].Username != "Admin") || !User.Contains("Admin"))
            AddAdmin();
    }

    private static void AddAdmin()
    {
        var usersCopy = _users.Where(user => user.Username != "Admin").Select(user => new User(user.Username, user.Password)).ToList();

        User.Clear();
        User.AddWithHash("Admin", "admin");
        _users.AddRange(usersCopy);
        Serialize();
    }

    public static bool VerifyHash(string input, string hash) => StringComparer.OrdinalIgnoreCase.Compare(ComputeHash(input), hash) == 0;
    public static string ComputeHash(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var builder = new StringBuilder();
        foreach (byte b in bytes)
            builder.Append(b.ToString("x2"));
        return builder.ToString();
    }
}

static class TestFile
{
    private static List<Test> _tests = new();
    private const string _filename = "tests.json";
    private static readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _filename);

    public static List<Test> Tests { get { return _tests; } }

    public static void Load()
    {
        if (!File.Exists(_path))
            File.Create(_path).Close();

        try { Deserialize(); }
        catch (Exception) { return; }

        foreach (var test in _tests)
            foreach (var user in UserFile.Users)
                if (user.Username == test.Username)
                    user.AddTest(test);
    }

    public static void Add(Test test)
    {
        if (!File.Exists(_path))
            File.Create(_path).Close();

        _tests.Add(test);
        Serialize();
    }

    public static void Remove(string username)
    {
        var testsCopy = new List<Test>();
        foreach (var test in _tests)
            if (test.Username != username)
                testsCopy.Add(test);

        _tests.Clear();
        foreach (var test in testsCopy)
            _tests.Add(test);

        Serialize();
    }

    private static void Serialize()
    {
        var serializer = new DataContractSerializer(typeof(List<Test>));
        using var stream = new FileStream(_path, FileMode.Create, FileAccess.Write);
        serializer.WriteObject(stream, _tests);
    }

    private static void Deserialize()
    {
        var serializer = new DataContractSerializer(typeof(List<Test>));
        using var stream = new FileStream(_path, FileMode.Open, FileAccess.Read);
        _tests = (List<Test>)serializer.ReadObject(stream);
    }
}
