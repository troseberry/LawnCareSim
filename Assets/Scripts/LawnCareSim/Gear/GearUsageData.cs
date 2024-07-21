using UnityEngine;

namespace LawnCareSim.Gear
{
    public abstract class GearUsageData
    {
        public GameObject UsageObject;

        public GearUsageData(GameObject usageObject)
        {
            UsageObject = usageObject;
        }
    }

    public class DefaultGearUsageData : GearUsageData
    {
        public DefaultGearUsageData(GameObject usageObject) : base(usageObject) { }
    }
}
