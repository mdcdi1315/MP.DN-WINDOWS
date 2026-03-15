
using System;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MP.AbstractionsLib.Analyzer.AttributesAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class VerifyCOMGUIDValuesAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor RULE = new(
            DiagnosticIds.MisformattedCOMGUIDUsed,
            "Detected a misformatted Windows COM GUID",
            "The GUID \"{0}\" is reported by .NET as misformatted because an exception of type {1} has been reported: {2}",
            DiagnosticIds.COMGenerationCategory,
            DiagnosticSeverity.Error,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RULE);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSymbolAction(new(ProcessInterface), SymbolKind.NamedType);
        }

        private static void VerifyGuid(TypedConstant constant, DiagnosticReporter reporter)
        {
            if (constant.Value is not System.String str) { return; }
            try {
                Guid.Parse(str);
            } catch (Exception ex) {
                reporter(Diagnostic.Create(RULE, null, str, ex.GetType().Name, ex.Message));
            }
        }

        private static void ProcessInterface(SymbolAnalysisContext context)
        {
            if (context.Symbol is INamedTypeSymbol p && p.TypeKind == TypeKind.Interface)
            {
                foreach (AttributeData d in p.GetAttributes())
                {
                    if (d.ConstructorArguments.Length == 0) { continue; }
                    switch (d.AttributeClass.GetFullTypeName())
                    {
                        case "MP.Annotations.COMInterfaceGeneratorAttribute":
                            VerifyGuid(d.ConstructorArguments[0], new(context.ReportDiagnostic));
                            break;
                        case "MP.Annotations.DefaultCOMInterfaceObjectGuidAttribute":
                            VerifyGuid(d.ConstructorArguments[0], new(context.ReportDiagnostic));
                            break;
                    }
                }
            }
        }
    }
}