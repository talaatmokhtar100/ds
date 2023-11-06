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
    public partial class Form1 : Form
    {
        Socket Server;
        List<SocketPacket> Clients = new List<SocketPacket>();
        public Form1()
        {
            InitializeComponent();
        }
        private void UpdateGUI(bool isConnected)
        {
            btnStart.Enabled = !isConnected;
            btnSend.Enabled = isConnected;
            lblStatus.Text = isConnected ? "Connected" : "DisConnected";
            lblStatus.ForeColor = isConnected ? Color.Green : Color.Red;

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            int portNum = int.Parse(txtPort.Text);
            Server.Bind(new IPEndPoint(IPAddress.Any, portNum));
            Server.Listen(5);

            UpdateGUI(true);

            Server.BeginAccept(new AsyncCallback(OnClientConnect), null);
        }

        private void AddClientToList(int clientNumber)
        {
            MethodInvoker invoke = new MethodInvoker(delegate {
                lstClients.Items.Add(clientNumber.ToString());
            });
            this.BeginInvoke(invoke);
        }


        private void OnClientConnect(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = Server.EndAccept(ar);
                SocketPacket clientPacket = new SocketPacket(clientSocket);
                Clients.Add(clientPacket);

                AddClientToList(clientPacket.clientNumber);  // add the cleint to list box

                ReadyForData(clientSocket);


            }
            catch
            {
                Server.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
        
        }

        private void ReadyForData(Socket client)
        {
            byte[] buffer = new byte[1024];
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), buffer);

        }
        private void OnReceive(IAsyncResult ar )
        {
            byte[] buffer = (byte[])ar.AsyncState; // Get the buffer
            int bytesRead = 0;
            foreach (SocketPacket packet in Clients)
            {
                    bytesRead = packet.client.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        string msg = Encoding.Unicode.GetString(buffer, 0, bytesRead);
                        WriteLog(msg);
                        ReadyForData(packet.client);
                        break;  //  the right client
                    }
                }

        }
            private void WriteLog(string msg)
        {


            MethodInvoker invoke = new MethodInvoker(delegate { txtLog.AppendText(  "Client Said : " + msg + Environment.NewLine); });
            this.BeginInvoke(invoke);

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItem == null)
            {
                // No client selected
                return;
            }

            int selectedClientId = int.Parse(lstClients.SelectedItem.ToString());
            string msg = txtMsg.Text;
            byte[] buffer = Encoding.Unicode.GetBytes(msg);

            foreach (SocketPacket packet in Clients)
            {
                if (packet.client.Connected && packet.clientNumber == selectedClientId)
                {
                    packet.client.Send(buffer);
                    break;
                }
            }
        }


        private void btnNewClient_Click(object sender, EventArgs e)
        {
            Client c = new Client();
            c.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
