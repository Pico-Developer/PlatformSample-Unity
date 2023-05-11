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
            [ParamName.ROOM_OPTION_NAME] = new string[] { "Name", "" },
            [ParamName.ROOM_OPTION_PASSWORD] = new string[] { "Password", "" },

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
            [ParamName.CREATE_IF_NOT_EXIST] = new string[] { "CreateIfNotExist", "true" },
            
            [ParamName.PAGE_INDEX] = new string[] { "PageIndex", "0" },
            [ParamName.PAGE_SIZE] = new string[] { "PageSize", "5" },
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
        
        
        
    }
}
