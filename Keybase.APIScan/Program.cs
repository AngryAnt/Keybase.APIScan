/*
 *
Copyright 2020 Emil "AngryAnt" Johansen

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 */


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
