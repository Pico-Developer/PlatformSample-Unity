using System.Collections.Generic;
using IngameDebugConsole;
using Newtonsoft.Json;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pico.Platform.Samples.IAP
{
    public class DLC : MonoBehaviour
    {
        private List<AssetFileItem> AssetFileItems = new List<AssetFileItem>();
        public GameObject AssetItemPrefab;
        public GameObject contentView;
        public bool UseName => this.toggleUseName.isOn; //使用byName系列的API接口
        public Button switchToIap;

        [FormerlySerializedAs("ToggleUseName")]
        public Toggle toggleUseName;

        [FormerlySerializedAs("ToggleShowAllButtons")]
        public Toggle toggleShowAllButtons;

        public Button refreshAssetList;
        public Toast toast;
        public Button buttonNextPage;
        public Button showLog;
        private AssetDetailsList detailsList;

        void Start()
        {
            toggleShowAllButtons.onValueChanged.AddListener((v) =>
            {
                foreach (var i in AssetFileItems)
                {
                    i.updateUI();
                }
            });
            showLog.onClick.AddListener(() =>
            {
                if (DebugLogManager.Instance != null)
                {
                    DebugLogManager.Instance.ShowLogWindow();
                }
            });
            switchToIap.onClick.AddListener(() => { SceneManager.LoadScene("Samples/IAP/IAP"); });
            refreshAssetList.onClick.AddListener(loadAssetList);
            buttonNextPage.onClick.AddListener(loadNextPage);
            CoreService.AsyncInitialize().OnComplete(msg =>
            {
                Log($"AsyncInitialize :{JsonConvert.SerializeObject(msg)}");
                loadAssetList();
            });
            AssetFileService.SetOnDeleteForSafetyCallback(msg => { Log($"AssetId={msg.Data.AssetId} Reason={msg.Data.Reason}"); });
            AssetFileService.SetOnDownloadUpdateCallback(msg =>
            {
                var info = msg.Data;
                if (info.CompleteStatus == AssetFileDownloadCompleteStatus.Failed)
                {
                    toast.Show($"Download Failed:{info.AssetId}");
                    Log($"Download Failed:AssetId={info.AssetId} Bytes:{info.BytesTransferred}/{info.BytesTotal}");
                    return;
                }

                if (info.CompleteStatus == AssetFileDownloadCompleteStatus.Succeed)
                {
                    Log($"Download Completed:AssetId={info.AssetId} Bytes:{info.BytesTransferred}/{info.BytesTotal}");
                }

                UpdateAssetFileDownloadStatus(info.AssetId, info.BytesTotal, info.BytesTransferred);
            });
        }

        void loadNextPage()
        {
            if (detailsList == null)
            {
                toast.Show("Please refresh data");
                return;
            }

            if (!detailsList.HasNextPage)
            {
                toast.Show("Has no next page");
                return;
            }

            AssetFileService.GetNextAssetDetailsListPage(detailsList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    toast.Show("GetNextPage failed");
                    Log($"GetNextAssetDetailsListPage failed:{JsonConvert.SerializeObject(msg.Error)}");
                    return;
                }

                Log($"GetNextAssetDetailsListPage success :{msg.Data}");
                ResetAssetFileList(msg.Data);
            });
        }

        private void loadAssetList()
        {
            AssetFileService.GetList().OnComplete(assetListMsg =>
            {
                if (assetListMsg.IsError)
                {
                    Log($"AssetFile GetList failed :{JsonConvert.SerializeObject(assetListMsg.Error)}");
                    return;
                }

                Log($"Get asset list successfully :count={assetListMsg.Data.Count}");
                toast.Show($"Get asset list successfully :count={assetListMsg.Data.Count}");
                foreach (var i in assetListMsg.Data)
                {
                    Debug.Log(JsonConvert.SerializeObject(i));
                }

                ResetAssetFileList(assetListMsg.Data);
            });
        }


        void ResetAssetFileList(AssetDetailsList assetDetailsList)
        {
            this.detailsList = assetDetailsList;
            for (int i = AssetFileItems.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(AssetFileItems[i].gameObject);
            }

            AssetFileItems.Clear();
            for (int i = 0; i < assetDetailsList.Count; i++)
            {
                var g = Instantiate(AssetItemPrefab, contentView.transform, false);
                var assetFileItem = g.GetComponent<AssetFileItem>();
                assetFileItem.Init(assetDetailsList[i], this);
                AssetFileItems.Add(assetFileItem);
            }
        }

        void UpdateAssetFileDownloadStatus(ulong assetId, ulong bytesTotal, long bytesTransferred)
        {
            foreach (var i in AssetFileItems)
            {
                if (i.detail.AssetId == assetId)
                {
                    i.updateProgress(bytesTransferred, bytesTotal);
                    return;
                }
            }

            Log($"cannot find asset id:{assetId}");
        }

        public void Log(string s)
        {
            Debug.Log(s);
        }
    }
}