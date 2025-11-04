
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Vincil.VUSharp.Spectate
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AutoManageTrigger : UdonSharpBehaviour
    {
        [SerializeField] private SpectateUI spectateUI;
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            spectateUI.RemovePlayerInternal(player);
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            spectateUI.AddPlayerInternal(player);
        }
    }
}