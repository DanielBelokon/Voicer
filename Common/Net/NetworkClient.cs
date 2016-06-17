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

        #region Listen Related Members
        private Thread _listenThread;
        private UdpClient _listenSocket;
        private AutoResetEvent _packetRecieved;
        private int _localPort;
        protected PacketHandler packetHandler;
        private bool _isListening;
        public bool IsListening
        {
            get
            {
                return _isListening;
            }

            set
            {
                _isListening = value;
            }
        }
        #endregion

        #region Sending Related Members
        private Socket _senderSocket;
        private bool _isConnected;
        public bool IsConnected
        {
            get
            { return _isConnected; }
            private set
            { _isConnected = value; }
        }
        #endregion

        #region Tick Related Members
        private int _tickInterval;
        private bool _shouldTick;
        private Thread _tickThread;
        #endregion

        public NetworkClient()
        {
            packetHandler = new PacketHandler();
        }

        /// <summary>
        /// Connect to a remote endpoint in order to send messages.
        /// </summary>
        /// <param name="remoteEP">The remote endpoint to connect to.</param>
        public void Connect(IPEndPoint remoteEP)
        {
            _senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _senderSocket.Connect(remoteEP);
            _isConnected = true;
        }

        #region Tick
        public void StartTick(int interval = 5)
        {
            _tickInterval = interval;
            _tickThread = new Thread(DoTick);
            _shouldTick = true;
            _tickThread.Start();
        }

        public void StopTick()
        {
            _shouldTick = false;
            if (_tickThread != null)
            {
                _tickThread.Abort();
                _tickThread = null;
            }
        }

        private void DoTick()
        {
            do
            {
                Thread.Sleep(_tickInterval * 1000);
                Tick();

            } while (_shouldTick);
        }

        protected virtual void Tick()
        {

        }
        #endregion Tick

        #region listen
        public void StartListen(int localPort)
        {
            _packetRecieved = new AutoResetEvent(true);
            _localPort = localPort;
            _listenThread = new Thread(BeginReceive);
            _listenThread.Start(); 
        }
        // Recieve any and all packets that come into the listening port, seperate them by type, and forward to appropriate functions
        private void BeginReceive()
        {
            if (_isListening || _listenSocket != null)
                Disconnect();
            // Provides the local endpoint (port) for the UDP client to listen on.
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _localPort);
            _isListening = true;
            // Listening socket
            _listenSocket = new UdpClient(localEndPoint);

            try
            {
                
                while (_isListening)
                {
                    _packetRecieved.WaitOne();
                    _listenSocket.BeginReceive(new AsyncCallback(OnReceived), null);
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

                if (!_isListening)
                    return;

                data = _listenSocket.EndReceive(ar, ref remoteEP);
                _packetRecieved.Set();

                Packet packet = new Packet(data, remoteEP);

                //Console.WriteLine("+Received: " + packet.Type.ToString());
                // Process buffer
                packetHandler.HandlePacket(packet);
                PacketRecieved(packet);
                packet.Dispose();
            }

            catch (SocketException)
            {
                Disconnect();
            }
        }

        public virtual void PacketRecieved(Packet packet)
        {

        }
        #endregion listen

        public void Disconnect()
        {
            Disconnecting();
            _isConnected = false;
            _isListening = false;
            try
            {
                StopTick();
                StopListen();
                StopSend();
            }
            catch (SocketException e)
            { Console.WriteLine("Exception closing connections in {0}: \n{1} \n\n {2}", GetType(), e.GetType(), e.StackTrace); }
        }

        public virtual void Disconnecting()
        {
            
        }

        public void StopListen()
        {
            if (_listenSocket != null)
            {
                _listenSocket.Close();
                _listenSocket = null;
            }

            if (_listenThread != null)
            {
                _listenThread.Abort();
                _listenThread = null;
            }

            if (_packetRecieved != null)
            {
                _packetRecieved.Dispose();
                _packetRecieved = null;
            }
        }

        public void StopSend()
        {
            if (_senderSocket != null)
            {
                _senderSocket.Close();
                _senderSocket = null;
            }
        }

        public void Send(IPacket packet)
        {
            if (!_isConnected)
                return;

            try
            {
                byte[] buffer = packet.Encode();

                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                
                e.SetBuffer(buffer, 0, buffer.Length);
                e.Completed += new EventHandler<SocketAsyncEventArgs>(MessageSent);
                MessageSending(packet.Type);
                Console.WriteLine("-Sent " + packet.Type.ToString());
                _senderSocket.SendAsync(e);
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

        public virtual void MessageSending(Messages message)
        {

        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
