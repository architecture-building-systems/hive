using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Hive.IO.Forms
{
    /// <summary>
    ///     A ViewModel to bind the EnergySystemsInput form to. This includes any defaults used and relationships to user
    ///     controls (ConversionProperties).
    /// </summary>
    public class EnergySystemsInputViewModel : ViewModelBase
    {
        public EnergySystemsInputViewModel()
        {
            ConversionTechnologies = new ObservableCollection<ConversionTechPropertiesViewModel>
            {
                new ConversionTechPropertiesViewModel
                {
                    Name = "Photovoltaic (PV)",
                },
                new ConversionTechPropertiesViewModel
                {
                    Name = "Boiler (Gas)"
                }
            };

            Emitters = new ObservableCollection<EmitterPropertiesViewModel>
            {
                new EmitterPropertiesViewModel
                {
                    Name = "Radiator",
                },
                new EmitterPropertiesViewModel
                {
                    Name = "Air diffuser",
                }
            };

            Surfaces = new ObservableCollection<SurfaceViewModel>
            {
                new SurfaceViewModel {Area = 1.0, Name = "Srf0"},
                new SurfaceViewModel {Area = 1.1, Name = "Srf1"},
                new SurfaceViewModel {Area = 2.2, Name = "Srf2"},
                new SurfaceViewModel {Area = 3.3, Name = "Srf3"},
                new SurfaceViewModel {Area = 4.4, Name = "Srf4"}
            };
        }

        public ObservableCollection<EmitterPropertiesViewModel> Emitters { get; }
        public ICollectionView EmitterPropertiesView => CollectionViewSource.GetDefaultView(Emitters);

        public ObservableCollection<ConversionTechPropertiesViewModel> ConversionTechnologies { get; }

        public ICollectionView ConversionTechnologiesView =>
            CollectionViewSource.GetDefaultView(ConversionTechnologies);

        public IEnumerable<SurfaceViewModel> FreeSurfaces => Surfaces.Where(sm =>
            sm.Connection == null || !sm.Connection.IsSurfaceTech);


        public ObservableCollection<SurfaceViewModel> Surfaces { get; set; }

        public IEnumerable<SurfaceViewModel> SurfacesForConversion(ConversionTechPropertiesViewModel vm)
        {
            return Surfaces.Where(
                sm => sm.Connection == null || sm.Connection == vm || !sm.Connection.IsSurfaceTech);
        }
    }
}