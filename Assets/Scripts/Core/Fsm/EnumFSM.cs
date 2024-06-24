
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Patterns
{
    public struct StateTransition
    {
        public EventHandler OutsideEvent;
        public EventHandler InternalEvent;
    }

    public class EnumFSM
    {
        public const int NO_TRIGGER = -1;

        private Dictionary<int, Enum> _states = new Dictionary<int, Enum>();

        /// FromState, (ToState, TransitionInfo)
        private Dictionary<Enum, Dictionary<Enum, StateTransition>> _eventTransitions = new Dictionary<Enum, Dictionary<Enum, StateTransition>>();

        /// FromState, (TriggerEnum, ToState)
        private Dictionary<Enum, Dictionary<int, Enum>> _triggerTransitions = new Dictionary<Enum, Dictionary<int, Enum>>();

        private Dictionary<Enum, Action> _onEnterMethods = new Dictionary<Enum, Action>();
        private Dictionary<Enum, Action> _onExitMethods = new Dictionary<Enum, Action>();
        private Dictionary<Enum, Func<int>> _onUpdateMethods = new Dictionary<Enum, Func<int>>();

        private Type _stateType;
        private Enum _currentState;
        private Func<int> _currentStateUpdateMethod;
        private int _currentStateResult = NO_TRIGGER;

        public Enum CurrentState => _currentState;

        public EnumFSM(Type stateType) 
        {
            _stateType = stateType;
        }

        /// <summary>
        /// Define a new EnumFSM state
        /// </summary>
        public EnumFSM State(int key, Enum state)
        {
            if (state.GetType() != _stateType)
            {
                return null;
            }

            _states.Add(key, state);
            return this;
        }

        /// <summary>
        /// Define new event-based transition from one state to another
        /// </summary>
        public EnumFSM OnEventTransition(ref EventHandler transitionEvent, Enum fromState, Enum toState)
        {
            EventHandler newTransition = (s, e) =>
            {
                TransitionState(toState);
            };

            transitionEvent += newTransition;

            if (!_eventTransitions.TryGetValue(fromState, out var stateTransitions))
            {
                stateTransitions = new Dictionary<Enum, StateTransition>();
            }

            stateTransitions.Add(toState, new StateTransition()
            {
                OutsideEvent = transitionEvent,
                InternalEvent = newTransition
            });

            _eventTransitions[fromState] = stateTransitions;

            return this;
        }

        public EnumFSM OnTriggerTransition(Enum fromState, int trigger, Enum toState)
        {
            if (!_triggerTransitions.TryGetValue(fromState, out var stateTransitions))
            {
                stateTransitions = new Dictionary<int, Enum>();
            }

            stateTransitions.Add(trigger, toState);
            _triggerTransitions[fromState] = stateTransitions;

            return this;
        }

        private void TransitionState(Enum newState)
        {
            if (newState == _currentState)
            {
                return;
            }
            
            if (_onExitMethods.TryGetValue(_currentState, out var exitMethod))
            {
                //Debug.Log($"[EnumFSM][TransitionState] - Exiting {_currentState} |");
                exitMethod?.Invoke();
            }

            _currentState = newState;
            _onUpdateMethods.TryGetValue(_currentState, out _currentStateUpdateMethod);

            if (_onEnterMethods.TryGetValue(_currentState, out var enterMethod))
            {
                //Debug.Log($"[EnumFSM][TransitionState] - Entering {_currentState} |");
                enterMethod?.Invoke();
            }
        }

        public EnumFSM OnEnter(Enum state, Action onEnterDelegate)
        {
            if (_onEnterMethods.ContainsKey(state))
            {
                _onEnterMethods[state] = onEnterDelegate;
            }
            else
            {
                _onEnterMethods.Add(state, onEnterDelegate);
            }

            return this;
        }

        public EnumFSM OnUpdate(Enum state, Func<int> onUpdateDelegate)
        {
            if (_onUpdateMethods.ContainsKey(state))
            {
                _onUpdateMethods[state] = onUpdateDelegate;
            }
            else
            {
                _onUpdateMethods.Add(state, onUpdateDelegate);
            }

            return this;
        }

        public EnumFSM OnExit(Enum state, Action onExitDelegate)
        {
            if (_onExitMethods.ContainsKey(state))
            {
                _onExitMethods[state] = onExitDelegate;
            }
            else
            {
                _onExitMethods.Add(state, onExitDelegate);
            }

            return this;
        }

        public void SetStartingState(Enum state)
        {
            _currentState = state;
            _onUpdateMethods.TryGetValue(_currentState, out _currentStateUpdateMethod);
        }

        public void Process()
        {
            _currentStateResult = _currentStateUpdateMethod?.Invoke() ?? NO_TRIGGER;

            if (_currentStateResult != NO_TRIGGER)
            {
                TryStartTransition(_currentState, _currentStateResult);
            }
        }

        private void TryStartTransition(Enum fromState, int trigger)
        {
            if (_triggerTransitions.Count <= 0)
            {
                return;
            }

            if (_triggerTransitions.TryGetValue(fromState, out var transitions))
            {
                if (transitions.TryGetValue(trigger, out var newState))
                {

                    TransitionState(newState);
                }
            }
        }
    }
}
