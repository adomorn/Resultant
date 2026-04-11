using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Resultant.Generators;

[Generator]
public class ErrorTypeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute source
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("GenerateResultErrorAttribute.g.cs", SourceText.From("""
                namespace Resultant.Generators;

                [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                internal sealed class GenerateResultErrorAttribute : System.Attribute { }
                """, Encoding.UTF8));
        });

        // Find all classes with the attribute
        var classDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Resultant.Generators.GenerateResultErrorAttribute",
                predicate: static (node, _) => node is RecordDeclarationSyntax,
                transform: static (ctx, _) => GetClassInfo(ctx))
            .Where(static m => m is not null);

        // Generate source
        context.RegisterSourceOutput(classDeclarations, static (spc, source) =>
        {
            if (source is null) return;
            var (namespaceName, className, properties) = source.Value;

            var sb = new StringBuilder();
            sb.AppendLine($"namespace {namespaceName};");
            sb.AppendLine();
            sb.AppendLine($"public partial record {className}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static Resultant.Result<T> ToFailure<T>()");
            sb.AppendLine($"        => Resultant.Result.Fail<T>(new {className}({string.Join(", ", properties.Select(p => $"default({p.Type})!"))}));");
            sb.AppendLine("}");

            spc.AddSource($"{className}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        });
    }

    private static (string Namespace, string ClassName, ImmutableArray<(string Type, string Name)> Properties)? GetClassInfo(
        GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
            return null;

        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;

        var properties = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic)
            .Select(p => (p.Type.ToDisplayString(), p.Name))
            .ToImmutableArray();

        return (namespaceName, className, properties);
    }
}
