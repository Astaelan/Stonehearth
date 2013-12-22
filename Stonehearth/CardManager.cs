using PegasusUtil;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Stonehearth
{
    public static class CardManager
    {
        public static List<CardAsset> AllCardAssets = new List<CardAsset>();
        public static Dictionary<string, CardAsset> AllCardAssetsByCardID = new Dictionary<string, CardAsset>();
        public static Dictionary<int, CardAsset> AllCardAssetsByAssetID = new Dictionary<int, CardAsset>();
        public static Dictionary<TAG_CLASS, List<CardAsset>> CoreFreeCardAssetsByClassID = new Dictionary<TAG_CLASS, List<CardAsset>>();
        public static Dictionary<TAG_CLASS, CardAsset> CoreHeroCardAssetsByClassID = new Dictionary<TAG_CLASS, CardAsset>();
        public static Dictionary<TAG_CLASS, CardAsset> CoreHeroPowerCardAssetsByClassID = new Dictionary<TAG_CLASS, CardAsset>();
        public static Dictionary<TAG_RARITY, List<CardAsset>> ExpertCollectibleCardAssetsByRarity = new Dictionary<TAG_RARITY, List<CardAsset>>();
        public static CardAsset TheCoinCardAsset = null;
        public static CardValues CardValues = null;

        public static void Initialize()
        {
            // Start of Bundle Processing
            DataReader reader = new DataReader(File.ReadAllBytes(@"Data\cardxml0.unity3d"));

            // Start of AssetBundleHeader
            reader.Skip(13); // Signature (8), Unknown (4), FileVersion (1)
            reader.SkipNulled(); // Version
            reader.SkipNulled(); // Revision
            reader.Skip(4); // FileSize
            int bundleDataOffset = reader.ReadInt(); // DataOffset
            reader.Skip(25); // Flags (4), Flags (4), CompressedSize (4), UncompressedSize (4), FileSize (4), Unknown (4), Unknown (1)
            reader.Align(); // Alignment
            // End of AssetBundleHeader

            // Start of AssetBundleEntry Array
            reader.Skip(4); // Count
            // Start of AssetBundleEntry
            reader.SkipNulled(); // Name
            int assetEntryDataOffset = reader.ReadInt(); // DataOffset
            reader.Skip(4); // DataLength
            // End of AssetBundleEntry
            // End of AssetBundleEntry Array

            reader.Position = bundleDataOffset + assetEntryDataOffset;

            // Start of AssetFileHeader
            reader.Skip(12); // TreeSize (4), FileSize (4), Format (4)
            int assetFileDataOffset = reader.ReadInt(); // DataOffset
            reader.Skip(4); // Unknown
            // End of AssetFileHeader

            reader.LittleEndian = true;

            // Start of AssetTypeTree
            reader.SkipNulled(); // Revision
            reader.Skip(4); // Version

            // Start of AssetFieldType Map
            Dictionary<int, FieldType> fieldTypes = new Dictionary<int, FieldType>();
            int fieldTypesCount = reader.ReadInt(); // Count
            while (fieldTypesCount > 0)
            {
                // Start of AssetFieldType Tuple
                int classID = reader.ReadInt(); // ClassID
                fieldTypes[classID] = reader.ReadFieldType(); // FieldType
                // End of AssetFieldType Tuple
                --fieldTypesCount;
            }
            // End of AssetFieldType Map
            reader.Skip(4); // Unknown
            // End of AssetTypeTree

            // Start of AssetObjectPathTable
            // Start of AssetObjectPath Array
            Dictionary<int, int> objectPaths = new Dictionary<int, int>();
            int objectPathsCount = reader.ReadInt(); // Count
            while (objectPathsCount > 0)
            {
                // Start of AssetObjectPath
                reader.Skip(4); // PathID
                int objectPathOffset = reader.ReadInt(); // Offset
                reader.Skip(8); // Length (4), ClassID1 (4)
                objectPaths[objectPathOffset] = reader.ReadInt(); // ClassID2
                // End of AssetObjectPath
                --objectPathsCount;
            }
            // End of AssetObjectPath Array
            // End of AssetObjectPathTable

            Dictionary<string, string> cardAssets = new Dictionary<string, string>();
            foreach (KeyValuePair<int, int> kv in objectPaths)
            {
                FieldType mainFieldType = fieldTypes[kv.Value];
                if (mainFieldType.Type != "TextAsset") continue;
                string name = null;
                string script = null;
                reader.Position = bundleDataOffset + assetEntryDataOffset + assetFileDataOffset + kv.Key;
                // Start of AssetObjectData
                foreach (FieldType childFieldType in mainFieldType.Children)
                {
                    switch (childFieldType.Name)
                    {
                        case "m_Name": name = reader.ReadPrefixed(Encoding.ASCII); break; // Name Value
                        case "m_Script": script = reader.ReadPrefixed(Encoding.UTF8); break;  // Script Value
                        case "m_PathName": reader.SkipPrefixed(); break; // PathName Value
                        default: throw new NotSupportedException(string.Format("Unknown AssetObjectData Field: {0}", childFieldType.Name));
                    }
                    reader.Align(); // Alignment
                }
                // End of AssetObjectData
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(script)) throw new NotSupportedException("Incomplete AssetObjectData");
                cardAssets[name] = script;
            }
            // End of Bundle Processing

            // Start of Manifest Processing
            string[] manifestLines = File.ReadAllLines(@"Data\manifest-cards.csv");
            foreach (string manifestLine in manifestLines)
            {
                string[] cardsManifestLineFields = manifestLine.Split(',');
                string cardID = cardsManifestLineFields[1];
                int assetID = int.Parse(cardsManifestLineFields[0]);
                string cardAsset = null;
                if (!cardAssets.TryGetValue(cardID, out cardAsset)) continue;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(cardAsset);
                AllCardAssets.Add(CardAsset.Load(cardID, assetID, doc.DocumentElement));
            }
            // End of Manifest Processing

            foreach (CardAsset cardAsset in AllCardAssets)
            {
                AllCardAssetsByCardID.Add(cardAsset.CardID, cardAsset);
                AllCardAssetsByAssetID.Add(cardAsset.AssetID, cardAsset);
                if (cardAsset.CardSet == TAG_CARD_SET.CORE && cardAsset.CardType == TAG_CARDTYPE.HERO)
                    CoreHeroCardAssetsByClassID.Add(cardAsset.Class, cardAsset);
                if (cardAsset.CardSet == TAG_CARD_SET.CORE && cardAsset.CardType == TAG_CARDTYPE.HERO_POWER)
                    CoreHeroPowerCardAssetsByClassID.Add(cardAsset.Class, cardAsset);
                if (cardAsset.CardSet == TAG_CARD_SET.CORE && cardAsset.Rarity == TAG_RARITY.FREE)
                {
                    List<CardAsset> coreFreeCardAssets = null;
                    if (!CoreFreeCardAssetsByClassID.TryGetValue(cardAsset.Class, out coreFreeCardAssets))
                    {
                        coreFreeCardAssets = new List<CardAsset>();
                        CoreFreeCardAssetsByClassID.Add(cardAsset.Class, coreFreeCardAssets);
                    }
                    if (!cardAsset.CardID.StartsWith("GAME_")) coreFreeCardAssets.Add(cardAsset);
                }
                if (cardAsset.CardSet == TAG_CARD_SET.EXPERT1 && cardAsset.Collectible)
                {
                    List<CardAsset> expertCollectibleCardAssets = null;
                    if (!ExpertCollectibleCardAssetsByRarity.TryGetValue(cardAsset.Rarity, out expertCollectibleCardAssets))
                    {
                        expertCollectibleCardAssets = new List<CardAsset>();
                        ExpertCollectibleCardAssetsByRarity.Add(cardAsset.Rarity, expertCollectibleCardAssets);
                    }
                    expertCollectibleCardAssets.Add(cardAsset);
                }
                if (cardAsset.CardID == "GAME_005") TheCoinCardAsset = cardAsset;
            }

            CardValues.Builder cardValues = CardValues.CreateBuilder();
            CardFlair.PremiumType[] premiumTypes = Enum.GetValues(typeof(CardFlair.PremiumType)).Cast<CardFlair.PremiumType>().ToArray();
            foreach (CardAsset cardAsset in AllCardAssets.Where(c => c.AllowCrafting))
            {
                foreach (CardFlair.PremiumType premiumType in premiumTypes)
                    cardValues.AddCards(CardValue.CreateBuilder().SetBuy(cardAsset.ArcaneDustBuyValues[premiumType]).SetSell(cardAsset.ArcaneDustSellValues[premiumType]).SetNerfed(false).SetCard(PegasusShared.CardDef.CreateBuilder().SetAsset(cardAsset.AssetID).SetPremium((int)premiumType)));
            }
            CardValues = cardValues.Build();

            LogManager.WriteLine(LogManagerLevel.Info, "[CardManager] Loaded {0} Cards", AllCardAssets.Count);

            //foreach (CardAsset cardAsset in AllCardAssets)
            //{
            //    if (cardAsset.Powers.Count == 0) continue;
            //    LogManager.WriteLine(LogManagerLevel.Debug, "[CardManager] {0}, {1} Powers: {2}", cardAsset.CardName, cardAsset.CardTextInHand, cardAsset.Powers.Count); 
            //}
        }

        private class FieldType
        {
            public string Type;
            public string Name;
            public List<FieldType> Children = new List<FieldType>();
        }

        private class DataReader
        {
            private byte[] mData = null;

            public DataReader(byte[] pData) { mData = pData; }

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
}
