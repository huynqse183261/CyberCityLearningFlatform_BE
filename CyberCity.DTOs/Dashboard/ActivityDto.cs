using System;

namespace CyberCity.DTOs.Dashboard
{
    public class ActivityDto
    {
        public string Type { get; set; } // user, order, enrollment, message
        public string Title { get; set; }
        public string Detail { get; set; }
        public Guid? UserUid { get; set; }
        public Guid? RelatedUid { get; set; }
        public DateTime When { get; set; }
    }
}