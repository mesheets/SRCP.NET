using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;

namespace nSrcp.Client
{
	public class FeedbackEventArgs : EventArgs
	{
		public readonly bool State;
		public readonly int Address;
		
		internal FeedbackEventArgs(int address, bool state)
		{
			State = state;
			Address = address;
		}
	}



	public delegate void FeedbackStateChanged(object sender, FeedbackEventArgs args);

	public class WaitForStateChange : IDisposable
	{
		private SrcpFeedback feedback;
		private int address;
		private bool disposed;

		internal WaitForStateChange(SrcpFeedback feedback, int address)
		{
			this.address = address;
			this.feedback = feedback;
			object oldRefCounter = this.feedback.waitObjects[address];
			if (oldRefCounter == null)
				this.feedback.waitObjects.Add(address, 1);
			else
				this.feedback.waitObjects[address] = (int) oldRefCounter + 1;

			this.feedback.StateChanged += new FeedbackStateChanged(OnStateChanged); 
		}

		public event FeedbackStateChanged StateChanged;

		public bool State
		{
			get
			{
				return feedback.ReadState(address);
			}
		}
		
		void OnStateChanged(object sender, FeedbackEventArgs args)
		{
			if (args.Address == address)
			{
				if (StateChanged != null)
				{
					StateChanged(this, args);
				}
			}
		}
		

		public void Dispose()
		{
			if (!disposed)
			{

				disposed = true;
				StateChanged = null;
				int refCounter = (int) this.feedback.waitObjects[address];
				refCounter--;
				if (refCounter == 0)
				{
					this.feedback.waitObjects.Remove(address);
				}
				else
				{
					this.feedback.waitObjects[address] = refCounter;
				}
			}
			GC.SuppressFinalize(this);
		}

		~WaitForStateChange()
		{
			Dispose();
		}


	}

	/// <summary>
	/// Summary description for SrcpFeedbak.
	/// </summary>
    [Serializable]
	public class SrcpFeedback : SrcpDevice, ISerializable    
	{
		string parameters;
		string type;

		Hashtable values = new Hashtable();

		internal Hashtable waitObjects = new Hashtable();

		public FeedbackStateChanged StateChanged;

		protected void OnFeedbackStateChanged(FeedbackEventArgs args)
		{
			if (StateChanged != null)
				StateChanged(this, args);
		}
		public override void Resend()
		{
		}

		public SrcpFeedback(ServerConnection connection) : base(connection)
		{
		}

		protected override void DoInit()
		{
			base.DoInit();
			SendInit(Bus, this.type, this.parameters);
			
		}

		public string Type
		{
			get
			{
				return type;
			}
		}

		public string Parameters
		{
			get
			{
				return parameters;
			}
		}

		void SendInit(int bus, string type, string parameters)
		{
			string command;
			if (Connection.SrcpVersion < ServerConnection.Version8)
			{
				command = string.Format("INIT FB {0} {1}", type, parameters);
			}
			else
			{
				command = string.Format("INIT {0} FB {1} {2}", bus, type, parameters);
			}
			Connection.Info += new InfoHandler(Connection_Info);
            Connection.SendCommand(command);
		}

		public void Init(int bus, string type, string parameters)
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
				    SendInit(bus, type, parameters);
			}
			catch
			{
				if (wasInitialized)
				{
					// Reset old values
					SendInit(Bus, this.type, this.parameters);
				}
				throw;
			}

			this.parameters = parameters;
			this.type = type;

			base.Init(bus);



		}

		const string deviceName = "FB";

		public override string DeviceName
		{
			get
			{
				return deviceName;
			}
		}

		protected override void DoTerm()
		{
			string command;
			if (Connection.SrcpVersion < ServerConnection.Version8)
			{
				command = string.Format("TERM FB {0}", type);
			}
			else
			{
				command = string.Format("TERM {0} FB", Bus);
			}
			Connection.SendCommand(command);

			Connection.Info -= new InfoHandler(Connection_Info);
		}

		public bool ReadState(int address)
		{

			object lastValue = values[address];
			if (lastValue != null)
			{
				return (bool) lastValue;
			}
			return false;
		
		}


		public WaitForStateChange CreateWaitObject(int address)
		{
			return new WaitForStateChange(this, address);
		}


		private void Connection_Info(object sender, InfoEventArgs e)
		{
			string answer = e.Answer;
			CheckAnswer(answer, -1, false);
		}

		bool CheckAnswer(string answer, int forAddress, bool throwError)
		{

			bool newState = false;
			int address = -2;
			try
			{
                while (answer.Contains("  "))
                {
				    answer = answer.Replace("  ", " ");
                }

				string[] parts = answer.Split(" ".ToCharArray());
                if (Connection.SrcpVersion >= ServerConnection.Version8)
                {
                    if (parts.Length == 7)
                    {
                        if (parts[2] == "INFO" && parts[4] == "FB" && parts[3] == Bus.ToString(CultureInfo.InvariantCulture))
                        {
                            address = int.Parse(parts[5]);
                            if (forAddress != -1 && forAddress != address && throwError)
                                throw new ApplicationException("Wrong server address");
                            int newValue = int.Parse(parts[6]);
                            newState = newValue > 0 ? true : false;
                        }
                    }
                }
                else
                {
                    if (parts.Length == 5)
                    {
                        if (parts[0] == "INFO" && parts[1] == "FB" && parts[2] == type)
                        {
                            address = int.Parse(parts[3]);
                            if (forAddress != -1 && forAddress != address && throwError)
                                throw new ApplicationException("Wrong server address");
                            int newValue = int.Parse(parts[4]);
                            newState = newValue > 0 ? true : false;
                        }
                    }
                }


			}
			catch (Exception e)
			{
				if (throwError)
                    throw new LocalizedException("InvalidServerAnswerFormat", LocalizedException.GetFullMessage(e));
				return false;
			}
			if (address == -2)
			{
				if (throwError)
				{
                    throw new LocalizedException("InvalidServerAnswer");
				}
				return false;
			}

			object oldState = values[address];
			if (!newState.Equals(oldState))
			{
				OnFeedbackStateChanged(new FeedbackEventArgs(address, newState));
				values[address] = newState;
			}
			return newState;
		}

		public override string ToString()
		{
			return string.Format("Feedback Type {1} (Bus {0}), Parameters: {2}", Bus, Type, Parameters);
		}


        #region ISerializable Members

        public SrcpFeedback(SerializationInfo info, StreamingContext context) : base(info.GetString("ConnectionName"))
        {
            Init(info.GetInt32("Bus"), info.GetString("Type"), info.GetString("Parameters"));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", (byte) 1);
            info.AddValue("ConnectionName", (Connection != null && Connection.Serialize) ? Connection.Name : string.Empty);
            info.AddValue("Bus", Bus);
            info.AddValue("Type", Type);
            info.AddValue("Parameters", Parameters);
        }

        #endregion
}
}
