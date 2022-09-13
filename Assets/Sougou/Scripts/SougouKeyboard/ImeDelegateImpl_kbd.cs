using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class SGViewGather
{
    private GameObject[] mViews;

    public SGViewGather(GameObject[] param)
    {
        mViews = param;
    }

    public void SetActive(bool bActive)
    {
        foreach (GameObject view in mViews)
        {
            view.SetActive(bActive);
        }
    }

    public bool FindName(string name)
    {
        foreach (GameObject view in mViews)
        {
            if (view.name == name)
            {
                return true;
            }
        }

        return false;
    }

    public void SetTexture(Texture2D tex)
    {
        foreach (GameObject view in mViews)
        {
            Renderer rend = view.GetComponent<Renderer>();
            rend.material.mainTexture = tex;
        }
    }
}

public class SGMouseTracker
{
    private bool mDownOld = false;
    private Vector2 mPtOld = new Vector2();
    private SGImeMotionEventType mEvent;
    private const float mTrackRadius = 10.0f;
    private long mTimeDown;
    private bool mLongPressed = false;
    private long mIntervelLongPress = 100;

    public bool Track(Vector2 pt, bool bDown)
    {
        bool bRes = false;
        if (bDown)
        {
            if (mDownOld)
            {
                mEvent = SGImeMotionEventType.ACTION_MOVE;
                if (!mLongPressed)
                {
                    long timeDiff = DateTime.Now.Ticks - mTimeDown;
                    if (timeDiff > mIntervelLongPress)
                    {
                        mLongPressed = true;
                        mEvent = SGImeMotionEventType.ACTION_LONGPRESS;
                        bRes = true; //force sendmessage
                    }
                }
            }
            else
            {
                mEvent = SGImeMotionEventType.ACTION_DOWN;
                mTimeDown = DateTime.Now.Ticks;
                mLongPressed = false;
            }
        }
        else
        {
            if (mDownOld)
            {
                mEvent = SGImeMotionEventType.ACTION_UP;
            }
            else
            {
                //mEvent = SGImeMotionEventType.ACTION_HOVER_MOVE;
                mEvent = SGImeMotionEventType.ACTION_MOVE; //c++代码只识别move事件
            }
        }

        if (mDownOld != bDown)
        {
            bRes = true;
        }
        else if (PointDist(mPtOld, pt) > mTrackRadius)
        {
            bRes = true;
        }

        mDownOld = bDown;

        if (bRes)
        {
            mPtOld = pt;
        }

        return bRes;
    }

    public bool TrackOuter()
    {
        bool bRes = false;
        if (mEvent != SGImeMotionEventType.ACTION_OUTSIDE)
        {
            SGImeMotionEventType eventOld = mEvent;
            mEvent = SGImeMotionEventType.ACTION_OUTSIDE;
        }

        return bRes;
    }

    public Vector2 GetPoint()
    {
        return mPtOld;
    }

    public SGImeMotionEventType GetEvent()
    {
        return mEvent;
    }

    private float PointDist(Vector2 ptNew, Vector2 ptOld)
    {
        return Math.Abs(ptNew[0] - ptOld[0]) + Math.Abs(ptNew[1] - ptOld[1]);
    }
}

public class ImeDelegateImpl_kbd : ImeDelegateBase

{
    //------------------Ilyas-------------------start
    public enum HandControl
    {
        LeftHand,
        RightHand,
    }

    public HandControl m_DomainHand;

    public XRRayInteractor LeftHandInteractor;
    public XRRayInteractor RightHandInteractor;

    private XRRayInteractor CurrentRayInteractor;

    private XRNode currentnode;

    //------------------Ilyas------------------end
    public Text mText;
    public GameObject[] mKbdViews;
    public SGViewGather mKbdView;
    public ImeManager mManager;
    private Texture2D mTexture;
    private Vector2 mTextureSize = new Vector2(780, 390);
    private Vector2 mPtKbd = new Vector2();
    private SGMouseTracker mTracker = new SGMouseTracker();


    //public Text Debugtext;
    public override void OnIMEShow(Vector2 vSize)
    {
        Debug.Log("OnIMEShow");
        mTextureSize = vSize;
        CreateTexture(vSize);
        mManager.Draw(mTexture);
        mKbdView.SetActive(true);
    }

