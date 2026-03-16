using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    public class Student : Person
    {
        public string Course { get; set; }

        public Student(int id, string name, string course)
            : base(id, name)
        {
            Course = course;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"ID: {Id}, Name: {Name}, Course: {Course}");
        }
    }
}
