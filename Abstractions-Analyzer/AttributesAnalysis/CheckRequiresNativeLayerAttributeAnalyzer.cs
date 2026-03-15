
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MP.AbstractionsLib.Analyzer.AttributesAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CheckRequiresNativeLayerAttributeAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor RULE = new(
            DiagnosticIds.DoesNotSpecifyRequiresNativeLayer,
            "The method calls a platform layer call without specifying that it does require the native layer in order to work",
            "Specify the RequiresNativeLayer attribute on the {0} {1} of class {2}",
            DiagnosticIds.ApiUsageCategory,
            DiagnosticSeverity.Error,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RULE);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(new(AnalyzeSyntaxNode), SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            IMethodSymbol ms;
            if (context.Node is BaseMethodDeclarationSyntax mds && (ms = context.SemanticModel.GetDeclaredSymbol(mds, context.CancellationToken)) is not null)
            {
                if (AnalyzerUtils.HasAttributeWithMetadataName(ms, "MP.Annotations.RequiresNativeLayerAttribute")) { return; }
                if (AnalyzerUtils.HasAttributeWithMetadataName(ms.ContainingType, "MP.Annotations.RequiresNativeLayerAttribute")) { return; }
                if (mds.Body is null) { return; }
                foreach (StatementSyntax ssx in mds.Body.Statements)
                {
                    if (context.CancellationToken.IsCancellationRequested) {
                        break;
                    } else {
                        foreach (ExpressionStatementSyntax esx in AnalyzerUtils.FindSyntaxNodeType<ExpressionStatementSyntax>(ssx, context.CancellationToken))
                        {
                            AnalyzeExpression(esx.Expression, ms, context);
                        }
                    }
                }
            }
        }

        private static void AnalyzeExpression(ExpressionSyntax expr, IMethodSymbol ms, SyntaxNodeAnalysisContext context)
        {
            if (expr is MemberAccessExpressionSyntax access_syntax) {
                AnalyzeMethodCall(access_syntax, ms, context);
                foreach (SyntaxNode n in access_syntax.ChildNodes())
                {
                    if (n is ExpressionSyntax e) { AnalyzeExpression(e, ms, context); }
                }
            } else if (expr is InvocationExpressionSyntax ies) {
                AnalyzeMethodCall(ies, ms, context);
                foreach (SyntaxNode n in ies.ChildNodes())
                {
                    if (n is ExpressionSyntax e) { AnalyzeExpression(e, ms, context); }
                }
            } else if (expr is AssignmentExpressionSyntax aes) {
                AnalyzeExpression(aes.Right, ms, context);
            }
        }

        private static void AnalyzeMethodCall(ExpressionSyntax expr, IMethodSymbol ms, SyntaxNodeAnalysisContext context)
        {
            switch (context.SemanticModel.GetSymbolInfo(expr, context.CancellationToken).Symbol)
            {
                case null:
                case IErrorTypeSymbol:
                    return;
                case IMethodSymbol ies_method:
                    if (ies_method.ContainingType.GetFullTypeName() == "MP.SystemInfo") 
                    {
                        switch (ies_method.Name)
                        {
                            case "UnloadPlatformLayer":
                            case "RegisterPlatformLayer":
                            default:
                                context.ReportDiagnostic(Diagnostic.Create(RULE, expr.GetLocation(), ms.MethodKind == MethodKind.Constructor ? "constructor" : "method", ms.Name, ms.ContainingType.GetFullTypeName()));
                                return;
                        }
                    } else if (
                        AnalyzerUtils.HasAttributeWithMetadataName(ies_method, "MP.Annotations.RequiresNativeLayerAttribute") ||
                        AnalyzerUtils.HasAttributeWithMetadataName(ies_method.ContainingType, "MP.Annotations.RequiresNativeLayerAttribute")
                    ) {
                        context.ReportDiagnostic(Diagnostic.Create(RULE, expr.GetLocation(), ms.MethodKind == MethodKind.Constructor ? "constructor" : "method", ms.Name, ms.ContainingType.GetFullTypeName()));
                    }
                    break;
                case IPropertySymbol property:
                    if (property.ContainingType.GetFullTypeName() == "MP.SystemInfo") {
                        context.ReportDiagnostic(Diagnostic.Create(RULE, expr.GetLocation(), ms.MethodKind == MethodKind.Constructor ? "constructor" : "method", ms.Name, ms.ContainingType.GetFullTypeName()));
                    } else if (
                        AnalyzerUtils.HasAttributeWithMetadataName(property, "MP.Annotations.RequiresNativeLayerAttribute") ||
                        AnalyzerUtils.HasAttributeWithMetadataName(property.ContainingType, "MP.Annotations.RequiresNativeLayerAttribute")
                    ) {
                        context.ReportDiagnostic(Diagnostic.Create(RULE, expr.GetLocation(), ms.MethodKind == MethodKind.Constructor ? "constructor" : "method", ms.Name, ms.ContainingType.GetFullTypeName()));
                    }
                    break;
            }
            
        }

    }
}