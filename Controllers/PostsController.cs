using System.ComponentModel;
using System.Security.Claims;
using System.Threading.Tasks;
using GeziBlogum.Data.Abstract;
using GeziBlogum.Entity;
using GeziBlogum.Models;
using GeziBlogum.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeziBlogum.Controllers
{
    public class PostsController : Controller
    {
        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;
        private ITagRepository _tagRepository;
        private ICommentVoteRepository _commentVoteRepository;

        public PostsController(IPostRepository postRepository, ICommentRepository commentRepository, ITagRepository tagRepository, ICommentVoteRepository commentVoteRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _commentVoteRepository = commentVoteRepository;
        }

        public async Task<IActionResult> Index(string tag)
        {
            var posts = _postRepository.Posts;

            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.Url == tag));
            }

            return View(new PostViewModel { Posts = await posts.ToListAsync() });
        }

        public async Task<IActionResult> Details(string url)
        {
            var result = await _postRepository.Posts
                .Include(x => x.Tags)
                .Include(x => x.Comments)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(p => p.Url == url);

            if (result != null)
            {
                result.ViewCount++;
                _postRepository.EditPost(result);
            }

            return View(result);
        }

        public async Task<IActionResult> Tag(int id)
        {
            var posts = _postRepository.Posts
                .Include(p => p.Tags)
                .Include(p => p.User)
                .Where(p => p.Tags.Any(t => t.TagId == id) && p.IsActive);

            ViewBag.SelectedTagId = id;

            return View("Index", new PostViewModel
            {
                Posts = await posts.ToListAsync()
            });
        }

        [HttpPost]
        public JsonResult AddComment([FromBody] CommentInputModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);

            var entity = new Comment
            {
                Text = model.Text,
                PublishedOn = DateTime.Now,
                PostId = model.PostId,
                UserId = int.Parse(userId ?? "0")
            };

            _commentRepository.CreateComment(entity);

            return Json(new
            {
                commentId = entity.CommentId,
                username,
                model.Text,
                entity.PublishedOn,
                avatar
            });
        }

        [HttpPost]
        public IActionResult LikeComment([FromBody] int commentId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Json(new { success = false, message = "Kullanıcı doğrulanamadı." });

            int userId = int.Parse(userIdStr);

            if (_commentVoteRepository.HasUserVoted(commentId, userId))
            {
                return Json(new { success = false, message = "Bu yoruma daha önce oy verdiniz." });
            }

            var comment = _commentRepository.Comments.FirstOrDefault(c => c.CommentId == commentId);
            if (comment == null)
            {
                return Json(new { success = false, message = "Yorum bulunamadı." });
            }

            comment.LikeCount++;
            _commentRepository.UpdateComment(comment);

            _commentVoteRepository.AddVote(new CommentVote
            {
                CommentId = commentId,
                UserId = userId,
                IsLike = true
            });

            return Json(new { success = true, likeCount = comment.LikeCount });
        }

        [HttpPost]
        public IActionResult DislikeComment([FromBody] int commentId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Json(new { success = false, message = "Kullanıcı doğrulanamadı." });

            int userId = int.Parse(userIdStr);

            if (_commentVoteRepository.HasUserVoted(commentId, userId))
            {
                return Json(new { success = false, message = "Bu yoruma daha önce oy verdiniz." });
            }

            var comment = _commentRepository.Comments.FirstOrDefault(c => c.CommentId == commentId);
            if (comment == null)
            {
                return Json(new { success = false, message = "Yorum bulunamadı." });
            }

            comment.DislikeCount++;
            _commentRepository.UpdateComment(comment);

            _commentVoteRepository.AddVote(new CommentVote
            {
                CommentId = commentId,
                UserId = userId,
                IsLike = false
            });

            return Json(new { success = true, dislikeCount = comment.DislikeCount });
        }


        [Authorize]
        public IActionResult Create()
        {
            var model = new PostCreateViewModel
            {
                Tags = _tagRepository.Tags.ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Tags = _tagRepository.Tags.ToList();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? imagePath = null;

            if (model.ImageFile != null)
            {
                imagePath = await FileHelper.FileLoaderAsync(model.ImageFile);
            }

            var tag = _tagRepository.Tags.FirstOrDefault(t => t.TagId == model.SelectedTagId);

            _postRepository.CreatePost(new Post
            {
                Title = model.Title,
                Description = model.Description,
                Content = model.Content,
                Url = model.Url,
                UserId = int.Parse(userId ?? "0"),
                PublishedOn = DateTime.Now,
                Image = imagePath,
                IsActive = false,
                Tags = tag != null ? new List<Tag> { tag } : new List<Tag>()
            });

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            _postRepository.DeletePost(id);
            TempData["SuccessMessage"] = "Post başarıyla silindi.";
            return RedirectToAction("List");
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var posts = _postRepository.Posts;

            if (string.IsNullOrEmpty(role))
            {
                posts = posts.Where(i => i.UserId == userId);
            }

            return View(await posts.ToListAsync());
        }

        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = _postRepository.Posts.FirstOrDefault(i => i.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(new PostUpdateViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Url = post.Url,
                IsActive = post.IsActive
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(PostUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var post = new Post
            {
                PostId = model.PostId,
                Title = model.Title,
                Description = model.Description,
                Content = model.Content,
                Url = model.Url,
                IsActive = model.IsActive
            };

            _postRepository.EditPost(post);
            TempData["SuccessMessage"] = "Post başarıyla güncellendi.";
            return RedirectToAction("List");
        }
    }
}