using System.IO;
using System.Reflection;
using Keybase.APIScan.Jekyll;


namespace Keybase.APIScan
{
	public static class Program
	{
		private static void Main (string[] args)
		{
			if (args.Length < 1)
			{
				Log.Message (
					"Usage: dotnet {0} [[go source path] [go source path]]\nExample: dotnet {0} {1}",
					Path.GetFileName (Assembly.GetExecutingAssembly ().Location),
					"../../../../../keybase/client/go/protocol/chat1/api.go"
				);

				return;
			}

			foreach (string fileName in args)
			{
				Log.Message ("Working directory: {0}", Directory.GetCurrentDirectory ());
				Log.Message ("Processing {0}:", fileName);
				foreach (APIStruct result in APIStruct.Read (File.ReadAllText (fileName)))
				{
					Log.Message (result.ToJekyll ());
				}
			}
		}
	}
}
