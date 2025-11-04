using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Vincil.VUSharp.Spectate
{
    /// <summary>
    /// Abstract class used by the spectating system to perform tracking with.
    /// </summary>
    public abstract class SpectateTrackingMethod : UdonSharpBehaviour
    {
        /// <summary>
        /// Called when the system provides the target player and the camera used for spectating. Use this to set these values internally.
        /// </summary>
        /// <param name="player">VRCPlayerApi of the player that is the target of spectating</param>
        /// <param name="spectateCamera">Camera used for spectating</param>
        public abstract void SetPlayerAndCamera(VRCPlayerApi player, Camera spectateCamera);

        /// <summary>
        /// Called when spectating of target player starts. Use this to start tracking the target player.
        /// </summary>
        public abstract void StartTracking();

        /// <summary>
        /// Called when spectating of target player stops. Use this to stop tracking the target player.  Note: this might get called again when already stopped.
        /// </summary>
        public abstract void StopTracking();
    }
}
