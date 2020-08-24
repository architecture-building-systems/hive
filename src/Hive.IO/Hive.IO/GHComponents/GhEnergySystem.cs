using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Rhino.Geometry;
using Hive.IO.EnergySystems;
using Hive.IO.GhParametricInputs;
//using Newtonsoft.Json;

namespace Hive.IO.GHComponents
{
    public class GhEnergySystem : GH_Component
    {
        public string Form_SystemType { get; set; }
        public double Form_pv_eff { get; set; }
        public double Form_thermal_eff { get; set; }
        public double Form_pv_cost { get; set; }
        public double Form_pv_co2 { get; set; }
        public string Form_pv_name { get; set; }
        private int indexNow { get; set; }


        public GhEnergySystem()
          : base("Input EnergySystems Hive", "HiveInputEnergySystems", "Hive.IO.EnergySystems input component (solar energy systems, other conversion technologies, emitters)." +
                "\nThe component opens a Form upon double click, where details of the solar energy system can be specified." +
                "\nPossible technologies are Photovoltaic (PV), Solar Thermal (ST), hybrid PVT, or Ground Collector (GC).", "[hive]", "IO") { indexNow = 0; }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh geometries of the solar energy systems (Photovolatic, Solar Thermal, or hybrid PVT)", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager.AddGenericParameter("SolarTechJson", "SolarTechJson", "List of jsons describing solar technologies. One json per mesh", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddGenericParameter("ConversionTechJson", "ConversionTechJson", "Json describing all other used conversion technologies (ASHP, boiler, CHP, etc", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddGenericParameter("EmitterJson", "EmitterJson", "Json describing emitter properties", GH_ParamAccess.list);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddGenericParameter("Hive.IO.EnergySystems.ConversionTech", "ConversionTech", "Hive.IO.EnergySystems.ConversionTech, such as PV, ST, PVT or GC, ASHP, CHP, boiler, etc.", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Hive.IO.EnergySystems.Emitter", "Emitter", "Hive.IO.EnergySystems.Emitter (e.g. Radiator, floor heating, cooling panel, etc. Will be depricated for Hive 1.x and become part of the Building.Zone.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Energy Systems", "EnergySystems", "Building Energy Systems of type <Hive.IO.EnergySystems.>, such as Emitters, ConversionTech, SolarTech, etc.", GH_ParamAccess.list);
        }


