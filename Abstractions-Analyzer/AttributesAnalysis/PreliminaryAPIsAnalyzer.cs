
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable RS1035 // Do not use APIs banned for analyzers

namespace MP.AbstractionsLib.Analyzer.AttributesAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PreliminaryAPIsAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor DESC_1 = new(
            DiagnosticIds.PreliminaryAttributedClassUsed,
            "Preliminary-attributed class was used from stable code",
            "Remove the usage of this class or set the MP_UNSTABLE property to True",
            DiagnosticIds.ApiUsageCategory,
            DiagnosticSeverity.Error,
            true
        );

        private static readonly DiagnosticDescriptor DESC_2 = new(
            DiagnosticIds.DeprecatedMayBeRemovedAttributedClassUsed,
            "This code element may be removed in the future",
            "Remove the deprecated class usage",
            DiagnosticIds.ApiUsageCategory,
            DiagnosticSeverity.Warning,
            true
        );

        private static readonly DiagnosticDescriptor DESC_3 = new(
            DiagnosticIds.DeprecatedMayBeRemovedAttributedClassUsed,
            "This code element will be removed in the future",
            "Remove the deprecated class usage, it will be removed by version {0} of the project {1}",
            DiagnosticIds.ApiUsageCategory,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DESC_1, DESC_2, DESC_3);

        public override void Initialize(AnalysisContext cxt)
        {
            cxt.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            cxt.EnableConcurrentExecution();
            cxt.RegisterSyntaxNodeAction(new(AnalyzeElement), SyntaxKind.LocalDeclarationStatement, SyntaxKind.ObjectCreationExpression, SyntaxKind.AttributeList, SyntaxKind.Parameter, SyntaxKind.SimpleAssignmentExpression, SyntaxKind.VariableDeclaration);
        }

        private static void OnAttribute(SyntaxNodeAnalysisContext cxt, AttributeData data)
        {
            string name = data.AttributeClass.Name;
            // System.Console.WriteLine("Processing class {0}: {1}" , data.AttributeClass.GetFullTypeName() , name);
            switch (name)
            {
                case "PreliminaryAttribute":
                    // System.Console.WriteLine("Reported!");
                    cxt.ReportDiagnostic(Diagnostic.Create(DESC_1, cxt.Node.GetLocation()));
                    break;
                case "DeprecatedMayBeRemovedAttribute":
                    // System.Console.WriteLine("Reported!");
                    var args = data.ConstructorArguments;
                    if (args.IsDefault == false &&
                        args.Length == 1 &&
                        args[0].Value is System.String s)
                    {
                        // The DeprecatedMayBeRemovedAttribute with the 'deprecated-by' version of the code.
                        cxt.ReportDiagnostic(Diagnostic.Create(DESC_3, cxt.Node.GetLocation(), s, cxt.Compilation.AssemblyName ?? System.String.Empty));
                    }
                    else
                    {
                        cxt.ReportDiagnostic(Diagnostic.Create(DESC_2, cxt.Node.GetLocation()));
                    }
                    break;
            }
        }

        private static void AnalyzeElement(SyntaxNodeAnalysisContext cxt)
        {
            // TypeSyntax tsx;
            switch (cxt.Node)
            {
                case BaseParameterSyntax ps:
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(cxt.SemanticModel.GetSymbolInfo(ps, cxt.CancellationToken).Symbol, cxt, new(OnAttribute));
                    break;
                case ObjectCreationExpressionSyntax oce:
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(cxt.SemanticModel.GetSymbolInfo(oce.Type, cxt.CancellationToken).Symbol, cxt, new(OnAttribute));
                    break;
                case AttributeListSyntax als:
                    foreach (var o in als.Attributes)
                    {
                        AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(cxt.SemanticModel.GetTypeInfo(o, cxt.CancellationToken).Type, cxt, new(OnAttribute));
                    }
                    break;
                case LocalDeclarationStatementSyntax lds:
                    if (lds.Declaration is not null)
                    {
                        AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(cxt.SemanticModel.GetTypeInfo(lds.Declaration.Type, cxt.CancellationToken).Type, cxt, new(OnAttribute));
                    }
                    break;
                case VariableDeclarationSyntax vds:
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(cxt.SemanticModel.GetTypeInfo(vds.Type, cxt.CancellationToken).Type, cxt, new(OnAttribute));
                    EqualsValueClauseSyntax s_1;
                    foreach (VariableDeclaratorSyntax dv in vds.Variables)
                    {
                        if ((s_1 = dv.Initializer) is not null)
                        {
                            ProcessExpressionSyntax(cxt, s_1.Value);
                        }
                    }
                    break;
                case AssignmentExpressionSyntax aes:
                    ProcessExpressionSyntax(cxt, aes.Left);
                    ProcessExpressionSyntax(cxt, aes.Right);
                    break;
            }
        }

        private static void ProcessExpressionSyntax(SyntaxNodeAnalysisContext cxt, ExpressionSyntax expr)
        {
            // System.Console.WriteLine("Processing: {0}: {1}" , expr, expr.GetType().Name);
            if (expr is FieldExpressionSyntax fes) {
                if (cxt.SemanticModel.GetSymbolInfo(fes, cxt.CancellationToken).Symbol is IFieldSymbol fs) {
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(fs, cxt, new(OnAttribute));
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(fs.Type, cxt, new(OnAttribute));
                }
            } else if (expr is AssignmentExpressionSyntax aes_recursive) {
                ProcessExpressionSyntax(cxt, aes_recursive.Left);
                ProcessExpressionSyntax(cxt, aes_recursive.Right);
            } else if (expr is InvocationExpressionSyntax ies) {
                if (cxt.SemanticModel.GetSymbolInfo(ies, cxt.CancellationToken).Symbol is IMethodSymbol ms)
                {
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(ms, cxt, new(OnAttribute));
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(ms.ReturnType, cxt, new(OnAttribute));
                    foreach (var p in ms.Parameters)
                    {
                        AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(p, cxt, new(OnAttribute));
                        AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(p.Type, cxt, new(OnAttribute));
                    }
                }
            } else if (expr is MemberAccessExpressionSyntax mes)
            {
                if (cxt.SemanticModel.GetTypeInfo(mes.Expression).Type is ITypeSymbol ts && !AnalyzerUtils.DoesSpecifyEitherPreliminaryOrDeprecatedAttributes(ts))
                {
                    string name = mes.Name.Identifier.ToString();
                    foreach (ISymbol member in ts.GetMembers())
                    {
                        if (member.Name == name)
                        {
                            AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(member, cxt, new(OnAttribute));
                            break;
                        }
                    }
                    AnalyzerUtils.GetMemberAttributesAndFilterOnlyByMPAnnotations(ts, cxt, new(OnAttribute));
                }
            }
        }
    }
}