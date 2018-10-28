using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogHost.Models
{
    public class Like
    {
        public int Id { get; set; }
        public ApplicationUser Author { get; set; }
        public Post Post { get; set; }
    }
}
