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
			public string
				Name = default,
				TypeString = default,
				CodecString = default,
				JSONString = default;


			public static Field Read ([NotNull] IEnumerable<string> source)
			{
				Field result = new Field ();

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

				return result;
			}


			public override string ToString ()
				=> $"{TypeString} {Name} ({CodecString}, {JSONString})";
		}


		public string
			Comment = default,
			Name = default;
		public readonly List<Field> Fields = new List<Field> ();


		public static IEnumerable<APIStruct> Read ([NotNull] string contents)
			=> new Regex (kStructRegex).Matches (contents).Select (m => m.Groups).Select (Create);


		private static APIStruct Create ([NotNull] GroupCollection collection)
		{
			APIStruct result = new APIStruct()
			{
				Comment = JoinOrDefault (collection[1].Captures, default, " "),
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
					string.IsNullOrEmpty (Comment) ? "" : $" ({Comment})",
					Fields.Select (f => f.ToString ()).
						Aggregate ("", (all, current) => all + "\n\t" + current)
				);
	}
}
