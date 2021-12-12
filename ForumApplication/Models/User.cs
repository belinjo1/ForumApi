using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApplication.Models
{
    public class User : IdentityUser 
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public ICollection<InvitedToPost> UsersInvitedToPost { get; set; }
        public ICollection<PostEvent> PostEvents { get; set; }
        public ICollection<DeleteForMyself> CommentsDeletedForMe { get; set; }
    }
}
