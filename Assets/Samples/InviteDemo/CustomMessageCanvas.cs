using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pico.Platform;
using UnityEngine;
using UnityEngine.UI;

namespace PICO.Platform.Samples.Invite
{
    public delegate void SendMessageFunc(string userId, Action<string> showToast);

    public class CustomMessageCanvas : MonoBehaviour
    {
        public Button buttonLaunchInvitePanel;
        public Button buttonSetPresence;
        public static SendMessageFunc currentAction;

        private int buttonCount = 0;


        private void Start()
        {
            currentAction = SendDefault;

            buttonLaunchInvitePanel.onClick.AddListener(() =>
            {
                PresenceService.LaunchInvitePanel().OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        Debug.Log($"send invites failed {JsonConvert.SerializeObject(msg.Error)}");
                        return;
                    }

                    Debug.Log($"send invites successfully");
                });
            });
            buttonSetPresence.onClick.AddListener(setPresence);
        }

        public static void setPresence()
        {
            PresenceService.GetDestinations().OnComplete(destinationResp =>
            {
                if (destinationResp.IsError)
                {
                    Debug.Log($"get destinations failed:error={JsonConvert.SerializeObject(destinationResp.Error)}");
                    return;
                }

                var destinationList = destinationResp.Data;
                if (destinationList.Count == 0)
                {
                    Debug.Log("Destination is empty,please configure destinations in the developer center.");
                    return;
                }

                Debug.Log($"Got destination list successfully : {destinationList.Count}");
                var defaultDestination = destinationList[0];
                PresenceOptions options = new PresenceOptions();
                options.SetIsJoinable(true);
                options.SetDestinationApiName(defaultDestination.ApiName);
                var extraInfo = new JObject();
                extraInfo["one"] = "two";
                extraInfo["three"] = 4;
                options.SetExtra(JsonConvert.SerializeObject(extraInfo));
                Debug.Log($"setting presence :DestinationApiName={defaultDestination.ApiName}");
                PresenceService.Set(options).OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        Debug.Log($"Presence设置失败：${JsonConvert.SerializeObject(msg.Error)}");
                        return;
                    }

                    Debug.Log($"set presence successfully.");
                });
            });
        }

        public static void Send(string userId, Action<string> showToast)
        {
            if (currentAction == null)
            {
                Debug.LogError("Current Action is null");
                return;
            }

            currentAction(userId, showToast);
        }

        void SendDefault(string userId, Action<string> showToast)
        {
            //什么都不带
            PresenceService.SendInvites(new[] {userId}).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    showToast("Send invites failed");
                    Debug.Log($"Send invites failed {JsonConvert.SerializeObject(msg.Error)}");
                    return;
                }

                showToast("send invites successfully");
                Debug.Log($"SendInvites successfully :{JsonConvert.SerializeObject(msg.Data)}");
            });
        }

        public void Load()
        {
            setPresence();
        }
    }
}