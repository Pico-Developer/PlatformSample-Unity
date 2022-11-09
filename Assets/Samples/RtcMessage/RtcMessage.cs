using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples.RtcMessage
{
    struct RtcUser
    {
        public string userId;
        public string userExtra;

        public RtcUser(string userId, string userExtra)
        {
            this.userId = userId;
            this.userExtra = userExtra;
        }
    }

    class RtcUserList
    {
        private List<RtcUser> users = new List<RtcUser>();
        private Dropdown dropdown;

        public RtcUserList(Dropdown userSelect)
        {
            this.dropdown = userSelect;
        }

        public void Add(string userId, string userExtra)
        {
            for (var i = 0; i < users.Count; i++)
            {
                if (userId.Equals(users[i].userId))
                {
                    return;
                }
            }

            users.Add(new RtcUser(userId, userExtra));
            dropdown.AddOptions(new List<string> {userExtra});
        }

        public void Remove(string userId)
        {
            for (var i = 0; i < users.Count; i++)
            {
                if (users[i].userId.Equals(userId))
                {
                    users.RemoveAt(i);
                    dropdown.options.RemoveAt(i);
                    return;
                }
            }
        }

        public RtcUser? Get(int index)
        {
            if (index < 0 || index > users.Count)
            {
                return null;
            }

            return users[index];
        }

        public string getUserName(string userId)
        {
            for (var i = 0; i < users.Count; i++)
            {
                if (userId.Equals(users[i].userId))
                {
                    return users[i].userExtra;
                }
            }

            return "NoName";
        }
    }

    public class RtcMessage : MonoBehaviour
    {
        private string userId;
        private string roomId = "1";
        private RtcUserList _rtcUsers;

        private void Start()
        {
            try
            {
                CoreService.Initialize();
            }
            catch (Exception e)
            {
                Debug.Log("Initialize Platform SDK failed");
                return;
            }

            Debug.Log("Initialize Successfully");
            var initResult = RtcService.InitRtcEngine();
            if (initResult != RtcEngineInitResult.Success)
            {
                Debug.Log($"Initialize RTC engine failed : {initResult}");
                return;
            }

            Debug.Log("Initialize RTC Engine Successfully!");
            var ttl = 3600;
            UserService.GetLoggedInUser().OnComplete(userResp =>
            {
                if (userResp.IsError)
                {
                    Debug.Log($"GetLoggedInUser failed: code={userResp.Error.Code} errorMessage={userResp.Error.Message}");
                    return;
                }

                Debug.Log("GetLoggedInUser successfully");
                userId = userResp.Data.ID;
                Debug.Log($"Getting RTC token roomId={roomId} userId={userId} ttl={ttl}");
                RtcUtil.JoinRoom(roomId, userId, userResp.Data.DisplayName, ttl);
            });
            Debug.Log("Setting RTC Callbacks");
            RtcUtil.SetRtcWarnErrorHandler();
            RtcService.SetOnJoinRoomResultCallback(m => { Debug.Log($"JoinRoom result={JsonConvert.SerializeObject(m.Data)}"); });
            RtcService.SetOnUserJoinRoomResultCallback(m =>
            {
                Debug.Log($"User join room={JsonConvert.SerializeObject(m.Data)}");
                var userExtra = m.Data.UserExtra;
                if (string.IsNullOrEmpty(userExtra))
                {
                    userExtra = m.Data.UserId;
                }

                _rtcUsers.Add(m.Data.UserId, userExtra);
            });
            RtcService.SetOnUserLeaveRoomResultCallback(m =>
            {
                Debug.Log($"User Leave Room {JsonConvert.SerializeObject(m.Data)}");
                _rtcUsers.Remove(m.Data.UserId);
            });
            //MessageSendResult
            RtcService.SetOnRoomMessageSendResult(m => { Debug.Log($"RoomMessageSendResult:{JsonConvert.SerializeObject(m.Data)}"); });

            RtcService.SetOnUserMessageSendResult(m => { Debug.Log($"UserMessageSendResult:{JsonConvert.SerializeObject(m.Data)}"); });

            //MessageReceived
            RtcService.SetOnRoomMessageReceived(m => { Debug.Log($"RoomMessageReceived:{JsonConvert.SerializeObject(m.Data)}"); });

            RtcService.SetOnUserMessageReceived(m => { Debug.Log($"UserMessageReceived:{JsonConvert.SerializeObject(m.Data)}"); });
            RtcService.SetOnRoomBinaryMessageReceived(m =>
            {
                var s = Encoding.UTF8.GetString(m.Data.Data);
                var userId = m.Data.UserId;
                var userName = _rtcUsers.getUserName(userId);
                Debug.Log($"RoomBinaryMessageReceived:{userName}:{s}");
            });
            RtcService.SetOnUserBinaryMessageReceived(m =>
            {
                var s = Encoding.UTF8.GetString(m.Data.Data);
                var userId = m.Data.UserId;
                var userName = _rtcUsers.getUserName(userId);
                Debug.Log($"UserBinaryMessageReceived:{userName}:{s}");
            });

            //MessageReceived end
            RtcService.SetOnJoinRoomResultCallback(m =>
            {
                if (m.IsError)
                {
                    Debug.Log($"JoinRoom failed :{JsonConvert.SerializeObject(m.Error)}");
                    return;
                }

                if (m.Data.ErrorCode != 0)
                {
                    Debug.Log($"JoinRoom error :{JsonConvert.SerializeObject(m.Data)}");
                    return;
                }

                RtcService.StartAudioCapture();
                RtcService.PublishRoom(roomId);
            });
            RtcService.SetOnStreamSyncInfoReceived(m =>
            {
                var x = m.Data;
                var userId = x.StreamKey.UserId;
                var username = _rtcUsers.getUserName(userId);
                Debug.Log($"StreamSyncInfo:userName={username} message={Encoding.UTF8.GetString(x.Data)}");
            });
            Debug.Log("BindEventToUi");
            var buttonSendRoom = GameObject.Find("ButtonSendRoom").GetComponent<Button>();
            var buttonSendUser = GameObject.Find("ButtonSendUser").GetComponent<Button>();
            var buttonSendSyncInfo = GameObject.Find("ButtonSendSyncInfo").GetComponent<Button>();
            var input = GameObject.Find("InputFieldSend").GetComponent<InputField>();
            var userSelect = GameObject.Find("DropdownUserSelection").GetComponent<Dropdown>();
            var toggleBinaryMessage = GameObject.Find("ToggleBinaryMessage").GetComponent<Toggle>();
            userSelect.ClearOptions();
            _rtcUsers = new RtcUserList(userSelect);
            buttonSendRoom.onClick.AddListener(() =>
            {
                long messageId;
                if (toggleBinaryMessage.isOn)
                {
                    messageId = RtcService.SendRoomBinaryMessage(roomId, Encoding.UTF8.GetBytes(input.text));
                }
                else
                {
                    messageId = RtcService.SendRoomMessage(roomId, input.text);
                }

                Debug.Log($"SendRoomMessage :IsBinary={toggleBinaryMessage.isOn} messageId={messageId} text={input.text}");
                input.text = "";
            });
            buttonSendUser.onClick.AddListener(() =>
            {
                var toUser = _rtcUsers.Get(userSelect.value);
                if (!toUser.HasValue)
                {
                    Debug.Log("Cannot find target user");
                    return;
                }

                long messageId;
                if (toggleBinaryMessage.isOn)
                {
                    messageId = RtcService.SendUserBinaryMessage(roomId, toUser.Value.userId, Encoding.UTF8.GetBytes(input.text));
                }
                else
                {
                    messageId = RtcService.SendUserMessage(roomId, toUser.Value.userId, input.text);
                }

                Debug.Log($"SendUserMessage :IsBinary={toggleBinaryMessage.isOn} messageId={messageId} userId={toUser.Value.userId} roomId={roomId} text={input.text}");
                input.text = "";
            });
            buttonSendSyncInfo.onClick.AddListener(() =>
            {
                var res = RtcService.SendStreamSyncInfo(Encoding.UTF8.GetBytes(input.text), 1);
                input.text = "";
                if (res < 0)
                {
                    Debug.Log($"Send StreamSyncInfo failed:{res}");
                    return;
                }

                Debug.Log($"SendStreamSyncInfoSuccessfully:{res}");
            });
        }
    }
}