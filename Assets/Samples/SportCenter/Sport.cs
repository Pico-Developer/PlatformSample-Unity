﻿using Newtonsoft.Json;
using Pico.Platform.Models;
using PICO.Platform.Samples;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples.SportCenter
{
    public class Sport : MonoBehaviour
    {
        void Start()
        {
            InitUtil.Initialize();
            UserService.RequestUserPermissions(Permissions.SportsSummaryData, Permissions.SportsUserInfo).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Debug.Log($"RequestUserPermission failed {m.Error}");
                    return;
                }

                Debug.Log($"Request user permission successfully.");
            });
            //sliders
            SliderText detailsBegin = GameObject.Find("SliderTextSummaryStart").GetComponent<SliderText>();
            SliderText detailsEnd = GameObject.Find("SliderTextSummaryEnd").GetComponent<SliderText>();
            SliderText dailySummaryBegin = GameObject.Find("SliderTextDailySummaryStart").GetComponent<SliderText>();
            SliderText dailySummaryEnd = GameObject.Find("SliderTextDailySummaryEnd").GetComponent<SliderText>();

            //buttons
            Button buttonGetUserInfo = GameObject.Find("ButtonGetUserInfo").GetComponent<Button>();
            Button buttonGetSummary = GameObject.Find("ButtonGetSummary").GetComponent<Button>();
            Button buttonGetDailySummary = GameObject.Find("ButtonGetDailySummary").GetComponent<Button>();

            buttonGetUserInfo.onClick.AddListener(() =>
            {
                SportService.GetUserInfo().OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        Debug.LogError($"GetUserInfo failed: code={msg.Error.Code},message={msg.Error.Message}");
                        return;
                    }

                    Log("GetUserInfo successfully");
                    Log(JsonConvert.SerializeObject(msg.Data));
                });
            });
            buttonGetSummary.onClick.AddListener(() =>
            {
                var begin = detailsBegin.value;
                var end = detailsEnd.value;
                if (begin > end)
                {
                    (begin, end) = (end, begin);
                }

                Log($"GetSummary begin :{begin} {end}");
                SportService.GetSummary(begin, end).OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        Debug.LogError($"Get summary failed:code={msg.Error.Code} message={msg.Error.Message}");
                        return;
                    }

                    Log($"Get Summary Successfully");
                    Log(JsonConvert.SerializeObject(msg.Data));
                });
            });
            buttonGetDailySummary.onClick.AddListener(() =>
            {
                var begin = dailySummaryBegin.value;
                var end = dailySummaryEnd.value;
                if (begin > end)
                {
                    (begin, end) = (end, begin);
                }

                Debug.Log($"GetDailySummary:{begin}-{end}");
                SportService.GetDailySummary(begin, end).OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        Debug.LogError($"GetDailySummary failed:code={msg.Error.Code},message={msg.Error.Message}");
                        return;
                    }

                    Log($"Get daily summary successfully :{msg.Data.Count}");
                    for (var i = 0; i < msg.Data.Count; i++)
                    {
                        Log($"{i} content={JsonConvert.SerializeObject(msg.Data[i])}");
                    }
                });
            });
        }

        void Log(string s)
        {
            print(s);
        }
    }
}