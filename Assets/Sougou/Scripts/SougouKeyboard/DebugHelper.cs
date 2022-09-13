using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugHelper : MonoBehaviour
{
    [SerializeField]
    public Text DebugText;
    public static DebugHelper instance;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }
    void Start()
    {
        
        
    }

    public void Log(string MSG)
    {
        if (DebugText)
        {
            Debug.Log("DebugHelper+"+MSG);
            DebugText.text = MSG;
        }
    }
}
