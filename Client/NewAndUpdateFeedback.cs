using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for NewFeedback.
	/// </summary>
	public class NewAndUpdateFeedback : UserControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private SelectBus selectBus;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxParameters;
		private System.Windows.Forms.ComboBox comboBoxType;
		private System.Windows.Forms.Label label5;
		private ServerConnection serverConnection;


		private SrcpFeedback srcpFeedback;
		private SrcpFeedback current;

		public NewAndUpdateFeedback(SrcpFeedback current) : this()
		{
			Initialize(current.Connection, current);
		}
		public void Initialize(ServerConnection connection)
		{
			Initialize(connection, null);
		}
		public void Initialize(SrcpFeedback current)
		{
			Initialize(current.Connection, current);
		}

		private void Initialize(ServerConnection connection, SrcpFeedback current)
		{
			this.current = current;
			serverConnection = connection;

			selectBus.SetServer(serverConnection);
			if (current != null)
			{
				selectBus.Bus = current.Bus;
				comboBoxType.Text = current.Type;
				comboBoxParameters.Text = current.Parameters;
			}
			else
			{
				selectBus.Bus = 8;
			}

			comboBoxType.SelectedIndex = 0;
			comboBoxParameters.SelectedIndex = 0;
		}

		/// <summary>
		/// Call <see cref="Initialize"/> if you use this constructor.
		/// </summary>
		public NewAndUpdateFeedback() 
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			UpdateComboBoxEntries();			
		}
		public NewAndUpdateFeedback(ServerConnection connection) : this()
		{
			Initialize(connection, null);

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewAndUpdateFeedback));
            this.selectBus = new nSrcp.Client.SelectBus();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxParameters = new System.Windows.Forms.ComboBox();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // selectBus
            // 
            resources.ApplyResources(this.selectBus, "selectBus");
            this.selectBus.Name = "selectBus";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxParameters
            // 
            this.comboBoxParameters.DisplayMember = "Path";
            resources.ApplyResources(this.comboBoxParameters, "comboBoxParameters");
            this.comboBoxParameters.Name = "comboBoxParameters";
            // 
            // comboBoxType
            // 
            this.comboBoxType.DisplayMember = "Path";
            this.comboBoxType.Items.AddRange(new object[] {
            resources.GetString("comboBoxType.Items"),
            resources.GetString("comboBoxType.Items1"),
            resources.GetString("comboBoxType.Items2"),
            resources.GetString("comboBoxType.Items3")});
            resources.ApplyResources(this.comboBoxType, "comboBoxType");
            this.comboBoxType.Name = "comboBoxType";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // NewAndUpdateFeedback
            // 
            this.Controls.Add(this.comboBoxParameters);
            this.Controls.Add(this.selectBus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.label5);
            this.Name = "NewAndUpdateFeedback";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

		}
		#endregion

		private int inputs = 16;

		[DefaultValue(16)]
		public int Inputs
		{
			get
			{
				return inputs;
			}
			set
			{
				inputs = value;
				UpdateComboBoxEntries();
			}
		}
		
		void UpdateComboBoxEntries()
		{
			string oldText = comboBoxParameters.Text;
			int oldIndex = comboBoxParameters.SelectedIndex;
			comboBoxParameters.Items.Clear();
			for (int i = 1; i <= 2; i++)
			{
				int modules = ((inputs - 1) / 16) + 1;
				comboBoxParameters.Items.Add(string.Format("LPT{0} {1}", i, modules));
			}			for (int i = 1; i <= 2; i++)
			{
				int modules = ((inputs - 1) / 16) + 1;
				comboBoxParameters.Items.Add(string.Format("LPT{0} {1} 35 100", i, modules));
			}

			if (oldIndex != -1)
			{
				comboBoxParameters.SelectedIndex = oldIndex;
			}
			else
			{
				comboBoxParameters.Text = oldText;
			}
			
		}


		public SrcpFeedback GetSrcpFeedback()
		{
			if (srcpFeedback == null)
			{
				Control control = null;
				try
				{
	
					control = selectBus;
					int bus = selectBus.Bus;

					control = comboBoxType;
					string type = comboBoxType.Text;

					control = comboBoxParameters;
					string parameters = comboBoxParameters.Text;

					control = null;

					if (current == null)
					{
						current = new SrcpFeedback(serverConnection);
					}
					current.Init(bus, type, parameters);
					srcpFeedback = current;

					FindForm().DialogResult = DialogResult.OK;
				}
				catch (Exception)
				{
					if (control != null)
						control.Focus();
					throw;
				}
			}
			return srcpFeedback;
			
		}



	
	}
}
