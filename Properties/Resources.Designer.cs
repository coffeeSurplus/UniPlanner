using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.IO;

namespace UniPlanner.Properties; 

[GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCodeAttribute()]
[CompilerGeneratedAttribute()]

internal class Resources {

	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;
		
	[SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]

	[EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (object.ReferenceEquals(resourceMan, null))
			{
				ResourceManager temp = new ResourceManager("UniPlanner.Properties.Resources", typeof(Resources).Assembly);
				resourceMan = temp;
			}
			return resourceMan;
		}
	}

	[EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture { get => resourceCulture; set => resourceCulture = value; }

	internal static UnmanagedMemoryStream Ascending => GetStream("Ascending");
	internal static UnmanagedMemoryStream Bounce => GetStream("Bounce");
	internal static UnmanagedMemoryStream Chimes => GetStream("Chimes");
	internal static UnmanagedMemoryStream Chords => GetStream("Chords");
	internal static UnmanagedMemoryStream Descending => GetStream("Descending");
	internal static UnmanagedMemoryStream Echo => GetStream("Echo");
	internal static UnmanagedMemoryStream Jingle => GetStream("Jingle");
	internal static UnmanagedMemoryStream Tap => GetStream("Tap");
	internal static UnmanagedMemoryStream Transition => GetStream("Transition");
	internal static UnmanagedMemoryStream Xylophone => GetStream("Xylophone");

	private static UnmanagedMemoryStream GetStream(string name) => ResourceManager.GetStream(name, resourceCulture); 
}