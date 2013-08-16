using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Models
{
    public class Error
    {
        public string ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }

        public Error(string errorCode, string errorMessage)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }
    }
}
