using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApplication.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int Limit { get; set; }
        public string Title { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public bool IsPrivate { get; set; } 
        public bool IsClosed { get; set; } 
        public ICollection<Comment> Comments { get; set; }
        public ICollection<InvitedToPost> UsersInvitedToPost { get; set; }
        public ICollection<PostEvent> PostEvents { get; set; }
    }
}
