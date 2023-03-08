using System;
using System.Text;
using System.Text.RegularExpressions;
using Samples.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples
{
    public delegate void Handler(string[] args);

    public class Fun
    {
        public Handler handler;
        public string desc;
        public string key;

        public Fun(string key, string desc, Handler handler)
        {
            this.key = key;
            this.desc = desc;
            this.handler = handler;
        }
    }

    public abstract class CommandFramework : MonoBehaviour
    {
        public Text dataOutput;
        public Text commandList;
        public InputField inputField;
        public Button executeButton;
        public bool useAsyncInit = true;
        private Fun[] funList;

        void Start()
        {
            Log($"UseAsyncInit={useAsyncInit}");
            if (useAsyncInit)
            {
                try
                {
                    InitUtil.AsyncInitialize().OnComplete(m =>
                    {
                        if (m.IsError)
                        {
                            Log($"Async initialize failed: code={m.GetError().Code} message={m.GetError().Message}");
                            return;
                        }

                        if (m.Data != PlatformInitializeResult.Success && m.Data != PlatformInitializeResult.AlreadyInitialized)
                        {
                            Log($"Async initialize failed: result={m.Data}");
                            return;
                        }

                        this.OnInit();

                        Log("AsyncInitialize Successfully");
                    });
                }
                catch (Exception e)
                {
                    Log($"Async Initialize Failed:{e}");
                    return;
                }
            }
            else
            {
                try
                {
                    InitUtil.Initialize();
                    this.OnInit();
                }
                catch (UnityException e)
                {
                    Log($"Init Platform SDK error:{e}");
                    throw;
                }
            }

            funList = GetFunList();

            StringBuilder s = new StringBuilder();
            foreach (var i in funList)
            {
                s.AppendLine(i.desc);
            }

            commandList.text = s.ToString();
            executeButton.onClick.AddListener(OnButtonClick);
        }

        public abstract Fun[] GetFunList();
        public abstract void OnInit();

        private void OnButtonClick()
        {
            string currentText = inputField.text.Trim();
            if (String.IsNullOrWhiteSpace(currentText))
            {
                return;
            }

            Log($"Got command text {currentText}");
            var args = Regex.Split(currentText, @"\s+");
            if (args.Length == 0)
            {
                Log("Please input a command");
                return;
            }

            var key = args[0];
            var handled = false;
            foreach (var cmd in funList)
            {
                if (cmd.key.Equals(key))
                {
                    try
                    {
                        cmd.handler(args);
                    }
                    catch (Exception e)
                    {
                        Log($"Handle command error:{cmd.desc} e={e}");
                    }

                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                Log($"Cannot find command for :{key}");
            }

            inputField.text = "";
        }

        protected void Log(String content)
        {
            Debug.Log(content);
            dataOutput.text = $"> {content}\n{dataOutput.text}";
            if (dataOutput.text.Length > 1000)
            {
                dataOutput.text = dataOutput.text.Substring(0, 1000);
            }
        }
    }
}