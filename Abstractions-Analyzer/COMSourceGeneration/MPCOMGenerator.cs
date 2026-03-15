
using System;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MP.AbstractionsLib.Analyzer.COMSourceGeneration
{
    [Generator(LanguageNames.CSharp)]
    public sealed class MPCOMGenerator : IIncrementalGenerator
    {
        public static readonly DiagnosticDescriptor DESC_1 = new(
            DiagnosticIds.MisformattedCOMGUIDUsed,
            "GUID specified in the COM interface declaration is misformatted",
            "Make sure that the format of the GUID for interface of type {0} is of type: \"xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx\"",
            DiagnosticIds.COMGenerationCategory,
            DiagnosticSeverity.Error,
            true
        );

        public static readonly DiagnosticDescriptor DESC_2 = new(
            DiagnosticIds.ExceptionGeneratingCOMInterface,
            "There was a run-time exception generating COM interface marshalling information",
            "Exceptional information: {0}",
            DiagnosticIds.COMGenerationCategory,
            DiagnosticSeverity.Error,
            true
        );

        public static readonly DiagnosticDescriptor DESC_3 = new(
            DiagnosticIds.UnresolvableBaseTypeMayCauseIssues,
            "Base interface type for a COM interface could not be resolved",
            "Base interface type {0} could not be resolved, this might cause resolving issues during COM interface generation",
            DiagnosticIds.COMGenerationCategory,
            DiagnosticSeverity.Warning,
            true
        );

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterSourceOutput(
                context.SyntaxProvider.ForAttributeWithMetadataName<DecodedCOMInterface>(
                    "MP.Annotations.COMInterfaceGeneratorAttribute" , 
                    new(COMInformationSyntaxProvider.COMGeneratorInterfaceDeclPred) , 
                    new(COMInformationSyntaxProvider.COMGeneratorGatherInterface)
                ).Collect().Combine(context.AdditionalTextsProvider.SelectMany(
                    new Func<AdditionalText, System.Threading.CancellationToken, ImmutableArray<DecodedCOMInterface>>(SelectFromAdditionalSourceFiles)
                ).Collect()).Combine(context.CompilationProvider),
                new(RunSourceOut)
            );
        }
    
        private static ImmutableArray<DecodedCOMInterface> SelectFromAdditionalSourceFiles(AdditionalText text, System.Threading.CancellationToken ct)
        {
            if (text.Path.EndsWith(".comint")) {
                DecodedCOMInterface dci = DecodedCOMInterface.FromSourceText(text.GetText(ct));
                if (dci is not null) {
                    return ImmutableArray.Create(dci);
                }
            }
            return ImmutableArray<DecodedCOMInterface>.Empty;
        }

        private static void RunSourceOut(SourceProductionContext cxt, ((ImmutableArray<DecodedCOMInterface> dci, ImmutableArray<DecodedCOMInterface> dci_additional) source, Compilation c) d)
        {
            string data = null;
            var i = d.source.dci.AddRange(d.source.dci_additional);
            TypeNamesMapperV2 mapper = new(d.c, i);
            foreach (DecodedCOMInterface c in i)
            {
                try {
                    mapper.PushInspectingInterface(c);
                    data = GeneratorCommon.GenerateInterfaceV2(c, mapper, new(cxt.ReportDiagnostic));
                    mapper.PopInspectingInterface();
                    if (data is null) { continue; }
                    cxt.AddSource($"MP_COM_INT_GEN_{c.IID.Replace('-' , '_')}.cs", data);
                } catch (Exception ex) {
                    cxt.ReportDiagnostic(Diagnostic.Create(DESC_2 , null , ex));
                }
            }
        }
    }
}