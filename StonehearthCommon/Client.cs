using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace StonehearthCommon
{
    public abstract class Client
    {
        public enum SocketState : byte
        {
            Offline = 0,
            Connecting = 1,
            Disconnecting = 2,
            Connected = 3,
        }

        public delegate void ClientLogEventHandler(Client pClient, LogManagerLevel pLogLevel, string pFormat, params object[] pArgs);
        public delegate void ClientEventHandler(Client pClient);
        public delegate void ClientPacketEventHandler(Client pClient, Packet pPacket);

        private Socket mSocket = null;
        private string mHost = null;
        private SocketState mState = SocketState.Offline;

        private byte[] mReceiveBuffer = new byte[ushort.MaxValue / 4];
        private int mReceiveStart = 0;
        private int mReceiveLength = 0;
        private LockFreeQueue<Packet> mReceivedPackets = new LockFreeQueue<Packet>();
        private LockFreeQueue<ByteArraySegment> mSendSegments = new LockFreeQueue<ByteArraySegment>();
        private int mSending = 0;
        private bool mFlushThenDisconnect = false;

        public event ClientLogEventHandler OnLog;
        public event ClientEventHandler OnConnect;
        public event ClientEventHandler OnDisconnect;
        public event ClientPacketEventHandler OnPacket;

        public string Host { get { return mHost; } }
        public SocketState State { get { return mState; } }

        public void Log(LogManagerLevel pLogLevel, string pFormat, params object[] pArgs) { if (OnLog != null) OnLog(this, pLogLevel, "(" + mHost + ") " + pFormat, pArgs); }

        public void Connect(Socket pSocket) { mSocket = pSocket; mHost = ((IPEndPoint)mSocket.RemoteEndPoint).Address.ToString(); mState = SocketState.Connecting; }
        public void Disconnect() { mState = SocketState.Disconnecting; }

        public bool Pulse()
        {
            bool sleep = true;
            switch (mState)
            {
                case SocketState.Connecting:
                    mState = SocketState.Connected;
                    if (OnConnect != null) OnConnect(this);
                    BeginReceive();
                    sleep = false;
                    break;
                case SocketState.Disconnecting:
                    mSocket.Close();
                    mSocket = null;
                    mState = SocketState.Offline;
                    if (OnDisconnect != null) OnDisconnect(this);
                    sleep = false;
                    break;
                case SocketState.Connected:
                    {
                        Packet packet;
                        while ((packet = ReceivePacket()) != null) { sleep = false; if (OnPacket != null) OnPacket(this, packet); }
                        break;
                    }
                default: break;
            }
            return sleep;
        }

        private Packet ReceivePacket()
        {
            Packet packet = null;
            if (mState == SocketState.Connected) mReceivedPackets.Dequeue(out packet);
            //if (packet != null) Log(LogLevel.Debug, "Receiving {0}, {1} Bytes", packet.Opcode, packet.Remaining);
            return packet;
        }

        private void BeginReceive()
        {
            if (mState != SocketState.Connected) return;
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += (s, a) => EndReceive(a);
            args.SetBuffer(mReceiveBuffer, mReceiveStart + mReceiveLength, mReceiveBuffer.Length - (mReceiveStart + mReceiveLength));
            try { if (!mSocket.ReceiveAsync(args)) EndReceive(args); }
            catch (ObjectDisposedException) { }
        }
        private void EndReceive(SocketAsyncEventArgs pArguments)
        {
            if (mState != SocketState.Connected) return;
            if (pArguments.BytesTransferred <= 0)
            {
                Disconnect();
                return;
            }
            mReceiveLength += pArguments.BytesTransferred;

            while (mReceiveLength > 0 && mState == SocketState.Connected)
            {
                int used = 0;
                if (mReceiveLength >= 8)
                {
                    int opcode = mReceiveBuffer[mReceiveStart + 0] << 0;
                    opcode |= mReceiveBuffer[mReceiveStart + 1] << 8;
                    opcode |= mReceiveBuffer[mReceiveStart + 2] << 16;
                    opcode |= mReceiveBuffer[mReceiveStart + 3] << 24;
                    int size = mReceiveBuffer[mReceiveStart + 4] << 0;
                    size |= mReceiveBuffer[mReceiveStart + 5] << 8;
                    size |= mReceiveBuffer[mReceiveStart + 6] << 16;
                    size |= mReceiveBuffer[mReceiveStart + 7] << 24;
                    if (mReceiveLength >= (size + 8))
                    {
                        used = size + 8;
                        mReceivedPackets.Enqueue(new Packet(opcode, mReceiveBuffer, mReceiveStart + 8, size));
                    }
                }
                if (used == 0) break;
                mReceiveStart += used;
                mReceiveLength -= used;
            }

            if (mReceiveLength == 0) mReceiveStart = 0;
            else if (mReceiveStart > 0)
            {
                Buffer.BlockCopy(mReceiveBuffer, mReceiveStart, mReceiveBuffer, 0, mReceiveLength);
                mReceiveStart = 0;
            }
            if (mReceiveLength == mReceiveBuffer.Length) Disconnect();
            else BeginReceive();
        }

        public void SendPacket(Packet pPacket, bool pFlushThenDisconnect = false)
        {
            //Log(LogLevel.Debug, "Sending {0}, {1} Bytes", pPacket.Opcode, pPacket.Length);

            int size = pPacket.Length;
            byte[] buffer = new byte[size + 8];
            int opcode = (int)pPacket.Opcode;
            buffer[0] = (byte)((opcode >> 0) & 0xFF);
            buffer[1] = (byte)((opcode >> 8) & 0xFF);
            buffer[2] = (byte)((opcode >> 16) & 0xFF);
            buffer[3] = (byte)((opcode >> 24) & 0xFF);
            buffer[4] = (byte)((size >> 0) & 0xFF);
            buffer[5] = (byte)((size >> 8) & 0xFF);
            buffer[6] = (byte)((size >> 16) & 0xFF);
            buffer[7] = (byte)((size >> 24) & 0xFF);
            pPacket.Flush(buffer, 8);
            Send(buffer, pFlushThenDisconnect);

            //string temp = BitConverter.ToString(buffer, 0, size + 8).Replace('-', ' ');
            //System.Diagnostics.Debug.WriteLine(temp);
        }

        private void Send(byte[] pBuffer, bool pFlushThenDisconnect = false)
        {
            if (mState != SocketState.Connected) return;
            mSendSegments.Enqueue(new ByteArraySegment(pBuffer));
            if (pFlushThenDisconnect) mFlushThenDisconnect = true;
            if (Interlocked.CompareExchange(ref mSending, 1, 0) == 0) BeginSend();
        }
        private void BeginSend()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += (s, a) => EndSend(a);
            ByteArraySegment segment = mSendSegments.Next;
            args.SetBuffer(segment.Buffer, segment.Start, segment.Length);
            try { if (!mSocket.SendAsync(args)) EndSend(args); }
            catch (ObjectDisposedException) { }
        }
        private void EndSend(SocketAsyncEventArgs pArguments)
        {
            if (mState != SocketState.Connected) return;
            if (pArguments.BytesTransferred <= 0)
            {
                Disconnect();
                return;
            }
            if (mSendSegments.Next.Advance(pArguments.BytesTransferred)) mSendSegments.Dequeue();
            if (mSendSegments.Next != null) BeginSend();
            else
            {
                mSending = 0;
                if (mFlushThenDisconnect) Disconnect();
            }
        }

        public void InvalidPacketDataDisconnect()
        {
            StackTrace trace = new StackTrace(1);
            StackFrame frame = trace.GetFrame(0);
            MethodBase method = frame.GetMethod();
            Log(LogManagerLevel.Warn, "Invalid Packet Data: {0}.{1}", method.DeclaringType.FullName, method.Name);
            Disconnect();
        }
    }
}
