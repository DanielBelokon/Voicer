using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Voicer.Common.Data;

namespace Voicer.Common.Net
{
    public class NetworkClient : IDisposable
    {
        private UdpClient listenSocket;
        private AutoResetEvent packetRecieved;
        private bool bConnected;
        public bool IsConnected
        {
            get
            { return bConnected; }
            private set
            { bConnected = value; }
        }
        private bool bListening;
        public PacketHandler packetHandler;
        private int localPort;

        private Socket senderSocket;
        private Thread tickThread;

        private int tickInterval = 5;
        private bool bTick;
        private Thread listenThread;

        public NetworkClient(int localPort, int remotePort)
        {
            this.localPort = localPort;
            packetHandler = new PacketHandler();
        }

        // Sending
        public void Connect(IPEndPoint remoteEP)
        {
            senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            senderSocket.Connect(remoteEP);
            bConnected = true;
        }

        #region Tick
        public void StartTick(int interval = 5)
        {
            tickInterval = interval;
            tickThread = new Thread(DoTick);
            bTick = true;
            tickThread.Start();
        }

        public void StopTick()
        {

        }

        private void DoTick()
        {
            do
            {
                Thread.Sleep(tickInterval * 1000);
                Tick();

            } while (bTick);
        }

        public virtual void Tick()
        {

        }
        #endregion Tick


        public void StartListen()
        {
            packetRecieved = new AutoResetEvent(true);
            listenThread = new Thread(BeginReceive);
            listenThread.Start(); 
        }
        // Recieve any and all packets that come into the listening port, seperate them by type, and forward to appropriate functions
        private void BeginReceive()
        {
            // Provides the local endpoint (port) for the UDP client to listen on.
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, localPort);
            bListening = true;
            // Listening socket
            listenSocket = new UdpClient(localEndPoint);

            try
            {
                
                while (bListening)
                {
                    packetRecieved.WaitOne();
                    listenSocket.BeginReceive(new AsyncCallback(OnReceived), null);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("An error occured when trying to start listening socket.");
            }
        }

        private void OnReceived(IAsyncResult ar)
        {
            IPEndPoint remoteEP = null;

            try
            {
                byte[] data;

                if (!bListening)
                    return;

                data = listenSocket.EndReceive(ar, ref remoteEP);
                packetRecieved.Set();

                Packet packet = new Packet(data, remoteEP);

                PacketRecieved(packet);

                // Process buffer
                packetHandler.HandlePacket(packet);
            }

            catch (SocketException)
            {
                Disconnect();
            }
        }

        public virtual void PacketRecieved(Packet packet)
        {

        }

        public void Disconnect()
        {
            Disconnecting();
            bConnected = false;
            bListening = false;
            try
            {
                if (tickThread != null)
                {
                    tickThread.Abort();
                    tickThread = null;
                }

                if (listenSocket != null && listenThread != null)
                {
                    listenSocket.Close();
                    listenThread.Abort();
                    listenSocket = null;
                    listenThread = null;
                }

                if (senderSocket != null)
                    senderSocket.Close();
            }
            catch (Exception e)
            { Console.WriteLine("Error closing connections in {0}: \n{1}\n\n {2}", GetType(), e.GetType(),e.Message); }
        }

        public virtual void Disconnecting()
        {
            
        }

        public void Send(Packet packet)
        {
            if (!bConnected)
                return;

            try
            {
                byte[] buffer = packet.Encode();

                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                
                e.SetBuffer(buffer, 0, buffer.Length);
                e.Completed += new EventHandler<SocketAsyncEventArgs>(MessageSent);
                MessageSending(packet.Type);
                senderSocket.SendAsync(e);
            }
            catch (SocketException exc)
            {
                Console.WriteLine("Socket exception: " + exc.ToString());
            }
        }

        public virtual void MessageSent(object sender, SocketAsyncEventArgs e)
        {
            e.Dispose();
        }

        public virtual void MessageSending(Packet.Messages message)
        {

        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
