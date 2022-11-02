using System.Collections.Generic;
using Newtonsoft.Json;
using Pico.Platform;
using UnityEngine;

namespace Pico.Platform.Samples
{
    public class SmallRtc : MonoBehaviour
    {
        void Start()
        {
            print("开始初始化");
            CoreService.Initialize();
            print("初始化成功");
            print("开始初始化RTC Engine");
            {
                var res = RtcService.InitRtcEngine();
                print($"初始化RTC：{res}");
            }
            var roomId = "1";
            UserService.GetLoggedInUser().OnComplete(userResp =>
            {
                if (userResp.IsError)
                {
                    print($"code={userResp.Error.Code};message={userResp.Error.Message}");
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
                        print($"code={msg.Error.Code},message={msg.Error.Message}");
                        return;
                    }

                    var token = msg.Data;
                    var res = RtcService.JoinRoom(roomId, userId, token, RtcRoomProfileType.Communication, true);
                    print($"join Room result {res}");
                });
            });
            RtcService.SetOnJoinRoomResultCallback(msg =>
            {
                if (msg.IsError)
                {
                    print($"加入房间错误:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                var res = msg.Data;
                print(JsonConvert.SerializeObject(res));
                RtcService.PublishRoom(res.RoomId);
                RtcService.StartAudioCapture();
            });
        }
    }
}