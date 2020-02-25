using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace nSrcp.Client
{
	/// <summary>
	/// Use this class to throw an localized exception.
	/// </summary>
	[Serializable]
	internal class LocalizedException : Exception
	{
		/// <summary>
		/// Show a error message box.
		/// </summary>
		/// <param name="owner">Parent of the modal window.</param>
		/// <param name="e">Exception which will be shown.</param>
		public static void ShowMessage(IWin32Window owner, Exception e)
		{
			MessageBox.Show(owner, GetFullMessage(e), new LocalizedString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// Format a message with the whole exception list.
		/// </summary>
		/// <param name="e">Exception.</param>
		/// <returns>Exception message.</returns>
		public static string GetFullMessage(Exception e)
		{
			StringBuilder message = new StringBuilder();	
			for (Exception current = e; current != null; current = current.InnerException)
			{
				if (current is TargetInvocationException)
					continue;
				if (message.Length != 0)
					message.Append("\r\n");
				message.Append(current.Message);
                ReflectionTypeLoadException typeLoadExecption = current as ReflectionTypeLoadException;
                if (typeLoadExecption != null)
                {
                    foreach (Exception ex in typeLoadExecption.LoaderExceptions)
                    {
                        message.Append("\r\n");
                        message.Append(ex.Message);
                    }
                }
            }
            return message.ToString();
		}

		/// <summary>
		/// Message in the neural culture. Should be used for tracing or other debugging services.
		/// </summary>
		public readonly string NeutralMessage;

		/// <summary>
		/// Runtime arguments for the message.
		/// </summary>
		public readonly object[] Arguments;

		/// <summary>
		/// Name of the calling assembly.
		/// </summary>
		public readonly string CallingAssemblyName;

		/// <summary>
		/// Name of the exception resource.
		/// </summary>
		public readonly string ExceptionResourceName;
		
		[NonSerialized] ResourceManager resourceManager;

        /// <summary>
        /// This constructor assumes, that the calling assembly have an string resource name "Exceptions.resx"
        /// </summary>
        /// <param name="innerException">Inner exception. Can be null.</param>
        /// <param name="exceptionResourceName">Name of the string in the resource.</param>
        public LocalizedException(Exception innerException, string exceptionResourceName)
            :
            this(innerException, Assembly.GetCallingAssembly(), exceptionResourceName, null)
        {
        }

		/// <summary>
		/// This constructor assumes, that the calling assembly have an string resource name "Exceptions.resx"
		/// </summary>
        /// <param name="exceptionResourceName">Name of the string in the resource.</param>
		public LocalizedException(string exceptionResourceName) :
			this(null, Assembly.GetCallingAssembly(), exceptionResourceName, null)
        {
		}

        /// <summary>
        /// This constructor assumes, that the calling assembly have an string resource name "Exceptions.resx"
        /// </summary>
        /// <param name="innerException">Inner exception. Can be null.</param>
        /// <param name="exceptionResourceName">Name of the string in the resource.</param>
        /// <param name="arguments">Runtime parameters.</param>
        public LocalizedException(Exception innerException, string exceptionResourceName, params object[] arguments)
            :
            this(innerException, Assembly.GetCallingAssembly(), exceptionResourceName, arguments)
        {

        }

        
        /// <summary>
		/// This constructor assumes, that the calling assembly have an string resource name "Exceptions.resx"
		/// </summary>
		/// <param name="exceptionResourceName">Name of the string in the resource.</param>
		/// <param name="arguments">Runtime parameters.</param>
		public LocalizedException(string exceptionResourceName, params object[] arguments) : 
			this(null, Assembly.GetCallingAssembly(), exceptionResourceName, arguments)
		{
			
		}
		
		/// <summary>
		/// Initalize the exception.
		/// </summary>
        /// <param name="innerException">Inner exception. Can be null.</param>
        /// <param name="callingAssembly">Assembly which creates the exception object (Contains the resource with the message).</param>
		/// <param name="exceptionResourceName">Name of the string in the resource.</param>
		/// <param name="arguments">Runtime parameters.</param>
        protected LocalizedException(Exception innerException, Assembly callingAssembly, string exceptionResourceName, params object[] arguments)
            : base(string.Empty, innerException)
		{
			this.Arguments = arguments;
			CallingAssemblyName = callingAssembly.GetName().Name;
			ExceptionResourceName = exceptionResourceName;

			string text = GetResourceManager(callingAssembly).GetString(ExceptionResourceName, CultureInfo.InvariantCulture);
			NeutralMessage = FormatMessage(text);
	
			Debug.Assert(NeutralMessage != null, string.Format("Resource string '{0}' not found.", ExceptionResourceName));
			if (NeutralMessage == null)
				throw new ApplicationException(string.Format("Error in assembly {0}", callingAssembly.GetName().FullName));

		}

		string FormatMessage(string text)
		{
            text = text.Replace(@"\r", "\r\n");
			StringBuilder message = new StringBuilder();
			if (Arguments == null)
				message.Append(text);
			else
				message.AppendFormat(text, Arguments);
			if (message.Length > 0)
			{
				char lastChar = message[message.Length - 1];
				if (lastChar != '.' && lastChar != '?' && lastChar != '!')
					message.Append('.');
			}
			return message.ToString();
			
		}

		// Get the exception resource manager for the specified assembly
		ResourceManager GetResourceManager(Assembly assembly)
		{
			if (resourceManager == null)
			{
				if (assembly == null)
				{
					assembly = Assembly.Load(CallingAssemblyName);
				}
				resourceManager = ResourceHelper.GetResourceManager(assembly, "Strings");
			}
			return resourceManager;

		}

		/// <summary>
		/// Localized error message for the current culture.
		/// </summary>
		public override string Message
		{
			get
			{
				return GetMessage(CultureInfo.CurrentUICulture);
			}
		}


		/// <summary>
		/// Localized error message for the specified culture.
		/// </summary>
		/// <param name="culture">Culture for the resource.</param>
		/// <returns>Localized error message.</returns>
		public string GetMessage(CultureInfo culture)
		{
            string text = GetResourceManager(null).GetString(ExceptionResourceName, culture);
			return FormatMessage(text);
		}

	}
}
