using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.Models
{
    public class User:DomainObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
