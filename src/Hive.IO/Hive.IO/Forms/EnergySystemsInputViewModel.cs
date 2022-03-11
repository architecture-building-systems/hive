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
            ConversionTechnologies = new BindingList<ConversionTechPropertiesViewModel>
            {
                new ConversionTechPropertiesViewModel
                {
                    Name = "Photovoltaic (PV)"
                },
                new ConversionTechPropertiesViewModel
                {
                    Name = "Boiler (Gas)"
                },
                new ConversionTechPropertiesViewModel
                {
                    Name = "Building Integrated Photovoltaic (BIPV)"
                }
            };

            Emitters = new BindingList<EmitterPropertiesViewModel>
            {
                new EmitterPropertiesViewModel
                {
                    Name = "Radiator"
                },
                new EmitterPropertiesViewModel
                {
                    Name = "Air diffuser"
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

        //public IEnumerable<SurfaceViewModel> _availableSurfaces = ConversionTechPropertiesViewModel.
        public BindingList<EmitterPropertiesViewModel> Emitters { get; }
        public ICollectionView EmitterPropertiesView => CollectionViewSource.GetDefaultView(Emitters);

        public BindingList<ConversionTechPropertiesViewModel> ConversionTechnologies { get; }

        public ICollectionView ConversionTechnologiesView =>
            CollectionViewSource.GetDefaultView(ConversionTechnologies);

        public IEnumerable<SurfaceViewModel> FreeSurfaces => Surfaces.Where(sm =>
            sm.Connection == null || !sm.Connection.IsSurfaceTech);


        /// <summary>
        ///     The list of surfaces that came from Meshes - these are either free surfaces or those
        ///     connected to form-defined surfaceTech. We also make sure that SurfaceViewModels connected
        ///     to non-surface-tech are included. (this happens when the type of technology is changed...)
        /// </summary>
        public IEnumerable<SurfaceViewModel> MeshSurfaces =>
            Surfaces.Where(sm =>
                sm.Connection == null || !sm.Connection.IsParametricDefined || !sm.Connection.IsSurfaceTech);


        public ObservableCollection<SurfaceViewModel> Surfaces { get; set; }

        public IEnumerable<SurfaceViewModel> SurfacesForConversion(ConversionTechPropertiesViewModel vm)
        {
            return Surfaces.Where(
                sm => sm.Connection == null || sm.Connection == vm || !sm.Connection.IsSurfaceTech ||
                      !ConversionTechnologies.Contains(sm.Connection));
        }

       
    }
}