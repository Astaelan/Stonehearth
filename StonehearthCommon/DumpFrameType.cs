using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthCommon
{
    public enum DumpFrameType
    {
        GameStarted = 100,
        GameFinished = 101,


        Init = 200,
        IsInitialized = 201,
        AppQuit = 202,
        ApplicationWasPaused = 203,
        ApplicationWasUnpaused = 204,
        GetLaunchOption = 205,
        GetStoredBNetIPAddress = 206,
        RequestCloseAurora = 207,
        CloseAurora = 208,
        QueryAurora = 209,
        ProvideWebAuthToken = 210,
        CheckWebAuth = 211,
        GetShutdownMinutes = 212,
        BattleNetStatus = 213,
        GetMyGameAccountId = 214,
        GetAccountCountry = 215,
        GetAccountRegion = 216,
        GetCurrentRegion = 217,
        GetPlayRestrictions = 218,
        SendUtilPacket = 219,
        NextUtilPacket = 220,
        GetQueueInfo = 221,
        GetQueueEvent = 222,
        GetBnetEvents = 223,
        ClearBnetEvents = 224,
        GetErrors = 225,
        ClearErrors = 226,
        GetFriendsInfo = 227,
        GetFriendsUpdates = 228,
        ClearFriendsUpdates = 229,
        SendFriendInvite = 230,
        ManageFriendInvite = 231,
        RemoveFriend = 232,
        GetWhisperInfo = 233,
        GetWhispers = 234,
        ClearWhispers = 235,
        SendWhisper = 236,
        FilterProfanity = 237,
        GetChallenges = 238,
        ClearChallenges = 239,
        AnswerChallenge = 240,
        CancelChallenge = 241,
        GetPartyUpdatesInfo = 242,
        GetPartyUpdates = 243,
        ClearPartyUpdates = 244,
        AcceptPartyInvite = 245,
        DeclinePartyInvite = 246,
        SendPartyInvite = 247,
        RescindPartyInvite = 248,
        SetPartyDeck = 249,
        GetPresence = 250,
        ClearPresence = 251,
        SetPresenceBlob = 252,
        SetPresenceBool = 253,
        SetPresenceInt = 254,
        SetPresenceString = 255,
        SetRichPresence = 256,
        DraftQueue = 257,
        RankedMatch = 258,
        UnrankedMatch = 259,
        StartScenario = 260,
        StartScenarioAI = 261,


        ClientGamePacket = 300,
        ServerGamePacket = 301,

        FrameTerminator = 0x0BADC0DE,
    }
}
