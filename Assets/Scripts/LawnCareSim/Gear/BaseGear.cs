using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace LawnCareSim.Gear
{
    public class BaseGear : MonoBehaviour, IGear
    {
        protected const float DECAY_RATE = 0.0001f;
        protected const float ENERGY_DRAIN_RATE = 0.001f;

        protected bool _isActive;
        //protected bool _requiresEnergy;
        protected EnergyType _energyType;
        protected Dictionary<GearStatName, GearStat> _stats = new Dictionary<GearStatName, GearStat>();

        private GearStat _durabilityStat;
        private GearStat _energyStat;

        #region Properties
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        public virtual GearType GearType => GearType.None;

        public GearStat DurabilityStat => _durabilityStat;

        public GearStat EnergyStat => _energyStat;

        public virtual bool RequiresEnergy => true;

        public Dictionary<GearStatName, GearStat> Stats => _stats;
        #endregion

        public virtual void Initialize(EnergyType energyType)
        {
            _energyType = energyType;
            _durabilityStat = new GearStat(GearStatName.Durability, 1.0f);
            _stats.Add(GearStatName.Durability, _durabilityStat);

            switch (_energyType)
            {
                case EnergyType.Fuel:
                    _energyStat = new GearStat(GearStatName.Fuel, 1.0f);
                    _stats.Add(GearStatName.Fuel, _energyStat);
                    break;
                case EnergyType.Electric:
                    _energyStat = new GearStat(GearStatName.Charge, 1.0f);
                    _stats.Add(GearStatName.Charge, _energyStat);
                    break;
            }
        }

        #region Power
        public virtual void TurnOn()
        {
            if (_energyStat.Value <= 0 || _durabilityStat.Value <= 0)
            {
                return;
            }

            IsActive = true;
        }

        public virtual void TurnOff()
        {
            IsActive = false;
        }

        public virtual void TogglePower()
        {
            if (CanPowerOn())
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }
        #endregion

        #region Usage
        public virtual void Use(GearUsageInfo usageData)
        {
            if(!CanUse())
            {
                TurnOff();
                return;
            }

            DecayUsageStat();
        }

        protected virtual bool CanPowerOn()
        {
            if (IsActive)
            {
                return false;
            }

            if (RequiresEnergy && _energyStat.Value <= 0)
            {
                return false;
            }

            if (_durabilityStat.Value <= 0)
            {
                return false;
            }

            return true;
        }

        protected virtual bool CanUse()
        {
            if (!IsActive)
            {
                return false;
            }

            if (RequiresEnergy && _energyStat.Value <= 0)
            {
                return false;
            }

            return _durabilityStat.Value > 0;
        }
        #endregion

        #region Stats
        protected virtual void DecayUsageStat()
        {
            _durabilityStat.Value -= DECAY_RATE;
            _energyStat.Value -= ENERGY_DRAIN_RATE;
        }
        #endregion

        public virtual string DebugUnuiqueStats()
        {
            return "";
        }
    }
}
