using Newtonsoft.Json;
using Pico.Platform.Samples;
using UnityEngine;
using UnityEngine.UI;

namespace PICO.Platform.Samples.Invite
{
    public class AllFriends : MonoBehaviour
    {
        //Refresh friend list button
        public Button refreshFriendList;

        //The container for the friend list
        public GameObject friendContent;
        public GameObject userPrefab;
        public ObjectPool userPool;
        private bool isLoading = false;
        public Toast toast;

        private void Start()
        {
            this.setLoading(false);
            userPool = new ObjectPool();

            refreshFriendList.onClick.AddListener(() => { this.Load(); });
        }

        void setLoading(bool value)
        {
            if (value)
            {
                this.isLoading = true;
                refreshFriendList.enabled = false;
            }
            else
            {
                this.isLoading = false;
                refreshFriendList.enabled = true;
            }
        }

        public void Load()
        {
            this.setLoading(true);
            Debug.Log("loading friends...");
            GetAllFriends.Run(users =>
            {
                this.setLoading(false);
                Debug.Log($"loading friends successfully {users.Count}");
                for (var i = friendContent.transform.childCount - 1; i >= 0; i--)
                {
                    var child = friendContent.transform.GetChild(i).gameObject;
                    userPool.Put(child);
                }

                foreach (var u in users)
                {
                    var user = InviteUtil.ToMyUser(u);
                    var userObj = userPool.Get(userPrefab);
                    userObj.GetComponent<UserPrefab>().SetUser(user, showToast);
                    userObj.transform.SetParent(friendContent.transform, false);
                }
            }, err =>
            {
                this.setLoading(false);
                Debug.Log($"load friends error: {JsonConvert.SerializeObject(err)}");
            });
        }

        void showToast(string s)
        {
            toast.Show(s);
        }
    }
}