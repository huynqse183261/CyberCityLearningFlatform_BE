using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Organizations
{
    public class CreateOrganizationRequestDto
    {
        [Required(ErrorMessage = "Tên tổ chức là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên tổ chức không được vượt quá 255 ký tự")]
        public string OrgName { get; set; }

        [StringLength(50, ErrorMessage = "Loại tổ chức không được vượt quá 50 ký tự")]
        public string OrgType { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string ContactEmail { get; set; }
    }
}
