using System;
using System.Windows.Forms;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for NewGL.
	/// </summary>
	public class NewAndUpdateGenericAccessory : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox textBoxAddress;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxAdditionalParameters;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private SelectBus selectBus;
		private System.Windows.Forms.ComboBox comboBoxType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxPorts;

		private SrcpGenericAccessory srcpGenericAccessoire;
		private SrcpGenericAccessory current;
		private ServerConnection serverConnection;

		public NewAndUpdateGenericAccessory(ServerConnection connection) : this()
		{
			Initialize(connection);
		}

		public NewAndUpdateGenericAccessory(SrcpGenericAccessory current) : this()
		{
			Initialize(current);
		}

		public void Initialize(ServerConnection connection)
		{
			Initialize(connection, null);
		}
		public void Initialize(SrcpGenericAccessory current)
		{
			Initialize(current.Connection, current);
		}

		void Initialize(ServerConnection connection, SrcpGenericAccessory current)
		{
			this.current = current;
			serverConnection = connection;
			selectBus.SetServer(serverConnection);
			if (current != null)
			{
				textBoxAddress.Text = current.Address.ToString();
				selectBus.Bus = current.Bus;
                foreach (ProtocolType type in comboBoxType.Items)
				{
					if (type.Protocol == current.Protocol)
					{
						comboBoxType.SelectedItem = type;
						break;
					}
				}
				textBoxPorts.Text = current.NumberOfPorts.ToString();
                textBoxAdditionalParameters.Text = current.AdditionalParameters;
			}
			else
			{
				selectBus.Bus = 5;
			}


			
		}

        public NewAndUpdateGenericAccessory()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			comboBoxType.Items.AddRange(new object[]
				{
					new ProtocolType("M", "Motorola"),
					new ProtocolType("N", "DCC"),
					new ProtocolType("L", "Loconect"),
					new ProtocolType("S", "Selectrix"),
                    new ProtocolType("P", new LocalizedString("ProtocolByServer"))
			});
			comboBoxType.SelectedIndex = 0;


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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewAndUpdateGenericAccessory));
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxAdditionalParameters = new System.Windows.Forms.TextBox();
            this.selectBus = new nSrcp.Client.SelectBus();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPorts = new System.Windows.Forms.TextBox();
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
            // textBoxAdditionalParameters
            // 
            resources.ApplyResources(this.textBoxAdditionalParameters, "textBoxAdditionalParameters");
            this.textBoxAdditionalParameters.Name = "textBoxAdditionalParameters";
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
            // textBoxPorts
            // 
            resources.ApplyResources(this.textBoxPorts, "textBoxPorts");
            this.textBoxPorts.Name = "textBoxPorts";
            this.textBoxPorts.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxSteps_Validating);
            // 
            // NewAndUpdateGenericAccessory
            // 
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxAdditionalParameters);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.textBoxAddress);
            this.Controls.Add(this.selectBus);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxPorts);
            this.Name = "NewAndUpdateGenericAccessory";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		struct ProtocolType
		{
			public readonly string Display;
			public readonly string Protocol;

            public ProtocolType(string protocol, string display)
			{
				Display = display;
				Protocol = protocol;
			}

			public override string ToString()
			{
				return Display;
			}
		}

        public SrcpGenericAccessory GetSrcpGenericAccessory()
		{
			if (srcpGenericAccessoire == null)
			{
				Control control = null;
				try
				{
	
					control = selectBus;
					int bus = selectBus.Bus;

					control = textBoxAddress;
					int address = int.Parse(textBoxAddress.Text);

					control = comboBoxType;
                    ProtocolType type = (ProtocolType)comboBoxType.SelectedItem;

					control = textBoxPorts;
					int numberOfPorts = int.Parse(textBoxPorts.Text);

					control = textBoxAdditionalParameters;
                    string additionalParameters = textBoxAdditionalParameters.Text;

					control = null;

					if (current == null)
					{
						current = new SrcpGenericAccessory(serverConnection);
					}
                    current.Init(bus, address, type.Protocol, additionalParameters, numberOfPorts);
					srcpGenericAccessoire = current;

				}
				catch
				{
					if (control != null)
						control.Focus();
					throw;
				}
			}
			return srcpGenericAccessoire;
		}

		private void comboBoxSteps_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				int value = int.Parse(textBoxPorts.Text);
				if (value < 0)
					throw new LocalizedException("ValueMustGreaterThanZero");
			}
			catch (Exception ex)
			{
				e.Cancel = true;
                textBoxPorts.Focus();
                LocalizedException.ShowMessage(this, ex);
			}
		}

	}
}
