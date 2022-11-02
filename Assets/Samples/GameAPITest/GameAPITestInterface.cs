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
        private static string tempToken = string.Empty;
        Dictionary<ParamName, string[]> paramsDic = new System.Collections.Generic.Dictionary<ParamName, string[]>()
        {
            [ParamName.PICO_ID] = new string[] { "PicoID", "" },
            [ParamName.ROOM_ID] = new string[] { "RoomID", "" },
            [ParamName.USER_ID] = new string[] { "UserID", "" },

            //SUBSCRIBE_TO_UPDATES,
            [ParamName.KICK_DURATION_SECONDS] = new string[] { "Kick time", "0" },
            [ParamName.DESCRIPTION] = new string[] { "Room Description", "room desc" },
            [ParamName.MEMBERSHIP_LOCK_STATUS] = new string[] { "Member lock status", "0" },
            [ParamName.INIT_HOST] = new string[] { "Host", "testhost" },
            [ParamName.INIT_PORT] = new string[] { "Port", "80" },
            [ParamName.INIT_TOKEN] = new string[] { "Token", "testtoken" },
            [ParamName.INIT_APPID] = new string[] { "AppID", "testappid" },

            [ParamName.POOL_NAME] = new string[] { "Pool name", "test_match_pool" }, //
            [ParamName.JOIN_POLICY] = new string[] { "JoinPolicy", "0" },

            [ParamName.MATCH_MAX_LEVEL] = new string[] { "MatchMaxLevel", "0" },
            [ParamName.MATCH_APPROACH] = new string[] { "MatchApproach", "0" },
            [ParamName.INVITE_TOKEN] = new string[] { "InviteToken", "" },

            //ROOM_OPTION_EXCLUDE_RECENTLY_MET,
            [ParamName.ROOM_OPTION_MAX_USER_RESULTS] = new string[] { "Max user results", "" },
            [ParamName.ROOM_OPTION_TURN_OFF_UPDATES] = new string[] { "Turn off updates", "" },

            [ParamName.ROOM_OPTION_DATASTORE_KEYS] = new string[] { "Keys", "" },
            [ParamName.ROOM_OPTION_DATASTORE_VALUES] = new string[] { "Valuse", "" },
            [ParamName.ROOM_OPTION_ELCLUDERECENTLYMET] = new string[] { "Elcluderecentlymet", "false" },


            [ParamName.MATCHMAKING_OPTION_ROOM_MAX_USERS] = new string[] { "Max users", "2" },
            [ParamName.MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG] = new string[] { "EnqueueIsDebug", "" },
            [ParamName.MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY] = new string[] { "Enqueue query key", "" },


            [ParamName.MATCHMAKING_OPTION_ENQUEUE_KEYS] = new string[] { "EnqueueKeys", "" },
            [ParamName.MATCHMAKING_OPTION_ENQUEUE_VALUES] = new string[] { "EnqueueValues", "" },
            [ParamName.MATCHMAKING_OPTION_ROOM_KEYS] = new string[] { "RoomKeys", "" },
            [ParamName.MATCHMAKING_OPTION_ROOM_VALUES] = new string[] { "RoomValues", "" },

            [ParamName.MATCHMAKING_REPORT_RESULT_KEYS] = new string[] { "ReportResultKeys", "" },
            [ParamName.MATCHMAKING_REPORT_RESULT_VALUES] = new string[] { "ReportResultValues", "" },

            [ParamName.MAX_USERS] = new string[] { "Max users", "2" },
            [ParamName.PACKET_BYTES] = new string[] { "Content", "test packetBytes" },
            [ParamName.ROOM_PAGE_INDEX] = new string[] { "Page index", "0" },
            [ParamName.ROOM_PAGE_SIZE] = new string[] { "Page size", "5" },
            [ParamName.RELIABLE] = new string[]{"Reliable", "true"},
            
            [ParamName.LEADERBOARD_NAME] = new string[]{"LeaderboardName", "http"},
            [ParamName.LEADERBOARD_SCORE] = new string[]{"score", "77"},
            [ParamName.LEADERBOARD_BYTES] = new string[]{"extraData", "test-extradata"},
            [ParamName.LEADERBOARD_UPDATE] = new string[]{"forceUpdate", "true"},
            [ParamName.LEADERBOARD_PAGESIZE] = new string[]{"pagesize", "5"},
            [ParamName.LEADERBOARD_PAGEIDX] = new string[]{"pageindex", "0"},
            [ParamName.LEADERBOARD_FILTER] = new string[]{"filter", "0"},
            [ParamName.LEADERBOARD_STARTAT] = new string[]{"startat", "0"},
            [ParamName.LEADERBOARD_SUPPLEMENTARYMETRIC] = new string[]{"supplementaryMetric", "0"},
            [ParamName.LEADERBOARD_AFTERRANK] = new string[]{"afterRank", "0"},
            [ParamName.LEADERBOARD_USERIDS] = new string[]{"userIDs", "0"},
            [ParamName.ROOM_INVITE_NOTIFICATION_ID] = new string[]{"notificationId", "0"},
            [ParamName.ROOM_INVITE_NOTIFICATION_PAGE_SIZE] = new string[]{"pageSize", "5"},
            [ParamName.ROOM_INVITE_NOTIFICATION_PAGE_INDEX] = new string[]{"pageIndex", "0"},
            
            [ParamName.ACHIEVEMENT_NAME] = new string[]{"name", "0"},
            [ParamName.ACHIEVEMENT_EXTRADATA] = new string[]{"extradata", "0"},
            [ParamName.ACHIEVEMENT_NAMES] = new string[]{"names", "0"},
            [ParamName.ACHIEVEMENT_PAGEINDEX] = new string[]{"pageIndex", "0"},
            [ParamName.ACHIEVEMENT_PAGESIZE] = new string[]{"pageSize", "5"},
            [ParamName.ACHIEVEMENT_FIELDS] = new string[]{"fields", "0"},
            [ParamName.ACHIEVEMENT_COUNT] = new string[]{"count", "0"},
            
            [ParamName.ROOM_INVITE_NOTIFICATION_ID] = new string[]{"notificationId", "0"},
            [ParamName.ROOM_INVITE_NOTIFICATION_PAGE_INDEX] = new string[]{"pageIndex", "0"},
            [ParamName.ROOM_INVITE_NOTIFICATION_PAGE_SIZE] = new string[]{"pageSize", "5"},

            [ParamName.INDEX] = new string[]{"index", "0"},
            [ParamName.APPID] = new string[] { "AppId", "" },
        };

        private static Dictionary<string, PPFFunctionConfig> initDic = new Dictionary<string, PPFFunctionConfig>()
        {
            ["GameInitialize"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                CoreService.Initialize(GameConfig.GetAppId());
                if (!CoreService.Initialized)
                {
                    LogHelper.LogError(TAG, "pico initialize failed");
                    return -1;
                }
                UserService.GetAccessToken().OnComplete(delegate (Message<string> message)
                {
                    if (message.IsError)
                    {
                        var err = message.GetError();
                        LogHelper.LogError(TAG, $"Got access token error {err.Message} code={err.Code}");
                        return;
                    }

                    string accessToken = message.Data;
                    LogHelper.LogInfo(TAG, $"Got accessToken {accessToken}, GameInitialize begin");

                    var request = CoreService.GameInitialize(accessToken);
                    if (request.TaskId != 0)
                    {
                        request.OnComplete(OnGameInitialize);
                    }
                    else
                    {
                        Debug.Log($"Core.GameInitialize requestID is 0! Repeated initialization or network error");
                    }
                });
                return 1;
            })),
            ["Uninitialize"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                Uninitialize();
                return true;
            })),
            ["GetLoggedInUser"] = new PPFFunctionConfig((paramList) => {
                return UserService.GetLoggedInUser().OnComplete(OnLoggedInUser); 
            }),
            ["Only Pico initialize"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                CoreService.Initialize(GameConfig.GetAppId());
                return 1;
            })),
            ["GetAccessToken"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                if (!CoreService.Initialized)
                {
                    LogHelper.LogError(TAG, "pico initialize failed");
                    return -1;
                }
                UserService.GetAccessToken().OnComplete(delegate (Message<string> message)
                {
                    if (message.IsError)
                    {
                        var err = message.GetError();
                        LogHelper.LogError(TAG, $"Got access token error {err.Message} code={err.Code}");
                        return;
                    }

                    string accessToken = message.Data;
                    LogHelper.LogInfo(TAG, $"Got accessToken {accessToken}, GameInitialize begin");
                    tempToken = accessToken;
                });
                return 1;
            })),
            ["Only game initialize"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                var request = CoreService.GameInitialize(tempToken);
                if (request.TaskId != 0)
                {
                    request.OnComplete(OnGameInitialize);
                }
                else
                {
                    Debug.Log($"Core.GameInitialize requestID is 0! Repeated initialization or network error");
                }
                return 1;
            })),
            ["GameInitializeWithoutToken"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                CoreService.Initialize(GameConfig.GetAppId());
                var request = CoreService.GameInitialize();
                request.OnComplete(OnGameInitialize);
                return 1;
            })),
            ["SetAppId"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                if (!string.IsNullOrEmpty(paramList[0]))
                {
                    Debug.Log($"set appid：{paramList[0]}");
                    GameConfig.SetAppId(paramList[0]);
                }
                else
                {
                    Dropdown appIdDropDown = GameObject.Find("Canvas/Panel/FunctionPanel/ParamsPanel/AppId/Dropdown").GetComponent<Dropdown>();
                    Debug.Log($"select appid：{appIdDropDown.captionText.text}");
                    GameConfig.SetAppId(appIdDropDown.captionText.text);
                }
                return 1;
            }), new List<ParamName>() {
                ParamName.APPID
            }),
        };
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
        };
        
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
    }
}
