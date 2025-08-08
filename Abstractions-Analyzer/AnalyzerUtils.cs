

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MP.AbstractionsLib.Analyzer
{
    public static class AnalyzerUtils
    {
        public static void GetMemberAttributes(ISymbol sym, SyntaxNodeAnalysisContext cxt, Action<SyntaxNodeAnalysisContext, AttributeData> action)
        {
            if (sym is null || sym is IErrorTypeSymbol)
            {
                return;
            }
            foreach (var o in sym.GetAttributes())
            {
                if (cxt.CancellationToken.IsCancellationRequested) { return; }
                if (o.AttributeClass is null) { continue; }
                action(cxt, o);
            }
        }

        public static void GetMemberAttributesAndFilterOnlyByMPAnnotations(ISymbol sym, SyntaxNodeAnalysisContext cxt, Action<SyntaxNodeAnalysisContext, AttributeData> action)
        {
            if (sym is null || sym is IErrorTypeSymbol)
            {
                return;
            }
            foreach (var o in sym.GetAttributes())
            {
                if (cxt.CancellationToken.IsCancellationRequested) { return; }
                if (o.AttributeClass is null) { continue; }
                var ns = o.AttributeClass.ContainingNamespace;
                if (ns is null) { continue; }
                if (ns.Name == "Annotations" && (ns = ns.ContainingNamespace) is not null && ns.Name == "MP")
                {
                    action(cxt, o);
                }
            }
        }
    }
}