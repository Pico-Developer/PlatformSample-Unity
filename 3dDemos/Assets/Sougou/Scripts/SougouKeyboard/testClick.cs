using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class testClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("mouse position:" + Input.mousePosition.ToString());
            if (Physics.Raycast(ray, out hitInfo))
            {
                string name = hitInfo.collider.gameObject.name;
                //if (name == "KeyBoard-LAYOUT")
                {
                    GameObject kbd = hitInfo.collider.gameObject;
                    Vector3 vecKbd = kbd.transform.InverseTransformPoint(hitInfo.point);
                    Vector2 pixelUV = hitInfo.textureCoord;
                    Renderer rend = hitInfo.transform.GetComponent<Renderer>();
                    Texture2D tex = rend.material.mainTexture as Texture2D;
                    Vector2 pixelOrg;
                    Vector2 texSize = new Vector2(813,345);
                    pixelOrg.x = pixelUV.x * texSize.x;
                    pixelOrg.y = (1-pixelUV.y) * texSize.y;
                    Debug.Log("ray click " + name + ": 3d point=" + vecKbd.ToString() + " uv=(" + pixelUV.x + "," + pixelUV.y + ") org=(" + pixelOrg.ToString() + ")" + " w="+texSize.x+",h="+texSize.y);
                }
            }
        }
    }

    public void Click()
    {
        Debug.Log("click kbd");
    }

    private void LogEvent(string prefix, PointerEventData eventData)
    {
        Debug.Log(prefix + ": " + eventData.pointerCurrentRaycast.gameObject.name + " x=" + eventData.position.x + ",y=" + eventData.position.y);
    }
}
