using PBin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PBin.Controllers
{
    public class HomeController : Controller
    {

        PBinEntities db = new PBinEntities();
        
        [Route("Home")]
        [Route("~/", Name = "default")]
        public ActionResult Home()
        {

            return View();
        }

        [Route("Login")]
        public ActionResult Login()
        {                        

            return View();
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult Login(string Email, string Password)
        {
            User requestedUser = db.User.Where(o => o.Email == Email).FirstOrDefault();
            string userSalt = requestedUser.Salt;          
            string providedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(Password).Concat(Encoding.UTF8.GetBytes(userSalt)).ToArray());

            string userPassword = requestedUser.Password;

            if (providedPassword.Equals(userPassword))
            {
                Session["UserId"] = requestedUser.Id;
                return RedirectToAction("FrontPage", "Home");
            } else
            {
                return RedirectToAction("Login", "Home");
            }            
        }

        //Kills session and brings user to home page
        [HttpGet]
        [Route("Logout")]
        public ActionResult Logout()
        {
            Session["UserId"] = null;            

            return RedirectToAction("Home", "Home");
        }

        /* Creates a new user
         * 
         * 
         */
        [HttpPost]
        [Route("CreateUser")]
        public ActionResult CreateUser(User NewUser)
        {
            NewUser.Id = Guid.NewGuid();            
            NewUser.DateCreated = DateTime.Now;
            NewUser.Active = true;            
            
            //Creates a salt, appends it to the provided password and then hashes the two values together to create the database password hash
            NewUser.Salt = GenerateRandomCryptographicKey(256);         
            byte[] byteSaltedPassword = Encoding.UTF8.GetBytes(NewUser.Password).Concat(Encoding.UTF8.GetBytes(NewUser.Salt)).ToArray();
            NewUser.Password = Convert.ToBase64String(byteSaltedPassword);

            db.User.Add(NewUser); 
            
            try
            {
                db.SaveChanges();
            } catch (Exception ex)
            {
                //Log here
                return RedirectToAction("Login", "Home"); //Need to add status errors
            }
           
            return RedirectToAction("Login", "Home");
        }

        //https://dotnetcodr.com/2016/10/05/generate-truly-random-cryptographic-keys-using-a-random-number-generator-in-net/
        public string GenerateRandomCryptographicKey(int keyLength)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

    }
}