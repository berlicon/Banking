using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Banking.Site.Models;
using Banking.Data;
using Banking.Data.Exceptions;
using System.Xml.Linq;
using System.Globalization;
using System.Net.Mail;
using System.Threading;

namespace Banking.Site.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        }

        // GET: /Manage/Index
        public ActionResult Index()
        {
            var userFullInfo = DataProvider.GetUserFullInfo(UserId);
            return View(userFullInfo);
        }

        // GET: /Manage/AddAccount
        public ActionResult AddAccount()
        {
            ViewBag.Currencies = new SelectList(DataProvider.GetCurrencies(), "Id", "Name");
            return View();
        }

        // POST: /Manage/AddAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAccount(Account model)
        {
            model.UserId = UserId;
            model.Number = DateTime.Now.Ticks.ToString().PadRight(20, '1').Substring(0, 20);
            DataProvider.AddAccount(model);

            return RedirectToAction("Index");
        }

        private void FillAccountsList()
        {
            var accounts = DataProvider.GetUserAccounts(UserId);
            var items = accounts.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = string.Format("{0}: {1} {2}", a.Number, a.Amount.ToString("0,0.00"), a.Currency.Prefix)
            });

            ViewBag.AccountFrom = items.ToList();
        }

        // GET: /Manage/AddTransaction
        public ActionResult AddTransaction()
        {
            FillAccountsList();
            return View(new TransactionViewModel());
        }

        // POST: /Manage/AddTransaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTransaction(TransactionViewModel model)
        {
            try
            {
                var fromCurrencyPrefix = DataProvider.GetCurrencyByAccountId(model.AccountFromId);
                var toCurrencyPrefix = DataProvider.GetCurrencyByAccountNumber(model.AccountToNumber);
                var url = string.Format(Data.Constants.YahooFinanceUrl, fromCurrencyPrefix, toCurrencyPrefix);

                var doc = XDocument.Load(url);
                decimal rate = decimal.Parse(doc.Root.Element("results").
                    Elements("rate").First().Elements("Rate").First().Value,
                    CultureInfo.InvariantCulture);

                var amountActual = rate * model.Amount * (100m - Data.Constants.ChargePercent) / 100m;
                DataProvider.TransferMoney(model.AccountFromId, model.AccountToNumber,
                    model.Amount, amountActual, model.Description);

                var email = DataProvider.GetUserEmailById(UserId);

                ThreadPool.QueueUserWorkItem(o =>
                {
                    using (var client = new SmtpClient(Data.Constants.SmtpHost, Data.Constants.SmtpPort))
                    {
                        client.UseDefaultCredentials = true;                        

                        using (var mailMessage = new MailMessage(
                            Data.Constants.To, email, Data.Constants.Subject,
                                string.Format(
                                    Data.Constants.Message,
                                    model.AccountFromId,
                                    model.AccountToNumber,
                                    fromCurrencyPrefix,
                                    toCurrencyPrefix,
                                    rate,
                                    model.Amount,
                                    amountActual,
                                    model.Description)))
                        {
                            client.Send(mailMessage);
                        }
                    }
                });
            }
            catch (AccountNotExistException ex)
            {
                ModelState.AddModelError("", Data.Constants.ErrorToAccountNotExist);
                FillAccountsList();
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.Message);
                FillAccountsList();
                return View(model);
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}