using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADMDATA;
using ADM.Data;
using ADMClass;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Timers;
using CookComputing.XmlRpc;
using System.Collections;

namespace ADMAPP
{
    public partial class frmADMAPP : Form
    {
        private ADMDATA.ConnString aDMDATA = new ADMDATA.ConnString();
        private string selectCubeTest = new ADMDATA.SQLSelection().selectCubeTestToday();
        private string barcodeKey =""; //used in barcorde keydown event 
        private SerialPort cubeTestingPort; // use private field for SerialPort
        private SerialPort cubeWeighingPort; // use private field for SerialPort
        private string cubeTestingLineReadIn="";
        private string cubeTestingRead;
        //private string cubeWeighingLineReadIn="";
        private string cubeWeighingRead;
        private string TestingMachine;
        private string portName;
       // private bool  lastLine=false ;
        private bool TestingStarted = false;
        private double cubeMaxLoad;
        //private double cubeMass;
        private int myListCount = 0;
        private int cubeTestedBy = 1;
        List<double> WeightValue = new List<double>();

        //string testLineReadIn; //read line from port
        //string lineReadIn;

        // this will prevent cross-threading between the serial port
        // received data thread & the display of that data on the central thread
        private delegate void preventCrossThreading(string x);
        private preventCrossThreading accessControlFromCentralThreadWeight;
        private preventCrossThreading accessControlFromCentralThreadSampleMass;
        private preventCrossThreading accessControlFromCentralThreadTest;
        private preventCrossThreading accessControlFromCentralThreadMaxLoad;
        private preventCrossThreading accessControlFromCentralThreadLastCubeID;
        private preventCrossThreading accessControlFromCentralThreadListView;
        private preventCrossThreading accessControlFromCentralThreadLastBarcode;
        private preventCrossThreading accessControlFromCentralThreadLastCubeRef;

        private System.Timers.Timer cubeTimer = new System.Timers.Timer();
        private string timerRead = "";

        private string MCSet = "";
        private string WCom = "";
        private string TCom = "";
        private string CubeRemarks = "";
        private string Tester = "";

        public frmADMAPP()
        {
            try
            {
                InitializeComponent();
                //InitializePenel();
                StartPosition = FormStartPosition.CenterScreen;
                //for cube test initial the form
                ribbonTab1.Active = false;
                ribbonTab3.Active = true;
                rbtnCubeTest_Click(new object(), new EventArgs());
                machineSet();
                setRS232();
                openRS232Port();
                livCubeTestList.Focus();
                if (livCubeTestList.Items.Count > 0)
                {
                    livCubeTestList.Items[0].Selected = true;
                }

                cubeTestingPort.NewLine = "\n";
            }

            catch (Exception ex)
            {
                MessageBox.Show("There was an error on Data-in:" +
                        "\nError Message:" + ex.Message, "Format");
            }


        }

        private void frmADMAPP_Load(object sender, EventArgs e)
        {

            statusStrip1.Text = "Load form";
            statusStrip1.BackColor = SystemColors.ControlDark;


            cubeTimer.Interval = (1000) * (1);
            cubeTimer.Elapsed += new System.Timers.ElapsedEventHandler(cubeTimer_Elapsed); // Everytime timer ticks, timer_Tick will be called
            if (TestingMachine == "MC1")
            {
                cubeTimer.Interval = (1000) * (2);

            }
            else if (TestingMachine == "MC2")
            {
                cubeTimer.Interval = (1000) * (1);
            }
            else if (TestingMachine == "MC5")
            {
                cubeTimer.Interval = (1000) * (1);
            }
            else if (TestingMachine == "MC6")
            {
                cubeTimer.Interval = (1000) * (1);
            }
            else if (TestingMachine == "MCW1")
            {
                cubeTimer.Interval = (5000) * (1);
                cubeTimer.AutoReset = true;
            }
            else if (TestingMachine == "MC8")
            {
                cubeTimer.Interval = (1000) * (1);
            }
            //MC3:
            else
            {
                cubeTimer.Interval = (10) * (1);
            }

                         // Timer will tick evert 2 seconds
            
            cubeTimer.Enabled = true;                       // Enable the timer
            cubeTimer.Stop();                              // Start the timer

        }   

       

        private void rbtnCubeTest_Click(object sender, EventArgs e)
        {

            livCubeTestList.Visible = true;
            
            rbtnTestDate.TextBoxText = DateTime.Now.ToString("dd/MM/yyyy");
            addCubeTestList("today");
            //openPort();
        }



        private void InitializePenel()
        {
            livCubeTestList.Visible = false;

        }

  

        private void roptbtnExit_Click(object sender, EventArgs e)
        {
            //Console.WriteLine("clicked");
            Close();
        }

        /*
        private void CopyDataSet(DataSet dataSet)
        {
            // Create an object variable for the copy.
            DataSet copyDataSet;
            copyDataSet = dataSet.Copy();

            // Insert code to work with the copy.
        }
        */

