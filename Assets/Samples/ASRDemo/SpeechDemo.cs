using Newtonsoft.Json;
using Pico.Platform;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class SpeechDemo : MonoBehaviour
{
    public Button buttonInit;

    public Button buttonStart;
    
    public Button buttonStop;

    public TMP_Text textAsrResult;

    public Toggle toggleAutoStop;

    public Toggle toggleShowPunctual;

    public TMP_InputField inputMaxDuration;

    private bool inited = false;

    // Start is called before the first frame update
    void Start()
    {
        CoreService.Initialize();
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        SpeechService.SetOnAsrResultCallback(msg =>
        {
            var m = msg.Data;
            Debug.Log($"text={m.Text} isFinal={m.IsFinalResult}");
            textAsrResult.SetText($"[{m.IsFinalResult}]{m.Text}");
        });
        SpeechService.SetOnSpeechErrorCallback(msg =>
        {
            var m = msg.Data;
            Debug.Log($"SpeechError :{JsonConvert.SerializeObject(m)}");
        });
        buttonInit.onClick.AddListener(() =>
        {
            var res = SpeechService.InitAsrEngine();
            if (res != AsrEngineInitResult.Success)
            {
                Debug.Log($"Init ASR Engine failed :{res}");
                textAsrResult.SetText($"init failed {res}");
            }
            else
            {
                inited = true;
                Debug.Log("Init engine successfully.");
                textAsrResult.SetText($"Init successfully");
            }
        });
        buttonStart.onClick.AddListener(() =>
        {
            if (!inited)
            {
                Debug.Log($"Please init before start ASR");
                textAsrResult.SetText("Please init engine first");
                return;
            }

            bool autoStop = toggleAutoStop.isOn;
            bool showPunctual = toggleShowPunctual.isOn;
            int.TryParse(inputMaxDuration.text, out var maxDuration);
            SpeechService.StartAsr(autoStop, showPunctual, maxDuration);
            Debug.Log($"engine started, {autoStop}, {showPunctual}, {maxDuration}");
        });
        buttonStop.onClick.AddListener(() =>
        {
            if (!inited)
            {
                Debug.Log($"Please init before start ASR");
                textAsrResult.SetText("Please init engine first");
                return;
            }

            SpeechService.StopAsr();
            Debug.Log("engine stopped");
        });
    }
}