namespace Hive.IO.Forms
{
    /// <summary>
    /// A ViewModel to bind the EnergySystemsInput form to. This includes any defaults used and relationships to user controls (ConversionProperties).
    /// </summary>
    public class EnergySystemsInputViewModel: ViewModelBase
    {
        private IConversionTechProperties _currentConversionTechProperties;

        public IConversionTechProperties CurrentConversionTechProperties
        {
            get => _currentConversionTechProperties;
            private set => Set(ref _currentConversionTechProperties, value);
        }
    }
}
