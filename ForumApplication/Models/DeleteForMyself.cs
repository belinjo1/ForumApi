using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApplication.Models
{
    public class DeleteForMyself
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int CommentId { get; set; }   
        public Comment Comment { get; set; }
    }
}
