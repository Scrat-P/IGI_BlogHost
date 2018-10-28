using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlogHost.Models.BlogViewModels
{
    public class EditBlogViewModel
    {
        public int Id { get; set; }
        public ApplicationUser Author { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
