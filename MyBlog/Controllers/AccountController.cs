using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO; //do this
using System.Web.Security; // and this

namespace MyBlog.Controllers
{
    public class AccountController : Controller
    {
        //set up data context
        Models.BlogEntities db = new Models.BlogEntities();

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // step 3. add a httpPostedFileBase parameter
        [HttpPost]
        public ActionResult Register(Models.Registration registration, HttpPostedFileBase ImageURL)
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
                registration.ImageURL = "/content/" + filename;
            }
            //create our Membership user
            Membership.CreateUser(registration.UserName, registration.Password);
            //create our author object
            Models.Author author = new Models.Author();
            author.Name = registration.Name;
            author.ImageURL = registration.ImageURL;
            author.UserName = registration.UserName;
            //add the author to the database
            db.Authors.Add(author);
            db.SaveChanges();

            //registration complete, log in the user
            FormsAuthentication.SetAuthCookie(registration.UserName, false);

            //kick the user to the create post section
            return RedirectToAction("Index", "Post");
        }

        public ActionResult Logout()
        {
            //to log out a user do this
            FormsAuthentication.SignOut();
            //kick to login screen
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.Login login)
        {
            //see if tye are a valid user
            if (Membership.ValidateUser(login.Username, login.Password))
            {
                //credentials are gold, log them in
                FormsAuthentication.SetAuthCookie(login.Username, false);
                //kick to create post page
                return RedirectToAction("Index", "Post");
            }
            else
            {
                //bad password or user name
                ViewBag.ErrorMessage = "Invalid username and/or password";
                return View();
            }
        }

    }
}
