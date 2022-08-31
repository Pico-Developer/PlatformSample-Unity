using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImeManager : MonoBehaviour
{
    public ImeDelegateBase mDelegate;
    private ImeBase mIme;
    private Vector2 mSize;

    //MonoBehaviour
    void Start()
    {
        if (null != mDelegate)
        {
#if UNITY_EDITOR
          //DebugHelper.instance.Log("DummyIme");
            mIme = new DummyIme();
#else
           //DebugHelper.instance.Log("SGIme");
            mIme = new SGIme();
#endif
            mIme.Create(mDelegate);
        }
    }

    void Update()
    {
        if (null == mIme)
        {
            return;
        }
        mIme.UpdateData();
    }

    //export
    public void Show(SGImeInputType typeInput, SGImeTextType typeText)
    {
        //DebugHelper.instance.Log("Imemanager.show()"); check right
        mIme.Show(typeInput, typeText);
        mIme.GetSize(ref mSize);
    }

    public void Hide()
    {
        mIme.Hide();
        mDelegate.OnIMEHide();
    }

    public void Draw(Texture2D tex)
    {
        mIme.Draw(tex);
    }

    public void OnTouch(float x, float y, SGImeMotionEventType type)
    {
        mIme.OnTouch(x, y, type);
    }
}