        #region WindowsForm
        public override void CreateAttributes()
        {
            m_attributes = new MySpecialComponentAttributes(this);
        }
        private class MySpecialComponentAttributes : GH_ComponentAttributes
        {
            public MySpecialComponentAttributes(IGH_Component component)
            : base(component)
            { }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                (Owner as GhEnergySystem)?.DisplayForm();
                return GH_ObjectResponse.Handled;
            }
        }

        FormEnSysSolar _form;
        public void DisplayForm()
        {
            if (_form != null)
                return;

            _form = new FormEnSysSolar();
            _form.comboBox1.SelectedIndex = indexNow;
            _form.textBox1.Text = _form.ElectricEfficiency[indexNow].ToString();// Convert.ToString(Form_pv_eff);
            _form.textBox2.Text = _form.Cost[indexNow].ToString();
            _form.textBox3.Text = _form.CO2[indexNow].ToString();
            _form.textBox4.Text = _form.ThermalEfficiency[indexNow].ToString();
            _form.pictureBox1.Image = _form.Image[indexNow];
            _form.helpProvider1.SetHelpString(_form.pictureBox1, _form.HelperText[indexNow]);

            _form.FormClosed += OnFormClosed;

            _form.comboBox1.SelectedIndexChanged += ComboBox1_ItemChanged;
            _form.textBox1.TextChanged += TextBox_TextChanged;
            _form.textBox2.TextChanged += TextBox_TextChanged;
            _form.textBox3.TextChanged += TextBox_TextChanged;
            _form.textBox4.TextChanged += TextBox_TextChanged;

            _form.radioButton1.CheckedChanged += RadioButton_Changed;
            _form.radioButton2.CheckedChanged += RadioButton_Changed;
            _form.radioButton3.CheckedChanged += RadioButton_Changed;
            _form.radioButton4.CheckedChanged += RadioButton_Changed;

            GH_WindowsFormUtil.CenterFormOnCursor(_form, true);
            _form.Show(Grasshopper.Instances.DocumentEditor);
            _form.Location = Cursor.Position;
        }

        private void Form_Update_Display()
        {
            // when radio buttons change, text on the form needs to change
            indexNow = 0;
            _form.comboBox1.SelectedIndex = indexNow;
            _form.textBox1.Text = _form.ElectricEfficiency[indexNow].ToString();// Convert.ToString(Form_pv_eff);
            _form.textBox2.Text = _form.Cost[indexNow].ToString();
            _form.textBox3.Text = _form.CO2[indexNow].ToString();
            _form.textBox4.Text = _form.ThermalEfficiency[indexNow].ToString();
            _form.pictureBox1.Image = _form.Image[indexNow];
            _form.helpProvider1.SetHelpString(_form.pictureBox1, _form.HelperText[indexNow]);
        }

        private void RadioButton_Changed(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                     switch (rb.Name)
                    {
                        default:
                        case "radioButton1":
                            _form.LoadData("pv");
                            break;
                        case "radioButton2":
                            _form.LoadData("st");
                            break;
                        case "radioButton3":
                            _form.LoadData("pvt");
                            break;
                        case "radioButton4":
                            _form.LoadData("gc");
                            break;
                    }
                }
            }
            Form_Update_Display();
            //Form_Update();
        }

        private void ComboBox1_ItemChanged(object sender, EventArgs e)
        {
            int i = _form.comboBox1.SelectedIndex;
            
            _form.pictureBox1.Image = _form.Image[i];
            _form.textBox1.Text = _form.ElectricEfficiency[i].ToString();
            _form.textBox2.Text = _form.Cost[i].ToString();
            _form.textBox3.Text = _form.CO2[i].ToString();
            _form.textBox4.Text = _form.ThermalEfficiency[i].ToString();
            _form.helpProvider1.SetHelpString(_form.pictureBox1, _form.HelperText[i]);

            //Form_Update();
        }
        
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            //Form_Update();
        }

        private void Form_Update()
        {
            Form_pv_eff = Convert.ToDouble(_form.textBox1.Text);
            Form_pv_cost = Convert.ToDouble(_form.textBox2.Text);
            Form_pv_co2 = Convert.ToDouble(_form.textBox3.Text);
            Form_thermal_eff = Convert.ToDouble(_form.textBox4.Text);
            Form_pv_name = _form.comboBox1.SelectedItem.ToString();
            Form_SystemType = _form.SystemType;

            indexNow = _form.comboBox1.SelectedIndex;

            ExpireSolution(true);
        }

        private void OnFormClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            Form_Update();
            _form = null;
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Show UI", ShowUiClicked, null, true, false);
        }
        private void ShowUiClicked(object sender, EventArgs e)
        {
            DisplayForm();
        }
        #endregion 


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Mesh> meshList = new List<Mesh>();
            DA.GetDataList(0, meshList); 

            var solarTechProperties = new List<SolarTechProperties>();
            DA.GetDataList(1, solarTechProperties);

            ConversionTechProperties conversionTechProperties = null;
            DA.GetData(2, ref conversionTechProperties);

            var emitterProperties = new List<EmitterProperties>();
            DA.GetDataList(3, emitterProperties);

            // feed the list into the listbox on the windows form


            // by default, its all PV



            // To do Daren: coding
            // To do Amr: UI/UX design
            // when opening the form, the user can select certain surfaces and change them to ST, GC, PVT, or PV individually

            

            var conversionTech = new List<ConversionTech>();


            if (solarTechProperties.Count == 0)
            {
                foreach (Mesh mesh in meshList)
                {
                    if (Form_SystemType == "pv") 
                        conversionTech.Add(new Photovoltaic(Form_pv_cost, Form_pv_co2, mesh, Form_pv_name, Form_pv_eff));
                    else if (Form_SystemType == "pvt") 
                        conversionTech.Add(new PVT(Form_pv_cost, Form_pv_co2, mesh, Form_pv_name, Form_pv_eff, Form_thermal_eff));
                    else if (Form_SystemType == "st") 
                        conversionTech.Add(new SolarThermal(Form_pv_cost, Form_pv_co2, mesh, Form_pv_name, Form_thermal_eff));
                    else 
                        conversionTech.Add(new GroundCollector(Form_pv_cost, Form_pv_co2, mesh, Form_pv_name)); // Form_thermal_eff, 
                }
            }
            else
            {
                for (int i=0; i<solarTechProperties.Count; i++)
                {
                    var solarProperties = solarTechProperties[i];
                    Mesh mesh = meshList[i];

                    if (solarProperties.Type == "PV")
                        conversionTech.Add(new Photovoltaic(solarProperties.InvestmentCost, solarProperties.EmbodiedEmissions, mesh, solarProperties.Technology, solarProperties.ElectricEfficiency));
                    else if (solarProperties.Type == "PVT")
                        conversionTech.Add(new PVT(solarProperties.InvestmentCost, solarProperties.EmbodiedEmissions, mesh, solarProperties.Technology, solarProperties.ElectricEfficiency, solarProperties.ThermalEfficiency));
                    else if (solarProperties.Type == "ST")
                        conversionTech.Add(new SolarThermal(solarProperties.InvestmentCost, solarProperties.EmbodiedEmissions, mesh, solarProperties.Technology, solarProperties.ThermalEfficiency));
                    else
                        conversionTech.Add(new GroundCollector(solarProperties.InvestmentCost, solarProperties.EmbodiedEmissions, mesh, solarProperties.Technology));
                }
            }

            if(conversionTechProperties == null)
            {
                conversionTech.Add(new GasBoiler(100.0, 100.0, 10.0, 0.9));
            }
            else
            {
                if (conversionTechProperties.ASHPCapacity > 0.0)
                    conversionTech.Add(new AirSourceHeatPump(conversionTechProperties.ASHPCost, conversionTechProperties.ASHPEmissions, conversionTechProperties.ASHPCapacity, conversionTechProperties.ASHPEtaRef));
                if (conversionTechProperties.GasBoilerCapacity > 0.0)
                    conversionTech.Add(new GasBoiler(conversionTechProperties.GasBoilerCost, conversionTechProperties.GasBoilerEmissions, conversionTechProperties.GasBoilerCapacity, conversionTechProperties.GasBoilerEfficiency));
                if (conversionTechProperties.CHPCapacity > 0.0)
                    conversionTech.Add(new CombinedHeatPower(conversionTechProperties.CHPCost, conversionTechProperties.CHPEmissions, conversionTechProperties.CHPCapacity, conversionTechProperties.CHPHTP, conversionTechProperties.CHPEffElec));
                if (conversionTechProperties.ChillerCapacity > 0.0)
                    conversionTech.Add(new Chiller(conversionTechProperties.ChillerCost, conversionTechProperties.ChillerEmissions, conversionTechProperties.ChillerCapacity, conversionTechProperties.ChillerEtaRef));
                if (conversionTechProperties.HeatExchangerCapacity > 0.0)
                    conversionTech.Add(new HeatCoolingExchanger(conversionTechProperties.HeatExchangerCost, conversionTechProperties.HeatExchangerEmissions, conversionTechProperties.HeatExchangerCapacity, conversionTechProperties.HeatExchangerLosses));
                if (conversionTechProperties.CoolExchangerCapacity > 0.0)
                    conversionTech.Add(new HeatCoolingExchanger(conversionTechProperties.CoolExchangerCost, conversionTechProperties.CoolExchangerEmissions, conversionTechProperties.CoolExchangerCapacity, conversionTechProperties.CoolExchangerLosses, false, true));
            }

            var emitters = new List<Emitter>();
            if (emitterProperties.Count == 0)
            {
                emitters.Add(new Radiator(100.0, 100.0, true, false, 65.0, 55.0));
                emitters[0].SetEmitterName("ConventionalRadiator");
                emitters.Add(new AirDiffuser(100.0, 100.0, false, true, 20.0, 25.0));
                emitters[1].SetEmitterName("AirDiffuser");
            }
            else
            {
                int counter = 0;
                foreach (EmitterProperties emProp in emitterProperties)
                {
                    if (emProp.IsRadiation)
                        emitters.Add(new Radiator(emProp.InvestmentCost, emProp.EmbodiedEmissions, emProp.IsHeating, emProp.IsCooling, emProp.SupplyTemperature, emProp.ReturnTemperature));
                    else
                        emitters.Add(new AirDiffuser(emProp.InvestmentCost, emProp.EmbodiedEmissions, emProp.IsHeating, emProp.IsCooling, emProp.SupplyTemperature, emProp.ReturnTemperature));
                    emitters[counter].SetEmitterName(emProp.Name);
                    counter++;
                }
            }

            var obj = new List<object>();
            foreach (object o in conversionTech)
                obj.Add(o);
            foreach (object e in emitters)
                obj.Add(e);
            DA.SetDataList(0, obj);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Hive.IO.Properties.Resources.IO_Solartech;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("59389231-9a1b-4732-99dd-2bdda6b78bb8"); }
        }
    }
}