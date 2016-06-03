using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;


[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UwpDesktopAnalyzerCS : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(UwpDesktopAnalyzer.Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterCodeBlockStartAction<SyntaxKind>(AnalyzeCodeBlockStart);
    }

    public void AnalyzeCodeBlockStart(CodeBlockStartAnalysisContext<SyntaxKind> context)
    { 
        var reports = new Dictionary<int, Diagnostic>();
        context.RegisterSyntaxNodeAction(c => AnalyzeExpression(c,reports), SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(c => AnalyzeExpression(c,reports), SyntaxKind.VariableDeclaration);
        context.RegisterSyntaxNodeAction(c => AnalyzeExpression(c,reports), SyntaxKind.CastExpression);
        context.RegisterCodeBlockEndAction(c => 
        {
            foreach (var d in reports.Values) c.ReportDiagnostic(d);
        });
    }

    void AnalyzeExpression(SyntaxNodeAnalysisContext context, Dictionary<int,Diagnostic> reports)
    {
        TypeSyntax t = null;
        if (context.Node.Kind() == SyntaxKind.ObjectCreationExpression) t = (context.Node as ObjectCreationExpressionSyntax).Type;
        else if (context.Node.Kind() == SyntaxKind.VariableDeclaration) t = (context.Node as VariableDeclarationSyntax).Type;
        else if (context.Node.Kind() == SyntaxKind.CastExpression) t = (context.Node as CastExpressionSyntax).Type;
        if (t == null) return;

        var loc = context.Node.GetLocation();
        if (!loc.IsInSource) return;
        var line = loc.GetLineSpan().StartLinePosition.Line;
        if (reports.ContainsKey(line)) return;

        var symbol = context.SemanticModel.GetSymbolInfo(t);
        if (symbol.Symbol == null || symbol.Symbol.Kind != SymbolKind.NamedType) return;
        var type = symbol.Symbol as INamedTypeSymbol;
        if (!type.IsType || type.IsValueType || !type.IsSealed) return;
        var name = type.ToDisplayString();
        if (!UwpDesktopAnalyzer.ForbiddenTypes.Contains(name)) return;

        reports[line] = Diagnostic.Create(UwpDesktopAnalyzer.Rule, loc, name);
    }
}


