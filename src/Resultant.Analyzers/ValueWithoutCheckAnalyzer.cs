using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Resultant.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValueWithoutCheckAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.ValueAccessedWithoutCheck);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
    }

    private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        var memberAccess = (MemberAccessExpressionSyntax)context.Node;

        if (memberAccess.Name.Identifier.Text != "Value")
            return;

        var typeInfo = context.SemanticModel.GetTypeInfo(memberAccess.Expression);
        if (typeInfo.Type == null)
            return;

        var typeName = typeInfo.Type.ToDisplayString();
        if (!typeName.StartsWith("Resultant.Result<"))
            return;

        // Check if there's an IsSuccess/IsFailure guard in the enclosing scope
        var enclosingBlock = memberAccess.FirstAncestorOrSelf<BlockSyntax>();
        if (enclosingBlock == null)
            return;

        var hasGuard = enclosingBlock.DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .Any(m => m.Name.Identifier.Text is "IsSuccess" or "IsFailure");

        if (!hasGuard)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.ValueAccessedWithoutCheck,
                memberAccess.GetLocation()));
        }
    }
}
