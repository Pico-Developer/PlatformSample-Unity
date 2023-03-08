using System;
using System.Collections;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Pico.Platform.Samples
{
    public class AsyncSimpleDemo : MonoBehaviour
    {
        public RawImage headImage;
        public Text nameText;
        public Text logText;

        // Start is called before the first frame update
        async void Start()
        {
            var initMsg = await CoreService.AsyncInitialize().Async();
            if (initMsg.IsError)
            {
                Log($"Async initialize failed: code={initMsg.Error.Code} message={initMsg.Error.Message}");
                return;
            }

            if (initMsg.Data != PlatformInitializeResult.Success && initMsg.Data != PlatformInitializeResult.AlreadyInitialized)
            {
                Log($"Async initialize failed: result={initMsg.Data}");
                return;
            }

            Log("AsyncInitialize Successfully");
            var permissionMessage = await UserService.RequestUserPermissions(new[] {Permissions.UserInfo, Permissions.FriendRelation}).Async();
            if (permissionMessage.IsError)
            {
                Log($"Permission failed code={permissionMessage.Error.Code} message={permissionMessage.Error.Message}");
                return;
            }

            Log($"RequestUserPermissions successfully:{String.Join(",", permissionMessage.Data.AuthorizedPermissions)}");
            var userMessage = await UserService.GetLoggedInUser().Async();
            if (userMessage.IsError)
            {
                Debug.Log($"GetLoggedInUser failed:code={userMessage.Error.Code} message={userMessage.Error.Message}");
                return;
            }

            StartCoroutine(DownloadImage(userMessage.Data.ImageUrl, headImage));
            nameText.text = userMessage.Data.DisplayName;
            Log($"DisplayName={userMessage.Data.DisplayName} UserId={userMessage.Data.ID}");
        }

        IEnumerator DownloadImage(string mediaUrl, RawImage rawImage)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
            yield return request.SendWebRequest();
            if (request.responseCode != 200)
            {
                Log("Load image failed");
            }
            else
            {
                rawImage.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                rawImage.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            }
        }

        void Log(string s)
        {
            logText.text = s;
            Debug.Log(s);
        }
    }
}