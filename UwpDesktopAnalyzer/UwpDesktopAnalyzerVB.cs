using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Collections.Generic;


[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
public class UwpDesktopAnalyzerVB : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(UwpDesktopAnalyzer.Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterCodeBlockStartAction<SyntaxKind>(AnalyzeCodeBlockStart);
    }

    public void AnalyzeCodeBlockStart(CodeBlockStartAnalysisContext<SyntaxKind> context)
    {
        var reports = new Dictionary<int, Diagnostic>();
        context.RegisterSyntaxNodeAction(c => AnalyzeExpression(c, reports), SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(c => AnalyzeExpression(c, reports), SyntaxKind.VariableDeclarator);
        context.RegisterSyntaxNodeAction(c => AnalyzeExpression(c, reports), SyntaxKind.DirectCastExpression);
        context.RegisterSyntaxNodeAction(c => AnalyzeExpression(c, reports), SyntaxKind.TryCastExpression);
        context.RegisterCodeBlockEndAction(c =>
        {
            foreach (var d in reports.Values) c.ReportDiagnostic(d);
        });
    }

    void AnalyzeExpression(SyntaxNodeAnalysisContext context, Dictionary<int, Diagnostic> reports)
    {
        TypeSyntax t = null;
        if (context.Node.Kind() == SyntaxKind.ObjectCreationExpression) t = (context.Node as ObjectCreationExpressionSyntax).Type;
        else if (context.Node.Kind() == SyntaxKind.VariableDeclarator) t = (context.Node as VariableDeclaratorSyntax).AsClause.Type();
        // TODO: the above line doesn't work. I need to get the type of the variable even if its As clause is omitted
        else if (context.Node.Kind() == SyntaxKind.DirectCastExpression) t = (context.Node as DirectCastExpressionSyntax).Type;
        else if (context.Node.Kind() == SyntaxKind.TryCastExpression) t = (context.Node as TryCastExpressionSyntax).Type;
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


