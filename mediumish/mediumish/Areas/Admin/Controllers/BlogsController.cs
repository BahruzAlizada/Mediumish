using mediumish.DAL;
using mediumish.Helpers;
using mediumish.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace mediumish.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public BlogsController(AppDbContext db,IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            List<Blog> blogs = await _db.Blogs.Include(x=>x.Author).Include(x=>x.BlogTags).ThenInclude(x=>x.Tag).ToListAsync();
            return View(blogs);
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _db.Authors.ToListAsync();
            ViewBag.Tags = await _db.Tags.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Blog blog,int authorId, int[] tagsId)
        {
            ViewBag.Authors = await _db.Authors.ToListAsync();
            ViewBag.Tags = await _db.Tags.ToListAsync();

            #region Exist
            bool isExist = await _db.Blogs.AnyAsync(x => x.Title == blog.Title);
            if(isExist)
            {
                ModelState.AddModelError("Title", "This title already is Exist");
                return View();
            }
            #endregion

            #region Image
            if (blog.Photo == null)
            {
                ModelState.AddModelError("Photo", "Photo can not be null");
                return View();
            }
            if (!blog.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "photo can not be null");
                return View();
            }
            if (blog.Photo.IsOlder256Kb())
            {
                ModelState.AddModelError("Photo", "Max 256Kb");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "assets", "img", "blog");
            blog.Image = await blog.Photo.SaveFileAsync(folder);
            #endregion

            #region Tags
            List<BlogTag> blogTags = new List<BlogTag>();
            foreach (int tagid in tagsId)
            {
                BlogTag blogTag = new BlogTag
                {
                    TagId=tagid,
                };
                blogTags.Add(blogTag);
            }
            #endregion

            blog.AuthorId = authorId;
            blog.BlogTags = blogTags;

            await _db.Blogs.AddAsync(blog);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Authors = await _db.Authors.ToListAsync();
            ViewBag.Tags = await _db.Tags.ToListAsync();

			if (id == null)
				return NotFound();
			Blog dbblog = await _db.Blogs.Include(x => x.Author).Include(x => x.BlogTags).ThenInclude(x => x.Tag).FirstOrDefaultAsync(x => x.Id == id);
			if (dbblog == null)
				return BadRequest();

            return View(dbblog);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]

		public async Task<IActionResult> Update(int? id,Blog blog,int authorId, int[] tagsId)
		{
			ViewBag.Authors = await _db.Authors.ToListAsync();
			ViewBag.Tags = await _db.Tags.ToListAsync();

			if (id == null)
				return NotFound();
			Blog dbblog = await _db.Blogs.Include(x => x.Author).Include(x => x.BlogTags).ThenInclude(x => x.Tag).FirstOrDefaultAsync(x => x.Id == id);
			if (dbblog == null)
				return BadRequest();

            #region Exist
            bool isExist = await _db.Blogs.AnyAsync(X=>X.Title==blog.Title && X.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "This Title already is exist");
                return View();
            }
            #endregion

            #region Image
            if (blog.Photo != null)
            {
                if (!blog.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Select image type");
                    return View();
                }
                if (blog.Photo.IsOlder256Kb())
                {
                    ModelState.AddModelError("Photo", "Max 256Kb");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "img", "blog");
                blog.Image = await dbblog.Photo.SaveFileAsync(folder);
                string path = Path.Combine(_env.WebRootPath, folder, dbblog.Image);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                dbblog.Image = blog.Image;
            }
            #endregion

            #region Tags           
                List<BlogTag> blogTags = new List<BlogTag>();
                foreach (int tagid in tagsId)
                {
                    BlogTag blogTag = new BlogTag
                    {
                        TagId = tagid,
                    };
                    blogTags.Add(blogTag);
                }
                dbblog.BlogTags=blogTags;  
            #endregion

            dbblog.IsFavorite=blog.IsFavorite;
            dbblog.AuthorId = authorId;
            dbblog.Title = blog.Title;
            dbblog.Description = blog.Description;
            
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
		}
		#endregion

		#region Detail
		public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();
            Blog dbblog = await _db.Blogs.Include(x=>x.Author).Include(x=>x.BlogTags).ThenInclude(x=>x.Tag).FirstOrDefaultAsync(x => x.Id == id);
            if (dbblog == null)
                return BadRequest();

            return View(dbblog);
        }
        #endregion

        #region Activity
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
                return NotFound();
            Blog dbblog = await _db.Blogs.FirstOrDefaultAsync(x => x.Id == id);
            if (dbblog == null)
                return BadRequest();

            if (dbblog.IsDeactive)
                dbblog.IsDeactive = false;
            else
                dbblog.IsDeactive = true;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
