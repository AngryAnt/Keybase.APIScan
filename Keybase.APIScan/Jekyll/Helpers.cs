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


using System.Diagnostics.CodeAnalysis;
using System.IO;


namespace Keybase.APIScan.Jekyll
{
	public static class Helpers
	{
		public static string ToJekyll ([NotNull] this APIStruct source)
		{
			StringWriter writer = new StringWriter ();
			source.ToJekyll (writer);
			return writer.ToString ();
		}


		public static void ToJekyll ([NotNull] this APIStruct source, [NotNull] TextWriter destination)
		{
			destination.Write ($"---\nname: {source.Name}\ndescription: {source.Description}\nfields:");

			foreach (APIStruct.Field field in source.Fields)
			{
				destination.Write ('\n');
				field.ToJekyll (destination);
			}

			destination.Write ("\n---");
		}


		public static void ToJekyll ([NotNull] this APIStruct.Field source, [NotNull] TextWriter destination)
		{
			destination.Write (
				$"\t - {{" +
					$"\"Name\"=>\"{source.Name}\", " +
					$"\"Type\"=>\"{source.Type}\", " +
					$"\"Array\"=>{source.Array}, " +
					$"\"JSONName\"=>\"{source.JSONName}\", " +
					$"\"JSONOmitEmpty\"=>{source.JSONOmitEmpty}" +
				$"}}"
			);
		}
	}
}
