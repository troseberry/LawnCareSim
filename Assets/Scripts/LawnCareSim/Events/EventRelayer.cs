using Core.Events;
using LawnCareSim.Camera;
//using LawnCareSim.Inventory;
//using LawnCareSim.Animation;
//using LawnCareSim.Time;
using LawnCareSim.UI;
using System;
using UnityEngine;
//using AnimationState = LawnCareSim.Animation.AnimationState;
using LawnCareSim.Player;
using LawnCareSim.Gear;
using LawnCareSim.Interaction;
using LawnCareSim.Jobs;

namespace LawnCareSim.Events
{
    [CreateAssetMenu(fileName = "EventRelayer", menuName = "Events/EventRelayer")]
    public class EventRelayer : EventRelayerScriptableObject<EventRelayer>
    {
        
        #region Animation
        /*
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
        */
        #endregion

        #region Camera
        /*
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
        */
        #endregion

        #region Gear
        public EventHandler<GearType> GearSwitchedEvent;

        public void OnGearSwitched(GearType newGear)
        {
            GearSwitchedEvent?.Invoke(this, newGear);
        }
        #endregion

        #region Grass
        public event EventHandler GrassCutEvent;
        public event EventHandler GrassEdgedEvent;
        public event EventHandler GrassStripedEvent;

        public void OnGrassCut()
        {
            GrassCutEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnGrassEdged()
        {
            GrassEdgedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnGrassStriped()
        {
            GrassStripedEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Interaction
        public EventHandler<(IInteractable, string)> EnteredInteractionZoneEvent;
        public EventHandler<IInteractable> ExitedInteractionZoneEvent;

        public void OnEnteredInteractionZone(IInteractable interactable, string promptToShow)
        {
            EnteredInteractionZoneEvent?.Invoke(this, (interactable, promptToShow));
        }

        public void OnExitedInteractionZone(IInteractable interactable)
        {
            ExitedInteractionZoneEvent?.Invoke(this, interactable);
        }
        #endregion

        #region Inventory
        /*
        public event EventHandler<Item> ItemAddedEvent;
        
        public void OnItemAdded(Item added)
        {
            ItemAddedEvent?.Invoke(this, added);
        }
        */
        #endregion

        #region Job
        public event EventHandler<Job> JobCreatedEvent;
        public event EventHandler<Job> LawnGeneratedEvent;
        public event EventHandler<Job> JobTasksCreatedEvent;
        public event EventHandler<Job> ActiveJobSetEvent;
        public event EventHandler<Job> ActiveJobProgressedEvent;
        public event EventHandler<JobTask> ActiveJobTaskProgressedEvent;

        public void OnJobCreated(Job job)
        {
            JobCreatedEvent?.Invoke(this, job);
        }

        public void OnLawnGenerated(Job job)
        {
            LawnGeneratedEvent?.Invoke(this, job);
        }

        public void OnJobTasksCreatedEvent(Job job)
        {
            JobTasksCreatedEvent?.Invoke(this, job);
        }

        public void OnActiveJobSetEvent(Job job)
        {
            ActiveJobSetEvent?.Invoke(this, job);
        }

        public void OnActiveJobProgressed(Job job)
        {
            ActiveJobProgressedEvent?.Invoke(this, job);
        }

        public void OnActiveJobTaskProgressed(JobTask task)
        {
            ActiveJobTaskProgressedEvent?.Invoke(this, task);
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

        #region Movement
        public event EventHandler<Transform> MovePlayerEvent;
        public event EventHandler<bool> DisablePlayerControlEvent;
        public event EventHandler<MovementSpeed> MovementSpeedChangedEvent;

        public void OnMovePlayer(Transform transform)
        {
            MovePlayerEvent?.Invoke(this, transform);
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

        
        #region Time
        /*
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
        */
        #endregion
    }
}
