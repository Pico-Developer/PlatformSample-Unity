using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MediaPlayer : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;

    public void ShowImage(string filepath)
    {
        if (!File.Exists(filepath))
        {
            Debug.LogError($"File not exists {filepath}");
            return;
        }

        var x = new Texture2D(200, 200);
        if (!x.LoadImage(File.ReadAllBytes(filepath)))
        {
            Debug.LogError($"Load image failed");
            return;
        }

        rawImage.texture = x;
    }

    public void ShowVideo(string filepath)
    {
        if (!File.Exists(filepath))
        {
            Debug.LogError($"File not exists {filepath}");
            return;
        }

        rawImage.texture = videoPlayer.targetTexture;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = filepath;
        Debug.Log($"before playing video filepath={filepath}");
        videoPlayer.Play();
    }
}