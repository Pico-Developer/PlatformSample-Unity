using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples.Game
{
    public class CloudStorageSample : MonoBehaviour
    {
        public Text debugText;
        public void StartNewBackup()
        {
            string debugMsg= "";
            debugMsg += "\nStartNewBackup Start";

            CloudStorageService.StartNewBackup().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    debugMsg += "\nStartNewBackup IsError, ErrorMessage:" + msg.Error.Message + " ErrorCode:" + msg.Error.Code;
                }
                else
                {
                    debugMsg = "\nStartNewBackup Successfully";
                }

                Debug.Log(debugMsg);
                debugText.text = debugMsg;
            });

            Debug.Log(debugMsg);
            debugText.text = debugMsg;
        }
    }
}