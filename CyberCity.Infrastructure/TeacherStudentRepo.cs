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

        public async Task<List<TeacherStudent>> GetStudentsByTeacherIdAsync(Guid teacherId)
        {
            var teacherIdString = teacherId.ToString();
            return await _context.TeacherStudents
                .Include(ts => ts.StudentU)
                .Include(ts => ts.CourseU)
                .Where(ts => ts.TeacherUid == teacherIdString)
                .ToListAsync();
        }

        public async Task<List<TeacherStudent>> GetTeachersByStudentIdAsync(Guid studentId)
        {
            var studentIdString = studentId.ToString();
            return await _context.TeacherStudents
                .Include(ts => ts.TeacherU)
                .Include(ts => ts.CourseU)
                .Where(ts => ts.StudentUid == studentIdString)
                .ToListAsync();
        }

        public async Task<TeacherStudent> GetByRelationshipAsync(Guid teacherId, Guid studentId, Guid courseId)
        {
            var teacherIdString = teacherId.ToString();
            var studentIdString = studentId.ToString();
            var courseIdString = courseId.ToString();
            return await _context.TeacherStudents
                .Include(ts => ts.TeacherU)
                .Include(ts => ts.StudentU)
                .Include(ts => ts.CourseU)
                .FirstOrDefaultAsync(ts => ts.TeacherUid == teacherIdString &&
                                         ts.StudentUid == studentIdString &&
                                         ts.CourseUid == courseIdString);
        }

        public async Task<bool> RelationshipExistsAsync(Guid teacherId, Guid studentId, Guid courseId)
        {
            var teacherIdString = teacherId.ToString();
            var studentIdString = studentId.ToString();
            var courseIdString = courseId.ToString();
            return await _context.TeacherStudents
                .AnyAsync(ts => ts.TeacherUid == teacherIdString &&
                              ts.StudentUid == studentIdString &&
                              ts.CourseUid == courseIdString);
        }

        public async Task<TeacherStudent> GetByIdWithDetailsAsync(Guid id)
        {
            var idString = id.ToString();
            return await _context.TeacherStudents
                .Include(ts => ts.TeacherU)
                .Include(ts => ts.StudentU)
                .Include(ts => ts.CourseU)
                .FirstOrDefaultAsync(ts => ts.Uid == idString);
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
