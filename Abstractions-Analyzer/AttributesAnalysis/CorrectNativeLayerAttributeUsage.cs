

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MP.AbstractionsLib.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CorrectNativeLayerAttributeUsage : BaseMPDiagnosticAnalyzer
    {
        protected override void InitializeAnalyzer(AnalysisContext cxt)
        {
            cxt.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
            cxt.EnableConcurrentExecution();
            cxt.RegisterSyntaxNodeAction(new(FindDeclaration), SyntaxKind.ClassDeclaration);
        }

        protected override void InitializeDiagnosticDescriptors(ImmutableArray<DiagnosticDescriptor>.Builder builder)
        {
            builder.Add(new DiagnosticDescriptor(
                DiagnosticIds.ClassExtendingAbstractPlatformLayerButDoesNotHaveNativeLayerAttribute,
                "Platform layer class extends AbstractPlatformLayer but does not specify a NativeLayerAtttribute instance",
                "The class '{0}' does not specify an instance of the NativeLayerAtttribute.",
                DiagnosticIds.ApiUsageCategory,
                DiagnosticSeverity.Warning,
                true
            ));
        }

        private void FindDeclaration(SyntaxNodeAnalysisContext cxt)
        {
            switch (cxt.Node)
            {
                case ClassDeclarationSyntax cds:
                    INamespaceSymbol ns;
                    var ctype = cxt.SemanticModel.GetTypeInfo(cds, cxt.CancellationToken).Type;
                    if (ctype is null || ctype is IErrorTypeSymbol) { break; }
                    //System.Console.WriteLine($"CN: {ctype.Name}");
                    if (ctype.Name == "AbstractPlatformLayer" || ctype.Name == "SystemInfo")
                    {
                        ns = ctype.ContainingNamespace;
                        if (ns is null || ns.Name == "MP") { break; }
                    }
                    System.Boolean hasapl = false;
                    foreach (var t in cds.BaseList.Types)
                    {
                        ctype = cxt.SemanticModel.GetTypeInfo(t.Type, cxt.CancellationToken).Type;
                        if (ctype is null || ctype is IErrorTypeSymbol) { continue; }
                        if (ctype.Name == "AbstractPlatformLayer" && (ns = ctype.ContainingNamespace) is not null && ns.Name == "MP")
                        {
                            hasapl = true;
                            break;
                        }
                    }
                    if (hasapl)
                    {
                        System.Boolean hasnativelayer = false;
                        foreach (var o in cds.AttributeLists)
                        {
                            foreach (var attrs in o.Attributes)
                            {
                                var typeinfo = cxt.SemanticModel.GetTypeInfo(attrs, cxt.CancellationToken).Type;
                                if (typeinfo is null || typeinfo is IErrorTypeSymbol) { continue; }
                                ns = typeinfo.ContainingNamespace;
                                if (ns is null) { continue; }

                                if (typeinfo.Name == "NativeLayerAttribute" &&
                                    ns.Name == "Annotations" &&
                                    (ns = ns.ContainingNamespace) is not null &&
                                    ns.Name == "MP")
                                {
                                    hasnativelayer = true;
                                    break;
                                }
                            }
                            if (hasnativelayer) { break; }
                        }
                        if (hasnativelayer == false)
                        {
                            cxt.ReportDiagnostic(Diagnostic.Create(SupportedDiagnostics[0], cxt.Node.GetLocation(), ctype.Name));
                        }
                    }
                    break;
            }
        }
    }
}