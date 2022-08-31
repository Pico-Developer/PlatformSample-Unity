using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyIme : ImeBase
{
    public bool Create(ImeDelegateBase pDelegate)
    {
        Debug.Log("ime create");
        return true;
    }
    public void GetSize(ref Vector2 size)
    {
        Debug.Log("ime getsize");
        size[0] = 780;
        size[1] = 390;
    }
    public void Draw(Texture2D tex)
    {
        //Debug.Log("ime draw");
    }
    public void OnTouch(float x, float y, SGImeMotionEventType type)
    {
        Debug.Log("ime ontouch:(" + x + "," + y + "),type=" + type);
    }
    public void UpdateData()
    {
        //Debug.Log("ime updatedata");
    }
    public void Show(SGImeInputType typeInput, SGImeTextType typeText)
    {
        DebugHelper.instance.Log("DummyIme out");
        Debug.Log("ime show:" + typeInput + ",texttype:" + typeText);
    }
    public void Hide()
    {
        Debug.Log("ime hide");
    }
}