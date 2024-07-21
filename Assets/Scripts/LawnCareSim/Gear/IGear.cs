namespace LawnCareSim.Gear
{
    public interface IGear
    {
        GearType GearType { get; }

        /// <summary>
        /// Whether the gear is switched on or off
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Health of gear. From 0 to 1
        /// </summary>
        float Durability { get; set; }

        /// <summary>
        /// How much fuel or power charge the gear has
        /// </summary>
        float Energy { get; set; }

        void Use(GearUsageData usageData);

        void TurnOn();

        void TurnOff();

        void TogglePower();

        string DebugUnuiqueStats();
    }
}
