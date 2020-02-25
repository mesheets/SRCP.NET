using System;
using System.Runtime.Serialization;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for SrcpPower.
	/// </summary>
    [Serializable]
	public class SrcpPower : SrcpDevice, ISerializable 
	{
		bool on;

		public bool On
		{
			get
			{
				return on;
			}
			set
			{
				on = value;
				Update();
			}
		}

		public override void Resend()
		{
            Update();
		}

		public SrcpPower(ServerConnection connection) : base(connection)
		{
		}

		const string deviceName = "POWER";

		public override string DeviceName
		{
			get
			{
				return deviceName;
			}
		}

		void SendInit(int bus)
		{
			if (Connection.SrcpVersion >= ServerConnection.Version8)
			{
				Connection.SendCommand(string.Format("INIT {0} POWER", bus));
			}
		}

		protected override void DoInit()
		{
			base.DoInit ();
			SendInit(Bus);
			Update();
		}

		public new void Init(int bus)
		{
			bool wasInitialized = false;
			if (Initialized)
			{
				Term();
				wasInitialized = true;
			}
			try
			{
                if (Connection != null && Connection.Started)
                    SendInit(bus);
			}
			catch
			{
				if (wasInitialized)
				{
					// Reset old values
					SendInit(Bus);
				}
				throw;
			}
			base.Init(bus);

			Update();
		}

		void Update()
		{
            if (Initialized && Connection != null && Connection.Started)
			{
				string command;
				string onCommand = on ? "ON" : "OFF";
				if (Connection.SrcpVersion < ServerConnection.Version8)
				{
					command = string.Format("SET POWER {0}", onCommand);
				}
				else
				{
					command = string.Format("SET {0} POWER {1}", Bus, onCommand);
				}

				Connection.SendCommand(command);
			}
		}

		protected override void DoTerm()
		{
			on = false;
			if (Connection.SrcpVersion >= ServerConnection.Version8)
			{
				string command = string.Format("TERM {0} POWER", Bus);
				try
				{
					Connection.SendCommand(command);
				}
				catch (Exception )
				{	
					if (Connection.DDW)
					{
						///WORKAROUND
					
						// Ingnore errors and send power off
						Update();
					}
					else
					{
						throw;
					}
				}
			}
			else
			{
				Update();
			}

		}

		public override string ToString()
		{
			return string.Format("Power (Bus {0})", Bus);
		}

	
        #region ISerializable Members

        public SrcpPower(SerializationInfo info, StreamingContext context)
            : base(info.GetString("ConnectionName"))
    	{
            int bus = info.GetInt16("Bus");
            Init(bus);
	    }

        public void  GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", (byte) 1);
            info.AddValue("Bus", Bus);
            info.AddValue("ConnectionName", (Connection != null && Connection.Serialize) ? Connection.Name : string.Empty);
         	
        }

        #endregion
    }
}
