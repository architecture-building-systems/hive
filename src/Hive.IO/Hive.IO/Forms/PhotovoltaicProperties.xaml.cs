using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rhino.Geometry;

namespace Hive.IO.Forms
{
    /// <summary>
    /// Interaction logic for PhotovoltaicProperties.xaml
    /// </summary>
    public partial class PhotovoltaicProperties : UserControl
    {
        public PhotovoltaicProperties()
        {
            InitializeComponent();
        }

        private bool _changingDataContext;
        private void ListBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ListBox list && e.NewValue is ConversionTechPropertiesViewModel vm)
            {
                _changingDataContext = true;
                list.UnselectAll(); 
                foreach (var surface in vm.SelectedSurfaces)
                {
                    list.SelectedItems.Add(surface);
                }
                _changingDataContext = false;
            }
        }

        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_changingDataContext && sender is ListBox list && list.DataContext is ConversionTechPropertiesViewModel vm)
            {
                foreach (var item in e.RemovedItems)
                {
                    if (item is SurfaceViewModel sm && sm.Connection == vm)
                    {
                        sm.Connection = null;
                    }
                }

                foreach (var item in e.AddedItems)
                {
                    if (item is SurfaceViewModel sm && sm.Connection == null)
                    {
                        sm.Connection = vm;
                    }
                }
                vm.SelectedSurfaces = list.SelectedItems.Cast<SurfaceViewModel>();
            }
        }
    }
}
