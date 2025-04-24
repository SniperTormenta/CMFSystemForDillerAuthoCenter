using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFSystemForDillerAuthoCenter.Models
{
    public class EmailMessage
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public List<string> Attachments { get; set; }
        public bool IsRead { get; set; }
        public bool IsSent { get; set; }
        public bool IsDraft { get; set; }
    }

    public class EmailStorage
    {
        public List<EmailMessage> SentEmails { get; set; } = new List<EmailMessage>();
        public List<EmailMessage> ReceivedEmails { get; set; } = new List<EmailMessage>();
        public Dictionary<string, bool> OpenStatistics { get; set; } = new Dictionary<string, bool>();
    }
}