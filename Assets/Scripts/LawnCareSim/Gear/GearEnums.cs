namespace LawnCareSim.Gear
{
    public enum GearType
    {
        Invalid = -1,

        None = 0,
        Mower,
        Edger,
        Striper,
        Vacuum
    }

    public enum GearVariant
    {
        Invalid,

        FuelPushMower,
        FuelEdger,
        ManualPushStriper,
        FuelVacuum
    }

    public enum GearStatName
    {
        Inavlid = 0,
        Durability,
        Fuel,
        Charge,
        Space
    }

    public enum EnergyType
    {
        None = 0,
        Fuel,
        Electric
    }
}
