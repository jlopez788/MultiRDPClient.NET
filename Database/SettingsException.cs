using System;

namespace Database
{
    public class DatabaseException : Exception
    {
        public enum ExceptionTypes
        {
            DUPLICATE_ENTRY
        }

        public ExceptionTypes ExceptionType { get; set; }

        public DatabaseException(ExceptionTypes exception_type)
        {
            ExceptionType = exception_type;
        }
    }
}