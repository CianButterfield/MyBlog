using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.Models;
using System.IO;
using System.Web.Security;

namespace MyBlog.Controllers
{
    public class PostController : Controller
    {
        private BlogEntities db = new BlogEntities();

        //
        // GET: /Post/

        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Author);
            return View(posts.ToList());
        }

        //
        // GET: /Post/Details/5

        public ActionResult Details(int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //
        // GET: /Post/Create

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "Name");
            return View();
        }

        //
        // POST: /Post/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post post , HttpPostedFileBase ImageURL)
        {
            if (ImageURL != null)
            {
                //save the image to our website
                string filename = Guid.NewGuid().ToString().Substring(0, 6) + ImageURL.FileName;
                //specify the path to save the file to
                string path = Path.Combine(Server.MapPath("~/content/"), filename);
                //save the file
                ImageURL.SaveAs(path);
                //update our regisration object, with the Image
                post.ImageURL = "/content/" + filename;
            }

            post.DateCreated = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "Name", post.AuthorID);
            return View(post);
        }

        //
        // GET: /Post/Edit/5

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "Name", post.AuthorID);
            return View(post);
        }

        //
        // POST: /Post/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Post post, HttpPostedFileBase ImageURL)
        {
            if (ImageURL != null)
            {
                //save the image to our website
                string filename = Guid.NewGuid().ToString().Substring(0, 6) + ImageURL.FileName;
                //specify the path to save the file to
                string path = Path.Combine(Server.MapPath("~/content/"), filename);
                //save the file
                ImageURL.SaveAs(path);
                //update our regisration object, with the Image
                post.ImageURL = "/content/" + filename;
            }

            post.DateCreated = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "Name", post.AuthorID);
            return View(post);
        }

        //
        // GET: /Post/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //
        // POST: /Post/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}