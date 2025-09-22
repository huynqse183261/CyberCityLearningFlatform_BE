using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Organizations
{
    public class OrganizationMemberDTO
    {
        public Guid Uid { get; set; }
        public Guid OrgUid { get; set; }
        public Guid UserUid { get; set; }
        public string MemberRole { get; set; }
        public DateTime? JoinedAt { get; set; }
        
        // Thông tin user
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserUsername { get; set; }
        public string UserImage { get; set; }
    }
}
