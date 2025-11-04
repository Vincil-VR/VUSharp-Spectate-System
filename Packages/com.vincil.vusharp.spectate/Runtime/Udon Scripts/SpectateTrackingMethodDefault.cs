
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Enums;

namespace Vincil.VUSharp.Spectate
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectateTrackingMethodDefault : SpectateTrackingMethod
    {
        #region Variables
        VRCPlayerApi targetPlayer;
        Camera spectateCamera;
        bool isCurrentlyTracking;
        #endregion

        #region Spectate System Event Methods
        public override void SetPlayerAndCamera(VRCPlayerApi player, Camera spectateCamera)
        {
            this.targetPlayer = player;
            this.spectateCamera = spectateCamera;
        }

        public override void StartTracking()
        {
            if (!isCurrentlyTracking)
            {
                isCurrentlyTracking = true;
                _TrackingLoop();
            }
        }

        public override void StopTracking()
        {
            isCurrentlyTracking = false;
        }
        #endregion

        #region Interal Methods

        /// <summary>
        /// Internally used tracking loop to update the spectate camera position and rotation to match the target player.  Public only as a requirement of SendCustomEvent.
        /// </summary>
        public void _TrackingLoop()
        {
            if (!isCurrentlyTracking) return;

            spectateCamera.transform.SetPositionAndRotation(targetPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position, targetPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation);

            SendCustomEventDelayedFrames(nameof(_TrackingLoop), 0, EventTiming.LateUpdate);
        }
        #endregion
    }
}
