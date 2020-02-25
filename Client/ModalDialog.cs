using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace nSrcp.Client
{
	/// <summary>
	/// Helper class for showing controls as modal dialog.
	/// </summary>
	internal class ModalDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Panel panel;

		/// <summary>
		/// Raised, if the Ok Button was pressed.
		/// </summary>
		public event EventHandler OkPressed;

		/// <summary>
		/// Control which is shown as modal dialog.
		/// </summary>
		public readonly Control Control;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initialize a modal dialog with the control which should be shown.
		/// </summary>
		/// <param name="control">Control which should be shown.</param>
		public ModalDialog(Control control)
		{

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Control = control;
			if (control.Text != "")
				Text = control.Text;
			else
				Text = control.Name;

			// Initialize Size
			int widthOffset = control.Width - panel.ClientSize.Width;
			if (widthOffset < 0)
				widthOffset = 0;
			
			int hightOffset = control.Height - panel.ClientSize.Height;
			
			ClientSize = new Size(ClientSize.Width + widthOffset, ClientSize.Height + hightOffset);

			panel.Controls.Add(control);



		}



		/// <summary>
		/// Called if ok was pressed. Raises the OkPressed event.
		/// </summary>
		/// <param name="args">Event arguments.</param>
		protected virtual void OnOkPressed(EventArgs args)
		{
			if (OkPressed != null)
				OkPressed(this, args);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModalDialog));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.AccessibleDescription = null;
            this.buttonOk.AccessibleName = null;
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.BackgroundImage = null;
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Font = null;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Click += new System.EventHandler(this.OkClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // panel
            // 
            this.panel.AccessibleDescription = null;
            this.panel.AccessibleName = null;
            resources.ApplyResources(this.panel, "panel");
            this.panel.BackgroundImage = null;
            this.panel.Font = null;
            this.panel.Name = "panel";
            this.panel.TabStop = true;
            // 
            // ModalDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.ControlBox = false;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModalDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

		}
		#endregion

		public void OkClick(object sender, System.EventArgs e)
		{
			OnOkPressed(EventArgs.Empty);
		}

	}
}
