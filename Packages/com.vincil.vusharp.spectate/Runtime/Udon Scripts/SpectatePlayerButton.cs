
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Vincil.VUSharp.Spectate
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectatePlayerButton : UdonSharpBehaviour
    {
        [SerializeField] private TextMeshProUGUI textField;

        SpectateUI spectateUI;
        //VRCPlayerApi player;
        SpectateTrackingMethod trackingMethod;

        internal void Setup(VRCPlayerApi player, Camera spectateCamera, SpectateUI spectateUI)
        {
            trackingMethod = GetComponent<SpectateTrackingMethod>();
            trackingMethod.SetPlayerAndCamera(player, spectateCamera);
            this.spectateUI = spectateUI;

            textField.text = player.displayName;
        }

        internal SpectateTrackingMethod GetTrackingMethod()
        {
            return trackingMethod;
        }

        /// <summary>
        /// Internally used to notify the behaviour of a button press.  Public only as a requirement of SendCustomEvent.
        /// </summary>
        public void OnButtonClicked()
        {
            if(trackingMethod == null)
            {
                Debug.LogError("[Spectate System Button] TrackingMethod is null", this);
                return;
            }
            if (spectateUI == null)
            {
                Debug.LogError("[Spectate System Button] SpectateUI is null", this);
                return;
            }
            spectateUI.StartNewSpectating(trackingMethod);
        }
    }
}


