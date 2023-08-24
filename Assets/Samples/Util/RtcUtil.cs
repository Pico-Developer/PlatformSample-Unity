using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform.Samples
{
    public interface RtcWarnErrorListener
    {
        void OnWarn(int code);
        void OnError(int code);
        void OnRoomWarn(string roomId, int code);
        void OnRoomError(string roomId, int code);
    }

    public class RtcWarnErrorLogHandler : RtcWarnErrorListener
    {
        public void OnWarn(int code)
        {
            Debug.LogWarning($"Rtc warn {code}");
        }

        public void OnError(int code)
        {
            Debug.LogError($"Rtc error {code}");
        }

        public void OnRoomWarn(string roomId, int code)
        {
            Debug.LogWarning($"Rtc Room warn roomId={roomId} code={code}");
        }

        public void OnRoomError(string roomId, int code)
        {
            Debug.LogError($"Rtc Room error roomId={roomId} code={code}");
        }
    }

    public interface RtcJoinLeaveHandler
    {
        void OnJoin(RtcJoinRoomResult msg);
        void OnLeave(RtcLeaveRoomResult msg);
        void OnUserJoin(RtcUserJoinInfo msg);
        void OnUserLeave(RtcUserLeaveInfo msg);
    }

    public class RtcUtil
    {
        public static void SetRtcWarnErrorHandler()
        {
            RtcService.SetOnWarnCallback(msg => { Debug.Log($"OnWarn:{msg.Data}"); });
            RtcService.SetOnErrorCallback(msg => { Debug.Log($"OnError:{msg.Data}"); });
            RtcService.SetOnRoomErrorCallback(msg => { Debug.Log($"OnRoomError :{JsonConvert.SerializeObject(msg.Data)}"); });
            RtcService.SetOnRoomWarnCallback(msg => { Debug.Log($"OnRoomWarn :{JsonConvert.SerializeObject(msg.Data)}"); });
        }

        public static void SetRtcWarnErrorHandler(RtcWarnErrorListener listener)
        {
            RtcService.SetOnWarnCallback(msg => { listener.OnWarn(msg.Data); });
            RtcService.SetOnErrorCallback(msg => { listener.OnError(msg.Data); });
            RtcService.SetOnRoomErrorCallback(msg => { listener.OnRoomError(msg.Data.RoomId, msg.Data.Code); });
            RtcService.SetOnRoomWarnCallback(msg => { listener.OnRoomWarn(msg.Data.RoomId, msg.Data.Code); });
        }

        public static void SetJoinLeaveHandler(RtcJoinLeaveHandler handler)
        {
            RtcService.SetOnJoinRoomResultCallback(msg => handler.OnJoin(msg.Data));
            RtcService.SetOnLeaveRoomResultCallback(msg => handler.OnLeave(msg.Data));
            RtcService.SetOnUserJoinRoomResultCallback(msg => handler.OnUserJoin(msg.Data));
            RtcService.SetOnUserLeaveRoomResultCallback(msg => handler.OnUserLeave(msg.Data));
        }

        public static Dictionary<RtcPrivilege, int> MakePrivilegeMap(int ttl)
        {
            return new Dictionary<RtcPrivilege, int>
            {
                {RtcPrivilege.PublishStream, ttl},
                {RtcPrivilege.SubscribeStream, ttl}
            };
        }

        public static Task<string> GetToken(string roomId, string userId, int ttlInSeconds)
        {
            return RtcService.GetToken(roomId, userId, ttlInSeconds, MakePrivilegeMap(ttlInSeconds));
        }

        public static void JoinRoom(string roomId, string userId, string userExtra, int ttlInSeconds)
        {
            GetToken(roomId, userId, ttlInSeconds).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Debug.Log($"GetToken failed:{JsonConvert.SerializeObject(msg.Error)}");
                    return;
                }

                var token = msg.Data;
                var roomOptions = new RtcRoomOptions();
                roomOptions.SetRoomId(roomId);
                roomOptions.SetUserId(userId);
                roomOptions.SetUserExtra(userExtra);
                roomOptions.SetToken(token);
                var res = RtcService.JoinRoom2(roomOptions, true);
                if (res != 0)
                {
                    Debug.Log($"JoinRoom failed: {res}");
                }
            });
        }

        public static void JoinRoom(string roomId, int ttlInSeconds)
        {
            UserService.GetLoggedInUser().OnComplete(m =>
            {
                if (m.IsError)
                {
                    Debug.Log($"Get User info failed :{JsonConvert.SerializeObject(m.Error)}");
                    return;
                }

                var user = m.Data;
                JoinRoom(roomId, user.ID, user.DisplayName, ttlInSeconds);
            });
        }

        public static void UpdateToken(string roomId, string userId, int ttlInSeconds)
        {
            GetToken(roomId, userId, ttlInSeconds).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Debug.Log($"GetToken failed:{JsonConvert.SerializeObject(msg.Error)}");
                    return;
                }

                var token = msg.Data;
                RtcService.UpdateToken(roomId, token);
            });
        }


        public static RtcAudioSampleRate GetSampleRate(int sampleRate)
        {
            switch (sampleRate)
            {
                case 8000:
                {
                    return RtcAudioSampleRate.F8000;
                }
                case 16000:
                {
                    return RtcAudioSampleRate.F16000;
                }
                case 32000:
                {
                    return RtcAudioSampleRate.F32000;
                }
                case 44100:
                {
                    return RtcAudioSampleRate.F44100;
                }
                case 48000:
                {
                    return RtcAudioSampleRate.F48000;
                }
            }

            throw new Exception("unsupported sample rate");
        }

        public static byte[] GetAudioData(float[] a)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            foreach (var i in a)
            {
                w.Write((short) (i * short.MaxValue));
            }

            return ms.GetBuffer();
        }
    }
}