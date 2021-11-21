using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Banking.Data.Exceptions;

namespace Banking.Data
{
    public static class DataProvider
    {
        public static BankingEntities Database;
        
        public static void AddUser(string name, string login, string password, string pin, string email)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                db.User.Add(
                    new User() { Login = login, Name = name, Password = password, Pin = pin, Email = email });
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }
        public static Tuple<int, string> GetUser(string login, string password)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                var items =
                    from p in db.User
                    where p.Login == login && p.Password == password
                    select p;

                var user = items.First();
                return Tuple.Create(user.Id, user.Name);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static Tuple<int, string> GetUserOnATM(string login, string pin)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                var items =
                    from p in db.User
                    where p.Login == login && p.Pin == pin
                    select p;

                var user = items.First();
                return Tuple.Create(user.Id, user.Name);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static User GetUserFullInfo(int id)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                var items =
                    from p in db.User.
                    Include("Account").
                    Include("Account.Currency").
                    Include("Account.TransactionFrom").
                    Include("Account.TransactionTo").
                    Include("Account.TransactionFrom.AccountFrom").
                    Include("Account.TransactionFrom.AccountTo").
                    Include("Account.TransactionTo.AccountFrom").
                    Include("Account.TransactionTo.AccountTo")
                    where p.Id == id
                    select p;

                var user = items.First();
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static List<Account> GetUserAccounts(int userId)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                var items =
                    from p in db.Account.Include("Currency")
                    where p.UserId == userId
                    select p;

                return items.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static string GetUserEmailById(int id)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                var items =
                    from p in db.User
                    where p.Id == id
                    select p.Email;

                return items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static string GetCurrencyByAccountId(int accountId)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                var items =
                    from p in db.Account
                    where p.Id == accountId
                    select p.Currency.Prefix;

                return items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static string GetCurrencyByAccountNumber(string accountNumber)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                var items =
                    from p in db.Account
                    where p.Number == accountNumber
                    select p.Currency.Prefix;

                return items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static void AddAccount(Account account)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                db.Account.Add(account);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static void AddTransaction(Transaction transaction)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                db.Transaction.Add(transaction);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static void TransferMoney(int accountFromId, string accountToNumber, decimal amount,
            decimal amountActual, string description)
        {
            var db = Database ?? new BankingEntities();
            try
            {
                db.TransferMoney(
                    accountFromId,
                    accountToNumber,
                    amount,
                    amountActual,
                    description
                    );
            }
            catch (SqlException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == Constants.ErrorFromAccountNotExist ||
                    ex.InnerException.Message == Constants.ErrorToAccountNotExist)
                    throw new AccountNotExistException();
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }

        public static List<Currency> GetCurrencies()
        {
            var db = Database ?? new BankingEntities();
            try
            {
                var items =
                    from t in db.Currency
                    select t;

                return items.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (Database == null) db.Dispose();
            }
        }        
    }
}
