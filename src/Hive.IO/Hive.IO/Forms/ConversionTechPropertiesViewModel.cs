namespace Hive.IO.Forms
{
    public interface IConversionTechProperties
    {
        string Name { get; }
        string Source { get; }
        string EndUse { get; }
    }

    public class PhotovoltaicPropertiesViewModel: ViewModelBase, IConversionTechProperties
    {
        public string Name => "Photovoltaic (PV)";
        public string Source => "Solar";
        public string EndUse => "Electricity demand";
    }

    public class GasBoilerPropertiesViewModel : ViewModelBase, IConversionTechProperties
    {
        public string Name => "Boiler (Gas)";
        public string Source => "Gas";
        public string EndUse => "Heating demand, DHW";
    }
}
