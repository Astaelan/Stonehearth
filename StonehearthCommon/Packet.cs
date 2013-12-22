using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthCommon
{
    public class Packet
    {
        private const int DEFAULT_SIZE = 256;

        public readonly int Opcode;
        private byte[] mData = null;
        private int mWriteCursor = 0;
        private int mReadCursor = 0;

        public Packet(int pOpcode)
        {
            Opcode = pOpcode;
            mData = new byte[DEFAULT_SIZE];
        }
        public Packet(int pOpcode, byte[] pData)
        {
            Opcode = pOpcode;
            mData = new byte[pData.Length];
            WriteRawBytes(pData, 0, pData.Length);
        }
        public Packet(int pOpcode, byte[] pData, int pStart, int pLength)
        {
            Opcode = pOpcode;
            mData = new byte[pLength];
            WriteRawBytes(pData, pStart, pLength);
        }

        protected byte[] Data { get { return mData; } }
        public int Length { get { return mWriteCursor; } }
        public int Cursor { get { return mReadCursor; } }
        public int Remaining { get { return mWriteCursor - mReadCursor; } }

        private void Prepare(int pLength)
        {
            if (mData.Length - mWriteCursor >= pLength) return;
            int newSize = mData.Length * 2;
            while (newSize < mWriteCursor + pLength) newSize *= 2;
            Array.Resize<byte>(ref mData, newSize);
        }

        public void Rewind(int pPosition)
        {
            if (pPosition < 0) pPosition = 0;
            else if (pPosition > mWriteCursor) pPosition = mWriteCursor;
            mReadCursor = pPosition;
        }

        public void WriteSkip(int pLength)
        {
            Prepare(pLength);
            mWriteCursor += pLength;
        }
        public void WriteBool(bool pValue)
        {
            Prepare(1);
            mData[mWriteCursor++] = (byte)(pValue ? 1 : 0);
        }
        public void WriteByte(byte pValue)
        {
            Prepare(1);
            mData[mWriteCursor++] = pValue;
        }
        public void WriteSByte(sbyte pValue)
        {
            Prepare(1);
            mData[mWriteCursor++] = (byte)pValue;
        }
        public void WriteRawBytes(byte[] pBytes) { WriteRawBytes(pBytes, 0, pBytes.Length); }
        public void WriteRawBytes(byte[] pBytes, int pStart, int pLength)
        {
            if (pLength <= 0) return;
            Prepare(pLength);
            Buffer.BlockCopy(pBytes, pStart, mData, mWriteCursor, pLength);
            mWriteCursor += pLength;
        }
        public void WriteBytes(byte[] pBytes) { WriteBytes(pBytes, 0, pBytes.Length); }
        public void WriteBytes(byte[] pBytes, int pStart, int pLength)
        {
            WriteUShort((ushort)pLength);
            if (pLength <= 0) return;
            WriteRawBytes(pBytes, pStart, pLength);
        }
        public void WriteUShort(ushort pValue)
        {
            Prepare(2);
            mData[mWriteCursor++] = (byte)(pValue & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 8) & 0xFF);
        }
        public void WriteShort(short pValue)
        {
            Prepare(2);
            mData[mWriteCursor++] = (byte)(pValue & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 8) & 0xFF);
        }
        public void WriteUInt(uint pValue)
        {
            Prepare(4);
            mData[mWriteCursor++] = (byte)(pValue & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 8) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 16) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 24) & 0xFF);
        }
        public void WriteInt(int pValue)
        {
            Prepare(4);
            mData[mWriteCursor++] = (byte)(pValue & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 8) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 16) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 24) & 0xFF);
        }
        public void WriteFloat(float pValue)
        {
            byte[] buffer = BitConverter.GetBytes(pValue);
            if (!BitConverter.IsLittleEndian) Array.Reverse(buffer);
            WriteRawBytes(buffer);
        }
        public void WriteULong(ulong pValue)
        {
            Prepare(8);
            mData[mWriteCursor++] = (byte)(pValue & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 8) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 16) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 24) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 32) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 40) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 48) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 56) & 0xFF);
        }
        public void WriteLong(long pValue)
        {
            Prepare(8);
            mData[mWriteCursor++] = (byte)(pValue & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 8) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 16) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 24) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 32) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 40) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 48) & 0xFF);
            mData[mWriteCursor++] = (byte)((pValue >> 56) & 0xFF);
        }
        public void WriteDouble(double pValue)
        {
            byte[] buffer = BitConverter.GetBytes(pValue);
            if (!BitConverter.IsLittleEndian) Array.Reverse(buffer);
            WriteRawBytes(buffer);
        }
        public void WriteGuid(Guid pValue) { WriteRawBytes(pValue.ToByteArray()); }
        public void WriteString(string pValue) { WriteBytes(Encoding.ASCII.GetBytes(pValue)); }
        public void WritePaddedString(string pValue, int pLength)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(pValue);
            Array.Resize(ref buffer, pLength);
            WriteRawBytes(buffer);
        }

        public bool ReadSkip(int pLength)
        {
            if (mReadCursor + pLength > mWriteCursor) return false;
            mReadCursor += pLength;
            return true;
        }
        public bool ReadBool(out bool pValue)
        {
            pValue = false;
            if (mReadCursor + 1 > mWriteCursor) return false;
            pValue = mData[mReadCursor++] != 0;
            return true;
        }
        public bool ReadByte(out byte pValue)
        {
            pValue = 0;
            if (mReadCursor + 1 > mWriteCursor) return false;
            pValue = mData[mReadCursor++];
            return true;
        }
        public bool ReadSByte(out sbyte pValue)
        {
            pValue = 0;
            if (mReadCursor + 1 > mWriteCursor) return false;
            pValue = (sbyte)mData[mReadCursor++];
            return true;
        }
        public bool ReadRawBytes(out byte[] pBytes, int pLength)
        {
            pBytes = new byte[pLength];
            if (pLength <= 0) return true;
            if (mReadCursor + pLength > mWriteCursor) return false;
            Buffer.BlockCopy(mData, mReadCursor, pBytes, 0, pLength);
            mReadCursor += pLength;
            return true;
        }
        public bool ReadBytes(out byte[] pBytes)
        {
            ushort length;
            pBytes = null;
            if (!ReadUShort(out length)) return false;
            if (mReadCursor + length > mWriteCursor) return false;
            return ReadRawBytes(out pBytes, length);
        }
        public bool ReadUShort(out ushort pValue)
        {
            pValue = 0;
            if (mReadCursor + 2 > mWriteCursor) return false;
            pValue = mData[mReadCursor++];
            pValue |= (ushort)(mData[mReadCursor++] << 8);
            return true;
        }
        public bool ReadShort(out short pValue)
        {
            pValue = 0;
            if (mReadCursor + 2 > mWriteCursor) return false;
            pValue = mData[mReadCursor++];
            pValue |= (short)(mData[mReadCursor++] << 8);
            return true;
        }
        public bool ReadFloat(out float pValue)
        {
            pValue = 0;
            byte[] buffer;
            if (!ReadRawBytes(out buffer, 4)) return false;
            if (!BitConverter.IsLittleEndian) Array.Reverse(buffer);
            pValue = BitConverter.ToSingle(buffer, 0);
            return true;
        }
        public bool ReadUInt(out uint pValue)
        {
            pValue = 0;
            if (mReadCursor + 4 > mWriteCursor) return false;
            pValue = mData[mReadCursor++];
            pValue |= (uint)(mData[mReadCursor++] << 8);
            pValue |= (uint)(mData[mReadCursor++] << 16);
            pValue |= (uint)(mData[mReadCursor++] << 24);
            return true;
        }
        public bool ReadInt(out int pValue)
        {
            pValue = 0;
            if (mReadCursor + 4 > mWriteCursor) return false;
            pValue = mData[mReadCursor++];
            pValue |= (mData[mReadCursor++] << 8);
            pValue |= (mData[mReadCursor++] << 16);
            pValue |= (mData[mReadCursor++] << 24);
            return true;
        }
        public bool ReadULong(out ulong pValue)
        {
            pValue = 0;
            if (mReadCursor + 8 > mWriteCursor) return false;
            pValue = mData[mReadCursor++];
            pValue |= ((ulong)mData[mReadCursor++] << 8);
            pValue |= ((ulong)mData[mReadCursor++] << 16);
            pValue |= ((ulong)mData[mReadCursor++] << 24);
            pValue |= ((ulong)mData[mReadCursor++] << 32);
            pValue |= ((ulong)mData[mReadCursor++] << 40);
            pValue |= ((ulong)mData[mReadCursor++] << 48);
            pValue |= ((ulong)mData[mReadCursor++] << 56);
            return true;
        }
        public bool ReadLong(out long pValue)
        {
            pValue = 0;
            if (mReadCursor + 8 > mWriteCursor) return false;
            pValue = mData[mReadCursor++];
            pValue |= ((long)mData[mReadCursor++] << 8);
            pValue |= ((long)mData[mReadCursor++] << 16);
            pValue |= ((long)mData[mReadCursor++] << 24);
            pValue |= ((long)mData[mReadCursor++] << 32);
            pValue |= ((long)mData[mReadCursor++] << 40);
            pValue |= ((long)mData[mReadCursor++] << 48);
            pValue |= ((long)mData[mReadCursor++] << 56);
            return true;
        }
        public bool ReadDouble(out double pValue)
        {
            pValue = 0;
            byte[] buffer;
            if (!ReadRawBytes(out buffer, 8)) return false;
            if (!BitConverter.IsLittleEndian) Array.Reverse(buffer);
            pValue = BitConverter.ToDouble(buffer, 0);
            return true;
        }
        public bool ReadGuid(out Guid pValue)
        {
            pValue = Guid.Empty;
            byte[] buffer;
            if (!ReadRawBytes(out buffer, 16)) return false;
            pValue = new Guid(buffer);
            return true;
        }
        public bool ReadString(out string pValue)
        {
            byte[] value;
            pValue = null;
            if (!ReadBytes(out value)) return false;
            pValue = Encoding.ASCII.GetString(value, 0, value.Length);
            return true;
        }
        public bool ReadPaddedString(out string pValue, int pLength)
        {
            byte[] value;
            pValue = null;
            if (!ReadRawBytes(out value, pLength)) return false;
            int length = 0;
            while (value[length] != 0x00 && length < pLength) ++length;
            if (length > 0) pValue = Encoding.ASCII.GetString(value, 0, length);
            return true;
        }
        public byte[] ReadAll()
        {
            byte[] buffer = null;
            ReadRawBytes(out buffer, Length);
            return buffer;
        }

        public void Flush(byte[] pBuffer, int pStart)
        {
            Buffer.BlockCopy(mData, 0, pBuffer, pStart, mWriteCursor);
        }
    }
}
