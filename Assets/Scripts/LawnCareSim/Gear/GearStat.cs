using LawnCareSim.Gear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawnCareSim.Gear
{
    public class GearStat
    {
        public GearStatName Name;
        public float Value;

        public GearStat(GearStatName name, float value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}
