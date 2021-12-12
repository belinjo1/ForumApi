using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApplication.DTOs
{
    public class PostDTO : CreatePostDTO
    {
        public int Id { get; set; }
        
        public ICollection<CommentDTO> Comments { get; set; }
        public ICollection<UserDTO> UsersInvited { get; set; }

    }
    public class CreatePostDTO
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Title must have at least 2 letters")]
        public string Title { get; set; }
        public string UserId { get; set; }
        [Required]
        public bool IsPrivate { get; set; }
        public bool IsClosed { get; set; } = false;
    }
}
