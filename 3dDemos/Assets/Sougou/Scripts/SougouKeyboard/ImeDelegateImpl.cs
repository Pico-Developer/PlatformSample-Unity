using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ImeDelegateImpl : ImeDelegateBase, IDragHandler, IPointerDownHandler, IPointerUpHandler

{
    public Text mText;
    public GameObject mPanel;
    public ImeManager mManager;
    private Texture2D mTexture;
    private Vector2 mTextureSize = new Vector2(780, 390);

    //ImeDelegateBase
    public override void OnIMEShow(Vector2 vSize)
    {
        Debug.Log("OnIMEShow");
        mTextureSize = vSize;
        mManager.Draw(mTexture);
        mPanel.SetActive(true);
    }
    public override void OnIMEHide()
    {
        Debug.Log("OnIMEHide");
        mPanel.SetActive(false);
    }
    public override void OnIMECommit(string strCommit)
    {
        mText.text += strCommit;
    }
    public override void OnIMEKey(SGImeKey key)
    {
        switch (key)
        {
            case SGImeKey.KEYCODE_DEL:
                String strText = mText.text;
                mText.text = strText.Remove(strText.Length - 1);
                break;
            case SGImeKey.KEYCODE_ENTER:
                mText.text = "";
                break;
        }
    }
    public override void OnIMEError(SGImeError nType, string strErr)
    {
    }

    //MonoBehaviour
    void Start()
    {
       // clickTest = GameObject.Find("Text2").GetComponent<Text>();
        CreateTexture();
    }
    void Update()
    {
        Debug.Log("panel update");
        mManager.Draw(mTexture);
    }
    //other
    private void CreateTexture()
    {
        // Create a texture
        mTexture = new Texture2D(780, 390, TextureFormat.RGBA32, false);
        // Set point filtering just so we can see the pixels clearly
        mTexture.filterMode = FilterMode.Point;
        // Call Apply() so it's actually uploaded to the GPU
        mTexture.Apply();

        Debug.Log("texture created");

        // Set texture onto cube
        //GameObject cube = GameObject.Find("Cube");
        //cube.GetComponent<Renderer>().material.mainTexture = mTexture;

        // Set texture onto panel
        Sprite sprite = Sprite.Create(mTexture, new Rect(0, 0, 780, 390), Vector2.zero);

        UnityEngine.UI.Image img = mPanel.GetComponent<UnityEngine.UI.Image>();
        img.sprite = sprite;
        mPanel.SetActive(false);
    }

    private void CorrectPos(ref float x, ref float y)
    {
        float w = UnityEngine.Screen.width;
        float h = UnityEngine.Screen.height;
        y = h - y;
        //adjust ratio
        x = x * mTextureSize[0] / w;
        y = y * mTextureSize[1] / h;
    }

    private void DispatchMessageToAndroid(SGImeMotionEventType type, PointerEventData eventData)
    {
        if (null != mManager)
        {
            float x = eventData.position.x;
            float y = eventData.position.y;
            CorrectPos(ref x, ref y);
            mManager.OnTouch(x, y, type);
        }
    }

    private void LogEvent(string prefix, PointerEventData eventData)
    {
        Debug.Log(prefix + ": " + eventData.pointerCurrentRaycast.gameObject.name + " x=" + eventData.position.x + ",y=" + eventData.position.y);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        LogEvent("Drag Begin", eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        LogEvent("Dragging", eventData);
        DispatchMessageToAndroid(SGImeMotionEventType.ACTION_MOVE, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LogEvent("Drag Ended", eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       
        LogEvent("Clicked", eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LogEvent("Mouse Down", eventData);
        DispatchMessageToAndroid(SGImeMotionEventType.ACTION_DOWN, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LogEvent("Mouse Enter", eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //no event data
        Debug.Log("Mouse Exit");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LogEvent("Mouse Up", eventData);
        DispatchMessageToAndroid(SGImeMotionEventType.ACTION_UP, eventData);
    }
}
