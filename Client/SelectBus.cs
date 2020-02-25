using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using ComponentModel_DesignerSerializationVisibility = System.ComponentModel.DesignerSerializationVisibility;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for SelectBus.
	/// </summary>
	public class SelectBus : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelNotSupported;
		private System.Windows.Forms.ComboBox comboBoxSelectBus;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SelectBus()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		public void SetServer(ServerConnection serverConnection)
		{
			comboBoxSelectBus.Items.Clear();
			comboBoxSelectBus.Items.AddRange(serverConnection.BusInformations);
			labelNotSupported.Visible = !serverConnection.SupportBusses;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Bus
		{
			get
			{
				BusInformation busInformation = (BusInformation) comboBoxSelectBus.SelectedItem;	
				if (busInformation == null)
				{
					comboBoxSelectBus.Focus();
                    throw new LocalizedException("SelectABus");
				}
				return busInformation.Number;
			}
			set
			{
				int newIndex = -1;
				for (int i = 0; i < comboBoxSelectBus.Items.Count; i++)
				{
					BusInformation busInformation = (BusInformation) comboBoxSelectBus.Items[i];
					if (busInformation.Number == value)
					{
						newIndex = i;
						break;
					}					
				}
				comboBoxSelectBus.SelectedIndex = newIndex;
			}
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectBus));
            this.comboBoxSelectBus = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelNotSupported = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxSelectBus
            // 
            resources.ApplyResources(this.comboBoxSelectBus, "comboBoxSelectBus");
            this.comboBoxSelectBus.DisplayMember = "Path";
            this.comboBoxSelectBus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectBus.Name = "comboBoxSelectBus";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // labelNotSupported
            // 
            resources.ApplyResources(this.labelNotSupported, "labelNotSupported");
            this.labelNotSupported.Name = "labelNotSupported";
            // 
            // SelectBus
            // 
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxSelectBus);
            this.Controls.Add(this.labelNotSupported);
            this.Name = "SelectBus";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

		}
		#endregion
	}
}
