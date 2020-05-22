using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Auth.Domain.Exceptions
{
    public class InvalidUsernameException:Exception
    {
        public InvalidUsernameException() : base()
        { }
        public InvalidUsernameException(string message) : base(message)
        { }
        public InvalidUsernameException(string message, params object[] args)
         : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
