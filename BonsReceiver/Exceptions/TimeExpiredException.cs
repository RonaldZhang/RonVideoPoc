using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Exceptions
{

    public class TimeExpiredException : Exception
    {
        public TimeExpiredException()
        {
        }

        public TimeExpiredException(string message)
            : base(message)
        {
        }

        public TimeExpiredException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
