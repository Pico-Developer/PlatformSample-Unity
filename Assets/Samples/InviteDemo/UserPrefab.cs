using System;
using Newtonsoft.Json;
using Pico.Platform;
using Pico.Platform.Samples;
using PICO.Platform.Samples.Invite.Models;
using UnityEngine;
using UnityEngine.UI;

namespace PICO.Platform.Samples.Invite
{
    public class UserPrefab : MonoBehaviour
    {
        private User user;

        public Text nickName;

        public RawImage headImage;

        public Button buttonSendInvites;

        public Button buttonAddFriend;
        private Action<string> showToast;

        private void SendInvites()
        {
            CustomMessageCanvas.Send(user.Id, showToast);
        }

        void toast(string s)
        {
            if (showToast != null)
            {
                showToast(s);
            }
        }

        private void addFriend()
        {
            UserService.LaunchFriendRequestFlow(user.Id).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    toast("Add friend failed");
                    Debug.Log($"Add friend failed:{JsonConvert.SerializeObject(msg.Error)}");
                    return;
                }

                toast("Add friends successfully.");
                Debug.Log($"Add friend successfully {JsonConvert.SerializeObject(msg.Data)}");
                buttonAddFriend.enabled = false;
                user.IsFriend = true;
            });
        }

        public void SetUser(User user, Action<string> toast)
        {
            this.showToast = toast;
            if (user == null)
            {
                this.user = null;
                return;
            }

            this.user = user;
            StartCoroutine(NetworkUtils.BindImage(user.Avatar, headImage));
            nickName.text = user.NickName;
            buttonSendInvites.onClick.RemoveAllListeners();
            buttonSendInvites.onClick.AddListener(SendInvites);
            buttonAddFriend.onClick.RemoveAllListeners();
            buttonAddFriend.onClick.AddListener(addFriend);
        }
    }
}