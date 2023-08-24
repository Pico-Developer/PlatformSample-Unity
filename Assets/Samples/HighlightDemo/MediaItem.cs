using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MediaItem : MonoBehaviour
{
    public Button buttonChoose;
    public Button buttonSee;
    public TMP_Text text;
    private string jobId;

    private string filepath;

    private object data;

    private ListMediaPanel panel;

    void Start()
    {
        buttonChoose.onClick.AddListener(() => { panel.setChosen(jobId, filepath, this.data); });
        buttonSee.onClick.AddListener(() => { panel.See(filepath, data); });
    }

    public void Init(string imgJobId, string imgImagePath, object data, ListMediaPanel panel)
    {
        this.jobId = imgJobId;
        this.filepath = imgImagePath;
        this.data = data;
        this.panel = panel;
        text.SetText($"path:{this.filepath}\njobId:{this.jobId}");
    }
}