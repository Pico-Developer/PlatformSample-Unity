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
        // Matchmaking
        Dictionary<string, PPFFunctionConfig> matchDic = new Dictionary<string, PPFFunctionConfig>()
        {
            ["Crash test"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return CLIB.ppf_Matchmaking_CrashTest();
            })),
            ["Matchmaking\nEnqueue2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                MatchmakingOptions options = GameUtils.GetMatchmakingOptions(paramList[1], paramList[2], paramList[3], paramList[4], paramList[5], paramList[6], paramList[7]);
                return MatchmakingService.Enqueue2(paramList[0], options).OnComplete(ProcessMatchmakingEnqueue);
            }), new List<ParamName>() {
            ParamName.POOL_NAME,
            ParamName.MATCHMAKING_OPTION_ROOM_MAX_USERS,    // 1
            ParamName.MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG,  // 2
            ParamName.MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY, // 3
            ParamName.MATCHMAKING_OPTION_ENQUEUE_KEYS,       // 4
            ParamName.MATCHMAKING_OPTION_ENQUEUE_VALUES,     // 5
            ParamName.MATCHMAKING_OPTION_ROOM_KEYS,          // 6
            ParamName.MATCHMAKING_OPTION_ROOM_VALUES,        // 7
        }),
            ["Cancel2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return MatchmakingService.Cancel().OnComplete(OnMatchmakingCancelComplete);
            })),
            ["ReportResultInsecure"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return MatchmakingService.ReportResultsInsecure(Convert.ToUInt64(paramList[0]), GameUtils.GetDicData(paramList[1], paramList[2])).OnComplete(OnReportResultsInsecureComplete);
            }), new List<ParamName>() { ParamName.ROOM_ID,
        ParamName.MATCHMAKING_REPORT_RESULT_KEYS,
        ParamName.MATCHMAKING_REPORT_RESULT_VALUES,}),
            ["StartMatch"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID
                return MatchmakingService.StartMatch(Convert.ToUInt64(paramList[0])).OnComplete(OnStartMatchComplete);
            }), new List<ParamName>() { ParamName.ROOM_ID }),
            ["GetAdminSnapshot"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // 
                return MatchmakingService.GetAdminSnapshot().OnComplete(ProcessMatchmakingGetAdminSnapshot);
            })),
            ["GetStats"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // pool, maxLevel, approach
                return MatchmakingService.GetStats(paramList[0], Convert.ToUInt32(paramList[1]), (MatchmakingStatApproach)Convert.ToInt32(paramList[2])).OnComplete(ProcessMatchmakingGetStats);

            }), new List<ParamName>() { ParamName.POOL_NAME, ParamName.MATCH_MAX_LEVEL, ParamName.MATCH_APPROACH }),
            ["CreateAndEnqueueRoom2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                MatchmakingOptions options = GameUtils.GetMatchmakingOptions(paramList[1], paramList[2], paramList[3], paramList[4], paramList[5], paramList[6], paramList[7]);
                return MatchmakingService.CreateAndEnqueueRoom2(paramList[0], options).OnComplete(ProcessCreateAndEnqueueRoom2);
            }), new List<ParamName>() {
            ParamName.POOL_NAME,
            ParamName.MATCHMAKING_OPTION_ROOM_MAX_USERS,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_KEYS,       // 4
            ParamName.MATCHMAKING_OPTION_ENQUEUE_VALUES,     // 5
            ParamName.MATCHMAKING_OPTION_ROOM_KEYS,          // 6
            ParamName.MATCHMAKING_OPTION_ROOM_VALUES,        // 7
        }),
            ["Browse2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                MatchmakingOptions options = GameUtils.GetMatchmakingOptions(paramList[1], paramList[2], paramList[3], paramList[4], paramList[5], paramList[6], paramList[7]);
                return MatchmakingService.Browse2(paramList[0], options).OnComplete(ProcessMatchmakingBrowse2);
            }), new List<ParamName>() {
            ParamName.POOL_NAME,
            ParamName.MATCHMAKING_OPTION_ROOM_MAX_USERS,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_KEYS,       // 4
            ParamName.MATCHMAKING_OPTION_ENQUEUE_VALUES,     // 5
            ParamName.MATCHMAKING_OPTION_ROOM_KEYS,          // 6
            ParamName.MATCHMAKING_OPTION_ROOM_VALUES,        // 7
        }),
            ["Browse2CustomPage"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                MatchmakingOptions options = GameUtils.GetMatchmakingOptions(paramList[1]
                    , paramList[2]
                    , paramList[3]
                    , paramList[4]
                    , paramList[5]
                    , paramList[6]
                    , paramList[7]);
                return MatchmakingService.Browse2ForCustomPage(paramList[0], options, Convert.ToInt32(paramList[8]), Convert.ToInt32(paramList[9])).OnComplete(OnMatchmakingBrowse2CustomPageComplete);
            }), new List<ParamName>() {
                ParamName.POOL_NAME,
                ParamName.MATCHMAKING_OPTION_ROOM_MAX_USERS,
                ParamName.MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG,
                ParamName.MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY,
                ParamName.MATCHMAKING_OPTION_ENQUEUE_KEYS,       // 4
                ParamName.MATCHMAKING_OPTION_ENQUEUE_VALUES,     // 5
                ParamName.MATCHMAKING_OPTION_ROOM_KEYS,          // 6
                ParamName.MATCHMAKING_OPTION_ROOM_VALUES,        // 7
                ParamName.PAGE_INDEX,                            // 8
                ParamName.PAGE_SIZE,                             // 9
            }),
        };
        // matchmaking enqueue callback
        static void ProcessMatchmakingEnqueue(Message<MatchmakingEnqueueResult> message)
        {
            CommonProcess("ProcessMatchmakingEnqueue", message, () =>
            {
                var result = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetMatchmakingEnqueueResultLogData(result));
            });
        }
        static void OnMatchmakingCancelComplete(Message message)
        {
            CommonProcess("OnMatchmakingCancelComplete", message,
                () => { LogHelper.LogInfo("OnMatchmakingCancelComplete", $"{message.Type}"); });
        }
        static void OnReportResultsInsecureComplete(Message message)
        {
            CommonProcess("OnReportResultsInsecureComplete", message,
                () => { LogHelper.LogInfo("OnReportResultsInsecureComplete", $"{message.Type}"); });
        }
        static void OnStartMatchComplete(Message message)
        {
            CommonProcess("OnStartMatchComplete", message,
                () => { LogHelper.LogInfo("OnStartMatchComplete", $"{message.Type}"); });
        }
        static void ProcessMatchmakingGetAdminSnapshot(Message<MatchmakingAdminSnapshot> message)
        {
            CommonProcess("ProcessMatchmakingGetAdminSnapshot", message, () =>
            {
                var o = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetMatchmakingAdminSnapshotLogData(o));
            });
        }
        static void ProcessMatchmakingGetStats(Message<MatchmakingStats> message)
        {
            CommonProcess("ProcessMatchmakingGetStats", message, () =>
            {
                var matchStats = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetMatchmakingStatsLogData(matchStats));
            });
        }
        static void ProcessCreateAndEnqueueRoom2(Message<MatchmakingEnqueueResultAndRoom> message)
        {
            CommonProcess("ProcessCreateAndEnqueueRoom2", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetMatchmakingEnqueueResultAndRoomLogData(message.Data));
            });
        }
        static void ProcessMatchmakingBrowse2(Message<MatchmakingBrowseResult> message)
        {
            CommonProcess("ProcessMatchmakingBrowse2", message, () =>
            {
                var data = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetMatchmakingEnqueueResultLogData(data.EnqueueResult));
                LogHelper.LogInfo(TAG, GameDebugLog.GetMatchmakingRoomListLogData(data.MatchmakingRooms));
            });
        }
        static void OnMatchmakingBrowse2CustomPageComplete(Message<MatchmakingBrowseResult> message)
        {
            CommonProcess("OnMatchmakingBrowse2CustomPageComplete", message, () =>
            {
                var data = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetMatchmakingEnqueueResultLogData(data.EnqueueResult));
                LogHelper.LogInfo(TAG, GameDebugLog.GetMatchmakingRoomListLogData(data.MatchmakingRooms));
            });
        }
    }
}