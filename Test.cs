using System.Runtime.Serialization;

namespace TestSimulator;

[DataContract]
public enum TestCategory
{
    [EnumMember]
    Unknown,
    [EnumMember]
    Programming,
    [EnumMember]
    Calisthenics
}

[DataContract]
class Test
{
    [DataMember]
    public TestCategory Category { get; set; } = TestCategory.Unknown;
    [DataMember]
    public int CorrectAnswers { get; set; } = 0;
    [DataMember]
    public List<Question> Questions { get; private set; } = new();
    [DataMember]
    public string Username { get; private set; } = "";
    [DataMember]
    public int SuccessPercent { get; set; } = 0;
    [DataMember]
    public int Grade { get; set; } = 0;

    public Test() { }
    public Test(string username) => Username = username;
    public Test(string username, TestCategory category, List<Question> questions)
    {
        Username = username;
        Category = category;
        Questions = new(questions);
    }

    public void Statistics(string message)
    {
        Console.WriteLine(Color.GREEN + message + ":" + Color.RESET);
        Console.WriteLine(Color.RED + "\tCategory: " + Color.WHITE + Category);
        Console.WriteLine(Color.RED + "\tCorrect Answers: " + Color.WHITE + CorrectAnswers + "/" + Questions.Count);
        Console.WriteLine(Color.RED + "\tSuccess Percent: " + Color.WHITE + SuccessPercent + "%");
        Console.WriteLine(Color.RED + "\tGrade: " + Color.WHITE + Grade + Color.RESET);
    }

    public void Start()
    {
        try
        {
            if (Questions == null || Questions.Count == 0)
                throw new TestException("No finished tests yet!", true);
        }
        catch (TestException exc) { exc.Get(); }

        for (int i = 0; i < Questions.Count; i++)
        {
            Console.WriteLine(Color.RED + (i + 1) + ". " + Color.GREEN + Questions[i].Text + Color.RESET);
            for (int j = 0; j < Questions[i].Answers.Count; j++)
                Console.WriteLine(Color.YELLOW + (char)(j + 'A') + ". " + Color.WHITE + Questions[i].Answers[j] + Color.CYAN);

            char answer = Console.ReadLine()[0];
            try
            {
                if ((answer < 'A' || answer >= 'A' + Questions[i].Answers.Count)
                    && (answer < 'a' || answer >= 'a' + Questions[i].Answers.Count))
                    throw new TestException("You need to choose from the given answers!");
                else if (answer != Questions[i].RightAnswer + 'A' - 1 && answer != Questions[i].RightAnswer + 'a' - 1)
                {
                    Console.WriteLine(Color.RED + "Incorrect answer!\n" + Color.RESET);
                    continue;
                }
                else
                {
                    Console.WriteLine(Color.GREEN + "Correct answer!\n" + Color.RESET);
                    CorrectAnswers++;
                }
            }
            catch (TestException exc)
            {
                exc.Get(Menu.Account);
                i--;
            }
        }
        CalcGrade();
    }

    public void CalcGrade()
    {
        SuccessPercent = CorrectAnswers * 100 / Questions.Count;
        Grade = CorrectAnswers * 2;
    }

    public void SetTemplate(int templateNum)
    {
        try
        {
            switch (templateNum)
            {
                case 1:
                    Category = TestCategory.Programming;
                    Questions = [
                        new("What does HTML stand for?", new()
                        {
                            "Hyper Text Markup Language", "Hyperlinks and Text Markup Language", "Home Tool Markup Language"
                        }, 1),
                        new("Which of the following is not a programming language?", new()
                        {
                            "Java", "CSS", "HTML"
                        }, 2),
                        new("Which data type is used to store a single character in C++?", new()
                        {
                            "char", "int", "float"
                        }, 1),
                        new("What is the result of 10 % 3?", new()
                        {
                            "1", "2", "3"
                        }, 1),
                        new("What is the output of the following code snippet? { print(5 == 5) }", new()
                        {
                            "True", "False", "Error"
                        }, 1),
                        new("What does the acronym 'IDE' stand for in programming?", new()
                        {
                            "Integrated Development Environment", "Interactive Development Environment", "Interpreted Development Environment"
                        }, 1)
                    ];
                    break;

                case 2:
                    Category = TestCategory.Calisthenics;
                    Questions = new()
                    {
                        new("What is the purpose of calisthenics exercises?", new()
                        {
                            "To build cardiovascular endurance",
                            "To improve flexibility",
                            "To strengthen and tone muscles using bodyweight exercises"
                        }, 3),
                        new("Which of the following is a common calisthenics exercise?", new()
                        {
                            "Deadlift", "Push Up", "Bench Press"
                        }, 2),
                        new("What is a burpee?", new()
                        {
                            "A type of push-up variation",
                            "A high-intensity interval training (HIIT) exercise",
                            "A full-body exercise involving a squat, push-up, and jump"
                        }, 3),
                        new("How can calisthenics exercises be modified to increase difficulty?", new()
                        {
                            "By adding weights", "By reducing the number of repetitions", "By performing advanced variations or progressions"
                        }, 3),
                        new("Which muscle group does a pull-up primarily target?", new()
                        {
                            "Quadriceps", "Back and Biceps", "Abdominals"
                        }, 2),
                        new("What is the benefit of including calisthenics exercises in a fitness routine?", new()
                        {
                            "Improved strength, flexibility, and cardiovascular health",
                            "Increased risk of injury compared to weightlifting",
                            "Decreased overall fitness level due to limited resistance"
                        }, 1)
                    };
                    break;

                default:
                    throw new TestException("Invalid Test template!", true);
            }
        }
        catch (TestException exc) { exc.Get(); }
    }
}

[DataContract]
class Question
{
    [DataMember]
    public List<string> Answers { get; private set; }
    [DataMember]
    public string Text { get; private set; }
    [DataMember]
    public int RightAnswer { get; private set; }

    public Question(string text, List<string> answers, int rightAnswer)
    {
        Text = text;
        Answers = new List<string>(answers);
        RightAnswer = rightAnswer;
    }

    public void Show()
    {
        try
        {
            if (Answers == null || Answers.Count == 0 || Text == null || Text == "")
                throw new TestException("Empty question!", true);
        }
        catch (TestException exc) { exc.Get(); }

        Console.WriteLine(Color.GREEN + Text + "\n" + Color.RESET);
        for (int i = 0; i < Answers.Count; i++)
            Console.WriteLine(Color.YELLOW + (i + 1) + ". " + Color.WHITE + Answers[i] + Color.RESET);
    }
}
