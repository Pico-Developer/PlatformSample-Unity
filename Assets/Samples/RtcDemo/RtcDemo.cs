using System;
using System.Collections.Generic;
using System.Text;
using Pico.Platform.Models;
using Samples.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pico.Platform.Samples.RtcDemo
{
    public class RtcDemo : MonoBehaviour
    {
        public GameObject roomPrefab;
        public Text outputText;
        public InputField inputFieldRoomId;
        public InputField inputFieldUserId;
        public GameObject roomListPanel;
        public Dropdown dropdownRoomProfile;
        public Button buttonEnterRoom;
        public Toggle toggleEarMonitorMode;
        public Toggle toggleCapture;
        public Slider sliderRecordingVolume;
        public Slider sliderPlaybackVolume;
        public Slider sliderEarMonitorVolume;
        public Dropdown dropDownScenarioType;
        public TMP_Text textLocalAudioReport;
        public TMP_Text textRemoteAudioReport;

        private List<Room> roomList = new List<Room>();

        bool CheckRoomId()
        {
            if (String.IsNullOrWhiteSpace(inputFieldRoomId.text))
            {
                Log($"Please input Room Id");
                return false;
            }

            return true;
        }

        bool CheckUserId()
        {
            if (String.IsNullOrWhiteSpace(inputFieldUserId.text))
            {
                Log($"Please input User Id");
                return false;
            }

            return true;
        }

        void initRtc()
        {
            var res = RtcService.InitRtcEngine();
            if (res != RtcEngineInitResult.Success)
            {
                Log($"Init RTC Engine Failed{res}");
                throw new UnityException($"Init RTC Engine Failed:{res}");
            }

            RtcService.EnableAudioPropertiesReport(2000);
        }

        private void Start()
        {
            try
            {
                if (CoreService.Initialized)
                {
                    initRtc();
                }
                else
                {
                    InitUtil.AsyncInitialize().OnComplete(m =>
                    {
                        if (m.IsError)
                        {
                            Log($"Init PlatformSdk failed:code={m.Error.Code},message={m.Error.Message}");
                            return;
                        }

                        if (m.Data == PlatformInitializeResult.Success || m.Data == PlatformInitializeResult.AlreadyInitialized)
                        {
                            Log($"Init PlatformSdk successfully");
                            initRtc();
                        }
                        else
                        {
                            Log($"Init PlatformSdk failed:{m.Data}");
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log($"Init Platform SDK failed {e}");
                throw;
            }

            buttonEnterRoom.onClick.AddListener(OnClickJoinRoom);

            #region endregionEngine Config List

            toggleCapture.onValueChanged.AddListener((v) =>
            {
                if (v)
                {
                    Log($"Before StartAudioCapture");
                    RtcService.StartAudioCapture();
                    Log($"StartAudioCapture Done");
                }
                else
                {
                    Log($"Before StopAudioCapture");
                    RtcService.StopAudioCapture();
                    Log($"StopAudioCapture Done");
                }
            });
            toggleEarMonitorMode.onValueChanged.AddListener(earMonitorMode =>
            {
                Log($"EarMonitorMode {earMonitorMode}");
                if (earMonitorMode)
                {
                    RtcService.SetEarMonitorMode(RtcEarMonitorMode.On);
                }
                else
                {
                    RtcService.SetEarMonitorMode(RtcEarMonitorMode.Off);
                }

                Log($"EarMonitorMode {earMonitorMode} Done");
            });
            sliderPlaybackVolume.OnEventTriggerEvent(EventTriggerType.PointerUp, x =>
            {
                var v = sliderPlaybackVolume.value;
                Log($"SetPlaybackAudio{v}");
                RtcService.SetPlaybackVolume((int) v);
            });
            sliderRecordingVolume.OnEventTriggerEvent(EventTriggerType.PointerUp, e =>
            {
                var v = sliderRecordingVolume.value;
                Log($"SetRecordingVolume{v}");
                RtcService.SetCaptureVolume((int) v);
            });
            sliderEarMonitorVolume.OnEventTriggerEvent(EventTriggerType.PointerUp, e =>
            {
                var v = sliderEarMonitorVolume.value;
                Log($"SetEarMonitorVolume {v}");
                RtcService.SetEarMonitorVolume((int) v);
            });
            dropDownScenarioType.onValueChanged.AddListener(v =>
            {
                Log($"setting scenarioType {v}");
                RtcService.SetAudioScenario((RtcAudioScenarioType) v);
                Log($"setting scenarioType {v} done");
            });

            #endregion

            #region Callbacks

            RtcService.SetOnJoinRoomResultCallback(OnJoinRoom);
            RtcService.SetOnLeaveRoomResultCallback(OnLeaveRoom);
            RtcService.SetOnUserLeaveRoomResultCallback(OnUserLeaveRoom);
            RtcService.SetOnUserJoinRoomResultCallback(OnUserJoinRoom);
            RtcService.SetOnRoomStatsCallback(OnRoomStats);
            RtcService.SetOnWarnCallback(OnWarn);
            RtcService.SetOnErrorCallback(OnError);
            RtcService.SetOnRoomWarnCallback(OnRoomWarn);
            RtcService.SetOnRoomErrorCallback(OnRoomError);
            RtcService.SetOnConnectionStateChangeCallback(OnConnectionStateChange);
            // RtcService.SetOnUserMuteAudio(OnUserMuteAudio);
            RtcService.SetOnUserPublishStream(OnUserPublishStream);
            RtcService.SetOnUserUnPublishStream(OnUserUnPublishStream);
            RtcService.SetOnUserStartAudioCapture(OnUserStartAudioCapture);
            RtcService.SetOnUserStopAudioCapture(OnUserStopAudioCapture);
            RtcService.SetOnLocalAudioPropertiesReport(OnLocalAudioPropertiesReport);
            RtcService.SetOnRemoteAudioPropertiesReport(OnRemoteAudioPropertiesReport);

            #endregion
        }

        private void OnUserUnPublishStream(Message<RtcUserUnPublishInfo> message)
        {
            var info = message.Data;
            Log($"RoomId={info.RoomId},UserId={info.UserId},MediaType={info.MediaStreamType} {info.Reason}");
        }

        private void OnUserPublishStream(Message<RtcUserPublishInfo> message)
        {
            var info = message.Data;
            Log($"RoomId={info.RoomId} UserId{info.UserId} MediaStreamType={info.MediaStreamType}");
        }

        private void OnRemoteAudioPropertiesReport(Message<RtcRemoteAudioPropertiesReport> message)
        {
            var d = message.Data;
            StringBuilder builder = new StringBuilder($"totalVolume={d.TotalRemoteVolume} ");
            foreach (var usr in d.AudioPropertiesInfos)
            {
                if (usr.AudioPropertiesInfo.Volume > 5)
                {
                    builder.Append($"user={usr.StreamKey.UserId} roomId={usr.StreamKey.RoomId} volume={usr.AudioPropertiesInfo.Volume} ");
                }
            }

            var report = builder.ToString();
            textRemoteAudioReport.text = report;
            Debug.Log($@"[RemoteAudioPropertiesReport]length={d.AudioPropertiesInfos.Length} report={report}");
        }

        private void OnLocalAudioPropertiesReport(Message<RtcLocalAudioPropertiesReport> message)
        {
            var d = message.Data;
            StringBuilder builder = new StringBuilder();
            foreach (var i in d.AudioPropertiesInfos)
            {
                builder.Append(i.AudioPropertyInfo.Volume).Append(",");
            }

            textLocalAudioReport.text = builder.ToString();
            Debug.Log($@"[LocalAudioPropertiesReport] {d.AudioPropertiesInfos.Length}LocalVolume={builder}");
        }

        private void OnUserStopAudioCapture(Message<string> message)
        {
            var d = message.Data;
            Log($"[UserStopAudioCapture]UserId={d}");
        }

        private void OnUserStartAudioCapture(Message<string> message)
        {
            var d = message.Data;
            Log($"[UserStartAudioCapture]UserId={d}");
        }

        private void OnUserMuteAudio(Message<RtcMuteInfo> message)
        {
            var d = message.Data;
            Log($"[UserMuteAudio]userId={d.UserId} muteState={d.MuteState}");
        }

        private void OnClickJoinRoom()
        {
            if (!(CheckRoomId() && CheckUserId()))
            {
                return;
            }

            var roomProfile = (RtcRoomProfileType) dropdownRoomProfile.value;
            var roomId = inputFieldRoomId.text;
            var userId = inputFieldUserId.text;
            Log($"userId={userId} roomId={roomId} scenarioType={roomProfile}");
            var privilege = new Dictionary<RtcPrivilege, int>();
            privilege.Add(RtcPrivilege.PublishStream, 3600 * 2);
            privilege.Add(RtcPrivilege.SubscribeStream, 3600 * 2);
            RtcService.GetToken(roomId, userId, 3600 * 2, privilege).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Get rtc token failed: code={msg.GetError().Code} message={msg.GetError().Message}");
                    return;
                }

                var token = msg.Data;
                Log($"Got RTC Token:{token}");
                int result = RtcService.JoinRoom(roomId, userId, token, roomProfile, true);
                Log($"Join Room Result={result} RoomId={inputFieldRoomId.text}");
            });
        }

        private void OnConnectionStateChange(Message<RtcConnectionState> message)
        {
            Log($"[ConnectionStateChange] {message.Data}");
        }

        private void OnRoomError(Message<RtcRoomError> message)
        {
            var e = message.Data;
            Log($"[RtcRoomError]RoomId={e.RoomId} Code={e.Code}");
        }

        private void OnRoomWarn(Message<RtcRoomWarn> message)
        {
            var e = message.Data;
            Log($"[RtcRoomWarn]RoomId={e.RoomId} Code={e.Code}");
        }

        private void OnError(Message<int> message)
        {
            Log($"[RtcError] {message.Data}");
        }

        private void OnWarn(Message<int> message)
        {
            Log($"[RtcWarn] {message.Data}");
        }

        private void OnJoinRoom(Message<RtcJoinRoomResult> msg)
        {
            if (msg.IsError)
            {
                var err = msg.GetError();
                var joinRoomResult = msg.Data;
                var roomId = "";
                if (joinRoomResult != null)
                {
                    roomId = joinRoomResult.RoomId;
                }

                Log($"[JoinRoomError]code={err.Code} message={err.Message} roomId={roomId}");
                return;
            }

            var rtcJoinRoomResult = msg.Data;
            if (rtcJoinRoomResult.ErrorCode != 0)
            {
                Log($"[JoinRoomError]code={rtcJoinRoomResult.ErrorCode} RoomId={rtcJoinRoomResult.RoomId} UserId={rtcJoinRoomResult.UserId}");
                return;
            }

            Log("Before Join Room");
            Log($"[JoinRoomOk] Elapsed:{rtcJoinRoomResult.Elapsed} JoinType:{rtcJoinRoomResult.JoinType} RoomId:{rtcJoinRoomResult.RoomId} UserId:{rtcJoinRoomResult.UserId}");
            var room = Instantiate(roomPrefab, roomListPanel.transform).GetComponent<Room>();
            room.id = rtcJoinRoomResult.RoomId;
            room.textRoomId.text = room.id;
            room.leaveRoomButton.onClick.AddListener(() =>
            {
                Log($"Doing leave room:{room.id}");
                int result = RtcService.LeaveRoom(room.id);
                // RemoveRoomFromUi(room.id);//在OnLeaveRoom回调中从UI上移除房间
                Log($"[LeaveRoomResult]={result} RoomId={room.id}");
            });
            room.toggleRemoteAudio.onValueChanged.AddListener(v =>
            {
                if (v)
                {
                    Log($"RoomResumeAllSubscribedStream {room.id}");
                    RtcService.RoomResumeAllSubscribedStream(room.id);
                    Log($"RoomResumeAllSubscribedStream {room.id} done");
                }
                else
                {
                    Log($"RoomPauseAllSubscribedStream {room.id}");
                    RtcService.RoomPauseAllSubscribedStream(room.id);
                    Log($"RoomPauseAllSubscribedStream {room.id} done");
                }
            });
            room.publishToggle.onValueChanged.AddListener(v =>
            {
                if (v)
                {
                    Log($"Publish {room.id}");
                    RtcService.PublishRoom(room.id);
                    Log($"Publish {room.id} done");
                }
                else
                {
                    Log($"UnPublish {room.id}");
                    RtcService.UnPublishRoom(room.id);
                    Log($"UnPublish {room.id} done");
                }
            });
            room.destroyRoomButton.onClick.AddListener(() =>
            {
                Log($"DestroyRoom {room.id}");
                RtcService.DestroyRoom(room.id);
                RemoveRoomFromUi(room.id);
                Log($"DestroyRoom {room.id} done");
            });
            roomList.Add(room);
        }

        private void OnLeaveRoom(Message<RtcLeaveRoomResult> msg)
        {
            if (msg.IsError)
            {
                var err = msg.GetError();
                Log($"[LeaveRoomResult]code={err.Code} message={err.Message}");
                return;
            }

            var res = msg.Data;
            Log($"[LeaveRoomOk]RoomId={res.RoomId}");
            RemoveRoomFromUi(res.RoomId);
        }

        void RemoveRoomFromUi(string roomId)
        {
            Room it = null;
            foreach (var r in roomList)
            {
                if (r.id.Equals(roomId))
                {
                    it = r;
                    break;
                }
            }

            if (it != null)
            {
                Log($"Remove Room {it.id} from ui");
                roomList.Remove(it);
                Destroy(it.gameObject);
            }
            else
            {
                Log($"Cannot find room in ui:{roomId}");
            }
        }

        private void OnUserLeaveRoom(Message<RtcUserLeaveInfo> msg)
        {
            var res = msg.Data;
            Log($"[UserLeave]User[{res.UserId}] left room[{res.RoomId}],offline reason：{res.OfflineReason}");
        }

        private void OnUserJoinRoom(Message<RtcUserJoinInfo> msg)
        {
            var res = msg.Data;
            Log($"[UserJoin]user={res.UserId} join room={res.RoomId},UserExtra={res.UserExtra},TimeElapsed{res.Elapsed}");
        }

        private void OnRoomStats(Message<RtcRoomStats> msg)
        {
            var res = msg.Data;
            foreach (var r in roomList)
            {
                if (r.id == res.RoomId)
                {
                    r.textRoomStats.text = $"UserCount={res.UserCount} Duration={res.TotalDuration}";
                }
            }

            Debug.Log($"[RoomStats]RoomId={res.RoomId} UserCount={res.UserCount} Duration={res.TotalDuration}");
        }

        void Log(string s)
        {
            Debug.Log($"[RtcDemo]{s}");
            if (outputText != null)
            {
                outputText.text = s + "\n" + outputText.text;
                if (outputText.text.Length > 1000)
                {
                    outputText.text = outputText.text.Substring(0, 1000);
                }
            }
        }
    }
}