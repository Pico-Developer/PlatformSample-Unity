using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SougouInputField : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private ImeManager mManager;
    private ImeDelegateImpl_kbd keyboard;
    public SGImeInputType mInputType = SGImeInputType.TYPE_CLASS_TEXT;

    public SGImeTextType mTextType = SGImeTextType.TYPE_TEXT_VARIATION_NORMAL;

    // Start is called before the first frame update
    void Awake()
    {
        mManager = GameObject.Find("Sougou/ImeManager").GetComponent<ImeManager>();
        keyboard = GameObject.Find("Sougou/KeyboardLayout").GetComponent<ImeDelegateImpl_kbd>();
    }

    void Update()
    {
        var placeholder = transform.GetChild(0).gameObject;
        var text = transform.GetChild(1).gameObject;
        var textText = text.GetComponent<Text>();
        if (textText.text.Length > 0)
        {
            placeholder.SetActive(false);
        }
        else
        {
            placeholder.SetActive(true);
        }
    }

    // Update is called once per frame
    //OnPointerDown is also required to receive OnPointerUp callbacks
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("pointer down");
        keyboard.mText = transform.GetChild(1).gameObject.GetComponent<Text>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LogEvent("click text", eventData);
        mManager.Show(mInputType, mTextType);
    }

    private void LogEvent(string prefix, PointerEventData eventData)
    {
        Debug.Log(prefix + ": " + eventData.pointerCurrentRaycast.gameObject.name + " x=" + eventData.position.x + ",y=" + eventData.position.y);
    }

    public string text
    {
        get
        {
            var text = transform.GetChild(1).gameObject;
            var t = text.GetComponent<Text>();
            return t.text;
        }
        set
        {
            var text = transform.GetChild(1).gameObject;
            var t = text.GetComponent<Text>();
            t.text = value;
        }
    }
}