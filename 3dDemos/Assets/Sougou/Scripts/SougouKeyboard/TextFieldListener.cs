using UnityEngine;
using UnityEngine.UI;

namespace Sougou.Scripts.SougouKeyboard
{
    public class TextFieldListener : MonoBehaviour
    {
        private void Update()
        {
            var a = FindObjectsOfType<InputField>();
            foreach (var i in a)
            {
                var obj = i.gameObject;
                if (obj.GetComponent<SougouInputField>() == null)
                {
                    obj.AddComponent<SougouInputField>();
                }
                Destroy(i); //删除InputField属性
            }
        }
    }
}