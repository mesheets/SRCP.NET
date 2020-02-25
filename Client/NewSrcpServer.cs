using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for NewSrcpServer.
	/// </summary>
	public class NewSrcpServer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxServer;
		private System.Windows.Forms.TextBox textBoxPort;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		bool start = true;
		private System.Windows.Forms.CheckBox checkBoxAutoStartSRCPServer;
		private System.Windows.Forms.Panel panelAutostart;
		private System.Windows.Forms.Button buttonSelectSRCPServerPath;
		private System.Windows.Forms.CheckBox checkBoxAutostopSRCPServer;
		private System.Windows.Forms.TextBox textBoxSrcpServerPath;
		private System.Windows.Forms.OpenFileDialog openFileDialogSrcpServerPath;

		public ServerConnection Connection;

		public NewSrcpServer(bool start) : this()
		{
			this.start = start;
		}
		public NewSrcpServer(ServerConnection connection) : this(connection, connection.Started)
		{
		}
		public NewSrcpServer(ServerConnection connection, bool start) : this()
		{
			if (connection == null)
				throw new ArgumentNullException("connection");
			this.start = start;
			Connection = connection;
			textBoxName.Text = connection.Name;
			textBoxServer.Text = connection.Server;
			textBoxPort.Text = connection.Port.ToString();
			checkBoxAutoStartSRCPServer.Checked = connection.SrcpServerAutoStart;
			checkBoxAutostopSRCPServer.Checked = connection.SrcpServerAutoStop;
			textBoxSrcpServerPath.Text = connection.SrcpServerPath;
		}

		public NewSrcpServer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewSrcpServer));
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxAutoStartSRCPServer = new System.Windows.Forms.CheckBox();
            this.panelAutostart = new System.Windows.Forms.Panel();
            this.buttonSelectSRCPServerPath = new System.Windows.Forms.Button();
            this.checkBoxAutostopSRCPServer = new System.Windows.Forms.CheckBox();
            this.textBoxSrcpServerPath = new System.Windows.Forms.TextBox();
            this.openFileDialogSrcpServerPath = new System.Windows.Forms.OpenFileDialog();
            this.panelAutostart.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxServer
            // 
            resources.ApplyResources(this.textBoxServer, "textBoxServer");
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.TextChanged += new System.EventHandler(this.textBoxServer_TextChanged);
            // 
            // textBoxPort
            // 
            resources.ApplyResources(this.textBoxPort, "textBoxPort");
            this.textBoxPort.Name = "textBoxPort";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // checkBoxAutoStartSRCPServer
            // 
            resources.ApplyResources(this.checkBoxAutoStartSRCPServer, "checkBoxAutoStartSRCPServer");
            this.checkBoxAutoStartSRCPServer.Name = "checkBoxAutoStartSRCPServer";
            this.checkBoxAutoStartSRCPServer.CheckedChanged += new System.EventHandler(this.checkBoxAutoStartSRCPServer_CheckedChanged);
            // 
            // panelAutostart
            // 
            this.panelAutostart.Controls.Add(this.buttonSelectSRCPServerPath);
            this.panelAutostart.Controls.Add(this.checkBoxAutostopSRCPServer);
            this.panelAutostart.Controls.Add(this.textBoxSrcpServerPath);
            resources.ApplyResources(this.panelAutostart, "panelAutostart");
            this.panelAutostart.Name = "panelAutostart";
            // 
            // buttonSelectSRCPServerPath
            // 
            resources.ApplyResources(this.buttonSelectSRCPServerPath, "buttonSelectSRCPServerPath");
            this.buttonSelectSRCPServerPath.Name = "buttonSelectSRCPServerPath";
            this.buttonSelectSRCPServerPath.Click += new System.EventHandler(this.buttonSelectSRCPServerPath_Click);
            // 
            // checkBoxAutostopSRCPServer
            // 
            resources.ApplyResources(this.checkBoxAutostopSRCPServer, "checkBoxAutostopSRCPServer");
            this.checkBoxAutostopSRCPServer.Name = "checkBoxAutostopSRCPServer";
            // 
            // textBoxSrcpServerPath
            // 
            resources.ApplyResources(this.textBoxSrcpServerPath, "textBoxSrcpServerPath");
            this.textBoxSrcpServerPath.Name = "textBoxSrcpServerPath";
            // 
            // openFileDialogSrcpServerPath
            // 
            this.openFileDialogSrcpServerPath.DefaultExt = "*.exe";
            resources.ApplyResources(this.openFileDialogSrcpServerPath, "openFileDialogSrcpServerPath");
            // 
            // NewSrcpServer
            // 
            this.AcceptButton = this.buttonOk;
            this.CancelButton = this.buttonCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panelAutostart);
            this.Controls.Add(this.checkBoxAutoStartSRCPServer);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxServer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewSrcpServer";
            this.ShowInTaskbar = false;
            this.panelAutostart.ResumeLayout(false);
            this.panelAutostart.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e)
		{
			Control control = null;
			try
			{
				control = textBoxName;
				string name = textBoxName.Text;
				if (name == "")
				{
					throw new LocalizedException("EnterAName");	
				}

				if (ServerConnection.ServerConnections.Contains(name))
				{
                    if (DialogResult.No == MessageBox.Show(this, LocalizedString.Format("NameAlreadyExist", name), null, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						textBoxName.Focus();
						return;
					}
				}
				control = textBoxServer;
				string server = textBoxServer.Text;
				if (server == "")
				{
                    throw new LocalizedException("EnterAServer");	
				}

				control = textBoxPort;
				int port;
				try
				{
					port = int.Parse(textBoxPort.Text);
					if (port < 1 || port > 65535)
                        throw new LocalizedException("PortMustBetween1And65535");	
				}
				catch
				{
                    throw new LocalizedException("PortMustBetween1And65535");	
				
				}
				control = textBoxSrcpServerPath;
				string srcpServerPath = string.Empty;
				if (checkBoxAutoStartSRCPServer.Checked)
				{
					srcpServerPath = textBoxSrcpServerPath.Text;
					if (srcpServerPath == "")
                        throw new LocalizedException("EnterLocalPathToSRCPServer");

					if (!File.Exists(srcpServerPath))
					{
                        throw new LocalizedException("SRCPServerDoesNotExist", srcpServerPath);
					}
				}

				control = null;
				if (Connection != null)
				{
					ServerConnection.ServerConnections.Remove(Connection.Name);
				}

				Connection = new ServerConnection(name, server, port, checkBoxAutoStartSRCPServer.Checked, checkBoxAutostopSRCPServer.Checked, srcpServerPath, start);
				
				DialogResult = DialogResult.OK;
			}
			catch (Exception ex)
			{
				if (control != null)
					control.Focus();
				LocalizedException.ShowMessage(this, ex);
				return;
			}
		}

		private void checkBoxAutoStartSRCPServer_CheckedChanged(object sender, System.EventArgs e)
		{
			panelAutostart.Enabled = checkBoxAutoStartSRCPServer.Checked;
		}

		private void textBoxServer_TextChanged(object sender, System.EventArgs e)
		{
			string server = textBoxServer.Text.ToLower();
			if (server == "localhost" ||
				server == "127.0.0.1" ||
				server == Environment.MachineName)
			{
				checkBoxAutoStartSRCPServer.Enabled = true;
			}
			else
			{
				checkBoxAutoStartSRCPServer.Enabled = false;
				checkBoxAutoStartSRCPServer.Checked = false;
			}
		}

		private void buttonSelectSRCPServerPath_Click(object sender, System.EventArgs e)
		{
			openFileDialogSrcpServerPath.FileName = textBoxSrcpServerPath.Text;
			if (DialogResult.OK == openFileDialogSrcpServerPath.ShowDialog(this))
			{
				textBoxSrcpServerPath.Text = openFileDialogSrcpServerPath.FileName;
			}

		}
	}
}
