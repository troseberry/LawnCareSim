using Core.Events;
//using LawnCareSim.Camera;
//using LawnCareSim.Inventory;
//using LawnCareSim.Animation;
//using LawnCareSim.Time;
//using LawnCareSim.UI;
using System;
using UnityEngine;
//using AnimationState = LawnCareSim.Animation.AnimationState;
using LawnCareSim.Player;
//using LawnCareSim.Crafting;

namespace LawnCareSim.Events
{
    [CreateAssetMenu(fileName = "EventRelayer", menuName = "Events/EventRelayer")]
    public class EventRelayer : EventRelayerScriptableObject<EventRelayer>
    {
        /*
        #region Animation
        public event EventHandler<AnimationState> AnimationStateStartedEvent;
        public event EventHandler<AnimationState> AnimationStateEndedEvent;
        public event EventHandler<(AnimationState, PlayerState)> AnimationStateChangedEvent;

        public void OnAnimationStateStarted(AnimationState state)
        {
            AnimationStateStartedEvent?.Invoke(this, state);
        }

        public void OnAnimationStateEnded(AnimationState state)
        {
            AnimationStateEndedEvent?.Invoke(this, state);
        }

        public void OnAnimationStateChanged((AnimationState, PlayerState) states)
        {
            AnimationStateChangedEvent?.Invoke(this, states);
        }
        #endregion
        */

        #region Camera
        public struct CameraBlendData
        {
            public CameraName BlendFromCamera;
            public CameraName BlendToCamera;
        }

        public event EventHandler<CameraBlendData> CameraChangeStartedEvent;
        public event EventHandler<CameraBlendData> CameraChangeFinishedEvent;

        public void OnCameraChangeStarted(CameraName blendFromCamera, CameraName blendToCamera)
        {
            var blendData = new CameraBlendData { BlendFromCamera = blendFromCamera, BlendToCamera = blendToCamera };
            CameraChangeStartedEvent?.Invoke(this, blendData);
        }

        public void OnCameraChangeFinished(CameraName blendFromCamera, CameraName blendToCamera)
        {
            var blendData = new CameraBlendData { BlendFromCamera = blendFromCamera, BlendToCamera = blendToCamera };
            CameraChangeFinishedEvent?.Invoke(this, blendData);
        }
        #endregion

        /*
        #region Crafting
        public event EventHandler<CraftingStation> CraftingStationEnteredEvent;

        public void OnCraftingStationEntered(CraftingStation name)
        {
            CraftingStationEnteredEvent?.Invoke(this, name);
        }
        #endregion

        #region Inventory
        public event EventHandler<Item> ItemAddedEvent;
        
        public void OnItemAdded(Item added)
        {
            ItemAddedEvent?.Invoke(this, added);
        }
        #endregion

        #region Menus
        public event EventHandler<MenuName> MenuOpenedEvent;
        public event EventHandler<MenuName> MenuClosedEvent;
        public event EventHandler<bool> ToggleGameMenusActiveEvent;

        public void OnMenuOpened(MenuName menu)
        {
            MenuOpenedEvent?.Invoke(this, menu);
        }

        public void OnMenuClosed(MenuName menu)
        {
            MenuClosedEvent?.Invoke(this, menu);
        }

        public void OnToggleGameMenusActive(bool active)
        {
            ToggleGameMenusActiveEvent?.Invoke(this, active);
        }
        #endregion
        */

        #region Movement
        public event EventHandler<Transform> RequestMovePlayerEvent;
        public event EventHandler<bool> DisablePlayerControlEvent;
        public event EventHandler<MovementSpeed> MovementSpeedChangedEvent;

        public void OnRequestMovePlayer(Transform transform)
        {
            RequestMovePlayerEvent?.Invoke(this, transform);
        }

        public void OnDisablePlayerControl(bool disable)
        {
            DisablePlayerControlEvent?.Invoke(this, disable);
        }

        public void OnMovementSpeedChanged(MovementSpeed newSpeed)
        {
            MovementSpeedChangedEvent?.Invoke(this, newSpeed);
        }
        #endregion

        #region Player

        public event EventHandler<PlayerState> RequestPlayerStateEvent;
        public event EventHandler<PlayerState> PlayerStateChangedEvent;

        public void OnRequestPlayerState(PlayerState state)
        {
            RequestPlayerStateEvent?.Invoke(this, state);
        }

        public void OnPlayerStateChanged(PlayerState state)
        {
            PlayerStateChangedEvent?.Invoke(this, state);
        }
        #endregion

        /*
        #region Time
        public event EventHandler<Day> DayChangedEvent;
        public event EventHandler<int> TimeChangedEvent;

        public void OnDayChanged(Day newDay)
        {
            DayChangedEvent?.Invoke(this, newDay);
        }

        public void OnTimeChanged(int newTime)
        {
            TimeChangedEvent?.Invoke(this, newTime);
        }
        #endregion
        */
    }
}
