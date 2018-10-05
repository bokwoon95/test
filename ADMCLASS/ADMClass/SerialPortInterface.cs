using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Threading;

 


namespace ADMClass
{
    public class SerialPortInterface
    {
        /*
         private SerialPort cubeTestingPort; // use private field for SerialPort
         private SerialPort cubeWeighingPort; // use private field for SerialPort
         private string comPort;
         private int countIn;
         private string cubeTestingLineReadIn;
         private string cubeTestingRead;
         private string cubeWeighingLineReadIn;
         private string cubeWeighingRead;

         private void tcBalance()
         {
             comPort = "COM4";
             cubeWeighingPort = new SerialPort(comPort, 115200, Parity.None, 8, StopBits.One); // now instantiates field
             cubeWeighingPort.DataReceived += new SerialDataReceivedEventHandler(weighingPort_DataReceived);
        }

         void weighingPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
         {

           
             if (cubeTestingPort.BytesToRead > 0)
             {
                 countIn = countIn + 1;
                 cubeWeighingRead = cubeWeighingPort.ReadExisting();
                 cubeWeighingLineReadIn = cubeWeighingPort + cubeWeighingRead;
                 Thread.Sleep(50);
                 //displayTextReadIn(cubeWeighingRead);
                 //displayTextReadIn(cubeWeighingLineReadIn);

             }
             else
             {
                 //MessageBox.Show("end of reading");

             }

        
         }
 */


    }
}
