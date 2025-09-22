using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Organizations
{
    public class UpdateMemberRoleRequestDto
    {
        [Required(ErrorMessage = "Vai trò thành viên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Vai trò thành viên không được vượt quá 50 ký tự")]
        public string MemberRole { get; set; }
    }
}
