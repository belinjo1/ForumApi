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
    public class PostController : Controller
    {
       
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        public readonly MyContext _context;
        public PostController(ILogger<AccountController> logger,
        IMapper mapper, MyContext context)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }
        


        [HttpPost]
        [Authorize]
        [Route("Create")]
        public IActionResult CreatePost([FromBody] CreatePostDTO postDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation($"Id of user attempting : {userId}");
            _logger.LogInformation($"Creation Attempt for {postDTO.Title}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                postDTO.UserId = userId;
                var post = _mapper.Map<Post>(postDTO);

                var result = _context.Posts.AddAsync(post);
                _context.SaveChanges();
                if (!result.IsCompletedSuccessfully)
                {
                    return BadRequest("Something went wrong");
                }
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(CreatePost)}");
                return StatusCode(500, $"Something went wrong in the {nameof(CreatePost)}!");
            }
        }

        //Change the post to closed!
        [HttpPut]
        [Authorize]
        [Route("ClosePost/{postId}")]
        public IActionResult ClosePost(int postId)
        {
            _logger.LogInformation($"Close the post Attempt");
            if (postId != 0)
            { 
                var post = _context.Posts.Find(postId);
                post.IsClosed = true;
                var result = _context.Update(post);
                 _context.SaveChanges();
            }
            else
            {
                return BadRequest("Id is not given correctly!");
            }
           
            return Accepted("Updated!");
        }



        //Invite user to a post and can't invite if the post limit is achieved.

        [HttpPost]
        [Authorize]
        [Route("InviteUser")]
        public IActionResult InviteUser([FromBody] PostEvent postEvent)
        {
            _logger.LogInformation($"Attempt to Invite a user");
            var post = _context.Posts.Find(postEvent.PostId);
            var eventsList = _context.PostEvents.Where(p => p.PostId == postEvent.PostId).ToList();

            if (postEvent != null && eventsList.Count < post.Limit )
            {
                postEvent.Status = "Invited";
                _context.Add(postEvent);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest("Post Event is Null");
            }

            return Accepted("Invitation Sent!");
        }

        //Invitation must be deletet both if the status is accepted, or just invited.
        [HttpDelete]
        [Authorize]
        [Route("DeleteInvitation")]
        public IActionResult DeleteInvitation([FromBody] PostEvent removeEvent)
        {
            _logger.LogInformation($"Attempt to Remove an Invitation");
            if (removeEvent != null)
            {
                if (removeEvent.Status.Equals("Invited"))
                {
                    _context.Remove(removeEvent);
                    _context.SaveChanges();
                }
                else if(removeEvent.Status.Equals("Accepted"))
                {
                    _context.Remove(removeEvent);
                    InvitedToPost a = new InvitedToPost
                    {
                        UserId = removeEvent.UserId,
                        PostId = removeEvent.PostId
                    };
                    _context.Remove(a);
                    _context.SaveChanges();
                }
                
            }
            else
            {
                return BadRequest("removeEvent Event is Null");
            }

            return Accepted("Invitation Removed!");
        }

        //You may accept or reject invitation through this method
        //rejected invitations are deleted.
        [HttpPost]
        [Authorize]
        [Route("ManageInvitation")]
        public IActionResult ManageInvitation([FromBody] PostEvent manageInvitation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation($"Attempt to Accept/Reject an Invitation");

            if (manageInvitation != null)
            {
                if(manageInvitation.UserId.Equals(userId))
                {
                    if (manageInvitation.Status.Equals("Accept"))
                    {
                        _context.PostEvents.Update(manageInvitation);

                        InvitedToPost iPost = new InvitedToPost
                        {
                            UserId = userId,
                            PostId = manageInvitation.PostId
                        };
                        _context.UsersInvitedToPosts.Add(iPost);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.PostEvents.Remove(manageInvitation);
                        _context.SaveChanges();
                    }
                }                        
            }
            return Accepted("--->Invitation has been managed!");
        }


        //Returns all posts where logged in user is invited
        [HttpGet]
        [Authorize]
        [Route("InvitedToPostsList")]
        public async Task<ActionResult> InvitedToPostList()
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var post = await _context.PostEvents.Include(a=>a.Post).Where(p=>p.UserId.Equals(userId)).ToListAsync();       
                if (post == null) { return NotFound($"No posts that you're invited are found!"); }
                return Ok(post);
     
        }

        //Number of comments for post
        [HttpGet]
        [Authorize]
        [Route("NumberOfComments/{postId}")]
        public IActionResult GetNumberOfComments(int postId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _context.Posts.Find(postId);
            
            if(post.Id==postId && post.UserId.Equals(userId))
            {
                var CommentsSum = _context.Comments.Where(p => p.PostId == postId).Count();
                return Ok(CommentsSum);
            }

            return BadRequest();

        }
        //Returns the number of events a post has!
        [HttpGet]
        [Authorize]
        [Route("Get/postStatuses/{postId}")]
        public IActionResult GetPostStatuses(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = _context.Posts.Find(postId);
            if (post.UserId.Equals(userId))
            {
                var postEvents = _context.PostEvents.Where(p => p.PostId == postId).Count();
                return Ok($"Number of postEvents for this post:{postEvents}");
            }
            else
            {
                return Unauthorized();
            }


        }


        //Returns the first user commented depending on postId
        [HttpGet]
        [Authorize]
        [Route("Get/FirstUserCommented/{postId}")]
        public IActionResult FirstUserCommented(int postId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _context.Posts.Where(p=>p.UserId.Equals(userId)).First();

            if (post != null && post.UserId.Equals(userId))
            {
                var FirstComment = _context.Comments.Where(p => p.PostId == postId).First();
                var user = _context.Users.Where(p => p.Id.Equals(FirstComment.OwnerId)).FirstOrDefault();
                return Ok(user);
                
            }
            return BadRequest();

        }


        //Returns the last user commented depending on postId
        [HttpGet]
        [Authorize]
        [Route("Get/LastUserCommented/{postId}")]
        public IActionResult LastUserCommented(int postId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _context.Posts.Where(p => p.UserId.Equals(userId)).First();

            if (post != null && post.UserId.Equals(userId))
            {
                var comments = _context.Comments.Where(p => p.PostId == postId).ToList();
                var LastComment = comments.Last();
                var user = _context.Users.Where(p => p.Id.Equals(LastComment.OwnerId)).FirstOrDefault();
                return Ok(user);

            }
            return BadRequest();

        }

    }
}
