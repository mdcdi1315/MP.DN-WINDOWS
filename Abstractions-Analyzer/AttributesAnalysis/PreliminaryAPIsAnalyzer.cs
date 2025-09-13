
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MP.AbstractionsLib.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PreliminaryAPIsAnalyzer : BaseMPDiagnosticAnalyzer
    {
        protected override void InitializeAnalyzer(AnalysisContext cxt)
        {
            cxt.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            cxt.EnableConcurrentExecution();
            cxt.RegisterSyntaxNodeAction(new(AnalyzeElement) , SyntaxKind.LocalDeclarationStatement, SyntaxKind.ObjectCreationExpression, SyntaxKind.AttributeList , SyntaxKind.Parameter);
        }

        protected override void InitializeDiagnosticDescriptors(ImmutableArray<DiagnosticDescriptor>.Builder builder)
        {
            builder.Add(new DiagnosticDescriptor(
                DiagnosticIds.PreliminaryAttributedClassUsed,
                "Preliminary-attributed class was used from stable code",
                "Remove the usage of this class or set the MP_UNSTABLE property to True.",
                DiagnosticIds.ApiUsageCategory,
                DiagnosticSeverity.Error,
                true
            ));
            builder.Add(new DiagnosticDescriptor(
                DiagnosticIds.DeprecatedMayBeRemovedAttributedClassUsed,
                "This code element may be removed in the future.",
                "Remove the deprecated class usage.",
                DiagnosticIds.ApiUsageCategory,
                DiagnosticSeverity.Warning,
                true
            ));
            builder.Add(new DiagnosticDescriptor(
                DiagnosticIds.DeprecatedMayBeRemovedAttributedClassUsed,
                "This code element will be removed by version {0} of the project {1}.",
                "Remove the deprecated class usage.",
                DiagnosticIds.ApiUsageCategory,
                DiagnosticSeverity.Warning,
                true
            ));
        }

        private void OnAttribute(SyntaxNodeAnalysisContext cxt, AttributeData data)
        {
            switch (data.AttributeClass.Name)
            {
                case "PreliminaryAttribute":
                    cxt.ReportDiagnostic(Diagnostic.Create(SupportedDiagnostics[0], cxt.Node.GetLocation()));
                    break;
                case "DeprecatedMayBeRemovedAttribute":
                    var args = data.ConstructorArguments;
                    if (args.IsDefault == false &&
                        args.Length == 1 &&
                        args[0].Value is System.String s) {
                        // The DeprecatedMayBeRemovedAttribute with the 'deprecated-by' version of the code.
                        cxt.ReportDiagnostic(Diagnostic.Create(SupportedDiagnostics[2], cxt.Node.GetLocation(), s, cxt.Compilation.AssemblyName ?? System.String.Empty));
                    } else {
                        cxt.ReportDiagnostic(Diagnostic.Create(SupportedDiagnostics[1], cxt.Node.GetLocation()));
                    }
                    break;
            }
        }

        private void AnalyzeElement(SyntaxNodeAnalysisContext cxt)
        {
            switch (cxt.Node)
            {
                case ParameterSyntax ps:
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(cxt.SemanticModel.GetSymbolInfo(ps.Type, cxt.CancellationToken).Symbol, cxt , new(OnAttribute));
                    break;
                case ObjectCreationExpressionSyntax oce:
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(cxt.SemanticModel.GetSymbolInfo(oce.Type, cxt.CancellationToken).Symbol, cxt , new(OnAttribute));
                    break;
                case AttributeListSyntax als:
                    foreach (var o in als.Attributes)
                    {
                        AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(cxt.SemanticModel.GetTypeInfo(o , cxt.CancellationToken).Type, cxt , new(OnAttribute));
                    }
                    break;
                case LocalDeclarationStatementSyntax lds:
                    if (lds.Declaration is not null)
                    {
                        AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(cxt.SemanticModel.GetTypeInfo(lds.Declaration.Type, cxt.CancellationToken).Type, cxt , new(OnAttribute));
                    }
                    break;
            }
        }
    }
}