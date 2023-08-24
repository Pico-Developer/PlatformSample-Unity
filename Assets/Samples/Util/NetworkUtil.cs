using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Pico.Platform.Samples
{
    public static class NetworkUtils
    {
        static HttpClient cli = new HttpClient();

        public static string post(string url, JObject data)
        {
            return post(url, data.ToString());
        }

        public static void post(string url, JObject data, Action<string> callback)
        {
            post(url, JsonConvert.SerializeObject(data), callback);
        }

        public static string get(string url)
        {
            var resp = cli.GetAsync(url);
            resp.Wait();
            var sTask = resp.Result.Content.ReadAsStringAsync();
            sTask.Wait();
            var s = sTask.Result;
            Debug.Log($"[NetworkUtils]result= {s}");
            return s;
        }

        public static void get(string url, Action<string> callback)
        {
            var resp = cli.GetAsync(url);
            resp.ContinueWith(response =>
            {
                response.Result.Content.ReadAsStringAsync().ContinueWith(respContent =>
                {
                    var content = respContent.Result;
                    Debug.Log($"[NetworkUtils]result for {url} is {content}");
                    callback(content);
                });
            });
        }

        public static string post(string url, string data)
        {
            var content = new StringContent(data, Encoding.UTF8);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var resp = cli.PostAsync(url, content);
            resp.Wait();
            var sTask = resp.Result.Content.ReadAsStringAsync();
            sTask.Wait();
            var s = sTask.Result;
            Debug.Log($"[NetworkUtils]result= {s}");
            return s;
        }

        public static void post(string url, string data, Action<string> callback)
        {
            var content = new StringContent(data, Encoding.UTF8);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var resp = cli.PostAsync(url, content);
            resp.ContinueWith(response =>
            {
                var result = resp.Result.Content.ReadAsStringAsync();
                var s = result.Result;
                Debug.Log($"[NetworkUtils]result= {s}");
                callback(s);
            });
        }


        public static IEnumerator BindImage(string mediaUrl, RawImage rawImage)
        {
            if (rawImage == null)
            {
                Debug.Log("BindImage rawImage is null");
                yield break;
            }

            if (string.IsNullOrWhiteSpace(mediaUrl))
            {
                Debug.Log($"mediaUrl is empty");
                yield break;
            }

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
            yield return request.SendWebRequest();
            if (request.responseCode != 200)
            {
                Debug.Log("Load image failed");
            }
            else
            {
                if (rawImage != null)
                {
                    rawImage.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                    var renderer = rawImage.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.mainTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                    }
                }
            }

            yield return null;
        }
    }
}