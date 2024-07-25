namespace LawnCareSim.Gear
{
    public static class GearHelpers
    {
        public static EnergyType GetEnergyTypeForVariant(GearVariant variant)
        {
            switch(variant)
            {
                case GearVariant.FuelPushMower:
                case GearVariant.FuelEdger:
                case GearVariant.FuelVacuum:
                    return EnergyType.Fuel;
                default:
                    return EnergyType.None;
            }
        }
    }
}
