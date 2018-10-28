using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogHost.Data;
using BlogHost.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BlogHost.Controllers
{
    [Produces("application/json")]
    [Route("Post/Show/{post_id:int}/comments")]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Comments
        [HttpGet]
        [Route("")]
        public IEnumerable<Comment> GetComment(int post_id)
        {
            Debug.WriteLine("-------------------------------------------------------------------------------");
            Debug.WriteLine("ololo");
            Debug.WriteLine("-------------------------------------------------------------------------------");
            return _context.Comments.Where(comment => comment.Post.Id == post_id).ToArray();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment([FromRoute] int id, int post_id)
        {
            Debug.WriteLine("-------------------------------------------------------------------------------");
            Debug.WriteLine("In get comment");
            Debug.WriteLine("-------------------------------------------------------------------------------");
            if (!PostExists(post_id)) { return NotFound(); }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _context.Comments.SingleOrDefaultAsync(m => (m.Id == id && m.Post.Id == post_id));

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutComment([FromRoute] int id, [FromBody] Comment comment, int post_id)
        {
            Debug.WriteLine("-------------------------------------------------------------------------------");
            Debug.WriteLine("In put comment");
            Debug.WriteLine("-------------------------------------------------------------------------------");
            if (!PostExists(post_id)) { return NotFound(); }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Comments
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> PostComment([FromForm] Comment comment, int post_id)
        {
            Debug.WriteLine("-------------------------------------------------------------------------------");
            Debug.WriteLine("In post comment");
            Debug.WriteLine("-------------------------------------------------------------------------------");
            if (!PostExists(post_id)) { return NotFound(); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            comment.Author = await _userManager.GetUserAsync(User);
            comment.Post = await _context.Posts.SingleOrDefaultAsync(m => m.Id == post_id);
            comment.LastUpdated = comment.Created = DateTime.Now;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetComment", new { id = comment.ID }, comment);
            return Redirect($"/Post/Show/{post_id}");
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment([FromRoute] int id, int post_id)
        {
            if (!PostExists(post_id)) { return NotFound(); }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _context.Comments.SingleOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
