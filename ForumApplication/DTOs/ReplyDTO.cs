using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApplication.DTOs
{
    public class ReplyDTO : CreateReplyDTO
    {
        public int Id { get; set; }   
     
    }
    public class CreateReplyDTO
    {
        [Required]
        public string ReplyText { get; set; }
        public int CommentId { get; set; }
        public string OwnerId { get; set; }
    }
}