    public override void OnIMEHide()
    {
        Debug.Log("OnIMEHide");
        mKbdView.SetActive(false);
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
        mKbdView = new SGViewGather(mKbdViews);
        //Ilyas7/26
        GetCurrentNode();
        // CreateTexture(mTextureSize);
#if UNITY_EDITOR
#else
        mKbdView.SetActive(false);
#endif
    }

    void GetCurrentNode()
    {
        //-------------------------------Duke/7/19----------------start-----------------
        //Check if the device is Pico G2 series.The default controller is recognized as left controller on G2 devices.
        string equipment_model = SystemInfo.deviceModel;
        if (equipment_model.Contains("G2"))
        {
            //Ilyas/7/26: If the device is G2 set tracking origin as Device.
            InputDevices.GetDeviceAtXRNode(XRNode.Head).subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Device);
            m_DomainHand = HandControl.LeftHand;
        }
        else
        {
            InputDevices.GetDeviceAtXRNode(XRNode.Head).subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
        }

        //-------------------------------Duke/7/19-----------------end------------------
        //-------------------------------Ilyas/4/23---------------start-----------------
        if (m_DomainHand == HandControl.LeftHand)
        {
            CurrentRayInteractor = LeftHandInteractor;
            currentnode = XRNode.LeftHand;
        }
        else if (m_DomainHand == HandControl.RightHand)
        {
            CurrentRayInteractor = RightHandInteractor;
            currentnode = XRNode.RightHand;
        }
        //-------------------------------Ilyas/4/23---------------end-------------------
    }

    //Ilyas:Refresh Main controller
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            GetCurrentNode();
        }
    }


    void Update()
    {
        if (mTexture != null)
        {
            mManager.Draw(mTexture);
        }

        CheckMouseEvent();
    }

    //other
    private void CreateTexture(Vector2 vSize)
    {
        if (mTexture)
        {
            return;
        }
#if UNITY_EDITOR
#else
        // Create a texture
        mTexture = new Texture2D((int)vSize.x, (int)vSize.y, TextureFormat.RGBA32, false);
        // Set point filtering just so we can see the pixels clearly
        mTexture.filterMode = FilterMode.Trilinear;
        // Call Apply() so it's actually uploaded to the GPU
        mTexture.Apply();

        Debug.Log("texture created");

        // Set texture onto cube
        //GameObject cube = GameObject.Find("Cube");
        //cube.GetComponent<Renderer>().material.mainTexture = mTexture;

        // Set texture onto kbdview
        mKbdView.SetTexture(mTexture);
#endif
    }

    private void DispatchMessageToAndroid(SGImeMotionEventType type, Vector2 pt)
    {
        if (null != mManager)
        {
            mManager.OnTouch(pt.x, pt.y, type);
        }
    }

    private void LogEvent(string prefix, PointerEventData eventData)
    {
        Debug.Log(prefix + ": " + eventData.pointerCurrentRaycast.gameObject.name + " x=" + eventData.position.x + ",y=" + eventData.position.y);
    }

    private void CheckMouseEvent()
    {
        if (Point2UV(Input.mousePosition, ref mPtKbd))
        {
            //-------------------------------Ilyas/4/23---------------start-----------------
            bool inputvalue;
            InputDevices.GetDeviceAtXRNode(currentnode).TryGetFeatureValue(CommonUsages.triggerButton, out inputvalue);

            // DebugHelper.instance.Log("MTracker_outside+"+inputvalue.ToString());
            if (mTracker.Track(mPtKbd, inputvalue))
            {
                //DebugHelper.instance.Log("MTracker_inside");
                // mText.text = "点击成功";
                DispatchMessageToAndroid(mTracker.GetEvent(), mTracker.GetPoint());
            }
            //-------------------------------Ilyas/4/23---------------end-----------------
        }
        else if (mTracker.TrackOuter())
        {
            DispatchMessageToAndroid(mTracker.GetEvent(), mTracker.GetPoint());
        }
    }


    private bool Point2UV(Vector3 ptScreen, ref Vector2 ptUV)
    {
        Ray ray = new Ray();
        bool bRes = false;

        //Controller Control mode---------Ilyas-----------start
        RaycastHit raycastHit;
        if (CurrentRayInteractor.GetCurrentRaycastHit(out raycastHit))
        {
            string name = raycastHit.collider.gameObject.name;
            if (mKbdView.FindName(name))
            {
                GameObject kbd = raycastHit.collider.gameObject;
                Vector3 vecKbd = kbd.transform.InverseTransformPoint(raycastHit.point);
                Vector2 pixelUV = raycastHit.textureCoord;
                Renderer rend = raycastHit.transform.GetComponent<Renderer>();
                Texture2D tex = rend.material.mainTexture as Texture2D;
                ptUV.x = pixelUV.x * mTextureSize.x;
                ptUV.y = (1 - pixelUV.y) * mTextureSize.y;
                // Debug.Log("ray click " + name + ": 3d point=" + vecKbd.ToString() + " uv=(" + pixelUV.x + "," + pixelUV.y + ") org=(" + ptUV.ToString() + ")");
                bRes = true;
            }
        }

        //Controller Control mode---------Ilyas-----------end


        // Ray ray = Camera.main.ScreenPointToRay(ptScreen);
        /*
        if (HeadSetController.activeSelf)//Head control
        {
            //-------------------------------Ilyas/4/23---------------start-----------------
            //HeadSetController.transform.parent.localRotation = Quaternion.Euler(Pvr_UnitySDKManager.SDK.HeadPose.Orientation.eulerAngles.x, Pvr_UnitySDKManager.SDK.HeadPose.Orientation.eulerAngles.y, 0);
            //-------------------------------Ilyas/4/23---------------end-----------------
            ray.direction = HeadSetController.transform.position - HeadSetController.transform.parent.parent.Find("Head").position;
            ray.origin = HeadSetController.transform.parent.parent.Find("Head").position;
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                string name = hitInfo.collider.gameObject.name;
                if (mKbdView.FindName(name))
                {
                    GameObject kbd = hitInfo.collider.gameObject;
                    Vector3 vecKbd = kbd.transform.InverseTransformPoint(hitInfo.point);
                    Vector2 pixelUV = hitInfo.textureCoord;
                    Renderer rend = hitInfo.transform.GetComponent<Renderer>();
                    Texture2D tex = rend.material.mainTexture as Texture2D;
                    ptUV.x = pixelUV.x * mTextureSize.x;
                    ptUV.y = (1 - pixelUV.y) * mTextureSize.y;
                   // Debug.Log("ray click " + name + ": 3d point=" + vecKbd.ToString() + " uv=(" + pixelUV.x + "," + pixelUV.y + ") org=(" + ptUV.ToString() + ")");
                    bRes = true;
                }
            }
        }

      else if (currentController != null)//Controller
        {
            ray.direction = currentController.transform.Find("dot").position - currentController.transform.Find("start").position;
            ray.origin = currentController.transform.Find("start").position;
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                string name = hitInfo.collider.gameObject.name;
                Debug.Log(111 + name);
                if (mKbdView.FindName(name))
                {
                    GameObject kbd = hitInfo.collider.gameObject;
                    Vector3 vecKbd = kbd.transform.InverseTransformPoint(hitInfo.point);
                    Vector2 pixelUV = hitInfo.textureCoord;
                    Renderer rend = hitInfo.transform.GetComponent<Renderer>();
                    Texture2D tex = rend.material.mainTexture as Texture2D;
                    ptUV.x = pixelUV.x * mTextureSize.x;
                    ptUV.y = (1 - pixelUV.y) * mTextureSize.y;
                    //Debug.Log("ray click " + name + ": 3d point=" + vecKbd.ToString() + " uv=(" + pixelUV.x + "," + pixelUV.y + ") org=(" + ptUV.ToString() + ")" + " w=" + texSize.x + ",h=" + texSize.y);
                    bRes = true;
                }
            }
        }*/
        return bRes;
    }

    private void ServiceStartSuccess()
    {
        //-------------------------------Ilyas/4/23---------------start-----------------
        /*
        if (Pvr_UnitySDKAPI.Controller.UPvr_GetMainHandNess() == 0)
        {
            currentController = controller0;
        }
        if (Pvr_UnitySDKAPI.Controller.UPvr_GetMainHandNess() == 1)
        {
            currentController = controller1;
        }*/
        //-------------------------------Ilyas/4/23---------------end-----------------
    }

    private void ControllerStateListener(string data)
    {
        /*
        if (Pvr_UnitySDKAPI.Controller.UPvr_GetMainHandNess() == 0)
        {
            currentController = controller0;
        }
        if (Pvr_UnitySDKAPI.Controller.UPvr_GetMainHandNess() == 1)
        {
            currentController = controller1;
        }*/
    }
}