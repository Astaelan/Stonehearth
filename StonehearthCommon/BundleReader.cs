using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthCommon
{
    public class BundleReader
    {
        public class FieldType
        {
            public string Type;
            public string Name;
            public List<FieldType> Children = new List<FieldType>();
        }

        private byte[] mData = null;

        public BundleReader(byte[] pData) { mData = pData; }

        public int Position { get; set; }
        public bool LittleEndian { get; set; }

        public void Skip(int pReadSkipped) { Position += pReadSkipped; }
        public void Align()
        {
            int overflow = Position & 0x03;
            if (overflow != 0) Position += 4 - overflow;
        }
        public void SkipNulled()
        {
            while (mData[Position] != 0) Position++;
            Position++;
        }
        public void SkipPrefixed() { Position += ReadInt(); }
        public int ReadInt()
        {
            int value = 0;
            if (LittleEndian)
            {
                value = mData[Position + 0] << 0;
                value |= mData[Position + 1] << 8;
                value |= mData[Position + 2] << 16;
                value |= mData[Position + 3] << 24;
            }
            else
            {
                value = mData[Position + 0] << 24;
                value |= mData[Position + 1] << 16;
                value |= mData[Position + 2] << 8;
                value |= mData[Position + 3] << 0;
            }
            Position += 4;
            return value;
        }
        public string ReadNulled()
        {
            int length = 0;
            while (mData[Position + length] != 0) ++length;
            string value = "";
            if (length > 0) value = Encoding.ASCII.GetString(mData, Position, length);
            Position += length + 1;
            return value;
        }
        public string ReadPrefixed(Encoding pEncoding)
        {
            int length = ReadInt();
            string value = "";
            if (length > 0) value = pEncoding.GetString(mData, Position, length);
            Position += length;
            return value;
        }
        public FieldType ReadFieldType()
        {
            FieldType fieldType = new FieldType();
            fieldType.Type = ReadNulled(); // Type
            fieldType.Name = ReadNulled(); // Name
            Skip(20); // Size (4), Index (4), ArrayFlags (4), Flags (4), Flags (4)
            int childCount = ReadInt(); // ChildCount
            for (int childIndex = 0; childIndex < childCount; ++childIndex) fieldType.Children.Add(ReadFieldType()); // ChildEntry
            return fieldType;
        }
    }
}
