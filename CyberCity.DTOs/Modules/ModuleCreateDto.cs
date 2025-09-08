using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Modules
{
    public class ModuleCreateDto
    {
        public Guid CourseUid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? OrderIndex { get; set; }
    }
}
