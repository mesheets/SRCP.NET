using System;
using System.Text;
using System.Runtime.Serialization;

namespace nSrcp.Client
{
	public enum Direction
	{
		Back = 0,
		Forward = 1
	}

	/// <summary>
	/// Summary description for SrcpGenericLoc.
	/// </summary>
    [Serializable]
	public class SrcpGenericLoc : SrcpDevice, ISerializable
	{
		private int address;
		private int steps;
		private string protocol;
		private string protocolIntern;
		private int step;
		private int direction = (int) Direction.Forward;
		private bool[] functions = new bool[0];
		
		const string deviceName = "GL";

		public override string DeviceName
		{
			get
			{
				return deviceName;
			}
		}


		public SrcpGenericLoc(ServerConnection connection) : base(connection)
		{
		}


		void SendInit(int bus, int address, string protocol, int steps, int numberOfFunctions)
		{

            protocol = GetProtocolIntern(Connection, protocol, address, steps);
			if (Connection.SrcpVersion >= ServerConnection.Version8)
			{
				string command = string.Format("INIT {0} GL {1} {2} {3} {4}", bus, address, protocol, steps, numberOfFunctions);
				Connection.SendCommand(command);
			}
            this.protocolIntern = protocol;
		}

		protected override void DoInit()
		{
			base.DoInit();
			SendInit(Bus, address, protocol, steps, functions.Length);
			Update();
		}

		public void Init(int bus, int address, string protocol, int steps, int numberOfFunctions) 
		{
            Init(bus, address, protocol, steps, numberOfFunctions, true);
		}

        static string GetProtocolIntern(ServerConnection connection, string protocol, int address, int steps)
        {
            string protocolIntern = protocol;
            if (connection.SrcpVersion >= ServerConnection.Version8)
            {
                if (protocol == "M 4")
                    protocolIntern = "M 2";
            }
            else
            {
                if (protocol == "P")
                    protocolIntern = "PS";
                else
                {
                    protocolIntern = protocol.Replace(" ", "");
                    if (protocolIntern.StartsWith("M"))
                    {
                        if (protocolIntern.EndsWith("2") || protocolIntern.EndsWith("4"))
                        {
                            if (steps == 27)
                            {
                                protocolIntern = "M5";
                            }
                            else if (steps == 28)
                            {
                                protocolIntern = "M3";
                            }
                            else if (steps == 14 && address > 80)
                            {
                                protocolIntern = "M4";
                            }
                        }
                    }
                    if (protocolIntern.StartsWith("N"))
                    {
                        if (protocolIntern.EndsWith("1"))
                        {
                            if (steps > 28)
                            {
                                protocolIntern = "N2";
                            }
                        }
                        if (protocolIntern.EndsWith("2"))
                        {
                            if (steps > 28)
                            {
                                protocolIntern = "N4";
                            }
                            else
                            {
                                protocolIntern = "N3";
                            }
                        }
                    }
                }
            }
            return protocolIntern;
        }
		public void Init(int bus, int address, string protocol, int steps, int numberOfFunctions, bool sendUpdate)
		{
			
			bool wasInitialized = false;
			if (Initialized)
			{
				wasInitialized = true;
				Term();
			}
			try
			{
                if (Connection != null && Connection.Started)
                    SendInit(bus, address, protocol, steps, numberOfFunctions);
			}
			catch
			{
				if (wasInitialized)
				{
					// Reset old values
					SendInit(Bus, this.address, this.protocol, this.steps, this.functions.Length);
				}
				throw;
			}

			this.address = address;
			this.steps = steps;
			this.protocol = protocol;

			bool[] newFunctions  = new bool[numberOfFunctions];
			if (this.functions != null)
			{
				Array.Copy(this.functions, newFunctions, System.Math.Min(this.functions.Length, numberOfFunctions));
			}
			this.functions = newFunctions;

			base.Init(bus);

			if (sendUpdate)
				Update();


		}

		public void EmergencyStop()
		{
			int oldDirection = direction;
			int oldStep = step;
			bool[] oldFunctions = functions;
			// set direction to emergency
			direction = 2;
			// set speed to zero and turn off all functions
			step = 0;
			// Send information 2 times for more security
			Update();
			Update();
			Update();

			// Some decoder need changing direction for stop
			if (oldDirection == 1)
				direction = 0;
			else
				direction = 1;
			Update();
			functions = new bool[functions.Length];
			Update();

			// set back old values - except the step
			direction = oldDirection;
			functions = oldFunctions;
			step = oldStep;
		}

		public override void Resend()
		{
			Update();
		}


