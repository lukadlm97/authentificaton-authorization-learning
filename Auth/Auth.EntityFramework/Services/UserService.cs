using Auth.Domain.Exceptions;
using Auth.Domain.Models;
using Auth.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Auth.EntityFramework.Services
{
    public class UserService : IUserService
    {
        private readonly DesignTimeDbContextFactory _context;
        
        public UserService(DesignTimeDbContextFactory context)
        {
            _context = context;
        }


        public User Authenticate(string username, string password)
        {
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            using(ApplicationDbContext context = _context.CreateDbContext(null))
            {
                var user = context.Users.SingleOrDefault(x => x.Username.ToLower() == username.ToLower());

                if (user == null)
                    return null;

                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    return null;


                return user;
            }
            

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Invalid value");
            if (passwordHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (passwordSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using(var hmac=new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

     

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidPasswordException();
            
            using(ApplicationDbContext context = _context.CreateDbContext(null))
            {
                if(context.Users.Any(x=>x.Username == user.Username))
                {
                    throw new InvalidUsernameException();
                }
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password,out passwordHash,out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                context.Users.Add(user);
                context.SaveChanges();

                return user;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArithmeticException("Value cannt be null");

            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            using(ApplicationDbContext context = _context.CreateDbContext(null) )
            {
                IEnumerable<User> users = context.Users.ToList();

                return users;
            }
        }

        public User GetById(int id)
        {
            using (ApplicationDbContext context = _context.CreateDbContext(null))
            {
                return context.Users.Find(id);
            }
        }

        public void Update(User userParam, string password = null)
        {
            using(var context = _context.CreateDbContext(null))
            {
                var user = context.Users.Find(userParam.Id);

                if (user == null)
                    throw new Exception("User not found");

                if(userParam.Username != user.Username)
                {
                    if(context.Users.Any(x=>x.Username == user.Username))
                    {
                        throw new InvalidUsernameException("Username \""+user.Username+"\" is already taken");

                    }
                }

                user.FirstName = userParam.FirstName;
                user.LastName = userParam.LastName;
                user.Username = userParam.Username;

                // update password if it was entered
                if (!string.IsNullOrWhiteSpace(password))
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash(password, out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }

                context.Users.Update(user);
                context.SaveChanges();
            }
        }
    }
}
