using GeziBlogum.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeziBlogum.ViewComponents
{
    public class NewComments : ViewComponent
    {
        private readonly ICommentRepository _commentRepository;

        public NewComments(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var comments = await _commentRepository.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .Where(c => c.Post != null && c.Post.Url != null)
                .OrderByDescending(c => c.PublishedOn)
                .Take(3)
                .ToListAsync();

            return View(comments);
        }
    }
}
