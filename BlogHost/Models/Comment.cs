using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogHost.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public ApplicationUser Author { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public Post Post{ get; set; }
    }
}
