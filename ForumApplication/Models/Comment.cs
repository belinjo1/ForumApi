using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ForumApplication.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string CommentText { get; set; }
        public string OwnerId { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public  ICollection<Reply> Replies { get; set; }
        public User Owner { get; set; }

    }
}
