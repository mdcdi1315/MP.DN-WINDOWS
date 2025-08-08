
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MP.AbstractionsLib.Analyzer
{
    public abstract class BaseMPDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private ImmutableArray<DiagnosticDescriptor> descs;

        protected abstract void InitializeAnalyzer(AnalysisContext cxt);

        protected abstract void InitializeDiagnosticDescriptors(ImmutableArray<DiagnosticDescriptor>.Builder builder);

        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1025:Configure generated code analysis", Justification = "Must be defined by inherting classes")]
        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1026:Enable concurrent execution", Justification = "Must be defined by inherting classes")]
        public sealed override void Initialize(AnalysisContext context)
        {
            InitializeAnalyzer(context);
            ImmutableArray<DiagnosticDescriptor>.Builder build = ImmutableArray.CreateBuilder<DiagnosticDescriptor>(10);
            InitializeDiagnosticDescriptors(build);
            descs = build.ToImmutableArray();
        }

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => descs;
    }
}