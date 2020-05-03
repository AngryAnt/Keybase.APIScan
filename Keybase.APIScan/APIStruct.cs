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
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace Keybase.APIScan
{
	public class APIStruct
	{
		// (?:(?:\/\/\s*(.+)\n)+|\s*)
		// type ([_a-zA-Z][0-9_a-zA-Z]+) struct\s*{
		// (?:\s*([_a-zA-Z][0-9_a-zA-Z]+)\s+([*\[_a-zA-Z][\].0-9_a-zA-Z]+)\s*`codec:"([_a-zA-Z,]+)"\s*json:"([_a-zA-Z,]+)"`\s*)+
		// }
		private const string kStructRegex =
			"(?:(?:\\/\\/\\s*(.+)\n)+|\\s*)" +
			"type ([_a-zA-Z][0-9_a-zA-Z]+) struct\\s*{" +
			"(?:" +
			"\\s*([_a-zA-Z][0-9_a-zA-Z]+)\\s+([*\\[_a-zA-Z][\\].0-9_a-zA-Z]+)\\s*" +
			"`codec:\"([_a-zA-Z,]+)\"\\s*json:\"([_a-zA-Z,]+)\"`\\s*" +
			")+" +
			"}";


		public class Field
		{
			private const string kOmitEmptyTag = "omitempty";


			public string
				Name = default,
				Type = default,
				JSONName = default;
			public bool
				Array = false,
				JSONOmitEmpty = false;


			private string
				TypeString = default,
				CodecString = default,
				JSONString = default;


			public static Field Read ([NotNull] IEnumerable<string> source)
			{
				Field result = new Field ();

				// Read the four source strings
				int index = 0;
				foreach (string value in source)
				{
					switch (index++)
					{
						case 0: result.Name = value; break;
						case 1: result.TypeString = value; break;
						case 2: result.CodecString = value; break;
						case 3: result.JSONString = value; break;
					}
				}

				// Parse name and omit empty flag from json string
				string jsonString = result.JSONString;
				int split = jsonString.IndexOf (',');
				if (split > 0)
				{
					result.JSONName = jsonString.Substring (0, split);
					result.JSONOmitEmpty = jsonString.Substring (split + 1).Trim ().
						Equals (kOmitEmptyTag, StringComparison.InvariantCultureIgnoreCase);
				}
				else
				{
					result.JSONName = jsonString;
				}
				result.JSONName = result.JSONName.Trim ();

				// Parse array and pointer markers from type string
				string typeString = result.TypeString;
				if (typeString.StartsWith ("[]"))
				{
					result.Array = true;
					result.Type = typeString.Substring (2);
				}
				else if (typeString[0] == '*')
				{
					result.Type = typeString.Substring (1);
				}
				else
				{
					result.Type = typeString;
				}

				return result;
			}


			public override string ToString ()
				=> $"{Type}" + (Array ? "[]" : "") + $" {Name} ({JSONName}, omit empty: {JSONOmitEmpty})";
		}


		public string
			Description = default,
			Name = default;
		public readonly List<Field> Fields = new List<Field> ();


		public static IEnumerable<APIStruct> Read ([NotNull] string contents)
			=> new Regex (kStructRegex).Matches (contents).Select (m => m.Groups).Select (Create);


		private static APIStruct Create ([NotNull] GroupCollection collection)
		{
			APIStruct result = new APIStruct()
			{
				Description = JoinOrDefault (collection[1].Captures, default, " "),
				Name = JoinOrDefault (collection[2].Captures)
			};

			result.Fields.AddRange (JoinGroups (collection, 3, 4).Select (Field.Read));

			return result;
		}


		private static IEnumerable<IEnumerable<string>> JoinGroups (
			[NotNull] GroupCollection collection,
			int from,
			int count
		)
			=> Enumerable.Range (0, collection[from].Captures.Count).
				Select (
					captureIndex => collection.Skip<Group> (from).Take (count).
						Select (c => c.Captures[captureIndex].Value)
				);


		private static string JoinOrDefault (
			[NotNull] CaptureCollection captures,
			string defaultReturn = default,
			[NotNull] string glue = default
		)
			=> captures.Count < 1
				? defaultReturn
				: captures.Select (c => c.Value).
					Aggregate ((all, current) => all + glue + current);


		private APIStruct ()
		{}


		public override string ToString ()
			=>
				string.Format (
					"{0}{1}:{2}",
					Name,
					string.IsNullOrEmpty (Description) ? "" : $" ({Description})",
					Fields.Select (f => f.ToString ()).
						Aggregate ("", (all, current) => all + "\n\t" + current)
				);
	}
}
