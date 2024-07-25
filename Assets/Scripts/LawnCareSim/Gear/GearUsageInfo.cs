using UnityEngine;

namespace LawnCareSim.Gear
{
    public abstract class GearUsageInfo
    {
        public GameObject UsageObject;

        public GearUsageInfo(GameObject usageObject)
        {
            UsageObject = usageObject;
        }
    }

    public class DefaultGearUsageInfo : GearUsageInfo
    {
        public DefaultGearUsageInfo(GameObject usageObject) : base(usageObject) { }
    }
}
