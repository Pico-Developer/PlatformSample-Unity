using Pico.Platform;
using Pico.Platform.Models;
using Pico.Platform.Samples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListMediaPanel : MonoBehaviour
{
    public GameObject imageList;
    public GameObject videoList;
    public GameObject mediaItemPrefab;
    public MediaPlayer mediaPlayer;

    public Button buttonShare;
    public TMP_Text textChosen;
    public Button buttonSave;
    public Button buttonClose;
    private SessionMedia sessionMedia;
    private string sessionId;
    private ObjectPool mediaItemPool;

    private object chosen;

    // Start is called before the first frame update
    void Start()
    {
        buttonShare.onClick.AddListener(DoShare);

        buttonClose.onClick.AddListener(() => { this.gameObject.SetActive(false); });

        buttonSave.onClick.AddListener(DoSave);
        checkPool();
    }

    void checkPool()
    {
        if (mediaItemPool == null)
        {
            mediaItemPool = new ObjectPool();
        }
    }

    public void DoSave()
    {
        if (chosen == null)
        {
            Debug.Log($"Please choose a media");
            return;
        }

        if (chosen is CaptureInfo)
        {
            var info = chosen as CaptureInfo;
            Debug.Log($"Saving captureInfo jobId={info.JobId} sessionId={sessionId}");
            HighlightService.SaveMedia(info.JobId, sessionId).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Debug.Log($"Save failed {m.Error}");
                    return;
                }

                Debug.Log($"Save successful");
            });
        }
        else
        {
            var info = chosen as RecordInfo;
            Debug.Log($"Saving captureInfo jobId={info.JobId} sessionId={sessionId}");
            HighlightService.SaveMedia(info.JobId, sessionId).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Debug.Log($"Save failed {m.Error}");
                    return;
                }

                Debug.Log($"Save successful");
            });
        }
    }

    public void DoShare()
    {
        if (chosen == null)
        {
            Debug.Log($"Please choose a media");
            return;
        }

        if (chosen is CaptureInfo)
        {
            var info = chosen as CaptureInfo;
            Debug.Log($"sharing captureInfo jobId={info.JobId} sessionId={sessionId}");
            HighlightService.ShareMedia(info.JobId, sessionId).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Debug.Log($"Share failed {m.Error}");
                    return;
                }

                Debug.Log($"Share successful");
            });
        }
        else
        {
            var info = chosen as RecordInfo;
            Debug.Log($"sharing captureInfo jobId={info.JobId} sessionId={sessionId}");
            HighlightService.ShareMedia(info.JobId, sessionId).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Debug.Log($"Share failed {m.Error}");
                    return;
                }

                Debug.Log($"Share successful");
            });
        }
    }

    void DestroyChildren(GameObject x, ObjectPool pool)
    {
        int childs = x.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            pool.Put(x.transform.GetChild(i).gameObject);
        }
    }

    public void Show(string sessionId, SessionMedia sessionMedia)
    {
        checkPool();
        this.sessionId = sessionId;
        this.sessionMedia = sessionMedia;
        this.gameObject.SetActive(true);

        DestroyChildren(imageList, mediaItemPool);
        DestroyChildren(videoList, mediaItemPool);
        Debug.Log($"images count {sessionMedia.Images.Length}");
        foreach (var img in sessionMedia.Images)
        {
            var x = mediaItemPool.Get(mediaItemPrefab);
            var mediaItem = x.GetComponent<MediaItem>();
            mediaItem.Init(img.JobId, img.ImagePath, img, this);
            mediaItem.transform.SetParent(imageList.transform, false);
        }

        Debug.Log($"videos count {sessionMedia.Videos.Length}");

        foreach (var video in sessionMedia.Videos)
        {
            var x = mediaItemPool.Get(mediaItemPrefab);
            var mediaItem = x.GetComponent<MediaItem>();
            mediaItem.Init(video.JobId, video.VideoPath, video, this);
            mediaItem.transform.SetParent(videoList.transform, false);
        }
    }

    public void setChosen(string jobId, string filePath, object data)
    {
        Debug.Log($"chosen jobId={jobId} filepath={filePath}");
        this.chosen = data;
        textChosen.SetText(filePath);
    }

    public void See(string filepath, object data)
    {
        if (string.IsNullOrEmpty(filepath))
        {
            Debug.LogError($"invalid filepath");
            return;
        }

        if (data is RecordInfo)
        {
            Debug.Log($"Playing video {filepath}");
            mediaPlayer.ShowVideo(filepath);
        }
        else
        {
            Debug.Log($"Showing image {filepath}");
            mediaPlayer.ShowImage(filepath);
        }
    }
}