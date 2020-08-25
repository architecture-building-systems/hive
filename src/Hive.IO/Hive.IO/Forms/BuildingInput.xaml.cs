using System;
using System.Collections.Generic;
using System.Linq;
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

        private BuildingInput()
        {
            State = new BuildingInputState(Sia2024Record.All().First() as Sia2024RecordEx, false);
            InitializeComponent();
        }


        public BuildingInput(BuildingInputState state) : this()
        {
            State.SiaRoom = state.SiaRoom;
            State.IsEditable = state.IsEditable;
        }
    }
}
