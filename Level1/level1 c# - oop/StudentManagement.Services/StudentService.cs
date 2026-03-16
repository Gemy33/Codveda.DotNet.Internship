using StudentManagement.Models;
using StudentManagement.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Services
{
    public class StudentService
    {
        private List<Student> students = new List<Student>();

        public void AddStudent(StudentViewModel student)
        {
            Student newStudent = new Student(student.Id , student.Name , student.Course);
            students.Add(newStudent);
        }

        public void ShowStudents()
        {
            if (students.Count == 0)
            {
                Console.WriteLine("No students found. please Enter New Studen First!");
                return;
            }
            foreach (var student in students)
            {
                student.DisplayInfo();
            }
        }
    }
}
