using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Organizations
{
    public class OrganizationDTO
    {
        public Guid Uid { get; set; }
        public string OrgName { get; set; }
        public string OrgType { get; set; }
        public string ContactEmail { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int MemberCount { get; set; }
    }
}
