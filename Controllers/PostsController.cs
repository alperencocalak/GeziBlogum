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

        public PostsController(IPostRepository postRepository, ICommentRepository commentRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
        }

        public async Task<IActionResult> Index(string tag)
        {
            var posts = _postRepository.Posts;

            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.Url == tag));
            }

            return View(new PostViewModel { Posts = await posts.ToListAsync()});
        }

        public async Task<IActionResult> Details(string url)
        {
            var result = await _postRepository.Posts
                .Include(x => x.Tags)
                .Include(x => x.Comments)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(p => p.Url == url);

            return View(result);
        }

        public async Task<IActionResult> Tag(string url)
        {
            var posts = await _postRepository.Posts.Where(p => p.Url == url).ToListAsync();

            return View("Index", new PostViewModel
            {
                Posts = posts
            });
        }

        [HttpPost]
        public JsonResult AddComment(int PostId, string Text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);

            var entity = new Comment
            {
                Text = Text,
                PublishedOn = DateTime.Now,
                PostId = PostId,
                UserId = int.Parse(userId ?? "")
            };

            _commentRepository.CreateComment(entity);

            return Json(new
            {
                username,
                Text,
                entity.PublishedOn,
                avatar
            });
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
