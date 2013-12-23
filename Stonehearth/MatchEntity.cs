using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth
{
    public class MatchEntity
    {
        public Match Match = null;
        public int EntityID = 0;
        public string Name = string.Empty;
        private Dictionary<GAME_TAG, int> mTags = new Dictionary<GAME_TAG, int>();

        public MatchEntity(Match pMatch)
        {
            Match = pMatch;
            EntityID = Match.GetNextEntityID();
            mTags[GAME_TAG.ENTITY_ID] = EntityID;
        }

        //public void ClearTags()
        //{
        //    mTags.Clear();
        //    mTags[GAME_TAG.ENTITY_ID] = EntityID;
        //}

        public PegasusGame.PowerHistoryTagChange GetTag(GAME_TAG pTag)
        {
            PegasusGame.PowerHistoryTagChange.Builder powerHistoryTagChangeBuilder = PegasusGame.PowerHistoryTagChange.CreateBuilder();
            powerHistoryTagChangeBuilder.SetEntity(EntityID);
            powerHistoryTagChangeBuilder.SetTag((int)pTag);
            if (mTags.ContainsKey(pTag)) powerHistoryTagChangeBuilder.SetValue(mTags[pTag]);
            return powerHistoryTagChangeBuilder.Build();
        }

        public PegasusGame.PowerHistoryTagChange SetTag(GAME_TAG pTag, int pValue)
        {
            mTags[pTag] = pValue;
            PegasusGame.PowerHistoryTagChange.Builder powerHistoryTagChangeBuilder = PegasusGame.PowerHistoryTagChange.CreateBuilder();
            powerHistoryTagChangeBuilder.SetEntity(EntityID);
            powerHistoryTagChangeBuilder.SetTag((int)pTag);
            powerHistoryTagChangeBuilder.SetValue(pValue);
            return powerHistoryTagChangeBuilder.Build();
        }

        public void SetZoneAndPositionTags(TAG_ZONE pZone, int pPosition)
        {
            mTags[GAME_TAG.ZONE] = (int)pZone;
            mTags[GAME_TAG.ZONE_POSITION] = pPosition;
        }


        public PegasusGame.Entity ToEntity()
        {
            PegasusGame.Entity.Builder entityBuilder = PegasusGame.Entity.CreateBuilder();
            entityBuilder.SetId(EntityID);
            foreach (KeyValuePair<GAME_TAG, int> tag in mTags)
                entityBuilder.AddTags(PegasusGame.Tag.CreateBuilder().SetName((int)tag.Key).SetValue(tag.Value));
            return entityBuilder.Build();
        }

        public PegasusGame.PowerHistoryEntity ToHiddenPowerHistoryEntity()
        {
            PegasusGame.PowerHistoryEntity.Builder powerHistoryEntityBuilder = PegasusGame.PowerHistoryEntity.CreateBuilder();
            powerHistoryEntityBuilder.SetEntity(EntityID);
            powerHistoryEntityBuilder.SetName("");
            foreach (KeyValuePair<GAME_TAG, int> tag in mTags)
            {
                switch (tag.Key)
                {
                    case GAME_TAG.ENTITY_ID:
                    case GAME_TAG.ZONE:
                    case GAME_TAG.ZONE_POSITION:
                    case GAME_TAG.CONTROLLER:
                    case GAME_TAG.CREATOR:
                    case GAME_TAG.CANT_PLAY:
                        break;
                    default: continue;
                }
                powerHistoryEntityBuilder.AddTags(PegasusGame.Tag.CreateBuilder().SetName((int)tag.Key).SetValue(tag.Value));
            }
            return powerHistoryEntityBuilder.Build();
        }

        public PegasusGame.PowerHistoryEntity ToShownPowerHistoryEntity()
        {
            PegasusGame.PowerHistoryEntity.Builder powerHistoryEntityBuilder = PegasusGame.PowerHistoryEntity.CreateBuilder();
            powerHistoryEntityBuilder.SetEntity(EntityID);
            powerHistoryEntityBuilder.SetName(string.IsNullOrEmpty(Name) ? "" : Name);
            foreach (KeyValuePair<GAME_TAG, int> tag in mTags)
                powerHistoryEntityBuilder.AddTags(PegasusGame.Tag.CreateBuilder().SetName((int)tag.Key).SetValue(tag.Value));
            return powerHistoryEntityBuilder.Build();
        }
    }
}
