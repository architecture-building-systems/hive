using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Hive.IO.Building;
using Hive.IO.GHComponents;

namespace Hive.IO.Forms
{
    /// <summary>
    /// Interaction logic for BuildingInput.xaml - tightly coupled to GhBuilding.
    /// </summary>
    public partial class BuildingInput
    {
        public BuildingInputState State { get; private set; }

        /// <summary>
        /// Empty constructor for Visual Studio designer - don't use this as the Zone object is not set and this will
        /// throw errors at runtime.
        /// </summary>
        private BuildingInput()
        {
            State = new BuildingInputState(Sia2024Record.First(), null, false);
            InitializeComponent();
        }


        public BuildingInput(BuildingInputState state)
        {
            State = state;
            InitializeComponent();
        }
    }
}
