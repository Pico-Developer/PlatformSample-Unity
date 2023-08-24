using TMPro;
using UnityEngine;

public class PanelInfo : MonoBehaviour
{
    public TMP_Text text;
    public int Duration = 3;

    private float showTime;

    public void Error(string s)
    {
        text.color = Color.red;
        text.text = s;
        this.showTime = Time.time;
        this.gameObject.SetActive(true);
    }

    public void Info(string s)
    {
        text.color = Color.green;
        text.text = s;
        this.showTime = Time.time;
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        if (Time.time - showTime > Duration)
        {
            this.gameObject.SetActive(false);
        }
    }
}