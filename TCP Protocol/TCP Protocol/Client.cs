using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace TCP_Protocol
{
    public partial class Client : Form
    {
        Socket client;
        byte[] buffer = new byte[1024];

        public Client()
        {
            InitializeComponent();
            SetIPAddress();
        }

        private void SetIPAddress() {

            string hostname  = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostByName(hostname);
            txtIP.Text = ipHost.AddressList[ipHost.AddressList.Length - 1].ToString();
            //calling in constructor
        }
        private void UpdateGUI(bool isConnected )
        {
            btnConnect.Enabled = !isConnected;
            btnSend.Enabled = isConnected;
            lblStatus.Text = isConnected ? "Connected" : "DisConnected";
            lblStatus.ForeColor = isConnected ? Color.Green : Color.Red;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // Connect To Server 
            client = new Socket(AddressFamily.InterNetwork , SocketType.Stream ,
                ProtocolType.Tcp);
            int PortNum = int.Parse(txtPort.Text);
            IPAddress ip = IPAddress.Parse(txtIP.Text);
            client.Connect(new IPEndPoint(ip,PortNum));
            UpdateGUI(true);
            WriteLog("Connected To Server .........."+ Environment.NewLine);

            WaitingForData(client);
        }

        private void WaitingForData(Socket client)
        {
          client.BeginReceive(buffer , 0 ,buffer.Length,SocketFlags.None ,
              new AsyncCallback(OnReceive),null);
        }

        private void OnReceive(IAsyncResult ar ) {

            client.EndReceive(ar);
            string msg = Encoding.Unicode.GetString(buffer);
            WriteLog(msg);
            WaitingForData(client);
        
        }
        private void WriteLog(string msg) {


            MethodInvoker invoke = new MethodInvoker(delegate {
                txtLog.AppendText("Server Said :" + msg + Environment.NewLine);
            });
            this.BeginInvoke(invoke);
        
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMsg.Text;
            buffer = Encoding.Unicode.GetBytes(msg);

            client.Send(buffer);

        }
    }
}
