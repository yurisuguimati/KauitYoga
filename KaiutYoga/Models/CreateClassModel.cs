using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.Models
{
    public class CreateClassModel : ClassModel
    {
        [SkipProperty]
        public string FirstClass { get; set; }
        [SkipProperty]
        public string LastClass { get; set; }
        [SkipProperty]
        public int Interval { get; set; }

        public CreateClassModel() { }
        public CreateClassModel (ClassModel classmodel)
        {
            this.Day = classmodel.Day;
            this.FirstClass = "";
            this.Id = classmodel.Id;
            this.Interval = 0;
            this.LastClass = "";

            this.Professor = classmodel.Professor;
            this.ProfessorId = classmodel.ProfessorId;
            this.Room = classmodel.Room;
            this.RoomId = classmodel.RoomId;
            this.SelectedTime = classmodel.SelectedTime;
            this.Time = classmodel.Time;
        }
    }
}