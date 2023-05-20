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
    public class AuthorsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public AuthorsController(AppDbContext db,IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            List<Author> author = await _db.Authors.ToListAsync();
            return View(author);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Author author)
        {
            #region Exist
            bool isExistname = await _db.Authors.AnyAsync(x => x.Name == author.Name);
            if (isExistname)
            {
                ModelState.AddModelError("Name", "This Author name already is exist");
                return View();
            }

            bool isExistemail = await _db.Authors.AnyAsync(x => x.Email == author.Email);
            if (isExistemail)
            {
                ModelState.AddModelError("Name", "This Author Email already is exist");
                return View();
            }
            #endregion

            #region Image
            if(author.Photo==null)
            {
                author.Image = "~/assets/img/1.png";
            }
            else
            {
                if (!author.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Select image type");
                    return View();
                }
                if (author.Photo.IsOlder256Kb())
                {
                    ModelState.AddModelError("Photo", "Max 256Kb");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "img");
                author.Image = await author.Photo.SaveFileAsync(folder);
            }
            #endregion

            await _db.Authors.AddAsync(author);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();
            Author dbauthor = await _db.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (dbauthor == null)
                return BadRequest();

            return View(dbauthor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(int? id,Author author)
        {
            if (id == null)
                return NotFound();
            Author dbauthor = await _db.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (dbauthor == null)
                return BadRequest();

            #region Exist
            bool isExistname = await _db.Authors.AnyAsync(x => x.Name == author.Name && x.Id != id);
            if (isExistname)
            {
                ModelState.AddModelError("Name", "This Author name already is exist");
                return View();
            }

            bool isExistemail = await _db.Authors.AnyAsync(x => x.Email == author.Email && x.Id != id);
            if (isExistemail)
            {
                ModelState.AddModelError("Name", "This Author Email already is exist");
                return View();
            }
            #endregion

            #region Image
            if(author.Photo != null)
            {
                if (!author.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Select image type");
                    return View();
                }
                //if (author.Photo.IsOlder256Kb())
                //{
                //    ModelState.AddModelError("Photo", "Max 256Kb");
                //    return View();
                //}
                string folder = Path.Combine(_env.WebRootPath, "assets", "img");
                author.Image = await author.Photo.SaveFileAsync(folder);
                string path = Path.Combine(_env.WebRootPath, folder, dbauthor.Image);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                dbauthor.Image = author.Image;
            }
            #endregion

            dbauthor.Name = author.Name;
            dbauthor.Email=author.Email;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Activity
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
                return NotFound();
            Author dbauthor = await _db.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (dbauthor == null)
                return BadRequest();

            if (dbauthor.IsDeactive)
                dbauthor.IsDeactive = false;
            else
                dbauthor.IsDeactive = true;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
