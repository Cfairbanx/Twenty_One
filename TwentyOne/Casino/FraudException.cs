using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino
{
    public class FraudException : Exception             //specific exception for fraudulent activity, inherets from Exception class
    {
        public FraudException()                         //Constructor for exception, inherets from base(Exception)
            : base() { }
        public FraudException(string message)           //Overload constructor method
            : base(message) { }
    }
}
