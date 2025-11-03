using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public class TeacherStudentRepo : GenericRepository<TeacherStudent>
    {
        public TeacherStudentRepo() { }
        public TeacherStudentRepo(CyberCityLearningFlatFormDBContext context) => _context = context;

        public IQueryable<TeacherStudent> GetAllAsync()
        {
            return _context.TeacherStudents.AsQueryable();
        }

        public async Task<List<TeacherStudent>> GetStudentsByTeacherIdAsync(string teacherId)
        {
            return await _context.TeacherStudents
                .Include(ts => ts.StudentU)
                .Include(ts => ts.CourseU)
                .Where(ts => ts.TeacherUid == teacherId)
                .ToListAsync();
        }

        public async Task<List<TeacherStudent>> GetTeachersByStudentIdAsync(string studentId)
        {
            return await _context.TeacherStudents
                .Include(ts => ts.TeacherU)
                .Include(ts => ts.CourseU)
                .Where(ts => ts.StudentUid == studentId)
                .ToListAsync();
        }

        public async Task<TeacherStudent> GetByRelationshipAsync(string teacherId, string studentId, string courseId)
        {
            return await _context.TeacherStudents
                .Include(ts => ts.TeacherU)
                .Include(ts => ts.StudentU)
                .Include(ts => ts.CourseU)
                .FirstOrDefaultAsync(ts => ts.TeacherUid == teacherId &&
                                         ts.StudentUid == studentId &&
                                         ts.CourseUid == courseId);
        }

        public async Task<bool> RelationshipExistsAsync(string teacherId, string studentId, string courseId)
        {
            return await _context.TeacherStudents
                .AnyAsync(ts => ts.TeacherUid == teacherId &&
                              ts.StudentUid == studentId &&
                              ts.CourseUid == courseId);
        }

        public async Task<TeacherStudent> GetByIdWithDetailsAsync(string id)
        {
            return await _context.TeacherStudents
                .Include(ts => ts.TeacherU)
                .Include(ts => ts.StudentU)
                .Include(ts => ts.CourseU)
                .FirstOrDefaultAsync(ts => ts.Uid == id);
        }

        public async Task<List<TeacherStudent>> GetAllWithDetailsAsync()
        {
            return await _context.TeacherStudents
                .Include(ts => ts.TeacherU)
                .Include(ts => ts.StudentU)
                .Include(ts => ts.CourseU)
                .ToListAsync();
        }
    }
}
