using AutoMapper;
using ForumApplication.DTOs;
using ForumApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class ReplyController : Controller
    {
       
        private readonly ILogger<ReplyController> _logger;
        private readonly IMapper _mapper;
        private readonly MyContext _context;

        public ReplyController(ILogger<ReplyController> logger,
        IMapper mapper, MyContext context)
        {
          
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [Route("Create")]
        public IActionResult CreateReply([FromBody] CreateReplyDTO replyDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = _context.Comments.Find(replyDTO.CommentId);
            var postId = comment.PostId;
            var postClosed = _context.Posts.Find(postId).IsClosed;
            var postPrivate = _context.Posts.Find(postId).IsPrivate;
            var postOwner = _context.Posts.Find(postId).UserId;
            var personInvited = _context.UsersInvitedToPosts.Where(p => p.UserId.Equals(userId) && p.PostId == postId).FirstOrDefault();

            try
            {
                if (userId.Equals(postOwner))
                {
                    replyDTO.OwnerId = userId;
                    var reply = _mapper.Map<Reply>(replyDTO);

                    var result = _context.Replies.AddAsync(reply);
                    _context.SaveChanges();
                    if (!result.IsCompletedSuccessfully)
                    {
                        return BadRequest("Something went wrong");
                    }
                    return Accepted();
                }
                else if (postClosed != true && postPrivate != true)
                {
                    replyDTO.OwnerId = userId;
                    var reply = _mapper.Map<Reply>(replyDTO);

                    var result = _context.Replies.AddAsync(reply);
                    _context.SaveChanges();
                    if (!result.IsCompletedSuccessfully)
                    {
                        return BadRequest("Something went wrong");
                    }
                    return Accepted();

                }
                else if (postPrivate == true && personInvited != null)
                {
                    replyDTO.OwnerId = userId;
                    var reply = _mapper.Map<Reply>(replyDTO);

                    var result = _context.Replies.AddAsync(reply);
                    _context.SaveChanges();
                    if (!result.IsCompletedSuccessfully)
                    {
                        return BadRequest("Something went wrong");
                    }

                    else
                    {
                        _logger.LogInformation("--->Sorry the post is Closed!..");
                        return BadRequest();
                    }
                }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Something went wrong in the {nameof(CreateReply)}");
                    return StatusCode(500, $"Something went wrong in the {nameof(CreateReply)}!");
                }
            return Unauthorized();
                     
        }
          

    }
}