        private void connectme()
        {

            string con = ConfigurationManager.ConnectionStrings["ADMDBConnectionString"].ConnectionString;

            foreach (string key in ConfigurationManager.AppSettings)
            {
                string value = ConfigurationManager.AppSettings[key];


                Console.WriteLine("Key: {0}, Value: {1}", key, value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //bind();
            //connectme();

            //Populate();
             //addCubeTestList("done");
             //addCubeTestList("pending");
            //addCubeTestList("today");
            //addCubeTestList("all");
            
            
            //livCubeTestList.Focus() ;
            //livCubeTestList.Items[120].Selected = true;
            //livCubeTestList.Items[120].EnsureVisible();
            //object sender  ;
            //SerialDataReceivedEventarg e;
           // testingPort_DataReceived(sender,  e);
            //string readln = "$ALL +0000290.510 +0000021.398 0\r" + "$ALL +0000291.510 +0000021.398 0\r" + "$ALL +0000292.510 +0000021.398 0\r";

            //string readln = "$ALL +0000290.510 +0000021.398 0\r$C    0  300 12/06/13 21:22   150.00   150.00     0.00     0.00          0.00    22500.00    0.400     322.750      14.344\r";
            //string readln = "$ALL +0000290.510 +0000021.398 0\r$C    0  300 12/06/13 21:22   150.00   150.00     0.00     0.00          0.00    22500.00    0.400     322.750      14.344";
            MessageBox.Show("start" + cubeTestingLineReadIn);
            //string readln = "14.344\r$C    0  300 12/06/13 21:22   150.00   150.00     0.00     0.00          0.00    22500.00    0.400     322.750      14.344\r";

            //string readln = "22500.00    0.400     322.750      14.344\r";
            //string readln = "7.7, 102.6, 4.56;\r\n";
            string readln = "CUBE                                       08-03-14150  150  150                                                               122.7     5.453999\r\n";

            //string readln = "510 +0000021.398 0\r$ALL +0000290.510 +0000021.398 0\r$ALL +";
           // string readln = "$C    0  300 12/06/13 21:22   150.00   150.00     0.00     0.00          0.00    22500.00    0.400     322.750      14.344\r";
            //string readln = "   150.00   150.00     0.00     0.00          0.00    22500.00    0.400     322.750      14.344\r";

            //MessageBox.Show(readln.Length.ToString());
            //MessageBox.Show(readln + " \r\nLen:" + readln.Length.ToString() );
            if (readln.Length > 0)
                {
                    if (TestingStarted == false)
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayMaxLoadTextReadIn("");
                        cubeTestingLineReadIn = "";
                        TestingStarted = true; 
                        //cubeTimer.Start();
                    }

                    //cubeTestingRead = cubeTestingPort.ReadLine();
                    //cubeTestingRead = cubeTestingPort.ReadExisting();
                    cubeTestingRead = readln;
                    cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;
                    //int all = cubeTestingLineReadIn.IndexOf("$ALL");
                    //if (all != 0 && all!=-1)
                    //{
                    //    cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(all, cubeTestingLineReadIn.Length - all);

                    //}
                    //M2displayData();
                    M3displayData();

                }
            
            
            /*
            if (readln.Length > 4)
            {
                int end = readln.IndexOf("$C");
                int first = readln.IndexOf("$ALL");
                int last = readln.IndexOf("\r");
                if (end == -1)
                {
                    if ( cubeTestingLineReadIn!="")
                    {
                        end = cubeTestingLineReadIn.IndexOf("$C");
                    }
                    if (end == -1)
                    {

                        if (first == -1 || last == -1 || first > last)
                        {

                            cubeTestingLineReadIn = cubeTestingLineReadIn + readln;

                            MessageBox.Show(first.ToString() + "," + last.ToString() + "\r\n" + cubeTestingLineReadIn);
                            first = cubeTestingLineReadIn.IndexOf("$ALL");
                            last = cubeTestingLineReadIn.IndexOf("\r");

                            if (first != -1 && last != -1 && first < last)
                            {
                                //cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(first, cubeTestingLineReadIn.Length -last);
                                cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(5, 12);
                                double db = Convert.ToDouble(cubeTestingLineReadIn);
                                displayTestingTextReadIn(db.ToString());
                                //display 2
                                MessageBox.Show(cubeTestingLineReadIn);
                                cubeTestingLineReadIn = "";
                            }
                            else if (first > last)
                            {
                                cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(first, cubeTestingLineReadIn.Length - first);
                                first = cubeTestingLineReadIn.IndexOf("$ALL");
                                last = cubeTestingLineReadIn.IndexOf("\r");
                                if (first != -1 && last != -1 && first < last)
                                {
                                    cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(5, 12);
                                    double db = Convert.ToDouble(cubeTestingLineReadIn);
                                    displayTestingTextReadIn(db.ToString());
                                    //display 3
                                    MessageBox.Show(cubeTestingLineReadIn);
                                    cubeTestingLineReadIn = "";
                                }


                            }

                        }
                        else
                        {
                            if (readln.Length >= 33)
                            {
                                cubeTestingLineReadIn = readln.Substring(5, 12);
                                MessageBox.Show(cubeTestingLineReadIn);
                                double db = Convert.ToDouble(cubeTestingLineReadIn);
                                displayTestingTextReadIn(db.ToString());
                                //display 1
                                cubeTestingLineReadIn = "";
                            }
                        }
                    }
                    else
                    {
                        cubeTestingLineReadIn = cubeTestingLineReadIn + readln;
                        end = cubeTestingLineReadIn.IndexOf("$C");
                        if (end==0 && cubeTestingLineReadIn.Length >= 123)
                        {
                            cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(98, 12).Trim();
                            //display end2
                            
                            double db = Convert.ToDouble(cubeTestingLineReadIn);
                            displayMaxLoadTextReadIn(db.ToString());
                            //saveLoad();
                            cubeTestingLineReadIn = "";

                        }

                    }

                }
                else //end string
                {
                    cubeTestingLineReadIn = readln.Substring(end, readln.Length - end);
                    if (cubeTestingLineReadIn.Length >= 123)
                    {
                        cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(98, 12).Trim();
                        //display end1

                        double db = Convert.ToDouble(cubeTestingLineReadIn);
                        displayMaxLoadTextReadIn(db.ToString());
                        //saveLoad();
                        cubeTestingLineReadIn = "";
                        
                    }

                }


            }
            */
            /*
            if (readln.Length > 14)
            {
                //readln = readln.Substring(0, readln.Length - 3);
                //readln = "\r" + readln;
                MessageBox.Show(readln.Length.ToString());
                MessageBox.Show(readln);

                int end = readln.LastIndexOf("$C");
                if (end > 0)
                {
                    readln = readln.Substring(end + 1, readln.Length - end - 1);
                    if (readln.LastIndexOf("\r") > 0)
                    {
                        MessageBox.Show("before end cut: \r\n" + readln);
                        readln = readln.Substring(98, 11);
                    }
                    else
                    {
                        MessageBox.Show("not finished reading: \r\n" + readln);

                    }
                }

                MessageBox.Show("end cut: \r\n" + readln);

                int first = readln.LastIndexOf("$ALL");
                if (first >= 0)
                {
                    cubeTestingLineReadIn = readln.Substring(first + 1, readln.Length - first - 1);

                    int firstComa = cubeTestingLineReadIn.IndexOf("+");
                    int lastComa = cubeTestingLineReadIn.LastIndexOf("+");
                    MessageBox.Show(firstComa.ToString() + ";" + lastComa.ToString());
                    cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(firstComa + 1, lastComa - firstComa - 1);
                } c 
                else
                Z
                    cubeTestingLineReadIn = cubeTestingLineReadIn + readln;
                }
                readln = readln.Trim();

                MessageBox.Show(cubeTestingLineReadIn);
                double db = Convert.ToDouble(cubeTestingLineReadIn);
                MessageBox.Show(db.ToString());
                displayTestingTextReadIn(cubeTestingLineReadIn);
                txtMaxLoad.Text = db.ToString();
            }
            */
            
        }


        private void addCubeTestList(string selectCube)
        {
            string source = aDMDATA.Con;

            SqlConnection connection = new SqlConnection(source);
            XMLRpcERPSelection sel = new XMLRpcERPSelection();
            Object[] result = null; 
            // MessageBox.Show(result.GetType().ToString()result.GetLength(0).ToString());
            //MessageBox.Show("Here");
            
            
            // if (xType == XmlRpcType.tStruct)


            
            bool s = false;
            // MessageBox.Show(s.GetType().ToString());
            //MessageBox.Show("struct : " + o.GetType().ToString());
            int DATASIZE = 12;
            /*
            MessageBox.Show(a.GetLength(0).ToString());
            
            for (int i = 0; i < a.GetLength(0); i++)
            {
                MessageBox.Show(i.ToString());
                XmlRpcStruct st = (XmlRpcStruct)a.GetValue(i);
                
                ArrayList lst = (ArrayList)st.Values;
                ArrayList lst2 = (ArrayList)st.Keys;
                
                MessageBox.Show(lst2[0].ToString() + ":" + lst[0].ToString());
                MessageBox.Show(lst2[1].ToString() + ":" + lst[1].ToString());
                MessageBox.Show(lst2[2].ToString() + ":" + lst[2].ToString());
                MessageBox.Show(lst2[3].ToString() + ":" + lst[3].ToString());
                MessageBox.Show(lst2[4].ToString() + ":" + lst[4].ToString());
                MessageBox.Show(lst2[5].ToString() + ":" + lst[5].ToString());
                MessageBox.Show(lst2[6].ToString() + ":" + lst[6].ToString());
                MessageBox.Show(lst2[7].ToString() + ":" + lst[7].ToString());
                MessageBox.Show(lst2[8].ToString() + ":" + lst[8].ToString());
                MessageBox.Show(lst2[9].ToString() + ":" + lst[9].ToString());
                MessageBox.Show(lst2[10].ToString() + ":" + lst[10].ToString());
                MessageBox.Show(lst2[11].ToString() + ":" + lst[11].ToString());
                MessageBox.Show("Here 12");
                MessageBox.Show(lst2[12].ToString() + ":" + lst[12].ToString());
                MessageBox.Show(lst2[13].ToString() + ":" + lst[13].ToString());
                MessageBox.Show("Completed " + i.ToString());

            }
             */

            //livCubeTestList.FullRowSelect = false;
            myListCount = 0;
            try
            {
                string queryString = null;
                DateTime selectCubeTestDate = Convert.ToDateTime(rbtnTestDate.TextBoxText);
                if (selectCube == "all")
                {
                    queryString = new ADMDATA.SQLSelection().selectCubeTestAll(selectCubeTestDate);
                    result = sel.selectCubeTestAll(selectCubeTestDate);
                }
                else if (selectCube == "done")
                {
                    queryString = new ADMDATA.SQLSelection().selectCubeTestDone(selectCubeTestDate);
                    result = sel.selectCubeTestAll(selectCubeTestDate);
                }
                else if (selectCube == "pending")
                {
                    queryString = new ADMDATA.SQLSelection().selectCubeTestPending(selectCubeTestDate);
                    result = sel.selectCubeTestAll(selectCubeTestDate);
                }
                else if (selectCube == "today")
                {
                    queryString = new ADMDATA.SQLSelection().selectCubeTestAll(DateTime.Now);
                    result = sel.selectCubeTestAll(DateTime.Now);

                }
                else
                {
                    MessageBox.Show("Error Slection on Date");
                }
                //MessageBox.Show(queryString);

                livCubeTestList.Columns.Clear();
                livCubeTestList.Items.Clear();
                livCubeTestList.View = View.Details;

                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = queryString;
                SqlDataReader dr = cmd.ExecuteReader();
                /*
               for (int i = 0; i < dr.FieldCount; i++ )
               {
                   ColumnHeader ch = new ColumnHeader();
                   ch.Text = dr.GetName(i);
                   livCubeTestList.Columns.Add(ch);

               }*/



                ColumnHeader ch = new ColumnHeader();
                ch.Text = "BarCode";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "SpecimenRef";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "Job Reference Code";
                livCubeTestList.Columns.Add(ch);


                ch = new ColumnHeader();
                ch.Text = "ProjectCode";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "Company";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "Title";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "Size";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "DateCast";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "Age";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "DateTest";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "Grade";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "SampleMass";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "MaxLoad";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "Cube Id";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "Cube Remarks";
                livCubeTestList.Columns.Add(ch);

                ch = new ColumnHeader();
                ch.Text = "CubeSystem";
                livCubeTestList.Columns.Add(ch);


                ListViewItem itmX;

                if (result != null)
                {
                    Object ids = result[0];
                    Object[] res = (Object[])result[1];

                    XmlRpcType xType = XmlRpcServiceInfo.GetXmlRpcType(res.GetType());
                    Array a = (Array)res;
                    Object o = (Object)a.GetValue(0);
                    //MessageBox.Show("Length " + a.GetLength(0).ToString());
                    
                    for (int i = 0; i < a.GetLength(0); i++)
                    {

                        XmlRpcStruct st = (XmlRpcStruct)a.GetValue(i);


                        ArrayList lst = (ArrayList)st.Values;
                       ArrayList lst2 = (ArrayList)st.Keys;
                            itmX = new ListViewItem();
                            itmX.Text = lst[0].ToString();
                            itmX.SubItems.Add(lst[1].ToString());
                            itmX.SubItems.Add(lst[9].ToString());
                            itmX.SubItems.Add(lst[6].ToString());
                            itmX.SubItems.Add(lst[2].ToString());
                            itmX.SubItems.Add(lst[10].ToString());
                            itmX.SubItems.Add(lst[13].ToString());
                            itmX.SubItems.Add(lst[4].ToString());
                            itmX.SubItems.Add(lst[7].ToString());
                            itmX.SubItems.Add(lst[5].ToString());
                            itmX.SubItems.Add(lst[3].ToString());
                            itmX.SubItems.Add(lst[11].ToString());
                            itmX.SubItems.Add(lst[12].ToString());
                            itmX.SubItems.Add(lst[8].ToString());
                            itmX.SubItems.Add(" ");
                            itmX.SubItems.Add("new");



                            livCubeTestList.Items.Add(itmX);
                      
                        myListCount = myListCount + 1;
                        lbltTotalCube.Text = myListCount.ToString();
                    }
                }


               /* MessageBox.Show("here");

                bool status = sel.updateCubeTestData(722, 7, "150x150x150", 7.9899, 1247.6, "B0030700", 1, "pass", true, "MC1", "PD318125/A", "F0917", "60H");
                if(status == false)
                     MessageBox.Show("Test Failed to Meet Criteria. Please Keep the Sample for Future Reference");*/

                while (dr.Read())
                {
                    itmX = new ListViewItem();
                    itmX.Text = dr.GetValue(0).ToString();
                    String projcode = dr.GetValue(3).ToString();
                    if (!(projcode.Equals("F0917")))
                    {
                        for (int i = 1; i < dr.FieldCount; i++)
                        {
                            itmX.SubItems.Add(dr.GetValue(i).ToString());

                        }
                        itmX.SubItems.Add("old");
                        livCubeTestList.Items.Add(itmX);
                        myListCount = myListCount + 1;
                        lbltTotalCube.Text = myListCount.ToString();
                    }
                        

                    
                }
                dr.Close();

                if (myListCount == 0)
                {
                    MessageBox.Show("No More cubes to test today");
                    return;
                }

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                MessageBox.Show("There was an error in executing the SQL." +
                        "\nError Message:" + ex.Message, "SQL");
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error in formatting." +
                        "\nError Message:" + ex.Message, "Format");
            }
            finally
            {
                connection.Close();
                ResizeListViewColumns(livCubeTestList);
                //livCubeTestList.FullRowSelect = true;

            }
        }
       


