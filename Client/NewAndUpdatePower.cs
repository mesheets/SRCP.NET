using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for NewPower.
	/// </summary>
	public class NewAndUpdatePower : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private SelectBus selectBus;


		private ServerConnection serverConnection;
		private SrcpPower srcpPower;
		private SrcpPower current;

		public NewAndUpdatePower(ServerConnection connection) : this()
		{
			Initialize(connection);
		}

		public NewAndUpdatePower(SrcpPower current) : this()
		{
			Initialize(current);
		}

		public NewAndUpdatePower()
		{
			InitializeComponent();	
		}

		public void Initialize(ServerConnection connection)
		{
			Initialize(connection, null);
		}
		public void Initialize(SrcpPower current)
		{
			Initialize(current.Connection, current);
		}

		void Initialize(ServerConnection connection, SrcpPower current)
		{
			serverConnection = connection;
			selectBus.SetServer(connection);
			if (current != null)
			{
				this.current = current;
				selectBus.Bus = current.Bus;
			}
			else
			{
				selectBus.Bus = 1;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewAndUpdatePower));
            this.selectBus = new nSrcp.Client.SelectBus();
            this.SuspendLayout();
            // 
            // selectBus
            // 
            resources.ApplyResources(this.selectBus, "selectBus");
            this.selectBus.Name = "selectBus";
            // 
            // NewAndUpdatePower
            // 
            this.Controls.Add(this.selectBus);
            this.Name = "NewAndUpdatePower";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

		}
		#endregion



		public SrcpPower GetSrcpPower()
		{
			if (srcpPower == null)
			{
				Control control = null;
				try
				{
	
					control = selectBus;
					int bus = selectBus.Bus;

					control = null;

					if (current == null)
					{
						current = new SrcpPower(serverConnection);
					}
					current.Init(bus);
					srcpPower = current;
				}
				catch
				{
					if (control != null)
						control.Focus();
					throw;
				}
			}
			return srcpPower;
		}

	}
}
