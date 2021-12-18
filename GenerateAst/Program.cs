if (args.Length != 1)
{
	Console.Error.WriteLine("Usage: generate_ast <output directory>");
	Environment.Exit(64);
}

var outputDir = args[0];

DefineAst(outputDir, "Expr", new List<string>
{
	"Binary		: Expr left, Token op, Expr right",
	"Grouping	: Expr expression",
	"Literal	: object? value",
	"Unary		: Token op, Expr right"
});

static async void DefineAst(string outputDir, string baseName, List<string> types)
{
	var path = $"{outputDir}/{baseName}.cs";

	using StreamWriter writer = File.CreateText(path);

	await writer.WriteLineAsync("namespace LoxNetCore");
	await writer.WriteLineAsync("{");
	await writer.WriteLineAsync($"	public abstract class {baseName}");
	await writer.WriteLineAsync("	{");

	foreach (var type in types)
	{
		var className = type.Split(':')[0].Trim();
		var fields = type.Split(':')[1].Trim();
		DefineType(writer, baseName, className, fields);
	}

	await writer.WriteLineAsync("	}");
	await writer.WriteLineAsync("}");
}

static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
{
	var fields = fieldList.Split(", ").ToList();

	writer.WriteLine($"		public class {className} : {baseName}");
	writer.WriteLine("		{");

	// Properties
	foreach (var field in fields)
	{
		var typeName = TypeName(field);
		var fieldName = Capitalize(FieldName(field));

		writer.WriteLine($"			public {typeName} {fieldName} {{ get; }}");
	}

	writer.WriteLine("");

	// Constructor
	writer.WriteLine($"			public {className}({fieldList})");
	writer.WriteLine("			{");

	foreach (var field in fields)
	{
		var paramName = FieldName(field);
		var fieldName = Capitalize(FieldName(field));

		writer.WriteLine($"				{fieldName} = {paramName};");
	}
	writer.WriteLine("			}");
	writer.WriteLine("		}");
	writer.WriteLine("");

}

static string Capitalize(string word) => word[0].ToString().ToUpper() + word[1..];

static string FieldName(string field) => field.Split(' ')[1];

static string TypeName(string field) => field.Split(' ')[0];