        private void livCubeTestList_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            try
            {
                //if (barcodeKey.Length == 13)
                //{
                  //  MessageBox.Show(barcodeKey);
                //}

                //MessageBox.Show(e.KeyCode.ToString());
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)// || e.KeyCode.ToString() == "ShiftKey")
                {
                    //MessageBox.Show("Here"+barcodeKey);
                    if (livCubeTestList.BackColor == Color.AliceBlue)
                    {
                        livCubeTestList.BackColor = Color.Aquamarine;
    
                    }
                    else
                    {
                        livCubeTestList.BackColor = Color.AliceBlue;
                        
                    }
                    if (barcodeKey.Length > 8)
                    {
                        if (barcodeKey[0].ToString().Equals("*") || barcodeKey[0].ToString().Equals("8"))
                        {
                            
                            barcodeKey = barcodeKey.Substring(1, barcodeKey.Length - 2);
                        }
                       

                        txtBarCode.Text = barcodeKey;

                        
                       // MessageBox.Show(barcodeKey);
                        //findCubeInList();
                    //    MessageBox.Show(barcodeKey);
                        ListViewItem lv = livCubeTestList.FindItemWithText(barcodeKey);
                        //MessageBox.Show(barcodeKey + lv.Index.ToString());
                        //barcodeKey = "";

                        lblNumberOfCube.Text = (lv.Index+1).ToString() + " of ";
                        livCubeTestList.Focus();
                        livCubeTestList.SelectedItems.Clear();
                        livCubeTestList.Items[lv.Index].Selected = true;
                        if (livCubeTestList.Items.Count - lv.Index > 5)
                        {
                            livCubeTestList.Items[lv.Index + 5].EnsureVisible();
                        }
                        else
                        {
                            livCubeTestList.Items[lv.Index].EnsureVisible();
                        }
                        //livCubeTestList.GetItemAt(0, lv.Index);

                        


                    }
                    else
                    {
                        // MessageBox.Show("Et");
                    }
                    //checkWeight;


                    if ((txtWeightLive.Text).Length >= 8)
                    {
                        txtSampleMass.Text = ((txtWeightLive.Text).Substring(1, 7)).Trim();
                       // txtSampleMass.Text = 0.0";
                       // MessageBox.Show("Here");

                        //CHEN HONG
                        //cubeWeighingPort.DiscardInBuffer();


                        //string mass = ((txtWeightLive.Text).Substring(1, 8)).Trim();
                        //displaySampleMassTextReadIn(mass);
                    }
                    else
                    {
                        txtSampleMass.Text = txtWeightLive.Text;
                        //MessageBox.Show("Here 2");
                        //txtSampleMassCurrent.Text = "";
                    }
                   // CHEN HONG M3.
                    if (TestingMachine == "MC3")
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        txtLoadLive.Text = "TESTING";

                        TestingStarted = true;
                    }
                    //Chen Hong MC6
                    if (TestingMachine == "MC6" || TestingMachine=="MC8" || TestingMachine== "MCW1")
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        txtLoadLive.Text = "TEST";
      
                        //TestingStarted = true;
                    }

