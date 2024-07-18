using UnityEngine;

namespace LawnCareSim.Gear
{
    public class BaseGear : MonoBehaviour, IGear
    {
        protected const float DECAY_RATE = 0.0001f;
        protected const float ENERGY_DRAIN_RATE = 0.001f;

        protected bool _isActive;
        protected float _energy = 1.0f;
        protected float _durability = 1.0f;

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        public float Durability
        {
            get => _durability;
            set => _durability = Mathf.Clamp(value, 0f, 1.0f);
        }

        public float Energy
        {
            get => _energy;
            set => _energy = Mathf.Clamp(value, 0f, 1.0f);
        }

        public virtual GearType GearType => GearType.None;

        public virtual void TurnOn()
        {
            if (Energy <= 0 || Durability <= 0)
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
            if (!IsActive)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        public virtual void Use()
        {
            //Durability -= DECAY_RATE;
            //Energy -= ENERGY_DRAIN_RATE;

            if (_durability <= 0 || _energy <= 0)
            {
                TurnOff();
            }
        }
    }
}
