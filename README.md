# VUSharp Spectate System

A simple spectating system for Udon VRC Worlds that supports per-player custom tracking and default behavior

  

## Features

- Simple spectating system that can manage itself with minimal setup required

- The ability to have manual control over the system

- The ability to code your own camera tracking behavior that the system will use

- The ability to assign per-player camera tracking behaviors and/or buttons

  

## How to install

### VRChat Package Manager

<https://vincil-vr.github.io/VPM-Listings/>

  

### Unity Package Manager

In your Unity project, go to `Window > Package Manager` then click the top left `+`, click on `Add package from git URL` and paste this link: 

  

<https://github.com/Vincil-VR/VUSharp-Spectate-System/tree/main/Packages/com.vincil.vusharp.spectate>

  

### Unity Package

Download the latest package from the [latest release](https://github.com/Vincil-VR/VUSharp-Spectate-System/releases/latest)

  

Then import the contained .unitypackage

  

## Simple Setup

  

1. In your project navigate to `Packages > VUSharp Spectate System > Runtime > Prefabs` and then click and drag the `SpectateSystem` prefab into your scene.

  > Import `TMP Essential Resources` if prompted.

2. Position the `SpectateUI` display to a position of your choosing

3. Position and resize the `AutoManageTrigger` box collider until it encompasses whatever lobby/room/area non-spectatable players will reside in.  Players ***within*** the box collider will ***not*** be able to be spectated.  Players ***outside*** the box collider ***will*** be able to be spectated.

  

## Manual System Management

  

> **NOTE:** Make sure to un-checkmark `Auto Manage` on the `Spectate UI` behavior inspector if you want to manually manage it.

  

All the public methods by which you can interact with are contained in the abstract  `SpectateSystem` class.  If you are only using a single `SpectateUI` in your scene and it has the expected hierarchy path of `/SpectateSystem/SpectateUI` you can use the static method `SpectateSystem.Instance()` to get that instance. 

  

### AddPlayer(VRCPlayerApi player)

  

Adds a player to the spectate UI using the default button prefab set in the `defaultButtonPrefab` inspector field.

  

### AddPlayer(VRCPlayerApi player, GameObject buttonPrefab)

  

Adds a player to the spectate UI using the specified button prefab.

  
  

### RemovePlayer(VRCPlayerApi player)

  

Removes the player from the spectate UI.

  

### RemoveAllPlayers()

  

Removes all players from the spectate UI.

  

### SetSpectatingEnabled(bool value)

  

Sets if spectating is enabled.  True enables spectating, false disables it.

  

## Button Prefabs

  

Buttons are instantiated when a new player is added to the spectate system and not only serve a visual purpose but also contain the camera tracking behavior for that player. .  

  

You can make your own button prefabs but they must contain two U# behavior components: `SpectatePlayerButton` and a behavior that inherits from the `SpectateTrackingMethod` abstract class.

  

You can look at the `SpectatePlayerButtonPrefab` button prefab contained in `VUSharp Spectate System > Runtime > Prefabs > Buttons` for an example of a valid button.

  

## Per-Player Custom Camera Tracking

  

The camera tracking behavior to be used when spectating a player is provided via the `SpectateTrackingMethod` behavior attached to that player's button.  `SpectateTrackingMethod` is an abstract class that can be freely inherited from that has the following required methods:

  

### public void SetPlayerAndCamera(VRCPlayerApi player, Camera spectateCamera)

  

Called when the system provides the target player and the camera used for spectating. Use this to set these values internally.

  

### public void StartTracking()

  

Called when spectating of the target player starts. Use this to start tracking the target player.

  

### public abstract void StopTracking()

  

Called when spectating of the target player stops. Use this to stop tracking the target player.  Note: this might get called again when already stopped.

### Example:
```c#
using UdonSharp;
using UnityEngine;
using Vincil.VUSharp.Spectate;
using VRC.SDKBase;
using VRC.Udon.Common.Enums;

namespace Vincil.VUSharp.Example
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectateTrackingMethodExample : SpectateTrackingMethod
    {
        // Variables
        VRCPlayerApi targetPlayer;
        Camera spectateCamera;
        bool isCurrentlyTracking;

        // Spectate System Event Methods
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

        // Interal Methods

        /// <summary>
        /// Internally used tracking loop to update the spectate camera position and rotation to match the target player.  Public only as a requirement of SendCustomEvent.
        /// </summary>
        public void _TrackingLoop()
        {
            if (!isCurrentlyTracking) return;

            spectateCamera.transform.SetPositionAndRotation(targetPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position, targetPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation);

            SendCustomEventDelayedFrames(nameof(_TrackingLoop), 0, EventTiming.LateUpdate);
        }
    }
}
```
