using mediumish.DAL;
using mediumish.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mediumish.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagsController : Controller
    {
        private readonly AppDbContext _db;

        public TagsController(AppDbContext db)
        {
            _db = db;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _db.Tags.ToListAsync();
            return View(tags);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Tag tag)
        {
            #region Exist
            bool isExist = await _db.Tags.AnyAsync(x=>x.Name==tag.Name);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This tagname already is exist");
                return View();
            }
            #endregion

            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();
            Tag dbtag = await _db.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (dbtag == null)
                return BadRequest();

            return View(dbtag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(int? id,Tag tag)
        {
            if (id == null)
                return NotFound();
            Tag dbtag = await _db.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (dbtag == null)
                return BadRequest();

            #region Exist
            bool isExist = await _db.Tags.AnyAsync(x => x.Name == tag.Name && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This tagname already is exist");
                return View();
            }
            #endregion

            dbtag.Name=tag.Name;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Activity
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
                return NotFound();
            Tag dbtag = await _db.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (dbtag == null)
                return BadRequest();

            if (dbtag.IsDeactive)
                dbtag.IsDeactive = false;
            else
                dbtag.IsDeactive= true;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
