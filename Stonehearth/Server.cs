using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Stonehearth
{
    public class Server<T> where T : Client, new()
    {
        public delegate void ServerEventHandler(Server<T> pServer);
        public delegate void ServerLogEventHandler(Server<T> pServer, LogManagerLevel pLogLevel, string pFormat, params object[] pArgs);
        public delegate void ServerClientEventHandler(Server<T> pServer, T pClient);
        public delegate void ServerClientPacketEventHandler(Server<T> pServer, T pClient, Packet pPacket);

        private string mName = null;
        private ushort mPort = 0;

        private Socket mListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private LockFreeQueue<T> mConnectingClients = new LockFreeQueue<T>();
        private List<T> mConnectedClients = new List<T>();

        public event ServerEventHandler OnStarted;
        public event ServerEventHandler OnStopped;

        public event ServerLogEventHandler OnLog;
        public event ServerClientEventHandler OnConnect;
        public event ServerClientEventHandler OnDisconnect;
        public event ServerClientPacketEventHandler OnPacket;

        public Server(string pName, ushort pPort) { mName = pName; mPort = pPort; }

        public ushort Capacity { get { return (ushort)mConnectedClients.Count; } }
        public List<T> Connected { get { return mConnectedClients; } }

        public void Log(LogManagerLevel pLogLevel, string pFormat, params object[] pArgs) { if (OnLog != null) OnLog(this, pLogLevel, "[" + mName + "] " + pFormat, pArgs); }

        public void Start()
        {
            mListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mListener.Bind(new IPEndPoint(IPAddress.Any, mPort));
            mListener.Listen(16);
            if (OnStarted != null) OnStarted(this);
            BeginAccept(null);
        }
        public void Stop()
        {
            mListener.Close();
            mListener = null;
            if (OnStopped != null) OnStopped(this);
        }

        public void DisconnectAll() { mConnectedClients.ForEach(c => c.Disconnect()); }

        public bool Pulse()
        {
            bool sleep = true;
            T client;
            while (mConnectingClients.Dequeue(out client)) mConnectedClients.Add(client);
            foreach (T connected in new List<T>(mConnectedClients)) if (!connected.Pulse()) sleep = false;
            return sleep;
        }

        private void BeginAccept(SocketAsyncEventArgs pArgs)
        {
            if (pArgs == null)
            {
                pArgs = new SocketAsyncEventArgs();
                pArgs.Completed += (s, a) => EndAccept(a);
            }
            pArgs.AcceptSocket = null;
            if (!mListener.AcceptAsync(pArgs)) EndAccept(pArgs);
        }

        private void EndAccept(SocketAsyncEventArgs pArgs)
        {
            try
            {
                if (pArgs.SocketError == SocketError.Success)
                {
                    T client = new T();
                    client.OnLog += (c, l, f, a) => Log(l, f, a);
                    client.OnConnect += c => { if (OnConnect != null) OnConnect(this, c as T); };
                    client.OnDisconnect += c => { mConnectedClients.Remove(c as T); if (OnDisconnect != null) OnDisconnect(this, c as T); };
                    client.OnPacket += (c, p) => { if (OnPacket != null) OnPacket(this, c as T, p); };
                    client.Connect(pArgs.AcceptSocket);
                    mConnectingClients.Enqueue(client);
                }
                BeginAccept(pArgs);
            }
            catch (Exception) { }
        }
    }
}
