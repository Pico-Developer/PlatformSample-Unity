using System.Collections.Generic;
using Newtonsoft.Json;
using PICO.Platform.Samples;
using TMPro;
using UnityEngine;

namespace Pico.Platform.Samples
{
    public class SmallRtc : MonoBehaviour
    {
        public TMP_Text textInfo;

        void Start()
        {
            InitUtil.Initialize();
            Debug.Log("Before init RTC Engine");
            {
                var res = RtcService.InitRtcEngine();
                Debug.Log($"After init RTC：{res}");
                if (res != 0)
                {
                    Debug.Log($"Init RTC failed {res}");
                    textInfo.text = $"Init RTC failed : {res}";
                    return;
                }
            }
            var roomId = "1";
            UserService.GetLoggedInUser().OnComplete(userResp =>
            {
                if (userResp.IsError)
                {
                    Debug.Log($"code={userResp.Error.Code};message={userResp.Error.Message}");
                    textInfo.SetText("GetLoggedInUser failed");
                    return;
                }

                var userId = userResp.Data.ID;
                var ttl = 3600;
                var ma = new Dictionary<RtcPrivilege, int>();
                ma[RtcPrivilege.PublishStream] = ttl;
                ma[RtcPrivilege.SubscribeStream] = ttl;
                RtcService.GetToken(roomId, userId, ttl, ma).OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        Debug.Log($"code={msg.Error.Code},message={msg.Error.Message}");
                        textInfo.SetText($"Get Token failed {msg.Error}");
                        return;
                    }

                    var token = msg.Data;
                    var res = RtcService.JoinRoom(roomId, userId, token, RtcRoomProfileType.Communication, true);
                    Debug.Log($"join Room result {res}");
                });
            });
            RtcService.SetOnJoinRoomResultCallback(msg =>
            {
                var res = msg.Data;
                Debug.Log(JsonConvert.SerializeObject(res));
                if (res.ErrorCode != 0)
                {
                    textInfo.text = $"Join room failed {res.ErrorCode}";
                    return;
                }

                RtcService.PublishRoom(res.RoomId);
                RtcService.StartAudioCapture();
                textInfo.text = "You can speak now";
            });
        }
    }
}