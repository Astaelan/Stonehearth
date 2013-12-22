using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Stonehearth
{
    public class CardAssetPowerRequirement
    {
        public PlayErrors.ErrorType Type = PlayErrors.ErrorType.NONE;
        public int Parameter = 0;

        public static CardAssetPowerRequirement Load(XmlElement pRootElement)
        {
            CardAssetPowerRequirement cardAssetPowerRequirement = new CardAssetPowerRequirement();
            cardAssetPowerRequirement.Type = (PlayErrors.ErrorType)int.Parse(pRootElement.Attributes["reqID"].Value);
            string param = pRootElement.Attributes["param"].Value;
            if (!string.IsNullOrEmpty(param)) cardAssetPowerRequirement.Parameter = int.Parse(param);
            return cardAssetPowerRequirement;
        }

        public override string ToString() { return Type.ToString() + " (" + Parameter + ")"; }
    }
}
