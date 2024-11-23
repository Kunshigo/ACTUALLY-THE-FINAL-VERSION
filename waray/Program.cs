using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace QuizApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Authentication auth = new Authentication();
            auth.Start();
        }
    }

    class Authentication
    {
        private List<User> users = new List<User>();
        private const string UsersFilePath = "users.txt";

        public void Start()
        {
            LoadUsers();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===========================================");
                Console.WriteLine("        WELCOME TO THE QUIZ APP!           ");
                Console.WriteLine("===========================================");
                Console.WriteLine("\n[1] Login\n[2] Register\n[3] Exit");
                Console.WriteLine("===========================================");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Login();
                }
                else if (choice == "2")
                {
                    Register();
                }
                else if (choice == "3")
                {
                    Console.WriteLine("\nThank you for using the Quiz App. Goodbye!");
                    Thread.Sleep(1500);
                    break;
                }
                else
                {
                    Console.WriteLine("\nInvalid option. Please try again.");
                    Thread.Sleep(1500);
                }
            }
        }

        private void LoadUsers()
        {
            if (!File.Exists(UsersFilePath))
            {
                Console.WriteLine("User file not found. Creating a new file...");
                File.Create(UsersFilePath).Close();
                return;
            }

            var lines = File.ReadAllLines(UsersFilePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) 
                    continue;
                string[] parts = line.Split('|');
                if (parts.Length < 3) 
                    continue; 

                try
                {
                    string username = parts[0].Trim();
                    string password = parts[1].Trim();
                    string role = parts[2].Trim();
                    
                    int gradeLevel = 0;

                    if (role.ToLower() == "student" && parts.Length > 3)
                    {
                        string gradeLevelString = parts[3].Trim();
                        gradeLevel = int.Parse(gradeLevelString);
                    }

                    users.Add(new User(username, password, role, gradeLevel));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error : {line}. Details: {ex.Message}");
                }
            }
        }

        private void SaveUsers()
        {
            var lines = new List<string>();
            foreach (var user in users)
            {
                lines.Add($"{user.Username}|{user.Password}|{user.Role}|{user.GradeLevel}");
            }
            File.WriteAllLines(UsersFilePath, lines);
        }


        private void Login()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===========================================");
            Console.WriteLine("                 LOGIN                     ");
            Console.WriteLine("===========================================");
            Console.ResetColor();

            Console.Write("Enter username: ");
            string username = Console.ReadLine()?.Trim();

            Console.Write("Enter password: ");
            string password = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nUsername or password cannot be empty. Please try again.");
                Console.ResetColor();
                Thread.Sleep(1500);
                return;
            }

            User user = null;
            foreach (var u in users)
            {
                if (u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password)
                {
                    user = u;
                    break;
                }
            }

            if (user != null)
            {
                Console.Clear();
                if (user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    Admin admin = new Admin(user.Username, user.Password, user.Role);
                    admin.Menu();
                }
                else if (user.Role.Equals("Student", StringComparison.OrdinalIgnoreCase))
                {
                    Student student = new Student(user.Username, user.Password, user.Role, user.GradeLevel);
                    student.Menu();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInvalid username or password. Please try again.");
                Console.ResetColor();
                Thread.Sleep(1500);
            }
        }


        private void Register()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("               REGISTER                    ");
            Console.WriteLine("===========================================");

            Console.Write("Enter username: ");
            string username = Console.ReadLine()?.Trim();

            bool usernameTaken = false;
            foreach (var user in users)
            {
                if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    usernameTaken = true;
                    break;
                }
            }

            if (usernameTaken)
            {
                Console.WriteLine("\nThis username is already taken. Please try a different one.");
                Thread.Sleep(1500);
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine()?.Trim();

            Console.Write("Enter role (Admin (Teacher) or Student): ");
            string role = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                Console.WriteLine("\nFields cannot be empty. Please try again.");
                Thread.Sleep(1500);
                return;
            }

            if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase) && !role.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\nInvalid role. Please enter either 'Admin' or 'Student'.");
                Thread.Sleep(1500);
                return;
            }

            int gradeLevel = 0;
            if (role.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Enter grade level (1, 2, or 3): ");
                if (!int.TryParse(Console.ReadLine(), out gradeLevel) || gradeLevel < 1 || gradeLevel > 3)
                {
                    Console.WriteLine("\nInvalid grade level. Please try again.");
                    Thread.Sleep(1500);
                    return;
                }
            }

            users.Add(new User(username, password, role, gradeLevel));
            SaveUsers();
            Console.WriteLine("\nRegistration successful!");
            Thread.Sleep(1500);
        }

    }


    class FileHandler
    {
        public static void SaveToFile(string filePath, List<string> data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                File.WriteAllLines(filePath, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public static List<string> LoadFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return new List<string>(File.ReadAllLines(filePath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading file: {ex.Message}");
            }
            return new List<string>();
        }
    }

    abstract class Question
    {
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
        public string Subject { get; set; }
        public int GradeLevel { get; set; }

        public abstract void Display(bool showAnswer = false);

        public Question(string questionText, string correctAnswer, string subject, int gradeLevel)
        {
            if (string.IsNullOrWhiteSpace(questionText) || string.IsNullOrWhiteSpace(correctAnswer) || string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Fields cannot be empty.");

            QuestionText = questionText;
            CorrectAnswer = correctAnswer;
            Subject = subject.ToLower(); 
            GradeLevel = gradeLevel;
        }

        public abstract string ToFileFormat();
    }

    class MultipleChoiceQuestion : Question
    {
        public List<string> Options { get; set; }

        public MultipleChoiceQuestion(string questionText, List<string> options, string correctAnswer, string subject, int gradeLevel)
            : base(questionText, correctAnswer, subject, gradeLevel)
        {
            if (options == null || options.Count < 2)
                throw new ArgumentException("Multiple-choice questions must have at least two options.");

            Options = options;
        }

        public string GetOptionByLetter(string letter)
        {
            string result = null;

            switch (letter)
            {
                case "a":
                    result = Options[0];
                    break;
                case "b":
                    result = Options[1];
                    break;
                case "c":
                    result = Options[2];
                    break;
                case "d":
                    result = Options[3];
                    break;
            }
            return result;
        }


        public override void Display(bool showAnswer = false)
        {
            Console.WriteLine("\n------------------------------------------");
            Console.WriteLine($"Question: {QuestionText}");
            Console.WriteLine("Options:");
            Console.WriteLine($"  a. {Options[0]}");
            Console.WriteLine($"  b. {Options[1]}");
            Console.WriteLine($"  c. {Options[2]}");
            Console.WriteLine($"  d. {Options[3]}");

            if (showAnswer)
            {
                Console.WriteLine($"Correct Answer: {CorrectAnswer}");
            }

            Console.WriteLine("------------------------------------------");
        }

        public override string ToFileFormat()
        {
            return $"MC|{GradeLevel}|{Subject}|{QuestionText}|{string.Join(",", Options)}|{CorrectAnswer}";
        }
    }

    class TrueFalseQuestion : Question
    {
        public TrueFalseQuestion(string questionText, string correctAnswer, string subject, int gradeLevel)
            : base(questionText, correctAnswer, subject, gradeLevel)
        {
            if (!IsValidTrueFalse(correctAnswer))
                throw new ArgumentException("Answer must be 'True' or 'False'.");
        }

        public bool IsValidTrueFalse(string input)
        {
            if (input.Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (input.Equals("False", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }


        public override void Display(bool showAnswer = false)
        {
            Console.WriteLine("\n------------------------------------------");
            Console.WriteLine($"Question: {QuestionText}");

            if (showAnswer)
            {
                Console.WriteLine($"Correct Answer: {CorrectAnswer}");
            }

            Console.WriteLine("------------------------------------------");
        }


        public override string ToFileFormat()
        {
            return $"TF|{GradeLevel}|{Subject}|{QuestionText}|{CorrectAnswer}";
        }
    }

    class IdentificationQuestion : Question
    {
        public IdentificationQuestion(string questionText, string correctAnswer, string subject, int gradeLevel)
            : base(questionText, correctAnswer, subject, gradeLevel)
        {
        }

        public override void Display(bool showAnswer = false)
        {
            Console.WriteLine("\n------------------------------------------");
            Console.WriteLine($"Question: {QuestionText}");

            if (showAnswer)
            {
                Console.WriteLine($"Correct Answer: {CorrectAnswer}");
            }

            Console.WriteLine("------------------------------------------");
        }


        public override string ToFileFormat()
        {
            return $"ID|{GradeLevel}|{Subject}|{QuestionText}|{CorrectAnswer}";
        }
    }

    class QuestionManager
    {
        private const string BasePath = "Questions";

        public static void SaveQuestion(Question question)
        {
            string folderPath = $"{BasePath}/{question.GradeLevel}/{question.Subject}";
            string filePath = $"{folderPath}/questions.txt";

            Directory.CreateDirectory(folderPath);

            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, append: true))
                {
                    sw.WriteLine(question.ToFileFormat());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving question to file: {ex.Message}");
            }
        }

        public static List<Question> LoadQuestions(string subject, int gradeLevel)
        {
            string filePath = $"{BasePath}/{gradeLevel}/{subject}/questions.txt";
            List<string> lines = FileHandler.LoadFromFile(filePath);
            List<Question> questions = new List<Question>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split('|');
                if (parts.Length < 4)
                {
                    Console.WriteLine($"Invalid line format: {line}");
                    continue;
                }

                string type = parts[0];
                string questionText = parts[3];

                string correctAnswer = parts.Length > 4 ? parts[parts.Length - 1] : null;

                if (type == "MC")
                {
                    if (parts.Length > 4)
                    {
                        List<string> options = parts[4].Split(',').ToList();
                        questions.Add(new MultipleChoiceQuestion(questionText, options, correctAnswer, subject, gradeLevel));
                    }
                    else
                    {
                        Console.WriteLine($"Missing options for MC question: {line}");
                    }
                }
                else if (type == "TF")
                {
                    if (!string.IsNullOrEmpty(correctAnswer))
                    {
                        questions.Add(new TrueFalseQuestion(questionText, correctAnswer, subject, gradeLevel));
                    }
                    else
                    {
                        Console.WriteLine($"Missing correct answer for TF question: {line}");
                    }
                }
                else if (type == "ID")
                {
                    questions.Add(new IdentificationQuestion(questionText, correctAnswer, subject, gradeLevel));
                }
                else
                {
                    Console.WriteLine($"Unknown question type: {line}");
                }
            }

            return questions;
        }

    }

    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int GradeLevel { get; set; }

        public User(string username, string password, string role, int gradeLevel = 0)
        {
            Username = username;
            Password = password;
            Role = role;
            GradeLevel = gradeLevel;
        }
    }

    class Student : User
    {
        public Student(string username, string password, string role, int gradeLevel) : base(username, password, role, gradeLevel)
        {
        }

        public void Menu()
        {
            Console.WriteLine($"Welcome, {Username}! Your Grade Level: {GradeLevel}");
            while (true)
            {
                Console.WriteLine("\n[1] Take Quiz\n[2] Logout");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Clear();
                    TakeQuiz();
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Logging out...");
                    Thread.Sleep(1500);
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
        }

        private void TakeQuiz()
        {
            SubjectMenu:
            Console.WriteLine("Select a subject for the quiz:");
            Console.WriteLine("1. Science");
            Console.WriteLine("2. Mathematics");
            Console.WriteLine("3. English");
            Console.WriteLine("4. Araling Panlipunan (Social Studies)");
            Console.WriteLine("5. Filipino");

            Console.Write("Enter the number corresponding to your choice: ");
            string subjectChoice = Console.ReadLine()?.Trim();

            string selectedSubject;
            switch (subjectChoice)
            {
                case "1":
                    selectedSubject = "Science";
                    break;
                case "2":
                    selectedSubject = "Mathematics";
                    break;
                case "3":
                    selectedSubject = "English";
                    break;
                case "4":
                    selectedSubject = "Araling Panlipunan";
                    break;
                case "5":
                    selectedSubject = "Filipino";
                    break;
                default:
                    selectedSubject = "";
                    break;
            }

            if (string.IsNullOrEmpty(selectedSubject))
            {
                Console.Clear();
                Console.WriteLine("Invalid subject choice. Please choose a valid subject.");
                goto SubjectMenu;
            }

            // Use the student's registered grade level
            int gradeLevel = GradeLevel;

            var questions = QuestionManager.LoadQuestions(selectedSubject, gradeLevel);

            if (questions.Count == 0)
            {
                Console.Clear();
                Console.WriteLine($"No questions found for {selectedSubject} at Grade {gradeLevel}.");
                return;
            }
            QuestionNo:
            Console.Write("How many questions would you like to answer? ");
            if (!int.TryParse(Console.ReadLine(), out int numberOfQuestions) || numberOfQuestions <= 0)
            {
                Console.Clear();
                Console.WriteLine("Invalid number of questions. Please enter a positive number.");
                goto QuestionNo;
            }

            // Randomly shuffle and select the desired number of questions
            var shuffledQuestions = new List<Question>(questions);
            var randomQuestions = new List<Question>();
            Random random = new Random();

            for (int i = shuffledQuestions.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = shuffledQuestions[i];
                shuffledQuestions[i] = shuffledQuestions[j];
                shuffledQuestions[j] = temp;
            }

            for (int i = 0; i < numberOfQuestions && i < shuffledQuestions.Count; i++)
            {
                randomQuestions.Add(shuffledQuestions[i]);
            }

            int score = 0;

            foreach (var question in randomQuestions)
            {
                string userAnswer = "";
                bool validAnswer = false;
                string feedback = "";

                while (!validAnswer)
                {
                    question.Display(showAnswer: false);

                    if (question is MultipleChoiceQuestion mcQuestion)
                    {
                        Console.Write("Your answer (enter a, b, c, or d): ");
                        userAnswer = Console.ReadLine()?.Trim().ToLower();

                        if (!"abcd".Contains(userAnswer))
                        {
                            Console.WriteLine("Invalid choice. Please enter a, b, c, or d.");
                            continue;
                        }

                        string selectedOption = mcQuestion.GetOptionByLetter(userAnswer);
                        Console.WriteLine($"\nYou selected: {selectedOption}");

                        // Confirmation logic
                        Console.Write("Do you want to [1] Rewrite your answer or [2] Proceed to the next question? ");
                        string confirmChoice = Console.ReadLine()?.Trim();
                        if (confirmChoice == "1")
                        {
                            Console.WriteLine("Rewriting your answer...");
                        }
                        else if (confirmChoice == "2")
                        {
                            validAnswer = true;

                            if (selectedOption != null && selectedOption.Trim().Equals(mcQuestion.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                feedback = "Correct!";
                                Console.ResetColor();
                                score++;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                feedback = $"Wrong! The correct answer was: {mcQuestion.CorrectAnswer}";
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                        }
                    }
                    else if (question is TrueFalseQuestion tfQuestion)
                    {
                        Console.Write("Your answer (True/False): ");
                        userAnswer = Console.ReadLine()?.Trim().ToLower();

                        if (userAnswer != "true" && userAnswer != "false")
                        {
                            Console.WriteLine("Invalid choice. Please enter True or False.");
                            continue;
                        }

                        Console.WriteLine($"\nYour answer: {userAnswer}");

                        // Confirmation logic
                        Console.Write("Do you want to [1] Rewrite your answer or [2] Proceed to the next question? ");
                        string confirmChoice = Console.ReadLine()?.Trim();
                        if (confirmChoice == "1")
                        {
                            Console.WriteLine("Rewriting your answer...");
                        }
                        else if (confirmChoice == "2")
                        {
                            validAnswer = true;

                            if (userAnswer.Equals(tfQuestion.CorrectAnswer.ToLower()))
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                feedback = "Correct!";
                                Console.ResetColor();
                                score++;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                feedback = $"Wrong! The correct answer was: {tfQuestion.CorrectAnswer}";
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                        }
                    }
                    else if (question is IdentificationQuestion idQuestion)
                    {
                        Console.Write("Your answer: ");
                        userAnswer = Console.ReadLine()?.Trim();

                        if (string.IsNullOrWhiteSpace(userAnswer))
                        {
                            Console.WriteLine("Answer cannot be empty.");
                            continue;
                        }

                        Console.WriteLine($"\nYour answer: {userAnswer}");

                        // Confirmation logic
                        Console.Write("Do you want to [1] Rewrite your answer or [2] Proceed to the next question? ");
                        string confirmChoice = Console.ReadLine()?.Trim();
                        if (confirmChoice == "1")
                        {
                            Console.WriteLine("Rewriting your answer...");
                        }
                        else if (confirmChoice == "2")
                        {
                            validAnswer = true;

                            if (userAnswer.Equals(idQuestion.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                feedback = "Correct!";
                                Console.ResetColor();
                                score++;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                feedback = $"Wrong! The correct answer was: {idQuestion.CorrectAnswer}";
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                        }
                    }

                    Console.Clear();
                    Console.WriteLine(feedback);
                    Console.WriteLine();
                }
            }

            Console.WriteLine($"Quiz completed! Your score: {score} out of {randomQuestions.Count}");
        }

    }


    class Admin : User
    {
        public static readonly List<string> AvailableSubjects = new List<string>
    {
        "Science", "Mathematics", "English", "Araling Panlipunan", "Filipino"
    };

        public Admin(string username, string password, string role) : base(username, password, role) { }

        public void Menu()
        {
            while (true)
            {
                Console.WriteLine("Welcome!");
                Console.WriteLine("[1] Add Question\n[2] View Questions\n[3] Delete Questions\n[4] Logout");
                Console.Write("Input your choice");
                string choice = Console.ReadLine();

                if (choice == "1")
                    AddQuestion();
                else if (choice == "2")
                    ViewQuestions();
                else if (choice == "3")
                    DeleteQuestion();
                else if (choice == "4")
                {
                    Console.WriteLine("Logging out...");
                    Thread.Sleep(1500);
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
        }

        private void AddQuestion()
        {
            Console.WriteLine("What type of question would you like to add?");
            Console.WriteLine("1. Multiple Choice");
            Console.WriteLine("2. True or False");
            Console.WriteLine("3. Identification");
            Console.Write("Enter the number corresponding to the question type: ");
            string typeChoice = Console.ReadLine()?.Trim();

            string type = "";
            switch (typeChoice)
            {
                case "1":
                    type = "MC";
                    break;
                case "2":
                    type = "TF";
                    break;
                case "3":
                    type = "ID";
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid question type.");
                    return;
            }

            Console.Write("Enter question text: ");
            string text = Console.ReadLine();

            string answer = "";
            List<string> options = new List<string>();

            if (type == "MC")
            {
                Console.Write("Enter option a: ");
                string optionA = Console.ReadLine();
                options.Add(optionA);

                Console.Write("Enter option b: ");
                string optionB = Console.ReadLine();
                options.Add(optionB);

                Console.Write("Enter option c: ");
                string optionC = Console.ReadLine();
                options.Add(optionC);

                Console.Write("Enter option d: ");
                string optionD = Console.ReadLine();
                options.Add(optionD);

                if (options.Count != 4)
                {
                    Console.WriteLine("You must provide exactly 4 options.");
                    return;
                }

                Console.Write("Enter the full correct answer (must match one of the options): ");
                answer = Console.ReadLine()?.Trim();

                if (!options.Contains(answer))
                {
                    Console.WriteLine("Invalid correct answer. It must match one of the provided options.");
                    return;
                }

                Console.WriteLine($"Question: {text}");
                Console.WriteLine("Choices:");
                for (int i = 0; i < options.Count; i++)
                {
                    Console.WriteLine($"{(char)('a' + i)}. {options[i]}");
                }
                Console.WriteLine($"Correct answer: {answer}");
            }
            else if (type == "TF")
            {
                Console.Write("Enter correct answer (True/False): ");
                answer = Console.ReadLine();

                Console.WriteLine($"Question: {text}");
                Console.WriteLine($"Correct answer: {answer}");
            }
            else if (type == "ID")
            {
                Console.Write("Enter correct answer: ");
                answer = Console.ReadLine();

                Console.WriteLine($"Question: {text}");
                Console.WriteLine($"Correct answer: {answer}");
            }

            Console.WriteLine("Available subjects: " + string.Join(", ", AvailableSubjects));
            Console.Write("Enter subject: ");
            string subject = Console.ReadLine();

            if (!AvailableSubjects.Contains(subject, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Invalid subject. Available subjects are: {string.Join(", ", AvailableSubjects)}");
                return;
            }

            Console.Write("Enter grade level (1, 2, or 3): ");
            int grade = int.Parse(Console.ReadLine());

            try
            {
                Question question;
                if (type == "MC")
                {
                    question = new MultipleChoiceQuestion(text, options, answer, subject, grade);
                }
                else if (type == "TF")
                {
                    question = new TrueFalseQuestion(text, answer, subject, grade);
                }
                else if (type == "ID")
                {
                    question = new IdentificationQuestion(text, answer, subject, grade);
                }
                else
                {
                    throw new ArgumentException("Invalid question type.");
                }

                QuestionManager.SaveQuestion(question);
                Console.WriteLine("Question added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding question: {ex.Message}");
            }
        }

        private void ViewQuestions()
        {
            Console.Clear();
            Console.WriteLine("[1] View All Questions");
            Console.WriteLine("[2] Filter Questions");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Clear();
                ViewAllQuestions();
            }
            else if (choice == "2")
            {
                Console.Clear();
                ViewFilteredQuestions();
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }

        private void ViewAllQuestions()
        {
            var allQuestions = new List<Question>();

            foreach (var subject in AvailableSubjects)
            {
                Console.WriteLine($"------------------------------------------");
                Console.WriteLine($"          {subject.ToUpper()} QUESTIONS");
                Console.WriteLine($"------------------------------------------");

                for (int gradeLevel = 1; gradeLevel <= 3; gradeLevel++)
                {
                    Console.WriteLine($"Grade Level {gradeLevel} Questions:");

                    var questions = QuestionManager.LoadQuestions(subject, gradeLevel);

                    if (questions.Count == 0)
                    {
                        Console.WriteLine($"No questions found for Grade {gradeLevel}.");
                        continue;
                    }

                    foreach (var question in questions)
                    {
                        question.Display(showAnswer: true); 
                        Console.WriteLine();
                    }
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(); 
            Console.Clear();
        }



        private void ViewFilteredQuestions()
        {
            Console.Write("Enter subject to filter: ");
            string subject = Console.ReadLine()?.Trim();

            if (!AvailableSubjects.Contains(subject, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Invalid subject. Available subjects are: {string.Join(", ", AvailableSubjects)}");
                return;
            }

            Console.Write("Enter grade level to filter (1, 2, 3): ");
            int gradeLevel;
            if (!int.TryParse(Console.ReadLine(), out gradeLevel) || gradeLevel < 1 || gradeLevel > 3)
            {
                Console.WriteLine("Invalid grade level. Please enter a grade level between 1 and 3.");
                return;
            }

            // Load questions based on subject and grade level
            var questions = QuestionManager.LoadQuestions(subject, gradeLevel);

            if (questions.Count == 0)
            {
                Console.WriteLine("No questions found for the selected subject and grade level.");
                return;
            }

            // Additional filter: Ask for question type or keyword
            Console.WriteLine("Would you like to apply additional filters?");
            Console.WriteLine("[1] Filter by question type");
            Console.WriteLine("[2] Filter by keyword");
            Console.WriteLine("[3] No additional filter");
            Console.Write("Select an option: ");
            string filterChoice = Console.ReadLine()?.Trim();

            List<Question> filteredQuestions = new List<Question>();

            if (filterChoice == "1")
            {
                Console.WriteLine("Select question type:");
                Console.WriteLine("[1] Multiple Choice");
                Console.WriteLine("[2] True/False");
                Console.WriteLine("[3] Identification");
                Console.Write("Enter your choice: ");
                string typeChoice = Console.ReadLine()?.Trim();

                foreach (var question in questions)
                {
                    if (typeChoice == "1" && question is MultipleChoiceQuestion)
                    {
                        filteredQuestions.Add(question);
                    }
                    else if (typeChoice == "2" && question is TrueFalseQuestion)
                    {
                        filteredQuestions.Add(question);
                    }
                    else if (typeChoice == "3" && question is IdentificationQuestion)
                    {
                        filteredQuestions.Add(question);
                    }
                }
            }
            else if (filterChoice == "2")
            {
                Console.Write("Enter a keyword to filter by (in the question text): ");
                string keyword = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    foreach (var question in questions)
                    {
                        if (question.QuestionText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            filteredQuestions.Add(question);
                        }
                    }
                }
            }
            else
            {
                // If no additional filters, use all questions
                filteredQuestions.AddRange(questions);
            }

            if (filteredQuestions.Count == 0)
            {
                Console.WriteLine("No questions match the selected filters.");
                return;
            }

            // Display filtered questions
            Console.Clear();
            foreach (var question in filteredQuestions)
            {
                question.Display(showAnswer: true);
                Console.WriteLine();
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            Console.Clear();
        }



        private void DeleteQuestion()
        {
            Console.WriteLine("Select subject: ");
            for (int i = 0; i < AvailableSubjects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {AvailableSubjects[i]}");
            }

            int subjectChoice = int.Parse(Console.ReadLine()) - 1;
            if (subjectChoice < 0 || subjectChoice >= AvailableSubjects.Count)
            {
                Console.WriteLine("Invalid subject choice.");
                return;
            }
            string selectedSubject = AvailableSubjects[subjectChoice];

            Console.WriteLine("Select grade level (1, 2, 3): ");
            int gradeLevel = int.Parse(Console.ReadLine());
            if (gradeLevel < 1 || gradeLevel > 3)
            {
                Console.WriteLine("Invalid grade level choice.");
                return;
            }

            var questionsToDelete = QuestionManager.LoadQuestions(selectedSubject, gradeLevel);

            if (questionsToDelete.Count == 0)
            {
                Console.WriteLine("No questions found for the selected filters.");
                return;
            }

            int pageSize = 10;
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)questionsToDelete.Count / pageSize);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Showing page {currentPage + 1} of {totalPages}\n");

                int startIndex = currentPage * pageSize;
                int endIndex = Math.Min(startIndex + pageSize, questionsToDelete.Count);

                for (int i = startIndex; i < endIndex; i++)
                {
                    Console.WriteLine($"{i + 1}. {questionsToDelete[i].QuestionText}");
                }

                // Ask user to choose an option
                Console.WriteLine("\n[1] Delete a question");
                Console.WriteLine("[2] Next page");
                Console.WriteLine("[3] Previous page");
                Console.WriteLine("[4] Quit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Enter the number of the question to delete: ");
                    int deleteChoice = int.Parse(Console.ReadLine()) - 1;

                    if (deleteChoice < startIndex || deleteChoice >= endIndex)
                    {
                        Console.WriteLine("Invalid choice.");
                        continue;
                    }
                    questionsToDelete.RemoveAt(deleteChoice);

                    string filePath = $"Questions/{gradeLevel}/{selectedSubject}/questions.txt";
                    
                    var updatedQuestions = new List<string>();

                    foreach (var question in questionsToDelete)
                    {
                        updatedQuestions.Add(question.ToFileFormat());
                    }

                    FileHandler.SaveToFile(filePath, updatedQuestions);

                    Console.WriteLine("Question deleted successfully!");
                }
                else if (choice == "2")
                {
                    if (currentPage < totalPages - 1)
                    {
                        currentPage++;
                    }
                    else
                    {
                        Console.WriteLine("You are already on the last page.");
                    }
                }
                else if (choice == "3")
                {
                    if (currentPage > 0)
                    {
                        currentPage--;
                    }
                    else
                    {
                        Console.WriteLine("You are already on the first page.");
                    }
                }
                else if (choice == "4")
                {
                    Console.Clear();
                    Console.WriteLine("Exiting the deletion process.");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }
    } //end of the line
}
