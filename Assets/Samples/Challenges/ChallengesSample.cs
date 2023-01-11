using System;
using Pico.Platform.Models;
using Pico.Platform.Samples.Game;
using Stark;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pico.Platform.Samples
{
    public class ChallengesSample : MonoBehaviour
    {
        public Button LaunchBtn;
        public Button BackToMainPanelBtn;
        private ulong curChallengeID;
        private const string TAG = "PPF_Game unity";
        public GameObject ChallengeItemObj;
        public Button GetListBtn;

        public InputField GetListPageSize;

        public InputField GetListPageIndex;

        public ChallengeOptionsItem Option;

        // search params
        public InputField GetEntriesPageIndex;

        public InputField GetEntriesPageSize;

        public Dropdown GetEntriesFilter;

        public Dropdown GetEntriesStartAt;

        public InputField GetEntriesAfterRank;

        public InputField GetEntriesUserIds;

        public Button GetEntriesBtn;

        public GameObject ChallengeEntryItemObj;

        public Button InviteBtn;

        public InputField InviteUserId;

        // Start is called before the first frame update
        void Start()
        {
            LogHelper.LogInfo(TAG, $"Start");

            GetListBtn.onClick.RemoveAllListeners();
            GetEntriesBtn.onClick.RemoveAllListeners();
            BackToMainPanelBtn.onClick.RemoveAllListeners();
            InviteBtn.onClick.RemoveAllListeners();
            LaunchBtn.onClick.RemoveAllListeners();
            

            GetListBtn.onClick.AddListener(OnGetListBtnClick);
            GetEntriesBtn.onClick.AddListener(OnGetEntriesBtnClick);
            BackToMainPanelBtn.onClick.AddListener(OnBackToMainPanelBtnClick);
            InviteBtn.onClick.AddListener(OnInviteBtnClick);
            LaunchBtn.onClick.AddListener(OnLaunchBtnClick);


#if !UNITY_EDITOR
            if (!CoreService.Initialized)
            {
                CoreService.Initialize(GameConfig.GetAppId());
            }
#endif
        }

        void Awake()
        {
            Application.logMessageReceivedThreaded += OnLogMessage;
        }

        void OnDestroy()
        {
            Application.logMessageReceivedThreaded -= OnLogMessage;
        }

        void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            DebugPanelHelper.Log(condition, stackTrace, type);
            // CaptureLogThread(condition, stackTrace, type);
        }

        void OnInviteBtnClick()
        {
            ChallengesService.Invite(curChallengeID, InviteUserId.text.Split(',')).OnComplete(OnInviteComplete);
        }

        void OnBackToMainPanelBtnClick()
        {
            LogHelper.LogInfo(TAG, $"Back To MainPanel");
            SceneManager.LoadScene("GameAPITestScene", LoadSceneMode.Single);
        }

        void OnGetListBtnClick()
        {
            var task = ChallengesService.GetList(Option.GetOption(), GetGetListPageIndex(), GetGetListPageSize());
            task.OnComplete(OnGetListComplete);
            LogHelper.LogInfo(TAG, $"GetList taskId: {task.TaskId}");
        }

        void OnLaunchBtnClick()
        {
            var task = ChallengesService.LaunchInvitableUserFlow(curChallengeID).OnComplete((Message message) =>
            {
                LogHelper.LogInfo(TAG, $"message.Type: {message.Type}");
                if (!message.IsError)
                {
                    LogHelper.LogInfo(TAG, $"OnLaunchInvitableUserFlowComplete no error");
                }
                else
                {
                    var error = message.GetError();
                    LogHelper.LogInfo(TAG, $"OnLaunchInvitableUserFlowComplete error: {error.Message}");
                }
            });
            LogHelper.LogInfo(TAG, $"GetList taskId: {task.TaskId}");
        }

        void OnGetEntriesBtnClick()
        {
            if (!string.IsNullOrEmpty(GetEntriesAfterRank.text))
            {
                var task = ChallengesService.GetEntriesAfterRank(curChallengeID,
                    Convert.ToUInt64(GetEntriesAfterRank.text),
                    Convert.ToInt32(GetEntriesPageIndex.text),
                    Convert.ToInt32(GetEntriesPageSize.text));
                task.OnComplete(OnGetEntriesComplete);
                LogHelper.LogInfo(TAG, $"GetEntriesAfterRank taskId: {task.TaskId}");
            }
            else if (!string.IsNullOrEmpty(GetEntriesUserIds.text))
            {
                var task = ChallengesService.GetEntriesByIds(Convert.ToUInt64(curChallengeID),
                    (LeaderboardStartAt) GetEntriesStartAt.value,
                    GetEntriesUserIds.text.Split(','),
                    Convert.ToInt32(GetEntriesPageIndex.text),
                    Convert.ToInt32(GetEntriesPageSize.text)).OnComplete(OnGetEntriesComplete);
                LogHelper.LogInfo(TAG, $"GetEntriesByIds taskId: {task.TaskId}");
            }
            else
            {
                var task = ChallengesService.GetEntries(Convert.ToUInt64(curChallengeID),
                    (LeaderboardFilterType) GetEntriesFilter.value,
                    (LeaderboardStartAt) GetEntriesStartAt.value,
                    Convert.ToInt32(GetEntriesPageIndex.text),
                    Convert.ToInt32(GetEntriesPageSize.text)).OnComplete(OnGetEntriesComplete);
                LogHelper.LogInfo(TAG, $"GetEntries taskId: {task.TaskId}");
            }
        }

        void OnGetEntriesComplete(Message<ChallengeEntryList> msg)
        {
            if (msg.IsError)
            {
                LogHelper.LogError(TAG, $"OnGetEntriesComplete error: {msg.GetError().Code}, {msg.GetError().Message}");
                return;
            }

            RemoveAllChildren(ChallengeEntryItemObj.transform.parent.gameObject);
            ChallengeEntryList challengeEntryList = msg.Data;
            LogHelper.LogInfo(TAG, $"OnGetEntriesComplete success challengeEntryList.count: {challengeEntryList.Count}");
            var list = challengeEntryList.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                GameObject obj = Instantiate(ChallengeEntryItemObj, ChallengeEntryItemObj.transform.parent);
                obj.name = item.ID.ToString();
                obj.transform.Find("Text").GetComponent<Text>().text = item.ID.ToString();
                obj.SetActive(true);

                var btn = obj.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => { LogHelper.LogInfo(TAG, GameDebugLog.GetLogData(item)); });
            }
        }

        void OnGetListComplete(Message<ChallengeList> msg)
        {
            if (msg.IsError)
            {
                LogHelper.LogError(TAG, $"OnGetListComplete error: {msg.GetError().Code}, {msg.GetError().Message}");
                return;
            }

            RemoveAllChildren(ChallengeItemObj.transform.parent.gameObject);
            RemoveAllChildren(ChallengeEntryItemObj.transform.parent.gameObject);
            ChallengeList challengeList = msg.Data;
            LogHelper.LogInfo(TAG, $"OnGetListComplete success challengeList.count: {challengeList.Count}");
            LogHelper.LogInfo(TAG, GameDebugLog.GetLogData(challengeList));
            var list = challengeList.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                GameObject obj = Instantiate(ChallengeItemObj, ChallengeItemObj.transform.parent);
                obj.name = item.ID.ToString();
                obj.transform.Find("Text").GetComponent<Text>().text = item.ID.ToString();
                obj.SetActive(true);

                var joinBtn = obj.transform.Find("JoinBtn").GetComponent<Button>();
                joinBtn.onClick.RemoveAllListeners();
                joinBtn.onClick.AddListener(() =>
                {
                    ChallengesService.Join(item.ID).OnComplete(OnJoinComplete); 
                });

                var leaveBtn = obj.transform.Find("LeaveBtn").GetComponent<Button>();
                leaveBtn.onClick.RemoveAllListeners();
                leaveBtn.onClick.AddListener(() =>
                {
                    ChallengesService.Leave(item.ID).OnComplete(OnLeaveComplete); 
                });

                var btn = obj.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    curChallengeID = item.ID;
                    LogHelper.LogInfo(TAG, $"curChallengeID = {curChallengeID}");
                    ChallengesService.Get(Convert.ToUInt64(item.ID)).OnComplete(OnGetComplete);
                });
            }
        }

        void OnGetComplete(Message<Challenge> msg)
        {
            CommonOnComplete(msg.Type, msg.IsError, msg.Data, msg.Error, "OnGetComplete");
        }

        void RemoveAllChildren(GameObject parent)
        {
            Transform transform;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                transform = parent.transform.GetChild(i);
                if (transform.gameObject.activeSelf)
                {
                    Destroy(transform.gameObject);
                }
            }
        }

        void OnJoinComplete(Message<Challenge> msg)
        {
            CommonOnComplete(msg.Type, msg.IsError, msg.Data, msg.Error, "OnJoinComplete");
        }

        void OnLeaveComplete(Message<Challenge> msg)
        {
            CommonOnComplete(msg.Type, msg.IsError, msg.Data, msg.Error, "OnLeaveComplete");
        }

        void OnInviteComplete(Message<Challenge> msg)
        {
            CommonOnComplete(msg.Type, msg.IsError, msg.Data, msg.Error, "OnInviteComplete");
        }

        void CommonOnComplete(MessageType type, bool isError, object data, Error error, string funName)
        {
            LogHelper.LogInfo(TAG, $"message.Type: {type}");
            if (!isError)
            {
                LogHelper.LogInfo(TAG, $"{funName} success");
                LogHelper.LogInfo(TAG, GameDebugLog.GetLogData(data));
            }
            else
            {
                LogHelper.LogError(TAG, $"{funName} error: {error.Message}");
            }
        }

        int GetGetListPageIndex()
        {
            return Convert.ToInt32(GetListPageIndex.text);
        }

        int GetGetListPageSize()
        {
            return Convert.ToInt32(GetListPageSize.text);
        }
    }
}