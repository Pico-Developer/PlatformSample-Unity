using System;
using Newtonsoft.Json;
using Pico.Platform;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.RtcTokenWillExpire
{
    public class TokenExpire : MonoBehaviour
    {
        string userId;
        string roomId = "1";
        private Toggle toggleAutoUpdate;
        private Button buttonJoinRoom;
        private Text textTime;

        private void Start()
        {
            CoreService.Initialize();
            buttonJoinRoom = GameObject.Find("ButtonJoinRoom").GetComponent<Button>();
            toggleAutoUpdate = GameObject.Find("ToggleAutoUpdateToken").GetComponent<Toggle>();
            textTime = GameObject.Find("TextTime").GetComponent<Text>();

            var rtcInitializeResult = RtcService.InitRtcEngine();
            if (rtcInitializeResult != RtcEngineInitResult.Success)
            {
                Debug.Log("Initialize RTC engine failed");
                return;
            }

            RtcUtil.SetRtcWarnErrorHandler();

            UserService.GetLoggedInUser().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Debug.Log($"Get user failed:code={msg.Error.Code} message={msg.Error.Message}");
                    return;
                }

                var user = msg.Data;
                Debug.Log($"user.id={user.ID}");
                this.userId = user.ID;
            });
            RtcService.SetOnJoinRoomResultCallback(msg =>
            {
                if (msg.IsError)
                {
                    Debug.Log($"JoinRoomResultCallback error:code={msg.Error.Code} message={msg.Error.Message}");
                    return;
                }

                var joinRoomResult = msg.Data;
                if (joinRoomResult.ErrorCode != 0)
                {
                    Debug.Log($"JoinRoomFailed");
                }
                else
                {
                    Debug.Log("JoinRoom Successfully");
                }

                Debug.Log($"joinRoomResult={JsonConvert.SerializeObject(joinRoomResult)}");
            });
            RtcService.SetOnTokenWillExpire(msg =>
            {
                if (msg.IsError)
                {
                    Debug.Log($"OnTokenWillExpire error:code={msg.Error.Code} message={msg.Error.Message}");
                    return;
                }

                var room = msg.Data;
                Debug.Log($"TokenWillExpire RoomId={room}");
                Debug.Log($"ToggleAutoUpdate={toggleAutoUpdate.isOn}");
                if (toggleAutoUpdate.isOn)
                {
                    Debug.Log("Now will update token");
                    this.updateToken();
                }
            });
            RtcService.SetOnLeaveRoomResultCallback(msg =>
            {
                if (msg.IsError)
                {
                    Debug.Log($"LeaveRoomResult:code={msg.Error.Code} message={msg.Error.Message}");
                    return;
                }

                var res = msg.Data;
                Debug.Log($"LeaveRoomResult={res.RoomId}");
            });

            buttonJoinRoom.onClick.AddListener(() => { joinRoom(); });
        }

        void updateToken()
        {
            var ttl = 60;
            Debug.Log($"Getting Token:roomId={roomId} userId={userId} ttl={ttl}");
            RtcUtil.GetToken("1", userId, ttl).OnComplete(tokenMessage =>
            {
                if (tokenMessage.IsError)
                {
                    Debug.LogError($"GetToken failed :code={tokenMessage.Error.Code} message={tokenMessage.Error.Message}");
                    return;
                }

                Debug.Log($"Now is updating token roomId={roomId} token={tokenMessage.Data}");
                RtcService.UpdateToken(roomId, tokenMessage.Data);
            });
        }

        void joinRoom()
        {
            if (this.userId == null)
            {
                Debug.Log("UserId为空");
                return;
            }

            var ttl = 60;
            Debug.Log($"Getting Token:roomId={roomId} userId={userId} ttl={ttl}");
            RtcUtil.GetToken(roomId, userId, ttl).OnComplete(tokenMessage =>
            {
                if (tokenMessage.IsError)
                {
                    Debug.LogError($"GetToken failed :code={tokenMessage.Error.Code} message={tokenMessage.Error.Message}");
                    return;
                }

                var token = tokenMessage.Data;
                Debug.Log($"token={token}");
                var joinResult = RtcService.JoinRoomWithRetry(roomId, userId, token, RtcRoomProfileType.Communication, true);
                if (joinResult != 0)
                {
                    Debug.Log($"JoinRoom failed roomId={roomId} userId={userId} {joinResult}");
                    return;
                }

                Debug.Log($"JoinRoom roomId={roomId} successfully. ");
            });
        }

        private void Update()
        {
            if (Time.frameCount % 10 == 0)
            {
                textTime.text = DateTime.Now.ToString();
            }
        }
    }
}