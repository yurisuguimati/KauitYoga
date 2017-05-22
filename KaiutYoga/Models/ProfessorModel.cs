using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KaiutYoga.Models
{
    public class ProfessorModel
    {
        [Key]
        public long Id { get; set; }
        [Display(Name = "Professor")]
        public string Name { get; set; }   

        public ProfessorModel(){}
        public ProfessorModel(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        public ProfessorModel(ProfessorModel professorModel)
        {
            this.Id = professorModel.Id;
            this.Name = professorModel.Name;
        }
    }
}