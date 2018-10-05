using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using CookComputing.XmlRpc;
using System.Collections;


[assembly: SecurityCritical]
namespace ADMDATA
{
    
    public class XMLRpcERPSelection
    {

        public string selectCubeTestToday()
        {
            return "";
        }
        public Object[] selectCubeTestAll(DateTime selectDate)
        {
            IXMLRpcERPLogin login_proxy = XmlRpcProxyGen.Create<IXMLRpcERPLogin>();

            int val = login_proxy.login("adms", "admin", "SKL@58#");

            IXMLRpcERPSelection sel = XmlRpcProxyGen.Create<IXMLRpcERPSelection>();
            Object[] filt = new Object[2];
            Object[] first_filt = new Object[3];
            first_filt[0] = "datetest";
            first_filt[1] = "=";
            first_filt[2] = selectDate.ToString("yyyy-MM-dd");
            Object[] second_filt = new Object[3];
            second_filt[0] = "status";
            second_filt[1] = "=";
            second_filt[2] = false;
            filt[0] = first_filt;
            filt[1] = second_filt;
            Object[] fields = { "ean13", "jobrefcode", "samplerefcode", "projectid", "display_name", "projecttitle", "size", "datecast", "ageday", "datetest", "concretetype", "samplemass", "maxload" };
            Object[] res;
            Object[] ids;
            ids = sel.search("adms", val, "SKL@58#", "cubejoblist.cubesample", "search", filt);

            if (ids.GetLength(0) == 0)
                return null;

            res = sel.read("adms", val, "SKL@58#", "cubejoblist.cubesample", "read", ids, fields);

            Object[] result = new Object[2];


            result[0] = ids;
            result[1] = res;
            return result;
        }
        struct WriteStruct
        {
            string key;
            string value;
        };

