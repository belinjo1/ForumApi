using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ForumApplication.Models
{
    //Reply as "React" to the comment
    public class Reply
    {
        [Key]
        public int Id { get; set; }
        public string ReplyText { get; set; }
        public int CommentId { get; set; }
        public string OwnerId { get; set; }

        [JsonIgnore]
        public Comment Comment { get; set; }
        public User Owner { get; set; }
    }
}
