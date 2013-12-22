using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth
{
    public sealed class MatchEntity
    {
        public int ID = 0;
        public string Name = null;
        private Dictionary<GAME_TAG, int> mTags = new Dictionary<GAME_TAG, int>();

        public MatchEntity(int pID)
        {
            ID = pID;
            mTags[GAME_TAG.ENTITY_ID] = pID;
        }

        public PegasusGame.PowerHistoryData GetTag(GAME_TAG pTag)
        {
            PegasusGame.PowerHistoryData.Builder powerHistoryDataBuilder = PegasusGame.PowerHistoryData.CreateBuilder();
            PegasusGame.PowerHistoryTagChange.Builder powerHistoryTagChangeBuilder = PegasusGame.PowerHistoryTagChange.CreateBuilder();
            powerHistoryTagChangeBuilder.SetEntity(ID);
            powerHistoryTagChangeBuilder.SetTag((int)pTag);
            if (mTags.ContainsKey(pTag)) powerHistoryTagChangeBuilder.SetValue(mTags[pTag]);
            powerHistoryDataBuilder.SetTagChange(powerHistoryTagChangeBuilder);
            return powerHistoryDataBuilder.Build();
        }

        public PegasusGame.PowerHistoryData SetTag(GAME_TAG pTag, int pValue)
        {
            mTags[pTag] = pValue;
            PegasusGame.PowerHistoryData.Builder powerHistoryDataBuilder = PegasusGame.PowerHistoryData.CreateBuilder();
            PegasusGame.PowerHistoryTagChange.Builder powerHistoryTagChangeBuilder = PegasusGame.PowerHistoryTagChange.CreateBuilder();
            powerHistoryTagChangeBuilder.SetEntity(ID);
            powerHistoryTagChangeBuilder.SetTag((int)pTag);
            powerHistoryTagChangeBuilder.SetValue(pValue);
            powerHistoryDataBuilder.SetTagChange(powerHistoryTagChangeBuilder);
            return powerHistoryDataBuilder.Build();
        }

        public void ClearTags()
        {
            mTags.Clear();
            mTags[GAME_TAG.ENTITY_ID] = ID;
        }


        public PegasusGame.Entity ToEntity()
        {
            PegasusGame.Entity.Builder entityBuilder = PegasusGame.Entity.CreateBuilder();
            entityBuilder.SetId(ID);
            foreach (KeyValuePair<GAME_TAG, int> tag in mTags)
                entityBuilder.AddTags(PegasusGame.Tag.CreateBuilder().SetName((int)tag.Key).SetValue(tag.Value));
            return entityBuilder.Build();
        }

        public PegasusGame.PowerHistoryEntity ToPowerHistoryEntity()
        {
            PegasusGame.PowerHistoryEntity.Builder powerHistoryEntityBuilder = PegasusGame.PowerHistoryEntity.CreateBuilder();
            powerHistoryEntityBuilder.SetEntity(ID);
            powerHistoryEntityBuilder.SetName(string.IsNullOrEmpty(Name) ? "" : Name);
            foreach (KeyValuePair<GAME_TAG, int> tag in mTags)
                powerHistoryEntityBuilder.AddTags(PegasusGame.Tag.CreateBuilder().SetName((int)tag.Key).SetValue(tag.Value));
            return powerHistoryEntityBuilder.Build();
        }
    }
}
