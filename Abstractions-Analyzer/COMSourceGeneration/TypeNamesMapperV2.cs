
using System;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MP.AbstractionsLib.Analyzer.COMSourceGeneration
{
    public sealed class TypeNamesMapperV2
    {
        private const string SYSTEM_VOID = "System.Void";

        private readonly Compilation compilation;
        private readonly Stack<DecodedCOMInterface> decoding_stack;
        private readonly Dictionary<String, INamedTypeSymbol> type_cache;
        private readonly ImmutableArray<DecodedCOMInterface> declared_com_interfaces;

        public TypeNamesMapperV2(Compilation compilation, ImmutableArray<DecodedCOMInterface> decoded)
        {
            if (compilation is null) { throw new ArgumentNullException(nameof(compilation)); }
            this.compilation = compilation;
            type_cache = new(10);
            decoding_stack = new(10);
            declared_com_interfaces = decoded;
        }

        public void PushInspectingInterface(DecodedCOMInterface dc)
        {
            if (dc is null) {
                throw new ArgumentNullException(nameof(dc));
            } else {
                decoding_stack.Push(dc);
            }
        }

        public void PopInspectingInterface() => decoding_stack.Pop();

        private bool ExpandNameThroughImports(string name, out string expanded)
        {
            if (decoding_stack.Count == 0) { goto g_fail; }
            DecodedCOMInterface current = decoding_stack.Peek();
            if (current.NamespacedImports is not null) 
            {
                bool unambiguous = false;
                string[] type_mappings = current.NamespacedImports;
                for (int I = 0; I < type_mappings.Length; I++)
                {
                    if (type_mappings[I].EndsWith(name, StringComparison.OrdinalIgnoreCase))
                    {
                        if (unambiguous) { goto g_fail; }
                        unambiguous = true;
                        name = type_mappings[I];
                    }
                }
                if (unambiguous) { 
                    expanded = name; 
                    return true; 
                }
            }
        g_fail:
            expanded = null;
            return false;
        }

        public string FullyQualify(DecodedCOMInterface.ParsedPartialTypeInformation type_info, out bool success)
        {
            if (type_info is null) { success = false; return null; }
            if (!ExpandNameThroughImports(type_info.QualifiedName, out string q)) { q = type_info.QualifiedName; }
            INamedTypeSymbol nts;
            // Try first in the type info cache.
            if (type_cache.TryGetValue(q, out nts)) {
                // If the cache gives result, we can return the type.
                success = true;
                q = nts.GetFullTypeName();
            } else {
                nts = compilation.GetTypeByMetadataName(q);
                if (nts is null) {
                    if (ResolveCommonType(q, out nts)) {
                        type_cache.Add(q, nts);
                        success = true;
                    } else {
                        success = false;
                    }
                } else {
                    type_cache.Add(q, nts);
                    success = true;
                    q = nts.GetFullTypeName();
                }
                if (success) { type_info.QualifiedName = q; }
            }
            return String.Concat(q, new string('*', type_info.PointerIndirections));
        }

        // Tries to resolve the symbol for the given type_string.
        // If lookup fails, null is returned.
        public INamedTypeSymbol GetSymbol(string type_string)
        {
            if (!ExpandNameThroughImports(type_string, out string q)) { q = type_string; }
            INamedTypeSymbol nts;
            // Try first in the type info cache.
            // If the cache gives result, we can return the type.
            if (!type_cache.TryGetValue(q, out nts)) {
                nts = compilation.GetTypeByMetadataName(q);
                if (nts is null) {
                    if (ResolveCommonType(q, out nts)) { type_cache.Add(q, nts); }
                } else {
                    type_cache.Add(q, nts);
                }
            }
            return nts;
        }

        private bool ResolveCommonType(string mapping, out INamedTypeSymbol named_symbol)
        {
            switch (mapping.ToLowerInvariant())
            {
                case "":
                case null:
                    named_symbol = null;
                    return false;
                case "void":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_Void);
                    break;
                case "byte":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_Byte);
                    break;
                case "sbyte":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_SByte);
                    break;
                case "char":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_Char);
                    break;
                case "float":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_Single);
                    break;
                case "double":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_Double);
                    break;
                case "short":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_Int16);
                    break;
                case "ushort":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_UInt16);
                    break;
                case "int":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_Int32);
                    break;
                case "uint":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_UInt32);
                    break;
                case "long":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_Int64);
                    break;
                case "ulong":
                    named_symbol = compilation.GetSpecialType(SpecialType.System_UInt64);
                    break;
                default:
                    named_symbol = null;
                    break;
            }
            return named_symbol is not null;
        }

        // Tries to resolve the DecodedCOMInterface for the given type_info.
        // If lookup fails, null is returned.
        public DecodedCOMInterface FindInterface(DecodedCOMInterface.ParsedPartialTypeInformation type_info)
        {
            if (type_info is null) { return null; }
            if (!ExpandNameThroughImports(type_info.QualifiedName, out string q)) { q = type_info.QualifiedName; }
            return FindInterfaceInternal(q);
        }

        private DecodedCOMInterface FindInterfaceInternal(string q)
        {
            foreach (DecodedCOMInterface dci in declared_com_interfaces)
            {
                if (dci.FullName == q) { return dci; }
            }
            // If the interface still remains unresolvable, try matching by name only.
            // However, this unsafe relatively process should be done by also checking whether a second element with the same name was found as well.
            bool found_before = false;
            DecodedCOMInterface d = null;
            foreach (DecodedCOMInterface dci in declared_com_interfaces)
            {
                if (dci.FriendlyName == q)
                {
                    if (found_before) { break; }
                    found_before = true;
                    d = dci;
                }
            }
            return d;
        }

        public static string TransformVoidNameToCS(string returned) => TransformVoidNameToCS(returned, out _);

        public static string TransformVoidNameToCS(string returned, out bool ispurevoid)
        {
            if (returned is null) {
                ispurevoid = false;
                return null;
            } else if (returned.StartsWith(SYSTEM_VOID)) {
                int svl = SYSTEM_VOID.Length;
                ispurevoid = svl == returned.Length;
                return String.Concat("void", returned.Substring(svl));
            } else {
                ispurevoid = false;
                return returned;
            }
        }
    }
}
