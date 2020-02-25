using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace nSrcp.Client
{
	/// <summary>
	/// Helper class for getting a string resource
	/// </summary>
	public class LocalizedString : IComparable
	{
		private readonly string currentCultureString;

		private ResourceManager resourceManger;
		private Assembly assembly;
		private string resourceName;

		/// <summary>
		/// Empty localized string.
		/// </summary>
		public readonly static LocalizedString Empty = new LocalizedStringNeutral(string.Empty);

        /// <summary>
        /// If you you this constructor, overload the function <see cref="ToString(CultureInfo)"/> and <see cref="ToString()"/>.
        /// </summary>
        internal LocalizedString()
        {
        }

		/// <summary>
		/// Initialize the localized string.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
		/// <param name="callingAssembly">Assembly where the resource can be found.</param>
		public LocalizedString(Assembly callingAssembly, string resourceName)
		{
			this.assembly = callingAssembly;
			this.resourceManger = ResourceHelper.GetResourceManager(callingAssembly, "Strings");
			this.resourceName = resourceName;

			this.currentCultureString = ToString(CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Initialize the localized string.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
        public LocalizedString(string resourceName)
            : this(Assembly.GetCallingAssembly(), resourceName)
		{
		}

		/// <summary>
		/// Returns the string for the current culture.
		/// </summary>
		/// <returns>String for the current culture.</returns>
		public override string ToString()
		{
			return currentCultureString;
		}

		/// <summary>
		/// Implicit cast operator. Makes it possible to use the string everywhere where a string is required.
		/// </summary>
		/// <param name="localizedString">Localized string.</param>
		/// <returns><see cref="currentCultureString"/> of the localized string.</returns>
		public static implicit operator string(LocalizedString localizedString)
		{
			return localizedString.ToString();
		}

		

		/// <summary>
		/// Returns the string in the specified culture.
		/// </summary>
		/// <param name="culture">Culture for which the string is requested.</param>
		/// <returns>String in the specified culture.</returns>
		public virtual string ToString(CultureInfo culture)
		{
			string localizedString = resourceManger.GetString(resourceName);
			
			if (localizedString == null)
			{
				Debug.Assert(localizedString != null, string.Format("String resource '{0}' not defined in Assembly '{1}'.", resourceName, assembly.GetName().Name));
				localizedString = resourceName;
			}
            return localizedString;
		}
		#region IComparable Members

		/// <summary>
		/// Compare the current localized string with an other with the current culture.
		/// </summary>
		/// <param name="obj">Localized string to compare.</param>
		/// <returns>-1 if smaller, 0 if equal, 1 if greater.</returns>
		public int CompareTo(object obj)
		{
			LocalizedString comp = obj as LocalizedString;
			if (comp == null)
				return -1;
			return currentCultureString.CompareTo(comp.currentCultureString);
		}

		#endregion

		/// <summary>
		/// Formats the string with a localized format.
		/// </summary>
		/// <param name="resourceName">Name of the resource which will be used for formatting.</param>
        /// <param name="args">Arguments which will be replaced in the formatting string.</param>
        /// <returns></returns>
        static public string Format(string resourceName, params object[] args)
		{
			string format = new LocalizedString(Assembly.GetCallingAssembly(), resourceName).ToString();
            format = format.Replace(@"\r", "\r\n");
			return string.Format(format, args);
		}
	}

    /// <summary>
    /// Encapsulate a neutral text.
    /// </summary>
    public sealed class LocalizedStringNeutral : LocalizedString
    {
        string neutralText;

        /// <summary>
        /// Initialize the object with a neutral text.
        /// </summary>
        /// <param name="neutralText"></param>
        public LocalizedStringNeutral(string neutralText)
        {
            if (neutralText == null)
                throw new ArgumentNullException("neutralText");
            this.neutralText = neutralText;
        }

        /// <summary>
        /// Returns the a neutral string.
        /// </summary>
        /// <returns>Neutral string.</returns>
        public override string ToString()
        {
            return neutralText;
        }

        /// <summary>
        /// Returns the a neutral string.
        /// </summary>
        /// <param name="culture">Not used.</param>
        /// <returns>Neutral string.</returns>
        public override string ToString(CultureInfo culture)
        {
            return neutralText;
        }

    }

   

}
