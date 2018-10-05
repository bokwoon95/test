using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ADMClass
{
    /// <summary>
    /// Validates employees to ensure their profiles meet business rules and standards
    /// </summary>
    public static class Class1
    {
        /// <summary>
        /// Validates the specified pay rate
        /// </summary>
        public static bool ValidatePayrate(float rate)
        {
            if (rate < 0f)
            {
                return false;
            }
            return true;
        }
    }

}
