using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Hive.IO.EnergySystems;

namespace Hive.IO.Forms
{
    /// <summary>
    /// A ViewModel to bind the EnergySystemsInput form to. This includes any defaults used and relationships to user controls (ConversionProperties).
    /// </summary>
    public class EnergySystemsInputViewModel: ViewModelBase
    {
        public IEnumerable<string> ConversionNames => ConversionTechPropertiesViewModel.ValidNames;

        public ObservableCollection<ConversionTechPropertiesViewModel> ConversionTechnologies { get; }

        public ICollectionView ConversionTechnologiesView =>
            CollectionViewSource.GetDefaultView(ConversionTechnologies);

        public EnergySystemsInputViewModel()
        {
            ConversionTechnologies = new ObservableCollection<ConversionTechPropertiesViewModel>
            {
                new ConversionTechPropertiesViewModel()
                {
                    Name = "Photovoltaic (PV)",
                },
                new ConversionTechPropertiesViewModel()
                {
                    Name = "Boiler (Gas)",
                }
            };
            ConversionTechnologies.CollectionChanged += OnConversionTechnologiesOnCollectionChanged;

            Surfaces = new ObservableCollection<SurfaceViewModel>
            {
                new SurfaceViewModel{Area=1.0, Name="Srf0"},
                new SurfaceViewModel{Area=1.1, Name="Srf1"},
                new SurfaceViewModel{Area=2.2, Name="Srf2"},
                new SurfaceViewModel{Area=3.3, Name="Srf3"},
                new SurfaceViewModel{Area=4.4, Name="Srf4"},
            };
        }

        public IEnumerable<SurfaceViewModel> FreeSurfaces => Surfaces.Where(sm => sm.Connection == null);

        public IEnumerable<SurfaceViewModel> SurfacesForConversion(ConversionTechPropertiesViewModel vm) =>
            Surfaces.Where(
                sm => sm.Connection == null || sm.Connection == vm);

        private void OnConversionTechnologiesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {

        }


        public ObservableCollection<SurfaceViewModel> Surfaces { get; set; }
    }
}
