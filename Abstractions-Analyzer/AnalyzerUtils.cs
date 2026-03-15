

using System;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MP.AbstractionsLib.Analyzer
{
    public static class AnalyzerUtils
    {
        public static void GetMemberAttributes(ISymbol sym, SyntaxNodeAnalysisContext cxt, Action<SyntaxNodeAnalysisContext, AttributeData> action)
        {
            if (sym is null || sym is IErrorTypeSymbol) { 
                return;
            } else {
                foreach (var o in sym.GetAttributes())
                {
                    if (cxt.CancellationToken.IsCancellationRequested) { return; }
                    if (o.AttributeClass is null) { continue; }
                    action(cxt, o);
                }
            }
        }

        public static System.String GetFullNamespaceName(this INamespaceSymbol containing_namespace)
        {
            List<String> strings = new();
            INamespaceSymbol temp = containing_namespace;
            while (temp is not null)
            {
                strings.Add(temp.Name);
                temp = temp.ContainingNamespace;
            }
            System.Text.StringBuilder sb = new();
            for (int upper = strings.Count - 1, I = upper; I > -1; I--)
            {
                sb.Append(strings[I]);
                if (I != upper && I > 0) { sb.Append('.'); }
            }
            return sb.ToString();
        }

        public static System.String GetFullTypeName(this ISymbol symbol)
        {
            if (symbol is null || symbol is IErrorTypeSymbol) {
                return String.Empty;
            } else {
                System.String s = GetFullNamespaceName(symbol.ContainingNamespace);
                if (s == System.String.Empty) {
                    return symbol.Name;
                } else {
                    return String.Concat(s, ".", symbol.Name);
                }
            }
        }

        public static System.String GetFullTypeNameIgnoreErrors(this ISymbol symbol)
        {
            if (symbol is null) {
                return String.Empty;
            } else {
                System.String s = GetFullNamespaceName(symbol.ContainingNamespace);
                if (s == System.String.Empty) {
                    return symbol.Name;
                } else {
                    return String.Concat(s, ".", symbol.Name);
                }
            }
        }

        public static void GetMemberAttributesAndFilterOnlyByMPAnnotations(ISymbol sym, SyntaxNodeAnalysisContext cxt, Action<SyntaxNodeAnalysisContext, AttributeData> action)
        {
            if (sym is null || sym is IErrorTypeSymbol) { return; }
            foreach (var o in sym.GetAttributes())
            {
                if (cxt.CancellationToken.IsCancellationRequested) { return; }
                if (o.AttributeClass is null) { continue; }
                if (o.AttributeClass.ContainingNamespace.GetFullNamespaceName().StartsWith("MP.Annotations")) { action(cxt, o); }
            }
        }

        public static IEnumerable<TS> FindSyntaxNodeType<TS>(SyntaxNode unknown)
            where TS : SyntaxNode
        {
            // Look everywhere, including the node itself.
            if (unknown is TS syn) {
                yield return syn;
            } else {
                foreach (SyntaxNode sn in unknown.ChildNodes())
                {
                    if (sn is TS s1) {
                        yield return s1;
                    } else {
                        foreach (TS repeated in FindSyntaxNodeType<TS>(sn)) { yield return repeated; }
                    }
                }
            }
        }

        public static IEnumerable<TS> FindSyntaxNodeType<TS>(SyntaxNode unknown, CancellationToken ct)
            where TS : SyntaxNode
        {
            // Look everywhere, including the node itself.
            if (ct.IsCancellationRequested) {
                
            } else if (unknown is TS syn) {
                yield return syn;
            } else {
                foreach (SyntaxNode sn in unknown.ChildNodes())
                {
                    if (ct.IsCancellationRequested) {
                        break;
                    } else if (sn is TS s1) {
                        yield return s1;
                    } else {
                        foreach (TS repeated in FindSyntaxNodeType<TS>(sn, ct)) { yield return repeated; }
                    }
                }
            }
        }

        public static IEnumerable<TS> FindSyntaxNodeType<TS>(Compilation cmp , CancellationToken ct = default)
            where TS : SyntaxNode
        {
            foreach (SyntaxTree tree in cmp.SyntaxTrees)
            {
                if (tree is TS t) {
                    yield return t;
                } else {
                    // Typically, trees contain more nodes to process.
                    SyntaxNode root = tree.GetRoot(ct);
                    if (root is TS t1) {
                        // But if is the root node, return it.
                        yield return t1;
                    } else {
                        foreach (SyntaxNode node in root.ChildNodes())
                        {
                            foreach (TS f in FindSyntaxNodeType<TS>(node)) { yield return f; }
                        }
                    }
                }
            }
        }
    
        public static bool DoesSpecifyEitherPreliminaryOrDeprecatedAttributes(ITypeSymbol ts)
        {
            foreach (AttributeData ad in ts.GetAttributes())
            {
                switch (ad.AttributeClass.GetFullTypeName())
                {
                    case "MP.Annotations.PreliminaryAttribute":
                    case "MP.Annotations.DeprecatedMayBeRemovedAttribute":
                        return true;
                    default: 
                        return false;
                }
            }
            return false;
        }

        public static bool HasAttributeWithMetadataName(ISymbol symbol, string meta_name)
        {
            if (symbol is null || symbol is IErrorTypeSymbol) {
                return false;
            } else {
                foreach (AttributeData ad in symbol.GetAttributes())
                {
                    if (meta_name == ad.AttributeClass.GetFullTypeName())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static bool HasAttributeWithMetadataName(ISymbol symbol, string meta_name, out AttributeData adc)
        {
            if (symbol is null || symbol is IErrorTypeSymbol) {
                adc = null;
                return false;
            } else {
                foreach (AttributeData ad in symbol.GetAttributes())
                {
                    if (meta_name == ad.AttributeClass.GetFullTypeName())
                    {
                        adc = ad;
                        return true;
                    }
                }
                adc = null;
                return false;
            }
        }

        public static bool ImplementsInterface(ITypeSymbol symbol, string full_meta_name)
        {
            if (symbol is null || symbol is IErrorTypeSymbol) {
                return false;
            } else {
                ITypeSymbol c = symbol;
                while (c is not null)
                {
                    foreach (var i in c.AllInterfaces)
                    {
                        if (i.GetFullTypeName() == full_meta_name) { return true; }
                    }
                    c = c.BaseType;
                }
                return false;
            }
        }

        public static bool ExtendsFromAnyAbstractClass(ITypeSymbol ts)
        {
            if (ts is null || ts.IsValueType) {
                return false; 
            } else {
                ITypeSymbol t = ts.BaseType;
                while (t is not null)
                {
                    if (t.IsAbstract) { return true; }
                    t = t.BaseType;
                }
                return false;
            }
        }

        public static bool IsValidNumericType(SpecialType st) => st switch
        {
            SpecialType.System_Byte or 
            SpecialType.System_SByte or 
            SpecialType.System_Char or 
            SpecialType.System_Int16 or 
            SpecialType.System_Int32 or 
            SpecialType.System_Int64 or 
            SpecialType.System_UInt16 or
            SpecialType.System_UInt32 or 
            SpecialType.System_UInt64 or 
            SpecialType.System_Single or 
            SpecialType.System_Double or 
            SpecialType.System_Decimal => true,
            _ => false,
        };

        public static bool IsValidIntegerType(SpecialType st) => st switch
        {
            SpecialType.System_Byte or
            SpecialType.System_SByte or
            SpecialType.System_Char or
            SpecialType.System_Int16 or
            SpecialType.System_Int32 or
            SpecialType.System_Int64 or
            SpecialType.System_UInt16 or
            SpecialType.System_UInt32 or
            SpecialType.System_UInt64 => true,
            _ => false,
        };

        public static bool IsFloatingPointIntegerType(SpecialType st) => st switch 
        {
            SpecialType.System_Single or
            SpecialType.System_Double or
            SpecialType.System_Decimal => true,
            _ => false,
        };

        public static string StripSpacesFromBeginning(string str)
        {
            if (System.String.IsNullOrWhiteSpace(str)) {
                return System.String.Empty;
            } else {
                System.Boolean not_processed = true;
                System.Text.StringBuilder builder = new(str.Length);
                foreach (System.Char c in str)
                {
                    if (not_processed && c == ' ' || c == '\t') {
                        continue; 
                    } else {
                        not_processed = false;
                        builder.Append(c);
                    }
                }
                return builder.ToString();
            }
        }
    }
}