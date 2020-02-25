using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Text;
using Microsoft.Win32;


namespace nSrcp.Client
{

	public class ServerConnections : IEnumerable
	{

		internal RegistryKey GetConnectionsKey()
		{
			// Check old registry path for downward compatiblity
			RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Michael Geramb\SrcpClient\Connections", true);
			if (key == null)
				key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\nSrcp\Client\Connections");
			return key;
		}
		RegistryKey GetConnectionKey(string connection)
		{
			return GetConnectionsKey().CreateSubKey(connection);
		}
		
		private SortedList array = new SortedList();
		internal ServerConnections()
		{
		}

		internal void Add(ServerConnection connection)
		{
			array.Add(connection.Name, connection);
			using (RegistryKey connectionKey = GetConnectionKey(connection.Name))
			{
				connectionKey.SetValue("Server", connection.Server);
				connectionKey.SetValue("Port", connection.Port);
				connectionKey.SetValue("SrcpServerAutoStart", connection.SrcpServerAutoStart ? 1 : 0);
				connectionKey.SetValue("SrcpServerAutoStop", connection.SrcpServerAutoStop ? 1 : 0);
				connectionKey.SetValue("SrcpServerPath", connection.SrcpServerPath);

			}
		}

		public void Remove(string name)
		{
			ServerConnection connection = (ServerConnection) array[name];
			array.Remove(name);
			if (connection != null)
				connection.Dispose();
			GetConnectionsKey().DeleteSubKeyTree(name);
		}

		public bool Contains(string name)
		{
			return array.Contains(name);
		}

		public IEnumerator GetEnumerator()
		{
			return array.Values.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return array.Count;
			}
		}

