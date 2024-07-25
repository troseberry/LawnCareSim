using System.Collections.Generic;

namespace LawnCareSim.Gear
{
    public interface IGear
    {
        GearType GearType { get; }

        /// <summary>
        /// Whether the gear is switched on or off
        /// </summary>
        bool IsActive { get; set; }

        Dictionary<GearStatName, GearStat> Stats { get; }

        GearStat DurabilityStat { get; }

        GearStat EnergyStat { get; }

        bool RequiresEnergy { get; }
        /*
        /// <summary>
        /// Health of gear. From 0 to 1
        /// </summary>
        float Durability { get; set; }

        /// <summary>
        /// How much fuel or power charge the gear has
        /// </summary>
        float Energy { get; set; }
        */

        void Initialize(EnergyType energyType);

        void Use(GearUsageInfo usageData);

        void TurnOn();

        void TurnOff();

        void TogglePower();

        string DebugUnuiqueStats();
    }
}
