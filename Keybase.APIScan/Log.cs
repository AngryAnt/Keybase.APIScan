using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;


namespace Keybase.APIScan
{
	public static class Log
	{
		public static void Message ([NotNull] string contents) => Console.WriteLine (contents);


		// TODO: Add markup to present format parameters
		public static void Message ([NotNull] string format, params object[] parameters) =>
			Message (string.Format (format, parameters));


		public static void All ([NotNull] GroupCollection groupCollection)
		{
			foreach (Group group in groupCollection)
			{
				Message ("Group");
				All (group);
			}
		}


		public static void All ([NotNull] Group group, string prefix = "")
		{
			foreach (Capture capture in group.Captures)
			{
				Message ("{0}Capture: {1}", prefix, capture.Value);
			}
		}
	}
}
