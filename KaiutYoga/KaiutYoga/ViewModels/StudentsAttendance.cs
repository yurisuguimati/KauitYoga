using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.ViewModels
{
    public class StudentsAttendance
    {
        public long StudentId { get; set; }
        public string Name { get; set; }
        public bool Presence { get; set; }
    }
}