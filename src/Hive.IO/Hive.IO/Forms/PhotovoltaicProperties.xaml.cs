using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hive.IO.Forms
{
    /// <summary>
    ///     Interaction logic for PhotovoltaicProperties.xaml
    /// </summary>
    public partial class PhotovoltaicProperties : UserControl
    {
        private bool _changingDataContext;

        public PhotovoltaicProperties()
        {
            InitializeComponent();
        }

        private void ListBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ListBox list && e.NewValue is ConversionTechPropertiesViewModel vm)
            {
                _changingDataContext = true;
                list.UnselectAll();
                foreach (var surface in vm.SelectedSurfaces) list.SelectedItems.Add(surface);
                _changingDataContext = false;
            }
        }

        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_changingDataContext && sender is ListBox list &&
                list.DataContext is ConversionTechPropertiesViewModel vm)
            {
                foreach (var item in e.RemovedItems)
                    if (item is SurfaceViewModel sm && sm.Connection == vm)
                        sm.Connection = null;

                foreach (var item in e.AddedItems)
                    if (item is SurfaceViewModel sm && sm.Connection == null)
                        sm.Connection = vm;
                vm.SelectedSurfaces = list.SelectedItems.Cast<SurfaceViewModel>();

                // make sure dependant properties are re-evaluated
                vm.ModuleType = vm.ModuleType;
            }
        }
    }
}