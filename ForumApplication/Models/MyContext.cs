using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApplication.Models
{
    public class MyContext : IdentityDbContext<User>
    {

        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }
        public DbSet<User> AllUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<InvitedToPost> UsersInvitedToPosts { get; set; }
        public DbSet<PostEvent> PostEvents { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<InvitedToPost>().HasKey(sc => new { sc.UserId, sc.PostId });
            builder.Entity<PostEvent>().HasKey(sc => new { sc.UserId, sc.PostId });


  
        }
     
      
    }
}
