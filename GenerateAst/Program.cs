if (args.Length != 1)
{
	Console.Error.WriteLine("Usage: generate_ast <output directory>");
	Environment.Exit(64);
}

string outputDir = args[0];

DefineAst(outputDir, "Expr", new List<string>
{
	"Assign		: Token name, Expr value", 
	"Ternary	: Expr boolExpr, Token op, Expr trueExpr, Expr falseExpr",
	"Binary		: Expr left, Token op, Expr right",
	"Grouping	: Expr expression",
	"Literal	: object? value",
	"Variable	: Token name",
	"Unary		: Token op, Expr right"
});

DefineAst(outputDir, "Stmt", new List<string>
{
	"Expression : Expr expression",
	"Var		: Token name, Expr? initializer",
	"Print		: Expr expression",
	"Block		: List<Stmt?> statements"
});

static async void DefineAst(string outputDir, string baseName, List<string> types)
{
	string path = $"{outputDir}/{baseName}.cs";

	using StreamWriter writer = File.CreateText(path);

	await writer.WriteLineAsync("namespace LoxNetCore");
	await writer.WriteLineAsync("{");
	await writer.WriteLineAsync($"	public abstract class {baseName}");
	await writer.WriteLineAsync("	{");

	foreach (string type in types)
	{
		string className = type.Split(':')[0].Trim();
		string fields = type.Split(':')[1].Trim();
		DefineType(writer, baseName, className, fields);
	}

	await writer.WriteLineAsync("	}");
	await writer.WriteLineAsync("}");
}

static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
{
	List<string> fields = fieldList.Split(", ").ToList();

	writer.WriteLine($"		public class {className} : {baseName}");
	writer.WriteLine("		{");

	// Properties
	foreach (string field in fields)
	{
		string typeName = TypeName(field);
		string fieldName = Capitalize(FieldName(field));
		if (fieldName == "Expression") fieldName = "Expr";

		writer.WriteLine($"			public {typeName} {fieldName} {{ get; }}");
	}

	writer.WriteLine("");

	// Constructor
	writer.WriteLine($"			public {className}({fieldList})");
	writer.WriteLine("			{");

	foreach (string field in fields)
	{
		string paramName = FieldName(field);
		string fieldName = Capitalize(FieldName(field));
		if (fieldName == "Expression") fieldName = "Expr";

		writer.WriteLine($"				{fieldName} = {paramName};");
	}
	writer.WriteLine("			}");
	writer.WriteLine("		}");
	writer.WriteLine("");

}

static string Capitalize(string word) => word[0].ToString().ToUpper() + word[1..];

static string FieldName(string field) => field.Split(' ')[1];

static string TypeName(string field) => field.Split(' ')[0];
