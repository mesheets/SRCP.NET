using System;

namespace nSrcp.Client
{
	/// <summary>
	/// Summary description for Device.
	/// </summary>
	public abstract class SrcpDevice : IDisposable
	{
		private ServerConnection connection;
		private int bus;

		bool initialized;
		bool connected;

		protected bool Connected
		{
			get
			{
				return connected;
			}
		}


		public abstract string DeviceName
		{
			get;
		}

		public bool Initialized
		{
			get
			{
				return initialized;
			}
		}

		public int Bus
		{
			get
			{
				return bus;
			}
		}

        protected SrcpDevice(string connectionName)
        {
            if (connectionName != null && connectionName != string.Empty)
            {
                Connection = ServerConnection.GetConnection(null, false, connectionName, true);
            }
        }

		protected SrcpDevice(ServerConnection connection)
		{
            Connection = connection;
		}

		private void ConnectionAfterStart(object sender, EventArgs args)
		{
			try
			{
				connected = true;
				if (Initialized)
					DoInit();
			}
			catch (Exception e)
			{
				throw new LocalizedException(e, "SRCPDecviceCouldNotStart", this, e.Message);	
			}
		}

		private void ConnectionBeforeStop(object sender, EventArgs args)
		{
			try
			{
				Term();
			}
			catch
			{
				// Ignore
			}
		}

		public ServerConnection Connection
		{
			get
			{
				return connection;
			}
            set
            {
                if (connection != value)
                {
                    if (connection != null)
                    {
                        if (connection.Started)
                            ConnectionBeforeStop(connection, EventArgs.Empty);
                        this.connection.AfterStart -= new EventHandler(ConnectionAfterStart);
                        this.connection.BeforeStop -= new EventHandler(ConnectionBeforeStop);
                    }
                    connection = value;
                    if (connection != null)
                    {
                        connection.AfterStart += new EventHandler(ConnectionAfterStart);
                        connection.BeforeStop += new EventHandler(ConnectionBeforeStop);
                        if (connection.Started)
                            ConnectionAfterStart(connection, EventArgs.Empty);
                    }
                    else
                    {
                        connected = false;
                    }
                }

            }
		}

		protected void Init(int bus)
		{
			initialized = true;
			this.bus = bus;
		}

		protected virtual void DoInit()
		{
		}


		public void Term()
		{
			if (Connected && Initialized)
			{
                try
                {
                    DoTerm();
                }
                finally
                {
                    connected = false;
                }
			}
		}

		protected abstract void DoTerm();

		public abstract void Resend();

		public override abstract string ToString();

		public void Dispose()
		{
            try
            {
                Term();
            }
            catch
            {
                // Ignore
            }
            if (connection != null)
            {
                this.connection.AfterStart -= new EventHandler(ConnectionAfterStart);
                this.connection.BeforeStop -= new EventHandler(ConnectionBeforeStop);
                connection = null;
                connected = false;
            }            

		}

		~SrcpDevice()
		{
            Dispose();
		}

	}
}
