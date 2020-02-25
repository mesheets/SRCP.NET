using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for SelectSrcpServer.
	/// </summary>
	public class SelectSrcpServer : UserControl
	{
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.ComboBox comboBoxConnections;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		bool start;
		private System.Windows.Forms.Button buttonEdit;

		ServerConnection connection;

        /// <summary>
        /// This event will be raised, if the parent form can be closed if it shows a modal dialog.
        /// </summary>
        public event EventHandler ModalDialogShouldBeClosed;

        protected void OnModalDialogShouldBeClosed(EventArgs args)
        {
            if (ModalDialogShouldBeClosed != null)
                ModalDialogShouldBeClosed(this, args);
        }

		protected bool Start
		{
			get
			{
				return start;
			}
			set
			{
				start = value;
			}
		}

		/// <summary>
		/// Use this constructor if you want intialize the control later.
		/// </summary>
		/// <param name="initialize">If this flag is false, call <see cref="Initialize"/> before using this class.</param>
		public SelectSrcpServer(bool initialize) : this(null, true, null, initialize)
		{
			
		}

		public SelectSrcpServer(string defaultConnectionName) : this(defaultConnectionName, true, null, true)
		{
		}

		public SelectSrcpServer() : this(null, true, null, true)
		{
		}
		
		public SelectSrcpServer(string defaultConnectionName, bool start) : this(defaultConnectionName, start, null, true)
		{
			
		}
		public SelectSrcpServer(string defaultConnectionName, bool start, string[] notShownConnections) : this(defaultConnectionName, start, notShownConnections, true)
		{
			
		}
		private SelectSrcpServer(string defaultConnectionName, bool start, string[] notShownConnections, bool initialize)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			if (initialize)
				Initialize(defaultConnectionName, start, notShownConnections);

            base.Text = new LocalizedString("SelectSRCPServer");
			
		}

		protected void Initialize(string defaultConnectionName, bool start, string[] notShownConnections)
		{
			this.start = start;
			
			foreach (ServerConnection connection in ServerConnection.ServerConnections)
			{
				bool isDefaultConnection = defaultConnectionName != null && connection.Name == defaultConnectionName;
				if (notShownConnections == null || 
					Array.IndexOf(notShownConnections, connection.Name) == -1 ||
					isDefaultConnection)
				{
					int index = comboBoxConnections.Items.Add(connection);
					if (isDefaultConnection)
						comboBoxConnections.SelectedIndex = index;
				}
			}
            if (defaultConnectionName == null && comboBoxConnections.Items.Count > 0)
                comboBoxConnections.SelectedIndex = 0;

			
		}

		public ServerConnection Connection
		{
			get
			{
				if (connection == null)
					connection = GetConnection();
				return connection;
			}
		}
		public DialogResult ShowDialog()
		{
			
			return ShowDialog(null);
		}
		public DialogResult ShowDialog(IWin32Window owner)
		{
			if (ServerConnection.ServerConnections.Count == 0)			
			{
				NewSrcpServer newConnection = new NewSrcpServer(start);
				DialogResult result = newConnection.ShowDialog(owner);
				if (result == DialogResult.OK)
				{
					connection = newConnection.Connection;
				}
				return result;
			}
			else
			{
				ModalDialog dialog = new ModalDialog(this);
                try
                {
                    this.ModalDialogShouldBeClosed += new EventHandler(dialog.OkClick);
                    dialog.OkPressed += new EventHandler(buttonOk_Click);
                    return dialog.ShowDialog(owner);
                }
                finally
                {
                    this.ModalDialogShouldBeClosed -= new EventHandler(dialog.OkClick);
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectSrcpServer));
            this.comboBoxConnections = new System.Windows.Forms.ComboBox();
            this.buttonNew = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxConnections
            // 
            this.comboBoxConnections.DisplayMember = "Path";
            this.comboBoxConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxConnections, "comboBoxConnections");
            this.comboBoxConnections.Name = "comboBoxConnections";
            this.comboBoxConnections.SelectedIndexChanged += new System.EventHandler(this.comboBoxConnections_SelectedIndexChanged);
            // 
            // buttonNew
            // 
            resources.ApplyResources(this.buttonNew, "buttonNew");
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonEdit
            // 
            resources.ApplyResources(this.buttonEdit, "buttonEdit");
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // SelectSrcpServer
            // 
            this.Controls.Add(this.buttonNew);
            this.Controls.Add(this.comboBoxConnections);
            this.Controls.Add(this.buttonEdit);
            this.Name = "SelectSrcpServer";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e)
		{
			connection = GetConnection();
		}

		private ServerConnection GetConnection()
		{
			Control control = null;
			try
			{
				control = comboBoxConnections;
				ServerConnection selectedConnection = (ServerConnection) comboBoxConnections.SelectedItem;
				if (selectedConnection == null)
					throw new LocalizedException("SelectConnection");
				if (start && !selectedConnection.Started)
					selectedConnection.Start();

				return selectedConnection;
			}
			catch (Exception)
			{
				if (control != null)
					control.Focus();
				throw;
			}
		}

		private void buttonNew_Click(object sender, System.EventArgs e)
		{
			NewSrcpServer newServer = new NewSrcpServer(start);
			if (DialogResult.OK == newServer.ShowDialog(this))
			{
				connection = newServer.Connection;
				if (!comboBoxConnections.Items.Contains(connection))
					comboBoxConnections.Items.Add(connection);
				comboBoxConnections.SelectedItem = connection;
				if (start)
				{
                    OnModalDialogShouldBeClosed(EventArgs.Empty);
				}
			}

		}

		private void buttonEdit_Click(object sender, System.EventArgs e)
		{
			ServerConnection connection = (ServerConnection) comboBoxConnections.SelectedItem;
			string nameOld = connection.Name;
			NewSrcpServer newServer = new NewSrcpServer(connection,  start);
			if (DialogResult.OK == newServer.ShowDialog(this))
			{
				if (newServer.Connection.Name != nameOld)
				{
					comboBoxConnections.Items.Remove(connection);
					int index = comboBoxConnections.Items.Add(newServer.Connection);
					comboBoxConnections.SelectedIndex = index;
				}
				connection = newServer.Connection;
			}

		}

		private void comboBoxConnections_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			buttonEdit.Enabled = comboBoxConnections.SelectedItem != null;
		}
	}
}
