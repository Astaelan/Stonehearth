using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthSniffer
{
    public sealed class TCPStream
    {
        public enum TCPStreamOwner
        {
            Client,
            Server,
        }

        private const int DefaultSize = 1 << 20;
        private const int MaxSize = 1 << 24;

        private TCPStreamOwner mOwner = TCPStreamOwner.Client;
        private byte[] mData = new byte[DefaultSize];
        private int mLength = 0;

        public TCPStream(TCPStreamOwner pOwner)
        {
            mOwner = pOwner;
        }

        public TCPStreamOwner Owner { get { return mOwner; } }
        public byte[] Data { get { return mData; } }
        public int Length { get { return mLength; } }

        public void Reset()
        {
            mLength = 0;
        }

        public bool Append(byte[] pData, int pStart, int pLength)
        {
            if (pLength == 0) return true;
            while (mData.Length < MaxSize && (mLength + pLength) > mData.Length) Array.Resize(ref mData, mData.Length << 1);
            if ((mLength + pLength) > mData.Length) return false;
            Buffer.BlockCopy(pData, pStart, mData, mLength, pLength);
            mLength += pLength;
            return true;
        }

        public bool Consume(int pLength)
        {
            if (pLength == 0) return true;
            if (pLength > mLength) return false;
            mLength -= pLength;
            if (mLength > 0) Buffer.BlockCopy(mData, pLength, mData, 0, mLength);
            return true;
        }

        public bool PeekInt32(bool pLittleEndian, int pOffset, out int pValue)
        {
            pValue = 0;
            if (pOffset + 4 >= mLength) return false;
            if (pLittleEndian)
            {
                pValue = mData[pOffset + 0] << 0;
                pValue |= mData[pOffset + 1] << 8;
                pValue |= mData[pOffset + 2] << 16;
                pValue |= mData[pOffset + 3] << 24;
            }
            else
            {
                pValue = mData[pOffset + 0] << 24;
                pValue |= mData[pOffset + 1] << 16;
                pValue |= mData[pOffset + 2] << 8;
                pValue |= mData[pOffset + 3] << 0;
            }
            return true;
        }
    }
}
