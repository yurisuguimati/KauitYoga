using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web;

namespace KaiutYoga.Models
{
    public class RoomModel
    {
        [Key]
        public long Id { get; set; }
        [Display(Name = "Sala")]
        public string Name { get; set; }
        [Display(Name = "Capacidade")]
        public int Capacity { get; set; }
        [Display(Name = "Folga")]
        public int SpareCapacity { get; set; }
        [Display(Name = "Localização")]
        public string Location { get; set; }

        public RoomModel(){}
        public RoomModel(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        public RoomModel (RoomModel roomModel)
        {
            this.Id = roomModel.Id;
            this.Name = roomModel.Name;
        }
    }
}