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
        private static List<string> InviteTokenList = new List<string>();
        // Rooms
        private Dictionary<string, PPFFunctionConfig> roomDic = new Dictionary<string, PPFFunctionConfig>()
        {
            ["Get"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID
                return RoomService.Get(Convert.ToUInt64(paramList[0])).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.ROOM_ID }),
            ["GetCurrentForUser"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // userID
                return RoomService.GetCurrentForUser(paramList[0]).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.USER_ID }),
            ["Join2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                var ulongRoomID = Convert.ToUInt64(paramList[0]);
                var roomOptions = GameUtils.GetRoomOptions(paramList[0], paramList[1], paramList[2], paramList[3], paramList[4], paramList[5]);
                return RoomService.Join2(ulongRoomID, roomOptions).OnComplete(ProcessRoomJoin2);
            }), new List<ParamName>() {
                ParamName.ROOM_ID,
                ParamName.ROOM_OPTION_MAX_USER_RESULTS,
                ParamName.ROOM_OPTION_TURN_OFF_UPDATES,
                ParamName.ROOM_OPTION_DATASTORE_KEYS,
                ParamName.ROOM_OPTION_DATASTORE_VALUES,
                ParamName.ROOM_OPTION_ELCLUDERECENTLYMET,
            }),
            ["Join2InviteRoom"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                var ulongRoomID = Convert.ToUInt64(inviteRoomID);
                var roomOptions = GameUtils.GetRoomOptions(inviteRoomID, paramList[0], paramList[1], paramList[2], paramList[3], paramList[4]);
                return RoomService.Join2(ulongRoomID, roomOptions).OnComplete(ProcessRoomJoin2);
            }), new List<ParamName>() {
                ParamName.ROOM_OPTION_MAX_USER_RESULTS,
                ParamName.ROOM_OPTION_TURN_OFF_UPDATES,
                ParamName.ROOM_OPTION_DATASTORE_KEYS,
                ParamName.ROOM_OPTION_DATASTORE_VALUES,
                ParamName.ROOM_OPTION_ELCLUDERECENTLYMET,
            }),
            ["KickUser"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, userID, kickDurationSeconds
                return RoomService.KickUser(Convert.ToUInt64(paramList[0]), paramList[1], Convert.ToInt32(paramList[2])).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.USER_ID, ParamName.KICK_DURATION_SECONDS }),
            ["GetCurrent"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // 
                return RoomService.GetCurrent().OnComplete(OnRoomMessage);
            })),
            ["Leave"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID
                return RoomService.Leave(Convert.ToUInt64(paramList[0])).OnComplete(ProcessRoomLeave);
            }), new List<ParamName>() { ParamName.ROOM_ID }),
            ["SetDescription"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, desc
                return RoomService.SetDescription(Convert.ToUInt64(paramList[0]), paramList[1]).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.DESCRIPTION }),
            ["UpdateDataStore"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID
                Dictionary<string, string> data = new Dictionary<string, string>();
                var rst = RoomService.UpdateDataStore(Convert.ToUInt64(paramList[0]), GameUtils.GetStringDicData(paramList[1], paramList[2])).OnComplete(OnRoomMessage);
                return rst;
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.ROOM_OPTION_DATASTORE_KEYS, ParamName.ROOM_OPTION_DATASTORE_VALUES }),
            ["UpdateMembershipLockStatus"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, membershipLockStatus
                return RoomService.UpdateMembershipLockStatus(Convert.ToUInt64(paramList[0]), (RoomMembershipLockStatus)Convert.ToInt32(paramList[1])).OnComplete(OnRoomMessage);

            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.MEMBERSHIP_LOCK_STATUS }),
            ["UpdateOwner"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, userID
                return RoomService.UpdateOwner(Convert.ToUInt64(paramList[0]), paramList[1]).OnComplete(ProcessRoomUpdateOwner);
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.USER_ID }),
            ["CreateAndJoinPrivate2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                var roomOptions = GameUtils.GetRoomOptions(paramList[2], paramList[3], paramList[4], paramList[5], paramList[6], paramList[7]);
                return RoomService.CreateAndJoinPrivate2((RoomJoinPolicy)Convert.ToInt32(paramList[0]), Convert.ToUInt32(paramList[1]), roomOptions).OnComplete(OnRoomMessage);
            }), new List<ParamName>() {
            ParamName.JOIN_POLICY,                      // 0
            ParamName.MAX_USERS,
            ParamName.ROOM_ID,
            ParamName.ROOM_OPTION_MAX_USER_RESULTS,     // 3
            ParamName.ROOM_OPTION_TURN_OFF_UPDATES,
            ParamName.ROOM_OPTION_DATASTORE_KEYS,       // 5
            ParamName.ROOM_OPTION_DATASTORE_VALUES,
            ParamName.ROOM_OPTION_ELCLUDERECENTLYMET,
        }),
            ["UpdatePrivateRoomJoinPolicy"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, joinPolicy
                return RoomService.UpdatePrivateRoomJoinPolicy(Convert.ToUInt64(paramList[0]), (RoomJoinPolicy)Convert.ToInt32(paramList[1])).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.JOIN_POLICY }),
            ["GetInvitableUsers2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                var roomOptions = GameUtils.GetRoomOptions(paramList[0], paramList[1], paramList[2], paramList[3], paramList[4], paramList[5]);
                return RoomService.GetInvitableUsers2(roomOptions).OnComplete(OnRoomUserListMessage);
            }), new List<ParamName>() {
            ParamName.ROOM_ID,
            ParamName.ROOM_OPTION_MAX_USER_RESULTS,
            ParamName.ROOM_OPTION_TURN_OFF_UPDATES,
            ParamName.ROOM_OPTION_DATASTORE_KEYS,       // 3
            ParamName.ROOM_OPTION_DATASTORE_VALUES,
            ParamName.ROOM_OPTION_ELCLUDERECENTLYMET,
        }),
            ["InviteUserByIndex"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                int index = Convert.ToInt32(paramList[1]);
                if (InviteTokenList.Count > index && index >= 0)
                {
                    string inviteToken = InviteTokenList[index];
                    return RoomService.InviteUser(Convert.ToUInt64(paramList[0]), inviteToken).OnComplete(OnRoomMessage);
                }
                else
                {
                    Debug.LogError("index out of bounds");
                    return 0;
                }
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.INDEX }),
            ["InviteUser"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, inviteToken
                return RoomService.InviteUser(Convert.ToUInt64(paramList[0]), paramList[1]).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.INVITE_TOKEN }),
            ["GetModeratedRooms"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // 
                return RoomService.GetModeratedRooms(Convert.ToInt32(paramList[0]), Convert.ToInt32(paramList[1])).OnComplete(OnRoomListMessage);
            }), new List<ParamName>() { ParamName.ROOM_PAGE_INDEX, ParamName.ROOM_PAGE_SIZE }),
            ["Net.SendPacket"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return NetworkService.SendPacket(paramList[0], System.Text.Encoding.UTF8.GetBytes(paramList[1]));
            }), new List<ParamName>() { ParamName.USER_ID, ParamName.PACKET_BYTES }),
            ["Net.SendPacketToCurrentRoom"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return NetworkService.SendPacketToCurrentRoom(System.Text.Encoding.UTF8.GetBytes(paramList[0]));
            }), new List<ParamName>() { ParamName.PACKET_BYTES }),
            ["Net.SendPacket2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return NetworkService.SendPacket(paramList[0], System.Text.Encoding.UTF8.GetBytes(paramList[1]), Convert.ToBoolean(paramList[2]));
            }), new List<ParamName>() { ParamName.USER_ID, ParamName.PACKET_BYTES, ParamName.RELIABLE }),
            ["Net.SendPacketToCurrentRoom2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return NetworkService.SendPacketToCurrentRoom(System.Text.Encoding.UTF8.GetBytes(paramList[0]), Convert.ToBoolean(paramList[1]));
            }), new List<ParamName>() { ParamName.PACKET_BYTES, ParamName.RELIABLE }),
            ["Net.ReadPacket"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                var packet = NetworkService.ReadPacket();
                if (packet != null)
                {
                    var str = packet.GetBytes();
                    if (!string.IsNullOrEmpty(str))
                    {
                        LogHelper.LogInfo(TAG, $"ReadPacket: {str}");
                        return true;
                    }
                }
                LogHelper.LogInfo(TAG, $"ReadPacket: null");
                return false;
            })),
            ["GetRoomInvites"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return NotificationService.GetRoomInviteNotifications(Convert.ToInt32(paramList[0]), Convert.ToInt32(paramList[1])).OnComplete(OnGetRoomInvitesComplete);
            }), new List<ParamName>() { ParamName.ROOM_INVITE_NOTIFICATION_PAGE_INDEX, ParamName.ROOM_INVITE_NOTIFICATION_PAGE_SIZE }),
            ["MarkAsRead"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return NotificationService.MarkAsRead(Convert.ToUInt64(paramList[0])).OnComplete(OnMarkAsReadComplete);
            }), new List<ParamName>() { ParamName.ROOM_INVITE_NOTIFICATION_ID }),
            ["LaunchInvitableUserFlow"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return RoomService.LaunchInvitableUserFlow(Convert.ToUInt64(paramList[0])).OnComplete(OnLaunchInvitableUserFlowComplete);
            }), new List<ParamName>() { ParamName.ROOM_ID }),
        };
        static void OnGetRoomInvitesComplete(Message<RoomInviteNotificationList> message)
        {
            CommonProcess("OnGetRoomInvitesComplete", message, () =>
            {
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomInviteNotificationListLogData(message.Data));
            });
        }

        static void OnMarkAsReadComplete(Message message)
        {
            CommonProcess("OnMarkAsReadComplete", message,
                () => { LogHelper.LogInfo("OnMarkAsReadComplete", $"{message.Type}"); });
        }
        static void OnRoomMessage(Message<Models.Room> message)
        {
            CommonProcess("OnRoomMessage", message, () =>
            {
                LogHelper.LogInfo("OnRoomMessage", $"msgType: {message.Type}, Room data: ");
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }

        static void OnRoomUserListMessage(Message<UserList> message)
        {
            CommonProcess("OnRoomUserMessage", message, () =>
            {
                InviteTokenList.Clear();
                LogHelper.LogInfo("OnRoomUserMessage", $"msgType: {message.Type}, UserList data: ");
                var userList = message.Data;
                for (int i = 0; i < userList.Capacity; i++)
                {
                    InviteTokenList.Add(userList[i].InviteToken);
                }
                LogHelper.LogInfo(TAG, GameDebugLog.GetUserListLogData(userList, true));
            });
        }

        static void OnRoomListMessage(Message<RoomList> message)
        {
            CommonProcess("OnRoomListMessage", message, () =>
            {
                LogHelper.LogInfo("OnRoomListMessage", message.Type.ToString());
                var roomList = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomListLogData(roomList));
            });
        }
        // join room callback
        static void ProcessRoomJoin2(Message<Models.Room> message)
        {
            CommonProcess("ProcessRoomJoin2", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }

        // leave room callback
        static void ProcessRoomLeave(Message<Models.Room> message)
        {
            CommonProcess("ProcessRoomLeave", message, () =>
            {
                var room = message.Data;
                LogHelper.LogInfo(TAG, GameDebugLog.GetRoomLogData(room));
            });
        }
    }
}