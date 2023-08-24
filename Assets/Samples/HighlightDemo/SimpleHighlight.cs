using Newtonsoft.Json;
using Pico.Platform;
using Pico.Platform.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

namespace PrivateSamples.DemoTest.Social.Highlight
{
    public class SimpleHighlight : MonoBehaviour
    {
        public Button buttonStartSession;
        public Button buttonCapture;
        public Button buttonStartRecord;
        public Button buttonStopRecord;
        public Button buttonListMedia;
        public ListMediaPanel listMediaPanel;
        public PanelInfo panelInfo;
        public TMP_Text textCurrentSession;

        private string currentSessionId = "";

        private void Start()
        {
            listMediaPanel.gameObject.SetActive(false);
            panelInfo.gameObject.SetActive(false);

            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }

            CoreService.AsyncInitialize().OnComplete(res =>
            {
                if (res.IsError)
                {
                    Debug.LogError($"Initialize failed {res.Error}");
                    panelInfo.Error("Initilialize failed");
                    return;
                }

                if (res.Data != PlatformInitializeResult.Success)
                {
                    Debug.LogError($"Initialize failed {res.Data}");
                    panelInfo.Error($"Initialize failed {res.Data}");
                    return;
                }

                UserService.RequestUserPermissions(Permissions.RecordHighlight).OnComplete(m =>
                {
                    if (m.IsError)
                    {
                        Debug.Log($"Request permission failed {m.Error}");
                        panelInfo.Error($"Request Permission failed");
                        return;
                    }

                    panelInfo.Info("Request Permission successfully");
                    Debug.Log($"Request permission successfully");
                });
            });
            buttonStartSession.onClick.AddListener(() =>
            {
                HighlightService.StartSession().OnComplete(m =>
                {
                    if (m.IsError)
                    {
                        Debug.LogError($"start session failed {m.Error}");
                        panelInfo.Error("StartSession failed");
                        return;
                    }

                    currentSessionId = m.Data;
                    textCurrentSession.SetText(currentSessionId);
                    panelInfo.Info("StartSession successfully");
                    Debug.Log($"start session successfully: sessionId={m.Data}");
                });
            });
            buttonCapture.onClick.AddListener(() =>
            {
                HighlightService.CaptureScreen().OnComplete(m =>
                {
                    if (m.IsError)
                    {
                        Debug.Log($"CaptureScreen failed :{m.Error}");
                        panelInfo.Error($"CaptureScreen failed");
                        return;
                    }

                    panelInfo.Info($"CaptureScreen successfully");
                    Debug.Log($"CaptureScreen result:{JsonConvert.SerializeObject(m.Data)}");
                });
            });
            buttonListMedia.onClick.AddListener(() =>
            {
                Debug.Log($"ListMedia of session={currentSessionId}");
                var sessionId = currentSessionId;
                HighlightService.ListMedia(currentSessionId).OnComplete(m =>
                {
                    if (m.IsError)
                    {
                        Debug.Log($"ListMedia failed for sessionId={currentSessionId}:{m.Error}");
                        panelInfo.Error($"ListMedia failed");
                        return;
                    }

                    Debug.Log($"Get media info successfully");
                    var mediaData = m.Data;
                    listMediaPanel.Show(sessionId, mediaData);
                });
            });
            buttonStartRecord.onClick.AddListener(() =>
            {
                Debug.Log("startRecord");
                HighlightService.StartRecord().OnComplete(m =>
                {
                    if (m.IsError)
                    {
                        Debug.Log($"StartRecord failed {m.Error}");
                        panelInfo.Error($"StartRecord failed");
                        return;
                    }

                    Debug.Log("StartRecord successful.Now is recording.");
                });
            });
            buttonStopRecord.onClick.AddListener(() =>
            {
                Debug.Log($"stopRecord");
                HighlightService.StopRecord().OnComplete(OnRecordStop);
            });
            HighlightService.SetOnRecordStopHandler(OnRecordStop);
        }

        void OnRecordStop(Message<RecordInfo> m)
        {
            Debug.Log("Record stop");
            if (m.IsError)
            {
                Debug.Log($"Record failed  {m.Error}");
                panelInfo.Error("Record failed");
                return;
            }

            panelInfo.Info($"Record done");
            Debug.Log($"Record done:{JsonConvert.SerializeObject(m.Data)}");
        }
    }
}