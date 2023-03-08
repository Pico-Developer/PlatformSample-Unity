using System;
using Newtonsoft.Json;
using Pico.Platform;
using Pico.Platform.Models;
using Pico.Platform.Samples;
using Samples.Util;
using UnityEngine;
using UnityEngine.UI;

namespace PICO.Platform.Samples.Invite
{
    enum MainStatus
    {
        error,
        main,
        loading,
    }

    public class MainScene : MonoBehaviour
    {
        public AllFriends allFriends;
        public GameObject noNetworkCanvas;

        public GameObject loadingCanvas;

        public GameObject mainCanvas;
        public CustomMessageCanvas customMessageCanvas;
        public LaunchDetailsCanvas launchDetailCanvas;
        private MainStatus status = MainStatus.main;

        public Button refresh;
        private User currentUser;

        public Text currentUserNickName;

        public RawImage currentUserHeadImage;

        // Start is called before the first frame update
        void Start()
        {
            this.Init();
            refresh.onClick.AddListener(() => { this.Init(); });
        }

        void Init()
        {
            try
            {
                this.setStatus(MainStatus.loading);
                InitUtil.Initialize();
                this.setStatus(MainStatus.main);
                var launchDetails = ApplicationService.GetLaunchDetails();
                launchDetailCanvas.SetLaunchDetail(launchDetails);
                Debug.Log($"LaunchDetails {JsonConvert.SerializeObject(launchDetails)}");

                ApplicationService.SetLaunchIntentChangedCallback(onLaunchIntentChanged);
                PresenceService.SetJoinIntentReceivedNotificationCallback(onJoinIntentChanged);
                UserService.GetLoggedInUser().OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        Debug.Log($"GetLoggedInUser failed {JsonConvert.SerializeObject(msg.Error)}");
                        this.setStatus(MainStatus.error);
                        return;
                    }

                    {
                        currentUser = msg.Data;
                        StartCoroutine(NetworkUtils.BindImage(currentUser.ImageUrl, currentUserHeadImage));
                        currentUserNickName.text = currentUser.DisplayName;
                    }
                    Debug.Log($"GetLoggedInUser successfully {JsonConvert.SerializeObject(msg.Data)}");
                    allFriends.Load();
                    customMessageCanvas.Load();
                });
            }
            catch (Exception e)
            {
                Debug.Log($"初始化失败{e}");
                this.setStatus(MainStatus.error);
            }
        }

        private void onJoinIntentChanged(Message<PresenceJoinIntent> message)
        {
            Debug.Log($"JoinIntentChanged:{JsonConvert.SerializeObject(message.Data)}");
        }

        private void onLaunchIntentChanged(Message<string> message)
        {
            var launchDetails = ApplicationService.GetLaunchDetails();
            Debug.Log($"LaunchIntentChanged {JsonConvert.SerializeObject(launchDetails)}");
            launchDetailCanvas.SetLaunchDetail(launchDetails);
        }

        void setStatus(MainStatus status)
        {
            this.status = status;
            switch (status)
            {
                case MainStatus.main:
                {
                    mainCanvas.SetActive(true);
                    noNetworkCanvas.SetActive(false);
                    loadingCanvas.SetActive(false);
                    break;
                }
                case MainStatus.error:
                {
                    mainCanvas.SetActive(false);
                    noNetworkCanvas.SetActive(true);
                    loadingCanvas.SetActive(false);
                    break;
                }
                case MainStatus.loading:
                {
                    mainCanvas.SetActive(false);
                    noNetworkCanvas.SetActive(false);
                    loadingCanvas.SetActive(true);
                    break;
                }
            }
        }
    }
}