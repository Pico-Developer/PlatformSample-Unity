using Newtonsoft.Json;
using Pico.Platform;
using Samples.RtcDemo;
using Samples.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Samples.RtcUserStream
{
    public class RtcStream : MonoBehaviour
    {
        private string roomId = "1";
        private string remoteUserId;

        private void Start()
        {
            CoreService.Initialize();
            if (RtcService.InitRtcEngine() != RtcEngineInitResult.Success)
            {
                Debug.Log("Initialize RTC failed");
                return;
            }

            //User publish and cancel publish
            RtcService.SetOnUserPublishStream(m => { Debug.Log($"UserPublishStream:{JsonConvert.SerializeObject(m.Data)}"); });
            RtcService.SetOnUserUnPublishStream(m => { Debug.Log($"UserUnPublishStream:{JsonConvert.SerializeObject(m.Data)}"); });
            //Others join and leave
            RtcService.SetOnUserJoinRoomResultCallback(m =>
            {
                if (string.IsNullOrEmpty(remoteUserId))
                {
                    remoteUserId = m.Data.UserId;
                    Debug.Log($"RemoteUserId={remoteUserId}");
                }

                Debug.Log($"UserJoinRoom:{JsonConvert.SerializeObject(m.Data)}");
            });
            RtcService.SetOnUserLeaveRoomResultCallback(m => { Debug.Log($"UserLeaveRoom:{JsonConvert.SerializeObject(m.Data)}"); });
            //I join and leave
            RtcService.SetOnJoinRoomResultCallback(m =>
            {
                Debug.Log($"JoinRoomResult:{JsonConvert.SerializeObject(m.Data)}");
                RtcService.StartAudioCapture();
                RtcService.PublishRoom(roomId);
            });
            RtcService.SetOnLeaveRoomResultCallback(m => { Debug.Log($"LeaveRoomResult:{JsonConvert.SerializeObject(m.Data)}"); });

            RtcUtil.SetRtcWarnErrorHandler();
            RtcUtil.JoinRoom(roomId, 3600);
            var togglePublish = GameObject.Find("TogglePublish").GetComponent<Toggle>();
            togglePublish.onValueChanged.AddListener(v =>
            {
                if (v)
                {
                    RtcService.UnPublishRoom(roomId);
                }
                else
                {
                    RtcService.PublishRoom(roomId);
                }
            });
            var toggleSubscribe = GameObject.Find("ToggleSubscribe").GetComponent<Toggle>();
            toggleSubscribe.onValueChanged.AddListener(v =>
            {
                if (v)
                {
                    RtcService.RoomResumeAllSubscribedStream(roomId);
                }
                else
                {
                    RtcService.RoomPauseAllSubscribedStream(roomId);
                }
            });
            var toggleSubscribeUser = GameObject.Find("ToggleSubscribeUser").GetComponent<Toggle>();
            toggleSubscribeUser.onValueChanged.AddListener(v =>
            {
                if (string.IsNullOrEmpty(remoteUserId))
                {
                    Debug.Log($"RemoteUserId is empty");
                    return;
                }

                if (v)
                {
                    RtcService.RoomSubscribeStream(roomId, remoteUserId);
                }
                else
                {
                    RtcService.RoomUnSubscribeStream(roomId, remoteUserId);
                }
            });
            var sliderRemoteVolume = GameObject.Find("SliderRemoteVolume").GetComponent<Slider>();
            sliderRemoteVolume.OnEventTriggerEvent(EventTriggerType.PointerUp, m =>
            {
                if (string.IsNullOrEmpty(remoteUserId))
                {
                    Debug.Log($"Remote user is empty");
                    return;
                }

                var v = sliderRemoteVolume.value;
                Debug.Log($"Set RemoteUser volume:remoteUserId={remoteUserId} volume={v}");
                RtcService.RoomSetRemoteAudioPlaybackVolume(roomId, remoteUserId, (int) v);
            });
        }
    }
}