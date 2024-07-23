using Core.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LawnCareSim.Gear
{
    

    [CreateAssetMenu(fileName = "GearDataManager", menuName = "Data/Managers/GearDataManager")]
    public class GearDataManager : ScriptableObject, IDataManager, ISerializationCallbackReceiver
    {
        [SerializeField]
        private GearInfo[] _gearEntries;

        private Dictionary<GearVariant, GearInfo> _gearDataMap;

        #region IDataManager
        public void DataArraysToDictionarys()
        {
            _gearDataMap = new Dictionary<GearVariant, GearInfo>();

            foreach (GearInfo data in _gearEntries)
            {
                if (!_gearDataMap.ContainsKey(data.Variant))
                {
                    _gearDataMap.Add(data.Variant, data);
                }
            }
        }
        #endregion

        #region ISerializationCallbackReceiver
        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            DataArraysToDictionarys();
        }
        #endregion

        public bool GetGearData(GearVariant variant, out GearInfo data)
        {
            return _gearDataMap.TryGetValue(variant, out data);
        }
    }
}
