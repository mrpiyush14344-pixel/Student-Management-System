using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentTrackingSystem
{
    // ─────────────────────────────────────────
    //  MODEL
    // ─────────────────────────────────────────
    class Student
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public int Year { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<double> Marks { get; set; } = new List<double>();
        public int AttendanceTotal { get; set; }
        public int AttendancePresent { get; set; }

        // Computed
        public double GPA =>
            Marks.Count == 0 ? 0 : Math.Round(Marks.Average(), 2);

        public double AttendancePercent =>
            AttendanceTotal == 0 ? 0 :
            Math.Round((double)AttendancePresent / AttendanceTotal * 100, 1);

        public string Status => GPA >= 6.0 ? "Passing" : "At Risk";
    }

    // ─────────────────────────────────────────
    //  REPOSITORY  (in-memory "database")
    // ─────────────────────────────────────────
    class StudentRepository
    {
        private List<Student> _students = new List<Student>();

        public void Add(Student s) => _students.Add(s);

        public bool Remove(string id)
        {
            var s = Find(id);
            if (s == null) return false;
            _students.Remove(s);
            return true;
        }

        public Student Find(string id) =>
            _students.FirstOrDefault(s =>
                s.StudentId.Equals(id, StringComparison.OrdinalIgnoreCase));

        public List<Student> All() => _students;

        public List<Student> Search(string query) =>
            _students.Where(s =>
                s.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.StudentId.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.Department.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();

        public bool Exists(string id) => Find(id) != null;

        // Seed demo data so the project runs immediately
        public void Seed()
        {
            _students = new List<Student>
            {
                new Student { StudentId="STU001", Name="Aarav Sharma",  Department="CS", Year=3,
                    Phone="9876543210", Email="aarav@college.edu",
                    Marks=new List<double>{85,90,78,92,88}, AttendanceTotal=100, AttendancePresent=91 },
                new Student { StudentId="STU002", Name="Priya Mehta",   Department="EC", Year=2,
                    Phone="9123456780", Email="priya@college.edu",
                    Marks=new List<double>{70,68,75,72,74}, AttendanceTotal=100, AttendancePresent=78 },
                new Student { StudentId="STU003", Name="Rohan Singh",   Department="ME", Year=1,
                    Phone="9988776655", Email="rohan@college.edu",
                    Marks=new List<double>{50,55,48,60,52}, AttendanceTotal=100, AttendancePresent=62 },
                new Student { StudentId="STU004", Name="Neha Gupta",    Department="IT", Year=4,
                    Phone="9871234567", Email="neha@college.edu",
                    Marks=new List<double>{95,91,88,93,96}, AttendanceTotal=100, AttendancePresent=95 },
                new Student { StudentId="STU005", Name="Karan Patel",   Department="CS", Year=2,
                    Phone="9765432109", Email="karan@college.edu",
                    Marks=new List<double>{65,70,68,72,66}, AttendanceTotal=100, AttendancePresent=84 },
            };
        }
    }

    // ─────────────────────────────────────────
    //  DISPLAY HELPERS
    // ─────────────────────────────────────────
    static class Display
    {
        static readonly int W = 72;

        public static void Header(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('═', W));
            Console.WriteLine($"  {title}");
            Console.WriteLine(new string('═', W));
            Console.ResetColor();
        }

        public static void TableHeader()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{"ID",-10} {"Name",-20} {"Dept",-6} {"Yr",-4} {"GPA",-6} {"Att%",-6} Status");
            Console.WriteLine(new string('─', W));
            Console.ResetColor();
        }

        public static void StudentRow(Student s)
        {
            Console.Write($"{s.StudentId,-10} {s.Name,-20} {s.Department,-6} {s.Year,-4} {s.GPA,-6} {s.AttendancePercent + "%",-6} ");
            Console.ForegroundColor = s.Status == "Passing" ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(s.Status);
            Console.ResetColor();
        }

        public static void StudentDetail(Student s)
        {
            Header($"Student Detail — {s.Name}");
            Console.WriteLine($"  ID          : {s.StudentId}");
            Console.WriteLine($"  Department  : {s.Department}");
            Console.WriteLine($"  Year        : {s.Year}");
            Console.WriteLine($"  Phone       : {s.Phone}");
            Console.WriteLine($"  Email       : {s.Email}");
            Console.WriteLine($"  Marks       : {string.Join(", ", s.Marks)}");
            Console.WriteLine($"  GPA         : {s.GPA} / 10");
            Console.WriteLine($"  Attendance  : {s.AttendancePresent}/{s.AttendanceTotal} ({s.AttendancePercent}%)");
            Console.Write($"  Status      : ");
            Console.ForegroundColor = s.Status == "Passing" ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(s.Status);
            Console.ResetColor();
        }

        public static void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ✔ " + msg);
            Console.ResetColor();
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ✘ " + msg);
            Console.ResetColor();
        }

        public static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ℹ " + msg);
            Console.ResetColor();
        }

        public static void Prompt(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("  " + text + " → ");
            Console.ResetColor();
        }
    }

    // ─────────────────────────────────────────
    //  SERVICE  (business logic)
    // ─────────────────────────────────────────
    class StudentService
    {
        private readonly StudentRepository _repo;

        public StudentService(StudentRepository repo) => _repo = repo;

        public bool AddStudent(Student s)
        {
            if (_repo.Exists(s.StudentId)) return false;
            _repo.Add(s);
            return true;
        }

        public bool DeleteStudent(string id) => _repo.Remove(id);

        public bool UpdateMarks(string id, List<double> marks)
        {
            var s = _repo.Find(id);
            if (s == null) return false;
            s.Marks = marks;
            return true;
        }

        public bool UpdateAttendance(string id, int total, int present)
        {
            var s = _repo.Find(id);
            if (s == null) return false;
            s.AttendanceTotal = total;
            s.AttendancePresent = present;
            return true;
        }

        public Student GetStudent(string id) => _repo.Find(id);

        public List<Student> GetAll() => _repo.All();

        public List<Student> Search(string q) => _repo.Search(q);

        public (double avgGpa, double avgAtt, int passing, int atRisk) Summary()
        {
            var all = _repo.All();
            if (all.Count == 0) return (0, 0, 0, 0);
            double avgGpa = Math.Round(all.Average(s => s.GPA), 2);
            double avgAtt = Math.Round(all.Average(s => s.AttendancePercent), 1);
            int passing = all.Count(s => s.Status == "Passing");
            int atRisk = all.Count(s => s.Status == "At Risk");
            return (avgGpa, avgAtt, passing, atRisk);
        }

        public List<Student> TopStudents(int n = 3) =>
            _repo.All().OrderByDescending(s => s.GPA).Take(n).ToList();

        public List<Student> AtRiskStudents() =>
            _repo.All().Where(s => s.Status == "At Risk").ToList();
    }

    // ─────────────────────────────────────────
    //  MAIN PROGRAM  (UI / Menu)
    // ─────────────────────────────────────────
    class Program
    {
        static StudentService _svc;

        static void Main(string[] args)
        {
            Console.Title = "Student Tracking System";
            var repo = new StudentRepository();
            repo.Seed();
            _svc = new StudentService(repo);

            while (true)
            {
                ShowMainMenu();
                var choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1": ViewAllStudents(); break;
                    case "2": AddStudent(); break;
                    case "3": SearchStudent(); break;
                    case "4": ViewStudent(); break;
                    case "5": UpdateMarks(); break;
                    case "6": UpdateAttendance(); break;
                    case "7": DeleteStudent(); break;
                    case "8": Reports(); break;
                    case "0":
                        Display.Info("Goodbye!");
                        return;
                    default:
                        Display.Error("Invalid choice. Try again.");
                        break;
                }
                Console.WriteLine("\n  Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        static void ShowMainMenu()
        {
            Console.Clear();
            Display.Header("STUDENT TRACKING SYSTEM  |  College Mini Project");
            Console.WriteLine("  [1] View All Students");
            Console.WriteLine("  [2] Add New Student");
            Console.WriteLine("  [3] Search Student");
            Console.WriteLine("  [4] View Student Details");
            Console.WriteLine("  [5] Update Marks");
            Console.WriteLine("  [6] Update Attendance");
            Console.WriteLine("  [7] Delete Student");
            Console.WriteLine("  [8] Reports & Analytics");
            Console.WriteLine("  [0] Exit");
            Console.WriteLine();
            Display.Prompt("Enter choice");
        }

        // ── Feature 1 ──────────────────────────
        static void ViewAllStudents()
        {
            Display.Header("All Students");
            var list = _svc.GetAll();
            if (list.Count == 0) { Display.Info("No students found."); return; }
            Display.TableHeader();
            list.ForEach(Display.StudentRow);
            Console.WriteLine($"\n  Total: {list.Count} student(s)");
        }

        // ── Feature 2 ──────────────────────────
        static void AddStudent()
        {
            Display.Header("Add New Student");

            Display.Prompt("Student ID"); var id = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(id)) { Display.Error("ID cannot be empty."); return; }

            Display.Prompt("Full Name"); var name = Console.ReadLine()?.Trim();
            Display.Prompt("Department"); var dept = Console.ReadLine()?.Trim()?.ToUpper();
            Display.Prompt("Year (1-4)"); int.TryParse(Console.ReadLine(), out int year);
            Display.Prompt("Phone"); var phone = Console.ReadLine()?.Trim();
            Display.Prompt("Email"); var email = Console.ReadLine()?.Trim();

            Display.Prompt("Marks (comma-separated, e.g. 75,80,90)");
            var marksInput = Console.ReadLine();
            var marks = marksInput?.Split(',')
                .Select(m => double.TryParse(m.Trim(), out var v) ? v : -1)
                .Where(v => v >= 0).ToList() ?? new List<double>();

            Display.Prompt("Total classes held");
            int.TryParse(Console.ReadLine(), out int total);
            Display.Prompt("Classes attended");
            int.TryParse(Console.ReadLine(), out int present);

            var student = new Student
            {
                StudentId = id,
                Name = name,
                Department = dept,
                Year = year,
                Phone = phone,
                Email = email,
                Marks = marks,
                AttendanceTotal = total,
                AttendancePresent = present
            };

            if (_svc.AddStudent(student))
                Display.Success($"Student '{name}' added successfully.");
            else
                Display.Error($"Student ID '{id}' already exists.");
        }

        // ── Feature 3 ──────────────────────────
        static void SearchStudent()
        {
            Display.Header("Search Students");
            Display.Prompt("Enter name / ID / department");
            var q = Console.ReadLine()?.Trim();
            var results = _svc.Search(q);
            if (results.Count == 0) { Display.Info("No matching students found."); return; }
            Display.TableHeader();
            results.ForEach(Display.StudentRow);
        }

        // ── Feature 4 ──────────────────────────
        static void ViewStudent()
        {
            Display.Prompt("Enter Student ID");
            var id = Console.ReadLine()?.Trim();
            var s = _svc.GetStudent(id);
            if (s == null) { Display.Error("Student not found."); return; }
            Display.StudentDetail(s);
        }

        // ── Feature 5 ──────────────────────────
        static void UpdateMarks()
        {
            Display.Header("Update Marks");
            Display.Prompt("Enter Student ID");
            var id = Console.ReadLine()?.Trim();
            var s = _svc.GetStudent(id);
            if (s == null) { Display.Error("Student not found."); return; }

            Display.Info($"Current marks: {string.Join(", ", s.Marks)} | GPA: {s.GPA}");
            Display.Prompt("Enter new marks (comma-separated)");
            var input = Console.ReadLine();
            var marks = input?.Split(',')
                .Select(m => double.TryParse(m.Trim(), out var v) ? v : -1)
                .Where(v => v >= 0).ToList() ?? new List<double>();

            if (_svc.UpdateMarks(id, marks))
                Display.Success($"Marks updated. New GPA: {_svc.GetStudent(id).GPA}");
            else
                Display.Error("Update failed.");
        }

        // ── Feature 6 ──────────────────────────
        static void UpdateAttendance()
        {
            Display.Header("Update Attendance");
            Display.Prompt("Enter Student ID");
            var id = Console.ReadLine()?.Trim();
            var s = _svc.GetStudent(id);
            if (s == null) { Display.Error("Student not found."); return; }

            Display.Info($"Current: {s.AttendancePresent}/{s.AttendanceTotal} ({s.AttendancePercent}%)");
            Display.Prompt("Total classes held"); int.TryParse(Console.ReadLine(), out int total);
            Display.Prompt("Classes attended"); int.TryParse(Console.ReadLine(), out int present);

            if (present > total) { Display.Error("Present cannot exceed total."); return; }

            if (_svc.UpdateAttendance(id, total, present))
                Display.Success($"Attendance updated. New: {present}/{total} ({Math.Round((double)present / total * 100, 1)}%)");
            else
                Display.Error("Update failed.");
        }

        // ── Feature 7 ──────────────────────────
        static void DeleteStudent()
        {
            Display.Header("Delete Student");
            Display.Prompt("Enter Student ID to delete");
            var id = Console.ReadLine()?.Trim();
            var s = _svc.GetStudent(id);
            if (s == null) { Display.Error("Student not found."); return; }

            Display.Info($"About to delete: {s.Name} ({s.StudentId})");
            Display.Prompt("Confirm? (yes/no)");
            if (Console.ReadLine()?.Trim().ToLower() == "yes")
            {
                _svc.DeleteStudent(id);
                Display.Success("Student deleted.");
            }
            else Display.Info("Deletion cancelled.");
        }

        // ── Feature 8 ──────────────────────────
        static void Reports()
        {
            Display.Header("Reports & Analytics");

            var (avgGpa, avgAtt, passing, atRisk) = _svc.Summary();
            Console.WriteLine($"  Average GPA        : {avgGpa}");
            Console.WriteLine($"  Average Attendance : {avgAtt}%");
            Console.WriteLine($"  Passing Students   : {passing}");
            Console.WriteLine($"  At-Risk Students   : {atRisk}");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ── Top 3 Students ──────────────────────");
            Console.ResetColor();
            Display.TableHeader();
            _svc.TopStudents(3).ForEach(Display.StudentRow);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ── At-Risk Students ────────────────────");
            Console.ResetColor();
            var risky = _svc.AtRiskStudents();
            if (risky.Count == 0) Display.Success("No at-risk students — great!");
            else { Display.TableHeader(); risky.ForEach(Display.StudentRow); }
        }
    }
}