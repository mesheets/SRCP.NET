using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace nSrcp.Client
{
	/// <summary>
	/// Helper functions for access the resource.
	/// </summary>
	internal sealed class ResourceHelper
	{
		private static Hashtable resourceManagerCache = new Hashtable();
		
		private ResourceHelper()
		{
		}

		/// <summary>
		/// Search for a resouce with the specified name as postfix and returns the resource manager.
		/// </summary>
		/// <param name="assembly">Assembly, in which the resource should be searched.</param>
		/// <param name="resourceBaseName">Last part of the resource name.</param>
		/// <returns>Resource manager of the specified resource.</returns>
		/// <remarks>If there exist more than one resource with the specified name at the end, an exception will be thrown.</remarks>
		public static ResourceManager GetResourceManager(Assembly assembly, string resourceBaseName)
		{
			string resourceManagerKey = string.Concat(assembly.FullName, "\r", resourceBaseName);
			ResourceManager resourceManager = (ResourceManager) resourceManagerCache[resourceManagerKey];
			if (resourceManager == null)
			{
				string[] resourceNames = assembly.GetManifestResourceNames();
				string foundResource = null;
				string regExPattern = string.Format(@"(?<ResourceName>.*[.]{0})\.resources$", resourceBaseName);
				foreach (string resourceName in resourceNames)
				{
					Match match = Regex.Match(resourceName, 
						regExPattern, 
						RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.ExplicitCapture);
					if (match.Success)
					{
						Debug.Assert(foundResource == null, string.Format("Only one resource can end with the name '{0}.resources'", resourceBaseName));
						if (foundResource != null)
							throw new ApplicationException(string.Format("Error in assembly {0}", assembly.GetName().FullName));
						foundResource = match.Groups["ResourceName"].Value;
					}
				}
				Debug.Assert(foundResource != null, string.Format("Resource named '{0}.resx' not found.", resourceBaseName));
				if (foundResource == null)
					throw new ApplicationException(string.Format("Error in assembly {0}", assembly.GetName().FullName));
				resourceManager = new ResourceManager(foundResource, assembly);
				resourceManagerCache[resourceManagerKey] = resourceManager;
			}
			return resourceManager;
		}

	}
}
