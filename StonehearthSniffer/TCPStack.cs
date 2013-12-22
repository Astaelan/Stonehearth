using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthSniffer
{
    public sealed class TCPStack
    {
        private bool mInitialized = false;
        private string mClient = null;
        private string mServer = null;
        private uint mClientSequence = 0;
        private uint mServerSequence = 0;
        private Dictionary<uint, byte[]> mClientBuffer = new Dictionary<uint, byte[]>();
        private Dictionary<uint, byte[]> mServerBuffer = new Dictionary<uint, byte[]>();
        private TCPStream mClientStream = new TCPStream(TCPStream.TCPStreamOwner.Client);
        private TCPStream mServerStream = new TCPStream(TCPStream.TCPStreamOwner.Server);

        //public TCPStream ConnectorStream { get { return mConnectorStream; } }
        //public TCPStream ListenerStream { get { return mListenerStream; } }

        public void Reset()
        {
            mInitialized = false;
            mClient = null;
            mServer = null;
            mClientSequence = 0;
            mServerSequence = 0;
            mClientBuffer.Clear();
            mServerBuffer.Clear();
            mClientStream.Reset();
            mServerStream.Reset();
        }

        public TCPStream Push(TcpPacket pPacket)
        {
            IpPacket parentPacket = (IpPacket)pPacket.ParentPacket;

            bool sourceIsClient = true;
            if (mClient != null) sourceIsClient = (parentPacket.SourceAddress.ToString() + ":" + pPacket.SourcePort.ToString()) == mClient;
            if (pPacket.Syn)
            {
                if (sourceIsClient)
                {
                    mClient = parentPacket.SourceAddress.ToString() + ":" + pPacket.SourcePort.ToString();
                    mClientSequence = pPacket.SequenceNumber + 1;
                }
                else
                {
                    mServer = parentPacket.SourceAddress.ToString() + ":" + pPacket.SourcePort.ToString();
                    mServerSequence = pPacket.SequenceNumber + 1;
                    mInitialized = true;
                }
            }
            if (mInitialized)
            {
                if (sourceIsClient) Process(pPacket, ref mClientSequence, mClientBuffer, mClientStream);
                else Process(pPacket, ref mServerSequence, mServerBuffer, mServerStream);
            }
            return sourceIsClient ? mClientStream : mServerStream;
        }

        private static void Process(TcpPacket pPacket, ref uint pSequence, Dictionary<uint, byte[]> pBuffer, TCPStream pStream)
        {
            if (pPacket.PayloadData == null || pPacket.PayloadData.Length == 0) return;
            byte[] payload = pPacket.PayloadData;
            if (pPacket.SequenceNumber > pSequence)
            {
                byte[] buffered = null;
                while (pBuffer.TryGetValue(pSequence, out buffered))
                {
                    pBuffer.Remove(pSequence);
                    pStream.Append(buffered, 0, buffered.Length);
                    pSequence += (uint)buffered.Length;
                }
                if (pPacket.SequenceNumber > pSequence) pBuffer[pPacket.SequenceNumber] = payload;
            }
            if (pPacket.SequenceNumber < pSequence)
            {
                int difference = (int)(pSequence - pPacket.SequenceNumber);
                if (payload.Length > difference)
                {
                    pStream.Append(payload, difference, payload.Length - difference);
                    pSequence += (uint)(payload.Length - difference);
                }
            }
            else if (pPacket.SequenceNumber == pSequence)
            {
                pStream.Append(payload, 0, payload.Length);
                pSequence += (uint)payload.Length;

                List<uint> unused = new List<uint>();
                foreach (KeyValuePair<uint, byte[]> kv in pBuffer) if ((kv.Key + kv.Value.Length) <= pSequence) unused.Add(kv.Key);
                unused.ForEach(u => pBuffer.Remove(u));
            }
        }
    }
}