        public bool updateCubeTestData(int id, int age, string size, double samplemass, double maxload, string jobref, int testerid, string passfail, bool status, string TestingMachine, string samplerefcode, string projectid, string concretetype, string Tester)
        {
            double ageval = Convert.ToDouble(age);
            IXMLRpcERPLogin login_proxy = XmlRpcProxyGen.Create<IXMLRpcERPLogin>();

            int val = login_proxy.login("adms", "admin", "SKL@58#");

            IXMLRpcERPSelection sel = XmlRpcProxyGen.Create<IXMLRpcERPSelection>();


            Object[] ids = new Object[1];
            ids[0] = id;


            Object[] fields = { "strength", "grade" };

            Object[] res = sel.read("adms", val, "SKL@58#", "cubejoblist.cubesample", "read", ids, fields);
            Array a = (Array)res;

            XmlRpcStruct st = (XmlRpcStruct)a.GetValue(0);

            ArrayList lst = (ArrayList)st.Values;
            ArrayList lst2 = (ArrayList)st.Keys;
            bool custspecified = false;
            bool unspecified = false;


            //Strength is Unspecified by Customer
            double strength = 0.0;
            double grade = 0.0;
            if (lst[1].GetType().ToString() == "System.Boolean")
            {
                strength = 0.0;
                if (lst[0].GetType().ToString() == "System.Boolean")
                {
                    unspecified = true;
                }
                else
                {
                    grade = (double)lst[0];
                }
                    
                custspecified = false;
            }
            else // Strength is Customer Specified
            {
                if (lst[0].GetType().ToString() == "System.Boolean")
                {
                    grade = 0.0;
                }
                else
                {
                    grade = (double)lst[0];
                }
                strength = (double)lst[1];
                custspecified = true;
            }
            bool result = true;
            /*
                        if (strength == 0.0)
                            custspecified = false;
                        else
                            custspecified = true;*/

            int sizeval = 100;
            double div = 1.0;
            if (size == "100x100x100")
            {
                sizeval = 100;
                div = 1.0;

            }

            else if (size == "150x150x150")
            {
                sizeval = 150;
                div = (1.5 * 1.5 * 1.5);
            }


            double compstrength = (maxload * 1000) / (sizeval * sizeval);
            double density = ((samplemass*1000) / div);

            if (!custspecified && (!unspecified))
            {
                switch (age)
                {
                    case 1: { if (compstrength < (0.4 * grade)) result = false; break; }
                    case 2: { if (compstrength < (0.45 * grade)) result = false; break; }
                    case 3: { if (compstrength < (0.5 * grade)) result = false; break; }
                    case 4: { if (compstrength < (0.6 * grade)) result = false; break; }
                    case 5: { if (compstrength < (0.7 * grade)) result = false; break; }
                    case 6: { if (compstrength < (0.7 * grade)) result = false; break; }
                }
                if (age >= 7 && age <= 14)
                {
                    if (compstrength < (0.7 * grade)) result = false;
                }
                else if (age >= 15 && age <= 20)
                {
                    if (compstrength < (0.9 * grade)) result = false;
                }
                else if (age >= 21 && age <= 24)
                {
                    if (compstrength < (0.95 * grade)) result = false;
                }
                else if (age >= 25 && age <= 27)
                {
                    if (compstrength < grade) result = false;
                }
                else if (age >= 28)
                {
                    if (compstrength < grade) result = false;
                }
                if (compstrength > (grade + 20.0))
                    result = false;
            }
            else if (!unspecified)
            {
                if (compstrength < strength) result = false;
                if (compstrength > (strength + 20.0))
                    result = false;
            }
            else
                result = true;
            
            if (samplemass > 0.00 && samplemass < 0.010)
                result = false;

            if (result == false)
                passfail = "fail";

            // ArrayList keys = new ArrayList();

            string[] keys = new string[8];
            //keys.Add("passfail".ToString());
            //keys.Add("status".ToString());
            keys[0] = "keys";
            keys[1] = "samplemass";
            keys[2] = "passfail";
            keys[3] = "status";
            keys[4] = "compstrength";
            keys[5] = "density";
            keys[6] = "machinecode";
            keys[7] = "testedby";
            

            ArrayList values = new ArrayList();
            values.Add("values");
            values.Add(samplemass);
            values.Add(maxload);
            values.Add(passfail);
            values.Add(status);
            values.Add(TestingMachine);
            values.Add(Tester);
            //values.Add(compstrength);
            //          values.Add(density);



            //XmlRpcStruct str = new XmlRpcStruct();
            //XmlRpcStruct[] resultvals = new XmlRpcStruct[1];

            XmlRpcStruct str = new XmlRpcStruct();
            //XmlRpcStruct[] resultvals = new XmlRpcStruct[1];


            //str.Add(keys[0], values[0]);
            //Object[] resultvals = new Object[1];
            //resultvals[0] = new XmlRpcStruct();
            str.Add("samplemass", samplemass);
            str.Add("maxload", maxload);
            str.Add("passfail", passfail);
            str.Add("status", status);
            str.Add("compstrength", compstrength);
            str.Add("density", density);
            str.Add("machinecode", TestingMachine);
            str.Add("testedby", Tester);
            //str.Add("strength", strength);
            //str.Add("grade", grade);
            //str.Add("samplerefcode", samplerefcode);
            //str.Add("projectid", projectid);
            //str.Add("ageday", ageval);
            //str.Add("size", size);
            //str.Add("id", id);
            //str.Add("concretetype", concretetype);

            //ArrayList resultvals = new ArrayList();
            //resultvals.Add(str);

            Object[] resultvals = new Object[1];
            resultvals[0] = str;


            //Array a = (Array)finresultvals;

            Object[] finresultvals = new Object[2];
            finresultvals[0] = ids; // = resultvals;//new Object[1];
            finresultvals[1] = resultvals;

            bool write = sel.write("adms", val, "SKL@58#", "cubejoblist.cubesample", "write", ids, str);

            Object[] first_filt = new Object[3];
            first_filt[0] = "id";
            first_filt[1] = "=";
            first_filt[2] = ids[0];
            Object[] filt = new Object[1];
            filt[0] = first_filt;

            ids = sel.search("adms", val, "SKL@58#", "cubejoblist.cubesample", "search", filt);

            if (ids.GetLength(0) == 0)
                return true;
            Object[] fieldReads = { "validation" };
            Object[] readVals = sel.read("adms", val, "SKL@58#", "cubejoblist.cubesample", "read", ids, fieldReads);
            Array arrVals = (Array)readVals;
            XmlRpcStruct stVals = (XmlRpcStruct)arrVals.GetValue(0);

            ArrayList lstVals = (ArrayList)stVals.Values;
            ArrayList lstKeys = (ArrayList)stVals.Keys;
            bool validation;
            if (lstKeys[0].Equals("id"))
            {
                validation = (bool)lstVals[1];
            }
            else
                validation = (bool)lstVals[0];

            if (validation == false)
                result = false;


            return result;
        }

        public string selectCubeTestPending(DateTime selectDate)
        {
            return "";
        }
        
        

    }

    [XmlRpcUrl("http://192.168.1.253:8069/xmlrpc/common")]
    public interface IXMLRpcERPLogin : IXmlRpcProxy
    {
        
        [XmlRpcMethod("login")]
        int login(string dbName, string dbUser, string dbPwd);

    }

    [XmlRpcUrl("http://192.168.1.253:8069/xmlrpc/object")]
    public interface IXMLRpcERPSelection : IXmlRpcProxy
    {
        [XmlRpcMethod("execute")]
        Object[] search(string dbName, int userId, string pwd, string model, string method, Object[] filters);

        [XmlRpcMethod("execute")]
        Object[] read(string dbName, int userId, string pwd, string model, string method, Object[] ids, Object[] fields);

        [XmlRpcMethod("execute")]
        bool write(string dbName, int userId, string pwd, string model, string method,Object[] ids, Object fields);
    }


  
}
