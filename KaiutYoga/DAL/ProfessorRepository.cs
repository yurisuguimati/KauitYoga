using KaiutYoga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.DAL
{
    public class ProfessorRepository
    {
        private KaiutYogaContext _context;

        public ProfessorRepository(KaiutYogaContext context)
        {
            this._context = context;
        }

        public IEnumerable<ProfessorModel> GetProfessors()
        {
            return _context.ProfessorModels.ToList();
        }

        public ProfessorModel GetProfessor(long professorId)
        {
            return _context.ProfessorModels.Where(m => m.Id == professorId).First();
        }
    }
}