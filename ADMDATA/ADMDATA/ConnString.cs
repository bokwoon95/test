using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient; 

namespace ADMDATA
{
    public class ConnString 
    {
            public string Con = ConfigurationManager.ConnectionStrings["ADMDBConnectionString"].ConnectionString;
 
    }
   


}
