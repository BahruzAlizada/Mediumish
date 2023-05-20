using mediumish.DAL;
using mediumish.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mediumish.ViewComponents
{
	public class FeaturedBlogViewComponent : ViewComponent
	{
		private readonly AppDbContext _db;

        public FeaturedBlogViewComponent(AppDbContext db)
        {
            _db = db;
        }

		public async Task<IViewComponentResult> InvokeAsync()
		{
			List<Blog> blogs = await _db.Blogs.Where(x => !x.IsDeactive && x.IsFavorite).Include(x => x.Author).OrderByDescending(x => x.Id).Take(4).ToListAsync();
			return View(blogs);

		}
	}
}
