using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for SrcpPower.
	/// </summary>
    [Serializable]
	public class SrcpGenericAccessory : SrcpDevice, ISerializable 
	{
        List<bool> ports = new List<bool>();
		private int address;
		private string protocol;
        private string additionalParameters;

		public void SetPort(int port, bool value)
		{
            ports[port] = value;
            Update(port);
		}

        public bool GetPort(int port)
        {
            return ports[port];
        }
        public int NumberOfPorts
        {
            get
            {
                return ports.Count;
            }
        }
        public override void Resend()
        {
            Update();
        }


		public SrcpGenericAccessory(ServerConnection connection) : base(connection)
		{
		}

		const string deviceName = "GA";

		public override string DeviceName
		{
			get
			{
				return deviceName;
			}
		}

		void SendInit(int bus, int address, string protocol, string additionalParameters)
		{
			if (Connection.SrcpVersion >= ServerConnection.Version8)
			{
				if (additionalParameters == null || additionalParameters.Length == 0)
                    Connection.SendCommand(string.Format("INIT {0} GA {1} {2}", bus, address, protocol));
                else
				    Connection.SendCommand(string.Format("INIT {0} GA {1} {2} {3}", bus, address, protocol, additionalParameters));
			}
		}

		protected override void DoInit()
		{
			base.DoInit ();
			SendInit(Bus, Address, Protocol, AdditionalParameters);
			Update();
		}

		public void Init(int bus, int address, string protocol, string additionalParameters, int numberOfPorts)
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
                    SendInit(bus, address, protocol, additionalParameters);
			}
			catch
			{
				if (wasInitialized)
				{
					// Reset old values
					SendInit(Bus, this.address, this.protocol, this.additionalParameters);
				}
				throw;
			}
			base.Init(bus);

            this.address = address;
			this.protocol = protocol;
			this.additionalParameters = additionalParameters;
            for (int i = ports.Count - 1; i >= numberOfPorts; i--)
            {
                ports.RemoveAt(i);   
            }
            for (int i = ports.Count; i < numberOfPorts; i++)
            {
                ports.Add(false);
            }

			Update();
		}
        void Update()
        {
            for (int port = 0; port < ports.Count; port++)
            {
                Update(port);
            }
        }
        void Update(int port)
		{
            if (Initialized && Connection != null && Connection.Started)
            {

                int value = ports[port] ? 1 : 0;

                string command;
                if (Connection.SrcpVersion < ServerConnection.Version8)
                {
                    command = string.Format("SET GA {0} {1} {2} {3} -1", protocol, Address, port, value);
                }
                else
                {
                    command = string.Format("SET {0} GA {1} {2} {3} -1", Bus, Address, port, value);
                }

                Connection.SendCommand(command);

            }
		}

		protected override void DoTerm()
		{
            for (int i = 0; i < ports.Count; i++)
            {
                ports[i] = false;
            }
			if (Connection.SrcpVersion >= ServerConnection.Version8)
			{
				string command = string.Format("TERM {0} GA {1}", Bus, Address);
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
			return string.Format("Generic Accessory (Bus {0})", Bus);
		}

        		
        public int Address
		{
			get
			{
				return address;
			}
		}


		public string Protocol
		{
			get
			{
				return protocol;
			}
		}

        public string AdditionalParameters
        {
            get
            {
                return additionalParameters;
            }
        }
	
        #region ISerializable Members

        public SrcpGenericAccessory(SerializationInfo info, StreamingContext context)
            : base(info.GetString("ConnectionName"))
    	{
            int bus = info.GetInt16("Bus");
            int address = info.GetInt16("Address");
            string protocol = info.GetString("Protocol");
            string additionalParameters = info.GetString("AdditionalParameters");
            int numberOfPorts = info.GetInt32("NumberOfPorts");
            Init(bus, address, protocol, additionalParameters, numberOfPorts);
	    }

        public void  GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", (byte) 1);
            info.AddValue("Bus", Bus);
            info.AddValue("Address", Address);
            info.AddValue("Protocol", Protocol);
            info.AddValue("AdditionalParameters", AdditionalParameters);
            info.AddValue("NumberOfPorts", ports.Count);
            info.AddValue("ConnectionName", (Connection != null && Connection.Serialize) ? Connection.Name : string.Empty);
         	
        }

        #endregion
    }
}
