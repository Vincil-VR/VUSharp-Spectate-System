
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Vincil.VUSharp.Spectate
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectateToggleTrigger : UdonSharpBehaviour
    {
        [SerializeField] SpectateUI spectateUI;
        
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            spectateUI.RequestTurnOnSpectatingUI();
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            spectateUI.RequestTurnOffSpectatingUI();
        }
    }
}
