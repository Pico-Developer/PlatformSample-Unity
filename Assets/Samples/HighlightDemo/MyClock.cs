using System;
using TMPro;
using UnityEngine;

public class MyClock : MonoBehaviour
{
    public TMP_Text text;

    // Update is called once per frame
    void Update()
    {
        text.SetText(DateTime.Now.ToString());
    }
}