using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Stonehearth
{
    public class CardAssetPower
    {
        public Guid Definition = Guid.Empty;

        public List<CardAssetPowerRequirement> Requirements = new List<CardAssetPowerRequirement>();

        public static CardAssetPower Load(XmlElement pRootElement)
        {
            CardAssetPower cardAssetPower = new CardAssetPower();
            cardAssetPower.Definition = new Guid(pRootElement.Attributes["definition"].Value);
            foreach (XmlNode playRequirementNode in pRootElement.GetElementsByTagName("PlayRequirement"))
            {
                CardAssetPowerRequirement cardAssetPowerRequirement = CardAssetPowerRequirement.Load((XmlElement)playRequirementNode);
                cardAssetPower.Requirements.Add(cardAssetPowerRequirement);
            }

            return cardAssetPower;
        }

        public override string ToString() { return Definition.ToString(); }
    }
}
