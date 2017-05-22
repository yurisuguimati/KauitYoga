using KaiutYoga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.ViewModels
{
    public class StudentOverview
    {
        public StudentModel Student { get; set; }
        public PresenceModel Presence { get; set; }
    }
}