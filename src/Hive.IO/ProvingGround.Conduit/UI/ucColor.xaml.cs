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

namespace ProvingGround.Conduit.UI
{
    /// <summary>
    /// Interaction logic for ucColor.xaml
    /// </summary>
    public partial class ucColor : UserControl
    {

        public System.Windows.Media.Color ThisColor;

        public ucColor(System.Windows.Media.Color thisColor)
        {
            InitializeComponent();
            Rectangle_Color.Fill = new SolidColorBrush(thisColor);
            ThisColor = thisColor;
        }

        private void Rectangle_Color_MouseUp(object sender, MouseButtonEventArgs e)
        {

            //formColorSet colorSet = new formColorSet(((SolidColorBrush)Rectangle_Color.Fill).Color);
            //colorSet.ShowDialog();

            //Rectangle_Color.Fill = new SolidColorBrush(colorSet._colorSet);
            //ThisColor = colorSet._colorSet;

            //var newEventArgs = new RoutedEventArgs(ColorChanged);
            //RaiseEvent(newEventArgs);
        }

        // This defines the custom event
        public static readonly RoutedEvent ColorChanged = EventManager.RegisterRoutedEvent(
            "ColorChangedHandler", // Event name
            RoutingStrategy.Bubble, // Bubble means the event will bubble up through the tree
            typeof(RoutedEventHandler), // The event type
            typeof(ucColor)); // Belongs to ChildControlBase

        // Allows add and remove of event handlers to handle the custom event
        public event RoutedEventHandler ColorChangedHandler
        {
            add { AddHandler(ColorChanged, value); }
            remove { RemoveHandler(ColorChanged, value); }
        }
    }
}
