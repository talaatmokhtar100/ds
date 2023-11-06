using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace TCP_Protocol
{
   public class SocketPacket
    {
       // property of type Socket & Name : Client 

        public Socket client  { get; set; }

        // property of type byte array  & Name : buffer 
        public byte [] buffer  { get; set; }

        public int clientNumber { get; private set; }
        private static int ids = 1;

        //Constructor , parameter client of type socket 
        public SocketPacket(Socket client )
        {
            this.client = client;
            this.buffer = new byte[1024];
            this.clientNumber = ids;
            ids += 1;

        }

    }
}
