using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain.Entities
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string AuthorName { get; set; }
        public DateTime PublishedDate { get; set; }
        public int ViewCount { get; set; }
        public bool IsPublished { get; set; }

        public BlogCategory Category { get; set; }
    }
}
