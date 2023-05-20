using mediumish.DAL;
using mediumish.Models;
using mediumish.ViewsModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace mediumish.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

		#region Index
		public async Task<IActionResult> Index()
		{
			ViewBag.BlogCount = await _db.Blogs.Where(x=>!x.IsDeactive).CountAsync();
			List<Blog> blogs = await _db.Blogs.Where(x => !x.IsDeactive).Include(x => x.Author).OrderByDescending(x => x.Id).Take(6).ToListAsync();
			return View(blogs);
		}
		#endregion

		#region Detail
		public async Task<IActionResult> Detail(int? id)
		{
			if (id == null)
				return NotFound();
			Blog blog = await _db.Blogs.Include(x=>x.Author).Include(x=>x.BlogTags).ThenInclude(x=>x.Tag).FirstOrDefaultAsync(x => x.Id == id);
			if (blog == null)
				return BadRequest();

			return View(blog);
		}
		#endregion

		#region Author
		public async Task<IActionResult> Author(int? id)
		{
			if (id == null)
				return NotFound();
			Author author = await _db.Authors.FirstOrDefaultAsync(x => x.Id == id);
			if (author == null)
				return BadRequest();

			List<Blog> blogs = await _db.Blogs.Where(x => !x.IsDeactive && x.AuthorId == author.Id).ToListAsync();

			AuthorVM authorVM = new AuthorVM
			{
				Author = author,
				Blogs = blogs
			};

			return View(authorVM);
		}
		#endregion

		#region LoadMore
		public async Task<IActionResult> LoadMore(int skipCount)
		{
			int blogcount = await _db.Blogs.Where(x=>!x.IsDeactive).CountAsync();
			if(blogcount <= skipCount)
				return Content("s");

			List<Blog> blogs = await _db.Blogs.Where(x => !x.IsDeactive).Include(x => x.Author).
								OrderByDescending(x => x.Id).Skip(skipCount).Take(6).ToListAsync();
			return PartialView("_loadMoreBlog", blogs);
		}
		#endregion

		#region Error
		public IActionResult Error()
		{
			return View();
		}
		#endregion
	}
}
