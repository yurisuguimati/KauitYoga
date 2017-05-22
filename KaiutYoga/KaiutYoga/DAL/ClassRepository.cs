using KaiutYoga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.DAL
{
    public class ClassRepository
    {
        private KaiutYogaContext _context;

        public ClassRepository (KaiutYogaContext context)
        {
            this._context = context;
        }

        public IEnumerable<ClassModel> GetClasses()
        {
            //IEnumerable<ClassModel> listclassmodel = _context.ClassModels.Include("Professor").ToList();
            return _context.ClassModels.Include("Professor").Where(c=>c.Active).ToList();
        }

        public ClassModel GetClass(long classId)
        {
            ClassModel cm = _context.ClassModels.Where(m => m.Id == classId).First();
            return cm;
        }

        public List<StudentModel> GetStudentsInClass(long classId)
        {
            List<StudentModel> students = new List<StudentModel>();
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in _context.StudentModels.Include("ReplacamentClasses").Include("Classes").Include("Plan").Where(c=>c.Active).ToList())
            {
                if (sm.Classes.ToList().Exists(i => i.Id == classId))
                    lsm.Add(sm);
                else if (sm.ReplacamentClasses.ToList().Exists(i=>i.NewClassId == classId)){
                    lsm.Add(sm);
                }
            }

            return lsm;
        }

        public List<StudentModel> GetWeeklyStudentsInClass(long classId)
        {
            List<StudentModel> students = new List<StudentModel>();
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in _context.StudentModels.Include("EnroledWeeklyClasses").Include("Classes").Include("Plan").Where(c => c.Active).ToList())
            {
                if (sm.Classes.ToList().Exists(i => i.Id == classId))
                    lsm.Add(sm);
            }

            return lsm;
        }

        public List<StudentModel> GetWeeklyStudentsInClass(long classId, DateTime date)
        {
            List<StudentModel> students = new List<StudentModel>();
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in _context.StudentModels.Include("EnroledWeeklyClasses").Include("Classes").Include("Plan").Where(c => c.Active && c.RegisterDate <= date).ToList())
            {
                if (sm.Classes.ToList().Exists(i => i.Id == classId))
                    lsm.Add(sm);
            }
            return lsm;
        }

        public List<StudentModel> GetTrialStudentsInClass(long classId)
        {
            List<StudentModel> students = new List<StudentModel>();
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in _context.StudentModels.Include("ReplacamentClasses").Include("Classes").Include("Plan").Where(c => c.Active).ToList())
            {
                if (sm.ReplacamentClasses.Where(c=>c.IsTrial).ToList().Exists(i => i.NewClassId == classId))
                    lsm.Add(sm);
            }

            return lsm;
        }

        public List<StudentModel> GetTrialStudentsInClass(long classId, DateTime date)
        {
            List<StudentModel> students = new List<StudentModel>();
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in _context.StudentModels.Include("ReplacamentClasses").Include("Classes").Include("Plan").Where(c => c.Active).ToList())
            {
                if (sm.ReplacamentClasses.Where(c => c.IsTrial && c.NewDate == date).ToList().Exists(i => i.NewClassId == classId))
                    lsm.Add(sm);
            }
            return lsm;
        }

        public List<StudentModel> GetReplacementStudentsInClass(long classId)
        {
            List<StudentModel> students = new List<StudentModel>();
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in _context.StudentModels.Include("ReplacamentClasses").Include("Classes").Include("Plan").Where(c => c.Active).ToList())
            {
                if (sm.ReplacamentClasses.Where(c => !c.IsTrial).ToList().Exists(i => i.NewClassId == classId))
                    lsm.Add(sm);
            }

            return lsm;
        }

        public List<StudentModel> GetReplacementStudentsInClass(long classId, DateTime date)
        {
            List<StudentModel> students = new List<StudentModel>();
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in _context.StudentModels.Include("ReplacamentClasses").Include("Classes").Include("Plan").Where(c => c.Active).ToList())
            {
                if (sm.ReplacamentClasses.Where(c => !c.IsTrial && c.NewDate == date).ToList().Exists(i => i.NewClassId == classId))
                    lsm.Add(sm);
            }
            return lsm;
        }

        public List<StudentModel> GetSpareStudentsInClass(long classId)
        {
            List<StudentModel> students = new List<StudentModel>();
            List<StudentModel> lsm = new List<StudentModel>();
            foreach (StudentModel sm in _context.StudentModels.Include("ReplacamentClasses").Include("Classes").Include("Plan").Where(c => c.Active).ToList())
            {
                if (sm.ReplacamentClasses.ToList().Exists(i => i.NewClassId == classId))
                    lsm.Add(sm);
            }

            return lsm;
        }

        public List<ClassModel> GetClassesByStudent(long studentId)
        {
            StudentModel sm = _context.StudentModels.Include("Classes").Where(i => i.Id == studentId).Where(c => c.Active).Single();
            return sm.Classes.ToList();
        }
    }
}