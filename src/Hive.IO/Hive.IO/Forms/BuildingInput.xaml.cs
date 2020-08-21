using Hive.IO.Building;

namespace Hive.IO.Forms
{
    /// <summary>
    /// Interaction logic for BuildingInput.xaml
    /// </summary>
    public partial class BuildingInput
    {
        public BuildingInput()
        {
            InitializeComponent();

            BuildingUseType.ItemsSource = Sia2024Record.BuildingUseTypes();
            BuildingUseType.SelectedIndex = 0;

            RoomType.ItemsSource = Sia2024Record.RoomTypes(BuildingUseType.SelectedItem as string);
            RoomType.SelectedIndex = 0;

            BuildingQuality.ItemsSource = Sia2024Record.Qualities();
            BuildingQuality.SelectedIndex = 0;
        }

        private void BuildingUseType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RoomType.ItemsSource = Sia2024Record.RoomTypes(BuildingUseType.SelectedItem as string);
            RoomType.SelectedIndex = 0;
        }
    }
}
