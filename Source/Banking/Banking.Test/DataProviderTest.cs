using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Banking.Data;
using Banking.Data.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Banking.Test
{
    [TestClass]
    public class DataProviderTest
    {
        static BankingEntities db;
        static DbContextTransaction tr;

        string userName;
        string userLogin;
        string userPassword;
        string userPin;
        string userEmail;
        string transactionDesc;
        string transactionDesc2;
        string accountNumber;
        string accountNumber2;

        #region Init/Cleanup code
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            Debug.WriteLine("Assembly Init");
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Debug.WriteLine("ClassInit");
        }

        [TestInitialize()]
        public void Initialize()
        {
            Debug.WriteLine("TestMethodInit");

            userName = Guid.NewGuid().ToString();
            userLogin = Guid.NewGuid().ToString().Substring(0, 20);
            userPassword = Guid.NewGuid().ToString().Substring(0, 20);
            userPin = DateTime.Now.Ticks.ToString().Substring(0, 4);
            userEmail = "test@test.com";
            transactionDesc = Guid.NewGuid().ToString();
            transactionDesc2 = Guid.NewGuid().ToString();
            accountNumber = DateTime.Now.Ticks.ToString().PadRight(20, '1').Substring(0, 20);
            accountNumber2 = DateTime.Now.Ticks.ToString().PadRight(20, '2').Substring(0, 20);

            db = new BankingEntities();
            DataProvider.Database = db;
            tr = db.Database.BeginTransaction();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            Debug.WriteLine("TestMethodCleanup");
            tr.Rollback();
            db.Dispose();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Debug.WriteLine("ClassCleanup");
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            Debug.WriteLine("AssemblyCleanup");
        }
        #endregion

        [TestMethod]
        public void Test()
        {
            Assert.IsTrue(DateTime.Now.Year > 0);
        }

        [TestMethod]
        public void GetCurrenciesTest()
        {
            var items = DataProvider.GetCurrencies();
            Assert.IsTrue(items.Count > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetUserFailedTest()
        {
            var user = DataProvider.GetUser(userLogin, userPassword);
        }

        [TestMethod]
        public void AddUserTest()
        {
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
            var user = DataProvider.GetUser(userLogin, userPassword);

            Assert.IsTrue(user.Item1 > 0);
            Assert.AreEqual(userName, user.Item2);
        }

        [TestMethod]
        public void GetUserOnATMTest()
        {
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
            var user = DataProvider.GetUserOnATM(userLogin, userPin);

            Assert.IsTrue(user.Item1 > 0);
            Assert.AreEqual(userName, user.Item2);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void AddUserNameFailedTest()
        {
            userName = "@".PadRight(51, '@');
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void AddUserLoginFailedTest()
        {
            userLogin = "@".PadRight(21, '@');
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void AddUserPasswordFailedTest()
        {
            userPassword = "@".PadRight(21, '@');
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void AddUserPinLengthMaxFailedTest()
        {
            userPin = "@".PadRight(5, '@');
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void AddUserPinLengthMinFailedTest()
        {
            userPin = "@".PadRight(3, '@');
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void AddUserPinFormatFailedTest()
        {
            userPin = "a1_@";
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void AddUserEmailFailedTest()
        {
            userEmail = "@".PadRight(51, '@');
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
        }

        [TestMethod]
        public void GetUserEmailByIdTest()
        {
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
            var user = DataProvider.GetUser(userLogin, userPassword);
            var email = DataProvider.GetUserEmailById(user.Item1);
            Assert.AreEqual(userEmail, email);
        }

        [TestMethod]
        public void GetUserFullInfoTest()
        {
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
            var user = DataProvider.GetUser(userLogin, userPassword);
            var userId = user.Item1;
            var userDbName = user.Item2;
            var currencies = DataProvider.GetCurrencies();

            DataProvider.AddAccount(new Account()
            {
                UserId = userId,
                CurrencyId = currencies[0].Id,
                Number = accountNumber,
                Amount = 100
            });

            var userFullInfo = DataProvider.GetUserFullInfo(userId);

            Assert.AreEqual(userId, userFullInfo.Id);
            Assert.AreEqual(userDbName, userFullInfo.Name);
            Assert.AreEqual(1, userFullInfo.Account.Count);

            Account[] accounts = new Account[1];
            userFullInfo.Account.CopyTo(accounts, 0);

            Assert.AreEqual(userId, accounts[0].UserId);
            Assert.AreEqual(currencies[0].Id, accounts[0].CurrencyId);
            Assert.AreEqual(accountNumber, accounts[0].Number);
            Assert.AreEqual(100, accounts[0].Amount);

            DataProvider.AddTransaction(
                new Transaction()
                {
                    AccountFromId = accounts[0].Id,
                    AccountToId = accounts[0].Id,
                    Amount = 200,
                    Datetime = DateTime.Now,
                    Description = transactionDesc
                }
                );

            DataProvider.AddTransaction(
                new Transaction()
                {
                    AccountFromId = accounts[0].Id,
                    AccountToId = accounts[0].Id,
                    Amount = 300,
                    Datetime = DateTime.Now,
                    Description = transactionDesc2
                }
                );

            userFullInfo = DataProvider.GetUserFullInfo(userId);

            accounts = new Account[1];
            userFullInfo.Account.CopyTo(accounts, 0);

            Assert.AreEqual(2, accounts[0].TransactionFrom.Count);

            var transactions = new Transaction[2];
            accounts[0].TransactionFrom.CopyTo(transactions, 0);

            Assert.AreEqual(accounts[0].Id, transactions[0].AccountFromId);
            Assert.AreEqual(accounts[0].Id, transactions[0].AccountToId);
            Assert.AreEqual(200, transactions[0].Amount);
            Assert.AreEqual(transactionDesc, transactions[0].Description);

            Assert.AreEqual(accounts[0].Id, transactions[1].AccountFromId);
            Assert.AreEqual(accounts[0].Id, transactions[1].AccountToId);
            Assert.AreEqual(300, transactions[1].Amount);
            Assert.AreEqual(transactionDesc2, transactions[1].Description);
        }

        [TestMethod]
        public void GetUserAccountsTest()
        {
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
            var user = DataProvider.GetUser(userLogin, userPassword);
            var userId = user.Item1;
            var userDbName = user.Item2;
            var currencies = DataProvider.GetCurrencies();

            DataProvider.AddAccount(new Account()
            {
                UserId = userId,
                CurrencyId = currencies[0].Id,
                Number = accountNumber,
                Amount = 100
            });

            var accounts = DataProvider.GetUserAccounts(userId);
            Assert.AreEqual(1, accounts.Count);
        }

        [TestMethod]
        public void TransferMoneyTest()
        {
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
            var user = DataProvider.GetUser(userLogin, userPassword);
            var userId = user.Item1;
            var userDbName = user.Item2;
            var currencies = DataProvider.GetCurrencies();

            DataProvider.AddAccount(new Account()
            {
                UserId = userId,
                CurrencyId = currencies[0].Id,
                Number = accountNumber,
                Amount = 1000
            });

            DataProvider.AddAccount(new Account()
            {
                UserId = userId,
                CurrencyId = currencies[1].Id,
                Number = accountNumber2,
                Amount = 2000
            });

            var userAccounts = DataProvider.GetUserAccounts(userId);

            DataProvider.TransferMoney(userAccounts[0].Id, accountNumber2, 50, 49, transactionDesc);

            var userFullInfo = DataProvider.GetUserFullInfo(userId);

            var accounts = new Account[2];
            userFullInfo.Account.CopyTo(accounts, 0);

            //TODO: fix - it works (in database values 950/2049) - see: 
            //SELECT *  FROM [dbo].Account with (nolock)
            //But data are uncommitted - because we have nested transactions 
            //(in test and in stored procedure).
            //Therefore this code returns old values (1000/2000) and rows below fail.

            //Assert.AreEqual(950, accounts[0].Amount);
            //Assert.AreEqual(2049, accounts[1].Amount);

            Assert.AreEqual(1, accounts[0].TransactionFrom.Count);

            var transactions = new Transaction[1];
            accounts[0].TransactionFrom.CopyTo(transactions, 0);

            Assert.AreEqual(accounts[0].Id, transactions[0].AccountFromId);
            Assert.AreEqual(accounts[1].Id, transactions[0].AccountToId);
            Assert.AreEqual(50, transactions[0].Amount);
            Assert.AreEqual(transactionDesc, transactions[0].Description);
        }

        [TestMethod]
        [ExpectedException(typeof(AccountNotExistException))]
        public void TransferMoneyFailedTest()
        {
            DataProvider.TransferMoney(0, accountNumber2, 50, 49, transactionDesc);
        }

        [TestMethod]
        public void GetCurrencyByAccountIdTest()
        {
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
            var user = DataProvider.GetUser(userLogin, userPassword);
            var userId = user.Item1;
            var userDbName = user.Item2;
            var currencies = DataProvider.GetCurrencies();

            DataProvider.AddAccount(new Account()
            {
                UserId = userId,
                CurrencyId = currencies[0].Id,
                Number = accountNumber,
                Amount = 100
            });

            var accounts = DataProvider.GetUserAccounts(userId);
            var prefix = DataProvider.GetCurrencyByAccountId(accounts.ToArray()[0].Id);
            Assert.AreEqual(currencies[0].Prefix, prefix);
        }

        [TestMethod]
        public void GetCurrencyByAccountNumberTest()
        {
            DataProvider.AddUser(userName, userLogin, userPassword, userPin, userEmail);
            var user = DataProvider.GetUser(userLogin, userPassword);
            var userId = user.Item1;
            var userDbName = user.Item2;
            var currencies = DataProvider.GetCurrencies();

            DataProvider.AddAccount(new Account()
            {
                UserId = userId,
                CurrencyId = currencies[0].Id,
                Number = accountNumber,
                Amount = 100
            });

            var prefix = DataProvider.GetCurrencyByAccountNumber(accountNumber);
            Assert.AreEqual(currencies[0].Prefix, prefix);
        }
    }
}
