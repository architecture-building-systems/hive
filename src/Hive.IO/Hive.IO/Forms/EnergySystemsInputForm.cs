using System.Linq;
using System.Windows.Forms;

namespace Hive.IO.Forms
{
    public partial class EnergySystemsInputForm : Form
    {
        public EnergySystemsInputForm()
        {
            InitializeComponent();
        }

        private bool _rendering = false;

        public EnergySystemsInputViewModel State { get; private set; } = new EnergySystemsInputViewModel();

        public DialogResult ShowDialog(EnergySystemsInputViewModel state)
        {
            State = state;
            return ShowDialog();
        }

        private void EnergySystemsInputForm_Load(object sender, System.EventArgs e)
        {
            gridConversion.DataSource = State.ConversionTechnologies;
            gridConversion.AutoGenerateColumns = false;
            gridConversion.Columns.Clear();
            gridConversion.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Source",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 0.2f,
                DataPropertyName = "Source",
                ReadOnly = true
            });
            var conversionColumn = new DataGridViewComboBoxColumn()
            {
                Name = "Conversion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 0.6f,
                DataPropertyName = "Name",
            };
            conversionColumn.Items.AddRange(ConversionTechPropertiesViewModel.AllNames.ToArray<object>());
            gridConversion.Columns.Add(conversionColumn);
            gridConversion.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "EndUse",
                HeaderText = "End Use",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 0.2f,
                DataPropertyName = "EndUse",
                ReadOnly = true
            });
        }
    }
}