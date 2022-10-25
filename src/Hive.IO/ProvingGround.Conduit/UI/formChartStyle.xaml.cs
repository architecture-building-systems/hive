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

using ProvingGround.Conduit.Classes;
using Rhino;

namespace ProvingGround.Conduit.UI
{
    /// <summary>
    /// Interaction logic for formChartStyle.xaml
    /// </summary>
    public partial class formChartStyle : Window
    {

        private System.Drawing.Color _categoryAxisColor;
        private System.Drawing.Color _valueAxisColor;

        private System.Drawing.Color _black = System.Drawing.Color.Black;
        private System.Drawing.Color _white = System.Drawing.Color.White;

        public formChartStyle(System.Drawing.Color categoryAxisColor, System.Drawing.Color valueAxisColor)
        {
            InitializeComponent();

            // Set button colors
            _categoryAxisColor = categoryAxisColor;
            _valueAxisColor = valueAxisColor;

            Color m_categorySet = Color.FromRgb(_categoryAxisColor.R, _categoryAxisColor.G, _categoryAxisColor.B);
            Button_CategoryAxisColor.Background = new SolidColorBrush(m_categorySet);

            Color m_categoryText = _categoryAxisColor.GetBrightness() > 0.5 ?
                Color.FromRgb(_black.R, _black.G, _black.B) :
                Color.FromRgb(_white.R, _white.G, _white.B);

            Button_CategoryAxisColor.Foreground = new SolidColorBrush(m_categoryText);

            Color m_valueSet = Color.FromRgb(_valueAxisColor.R, _valueAxisColor.G, _valueAxisColor.B);
            Button_ValueAxisColor.Background = new SolidColorBrush(m_valueSet);

            Color m_valueText = _valueAxisColor.GetBrightness() > 0.5 ?
                Color.FromRgb(_black.R, _black.G, _black.B) :
                Color.FromRgb(_white.R, _white.G, _white.B);

            Button_ValueAxisColor.Foreground = new SolidColorBrush(m_valueText);


        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Button_CategoryAxisColor_Click(object sender, RoutedEventArgs e)
        {

            if(Rhino.UI.Dialogs.ShowColorDialog(ref _categoryAxisColor))
            {
                Color m_set = Color.FromRgb(_categoryAxisColor.R, _categoryAxisColor.G, _categoryAxisColor.B);
                Button_CategoryAxisColor.Background = new SolidColorBrush(m_set);

                Color m_categoryText = _categoryAxisColor.GetBrightness() > 0.5 ?
                Color.FromRgb(_black.R, _black.G, _black.B) :
                Color.FromRgb(_white.R, _white.G, _white.B);

                Button_CategoryAxisColor.Foreground = new SolidColorBrush(m_categoryText);

                ((clsChartStyleDynamic)this.DataContext).CategoryAxisColor = _categoryAxisColor;
            }

        }

        private void Button_ValueAxisColor_Click(object sender, RoutedEventArgs e)
        {
            if (Rhino.UI.Dialogs.ShowColorDialog(ref _valueAxisColor))
            {
                Color m_set = Color.FromRgb(_valueAxisColor.R, _valueAxisColor.G, _valueAxisColor.B);
                Button_ValueAxisColor.Background = new SolidColorBrush(m_set);

                Color m_valueText = _valueAxisColor.GetBrightness() > 0.5 ?
                Color.FromRgb(_black.R, _black.G, _black.B) :
                Color.FromRgb(_white.R, _white.G, _white.B);

                Button_ValueAxisColor.Foreground = new SolidColorBrush(m_valueText);

                ((clsChartStyleDynamic)this.DataContext).ValueAxisColor = _valueAxisColor;
            }
        }
    }
}
