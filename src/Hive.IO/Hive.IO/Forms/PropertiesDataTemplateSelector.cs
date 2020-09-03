using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Hive.IO.Forms
{
    public class PropertiesDataTemplateSelector : DataTemplateSelector
    {
        private static readonly Dictionary<string, string> TemplateMapping = new Dictionary<string, string>
        {
            {"Photovoltaic (PV)", "PhotovoltaicProperties"},
            {"Solar Thermal (ST)", "PhotovoltaicProperties"},
            {"Boiler (Gas)", "GasBoilerProperties"},
            {"CHP", "ChpProperties"},
            {"Chiller (Electricity)", "ChillerProperties"},
            {"ASHP (Electricity)", "AshpProperties"},
            {"Heat Exchanger", "HeatExchangerProperties" },
            {"Cooling Exchanger", "CoolingExchangerProperties" }
        };

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && item != null 
                                                      && item is ConversionTechPropertiesViewModel vm 
                                                      && TemplateMapping.ContainsKey(vm.Name))
                return element.FindResource(TemplateMapping[vm.Name]) as DataTemplate;
            return null;
        }
    }
}