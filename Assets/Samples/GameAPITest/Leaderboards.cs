using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Pico.Platform;
using Pico.Platform.Models;


namespace Pico.Platform.Samples.Game
{
    public partial class GameAPITest : MonoBehaviour
    {
        private static Dictionary<string, PPFFunctionConfig> leaderboardDic = new Dictionary<string, PPFFunctionConfig>()
        {
            ["Get"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return LeaderboardService.Get(paramList[0]).OnComplete(OnLeaderboardGet);
            }), new List<ParamName>() { ParamName.LEADERBOARD_NAME }),
            ["GetEntries"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return LeaderboardService.GetEntries(paramList[0]
                    , Convert.ToInt32(paramList[1])
                    , Convert.ToInt32(paramList[2])
                    , (LeaderboardFilterType)Convert.ToInt32(paramList[3])
                    , (LeaderboardStartAt)Convert.ToInt32(paramList[4])).OnComplete(OnLeaderboardGetEntries);
            }), new List<ParamName>()
            {
                ParamName.LEADERBOARD_NAME,
                ParamName.LEADERBOARD_PAGESIZE,
                ParamName.LEADERBOARD_PAGEIDX,
                ParamName.LEADERBOARD_FILTER,
                ParamName.LEADERBOARD_STARTAT
            }),
            ["WriteEntry"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return LeaderboardService.WriteEntry(paramList[0]
                    , Convert.ToInt64(paramList[1])
                    , System.Text.Encoding.UTF8.GetBytes(paramList[2])
                    , Convert.ToBoolean(paramList[3])).OnComplete(OnLeaderboardWriteEntry);
            }), new List<ParamName>()
            {
                ParamName.LEADERBOARD_NAME, 
                ParamName.LEADERBOARD_SCORE, 
                ParamName.LEADERBOARD_BYTES, 
                ParamName.LEADERBOARD_UPDATE
            }),
            ["GetEntriesAfterRank"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return LeaderboardService.GetEntriesAfterRank(paramList[0]
                , Convert.ToInt32(paramList[1])
                , Convert.ToInt32(paramList[2])
                , Convert.ToUInt64(paramList[3])).OnComplete(OnLeaderboardGetEntriesAfterRank);
            }), new List<ParamName>()
            {
                ParamName.LEADERBOARD_NAME,
                ParamName.LEADERBOARD_PAGESIZE,
                ParamName.LEADERBOARD_PAGEIDX,
                ParamName.LEADERBOARD_AFTERRANK
            }),
            ["GetEntriesByIds"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                string[] userids = paramList[4].Split(';');
                return LeaderboardService.GetEntriesByIds(paramList[0]
                    , Convert.ToInt32(paramList[1])
                    , Convert.ToInt32(paramList[2])
                    , (LeaderboardStartAt)Convert.ToInt32(paramList[3])
                    , userids).OnComplete(OnLeaderboardGetEntriesByIds);
            }), new List<ParamName>()
            {
                ParamName.LEADERBOARD_NAME,
                ParamName.LEADERBOARD_PAGESIZE,
                ParamName.LEADERBOARD_PAGEIDX,
                ParamName.LEADERBOARD_STARTAT,
                ParamName.LEADERBOARD_USERIDS,
            }),
            ["WriteEntryWithSupplementaryMetric"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return LeaderboardService.WriteEntryWithSupplementaryMetric(paramList[0]
                    , Convert.ToInt32(paramList[1])
                    , Convert.ToInt32(paramList[2])
                    , System.Text.Encoding.UTF8.GetBytes(paramList[3])
                    , Convert.ToBoolean(paramList[4])).OnComplete(OnLeaderboardWriteEntryWithSupplementaryMetric);
            }), new List<ParamName>()
            {
                ParamName.LEADERBOARD_NAME,
                ParamName.LEADERBOARD_SCORE,
                ParamName.LEADERBOARD_SUPPLEMENTARYMETRIC,
                ParamName.LEADERBOARD_BYTES,
                ParamName.LEADERBOARD_UPDATE
            }),
        };
        #region leaderboard oncomplete
        static void OnLeaderboardGet(Message<LeaderboardList> message)
        {
            CommonProcess("OnLeaderboardGet", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetLeaderboardListLogData(message.Data));
            });
        }

        static void OnLeaderboardGetEntries(Message<LeaderboardEntryList> message)
        {
            CommonProcess("OnLeaderboardGetEntries", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetLeaderboardEntryListLogData(message.Data));
            });
        }
        static void OnLeaderboardWriteEntry(Message<bool> message)
        {
            CommonProcess("OnLeaderboardWriteEntry", message, () =>
            {
                LogHelper.LogInfo(TAG, message.Data.ToString());
            });
        }
        static void OnLeaderboardGetEntriesAfterRank(Message<LeaderboardEntryList> message)
        {
            CommonProcess("OnLeaderboardGetEntriesAfterRank", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetLeaderboardEntryListLogData(message.Data));
            });
        }
        static void OnLeaderboardGetEntriesByIds(Message<LeaderboardEntryList> message)
        {
            CommonProcess("OnLeaderboardGetEntriesByIds", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetLeaderboardEntryListLogData(message.Data));
            });
        }
        static void OnLeaderboardWriteEntryWithSupplementaryMetric(Message<bool> message)
        {
            CommonProcess("OnLeaderboardWriteEntryWithSupplementaryMetric", message, () =>
            {
                LogHelper.LogInfo(TAG, message.Data.ToString());
            });
        }
        #endregion leaderboard oncomplete
    }
}