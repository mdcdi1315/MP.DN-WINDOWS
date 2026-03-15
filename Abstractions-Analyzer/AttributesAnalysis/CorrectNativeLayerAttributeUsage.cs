
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MP.AbstractionsLib.Analyzer.AttributesAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CorrectNativeLayerAttributeUsage : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor RULE = new(
            DiagnosticIds.ClassExtendingAbstractPlatformLayerButDoesNotHaveNativeLayerAttribute,
            "Platform layer class extends AbstractPlatformLayer but does not specify a NativeLayerAtttribute instance",
            "The class '{0}' does not specify an instance of the NativeLayerAtttribute",
            DiagnosticIds.ApiUsageCategory,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RULE);

        public override void Initialize(AnalysisContext cxt)
        {
            cxt.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
            cxt.EnableConcurrentExecution();
            cxt.RegisterSyntaxNodeAction(new(DetectMistakenUsage), SyntaxKind.ClassDeclaration);
        }

        private static bool IsNativePlatformLayer(ITypeSymbol ts) => ts.GetFullTypeName() switch
        {
            "MP.AbstractPlatformLayer" or "MP.SystemInfo" => true,
            _ => false
        };

        private static void DetectMistakenUsage(SyntaxNodeAnalysisContext cxt)
        {
            if (cxt.Node is ClassDeclarationSyntax cds && cxt.SemanticModel.GetDeclaredSymbol(cds, cxt.CancellationToken) is ITypeSymbol ts)
            {
                if (!IsNativePlatformLayer(ts))
                {
                    bool has_apl = false;
                    ITypeSymbol ts_temp = ts.BaseType;
                    while (has_apl == false && ts_temp is not null)
                    {
                        has_apl = IsNativePlatformLayer(ts_temp);
                        ts_temp = ts_temp.BaseType;
                    }
                    if (has_apl)
                    {
                        foreach (AttributeData attr in ts.GetAttributes())
                        {
                            if (attr.AttributeClass.GetFullTypeName() == "MP.Annotations.NativeLayerAttribute")
                            {
                                return;
                            }
                        }
                        // Otherwise, report
                        cxt.ReportDiagnostic(Diagnostic.Create(RULE, cds.GetLocation(), ts.Name));
                    }
                }
            }
        }
    }
}