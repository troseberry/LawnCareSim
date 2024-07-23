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
        public GearInfo GearData;
        public IGear GearScriptRef;
        public GameObject GearGameObject;

        public RuntimeGearData(GearInfo gearData, IGear gearScript, GameObject gearGO)
        {
            GearData = gearData;
            GearScriptRef = gearScript;
            GearGameObject = gearGO;
        }
    }

    public struct EquippedGear
    {
        public GearType GearType;
        public IGear IGear;
        public GameObject GameObject;
    }
}
