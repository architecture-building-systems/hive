using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Hive.IO.Building;
using Hive.IO.Forms.Controls;

namespace Hive.IO.Forms
{
    public partial class EnergySystemsInputForm : Form
    {
        public EnergySystemsInputForm()
        {
            InitializeComponent();
        }

        private bool _updatingGrid;

        public EnergySystemsInputViewModel State { get; private set; } = new EnergySystemsInputViewModel();

        public DialogResult ShowDialog(EnergySystemsInputViewModel state)
        {
            State = state;
            return ShowDialog();
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (_updatingGrid)
            {
                return;
            }
        }

        private void EnergySystemsInputForm_Load(object sender, EventArgs e)
        {
            SetUpGridConversion();
            SetUpGridEmission();
        }

        #region tabConversion
        private void SetUpGridEmission()
        {
            // set up gridEmission
            gridEmission.AutoGenerateColumns = false;
            gridEmission.Columns.Clear();
            gridEmission.Columns.Add(new DataGridViewComboBoxColumn()
            {
                Name = "Emitter",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 0.6f,
                DataPropertyName = "Name",
            });
            gridEmission.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "EndUse",
                HeaderText = "End Use",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 0.2f,
                DataPropertyName = "EndUse",
                ReadOnly = true
            });
            gridEmission.DataSource = new BindingList<EmitterPropertiesViewModel>(State.Emitters);
            UpdateEditableForEmitterRows();
        }

        private void SetUpGridConversion()
        {
            // set up gridConversion
            gridConversion.AutoGenerateColumns = false;
            gridConversion.Columns.Clear();
            gridConversion.Columns.Add(new DataGridViewButtonColumn()
            {
                Name = "AddDelete",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 0.2f,
                DataPropertyName = "Edit",
                ReadOnly = true
            });
            gridConversion.Columns.Add(new DataGridViewComboBoxColumn()
            {
                Name = "Conversion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 0.6f,
                DataPropertyName = "Name",
            });
            gridConversion.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Source",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 0.2f,
                DataPropertyName = "Source",
                ReadOnly = true
            });
            // conversionColumn.Items.AddRange(ConversionTechPropertiesViewModel.AllNames.ToArray<object>());
            gridConversion.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "EndUse",
                HeaderText = "End Use",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 0.2f,
                DataPropertyName = "EndUse",
                ReadOnly = true
            });
            gridConversion.DataSource = new BindingList<ConversionTechPropertiesViewModel>(State.ConversionTechnologies);
            UpdateEditableForConversionRows();
        }

        /// <summary>
        /// Make sure the "editable" property is set correctly for each row. In effect, that
        /// means setting DataGridViewComboBoxCell.Items
        /// </summary>
        private void UpdateEditableForConversionRows()
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

        private void gridConversion_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            UpdateEditableForConversionRows();
        }

        /// <summary>
        /// Switch out the control used to display the conversion tech properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridConversion_SelectionChanged(object sender, EventArgs e)
        {
            if (gridConversion.CurrentRow.DataBoundItem == null)
            {
                return;
            }

            tableLayoutPanelMain.Controls.Remove(ConversionProperties);

            var conversionTech = (ConversionTechPropertiesViewModel) gridConversion.CurrentRow.DataBoundItem;

            // figure out which surfaces can be used for this conversion (used by surface-based conversions...)
            conversionTech.AvailableSurfaces = State.SurfacesForConversion(conversionTech);

            // select the correct user control to display the conversion's properties
            ConversionProperties = ConversionPropertiesFactory[conversionTech.Name]();
            ConversionProperties.Dock = DockStyle.Fill;
            ConversionProperties.Conversion = conversionTech;
            
            tableLayoutPanelMain.Controls.Add(ConversionProperties, 0, 1);
            Update();
        }

        /// <summary>
        /// This dictionary is used to select which control to display the conversion tech's properties
        /// in based on the value of ConversionTechPropertiesViewModel.Name.
        /// </summary>
        private static readonly Dictionary<string, Func<ConversionTechPropertiesBase>> ConversionPropertiesFactory 
            = new Dictionary<string, Func<ConversionTechPropertiesBase>>
            {
                {"Photovoltaic (PV)", () => new Controls.SurfaceTechnologyProperties()},
                {"Solar Thermal (ST)", () => new Controls.SurfaceTechnologyProperties()}, // same as PV...
                {"Boiler (Gas)", () => new Controls.GasBoilerProperties()},
                {"CHP", () => new Controls.ChpProperties()},
                {"Chiller (Electricity)", () => new Controls.ChillerProperties()},
                {"ASHP (Electricity)", () => new Controls.ChillerProperties()},  // same as Chiller...
                {"Heat Exchanger", () => new Controls.HeatExchangerProperties()},
                {"Cooling Exchanger", () => new Controls.HeatExchangerProperties() } // same as Heat Exchanger...
            };


        /// <summary>
        /// See here for how to catch changes to the combobox edit in the grid: https://stackoverflow.com/a/21321724/2260
        ///
        /// This raises gridConversion_CellValueChanged, which in turn ends the edit and also calls
        /// gridConversion_SelectionChanged to update the properties control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridConversion_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (gridConversion.IsCurrentCellDirty)
            {
                gridConversion.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void gridConversion_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex != 1)
            {
                return;
            }

            gridConversion.EndEdit();
            gridConversion.InvalidateRow(e.RowIndex);
            gridConversion_SelectionChanged(sender, e);
        }
        #endregion tabConversion


        

        #region tabEmission
        /// <summary>
        /// Switch out the control used to display the emitter properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridEmission_SelectionChanged(object sender, EventArgs e)
        {
            if (gridEmission.CurrentRow.DataBoundItem == null)
            {
                return;
            }

            emitterProperties.Emitter = (EmitterPropertiesViewModel) gridEmission.CurrentRow.DataBoundItem;

            Update();
        }

        /// <summary>
        /// Make sure the "editable" property is set correctly for each row. In effect, that
        /// means setting DataGridViewComboBoxCell.Items
        /// </summary>
        private void UpdateEditableForEmitterRows()
        {
            foreach (var row in gridEmission.Rows.Cast<DataGridViewRow>())
            {
                var emitter = row.DataBoundItem as EmitterPropertiesViewModel;
                var nameCell = (DataGridViewComboBoxCell) row.Cells[0];

                try
                {
                    _updatingGrid = true;
                    nameCell.Items.Clear();
                }
                finally
                {
                    _updatingGrid = false;
                }
                
                nameCell.Items.AddRange(emitter?.ValidNames.ToArray<object>() 
                                        ?? EmitterPropertiesViewModel.AllNames.ToArray<object>());

                nameCell.ReadOnly = emitter?.IsParametricDefined ?? false;
                nameCell.DisplayStyle = emitter == null || emitter.IsEditable
                    ? DataGridViewComboBoxDisplayStyle.DropDownButton
                    : DataGridViewComboBoxDisplayStyle.Nothing;
            }
        }

        private void gridEmission_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            UpdateEditableForEmitterRows();
        }

        /// <summary>
        /// See here for how to catch changes to the combobox edit in the grid: https://stackoverflow.com/a/21321724/2260
        ///
        /// This raises gridEmission_CellValueChanged, which in turn ends the edit and also calls
        /// gridEmission_SelectionChanged to update the properties control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridEmission_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (gridEmission.IsCurrentCellDirty)
            {
                gridEmission.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void gridEmission_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex != 0)
            {
                return;
            }

            gridEmission.EndEdit();
            gridEmission.InvalidateRow(e.RowIndex);
            gridEmission_SelectionChanged(sender, e);
        }

        #endregion tabEmission

        private void gridConversion_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridConversion.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow item in gridConversion.SelectedRows)
                {
                    gridConversion.Rows.RemoveAt(item.Index);
                }
            }
            //int row = gridConversion.CurrentCell.RowIndex;
            //gridConversion.Rows.RemoveAt(row);


            //add new row
            else
            { 
                foreach (var row in gridConversion.Rows.Cast<DataGridViewRow>())
                {
                    var conversionTech = row.DataBoundItem as ConversionTechPropertiesViewModel;
                    var nameCell = (DataGridViewComboBoxCell)row.Cells[1];

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


        }

        private void tableLayoutPanelMain_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}