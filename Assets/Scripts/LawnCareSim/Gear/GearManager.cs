using Core.GameFlow;
using LawnCareSim.Events;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public partial class GearManager : MonoBehaviour
    {
        public static GearManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void OnGUI()
        {
            var width = UnityEngine.Camera.main.pixelWidth;
            var height = UnityEngine.Camera.main.pixelHeight;

            GUIStyle fontStyle = GUI.skin.label;
            fontStyle.fontSize = 20;

            // Equipped Gear
            Rect mainRect = new Rect(width * 0.82f, height * 0.04f, 300, 200);
            GUI.Box(mainRect, GUIContent.none);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y, 250, 30), $"Equipped Gear: {_equippedGear?.GearType}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 30, 250, 30), $"Energy {_equippedGear?.Energy}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 60, 250, 30), $"Durability: {_equippedGear?.Durability}", fontStyle);

            // Gear Switching
            Rect bottomRect = new Rect(width * 0.5f - 450, height * 0.8f, 900, 200);
            GUI.Box(bottomRect, GUIContent.none);
            if (GUI.Button(new Rect(bottomRect.x + 15, bottomRect.y + 25, 150, 150), "None"))
            {
                SwitchGear(GearType.None);
            }

            if (GUI.Button(new Rect(bottomRect.x + 180, bottomRect.y + 25, 150, 150), "Mower"))
            {
                SwitchGear(GearType.Mower);
            }
        }
    }

    public partial class GearManager : IManager
    {
        private GearType _equippedGearType = GearType.None;
        private IGear _equippedGear;

        public void InitializeManager()
        {
            
        }

        private void SwitchGear(GearType newGear)
        {
            var prevGearType = _equippedGearType;
            _equippedGearType = newGear;

            EventRelayer.Instance.OnGearSwitched(prevGearType, _equippedGearType);
        }
    }
}
