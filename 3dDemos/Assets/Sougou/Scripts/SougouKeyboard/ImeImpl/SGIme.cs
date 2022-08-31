using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGIme : ImeBase
{
#if UNITY_EDITOR
    private bool mUseAndroid = false;
#else
    private bool mUseAndroid = true;
#endif
    private AndroidJavaObject javaIme = null;
    private ImeDelegateBase mDelegate;
    private Vector2 mTextureSize;
    private string mStrCommit;
    private bool mShow = false;

    public bool Create(ImeDelegateBase pDelegate)
    {
        Debug.Log("ime create");
        //DebugHelper.instance.Log("JavaInit out");
        mDelegate = pDelegate;
        JavaInit();
        return true;
    }
    public void GetSize(ref Vector2 size)
    {
        size = mTextureSize;
        Debug.Log("ime getsize:" + size[0] + "," + size[1]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tex"></param>
        //---------First time call this function
    bool isFirstTime = true;
    public void Draw(Texture2D tex)
    {
        if (isFirstTime)
        {
            byte[] data = javaIme.Call<byte[]>("getTextureData");
            tex.LoadRawTextureData(data);
            tex.Apply();
            isFirstTime = false;
        }
        if (IsInited() && IsNeedUpdate())
        {
            //--------------------------Ilyas----------------------
            Debug.Log("ime draw");
            byte[] data = javaIme.Call<byte[]>("getTextureData");
            tex.LoadRawTextureData(data);
            tex.Apply();
        }
    }
    public void Show(SGImeInputType typeInput, SGImeTextType typeText)
    {

        if (IsInited())
        {
            Debug.Log("ime show:" + typeInput + "," + typeText);
            javaIme.Call<bool>("show", (int)typeInput, (int)typeText);

        }
    }
    public void Hide()
    {
        if (IsInited())
        {
            Debug.Log("ime hide");
            javaIme.Call<bool>("hide");
            //mShow = false;
        }
    }
    public void OnTouch(float x, float y, SGImeMotionEventType type)
    {
        if (IsInited())
        {
            Debug.Log("ime ontouch:(" + x + "," + y + "),type=" + type);
            bool bHandle = javaIme.Call<bool>("onTouch", x, y, (int)type);
        }
    }
    public void UpdateData()
    {
        if (!IsInited())
        {
            return;
        }
        //update commit
        SGImeKey nCommit = (SGImeKey)GetCommitCode();
        if (nCommit == SGImeKey.KEYCODE_COMMIT)
        {
            mStrCommit = GetCommitString();
            Debug.Log("ime updatedata commit string:" + mStrCommit);
            mDelegate.OnIMECommit(mStrCommit);
        }
        else if (nCommit != SGImeKey.KEYCODE_UNKNOWN)
        {
            Debug.Log("ime updatedata commit key:"+nCommit);
            mDelegate.OnIMEKey(nCommit);
        }
        //update hide
        bool bShow = IsShow();
        if (!bShow && mShow)
        {
            mShow = bShow;
            mDelegate.OnIMEHide();
        }
        else if (bShow && !mShow)
        {
            mShow = bShow;
            mDelegate.OnIMEShow(mTextureSize);
        }
    }
    private void JavaInit()
    {
           
        if (mUseAndroid)
        {
            

            javaIme = new AndroidJavaObject("com.sohu.inputmethod.sogou.sgrenderproxy.RenderProxyUnity");
           
            int[] s = javaIme.Call<int[]>("getSize");
            mTextureSize[0] = s[0];
            mTextureSize[1] = s[1];
        }
    }

    private bool IsInited()
    {
       // DebugHelper.instance.Log((null != javaIme).ToString());
        return mUseAndroid && null != javaIme;
    }


    private bool IsNeedUpdate()
    {
            bool bNeedUpdate = javaIme.Call<bool>("isNeedRefresh");
            return bNeedUpdate;
    }

    private int GetCommitCode()
    {
        return javaIme.Call<int>("getCommitCode");
    }
    private string GetCommitString()
    {
        string strCommit = javaIme.Call<string>("getCommitString");
        return strCommit;
    }
    private bool IsShow()
    {
        return javaIme.Call<bool>("isShow");
    }
}
