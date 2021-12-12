using AutoMapper;
using ForumApplication.DTOs;
using ForumApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ForumApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly MyContext _context;
        
        public CommentController(ILogger<AccountController> logger,
        IMapper mapper, MyContext context)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        //Create comments but also check if the post is closed so you can't create.
        [HttpPost]
        [Authorize]
        [Route("Create")]
        public IActionResult CreateComment([FromBody] CommentDTO commentDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation($"Id of user attempting : {userId}");
            _logger.LogInformation($"Creation Attempt for {commentDTO.CommentText}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var post = _context.Posts.Find(commentDTO.PostId);
                if (post.IsClosed)
                {
                    return BadRequest("Post is Closed!, Can't Comment");
                }
                else
                {
                    commentDTO.OwnerId = userId;
                    var comment = _mapper.Map<Comment>(commentDTO);

                    var result = _context.Comments.AddAsync(comment);
                    _context.SaveChanges();
                    if (!result.IsCompletedSuccessfully)
                    {
                        return BadRequest("Something went wrong");
                    }
                }     
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(CreateComment)}");
                return StatusCode(500, $"Something went wrong in the {nameof(CreateComment)}!");
            }
            return Accepted();
        }


        
        //Gets comments but also checks if the post is private or not (Reads Comments)
        //Only users that have accepted the invite to a private post, can read/get comments from that post
        //Users cannot see Comments that they deleted for themselves!
        [HttpGet]
        [Authorize]
        [Route("Get/{postId}")]
        public  IActionResult GetCommentsByPost(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var DeletedForMe = _context.CommentsDeletedForMe.Where(p => p.UserId.Equals(userId)).ToList();            
            var postt = _context.Posts.Find(postId);
            

            if (postt.IsPrivate == true && !postt.UserId.Equals(userId))
            {              
                    foreach (InvitedToPost user in _context.UsersInvitedToPosts)
                    {
                        if (user.PostId == postt.Id && user.UserId.Equals(userId))
                        {
                            var commentsInPrivate = _context.Comments.Where(a => a.PostId == postId).ToList();
                            return Ok(commentsInPrivate);
                        }
                    }
                }           
            else
            {
                var commentsInPublic = _context.Comments.Where(p=>p.PostId==postt.Id).ToList();
                List<Comment> list = new List<Comment>();
                foreach (Comment c in commentsInPublic)
                {
                    foreach(DeleteForMyself dm in DeletedForMe)
                    {
                        if(c.Id != dm.CommentId && !c.OwnerId.Equals(dm.UserId))
                        {
                            list.Add(c);
                        }
                    }
                }
              
                return Ok(list);
            }

            return BadRequest();
           
        }



        //Returns the comment with most replies/reactions!
        [HttpGet]
        [Authorize]
        [Route("Get/PopularComment")]
        public IActionResult CommentWithMaxReactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comments = _context.Comments.Include(p=>p.Replies).Where(p => p.OwnerId.Equals(userId)).ToList();

            if(comments == null)
            {
                _logger.LogInformation("There is no comment by this user yet!");
                return BadRequest("No comments from this user yet!");
            }
            else
            {
                var result = comments.FirstOrDefault();
                var max = 0;

                foreach (Comment c in comments)
                {
                    if (c.Replies.Count >= max)
                    {
                        max = c.Replies.Count;
                        result = c;
                    }
                }
                return Ok(result);
            }           

        }


        //Delete for me or for everybody
        [HttpPost]
        [Authorize]
        [Route("Delete")]
        public IActionResult Delete([FromBody] DeleteCommentDTO commentDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = _context.Comments.Include(p => p.Replies)
                .Where(p => p.OwnerId.Equals(userId) && p.Id == commentDTO.CommentId).FirstOrDefault();

            if (comment == null)
            {
                _logger.LogInformation("There is no comment by this user yet!");
                return BadRequest("No comments from this user yet!");
            }
            else
            {
                try
                {
                    if(commentDTO.DeleteForMe == true)
                    {
                        DeleteForMyself dM = new DeleteForMyself
                        {
                            UserId = userId,
                            CommentId = commentDTO.CommentId
                        };
                        _context.CommentsDeletedForMe.Add(dM);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Comments.Remove(comment);
                        _context.SaveChanges();
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"Something went wrong in the {nameof(Delete)}");
                    return StatusCode(500, $"Something went wrong in the {nameof(Delete)}!");
                }
            }
            return Accepted();

        }

    }


}
