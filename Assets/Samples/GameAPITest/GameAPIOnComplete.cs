using System;
using System.Collections.Generic;
using System.IO;
using Stark;
using UnityEngine;
using UnityEngine.UI;

using Pico.Platform;
using Pico.Platform.Models;

namespace Pico.Platform.Samples.Game
{
    public partial class GameAPITest : MonoBehaviour
    {
        void OnMatchmakingCancel2Notification(Message message)
        {
            CommonProcess("OnMatchmakingCancel2Notification", message, () =>
            {
                LogHelper.LogInfo(TAG, "OnMatchmakingCancel2Notification");
            });
        }
        void OnRoomLeaveNotification(Message<Models.Room> message)
        {
            CommonProcess("OnRoomLeaveNotification", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }
        void OnRoomJoin2Notification(Message<Models.Room> message)
        {
            CommonProcess("OnRoomJoin2Notification", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }
        void OnRoomSetDescriptionNotification(Message<Models.Room> message)
        {
            CommonProcess("OnRoomSetDescriptionNotification", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }
        void OnRoomKickUserNotification(Message<Models.Room> message)
        {
            CommonProcess("OnRoomKickUserNotification", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }
        void OnRoomUpdateOwnerNotification(Message message)
        {
            CommonProcess("OnRoomUpdateOwnerNotification", message, () =>
            {
                LogHelper.LogInfo(TAG, "OnRoomUpdateOwnerNotification");
            });
        }
        void OnRoomUpdateDataStoreNotification(Message<Models.Room> message)
        {
            CommonProcess("OnRoomUpdateDataStoreNotification", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }
        void OnRoomUpdateMembershipLockStatusNotification(Message<Models.Room> message)
        {
            CommonProcess("OnRoomUpdateMembershipLockStatusNotification", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }
        
        
        static void OnLoggedInUser(Message<Models.User> msg)
        {
            if (!msg.IsError)
            {
                LogHelper.LogInfo(TAG, "Received get user success");
                Models.User user = msg.Data;
                LogHelper.LogInfo(TAG, $"User: name={user.DisplayName},ID={user.ID},headImage={user.ImageUrl},presenceStatus={user.PresenceStatus}");
            }
            else
            {
                LogHelper.LogInfo(TAG, "Received get user error");
                Error error = msg.GetError();
                LogHelper.LogInfo(TAG, "Error: " + error.Message);
            }
        }
        void OnGameConnectionEvent(Message<GameConnectionEvent> msg)
        {
            var state = msg.Data;
            LogHelper.LogInfo(TAG, $"OnGameConnectionEvent: {state}");
            if (state == GameConnectionEvent.Connected)
            {
                LogHelper.LogInfo(TAG, "GameConnection: success！");
            }
            else if (state == GameConnectionEvent.Closed)
            {
                Uninitialize();
                LogHelper.LogInfo(TAG, "GameConnection: fail！Please re-initialize！");
            }
            else if (state == GameConnectionEvent.GameLogicError)
            {
                Uninitialize();
                LogHelper.LogInfo(TAG, "GameConnection: fail！After successful reconnection, the logic state is found to be wrong，Please re-initialize！");
            }
            else if (state == GameConnectionEvent.Lost)
            {
                LogHelper.LogInfo(TAG, "GameConnection: Reconnecting, please wait！");
            }
            else if (state == GameConnectionEvent.Resumed)
            {
                LogHelper.LogInfo(TAG, "GameConnection: successful reconnection！");
            }
            else if (state == GameConnectionEvent.KickedByRelogin)
            {
                Uninitialize();
                LogHelper.LogInfo(TAG, "GameConnection: Repeat login! Please reinitialize！");
            }
            else if (state == GameConnectionEvent.KickedByGameServer)
            {
                Uninitialize();
                LogHelper.LogInfo(TAG, "GameConnection: Server kicks people! Please reinitialize！");
            }
            else
            {
                LogHelper.LogInfo(TAG, "GameConnection: unknown error！");
            }
        }

        void OnRequestFailed(Message<GameRequestFailedReason> msg)
        {
            LogHelper.LogInfo(TAG, $"OnRequestFailed: {msg.Data}");
        }

        void OnGameStateReset(Message msg)
        {
            LogHelper.LogInfo(TAG, $"OnGameStateReset");
        }

        void OnRoomInviteAcceptedNotification(Message<string> msg)
        {
            inviteRoomID = msg.Data;
            LogHelper.LogInfo(TAG, $"OnRoomInviteAcceptedNotification: {msg.Data}");
            var details = ApplicationService.GetLaunchDetails();
            LogHelper.LogInfo(TAG, $"GetLaunchDetails: RoomID: {details.RoomID}, ChallengeID: {details.ChallengeID}");
        }
        
        void OnChallengeInviteAcceptedOrLaunchAppNotification(Message<string> msg)
        {
            LogHelper.LogInfo(TAG, $"OnChallengeInviteAcceptedOrLaunchAppNotification: {msg.Data}");
            var details = ApplicationService.GetLaunchDetails();
            LogHelper.LogInfo(TAG, $"GetLaunchDetails: RoomID: {details.RoomID}, ChallengeID: {details.ChallengeID}");
        }

        void OnLaunchIntentChanged(Message<string> msg)
        {
            var details = ApplicationService.GetLaunchDetails();
            LogHelper.LogInfo(TAG, $"OnLaunchIntentChanged GetLaunchDetails: RoomID: {details.RoomID}, ChallengeID: {details.ChallengeID}");
        }
        
        static void OnGameInitialize(Message<GameInitializeResult> msg)
        {
            if (msg == null)
            {
                LogHelper.LogInfo(TAG, $"OnGameInitialize: fail, message is null");
                return;
            }

            if (msg.IsError)
            {
                LogHelper.LogError(TAG, $"GameInitialize Failed: {msg.Error.Code}, {msg.Error.Message}");
            }
            else
            {
                LogHelper.LogInfo(TAG, $"OnGameInitialize: {msg.Data}");
                if (msg.Data == GameInitializeResult.Success)
                {
                    LogHelper.LogInfo(TAG, "GameInitialize: success！");
                }
                else
                {
                    Uninitialize();
                    LogHelper.LogInfo(TAG, "GameInitialize: fail！Please re-initialize！");
                }
            }
        }

        
        static void OnLaunchInvitableUserFlowComplete(Message message)
        {
            CommonProcess("OnLaunchInvitableUserFlowComplete", message,
                () => { LogHelper.LogInfo("OnLaunchInvitableUserFlowComplete", $"{message.Type}"); });
        }
        
        static void ProcessRoomUpdateOwner(Message message)
        {
            CommonProcess("ProcessRoomUpdateOwner", message, () =>
            {
                LogHelper.LogInfo(TAG, "Room_UpdateOwner is VoidMessage");
            });
        }

        void ProcessRoomUpdate(Message<Models.Room> message)
        {
            CommonProcess("ProcessRoomUpdate", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }

        // match found callback
        void ProcessMatchmakingMatchFound(Message<Models.Room> message)
        {
            CommonProcess("ProcessMatchmakingMatchFound", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }

        static void CommonProcess(string funName, Message message, Action action)
        {
            LogHelper.LogInfo(TAG, $"message.Type: {message.Type}");
            if (!message.IsError)
            {
                LogHelper.LogInfo(TAG, $"{funName} no error");
                action();
            }
            else
            {
                var error = message.GetError();
                LogHelper.LogInfo(TAG, $"{funName} error: {error.Message}");
            }
        }
        
    }
}