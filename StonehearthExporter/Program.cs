using StonehearthCommon;
using StonehearthExporter.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace StonehearthExporter
{
    internal static class Program
    {
        private static void Main()
        {
            string basePath = Path.GetDirectoryName(Settings.Default.ExecutablePath) + Path.DirectorySeparatorChar;
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                db.Execute(null, "DELETE FROM [Achievement]");

                DataTable dataTableAchieve = new DataTable();
                dataTableAchieve.Columns.Add("AchievementID", typeof(int));
                dataTableAchieve.Columns.Add("Group", typeof(string));
                dataTableAchieve.Columns.Add("MaxProgress", typeof(int));
                dataTableAchieve.Columns.Add("RaceRequirement", typeof(int));
                dataTableAchieve.Columns.Add("CardSetRequirement", typeof(int));
                dataTableAchieve.Columns.Add("RewardType", typeof(string));
                dataTableAchieve.Columns.Add("Parameter1", typeof(int));
                dataTableAchieve.Columns.Add("Parameter2", typeof(int));
                dataTableAchieve.Columns.Add("UnlockableFeature", typeof(string));
                dataTableAchieve.Columns.Add("ParentID", typeof(int));
                dataTableAchieve.Columns.Add("Trigger", typeof(string));

                string[] lines = File.ReadAllLines(basePath + "manifest-achieves.csv");
                foreach (string line in lines)
                {
                    string[] fields = line.Split(',');

                    dataTableAchieve.Rows.Add(
                        int.Parse(fields[0]),
                        fields[1],
                        int.Parse(fields[2]),
                        int.Parse(fields[3]),
                        int.Parse(fields[4]),
                        fields[5],
                        int.Parse(fields[6]),
                        int.Parse(fields[7]),
                        fields[8],
                        int.Parse(fields[9]),
                        fields[10]);
                }
                new SqlBulkCopy(db) { DestinationTableName = "Achievement" }.WriteToServer(dataTableAchieve);

                // Start of Bundle Processing
                BundleReader reader = new BundleReader(File.ReadAllBytes(basePath + "Data" + Path.DirectorySeparatorChar + "Win" + Path.DirectorySeparatorChar + "cardxml0.unity3d"));

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
                Dictionary<int, BundleReader.FieldType> fieldTypes = new Dictionary<int, BundleReader.FieldType>();
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
                    BundleReader.FieldType mainFieldType = fieldTypes[kv.Value];
                    if (mainFieldType.Type != "TextAsset") continue;
                    string name = null;
                    string script = null;
                    reader.Position = bundleDataOffset + assetEntryDataOffset + assetFileDataOffset + kv.Key;
                    // Start of AssetObjectData
                    foreach (BundleReader.FieldType childFieldType in mainFieldType.Children)
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

                db.Execute(null, "DELETE FROM [Card]");

                DataTable dataTableCard = new DataTable();
                dataTableCard.Columns.Add("CardID", typeof(string));
                dataTableCard.Columns.Add("AssetID", typeof(int));
                dataTableCard.Columns.Add("Name", typeof(string));
                dataTableCard.Columns.Add("Set", typeof(string));
                dataTableCard.Columns.Add("Type", typeof(string));
                dataTableCard.Columns.Add("Craftable", typeof(bool));
                dataTableCard.Columns.Add("Collectible", typeof(bool));
                dataTableCard.Columns.Add("MasterPowerID", typeof(Guid));
                dataTableCard.Columns.Add("Faction", typeof(string));
                dataTableCard.Columns.Add("Race", typeof(string));
                dataTableCard.Columns.Add("Class", typeof(string));
                dataTableCard.Columns.Add("Rarity", typeof(string));
                dataTableCard.Columns.Add("Cost", typeof(int));
                dataTableCard.Columns.Add("Atk", typeof(int));
                dataTableCard.Columns.Add("Health", typeof(int));
                dataTableCard.Columns.Add("AttackVisualType", typeof(int));
                dataTableCard.Columns.Add("TextInHand", typeof(string));
                dataTableCard.Columns.Add("TextInPlay", typeof(string));
                dataTableCard.Columns.Add("DevState", typeof(int));
                dataTableCard.Columns.Add("EnchantmentBirthVisual", typeof(string));
                dataTableCard.Columns.Add("EnchantmentIdleVisual", typeof(string));
                dataTableCard.Columns.Add("FlavorText", typeof(string));
                dataTableCard.Columns.Add("ArtistName", typeof(string));
                dataTableCard.Columns.Add("TargetingArrowText", typeof(string));
                dataTableCard.Columns.Add("HowToGetThisGoldCard", typeof(string));
                dataTableCard.Columns.Add("HowToGetThisCard", typeof(string));
                dataTableCard.Columns.Add("StandardBuyValue", typeof(int));
                dataTableCard.Columns.Add("StandardSellValue", typeof(int));
                dataTableCard.Columns.Add("FoilBuyValue", typeof(int));
                dataTableCard.Columns.Add("FoilSellValue", typeof(int));
                dataTableCard.Columns.Add("Recall", typeof(int));
                dataTableCard.Columns.Add("Durability", typeof(int));
                dataTableCard.Columns.Add("TriggerVisual", typeof(bool));
                dataTableCard.Columns.Add("Elite", typeof(bool));
                dataTableCard.Columns.Add("Deathrattle", typeof(bool));
                dataTableCard.Columns.Add("Charge", typeof(bool));
                dataTableCard.Columns.Add("DivineShield", typeof(bool));
                dataTableCard.Columns.Add("Windfury", typeof(bool));
                dataTableCard.Columns.Add("Taunt", typeof(bool));
                dataTableCard.Columns.Add("Aura", typeof(bool));
                dataTableCard.Columns.Add("Enrage", typeof(bool));
                dataTableCard.Columns.Add("OneTurnEffect", typeof(bool));
                dataTableCard.Columns.Add("Stealth", typeof(bool));
                dataTableCard.Columns.Add("Battlecry", typeof(bool));
                dataTableCard.Columns.Add("Secret", typeof(bool));
                dataTableCard.Columns.Add("Morph", typeof(bool));
                dataTableCard.Columns.Add("AffectedBySpellPower", typeof(bool));
                dataTableCard.Columns.Add("Freeze", typeof(bool));
                dataTableCard.Columns.Add("Spellpower", typeof(bool));
                dataTableCard.Columns.Add("Combo", typeof(bool));
                dataTableCard.Columns.Add("Silence", typeof(bool));
                dataTableCard.Columns.Add("Summoned", typeof(bool));
                dataTableCard.Columns.Add("ImmuneToSpellpower", typeof(bool));
                dataTableCard.Columns.Add("AdjacentBuff", typeof(bool));
                dataTableCard.Columns.Add("GrantCharge", typeof(bool));
                dataTableCard.Columns.Add("Poisonous", typeof(bool));
                dataTableCard.Columns.Add("HealTarget", typeof(bool));

                db.Execute(null, "DELETE FROM [CardPower]");

                DataTable dataTableCardPower = new DataTable();
                dataTableCardPower.Columns.Add("CardPowerID", typeof(Guid));
                dataTableCardPower.Columns.Add("CardID", typeof(string));

                db.Execute(null, "DELETE FROM [CardPowerRequirement]");
                DataTable dataTableCardPowerRequirement = new DataTable();
                dataTableCardPowerRequirement.Columns.Add("CardPowerID", typeof(Guid));
                dataTableCardPowerRequirement.Columns.Add("Type", typeof(string));
                dataTableCardPowerRequirement.Columns.Add("Parameter", typeof(int));

                // Start of Manifest Processing
                HashSet<string> unhandledTags = new HashSet<string>();
                lines = File.ReadAllLines(basePath + "manifest-cards.csv");
                foreach (string line in lines)
                {
                    string[] fields = line.Split(',');
                    string cardID = fields[1];
                    int assetID = int.Parse(fields[0]);
                    string cardAsset = null;
                    if (!cardAssets.TryGetValue(cardID, out cardAsset)) continue;

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(cardAsset);
                    //AllCardAssets.Add(CardAsset.Load(cardID, assetID, doc.DocumentElement));

                    DataRow dataRow = dataTableCard.NewRow();
                    dataRow["CardID"] = cardID;
                    dataRow["AssetID"] = assetID;

                    TAG_CARD_SET cardSet = TAG_CARD_SET.INVALID;
                    bool collectible = false;
                    TAG_RARITY rarity = TAG_RARITY.INVALID;
                    foreach (XmlNode tagNode in doc.DocumentElement.GetElementsByTagName("Tag"))
                    {
                        switch (tagNode.Attributes["name"].Value)
                        {
                            case "CardName": dataRow["Name"] = tagNode.ChildNodes.Item(0).InnerText; break;
                            case "CardSet":
                                cardSet = (TAG_CARD_SET)int.Parse(tagNode.Attributes["value"].Value);
                                dataRow["Set"] = cardSet.ToString();
                                break;
                            case "CardType": dataRow["Type"] = ((TAG_CARDTYPE)int.Parse(tagNode.Attributes["value"].Value)).ToString(); break;
                            case "Faction": dataRow["Faction"] = ((TAG_FACTION)int.Parse(tagNode.Attributes["value"].Value)).ToString(); break;
                            case "Race": dataRow["Race"] = ((TAG_RACE)int.Parse(tagNode.Attributes["value"].Value)).ToString(); break;
                            case "Class": dataRow["Class"] = ((TAG_CLASS)int.Parse(tagNode.Attributes["value"].Value)).ToString(); break;
                            case "Rarity":
                                rarity = (TAG_RARITY)int.Parse(tagNode.Attributes["value"].Value);
                                dataRow["Rarity"] = rarity.ToString();
                                break;
                            case "Cost": dataRow["Cost"] = int.Parse(tagNode.Attributes["value"].Value); break;
                            case "Atk": dataRow["Atk"] = int.Parse(tagNode.Attributes["value"].Value); break;
                            case "Health": dataRow["Health"] = int.Parse(tagNode.Attributes["value"].Value); break;
                            case "AttackVisualType": dataRow["AttackVisualType"] = int.Parse(tagNode.Attributes["value"].Value); break;
                            case "CardTextInHand": dataRow["TextInHand"] = tagNode.ChildNodes.Item(0).InnerText; break;
                            case "CardTextInPlay": dataRow["TextInPlay"] = tagNode.ChildNodes.Item(0).InnerText; break;
                            case "DevState": dataRow["DevState"] = int.Parse(tagNode.Attributes["value"].Value); break;
                            case "EnchantmentBirthVisual": dataRow["EnchantmentBirthVisual"] = ((TAG_ENCHANTMENT_VISUAL)int.Parse(tagNode.Attributes["value"].Value)).ToString(); break;
                            case "EnchantmentIdleVisual": dataRow["EnchantmentIdleVisual"] = ((TAG_ENCHANTMENT_VISUAL)int.Parse(tagNode.Attributes["value"].Value)).ToString(); break;
                            case "FlavorText": dataRow["FlavorText"] = tagNode.ChildNodes.Item(0).InnerText; break;
                            case "ArtistName": dataRow["ArtistName"] = tagNode.ChildNodes.Item(0).InnerText; break;
                            case "TargetingArrowText": dataRow["TargetingArrowText"] = tagNode.ChildNodes.Item(0).InnerText; break;
                            case "HowToGetThisGoldCard": dataRow["HowToGetThisGoldCard"] = tagNode.ChildNodes.Item(0).InnerText; break;
                            case "HowToGetThisCard": dataRow["HowToGetThisCard"] = tagNode.ChildNodes.Item(0).InnerText; break;
                            case "Recall": dataRow["Recall"] = int.Parse(tagNode.Attributes["value"].Value); break;
                            case "Durability": dataRow["Durability"] = int.Parse(tagNode.Attributes["value"].Value); break;
                            case "TriggerVisual": dataRow["TriggerVisual"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Collectible":
                                collectible = tagNode.Attributes["value"].Value == "1";
                                dataRow["Collectible"] = collectible;
                                break;
                            case "Elite": dataRow["Elite"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Deathrattle": dataRow["Deathrattle"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Charge": dataRow["Charge"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Divine Shield": dataRow["DivineShield"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Windfury": dataRow["Windfury"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Taunt": dataRow["Taunt"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Aura": dataRow["Aura"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Enrage": dataRow["Enrage"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "OneTurnEffect": dataRow["OneTurnEffect"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Stealth": dataRow["Stealth"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Battlecry": dataRow["Battlecry"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Secret": dataRow["Secret"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Morph": dataRow["Morph"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "AffectedBySpellPower": dataRow["AffectedBySpellPower"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Freeze": dataRow["Freeze"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Spellpower": dataRow["Spellpower"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Combo": dataRow["Combo"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Silence": dataRow["Silence"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Summoned": dataRow["Summoned"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "ImmuneToSpellpower": dataRow["ImmuneToSpellpower"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "AdjacentBuff": dataRow["AdjacentBuff"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "GrantCharge": dataRow["GrantCharge"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "Poisonous": dataRow["Poisonous"] = tagNode.Attributes["value"].Value == "1"; break;
                            case "HealTarget": dataRow["HealTarget"] = tagNode.Attributes["value"].Value == "1"; break;

                            default:
                                if (!unhandledTags.Contains(tagNode.Attributes["name"].Value))
                                {
                                    unhandledTags.Add(tagNode.Attributes["name"].Value);
                                    Console.WriteLine("Unhandled Card Tag: {0} on {1}", tagNode.Attributes["name"].Value, cardID);
                                }
                                break;
                        }
                    }
                    foreach (XmlNode masterPowerNode in doc.DocumentElement.GetElementsByTagName("MasterPower")) dataRow["MasterPowerID"] = new Guid(masterPowerNode.InnerText);
                    bool craftable = cardSet == TAG_CARD_SET.EXPERT1 && collectible;
                    dataRow["Craftable"] = craftable;
                    if (craftable)
                    {
                        switch (rarity)
                        {
                            case TAG_RARITY.COMMON:
                                dataRow["StandardBuyValue"] = 40;
                                dataRow["StandardSellValue"] = 5;
                                dataRow["FoilBuyValue"] = 400;
                                dataRow["FoilSellValue"] = 50;
                                break;
                            case TAG_RARITY.RARE:
                                dataRow["StandardBuyValue"] = 100;
                                dataRow["StandardSellValue"] = 20;
                                dataRow["FoilBuyValue"] = 800;
                                dataRow["FoilSellValue"] = 100;
                                break;
                            case TAG_RARITY.EPIC:
                                dataRow["StandardBuyValue"] = 400;
                                dataRow["StandardSellValue"] = 100;
                                dataRow["FoilBuyValue"] = 1600;
                                dataRow["FoilSellValue"] = 400;
                                break;
                            case TAG_RARITY.LEGENDARY:
                                dataRow["StandardBuyValue"] = 1600;
                                dataRow["StandardSellValue"] = 400;
                                dataRow["FoilBuyValue"] = 3200;
                                dataRow["FoilSellValue"] = 1600;
                                break;
                            default: break;
                        }
                    }
                    dataTableCard.Rows.Add(dataRow);

                    foreach (XmlNode powerNode in doc.DocumentElement.GetElementsByTagName("Power"))
                    {
                        Guid powerID = new Guid(powerNode.Attributes["definition"].Value);

                        dataRow = dataTableCardPower.NewRow();
                        dataRow["CardPowerID"] = powerID;
                        dataRow["CardID"] = cardID;
                        dataTableCardPower.Rows.Add(dataRow);

                        foreach (XmlNode playRequirementNode in ((XmlElement)powerNode).GetElementsByTagName("PlayRequirement"))
                        {
                            dataRow = dataTableCardPowerRequirement.NewRow();
                            dataRow["CardPowerID"] = powerID;
                            dataRow["Type"] = ((PlayErrors.ErrorType)int.Parse(playRequirementNode.Attributes["reqID"].Value)).ToString();
                            string param = playRequirementNode.Attributes["param"].Value;
                            if (!string.IsNullOrEmpty(param)) dataRow["Parameter"] = int.Parse(param);
                            dataTableCardPowerRequirement.Rows.Add(dataRow);
                        }
                    }
                }
                new SqlBulkCopy(db) { DestinationTableName = "Card" }.WriteToServer(dataTableCard);
                new SqlBulkCopy(db) { DestinationTableName = "CardPower" }.WriteToServer(dataTableCardPower);
                new SqlBulkCopy(db) { DestinationTableName = "CardPowerRequirement" }.WriteToServer(dataTableCardPowerRequirement);
                // End of Manifest Processing
            }
        }
    }
}
