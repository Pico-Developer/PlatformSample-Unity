using Newtonsoft.Json;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples.IAP
{
    public class AssetFileItem : MonoBehaviour
    {
        public DLC root;
        public AssetDetails detail;
        public Button ButtonDownload;
        public Button ButtonStatus;
        public Button ButtonDelete;
        public Button ButtonCancelDownload;
        public Button ButtonBuy;
        public Slider SliderDownloadProgress;
        public Text mainDesc;
        public GameObject downloadProgressPanel;

        private void Start()
        {
            ButtonStatus.onClick.AddListener(() => { this.StatusAsset(); });
            ButtonDelete.onClick.AddListener(() =>
            {
                (root.UseName ? AssetFileService.DeleteByName(detail.Filename) : AssetFileService.DeleteById(detail.AssetId))
                    .OnComplete(msg =>
                    {
                        if (msg.IsError)
                        {
                            root.Log($"Delete error:{JsonConvert.SerializeObject(msg.Error)}");
                            root.toast.Show("Delete error");
                            return;
                        }

                        root.Log($"DeleteByName:{JsonConvert.SerializeObject(msg.Data)}");
                        if (!msg.Data.Success)
                        {
                            root.toast.Show("Delete failed");
                            return;
                        }

                        root.toast.Show("Delete success");
                    });
            });
            ButtonDownload.onClick.AddListener(() =>
            {
                (root.UseName ? AssetFileService.DownloadByName(detail.Filename) : AssetFileService.DownloadById(detail.AssetId)).OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        root.toast.Show("download error");
                        root.Log($"DownloadError:{JsonConvert.SerializeObject(msg.Error)}");
                        return;
                    }

                    root.Log($"DownloadByName:{JsonConvert.SerializeObject(msg.Data)}");
                    this.StatusAsset();
                });
            });
            ButtonCancelDownload.onClick.AddListener(() =>
            {
                (root.UseName ? AssetFileService.DownloadCancelByName(detail.Filename) : AssetFileService.DownloadCancelById(detail.AssetId)).OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        root.Log($"CancelDownload failed:{JsonConvert.SerializeObject(msg.Error)}");
                        return;
                    }

                    root.Log($"CancelDownloadByName success:{JsonConvert.SerializeObject(msg.Data)}");
                    this.StatusAsset();
                });
            });
            ButtonBuy.onClick.AddListener(() =>
            {
                IAPService.LaunchCheckoutFlow(detail.IapSku, detail.IapPrice, detail.IapCurrency).OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        root.toast.Show("LaunchCheckFlow failed");
                        root.Log($"LaunchCheckoutFlow failed:{JsonConvert.SerializeObject(msg.Error)}");
                        return;
                    }

                    root.Log($"LaunchCheckoutFlow success:{JsonConvert.SerializeObject(msg.Data)}");
                    detail.IapStatus = "entitled";
                    this.updateUI();
                });
            });
        }

        private void StatusAsset()
        {
            (root.UseName ? AssetFileService.StatusByName(detail.Filename) : AssetFileService.StatusById(detail.AssetId)).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    root.toast.Show("StatusByName failed");
                    root.Log($"StatusByName failed:{JsonConvert.SerializeObject(msg.Error)}");
                    return;
                }

                root.Log($"StatusByName:{JsonConvert.SerializeObject(msg.Data)}");
                updateAssetStatus(msg.Data);
            });
        }

        private void updateAssetStatus(AssetStatus msgData)
        {
            if (this.detail.AssetId != msgData.AssetId)
            {
                root.toast.Show("Asset Status assetId inconsistent");
                Debug.LogError($"Asset Status assetId inconsistent");
                return;
            }

            detail.Filename = msgData.Filename;
            detail.Filepath = msgData.Filepath;
            detail.DownloadStatus = msgData.DownloadStatus;
            this.updateUI();
        }

        public void updateUI()
        {
            var iapStatus = detail.IapStatus;
            var downloadStatus = detail.DownloadStatus;

            //设置按钮的状态
            setButtonStatus(ButtonStatus, true);
            setButtonStatus(ButtonDelete, downloadStatus == DownloadStatus.Downloaded);
            bool buttonDownloadStatus = !(downloadStatus == DownloadStatus.Downloaded || iapStatus == IapStatus.NotEntitled);
            setButtonStatus(ButtonDownload, buttonDownloadStatus);
            setButtonStatus(ButtonBuy, iapStatus == IapStatus.NotEntitled);
            setButtonStatus(ButtonCancelDownload, downloadStatus == DownloadStatus.InProgress);

            // 当处于下载状态的时候，显示进度条
            if (downloadStatus != DownloadStatus.InProgress)
            {
                downloadProgressPanel.SetActive(false);
            }
            else
            {
                downloadProgressPanel.SetActive(true);
            }

            mainDesc.text = $"{JsonConvert.SerializeObject(detail, Formatting.Indented)}";
        }

        public void Init(AssetDetails assetDetails, DLC root)
        {
            detail = assetDetails;
            this.root = root;
            this.updateUI();
        }

        void setButtonStatus(Button b, bool status)
        {
            if (b == null) return;
            if (root.toggleShowAllButtons.isOn)
            {
                b.gameObject.SetActive(true);
                return;
            }

            b.gameObject.SetActive(status);
        }

        public void updateProgress(long bytesTransferred, ulong bytesTotal)
        {
            if (bytesTransferred < 0)
            {
                root.Log("Download failed");
                root.toast.Show("Download failed");
                return;
            }

            var progress = bytesTransferred * 1.0f / bytesTotal;
            SliderDownloadProgress.value = progress;
        }
    }
}