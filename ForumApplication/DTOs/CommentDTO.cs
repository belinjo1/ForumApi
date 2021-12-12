using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApplication.DTOs
{
    public class CommentDTO : CreateCommentDTO
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public ICollection<ReplyDTO> Replies { get; set; }
    }

    public class CreateCommentDTO
    {
        [Required]
        public string CommentText { get; set; }    
        public int PostId { get; set; }
    }
}
