using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Banking.Site.Models;
using Banking.Data;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Facebook;

namespace Banking.Site.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private int UserId
        {
            get { return int.Parse(HttpContext.Session["UserId"].ToString()); }
            set { HttpContext.Session["UserId"] = value; }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Tuple<int, string> user;
            try
            {
                user = DataProvider.GetUser(model.Login, model.Password);
                UserId = user.Item1;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", Data.Constants.ErrorUserNotExist);
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(user.Item2, model.Password, false, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", Data.Constants.ErrorInvalidLogin);
                    return View(model);
            }
        }

        //
        // GET: /Account/LoginViaFacebook
        [AllowAnonymous]
        public ActionResult LoginViaFacebook()
        {
            string url;
            if (Request["code"] == null)
            {
                url = string.Format(
                Data.Constants.FacebookLoginUrl,
                Data.Constants.FacebookAppId,
                Request.Url.AbsoluteUri,
                Data.Constants.FacebookScope);

                Response.Redirect(url);
            }
            else {
                url = string.Format(
                Data.Constants.FacebookAuthorizeUrl,
                Data.Constants.FacebookAppId,
                Request.Url.AbsoluteUri,
                Data.Constants.FacebookScope,
                Request["code"].ToString(),
                Data.Constants.FacebookAppSecret);

                var request = WebRequest.Create(url);

                var tokens = new Dictionary<string, string>();
                using (var response = request.GetResponse())
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    var vals = reader.ReadToEnd();

                    foreach (var token in vals.Split('&'))
                    {
                        tokens.Add(token.Substring(0, token.IndexOf("=")),
                            token.Substring(token.IndexOf("=") + 1,
                            token.Length - token.IndexOf("=") - 1));
                    }

                    var accessToken = tokens["access_token"];
                    var client = new FacebookClient(accessToken);

                    dynamic me = client.Get("me");
                    string name = me.name;

                    RedirectToAction("Index", "Home");
                    return View("Register", new RegisterViewModel() { Name = name });                    
                }
            }

            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    try
                    {
                        DataProvider.AddUser(model.Name, model.Login, model.Password, model.Pin, model.Email);
                        var userDb = DataProvider.GetUser(model.Login, model.Password);
                        UserId = userDb.Item1;
                    }
                    catch (Exception ex)
                    {
                        AddErrors(new IdentityResult(Data.Constants.ErrorUserAlreadyExisted));
                        return View(model);
                    }

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            UserId = 0;
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}