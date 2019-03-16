using PBin.Models;
using PBin.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
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
        public ActionResult Home(string sts, string SearchTerms)
        {
            HomeViewModel hvm = new HomeViewModel() { sts = sts, SearchTerms = SearchTerms ?? "" };

            if (SearchTerms == null)
            {
                hvm.Posts = db.Post.Where(o => o.Public == true && o.Enabled == true && o.User.Active == true).OrderByDescending(o => o.DateCreated).Take(10).ToList();
            } else
            {
                hvm.Posts = db.Post.Where(o => o.Public == true && o.Enabled == true && o.User.Active == true && (o.Content.Contains(SearchTerms) || o.Title.Contains(SearchTerms))).OrderByDescending(o => o.DateCreated).Take(10).ToList();
            }

            return View(hvm);
        }

        [Route("Login")]
        [Route("AdminLogin")]        
        public ActionResult Login(string sts)
        {
            LoginViewModel lvm = new LoginViewModel()
            {
                sts = sts
            };

            return View(lvm);
        }

        [HttpPost]
        [Route("Login")] 
        [Route("AdminLogin")]
        public ActionResult Login(string Email, string Password)
        {
            User requestedUser = db.User.Where(o => o.Email == Email).FirstOrDefault();

            if (requestedUser == null || requestedUser.Active == false)
            {
                return RedirectToAction("Login", "Home", new { sts = "Error: Could not log in" });
            }

            string userSalt = requestedUser.Salt;          
            string providedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(Password).Concat(Encoding.UTF8.GetBytes(userSalt)).ToArray());

            string userPassword = requestedUser.Password;

            if (providedPassword.Equals(userPassword))
            {
                Session["UserId"] = requestedUser.Id;
                string fullName = requestedUser.FirstName + " " + requestedUser.LastName;

                if (fullName.Length > 12)
                {
                    if (requestedUser.FirstName.Length >= 12)
                    {
                        fullName = requestedUser.FirstName[0] + " " + requestedUser.LastName[0];
                    } else {
                        fullName = requestedUser.FirstName + " " + requestedUser.LastName[0];
                    }
                }

                Session["Username"] = fullName;
                
                List<UserRole> userRoles = db.UserRole.Where(o => o.UserId == requestedUser.Id).ToList();
                Guid adminRoleId = db.Role.Where(o => o.Role1 == "Administrator").FirstOrDefault().Id;

                if (userRoles.Select(o => o.RoleId).Contains(adminRoleId))
                {
                    Session["IsAdmin"] = true;
                } else
                {
                    Session["IsAdmin"] = false;                       
                }                              

                return RedirectToAction("Home", "Home");
            } else
            {                
                return RedirectToAction("Login", "Home", new { sts = "Error: Could not log in" });
            }            
        }    

        [Route("RemovePost")]
        public ActionResult RemovePost(Guid PostId)
        {
            Post removePost = db.Post.Where(o => o.Id == PostId).FirstOrDefault();

            removePost.Enabled = false;

            db.Entry(removePost).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            } catch (Exception ex)
            {
                return RedirectToAction("Home", "Home", new { sts = "Could not remove post" });
            }            

            return RedirectToAction("Home", "Home", new { sts = "Successfully removed post" });
        }

        //Kills session and brings user to home page
        [HttpGet]
        [Route("Logout")]
        public ActionResult Logout(string sts)
        {
            Session["UserId"] = null;
            Session["Username"] = null;
            Session["IsAdmin"] = null;

            return RedirectToAction("Home", "Home", new { sts = sts});
        }     

        [Route("CreateAccount")]        
        public ActionResult CreateAccount(CreateAccountViewModel cavm)
        {
            if (cavm.NewUser == null)
            {
                cavm.NewUser = new User();
            }            

            return View(cavm);
        }

        /* Creates a new user
         * 
         * 
         */
        [HttpPost]
        [Route("CreateAccount")]
        public ActionResult CreateAccount(User NewUser)
        {
            NewUser.Id = Guid.NewGuid();            
            NewUser.DateCreated = DateTime.Now;
            NewUser.Active = true;

            CreateAccountViewModel cavm = new CreateAccountViewModel() { NewUser = NewUser };

            if (NewUser.Email == null)
            {
                cavm.sts = "Please provide an Email Address";
                return View(cavm);
            }

            if (NewUser.Password == null)
            {
                cavm.sts = "Please provide a Password";
                return View(cavm);
            }

            if (NewUser.FirstName == null)
            {
                cavm.sts = "Please provide a First Name";
                return View(cavm);
            }

            if (NewUser.LastName == null)
            {
                cavm.sts = "Please provide a Last Name";
                return View(cavm);
            }

         
            //Creates a salt, appends it to the provided password and then hashes the two values together to create the database password hash
            NewUser.Salt = GenerateRandomCryptographicKey(256);         
            byte[] byteSaltedPassword = Encoding.UTF8.GetBytes(NewUser.Password).Concat(Encoding.UTF8.GetBytes(NewUser.Salt)).ToArray();
            NewUser.Password = Convert.ToBase64String(byteSaltedPassword);

            UserRole NewRole = new UserRole() {
                Id = Guid.NewGuid(),
                RoleId = db.Role.Where(o=>o.Role1 == "User").FirstOrDefault().Id,
                UserId = NewUser.Id
            };

            db.User.Add(NewUser);
            db.UserRole.Add(NewRole);

            try
            {
                db.SaveChanges();
            } catch (Exception ex)
            {
                //Log here
                return RedirectToAction("CreateAccount", "Home", new { sts = "Error: Could not create account"  }); 
            }
           
            return RedirectToAction("Login", "Home", new { sts = "Successfully created account." });
        }

        //https://dotnetcodr.com/2016/10/05/generate-truly-random-cryptographic-keys-using-a-random-number-generator-in-net/
        public string GenerateRandomCryptographicKey(int keyLength)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        
        [Route("CreatePost")]
        public ActionResult CreatePost()
        {
            if (!AuthorizeUser())
            {
                return RedirectToAction("Home", "Home");
            }

            return View();
        }

        [HttpPost]        
        [ValidateInput(false)]  // THIS SHOULD NOT STAY HERE REMOVE BEFORE PRODUCTION
        [Route("CreatePost")]
        public ActionResult CreatePost(Post NewPost)
        {           
            if (!AuthorizeUser())
            {
                return RedirectToAction("Home", "Home");
            }

            NewPost.Id = Guid.NewGuid();
            NewPost.DateCreated = DateTime.Now;
            NewPost.Enabled = true;
            NewPost.UserId = (Guid)Session["UserId"];
            //NewPost.Public = true;
            db.Post.Add(NewPost);

            try
            {
                db.SaveChanges();
            } catch (Exception ex)
            {
                //Throw error
                return RedirectToAction("CreatePost", "Home"); //need status report here
            }


            return RedirectToAction("UserPosts", "Home");
        }  

        [Route("UserPosts")]
        public ActionResult UserPosts(Guid UserId)
        {
            if (!AuthorizeUser())
            {
                return RedirectToAction("Home", "Home", new { sts = "Please re-authenticate"});
            }

            Guid SessionUserId = (Guid)Session["UserId"];

            if (SessionUserId != UserId)
            {
                return RedirectToAction("Home", "Home", new { sts = "Error: Could not find user posts" });
            }

            UserPostsViewModel upvm = new UserPostsViewModel()
            {
                Posts = db.Post.Where(o => o.UserId == UserId && o.Enabled == true).OrderByDescending(o=>o.DateCreated).ToList()
            };            

            return View(upvm);
        }

        [Route("EditPosts")]
        public ActionResult EditPosts(Guid PostId, string sts)
        {
            if (!AuthorizeUser())
            {
                return RedirectToAction("Home", "Home", new { sts = "Please re-authenticate" });
            }

            Guid CurrentUserId = (Guid)Session["UserId"];

            EditPostViewModel epvm = new EditPostViewModel() {
                Post = db.Post.Where(o => o.Id == PostId && o.UserId == CurrentUserId).FirstOrDefault(),
                sts = sts
            };

            if ((bool)epvm.Post.Public)
            {
                epvm.checkBoxValue = "checked";
            } else
            {
                epvm.checkBoxValue = "";
            }

            if ((bool)epvm.Post.Enabled)
            {
                epvm.postEnabled = "True";
            } else
            {
                epvm.postEnabled = "False";
            }

            return View(epvm);
        }

        [Route("EditPosts")]
        [HttpPost]
        public ActionResult EditPosts(Post Post) 
        {
            db.Entry(Post).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            } catch (Exception ex)
            {
                return RedirectToAction("EditPosts", "Home", new { PostId = Post.Id, sts = "An error occurred." });
            }
            if (!(bool)Post.Enabled)
            {
                return RedirectToAction("UserPosts", "Home", new { UserId = Post.UserId, sts = "Successfully removed post" });
            }
            return RedirectToAction("EditPosts", "Home", new { PostId = Post.Id, sts = "Successfully altered post" });
        }

        [Route("UserList")]
        public ActionResult UserList(string sts)
        {
            if (!AuthorizeAdmin())
            {
                return RedirectToAction("PageNotFound", "Home");
            }

            Guid UserRoleId = db.Role.Where(o => o.Role1 == "User").FirstOrDefault().Id;
            Guid AdminRoleId = db.Role.Where(o=>o.Role1 == "Administrator").FirstOrDefault().Id;

            UserListViewModel ulvm = new UserListViewModel()
            {
                Users = db.User.Where(o=>o.UserRole.Select(i=>i.RoleId).Contains(UserRoleId) && !o.UserRole.Select(i=>i.RoleId).Contains(AdminRoleId) && o.Active == true).ToList(),
                sts = sts
            };

            return View(ulvm);
        }

        [Route("AdminDeleteUser")]
        public ActionResult AdminDeleteUser(Guid UserId)
        {
            if (!AuthorizeAdmin())
            {
                return RedirectToAction("PageNotFound", "Home");
            }

            User userToBan = db.User.Where(o => o.Id == UserId).FirstOrDefault();

            userToBan.Active = false;

            db.Entry(userToBan).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return RedirectToAction("UserList", "Home", new { sts = "Could not remove user" });
            }

            return RedirectToAction("UserList", "Home", new { sts = "Successfully removed user" });

        }

        [Route("Settings")]
        public ActionResult Settings(string sts)
        {
            if (!AuthorizeUser())
            {
                return RedirectToAction("Home", "Home", new { sts = "Please re-authenticate" });
            }
            Guid UserId = (Guid)Session["UserId"];

            EditSettingsViewModel esvm = new EditSettingsViewModel()
            {
                sts = sts,
                User = db.User.Where(o => o.Id == UserId).FirstOrDefault()
            };
    
            return View(esvm);
        }

        [Route("EditSettings")]
        public ActionResult EditSettings(string FirstName, string LastName, Guid UserId)
        {
            if (!AuthorizeUser())
            {
                return RedirectToAction("Home", "Home", new { sts = "Please re-authenticate" });
            }

            if ((Guid)Session["UserId"] != UserId)
            {
                return RedirectToAction("Logout", "Home");
            }

            User editUserSettings = db.User.Where(o => o.Id == UserId).First();

            editUserSettings.FirstName = FirstName;
            editUserSettings.LastName = LastName;

            db.Entry(editUserSettings).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            } catch (Exception ex)
            {
                return RedirectToAction("Settings", "Home", new { sts = "Could not change settings" });
            }

            return RedirectToAction("Settings", "Home", new { sts = "Successfully changed settings" });


        }

        [Route("DeleteAccount")]
        public ActionResult DeleteAccount(Guid UserId)
        {
            if (!AuthorizeUser())
            {
                return RedirectToAction("Home", "Home", new { sts = "Please re-authenticate" });
            }

            if ((Guid)Session["UserId"] != UserId)
            {
                return RedirectToAction("Logout", "Home");
            }

            User editUserSettings = db.User.Where(o => o.Id == UserId).First();

            editUserSettings.Active = false;

            db.Entry(editUserSettings).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Settings", "Home", new { sts = "Could not delete account" });
            }

            return RedirectToAction("Logout", "Home", new { sts = "Successfully deleted account" });

        }

        [Route("PageNotFound")]
        public ActionResult PageNotFound()
        {

            return View();
        }      

        public bool AuthorizeUser()
        {           
            if (Session["UserId"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        public bool AuthorizeAdmin()
        {
            if (Session["UserId"] != null)
            {
                return ((bool)Session["IsAdmin"]);                                                    

            } else
            {
                return false;
            }
        }

    }
}