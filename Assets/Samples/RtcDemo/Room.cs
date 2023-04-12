using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples.RtcDemo
{
    class Room : MonoBehaviour
    {
        public string id;
        public Button leaveRoomButton;
        public Toggle publishToggle;
        public Text textRoomId;
        public Button destroyRoomButton;
        public Toggle toggleRemoteAudio;
        public TMP_Text textRoomStats;
    }
}