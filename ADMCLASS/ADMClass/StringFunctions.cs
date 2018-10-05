using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMClass
{
    public class StringFunctions
    {
        public string strRight(string s, int strLength)
        {

            String text = s;
            String numbers = text;
            if (text.Length > strLength)
            {
                numbers = text.Substring(text.Length - strLength);
            }
            else
            {
                numbers = "";
            }

            return numbers;

        }

    }
}
