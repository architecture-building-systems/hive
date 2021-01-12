using System;
using System.ComponentModel;
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
        private bool _updatingGrid;

        public EnergySystemsInputViewModel State { get; private set; } = new EnergySystemsInputViewModel();

        public DialogResult ShowDialog(EnergySystemsInputViewModel state)
        {
            State = state;
            return ShowDialog();
        }

        private void EnergySystemsInputForm_Load(object sender, System.EventArgs e)
        {
            gridConversion.DataSource = new BindingList<ConversionTechPropertiesViewModel>(State.ConversionTechnologies);
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
            // conversionColumn.Items.AddRange(ConversionTechPropertiesViewModel.AllNames.ToArray<object>());
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

            UpdateEditableForRows();
        }

        /// <summary>
        /// Make sure the "editable" property is set correctly for each row. In effect, that
        /// means setting DataGridViewComboBoxCell.Items
        /// </summary>
        private void UpdateEditableForRows()
        {
            foreach (var row in gridConversion.Rows.Cast<DataGridViewRow>())
            {
                var conversionTech = row.DataBoundItem as ConversionTechPropertiesViewModel;
                var nameCell = (DataGridViewComboBoxCell) row.Cells[1];

                try
                {
                    _updatingGrid = true;
                    nameCell.Items.Clear();
                }
                finally
                {
                    _updatingGrid = false;
                }
                
                nameCell.Items.AddRange(conversionTech?.ValidNames.ToArray<object>() ?? ConversionTechPropertiesViewModel.AllNames.ToArray<object>());

                nameCell.ReadOnly = conversionTech?.IsParametricDefined ?? false;
                nameCell.DisplayStyle = conversionTech == null || conversionTech.IsEditable
                    ? DataGridViewComboBoxDisplayStyle.DropDownButton
                    : DataGridViewComboBoxDisplayStyle.Nothing;
            }
        }

        private void gridConversion_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (_updatingGrid)
            {
                return;
            }
        }

        private void gridConversion_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            UpdateEditableForRows();
        }

        private void gridConversion_SelectionChanged(object sender, EventArgs e)
        {
            if (gridConversion.CurrentRow.DataBoundItem == null)
            {
                return;
            }

            tableLayoutPanelMain.Controls.Remove(ConversionProperties);

            ConversionProperties = new Controls.ChillerProperties
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Conversion = (ConversionTechPropertiesViewModel) gridConversion.CurrentRow.DataBoundItem
            };
            tableLayoutPanelMain.Controls.Add(ConversionProperties, 0, 1);
            Update();
        }
    }
}