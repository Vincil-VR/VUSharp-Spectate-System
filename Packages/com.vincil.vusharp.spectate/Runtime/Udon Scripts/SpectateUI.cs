
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;

namespace Vincil.VUSharp.Spectate
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectateUI : SpectateSystem
    {
        #region Variables

        #region Inspector Fields
        [Header("Spectate UI Settings")]

        [Tooltip("If enabled, the spectate system will automatically manage adding and removing players from the spectate UI.")]
        [SerializeField] private bool autoManage = true;

        [Tooltip("If enabled, players will be automatically removed from the spectate UI when they leave the instance.  This setting has no affect if Auto Manage is enabled.")]
        [SerializeField] private bool autoRemoveLeavingPlayers = true;

        [Tooltip("Whether the local player will be excluded from the spectator system. Set to false if you want players to be able to spectate themselves.")]
        [SerializeField] private bool excludeLocalPlayer = true;

        [Header("References")]
        [Tooltip("The default button prefab to use when adding players to the spectate UI.")]
        [SerializeField] private GameObject defaultButtonPrefab;
        [SerializeField] private Camera spectateCamera;
        [SerializeField] private GameObject autoManageNonSpectatedZone;

        [SerializeField] private GameObject disabledWindow;
        [SerializeField] private GameObject enabledWindow;
        [SerializeField] private GameObject spectateScreen;
        [SerializeField] private GameObject scrollContentPlayerList;
        #endregion

        #region Private Fields
        DataDictionary playerButtonsByID = new DataDictionary();

        SpectateTrackingMethod currentTrackingMethodInUse;

        bool isSpectatingEnabled = true;
        bool isWantingToEnableSpectatingUI = false;
        #endregion
        #endregion

        void Start()
        {
            autoManageNonSpectatedZone.SetActive(autoManage);
        }

        #region Public Methods
        public override void AddPlayer(VRCPlayerApi player, GameObject buttonPrefab)
        {
            if (autoManage)
            {
                Debug.LogWarning($"Manually adding players to spectate screen while system is auto managed");
            }
            AddPlayerInternal(player, buttonPrefab);
        }

        public override void AddPlayer(VRCPlayerApi player)
        {
            if (autoManage)
            {
                Debug.LogWarning($"Manually adding players to spectate screen while system is auto managed");
            }
            AddPlayerInternal(player);
        }

        public override void RemovePlayer(VRCPlayerApi player)
        {
            if (autoManage)
            {
                Debug.LogWarning($"Manually removing players from spectate screen while system is auto managed");
            }
            RemovePlayerInternal(player);
        }

        public override void RemoveAllPlayers()
        {
            if (autoManage)
            {
                Debug.LogWarning($"Manually clearing all players from spectate screen while system is auto managed");
            }
            RemoveAllPlayersInternal();
        }

        public override void SetSpectatingEnabled(bool value)
        {
            isSpectatingEnabled = value;
            if (isSpectatingEnabled && isWantingToEnableSpectatingUI)
            {
                TurnOnSpectatingUI();
            }
            else
            {
                TurnOffSpectatingUI();
            }
        }
        #endregion

        #region Internal Methods

        #region Player Management
        private void AddPlayerInternal(VRCPlayerApi player, GameObject buttonPrefab)
        {
            if (excludeLocalPlayer && player.isLocal) return;
            GameObject buttonGO = Instantiate(buttonPrefab, scrollContentPlayerList.transform);
            SpectatePlayerButton button = buttonGO.GetComponent<SpectatePlayerButton>();
            playerButtonsByID.SetValue(player.playerId, button);
            button.Setup(player, spectateCamera, this);
        }
        
        internal void AddPlayerInternal(VRCPlayerApi player)
        {
            AddPlayerInternal(player, defaultButtonPrefab);
        }

        internal void RemovePlayerInternal(VRCPlayerApi player)
        {
            if (!playerButtonsByID.ContainsKey(player.playerId)) return;
            if (playerButtonsByID.TryGetValue(player.playerId, TokenType.Reference, out DataToken token))
            {
                SpectatePlayerButton button = (SpectatePlayerButton)token.Reference;
                if (button.GetTrackingMethod() == currentTrackingMethodInUse)
                {
                    spectateScreen.SetActive(false);
                    spectateCamera.enabled = false;
                    currentTrackingMethodInUse = null;
                }
                Destroy(button.gameObject);
                playerButtonsByID.Remove(player.playerId);
            }
            else
            {
                Debug.LogError($"Error trying to remove player button: {token.Error}");
            }
        }        

        private void RemoveAllPlayersInternal()
        {
            DataList buttons = playerButtonsByID.GetValues();
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(((SpectatePlayerButton)buttons[i].Reference).gameObject);
            }
            playerButtonsByID.Clear();

            spectateScreen.SetActive(false);
            spectateCamera.enabled = false;
            currentTrackingMethodInUse = null;
        }

        #endregion

        #region Spectating Management
        internal void StartNewSpectating(SpectateTrackingMethod trackingMethod)
        {
            if (trackingMethod == currentTrackingMethodInUse) return;

            spectateScreen.SetActive(true);
            spectateCamera.enabled = true;
            if (currentTrackingMethodInUse != null)
            {
                currentTrackingMethodInUse.StopTracking();
            }
            trackingMethod.StartTracking();
            currentTrackingMethodInUse = trackingMethod;
        }

        internal void RequestTurnOnSpectatingUI()
        {
            isWantingToEnableSpectatingUI = true;
            if (isSpectatingEnabled)
            {
                TurnOnSpectatingUI();
            }
        }

        internal void RequestTurnOffSpectatingUI()
        {
            isWantingToEnableSpectatingUI = false;
            TurnOffSpectatingUI();
        }

        private void TurnOnSpectatingUI()
        {
            disabledWindow.SetActive(false);
            enabledWindow.SetActive(true);
            if (currentTrackingMethodInUse != null)
            {
                spectateScreen.SetActive(true);
                spectateCamera.enabled = true;
                currentTrackingMethodInUse.StartTracking();
            }
        }
        
        private void TurnOffSpectatingUI()
        {
            disabledWindow.SetActive(true);
            enabledWindow.SetActive(false);
            spectateScreen.SetActive(false);
            spectateCamera.enabled = false;
            if (currentTrackingMethodInUse != null)
            {
                currentTrackingMethodInUse.StopTracking();
            }
        }
        #endregion

        #region Udon Events
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (autoManage || autoRemoveLeavingPlayers)
            {
                RemovePlayerInternal(player);
            }
        }
        #endregion

        #endregion
    }
}