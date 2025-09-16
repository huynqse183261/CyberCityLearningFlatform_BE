using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Notifications
{
    public class NotificationRequest
    {
        public Guid ReceiverUid { get; set; }
        public Guid SenderUid { get; set; }
        public string Message { get; set; }
    }
}
