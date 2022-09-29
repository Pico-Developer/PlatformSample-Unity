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
        private static Dictionary<string, PPFFunctionConfig> achievementDic = new Dictionary<string, PPFFunctionConfig>()
        {
            ["AddCount"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return AchievementsService.AddCount(paramList[0], Convert.ToInt64(paramList[1]), System.Text.Encoding.UTF8.GetBytes(paramList[2])).OnComplete(OnAchievementAddCountComplete);
            }), new List<ParamName>()
            {
                ParamName.ACHIEVEMENT_NAME,
                ParamName.ACHIEVEMENT_COUNT,
                ParamName.ACHIEVEMENT_EXTRADATA
            }),
            ["AddFields"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return AchievementsService.AddFields(paramList[0]
                    , paramList[1]
                    , System.Text.Encoding.UTF8.GetBytes(paramList[2])).OnComplete(OnAchievementAddFieldsComplete);
            }), new List<ParamName>()
            {
                ParamName.ACHIEVEMENT_NAME,
                ParamName.ACHIEVEMENT_FIELDS,
                ParamName.ACHIEVEMENT_EXTRADATA
            }),
            ["GetAllDefinitions"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return AchievementsService.GetAllDefinitions(Convert.ToInt32(paramList[0])
                    , Convert.ToInt32(paramList[1])).OnComplete(OnAchievementGetAllDefinitionsComplete);
            }), new List<ParamName>()
            {
                ParamName.ACHIEVEMENT_PAGEINDEX, 
                ParamName.ACHIEVEMENT_PAGESIZE, 
            }),
            ["GetAllProgress"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return AchievementsService.GetAllProgress(Convert.ToInt32(paramList[0])
                , Convert.ToInt32(paramList[1])).OnComplete(OnAchievementGetAllProgressComplete);
            }), new List<ParamName>()
            {
                ParamName.ACHIEVEMENT_PAGEINDEX,
                ParamName.ACHIEVEMENT_PAGESIZE,
            }),
            ["GetDefinitionsByName"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                string[] names = paramList[0].Split(';');
                return AchievementsService.GetDefinitionsByName(names).OnComplete(OnAchievementGetDefinitionsByNameComplete);
            }), new List<ParamName>()
            {
                ParamName.ACHIEVEMENT_NAMES,
            }),
            ["GetProgressByName"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                string[] names = paramList[0].Split(';');
                return AchievementsService.GetProgressByName(names).OnComplete(OnAchievementGetProgressByNameComplete);
            }), new List<ParamName>()
            {
                ParamName.ACHIEVEMENT_NAMES,
            }),
            ["Unlock"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                return AchievementsService.Unlock(paramList[0]
                    , System.Text.Encoding.UTF8.GetBytes(paramList[1])).OnComplete(OnAchievementUnlockComplete);
            }), new List<ParamName>()
            {
                ParamName.ACHIEVEMENT_NAME,
                ParamName.ACHIEVEMENT_EXTRADATA,
            }),
        };
        static void OnAchievementAddCountComplete(Message<AchievementUpdate> message)
        {
            CommonProcess("OnAchievementAddCountComplete", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetAchievementUpdateLogData(message.Data));
            });
        }
        static void OnAchievementAddFieldsComplete(Message<AchievementUpdate> message)
        {
            CommonProcess("OnAchievementAddFieldsComplete", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetAchievementUpdateLogData(message.Data));
            });
        }
        static void OnAchievementGetAllDefinitionsComplete(Message<AchievementDefinitionList> message)
        {
            CommonProcess("OnAchievementGetAllDefinitionsComplete", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetAchievementDefinitionListLogData(message.Data));
            });
        }
        static void OnAchievementGetAllProgressComplete(Message<AchievementProgressList> message)
        {
            CommonProcess("OnAchievementGetAllProgressComplete", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetAchievementProgressListLogData(message.Data));
            });
        }
        static void OnAchievementGetDefinitionsByNameComplete(Message<AchievementDefinitionList> message)
        {
            CommonProcess("OnAchievementGetDefinitionsByNameComplete", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetAchievementDefinitionListLogData(message.Data));
            });
        }
        static void OnAchievementGetProgressByNameComplete(Message<AchievementProgressList> message)
        {
            CommonProcess("OnAchievementGetProgressByNameComplete", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetAchievementProgressListLogData(message.Data));
            });
        }
        static void OnAchievementUnlockComplete(Message<AchievementUpdate> message)
        {
            CommonProcess("OnAchievementUnlockComplete", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetAchievementUpdateLogData(message.Data));
            });
        }
    }
}