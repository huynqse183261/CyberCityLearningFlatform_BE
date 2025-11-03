using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Notifications
{
    public class NotificationResponse
    {
        public string Uid { get; set; }

        public string? SenderUid { get; set; }

        public string SenderUsername { get; set; }

        public string ReceiverUid { get; set; }

        public string ReceiverUsername { get; set; }

        public string Message { get; set; }

        public bool? IsRead { get; set; }

    }
}