		public ServerConnection this[int index]
		{
			get
			{
				return (ServerConnection) array.GetByIndex(index);
			}
		}
		public ServerConnection this[string name]
		{
			get
			{
				return (ServerConnection) array[name];
			}
		}

	}

	
	
	
	public class InfoEventArgs : EventArgs
	{
		public readonly string Answer;

		internal InfoEventArgs(string answer)
		{
			Answer = answer;
		}
	}

	public delegate void InfoHandler(object sender, InfoEventArgs e);

	public class ServerConnection : IDisposable
	{

		public static readonly Version Version8 = new Version(0, 8);
		static readonly Version MaxSupportedVersion = new Version(0, 8, 2, 0);

		static readonly string SrcpServerDefaultPath = @"C:\ddw\ddwserver.exe";
		Process srcpServerProcess;

		public EventHandler BeforeStop;
		public EventHandler AfterStart;
        
        /// <summary>
        /// If true, the server connection will be serialized with the devices which use it.
        /// </summary>
        public bool Serialize = true;


		Thread infoThread;
		TcpClient client;
		TcpClient clientInfo;
		StreamWriter writer;
		StreamReader reader;

		static ServerConnection()
		{
			using (RegistryKey key = ServerConnections.GetConnectionsKey())
			{
				foreach (string connection in key.GetSubKeyNames())
				{
					try
					{
						using (RegistryKey keyConnection = key.CreateSubKey(connection))
						{
							string server = (string) keyConnection.GetValue("Server");
							int port = (int) keyConnection.GetValue("Port");
							bool autostart = (int) keyConnection.GetValue("SrcpServerAutoStart", 0) != 0;
							bool autostop = (int) keyConnection.GetValue("SrcpServerAutoStop", 0) != 0;
							string srcpserverpath = (string) keyConnection.GetValue("SrcpServerPath", SrcpServerDefaultPath);
						
							new ServerConnection(connection, server, port, autostart, autostop, srcpserverpath, false);
						}
					}
					catch
					{
						// Ignore errors in loading - continue with next one
					}
				}
			}

		}

		static RegistryKey GetApplicationDefaultConnectionKey()
		{
			return Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Michael Geramb\SrcpClient\DefaultConnection");
		}

		static string GetApplicationName()
		{
			return Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
		}

		static public string GetDefaultConnectionName()
		{
			string defaultConnectionName = null;
			using (RegistryKey key = GetApplicationDefaultConnectionKey())
			{
				defaultConnectionName = (string) key.GetValue(GetApplicationName());
				if (defaultConnectionName != null && ServerConnections.Contains(defaultConnectionName))
				{
					return defaultConnectionName;
				}
			}
			return null;
		}

		public static ServerConnection GetDefaultConnection(Control parent, bool start)
		{
			string defaultConnectionName = null;
				defaultConnectionName = GetDefaultConnectionName();
            ServerConnection connection = GetConnection(parent, start, defaultConnectionName, false);
			if (connection != null)
				connection.SetDefault();
			return connection;
		}

        public static ServerConnection GetConnection(Control parent, bool start, string connectionName, bool throwExceptionIfCanceled)
        {
            ServerConnection connection = null;
            if (connectionName != null)
            {
                connection = ServerConnections[connectionName];
            }
            while (true)
            {
                if (connection == null)
                {
                    SelectSrcpServer selectServer = new SelectSrcpServer(connectionName, start);
                    selectServer.ShowDialog(parent);
                    connection = selectServer.Connection;
                }
                if (start && connection != null && !connection.Started)
                {
                    try
                    {
                        connection.Start();
                    }
                    catch (Exception ex)
                    {
                        connection = null;
                        MessageBox.Show(parent, LocalizedString.Format("ConnectionToSRCPServerFailed", LocalizedException.GetFullMessage(ex)), null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }
                }
                break;
            }
            if (connection == null && throwExceptionIfCanceled)
                throw new LocalizedException("OperationCanceled");
            return connection;
        }

	

		public readonly string Server;
		public readonly int Port;
		public readonly bool SrcpServerAutoStart;
		public readonly bool SrcpServerAutoStop;
		public readonly string SrcpServerPath;

		private bool defaultConnection;

		private string welcome = string.Empty;
		private Version srcpVersion = new Version(0, 7, 0 , 0);
		private BusInformation[] busInformations = new BusInformation[0];

		internal bool DDW;

		public BusInformation[] BusInformations
		{
			get
			{
				return busInformations;
			}
		}
		public string Welcome
		{
			get
			{
				return welcome;
			}
		}

		public Version SrcpVersion
		{
			get
			{
				return srcpVersion;
			}
		}

		public bool SupportBusses
		{
			get
			{
				return srcpVersion >= Version8;
			}
		}

		private string name;

		public override string ToString()
		{
			return name;
		}
		
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (name != null)
				if (ServerConnections.Contains(name))
					ServerConnections.Remove(name);
				name = value;
				if (name != null)
				{
					if (ServerConnections.Contains(name))
						ServerConnections.Remove(name);
					ServerConnections.Add(this);
					if (defaultConnection)
						SetDefault();
				}

			}
		}

		public void SetDefault()
		{
			defaultConnection = true;
			using (RegistryKey key = GetApplicationDefaultConnectionKey())
			{
				key.SetValue(GetApplicationName(), name);
			}
		}

		public static ServerConnection ChangeDefaultConnection(Control parent)
		{
			SelectSrcpServer selectSrcpServer = new SelectSrcpServer(GetDefaultConnectionName(), true);
			if (DialogResult.Cancel == selectSrcpServer.ShowDialog(parent))
				return null;
			selectSrcpServer.Connection.SetDefault();
			return selectSrcpServer.Connection;
		}

		public static ServerConnections ServerConnections = new ServerConnections();

		public ServerConnection(string name, string server, int port, bool srcpServerAutoStart, bool srcpServerAutoStop, string srcpServerPath, bool start)
		{
			try
			{
				Server = server;
				Port = port;
				SrcpServerAutoStart = srcpServerAutoStart;
				SrcpServerAutoStop = srcpServerAutoStop;
				SrcpServerPath = srcpServerPath;
			
				if (start)
					Start();

				Name = name;			
			}
			catch
			{
				Stop();
				throw;
			}
		}

		public void Start()
		{
			if (Thread.CurrentThread.Name == null)
				Thread.CurrentThread.Name = string.Format("Command Thread {0}:{1}", Server, Port);

	
			EnterMode(out client, out writer, out reader, true, true);
			StartInfoThread();

			StringBuilder error = null;
			if (AfterStart != null)
			{
				foreach (EventHandler handler in AfterStart.GetInvocationList())
				{
					try
					{
						handler(this, EventArgs.Empty);
					}
					catch (Exception e)
					{
						if (error == null)
						{
							error = new StringBuilder();
							error.Append("The following errors occurs:");
							error.Append("\r\n");
							error.Append("\r\n");
						}
						else
						{
							error.Append(e.Message);
							error.Append("\r\n");
						}
						
					}
				}
			}
			if (error != null)
				throw new ApplicationException(error.ToString());

		}

		public bool Started
		{
			get
			{
				return client != null;
			}
		}

		public void Stop()
		{
			if (BeforeStop != null)
				BeforeStop(this, EventArgs.Empty);
			
			if (srcpServerProcess != null && this.SrcpVersion >= Version8)
			{
				if (SrcpServerAutoStop)
				{
					try
					{
						SendCommand("TERM 0 SERVER");
					}
					catch (Exception e)
					{
						Trace.WriteLine(e.Message);
						// Ignore error
					}
				}
			}

			if (writer != null)
			{
				((IDisposable) writer).Dispose();
				writer = null;
			}
			if (reader != null)
			{
				((IDisposable) reader).Dispose();
				reader = null;
			}

			if (readerInfo != null)
			{
				try
				{
					readerInfo.Close();
				}
				catch(Exception )
				{
				}
			}
            if (client != null)
            {
                ((IDisposable)client).Dispose();
                client = null;
            }

			infoHandlers = null;
			if (infoThread != null)
			{
				if (clientInfo != null)
				{
					clientInfo.Close();
					infoThread.Join(1000);
				}
				if (infoThread.IsAlive)
				{
					try
					{
						infoThread.Abort();
					}
					catch
					{
						// Ignore
					}
					infoThread.Join();
				}
				infoThread = null;
			}
            new MethodInvoker(StopSrcpServerInThread).BeginInvoke(null, null);
		}

        void StopSrcpServerInThread()
        {
            Thread.Sleep(1000);
            // check if not started again
            if (client == null)
                StopSrcpServer();
        }

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Stop();			
		}

		~ServerConnection()
		{
            try
            {
                Dispose();
            }
            catch
            {
                // Ignore
            }
		}
		
		private ArrayList infoHandlers;

		private void StartInfoThread()
		{
			if (infoHandlers != null)
			{
				infoThread = new Thread(new ThreadStart(ThreadInfo));
				infoThread.Name = string.Format("Info Thread {0}:{1}", Server, Port);
				infoThread.IsBackground = true;
				infoThread.Start();
			}
		}

		public event InfoHandler Info
		{
			add
			{
				if (infoHandlers == null)
				{
					infoHandlers = new ArrayList();
					StartInfoThread();
				}
				infoHandlers.Add(value);
			}
			remove
			{
				infoHandlers.Remove(value);
			}
		}

		void StopSrcpServer()
		{
			if (srcpServerProcess != null)
			{
				Process temp = srcpServerProcess;
				srcpServerProcess = null;
				if (SrcpServerAutoStop)
				{
					if (!temp.HasExited)
					{
						try
						{
							if (temp.MainWindowHandle != IntPtr.Zero)
							{
								temp.CloseMainWindow();
								temp.WaitForExit(4000);
							}
						}
						catch
						{
							// Ignore errors
						}
					}
					if (!temp.HasExited)
					{
						try
						{
							temp.Kill();
						}
						catch
						{
							// Ignore errors

						}
					}
				}
			}
		}

		void EnterMode(out TcpClient client, out StreamWriter writer, out StreamReader reader, bool command, bool initVersionAndWelcome)
		{
			writer = null;
			reader = null;
			string welcome = "";
			client = null;
			bool infoChannel7 = command == false && srcpVersion < Version8;
			if (infoChannel7)
			{
				client = new TcpClient(Server, Port + 1);
			}
			else
			{
				try
				{
					client = new TcpClient(Server, Port);
				}
				catch (SocketException )
				{
					if (!SrcpServerAutoStart)
					{
						throw;
					}
					// Autostart srcp Server
                    Form activeForm = Form.ActiveForm;
                    try
					{
						srcpServerProcess = Process.Start(SrcpServerPath);
					}
					catch (Exception ex)
					{
						throw new LocalizedException("StartingSRCPServerFailed", SrcpServerPath, ex.Message);
					}
                    if (activeForm != null)
                        activeForm.Activate();
					Exception connectionError = null;
					for (int i = 0; i < 10; i++)
					{
						// Give server time to open socket
						Thread.Sleep(200);
						try
						{
							// try to open
							client = new TcpClient(Server, Port);
							break;
						}
						catch (Exception ex)
						{
							connectionError = ex;
						}
					}
					if (client == null)
					{
						StopSrcpServer();
						throw new LocalizedException("ConnectionToNewlyStartedSRCPServerFailed", LocalizedException.GetFullMessage(connectionError));
					}
				}
			}
			try
			{
				client.SendTimeout = 2000;
				if (command)
					client.ReceiveTimeout = 2000;

				reader = new StreamReader(client.GetStream(), Encoding.ASCII);

				if (!infoChannel7)
				{
					writer = new StreamWriter(client.GetStream(), Encoding.ASCII);
			

					writer.AutoFlush = true;
					try
					{
						while (-1 != reader.Peek())
						{
							string line = string.Empty;
							do
							{ 
								line = reader.ReadLine();
							}
							while (line == "");


							welcome += line;
							
							// Workaround DDW:
							Thread.Sleep(500);
						}
					}
					catch (SystemException )
					{
						// Ignore
					}

					Version versionNumber = new Version(0,7);
					if (initVersionAndWelcome)
					{
						ArrayList bussesInformations = new ArrayList();
						this.welcome = welcome;
						if (welcome.IndexOf(" DDW ") != -1 || welcome.IndexOf(" DDW;") != -1)
							DDW = true;
						else
							DDW = false;
						string[] parts = welcome.Split(";".ToCharArray());
						for (int i = 0 ; i < parts.Length; i++)
						{
							string part = parts[i].Trim();
							string partLowerCase = part.ToLower(CultureInfo.InvariantCulture);
							parts[i] = part;
							if (partLowerCase.StartsWith("server information:"))
							{
								try
								{
									string serverInformation = part.Substring(19).Trim();
									string[] busses = Regex.Split(serverInformation, @"bus\s*(?<Number>\d+):", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
									if (busses.Length >= 2)
									{
										if (busses[0] == "")
										{
											for (int iBus = 1; iBus < busses.Length - 1; iBus++)
											{
												int busNumber = int.Parse(busses[iBus]); 
												iBus++;
												string busDescription = busses[iBus].Trim();
												bussesInformations.Add(new BusInformation(busNumber, busDescription));
											}
										}
									}
								}
								catch
								{
									// Ignore - no bus information	
								}
								
							}
							if (DDW && partLowerCase.StartsWith("server information:"))
							{
								// Workaround DDW 6.7 do not send SRCP version
								if (versionNumber == new Version(0,7))
									versionNumber = new Version(0,8);
							}
							if (partLowerCase.StartsWith("srcp "))
							{
								int iBegin = 5;
								if (iBegin != -1)
								{
									int iEnd = part.IndexOf(';', iBegin);
									if (iEnd == -1)
										iEnd = part.Length;
									string version = part.Substring(iBegin, iEnd - iBegin);
									version.Trim();
									versionNumber = new Version(version);
								}
							}
						}
						this.srcpVersion = versionNumber;
						if (bussesInformations.Count == 0)
						{
							for (int i = 0; i <= 100; i++)
							{
								bussesInformations.Add(new BusInformation(i));
							}
						}
						this.busInformations = (BusInformation[]) bussesInformations.ToArray(typeof(BusInformation));


					}
					if (srcpVersion >= Version8)
					{
						if (srcpVersion >= MaxSupportedVersion)
							srcpVersion = MaxSupportedVersion;

						SendCommand(writer, reader, string.Concat("SET PROTOCOL SRCP ", srcpVersion.ToString(3)));
						SendCommand(writer, reader, "SET CONNECTIONMODE SRCP " + (command ? "COMMAND" : "INFO"));
						SendCommand(writer, reader, "GO");
					}
				}
			}
			catch 
			{
				if (writer != null)
				{
					((IDisposable) writer).Dispose();
					writer = null;
				}
				if (reader != null)
				{
					((IDisposable) reader).Dispose();
					reader = null;
				}
				if (client != null)
				{
					((IDisposable) client).Dispose();
					client = null;
				}
				throw;
			}
		}



		StreamReader readerInfo;
		void ThreadInfo()
		{
			try
			{
				StreamWriter writer;

				EnterMode(out clientInfo, out writer, out readerInfo, false, false);
				using (clientInfo)
				{
					using (writer)
					{
						using (readerInfo)
						{
							while(true)
							{
								string line = readerInfo.ReadLine();
								if (line == null)
									throw new LocalizedException("ConnectionClosed");
								OnInfo(new InfoEventArgs(line));
							}
						}
					}
				}
			}
			catch
			{
				
			}
		}
		private void OnInfo(InfoEventArgs e)
		{
			if (infoHandlers != null)
			{
				foreach (InfoHandler handler in infoHandlers)
				{
					ISynchronizeInvoke syncInvoke = handler.Target as ISynchronizeInvoke;
					if (syncInvoke != null && syncInvoke.InvokeRequired)
					{
						syncInvoke.Invoke(handler, new object[] {this, e});
					}
					else
					{
						handler(this, e);
					}
				}
			}
		}

		public struct Answer
		{
			static readonly Regex regEx = new Regex(@"[\x09\x20]+", RegexOptions.Compiled);
			public DateTime TimeStamp;
			public int Code;
			public string Command;
			public string Description;

			public override string ToString()
			{
				return string.Format("{0}: {1}, {2}", Code, Command, Description);
			}

			internal Answer(Version srcpVersion, string command, string answerString, bool throwException)
			{
				if (srcpVersion < ServerConnection.Version8)
				{
					TimeStamp = DateTime.Now;
					Code = 101;
					Command = command.Split(" ".ToCharArray(), 2)[0];
					Description = answerString;
				}
				else
				{
					string[] parts = regEx.Split(answerString, 4);
					if (parts.Length < 3)
						throw new LocalizedException("WrongServerAnswer", parts.Length);
				
					try
					{
						double seconds = double.Parse(parts[0], CultureInfo.InvariantCulture);
						//		double seconds = 0;
						TimeStamp = new DateTime(1970, 1, 1).AddSeconds(seconds);
					}
					catch (Exception )
					{
						throw new LocalizedException("WrongTimestampFormat");
					}
					if (parts[1] == "INFO")
					{
						Code = 101;
						if (parts.Length >= 4)
							Command = parts[2] + " " + parts[3];
						else
							Command = parts[2];

						Description = "";
					}
					else
					{
						try
						{
							Code = int.Parse(parts[1], CultureInfo.InvariantCulture);
						}
						catch (Exception )
						{
							throw new LocalizedException("WrongCodeFormat");
						}
						Command = parts[2];

						if (parts.Length >= 4)
							Description = parts[3];
						else
							Description = string.Empty;
					}
				
				}
				if (Code > 299)
					throw new LocalizedException("ServerError", Command, Code, Description);
			}
		}

		public Answer SendCommand(string command)
		{
			return SendCommand(writer, reader, command);
		}

		Answer SendCommand(StreamWriter writer, StreamReader reader, string command)
		{
			Debug.WriteLine(command, "SEND");
			writer.WriteLine(command);
			
			string completeLine = string.Empty;
			if (srcpVersion >= Version8 || command.StartsWith("GET"))
			{
				while (true)
				{
					string line = reader.ReadLine();
					Debug.WriteLine(line, "RECEIVE");
					if (line == null)
						throw new Exception("Connection broken");
					if (!line.EndsWith(@"\"))
					{
						completeLine += line;
						break;
					}
					else
					{
						completeLine += line.Substring(0, line.Length - 1);
					}
				}
			}
			else
			{
				if (reader.Peek() != -1)
				{
					completeLine = reader.ReadToEnd();
					Debug.WriteLine(completeLine, "RECEIVE");
				}
			}

			try
			{
				return new Answer(srcpVersion, command, completeLine, true);
			}
			catch (Exception e)
			{
				throw new LocalizedException("CommandFailed", command, LocalizedException.GetFullMessage(e));
			}
		}

	}
	public class BusInformation
	{
		public readonly int Number;	
		public readonly string Description;
		private readonly string toString;

		internal BusInformation(int number, string description)
		{
			Number = number;
			Description = description;
			toString = string.Format("{0}: {1}", Number, Description);
		}

        internal BusInformation(int number)
            : this(number, LocalizedString.Format("BusFormat", number))
		{
		}

		public override string ToString()
		{
			return toString;
		}

	}
}
