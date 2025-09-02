using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;

namespace CyberCity.Infrastructure
{
    public class TeacherStudentRepo:GenericRepository<TeacherStudent>
    {
        public TeacherStudentRepo() { }
        public TeacherStudentRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<TeacherStudent> GetAllAsync()
        {
            return _context.TeacherStudents.AsQueryable();
        }
    }
}