                    barcodeKey = "";

                }
                else
                {
 
                    if (e.KeyCode.ToString() != "ShiftKey")
                    {
                        if (e.KeyCode.ToString().Length == 2 && e.KeyCode.ToString().Substring(0, 1) == "D")
                        {
                            barcodeKey = barcodeKey + e.KeyCode.ToString().Substring(1, 1);
                            //MessageBox.Show(e.KeyCode.ToString().Substring(1, 1));
                        }
                        else if (e.KeyCode.ToString() == "OemMinus")
                        {

                            barcodeKey = barcodeKey + "-";
                           // MessageBox.Show("-");
                        }
                        else if (e.KeyCode == Keys.Space)
                        {

                            barcodeKey = barcodeKey + " ";
                            //MessageBox.Show(" ");
                        }

                        else if (e.KeyCode == System.Windows.Forms.Keys.Up || e.KeyCode == System.Windows.Forms.Keys.Down || e.KeyCode == System.Windows.Forms.Keys.Right || e.KeyCode == System.Windows.Forms.Keys.Left)
                        {
                            //MessageBox.Show("arrow");
                            barcodeKey = "";
                        }
                        else
                        {

                            barcodeKey = barcodeKey + e.KeyCode.ToString();
                            //MessageBox.Show(e.KeyCode.ToString());
                        }
                        


                    }
                    else if (e.KeyCode.ToString() == "EnterKey")
                    {
                        MessageBox.Show("Et" );
                    }
                    else
                    {
                        //MessageBox.Show(barcodeKey);
                    }


                    //MessageBox.Show(barcodeKey);
                    // MessageBox.Show(barcodeKey);
                    // MessageBox.Show(barcodeKey);
                    //barcodeKey = barcodeKey +  e.KeyCode.ToString();
                }

            }
            catch (Exception ex)
            {
                if (barcodeKey.Length  >= 4)
                {
                    if (barcodeKey.Substring(0, 4) == "TBY-")
                    {

                        int first = barcodeKey.Substring(4, 4).Trim().IndexOf("-");
                        cubeTestedBy = Convert.ToInt32(barcodeKey.Substring(4, first).Trim());
                        toolStripStatusLabel2.Text = barcodeKey.Substring(4, barcodeKey.Length - 4);
                    }
                    else
                    {
                        //MessageBox.Show(barcodeKey.Substring(0, 4));
                        MessageBox.Show(barcodeKey + "\r\n" + ex.Source.ToString() + " Find Cube Error/ This cube maybe already tested. \r\n" + ex.Message);
                    }
                }
                barcodeKey = "";
                livCubeTestList.Refresh();
            }
            

        }

        
        private void livCubeTestList_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                if (livCubeTestList.SelectedItems.Count > 0)
                {

                    //livCubeTestList.KeyDown;
                    //0.[BarCode]
                    txtBarCode.Text = livCubeTestList.SelectedItems[0].SubItems[0].Text;
                    //1.[SpecimenRef]
                    txtSpecimenRef.Text = livCubeTestList.SelectedItems[0].SubItems[1].Text;
                    //2.[JobRef]
                    txtJobRef.Text = livCubeTestList.SelectedItems[0].SubItems[2].Text;
                    //3.[ProjectCode]
                    txtProjectCode.Text = livCubeTestList.SelectedItems[0].SubItems[3].Text;
                    //4.[Company]
                    txtCompany.Text = livCubeTestList.SelectedItems[0].SubItems[4].Text;
                    //5.[ProjectTitle]
                    txtProjectTitle.Text = livCubeTestList.SelectedItems[0].SubItems[5].Text;
                    //6.[Size]
                    txtSize.Text = livCubeTestList.SelectedItems[0].SubItems[6].Text;
                    //7.[DateCast]
                    DateTime dt = Convert.ToDateTime((livCubeTestList.SelectedItems[0].SubItems[7].Text));
                    txtDateCast.Text = String.Format("{0:dd/MM/yyyy}", dt);
                    //8.[Age]
                    txtAge.Text = livCubeTestList.SelectedItems[0].SubItems[8].Text;
                    //9.[DateTest]
                    dt = Convert.ToDateTime(livCubeTestList.SelectedItems[0].SubItems[9].Text);
                    txtDateTest.Text = String.Format("{0:dd/MM/yyyy}", dt);
                    //10.[Grade]
                    txtGrade.Text = livCubeTestList.SelectedItems[0].SubItems[10].Text;
                    //11.[SampleMass]
                    txtSampleMass.Text = ((txtWeightLive.Text).Substring(0, 5)).Trim();
                    //12.[MaxLoad]
                    txtMaxLoad.Text = livCubeTestList.SelectedItems[0].SubItems[12].Text;
                    //13.[CubeID]
                    txtID.Text = livCubeTestList.SelectedItems[0].SubItems[13].Text;
                    //14. Remarks
                    CubeRemarks = livCubeTestList.SelectedItems[0].SubItems[14].Text;
                    lblSystem.Text = livCubeTestList.SelectedItems[0].SubItems[15].Text;
                    lblNumberOfCube.Text = (livCubeTestList.SelectedIndices[0] + 1).ToString() + " of ";
                    


                    // CHEN HONG M3.
                    if (TestingMachine == "MC3")
                    {
                        displayLastCubeIDTextReadIn("");
                        displayLastBarcodeTextReadIn("");
                        displayLastSpecimenRefTextReadIn("");
                        displaySampleMassTextReadIn("");
                        displayMaxLoadTextReadIn("");
                        txtLoadLive.Text = "";

                        TestingStarted = false;
                    }
                    //CHEN HONG ADD MC6
                    if (TestingMachine == "MC6")
                    {
                        displayLastCubeIDTextReadIn("");
                        displayLastBarcodeTextReadIn("");
                        displayLastSpecimenRefTextReadIn("");
                        displaySampleMassTextReadIn("");
                        displayMaxLoadTextReadIn("");
                        txtLoadLive.Text = "";
                        //displayWeighingTextReadIn("");
                        //TestingStarted = false;
                    }

                    //Vignesh ADD MCW1
                    if (TestingMachine == "MCW1")
                    {
                        //displayLastCubeIDTextReadIn("");
                        //displayLastBarcodeTextReadIn("");
                        //displayLastSpecimenRefTextReadIn("");
                        //displaySampleMassTextReadIn("");
                        //displayMaxLoadTextReadIn("");
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn(txtMaxLoad.Text);
                        txtLoadLive.Text = "";
                    }

                    // Xuejie add MC8
                    if (TestingMachine == "MC8")
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn(txtMaxLoad.Text);
                        txtLoadLive.Text = "";
                        //displayWeighingTextReadIn("");
                        //TestingStarted = false;
                    }




                }

                else
                {
                    // txtBarCode.Text = string.Empty;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(barcodeKey + "\r\n" + ex.Source.ToString() + " Finding Cube Error. \r\n" + ex.Message);
            }

        }
         
        private void ResizeListViewColumns(ListView lv)
        {
            int i = 0;
            foreach (ColumnHeader column in lv.Columns)
            {
                i += 1;
                if (i == 1)
                {
                    column.Width = 160;
                }
                else if (i == 2)
                {
                    column.Width = 250;
                }
                else
                {
                    column.Width = 80;
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Save Mass and Load
            cubeTestingRead = "0.0, 3.3, 0.15;\r\n";
            M1displayData();

           
            //MC1MaxLoad();
            // saveLoad();
            


        }

        private void saveLoad()
        {
            /* test xuejie
            if ((txtMaxLoadCurrent.Text).Trim() != "")
            { 
                double maxLoad = Math.Round(Convert.ToDouble(txtMaxLoadCurrent.Text), 1);
                displayTestingTextReadIn("Saved");
            }
            else
            {
                displayTestingTextReadIn("Completed, no data");
            }
            return;
            */

            string source = aDMDATA.Con;
            SqlConnection connection = new SqlConnection(source);
            CubeRemarks = CubeRemarks + "\\" + DateTime.Now.ToString("hh:mm dd/MM/yyyy");
            if (txtLastCubeID.Text != "CubeID")
            {
                //string qry = @"select * from vCubeTestToday";
                //string upd = @"update vCubeTestToday set MaxLoad = @MaxLoad, 
                //                SampleMass =@SampleMass where CubeID = @cubeid";
                string qry = @"select [CubeID],[MaxLoad], [SampleMass],[Remarks] from [Cube Details] where CubeID = @cubeid";
                string upd = @"update [Cube Details] set MaxLoad = @MaxLoad, 
                            SampleMass =@SampleMass,
                            Remarks =@Remarks where CubeID = @cubeid";
                string qry2 = @"select [JobRef],[TestedBy]from [Cube Receiving] where JobRef = @JobRef";
                string upd2 = @"update [Cube Receiving] set TestedBy = @TestedBy  
                            where JobRef = @JobRef";

                string qry3 = @"select [CubeID],[MaxLoad], [SampleMass],[Remarks] from [Cube Details] where SpecimenRef=@SpecimenRef";
                string upd3 = @"update [Cube Details] set MaxLoad = @MaxLoad, 
                            SampleMass =@SampleMass,
                            Remarks =@Remarks where SpecimenRef=@SpecimenRef";
                try
                {
                    if ((txtSampleMassCurrent.Text).Trim() != "" | (txtMaxLoadCurrent.Text).Trim() != "")
                    {

                        int cubeid = Convert.ToInt32(txtLastCubeID.Text);
                        double sampleMass = Convert.ToDouble(txtSampleMassCurrent.Text);
                        double maxLoad = Math.Round(Convert.ToDouble(txtMaxLoadCurrent.Text),1);
                        string JobeRef = txtJobRef.Text;
                        string SampleRef = txtSpecimenRef.Text;
                        string projectid = txtProjectCode.Text;
                        string concretetype = txtGrade.Text;
                        int TestedBy = cubeTestedBy;
                        int age = Convert.ToInt32(txtAge.Text);
                        string size = txtSize.Text;

                        if (lblSystem.Text.Equals("new"))
                        {
                            XMLRpcERPSelection sel = new XMLRpcERPSelection();
                           
                            bool status = sel.updateCubeTestData(cubeid, age, size, sampleMass, maxLoad, JobeRef, TestedBy, "pass", true, TestingMachine, SampleRef, projectid, concretetype,Tester);
                            if (!status)
                                MessageBox.Show("Test Failed to Meet Criteria. Please Keep the Sample for Future Reference");
                            
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = new SqlCommand(qry3, connection);
                            da.SelectCommand.Parameters.Add("@SpecimenRef", SqlDbType.NVarChar);
                            da.SelectCommand.Parameters["@SpecimenRef"].Value = SampleRef;
                            DataSet ds = new DataSet();
                            da.Fill(ds, "[Cube Details]");
                            DataTable dt = ds.Tables["[Cube Details]"];
                            
                            //dt.Rows[0]["Cubeid"] = cubeid;
                            SqlCommand cmd = new SqlCommand(upd3, connection);
                            cmd.Parameters.Add("@SpecimenRef", SqlDbType.NVarChar);
                            cmd.Parameters["@SpecimenRef"].Value = SampleRef;
                            //cmd.Parameters["@CubeID"].Value = cubeid;
                            cmd.Parameters.Add("@sampleMass", SqlDbType.Real);
                            cmd.Parameters["@sampleMass"].Value = sampleMass;
                            cmd.Parameters.Add("@MaxLoad", SqlDbType.Real);
                            cmd.Parameters["@MaxLoad"].Value = maxLoad;
                            cmd.Parameters.Add("@Remarks", SqlDbType.Text);
                            cmd.Parameters["@Remarks"].Value = CubeRemarks;
                            da.UpdateCommand = cmd;
                            da.Update(ds, "[Cube Details]");
                           
                           // da.Update()
                        }

                        
                        
                        if(lblSystem.Text.Equals("old"))
                        {
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = new SqlCommand(qry, connection);
                            da.SelectCommand.Parameters.Add("@cubeid", SqlDbType.Real);
                            da.SelectCommand.Parameters["@cubeid"].Value = cubeid;
                            DataSet ds = new DataSet();
                            da.Fill(ds, "[Cube Details]");
                            DataTable dt = ds.Tables["[Cube Details]"];

                            dt.Rows[0]["cubeid"] = cubeid;
                            SqlCommand cmd = new SqlCommand(upd, connection);
                            cmd.Parameters.Add("@cubeid", SqlDbType.Real);
                            cmd.Parameters["@cubeid"].Value = cubeid;
                            cmd.Parameters.Add("@sampleMass", SqlDbType.Real);
                            cmd.Parameters["@sampleMass"].Value = sampleMass;
                            cmd.Parameters.Add("@MaxLoad", SqlDbType.Real);
                            cmd.Parameters["@MaxLoad"].Value = maxLoad;
                            cmd.Parameters.Add("@Remarks", SqlDbType.Text);
                            cmd.Parameters["@Remarks"].Value = CubeRemarks;
                            da.UpdateCommand = cmd;
                            da.Update(ds, "[Cube Details]");

                            //save Testedby

                            /*
                            SqlDataAdapter da2 = new SqlDataAdapter();
                            da2.SelectCommand = new SqlCommand(qry2, connection);
                            da.SelectCommand.Parameters.Add("@JobRef", SqlDbType.NVarChar);
                            da.SelectCommand.Parameters["@JobRef"].Value = JobeRef;
                            DataSet ds2 = new DataSet();
                            da2.Fill(ds2, "[Cube Receiving]");
                            DataTable dt2 = ds2.Tables["[Cube Receiving]"];

                            dt2.Rows[0]["JobRef"] = cubeid;
                            SqlCommand cmd2 = new SqlCommand(upd2, connection);
                            cmd2.Parameters.Add("@JobRef", SqlDbType.NVarChar);
                            cmd2.Parameters["@JobRef"].Value = JobeRef;
                            cmd2.Parameters.Add("@TestedBy", SqlDbType.Int);
                            cmd2.Parameters["@TestedBy"].Value = TestedBy;
                            da2.UpdateCommand = cmd2;
                            da2.Update(ds2, "[Cube Receiving]");*/
                        }
                        
                        
                        //need yoooo
                        //cubeMaxLoad = 0;
                        //displayMaxLoadTextReadIn("");
                        displayTestingTextReadIn("Saved");
                        myListCount = myListCount - 1;
                        //lbltTotalCube.Text  = myListCount.ToString()

                        //livCubeTestList.Focus(); 
                        //livCubeTestList.Items[0].Selected = true;
                        //ListViewItem lv = livCubeTestList.FindItemWithText(txtBarCode.Text);

                        //lv.Remove();
                        removeLineFromCubeList(txtLastBarCode.Text);




                    }

                }
                catch (FormatException ex)
                {

                    MessageBox.Show(ex.Message);
                    //cubeMaxLoad = 0;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //cubeMaxLoad = 0;

                }


                finally
                {
                    connection.Close();
                    //cubeMaxLoad = 0;

                }

            }
        }

        



        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            //we must set the text of the host control so it will get copied to the combobox text.
            //This is done automatically by the combo box
            ribbonHost1.Text = monthCalendar1.SelectionStart.ToShortDateString();
            ribbonHost1.HostCompleted();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            setRS232();
        }

        private void setRS232()
        {
            ;
            initiateWeighingPort();
            initiateTestingPort();
            btnClose.Enabled = false; // disable close button
            btnOpen.Enabled = true; // enable open button
            btnSetRS232.Enabled = false; // disable setting button

      
            
        }

        private void initiateWeighingPort()
        {
            int baudRate = 9600;
            int dataBits = 8;
            Handshake handshake = Handshake.None;
            Parity parity = Parity.None;
            string portName = WCom;
            StopBits stopBits = StopBits.One;
    
            cubeWeighingPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            cubeWeighingPort.Handshake = handshake;
            cubeWeighingPort.ReadTimeout = 50;
            toolStripStatusLabel3.Text = cubeWeighingPort.PortName.ToString() + "/" + cubeWeighingPort.BaudRate.ToString() + "/" + cubeWeighingPort.Parity.ToString() + "/" + cubeWeighingPort.DataBits.ToString() + "/" + cubeWeighingPort.StopBits.ToString();
            cubeWeighingPort.DataReceived += new SerialDataReceivedEventHandler(weighingPort_LA600_DataReceived);
        
        
        }


        private void initiateTestingPort()
        {
            //Get name from Configuration 
            portName = TCom ;
             
            TestingMachine = MCSet;
            int baudRate=9600; 
            int dataBits = 8;
            Handshake handshake = Handshake.None;
            Parity parity = Parity.None;
            StopBits stopBits = StopBits.One;
            if (TestingMachine == "MC8") // Testing
            {
                baudRate = 9600;
                dataBits = 8;
                handshake = Handshake.None;
            }
            else if (TestingMachine == "MC7") // Matest
            {
                baudRate = 9600;
                dataBits = 8;
                handshake = Handshake.None;
            }
            else if (TestingMachine == "MC6")
            {
                baudRate = 9600;
                dataBits = 8;
                //handshake = Handshake.RequestToSendXOnXOff;
                handshake = Handshake.None;
            }
            else if (TestingMachine == "MCW1")
            {
                baudRate = 9600;
                dataBits = 8;
                //handshake = Handshake.RequestToSendXOnXOff;
                handshake = Handshake.None;
            }


            //M/C: ELE 
            else if (TestingMachine == "MC2")
            {
                 baudRate = 9600;
            }

            //M/C: Denison 
            else if (TestingMachine == "MC3")
            {
                baudRate = 9600;
                dataBits = 7;
                parity = Parity.Even;
                handshake = Handshake.RequestToSendXOnXOff;
            }

             //M/C: Denison 
            else if (TestingMachine == "MC4")
            {
                baudRate = 9600;
                dataBits = 7;
                parity = Parity.Even;
                handshake = Handshake.RequestToSendXOnXOff;
            }

            //M/C: ELE New 
            else if (TestingMachine == "MC5")
            {
                baudRate = 9600;
            }

           
           

            cubeTestingPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            cubeTestingPort.Handshake = handshake;

            if (TestingMachine == "MC8")
            {

                cubeTestingPort.ReceivedBytesThreshold = 4;
                cubeTestingPort.ReadTimeout = 1000;
                // cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(testingPortM1_DataReceived);
                cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(M8_DataReceived);


            }
            else if (TestingMachine == "MC7")
            {

                cubeTestingPort.ReceivedBytesThreshold = 4;
                cubeTestingPort.ReadTimeout = 1000;
                // cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(testingPortM1_DataReceived);
                cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(M7_DataReceived);


            }
            else if (TestingMachine == "MC6")
            {

                cubeTestingPort.ReceivedBytesThreshold = 4;
                cubeTestingPort.ReadTimeout = 1000;
                // cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(testingPortM1_DataReceived);
                cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(M6_DataReceived);
           

            }
            else if (TestingMachine == "MCW1")
            {

                cubeTestingPort.ReceivedBytesThreshold = 4;
                cubeTestingPort.ReadTimeout = 1000;
                // cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(testingPortM1_DataReceived);
                cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(MCW1_DataReceived);


            }

            else if (TestingMachine == "MC1")
            {

                cubeTestingPort.ReceivedBytesThreshold = 1;
                cubeTestingPort.ReadTimeout = 500;
                // cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(testingPortM1_DataReceived);
                cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(M1_DataReceived);


            }


            //M/C: ELE 
            else if (TestingMachine == "MC2")
            {
                cubeTestingPort.ReceivedBytesThreshold = 4;

                cubeTestingPort.ReadTimeout = 1000;
                
                //cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(testingPortM2_DataReceived);
                cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(M2_DataReceived);
            }

            //M/C: Denison
            else if (TestingMachine == "MC3")
            {
                //Chen Hong AAAAA
                //cubeTestingPort.RtsEnable = false;
                //cubeTestingPort.ReceivedBytesThreshold = 4;

                //cubeTestingPort.ReadTimeout = 1;

                //cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(testingPortM3_DataReceived);
                cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(M3_DataReceived);
            }

             //M/C: Denison
            else if (TestingMachine == "MC4")
            {
                //Chen Hong AAAAA
                //cubeTestingPort.RtsEnable = false;
                //cubeTestingPort.ReceivedBytesThreshold = 4;

                //cubeTestingPort.ReadTimeout = 1;

                //cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(testingPortM3_DataReceived);
                cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(M4_DataReceived);
            }

            //M/C: ELE 
            else if (TestingMachine == "MC5")
            {
                cubeTestingPort.ReceivedBytesThreshold = 4;

                cubeTestingPort.ReadTimeout = 1000;

                //cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(testingPortM5_DataReceived);
                cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler(M5_DataReceived);
            }
            toolStripStatusLabel4.Text = cubeTestingPort.PortName.ToString() + "/" + cubeTestingPort.BaudRate.ToString() + "/" + cubeTestingPort.Parity.ToString() + "/" + cubeTestingPort.DataBits.ToString() + "/" + cubeTestingPort.StopBits.ToString();



        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openRS232Port();

        }

        private void openRS232Port()
        {
            openWeighingPort();
            openTestingPort();
        }
       
        private void openTestingPort()
        {
            try
            {
                cubeTestingPort.Open(); // Open the port

                if (cubeTestingPort.IsOpen)
                {
                    // clear the RCVbox text string and write the VER command
                    //textBox1.Text = cubeTestingLineReadIn = "";
                    cubeMaxLoad = 0;
                    txtLoadLive.Text = cubeTestingRead = "";
                    //cubeTestingPort.Write("VER\r");
                    

                    // set the 'invoke' delegate and attach the 'receive-data' function
                    // to the serial port 'receive' event.
                    accessControlFromCentralThreadTest = displayTestingTextReadIn;
                    accessControlFromCentralThreadSampleMass = displaySampleMassTextReadIn;
                    accessControlFromCentralThreadMaxLoad = displayMaxLoadTextReadIn;
                    accessControlFromCentralThreadLastCubeID = displayLastCubeIDTextReadIn;
                    accessControlFromCentralThreadListView = removeLineFromCubeList;
                    accessControlFromCentralThreadLastBarcode = displayLastBarcodeTextReadIn;
                    accessControlFromCentralThreadLastCubeRef = displayLastSpecimenRefTextReadIn;
                    //CCCC
                    //cubeTestingPort.DataReceived += new SerialDataReceivedEventHandler;
                   

                }
                btnClose.Enabled = true; // enable close button 
                btnSetRS232.Enabled = false; // disable setting button
                btnOpen.Enabled = false; // disable open button 
                //txtCompany.Text = "Waiting for incoming data...";
                //Console.WriteLine("Waiting for incoming data...");

                //Console.ReadKey();
            }
            catch(TimeoutException e)
            {
                MessageBox.Show("Time Out : " +e.Message);
            }
            catch (Exception ex)
            {
                txtCompany.Text = "Port does not existing...";
                MessageBox.Show(ex.Message);
                btnSetRS232.Enabled = true;
                btnOpen.Enabled = true;
            }

            
            finally
            {
                btnOpen.Enabled = false; // disable open button 
            }
        }

        private void openWeighingPort()
        {
            try
            {
                cubeWeighingPort.Open(); // Open the port

                if (cubeWeighingPort.IsOpen)
                {
                    // clear the RCVbox text string and write the VER command
                    //textBox1.Text = cubeWeighingLineReadIn = "";
                    //cubeMass = 0;
                    txtLoadLive.Text = cubeWeighingRead = "";
                    //cubeWeighingPort.Write("VER\r");

                    
                    // set the 'invoke' delegate and attach the 'receive-data' function
                    // to the serial port 'receive' event.
                    accessControlFromCentralThreadWeight = displayWeighingTextReadIn;
                    //port.DataReceived += new SerialDataReceivedEventHandler;


                }
                btnClose.Enabled = true; // enable close button
                btnSetRS232.Enabled = false; // disable setting button
                btnOpen.Enabled = false; // disable open button 
                //txtCompany.Text = "Waiting for incoming data...";
                //Console.WriteLine("Waiting for incoming data...");

                //Console.ReadKey();
            }
            catch (TimeoutException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception ex)
            {
                txtCompany.Text = "Port does not existing...";
                MessageBox.Show(ex.Message);
                btnSetRS232.Enabled = true;
                btnOpen.Enabled = true;
            }
                

            finally
            {
                btnOpen.Enabled = false; // disable open button 
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            TestingStarted = false;
            /*
            try
            {
                cubeTestingPort.DiscardInBuffer();

                cubeTestingPort.Close(); // Close the port
                cubeWeighingPort.DiscardInBuffer();

                cubeWeighingPort.Close(); // Close the port
                 
            }
            catch (Exception ex)
            {
                txtCompany.Text = "Port does not opened...";
                MessageBox.Show(ex.Message);
                btnClose.Enabled = false;
                btnOpen.Enabled = false;
                btnSetRS232.Enabled = true;



            }
            finally
            {
                txtCompany.Text = "Port Closed...";
                btnOpen.Enabled = true; // enable open button
                btnSetRS232.Enabled = true; // enable setting button
                btnClose.Enabled = false; // disable close button 
            }
            */
        }


        void weighingPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (cubeWeighingPort.BytesToRead > 0)
                {

                    //cubeWeighingRead = cubeWeighingPort.ReadExisting();
                    //new error hear
                    cubeWeighingRead = cubeWeighingPort.ReadLine();
                    //Thread.Sleep(1);  //cannot sleep even 1 
                    //cubeWeighingRead = "+ 8.8650 kg\r\n+ 9.8650 kg\r\n+19.8650 kg\r\n";
                    if (cubeWeighingRead.Length > 8)
                    {
                        cubeWeighingRead = cubeWeighingRead.Substring(0, cubeWeighingRead.Length - 4);
                        //MessageBox.Show(cubeWeighingRead.ToString());
                        int len = cubeWeighingRead.Length;
                        int first = cubeWeighingRead.LastIndexOf("\r\n");

                        if (first > 0)
                        {
                            cubeWeighingRead = cubeWeighingRead.Substring(first + 2, len - first - 3);
                            //cubeWeighingRead = cubeWeighingRead.Substring(13, 8);
                        }
                        //MessageBox.Show(cubeWeighingRead.ToString());
                    }
                    //cubeWeighingRead = cubeWeighingRead.Substring(1, 10);

                    //cubeWeighingLineReadIn = cubeWeighingLineReadIn + cubeWeighingRead;

                    //try remove sleep here
                    //Thread.Sleep(50);

                    displayWeighingTextReadIn(cubeWeighingRead);
                    //displayWeighingTextReadIn(cubeWeighingLineReadIn);

                }
                else
                {
                    MessageBox.Show("end of reading");

                }
            }
            catch (TimeoutException)
            {
                MessageBox.Show("time out of weighing");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Weighing Error \r\n" + ex.Message);

            }
        }

        void weighingPort_LA600_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (cubeWeighingPort.BytesToRead > 0)
                {
                    cubeWeighingRead = cubeWeighingPort.ReadLine();
                    //cubeWeighingRead = "st,gs      1542\r\n";

                    if (cubeWeighingRead == null || cubeWeighingRead.Length == 0)
                        return;

                    string[] ss = splitStringsAndIgnoreWhiteSpaces(cubeWeighingRead);
                    if (ss.Length == 2)
                    {
                        string[] tt = ss[1].Trim().Split(',');
                        if(tt.Length == 2)
                        {
                            cubeWeighingRead = tt[0].Trim();
                        }
                    }

                    displayWeighingTextReadIn(cubeWeighingRead);

                }
                else
                {
                    MessageBox.Show("end of reading");

                }
            }
            catch (TimeoutException)
            {
                MessageBox.Show("time out of weighing");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Weighing Error \r\n" + ex.Message);

            }
        }

        void M7_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                if (cubeTestingPort.BytesToRead > 0)
                {
                    if (TestingStarted == false)
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        TestingStarted = true;
                        //cubeTimer.Start();
                    }

                    //cubeTestingRead = cubeTestingPort.ReadLine();
                    cubeTestingRead = cubeTestingPort.ReadExisting();
                    //Thread.Sleep(50);
                    cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;
                    int all = cubeTestingLineReadIn.IndexOf("$ALL");
                    if (all != 0 && all != -1)
                    {
                        cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(all, cubeTestingLineReadIn.Length - all);

                    }
                    M7displayData();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("M7" + ex.Message);
                cubeTestingLineReadIn = "";
            }

        }

        void M8_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                if (cubeTestingPort.BytesToRead > 0)
                {
                    if (TestingStarted == false)
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        TestingStarted = true;
                        //cubeTimer.Start();
                    }

                    cubeTestingRead = cubeTestingPort.ReadLine();
                    cubeTestingRead = cubeTestingRead.Trim();
                    cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;
                    M8displayData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("M8_DataReceived error: " + ex.Message);
                cubeTestingLineReadIn = "";
            }
        }

        void M1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                if (cubeTestingPort.BytesToRead > 0)
                {
                    if (TestingStarted == false)
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        TestingStarted = true;
                        cubeTimer.Start();
                    }

                    //cubeTestingRead = cubeTestingPort.ReadLine();
                    cubeTestingRead = cubeTestingPort.ReadExisting();
                    cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;

                    M1displayData();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("M1 Data Error: \r\n" + ex.Message);
                cubeTestingLineReadIn = "";
            }

        }

        void M6_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //MessageBox.Show("M6" );
                //Thread.Sleep(500);
                if (cubeTestingPort.BytesToRead > 0)
                {
                    cubeTestingRead = cubeTestingPort.ReadLine();
                    //Thread.Sleep(500);
                    if (cubeTestingRead.Length == 26)
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        cubeTestingRead = cubeTestingRead.Substring(0, cubeTestingRead.Length - 2);
                        cubeTestingRead = cubeTestingRead.Substring(17);
                        cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;
                        M6displayData();
   
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("M6 R" + ex.Message);
                cubeTestingLineReadIn = "";
            }

        }

        void MCW1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                if (cubeTestingPort.BytesToRead > 0) 
                {
                    cubeTestingRead = cubeTestingPort.ReadLine();                                        
                    if (!string.IsNullOrEmpty(cubeTestingRead))
                    {

                        if (TestingStarted == false)
                                cubeTimer.Start();
                        if ((cubeTestingRead.Length - 6) > 0)
                        {                        
                            cubeTestingRead = cubeTestingRead.Substring(cubeTestingRead.Length - 6, 6);
                            double value = Convert.ToDouble(cubeTestingRead);
                            WeightValue.Add(value);
                            TestingStarted = true;
                        }
                    }                    


                }
                //else if(WeightValue.Count != 0)
                //{
                //    WeightValue.Sort();
                //    displayLastCubeIDTextReadIn(txtID.Text);
                //    displayLastBarcodeTextReadIn(txtBarCode.Text);
                //    displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                //    displaySampleMassTextReadIn(txtSampleMass.Text);
                //    displayMaxLoadTextReadIn("");                   
                //    MCW1displayData();

                //}

                //if (cubeTestingPort.BytesToRead > 0)
                //{
                //    cubeTestingRead = cubeTestingPort.ReadLine();
                //    if (cubeTestingRead.Length == 26)
                //    {
                //        displayLastCubeIDTextReadIn(txtID.Text);
                //        displayLastBarcodeTextReadIn(txtBarCode.Text);
                //        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                //        displaySampleMassTextReadIn(txtSampleMass.Text);
                //        displayMaxLoadTextReadIn("");
                //        cubeTestingRead = cubeTestingRead.Substring(0, cubeTestingRead.Length - 2);
                //        cubeTestingRead = cubeTestingRead.Substring(17);
                //        cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;
                //        MCW1displayData();
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("MCW1 R" + ex.Message);
                cubeTestingLineReadIn = "";
            }

        }

        void M2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                if (cubeTestingPort.BytesToRead > 0)
                {
                    if (TestingStarted == false)
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        TestingStarted = true;
                        //cubeTimer.Start();
                    }

                    //cubeTestingRead = cubeTestingPort.ReadLine();
                    cubeTestingRead = cubeTestingPort.ReadExisting();
                    //Thread.Sleep(50);
                    cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;
                    int all = cubeTestingLineReadIn.IndexOf("$ALL");
                    if (all != 0 && all != -1)
                    {
                        cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(all, cubeTestingLineReadIn.Length - all);

                    }
                    M2displayData();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("M2" + ex.Message);
                cubeTestingLineReadIn = "";
            }

        }

        void M5_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                if (cubeTestingPort.BytesToRead > 0)
                {
                    if (TestingStarted == false)
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        TestingStarted = true;
                        //cubeTimer.Start();
                    }

                    //cubeTestingRead = cubeTestingPort.ReadLine();
                    cubeTestingRead = cubeTestingPort.ReadExisting();
                    //Thread.Sleep(50);
                    cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;
                    int all = cubeTestingLineReadIn.IndexOf("$ALL");
                    if (all != 0 && all != -1)
                    {
                        cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(all, cubeTestingLineReadIn.Length - all);

                    }
                    M5displayData();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("M5" + ex.Message);
                cubeTestingLineReadIn = "";
            }

        }
        void M3_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //MessageBox.Show("M3" );
                Thread.Sleep(500);
                if (cubeTestingPort.BytesToRead > 0)
                {
                    Thread.Sleep(500);
                    if (TestingStarted == false)
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        TestingStarted = true;
                        //cubeTimer.Start();
                    }
                    
                        //CHEN HONG 008
                       // cubeTestingRead = cubeTestingPort.ReadLine();
                    //cubeTestingRead = cubeTestingPort.ReadLine();
                    cubeTestingRead = cubeTestingPort.ReadExisting();

                    //Thread.Sleep(50);
                    cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;
                    //int all = cubeTestingLineReadIn.IndexOf("$ALL");
                    //if (all != 0 && all != -1)
                    //{
                    //    cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(all, cubeTestingLineReadIn.Length - all);

                    //}
                    M3displayData();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("M3 R" + ex.Message);
                cubeTestingLineReadIn = "";
            }

        }

        void M4_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //MessageBox.Show("M3" );
                Thread.Sleep(500);
                if (cubeTestingPort.BytesToRead > 0)
                {
                    Thread.Sleep(500);
                    if (TestingStarted == false)
                    {
                        displayLastCubeIDTextReadIn(txtID.Text);
                        displayLastBarcodeTextReadIn(txtBarCode.Text);
                        displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                        displaySampleMassTextReadIn(txtSampleMass.Text);
                        displayMaxLoadTextReadIn("");
                        TestingStarted = true;
                        //cubeTimer.Start();
                    }

                    //CHEN HONG 008
                    // cubeTestingRead = cubeTestingPort.ReadLine();
                    //cubeTestingRead = cubeTestingPort.ReadLine();
                    cubeTestingRead = cubeTestingPort.ReadExisting();

                    //Thread.Sleep(50);
                    cubeTestingLineReadIn = cubeTestingLineReadIn + cubeTestingRead;
                    //int all = cubeTestingLineReadIn.IndexOf("$ALL");
                    //if (all != 0 && all != -1)
                    //{
                    //    cubeTestingLineReadIn = cubeTestingLineReadIn.Substring(all, cubeTestingLineReadIn.Length - all);

                    //}
                    M4displayData();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("M4 R" + ex.Message);
                cubeTestingLineReadIn = "";
            }

        }


        void cubeTimer_Elapsed(object sender, EventArgs e)
        {
            if (WeightValue.Count != 0)
            {
                int a=WeightValue.Count();
                Thread.Sleep(400);
                int b = WeightValue.Count();
                if (a == b)
                {
                    cubeTimer.Stop();
                    WeightValue.Sort();
                    displayLastCubeIDTextReadIn(txtID.Text);
                    displayLastBarcodeTextReadIn(txtBarCode.Text);
                    displayLastSpecimenRefTextReadIn(txtSpecimenRef.Text);
                    displaySampleMassTextReadIn(txtSampleMass.Text);
                    displayMaxLoadTextReadIn("");
                    MCW1displayData();                    
                }
            }            

            //if (cubeTestingLineReadIn == timerRead && cubeTestingLineReadIn != "" && cubeMaxLoad!=0)
            //{
            //    //after 1 second no new reading
            //    //MessageBox.Show("M3");
            //    if (TestingStarted == true)
            //    {
            //        TestingStarted = false;
            //        cubeTimer.Stop();
            //        //CCCC
            //        //displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
            //        MC1MaxLoad();
            //        saveLoad();
            //        cubeMaxLoad = 0;
            //        cubeTestingRead = "";
            //        cubeTestingLineReadIn = "";
            //    }


            //}
            //cubeMaxLoad = 1;
            //timerRead = cubeTestingLineReadIn;



        }

        void M8displayData()
        {
            if (cubeTestingRead.Length > 1)
            {
                cubeMaxLoad = Convert.ToDouble(cubeTestingRead);
                displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                Thread.Sleep(50);
                saveLoad();
                cubeMaxLoad = 0;
                cubeTestingRead = "";
                cubeTestingLineReadIn = "";
            }
        }

        void M7displayData()
        {
            if (cubeTestingRead.Length > 1)
            {
                int end = cubeTestingRead.IndexOf("$$C");
                string findMaxload = "";
                if (end == -1)
                {
                    //cubeTestingLineReadIn = cubeTestingLineReadIn + readln;
                    end = cubeTestingLineReadIn.IndexOf("$$C");
                    if (end == -1) // not final result, show the instantanious value
                    {
                        end = cubeTestingLineReadIn.LastIndexOf("$ALL");
                        if (end >= 0)
                        {
                            string subs = cubeTestingLineReadIn.Substring(end);
                            string[] splitted = splitStringsAndIgnoreWhiteSpaces(subs);
                            if (splitted.Length >= 3)
                            {
                                double db = Convert.ToDouble(splitted[1]);
                                displayTestingTextReadIn(db.ToString());
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        findMaxload = cubeTestingLineReadIn.Substring(end, cubeTestingLineReadIn.Length - end);
                        string[] splitted = splitStringsAndIgnoreWhiteSpaces(findMaxload);
                        if (splitted.Length >= 18)
                        {
                            cubeMaxLoad = Convert.ToDouble(splitted[17]);
                            displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                            Thread.Sleep(50);
                            saveLoad();
                            cubeMaxLoad = 0;
                            cubeTestingRead = "";
                            cubeTestingLineReadIn = "";
                            TestingStarted = false;
                        }

                    }
                }
                else
                {
                    Thread.Sleep(500);
                    findMaxload = cubeTestingRead.Substring(end, cubeTestingRead.Length - end);
                    string[] splitted = splitStringsAndIgnoreWhiteSpaces(findMaxload);
                    if (splitted.Length >= 18)
                    {
                        cubeMaxLoad = Convert.ToDouble(splitted[17]);
                        displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                        Thread.Sleep(50);
                        saveLoad();
                        cubeMaxLoad = 0;
                        cubeTestingRead = "";
                        cubeTestingLineReadIn = "";
                        TestingStarted = false;
                    }
                }

            }
            else
            {
                MessageBox.Show("no data");
            }


        }

        void M1displayData()
        {

            try
            {
                int firstComa = cubeTestingRead.IndexOf(",");
                int lenthComa = cubeTestingRead.Length;
                cubeTestingRead = cubeTestingRead.Substring(firstComa + 1, lenthComa - firstComa - 2);
                int lastComa = cubeTestingRead.IndexOf(",");
                cubeTestingRead = cubeTestingRead.Substring(0, lastComa);
                displayTestingTextReadIn(cubeTestingRead);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("M1 Data input error" + ex.Message);

            }
            /*
                        //int lastComa = cubeTestingRead.LastIndexOf(",");
                        //MessageBox.Show(firstComa.ToString() + ";" + lastComa.ToString());
                        if (firstComa > 0 && lastComa > 0 && firstComa != lastComa)
                        {
                            //cubeTestingRead = cubeTestingRead.Substring(firstComa+1, lastComa - firstComa - 1);
                            //Check RS232 Cable

                            cubeTestingRead = cubeTestingRead.Substring(firstComa+1, lastComa - firstComa - 1);

                            cubeTestingRead = cubeTestingRead.Trim();
                            //MessageBox.Show(cubeTestingRead);
                        }
 
 
                        if (cubeTestingRead.Length > 1)
                        {
                            displayTestingTextReadIn(cubeTestingRead);

                            string str2 = txtLoadLive.Text;
                            double l = 0;
                            if (str2.Length > 0)
                            {

                                l = Convert.ToDouble(str2);

                            }
                            if (l >= cubeMaxLoad)
                            {
                                cubeMaxLoad = l;

                            }



                        }
             */
        }

        void M6displayData()
        {
            if (cubeTestingRead.Length == 7)
            {
                cubeMaxLoad = Convert.ToDouble(cubeTestingRead.Substring(0, 7).Trim());
                displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                Thread.Sleep(50);
                saveLoad();
                cubeMaxLoad = 0;
                cubeTestingRead = "";
                cubeTestingLineReadIn = "";
            }

        }

        void MCW1displayData()
        {
            if (WeightValue.Last()>0)
            {
                cubeMaxLoad = WeightValue.Last();
                displayMaxLoadTextReadIn(cubeMaxLoad.ToString());                
                saveLoad();
                cubeMaxLoad = 0;
                cubeTestingRead = "";
                cubeTestingLineReadIn = "";
                WeightValue = new List<double>();
                TestingStarted = false;
            }
        }

        void M2displayData()
        {
            if (cubeTestingRead.Length > 1)
            {
                int end = cubeTestingRead.IndexOf("$C");
                string findMaxload = "";
                if (end == -1)
                {
                    //cubeTestingLineReadIn = cubeTestingLineReadIn + readln;
                    end = cubeTestingLineReadIn.IndexOf("$C");
                    if (end == -1)
                    {
                        int s = Convert.ToInt32(cubeTestingLineReadIn.Length / 33);
                        if (s > 0)
                        {
                            end = cubeTestingLineReadIn.IndexOf("$ALL");
                            if (end == 0)
                            {
                                //readln = cubeTestingLineReadIn.Substring((s - 1) * 33 + 17, 12);
                                double db = Convert.ToDouble(cubeTestingLineReadIn.Substring((s - 1) * 33 + 5, 12).Trim());
                                displayTestingTextReadIn(db.ToString());
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        findMaxload = cubeTestingLineReadIn.Substring(end, cubeTestingLineReadIn.Length - end);
                        if (findMaxload.Length >= 123)
                        {
                            cubeMaxLoad = Convert.ToDouble(findMaxload.Substring(98, 12).Trim());
                            //cubeTestingLineReadIn = cubeMaxLoad.ToString();
                            displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                            Thread.Sleep(50);
                            saveLoad();
                            cubeMaxLoad = 0;
                            cubeTestingRead = "";
                            cubeTestingLineReadIn = "";
                            TestingStarted = false;
                        }

                    }
                }
                else
                {
                    Thread.Sleep(500);
                    findMaxload = cubeTestingRead.Substring(end, cubeTestingRead.Length - end);
                    if (findMaxload.Length >= 123)
                    {
                        cubeMaxLoad = Convert.ToDouble(findMaxload.Substring(98, 12).Trim());
                        //cubeTestingLineReadIn = cubeMaxLoad.ToString();
                        displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                        Thread.Sleep(50);
                        saveLoad();
                        cubeMaxLoad = 0;
                        cubeTestingRead = "";
                        cubeTestingLineReadIn = "";
                        TestingStarted = false;

                    }
                }

            }
            else
            {
                MessageBox.Show("no data");
            }
            
         
        }

        void M5displayData()
        {

            if (cubeTestingRead.Length > 1)
            {
                int end = cubeTestingRead.IndexOf("$$C");
                string findMaxload = "";
                if (end == -1)
                {
                    //cubeTestingLineReadIn = cubeTestingLineReadIn + readln;
                    end = cubeTestingLineReadIn.IndexOf("$$C");
                    if (end == -1)
                    {
                        int s = Convert.ToInt32(cubeTestingLineReadIn.Length / 33);
                        if (s > 0)
                        {
                            end = cubeTestingLineReadIn.IndexOf("$ALL");
                            if (end == 0)
                            {
                                //readln = cubeTestingLineReadIn.Substring((s - 1) * 33 + 17, 12);
                                double db = Convert.ToDouble(cubeTestingLineReadIn.Substring((s - 1) * 33 + 5, 12).Trim());
                                displayTestingTextReadIn(db.ToString());
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        findMaxload = cubeTestingLineReadIn.Substring(end, cubeTestingLineReadIn.Length - end);
                        if (findMaxload.Length >= 160)
                        {
                            cubeMaxLoad = Convert.ToDouble(findMaxload.Substring(135, 12).Trim());
                            //cubeTestingLineReadIn = cubeMaxLoad.ToString();
                            displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                            Thread.Sleep(50);
                            saveLoad();
                            cubeMaxLoad = 0;
                            cubeTestingRead = "";
                            cubeTestingLineReadIn = "";
                            TestingStarted = false;
                        }

                    }
                }
                else
                {
                    Thread.Sleep(500);
                    findMaxload = cubeTestingRead.Substring(end, cubeTestingRead.Length - end);
                    if (findMaxload.Length >= 160)
                    {
                        cubeMaxLoad = Convert.ToDouble(findMaxload.Substring(135, 12).Trim());
                        //cubeTestingLineReadIn = cubeMaxLoad.ToString();
                        displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                        Thread.Sleep(50);
                        saveLoad();
                        cubeMaxLoad = 0;
                        cubeTestingRead = "";
                        cubeTestingLineReadIn = "";
                        TestingStarted = false;

                    }
                }

            }
            else
            {
                MessageBox.Show("no data");
            }
        }

        string[] splitStringsAndIgnoreWhiteSpaces(string input)
        {
            string[] split1 = input.Split(null);
            List<string> split2 = new List<string>(split1);
            split2.RemoveAll(s => s.Trim().Equals(""));
            return split2.ToArray();
        }

        void M3displayData()
        {
            try
            {
                if (cubeTestingRead.Length > 146)
                {
                    cubeMaxLoad = Convert.ToDouble(cubeTestingRead.Substring(127, 6).Trim());
                    //cubeTestingLineReadIn = cubeMaxLoad.ToString();
                    displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                    Thread.Sleep(50);
                    saveLoad();
                    cubeMaxLoad = 0;
                    cubeTestingRead = "";
                    cubeTestingLineReadIn = "";
                    TestingStarted = false;



                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("M3 Data input error" + ex.Message);

            }

        }

        void M4displayData()
        {
            try
            {
                if (cubeTestingRead.Length > 146)
                {
                    cubeMaxLoad = Convert.ToDouble(cubeTestingRead.Substring(127, 6).Trim());
                    //cubeTestingLineReadIn = cubeMaxLoad.ToString();
                    displayMaxLoadTextReadIn(cubeMaxLoad.ToString());
                    Thread.Sleep(50);
                    saveLoad();
                    cubeMaxLoad = 0;
                    cubeTestingRead = "";
                    cubeTestingLineReadIn = "";
                    TestingStarted = false;


                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("M3 Data input error" + ex.Message);

            }

        }


        private void rbtnTestAll_Click(object sender, EventArgs e)
        {
            addCubeTestList("all");
        }
        private void rbtnTestDone_Click(object sender, EventArgs e)
        {
            addCubeTestList("done");
        }
        private void rbtnTestPending_Click(object sender, EventArgs e)
        {
            addCubeTestList("pending");
        }
   
        private void txtBarCode_TextChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("TexChanged");
           /*
            System.Media.SystemSounds.Asterisk.Play();
            String text = txtBarCode.Text;
            String rightStr = text;
            if (text.Length >1)
            {
                rightStr = text.Substring(text.Length - 1);
            }
            if (rightStr == "\r\n")
            {
                //System.Media.SystemSounds.Beep.Play();
                //System.Media.SystemSounds.Asterisk.Play();
                //System.Media.SystemSounds.Exclamation.Play();
                //System.Media.SystemSounds.Question.Play();
                System.Media.SystemSounds.Hand.Play();
                
                //saveLoad();
            }
            */
        } 
       
        // this, hopefully, will prevent cross threading.


        private void displayTestingTextReadIn(string ToBeDisplayed)
        {

            if (txtLoadLive.InvokeRequired)
            {
                 txtLoadLive.Invoke(accessControlFromCentralThreadTest, ToBeDisplayed);
            }
            else
            {
                txtLoadLive.Text = ToBeDisplayed;
            }
        }

        private void displayMaxLoadTextReadIn(string ToBeDisplayed)
        {

            if (txtMaxLoadCurrent.InvokeRequired)
            {
                txtMaxLoadCurrent.Invoke(accessControlFromCentralThreadMaxLoad, ToBeDisplayed);
            }
            else
            {
                txtMaxLoadCurrent.Text = ToBeDisplayed;
            }
        }
        private void displayLastCubeIDTextReadIn(string ToBeDisplayed)
        {

            if (txtLastCubeID.InvokeRequired)
            {
                txtLastCubeID.Invoke(accessControlFromCentralThreadLastCubeID, ToBeDisplayed);
            }
            else
            {
                txtLastCubeID.Text = ToBeDisplayed;
            }
        }
        private void displayLastBarcodeTextReadIn(string ToBeDisplayed)
        {

            if (txtLastBarCode.InvokeRequired)
            {
                txtLastBarCode.Invoke(accessControlFromCentralThreadLastBarcode, ToBeDisplayed);
            }
            else
            {
                txtLastBarCode.Text = ToBeDisplayed;
            }
        }



        private void displayWeighingTextReadIn(string ToBeDisplayedWeight)
        {



            if (txtWeightLive.InvokeRequired)
            {
                txtWeightLive.Invoke(accessControlFromCentralThreadWeight, ToBeDisplayedWeight);
            }
            else
            {
                txtWeightLive.Text = ToBeDisplayedWeight;
            }
            
        }

        private void displaySampleMassTextReadIn(string ToBeDisplayedWeight)
        {

            

            if (txtSampleMassCurrent.InvokeRequired)
            {
                txtSampleMassCurrent.Invoke(accessControlFromCentralThreadSampleMass, ToBeDisplayedWeight);
            
            }
            else
            {
                txtSampleMassCurrent.Text = ToBeDisplayedWeight;
            }

        }

        

        private void displayLastSpecimenRefTextReadIn(string ToBeDisplayedWeight)
        {

            if (txtSpecimenRefCurrent.InvokeRequired)
            {
                txtSpecimenRefCurrent.Invoke(accessControlFromCentralThreadLastCubeRef, ToBeDisplayedWeight);
            
            }
            else
            {
                txtSpecimenRefCurrent.Text = ToBeDisplayedWeight;
            }

        }


        private void removeLineFromCubeList(string ToBeRemoved)
        {
            if (livCubeTestList.InvokeRequired)
            {
                livCubeTestList.Invoke(accessControlFromCentralThreadListView, ToBeRemoved);
            }
            else
            {

                ListViewItem lv = livCubeTestList.FindItemWithText(ToBeRemoved);
                lv.Remove();
            }

        }

        private void checkWeight()
        {
            string wt = txtWeightLive.Text;
            double sampleWt = Convert.ToDouble(wt.Substring(0, 7));

            if (sampleWt >= 2.0)
            {
                txtSampleMassCurrent.Text = sampleWt.ToString();
            }
            else
            {
                txtSampleMassCurrent.Text = "";
            }

        }
        private void machineSet()
        {
            try
            {

                string line = "";
                int count = 0;
                System.IO.StreamReader file = new System.IO.StreamReader("C:\\CubeTest\\COMPORT.ini");
                while ((line = file.ReadLine()) != null)
                {
                    count = line.IndexOf("MC:");
                    if (count != -1)
                    {
                        MCSet = line.Substring(count + 3, line.Length - count - 3);

                    }
                    count = line.IndexOf("WCOM:");
                    if (count != -1)
                    {
                        WCom = line.Substring(count + 5, line.Length - count - 5);

                    }
                    count = line.IndexOf("TCOM:");
                    if (count != -1)
                    {
                        TCom = line.Substring(count + 5, line.Length - count - 5);

                    }
                    count = line.IndexOf("TESTER:");
                    if (count != -1)
                    {
                        
                        Tester = line.Substring(count + 7, line.Length - count - 7);

                    }



                }

                file.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Initial Setting Error \r\n" + ex.Message);
            }
            

        }

        private void MC1MaxLoad()
        {
            double myMaxLoad=0.0;
            try
            {

                string line = "";
                string dataImput = "";
                int count = 0;
                string loadstring = "";
                
                
                //System.IO.StreamReader file = new System.IO.StreamReader("C:\\CubeTest\\M1.txt");
                //dataImput = file.ReadToEnd();

                dataImput = cubeTestingLineReadIn;

                while (dataImput.Length > 0)
                {
                    
                    
                    count = dataImput.IndexOf(";");
                    if (count > 0)
                    {
                        line = dataImput.Substring(0, count);
                        int firstComa = line.IndexOf(",");
                        int lenthComa = line.Length;
                        line = line.Substring(firstComa + 1, lenthComa - firstComa - 2);
                        int lastComa = line.IndexOf(",");
                        loadstring = line.Substring(0, lastComa);

                        double l = 0;
                        if (loadstring.Length > 0)
                        {

                            l = Convert.ToDouble(loadstring);

                        }
                        if (l >= myMaxLoad)
                        {
                            myMaxLoad = l;

                        }


                        
                        if (dataImput.Length > count + 3)
                        {
                            dataImput = dataImput.Substring(count + 3);

                        }
                        else
                        {
                            //MessageBox.Show("Data Imput End  \r\n" + dataImput);
                            dataImput = "";
                        }
                    

                    }
                    else
                    {
                        dataImput = "";
                    }

                
                    

                }

                

            }
            catch (Exception ex)
            {
                MessageBox.Show("Data Imput Error \r\n" + ex.Message);
            }
            finally 
            {
                displayMaxLoadTextReadIn(myMaxLoad.ToString());
                // For this machine, in most time the difference between data output and machine disply is 0.1 to 0.2 
                myMaxLoad = myMaxLoad + 0.2;
                //MessageBox.Show(myMaxLoad.ToString());
            }
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }




    
    }
      


    
}

    
    
    
        
     

        

   


