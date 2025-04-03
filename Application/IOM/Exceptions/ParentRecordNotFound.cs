using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOM.Exceptions
{
    [Serializable]
    public class ParentRecordNotFoundException : Exception
    {
        public ParentRecordNotFoundException(string message) : base(message) { }

        public ParentRecordNotFoundException()
        {
        }

        public ParentRecordNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}