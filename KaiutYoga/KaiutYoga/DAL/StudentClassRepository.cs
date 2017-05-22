using KaiutYoga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.DAL
{
    public class StudentClassRepository
    {
        private KaiutYogaContext _context;

        public StudentClassRepository(KaiutYogaContext context)
        {
            this._context = context;
        }

        
        public IEnumerable<StudentModel> GetStudents()
        {
            return _context.StudentModels.ToList();
        }

        public PlanModel GetPlan(int planId)
        {
            return _context.PlanModels.Where(m => m.Id == planId).Single();
        }

        
    }
}