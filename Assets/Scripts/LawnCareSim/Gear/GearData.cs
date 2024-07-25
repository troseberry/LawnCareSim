using System;
using UnityEngine;

namespace LawnCareSim.Gear
{
    [Serializable]
    public struct GearInfo
    {
        public GearVariant Variant;
        public GearType GearType;
        public Sprite Sprite;
        public GameObject Prefab;
    }

    public struct RuntimeGearData
    {
        public GearType GearType;
        public GearInfo GearInfo;
        public IGear IGear;
        public GameObject GameObject;

        public RuntimeGearData(GearType gearType, GearInfo gearData, IGear gearScript, GameObject gearGO)
        {
            GearType = gearType;
            GearInfo = gearData;
            IGear = gearScript;
            GameObject = gearGO;
        }
    }

    public struct EquippedGear
    {
        public GearType GearType;
        public IGear IGear;
        public GameObject GameObject;
    }
}
