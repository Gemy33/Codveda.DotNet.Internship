
using StudentManagement.Services;
using StudentManagement.Services.ViewModels;

namespace Level1_CSharp_OOP_StudentManagement_Codvead
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StudentService service = new StudentService();

            Console.WriteLine("Student Management");
            while (true)
            {
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. Show Students");
                Console.WriteLine("3. Exit");

                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Enter ID: ");
                    int id = int.Parse(Console.ReadLine());

                    Console.Write("Enter Name: ");
                    string name = Console.ReadLine();

                    Console.Write("Enter Course: ");
                    string course = Console.ReadLine();

                    var stdViewModel = new StudentViewModel(id, name, course);
                    service.AddStudent(stdViewModel);
                }

                else if (choice == "2")
                {
                    service.ShowStudents();
                }

                else if (choice == "3")
                {
                    break;
                }
            }
        }
    }
}
