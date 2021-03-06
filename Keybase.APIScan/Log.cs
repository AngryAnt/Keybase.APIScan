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
