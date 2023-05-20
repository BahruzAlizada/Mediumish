using mediumish.DAL;
using mediumish.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mediumish.ViewComponents
{
	public class RelatedPostViewComponent : ViewComponent
	{
		private readonly AppDbContext _db;

        public RelatedPostViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Blog> blog = await _db.Blogs.Where(x=>!x.IsDeactive).Include(x=>x.Author).OrderByDescending(x=>x.Id).Take(3).ToListAsync();
            return View(blog);   
        }
    }
}
