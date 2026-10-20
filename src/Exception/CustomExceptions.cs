using System;

namespace QuanLyBanDoGomSu.Exception
{
    public class NotFoundException : System.Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ValidationCustomException : System.Exception
    {
        public ValidationCustomException(string message) : base(message) { }
    }

    public class UnauthorizedCustomException : System.Exception
    {
        public UnauthorizedCustomException(string message) : base(message) { }
    }

    public class BusinessRuleException : System.Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
