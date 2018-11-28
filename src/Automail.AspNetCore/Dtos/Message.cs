using System.Collections.Generic;

namespace Automail.AspNetCore.Dtos
{
    public class Message
    {
        public string Subject { get; set; }

        public Body Body { get; set; }

        public ICollection<string> To { get; set; }
        
        public ICollection<string> Cc { get; set; }
    }

    public class Body
    {
        public string Type { get; set; }

        public string Content { get; set; }
    }
}