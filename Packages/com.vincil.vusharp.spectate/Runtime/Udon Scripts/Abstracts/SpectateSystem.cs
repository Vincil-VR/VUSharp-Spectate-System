
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Vincil.VUSharp.Spectate
{
    /// <summary>
    /// Abstract system for spectating functionality in VRChat worlds.
    /// </summary>
    public abstract class SpectateSystem : UdonSharpBehaviour
    {
        /// <summary>
        /// Looks for the spectate system at its expected location and returns it if found.
        /// 
        /// Search location: /SpectateSystem/SpectateUI
        /// </summary>
        public static SpectateSystem Instance()
        {
            SpectateSystem behaviour = null;
            GameObject objGO = GameObject.Find("/SpectateSystem/SpectateUI");
            if (objGO != null)
            {
                behaviour = objGO.GetComponent<SpectateSystem>();
                if (behaviour == null)
                {
                    Debug.LogError("Failed to find Spectate System Behaviour!");
                }
            }
            else
            {
                Debug.LogError("Failed to find Spectate System GameObject!");
            }
            return behaviour;
        }

        /// <summary>
        /// Adds a player to the spectate UI using the specified button prefab.
        /// </summary>
        /// <param name="player">VRCPlayerApi of the player to be added</param>
        /// <param name="buttonPrefab">Button prefab to be used for player</param>
        public abstract void AddPlayer(VRCPlayerApi player, GameObject buttonPrefab);

        /// <summary>
        /// Adds a player to the spectate UI using the default button prefab.
        /// </summary>
        /// <param name="player">VRCPlayerApi of the player to be added</param>
        public abstract void AddPlayer(VRCPlayerApi player);

        /// <summary>
        /// Removes the player from the spectate UI.
        /// </summary>
        /// <param name="player">VRCPlayerApi of the player to be removed</param>
        public abstract void RemovePlayer(VRCPlayerApi player);

        /// <summary>
        /// Removes all players from the spectate UI.
        /// </summary>
        public abstract void RemoveAllPlayers();

        /// <summary>
        /// Sets if spectating is enabled.
        /// </summary>
        /// <param name="value">bool: true enables spectating, false disables it</param>
        public abstract void SetSpectatingEnabled(bool value);
    }
}
