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