		void Update()
		{
			if (Initialized && Connection != null && Connection.Started)
			{
				StringBuilder commandBuilder = new StringBuilder();
				commandBuilder.Append("SET ");
				if (Connection.SrcpVersion < ServerConnection.Version8)
				{
					commandBuilder.Append("GL ");
					commandBuilder.Append(protocolIntern);
					commandBuilder.Append(" ");

				}
				else
				{
					commandBuilder.Append(Bus);
					commandBuilder.Append(" GL ");

				}
				commandBuilder.Append(address);
				commandBuilder.Append(" ");
				commandBuilder.Append(direction);
				commandBuilder.Append(" ");
				commandBuilder.Append(step);
				commandBuilder.Append(" ");
				commandBuilder.Append(steps);



				for (int i = 0; i < functions.Length; i++)
				{
					bool function = functions[i];
					commandBuilder.Append(" ");
					commandBuilder.Append(function ? "1" : "0");

					if (i == 0 && Connection.SrcpVersion < ServerConnection.Version8)
					{
						commandBuilder.Append(" ");
						commandBuilder.Append(functions.Length - 1);
					}
				}
				// WORKAROUND:
				if (Connection.DDW && functions.Length < 5 && Connection.SrcpVersion >= ServerConnection.Version8)
				{
					for (int i = functions.Length; i < 5; i++)
					{
						commandBuilder.Append(" 0");
					}
				}
				string command = commandBuilder.ToString();
				Connection.SendCommand(command);
			}
		}

		protected override void DoTerm()
		{
			step = 0;
			Array.Clear(functions, 0, functions.Length);
			if (Connection.SrcpVersion >= ServerConnection.Version8)
			{
				string command = string.Format("TERM {0} GL {1}", Bus, address);
				Connection.SendCommand(command);
			}
			else
			{
				Update();
			}
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

		public int Steps
		{
			get
			{
				return steps;
			}
		}


		public int NumberOfFunctions
		{
			get
			{
				return functions.Length;
			}
		}

		public bool GetFunction(int index)
		{
			return functions[index];
		}

		public void SetFunction(int index, bool value)
		{
			functions[index] = value;
			Update();
		}

		public bool[] GetFunctions()
		{
			return (bool[]) functions.Clone();
		}


		public void Set(int step, Direction direction, bool[] functions)
		{
			if (functions.Length != this.functions.Length)
				throw new ArgumentException("Wrong number of function values", "functions");
			if (step > Steps)
				throw new ArgumentOutOfRangeException("step", step, "Value must not greater than Steps.");
			if (step < 0)
				throw new ArgumentOutOfRangeException("step", step, "Value must not smaller than 0.");
			if (!Enum.IsDefined(typeof(Direction), direction))
				throw new ArgumentException("direction");

			Array.Copy(functions, this.functions, this.functions.Length);

			if (!Enum.IsDefined(typeof(Direction), direction))
				throw new ArgumentException("direction");
			this.direction = (int) direction;

			if (step > Steps)
				throw new ArgumentOutOfRangeException("step", step, "Value must not greater than Steps.");
			if (step < 0)
				throw new ArgumentOutOfRangeException("step", step, "Value must not smaller than 0.");
			this.step = step;
			Update();


		}

		public int Step
		{
			get
			{
				return step;
			}
			set
			{
				if (value > Steps)
					throw new ArgumentOutOfRangeException("value", value, "Value must not greater than Steps.");
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", value, "Value must not smaller than 0.");
				step = value;
				Update();
			}
		}

		public Direction Direction
		{
			get
			{
				return (Direction) direction;
			}
			set
			{
				if (!Enum.IsDefined(typeof(Direction), value))
					throw new ArgumentException("value");
				direction = (int) value;
				Update();
			}
		}

		public override string ToString()
		{
			return string.Format("Generic Loc (Bus {0}, Address {1})", Bus, Address);
		}


        #region ISerializable Members

        public SrcpGenericLoc(SerializationInfo info, StreamingContext context) : base(info.GetString("ConnectionName"))
        {
		    Init(info.GetInt32("Bus"), info.GetInt32("Address"), info.GetString("Protocol"), info.GetInt32("Steps"), info.GetInt32("NumberOfFunctions"));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", (byte)1);
            info.AddValue("ConnectionName", (Connection != null && Connection.Serialize) ? Connection.Name : string.Empty);
            info.AddValue("Bus", Bus);
            info.AddValue("Address", Address);
            info.AddValue("Protocol", Protocol);
            info.AddValue("Steps", Steps);
            info.AddValue("NumberOfFunctions", NumberOfFunctions);
        }

        #endregion
}
}
