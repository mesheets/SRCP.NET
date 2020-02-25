using System;
using System.Windows.Forms;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for NewGL.
	/// </summary>
	public class NewAndUpdateGenericLoc : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private SelectBus selectBus;
		private System.Windows.Forms.ComboBox comboBoxType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboBoxSteps;

		private SrcpGenericLoc srcpGenericLoc;
		private SrcpGenericLoc current;
        private TextBox textBoxFunctions;
		private ServerConnection serverConnection;

		public NewAndUpdateGenericLoc(ServerConnection connection) : this()
		{
			Initialize(connection);
		}

		public NewAndUpdateGenericLoc(SrcpGenericLoc current) : this()
		{
			Initialize(current);
		}

		public void Initialize(ServerConnection connection)
		{
			Initialize(connection, null);
		}
		public void Initialize(SrcpGenericLoc current)
		{
			Initialize(current.Connection, current);
		}

		void Initialize(ServerConnection connection, SrcpGenericLoc current)
		{
			this.current = current;
			serverConnection = connection;
		    selectBus.SetServer(serverConnection);
			if (current != null)
			{
				textBoxAddress.Text = current.Address.ToString();
				selectBus.Bus = current.Bus;
				foreach (GlType type in comboBoxType.Items)
				{
					if (type.Protocol == current.Protocol)
					{
						comboBoxType.SelectedItem = type;
						break;
					}
				}
				comboBoxSteps.Text = current.Steps.ToString();
				textBoxFunctions.Text = current.NumberOfFunctions.ToString();
			}
			else
			{
				selectBus.Bus = 1;
			}


			
		}

		public NewAndUpdateGenericLoc()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			comboBoxType.Items.AddRange(new object[]
				{
					new GlType("M 1", "Motorola I"),
					new GlType("M 2", "Motorola II"),
					new GlType("M 4", new LocalizedString("MotorolaII256Addresses")),
					new GlType("N 1", new LocalizedString("DCCShortAddress")),
					new GlType("N 2", new LocalizedString("DCCLongAddress")),
					new GlType("L", "Loconect"),
					new GlType("S", "Selectrix"),
					new GlType("P", new LocalizedString("ProtocolByServer")),
			});
			comboBoxType.SelectedIndex = 1;


		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewAndUpdateGenericLoc));
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.selectBus = new nSrcp.Client.SelectBus();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxSteps = new System.Windows.Forms.ComboBox();
            this.textBoxFunctions = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxAddress
            // 
            resources.ApplyResources(this.textBoxAddress, "textBoxAddress");
            this.textBoxAddress.Name = "textBoxAddress";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // selectBus
            // 
            resources.ApplyResources(this.selectBus, "selectBus");
            this.selectBus.Name = "selectBus";
            // 
            // comboBoxType
            // 
            this.comboBoxType.DisplayMember = "Path";
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxType, "comboBoxType");
            this.comboBoxType.Name = "comboBoxType";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // comboBoxSteps
            // 
            this.comboBoxSteps.Items.AddRange(new object[] {
            resources.GetString("comboBoxSteps.Items"),
            resources.GetString("comboBoxSteps.Items1"),
            resources.GetString("comboBoxSteps.Items2"),
            resources.GetString("comboBoxSteps.Items3")});
            resources.ApplyResources(this.comboBoxSteps, "comboBoxSteps");
            this.comboBoxSteps.Name = "comboBoxSteps";
            this.comboBoxSteps.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxSteps_Validating);
            // 
            // textBoxFunctions
            // 
            resources.ApplyResources(this.textBoxFunctions, "textBoxFunctions");
            this.textBoxFunctions.Name = "textBoxFunctions";
            // 
            // NewAndUpdateGenericLoc
            // 
            this.Controls.Add(this.textBoxFunctions);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.textBoxAddress);
            this.Controls.Add(this.selectBus);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxSteps);
            this.Name = "NewAndUpdateGenericLoc";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		struct GlType
		{
			public readonly string Display;
			public readonly string Protocol;
			
			public GlType(string protocol, string display)
			{
				Display = display;
				Protocol = protocol;
			}

			public override string ToString()
			{
				return Display;
			}
		}

		public SrcpGenericLoc GetSrcpGenericLoc()
		{
			if (srcpGenericLoc == null)
			{
				Control control = null;
				try
				{
	
					control = selectBus;
					int bus = selectBus.Bus;

					control = textBoxAddress;
					int address = int.Parse(textBoxAddress.Text);

					control = comboBoxType;
					GlType type = (GlType) comboBoxType.SelectedItem;

					control = comboBoxSteps;
					int steps = int.Parse(comboBoxSteps.Text);

                    control = textBoxFunctions;
					int functions = int.Parse(textBoxFunctions.Text);

					control = null;

					if (current == null)
					{
						current = new SrcpGenericLoc(serverConnection);
					}
					current.Init(bus, address, type.Protocol, steps, functions);
					srcpGenericLoc = current;

				}
				catch
				{
					if (control != null)
						control.Focus();
					throw;
				}
			}
			return srcpGenericLoc;
		}

		private void comboBoxSteps_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				int value = int.Parse(comboBoxSteps.Text);
				if (value < 0)
					throw new ApplicationException("Value must be greater than 0");
			}
			catch
			{
				e.Cancel = true;
				MessageBox.Show(this, "Enter a value greater than 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

	}
}
