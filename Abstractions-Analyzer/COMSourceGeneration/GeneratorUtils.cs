
using System;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MP.AbstractionsLib.Analyzer.COMSourceGeneration
{
    public static class GeneratorUtils
    {
        public static string GenerateCCWWrapperFunction(DecodedCOMInterface.Method method , TypeNamesMapperV2 mapper, DiagnosticReporter reporter, string full_interface_name)
        {
            string t;
            System.Text.StringBuilder builder = new();

            builder.AppendLine("[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]");

            t = mapper.FullyQualify(method.ReturnType, out bool s);

            if (s) {
                builder.AppendTabs(2);
                builder.AppendFormat("private static {0}", TypeNamesMapperV2.TransformVoidNameToCS(t));
                builder.AppendFormat(" CCW_{0}(CCWComObjectDetails* p_self", method.Name);
            } else {
                reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find declared type for COM Interface declaration for method {method.Name} with name {full_interface_name}: {method.ReturnType}"));
                return null;
            }

            System.Text.StringBuilder call_builder = new();

            foreach (var pm in method.Arguments)
            {
                builder.Append(',');
                t = mapper.FullyQualify(pm.Type, out s);
                if (s) {
                    builder.Append(TypeNamesMapperV2.TransformVoidNameToCS(t));
                } else {
                    reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find declared type for COM Interface declaration for method {method.Name} with name {full_interface_name}: {pm.Type}"));
                    return null;
                }
                builder.Append(' ');
                builder.Append(pm.Name);
                if (call_builder.Length != 0) {
                    call_builder.Append(',');
                }
                call_builder.Append(pm.Name);
            }

            builder.AppendFormat(") => (({0})(p_self->GetAttachedObject())).{1}({2});", full_interface_name, method.Name, call_builder);

            return builder.ToString();
        }

        public static bool HasInvalidIID(string iid_read) => !Guid.TryParse(iid_read, out _);

        public static string GenerateDelegatePointerMethodFieldDeclaration(DecodedCOMInterface.Method m, string name, TypeNamesMapperV2 mapper, DiagnosticReporter reporter)
        {
            string tg;
            System.Text.StringBuilder sb = new();

            tg = mapper.FullyQualify(m.ReturnType, out bool s);

            if (s) {
                sb.Append("public delegate* unmanaged[Stdcall]<void*,");

                string t;
                for (int bound = m.Arguments.Length, I = 0; I < bound; I++)
                {
                    t = mapper.FullyQualify(m.Arguments[I].Type, out s);
                    if (s) {
                        sb.Append(TypeNamesMapperV2.TransformVoidNameToCS(t));
                        sb.Append(',');
                    } else {
                        reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find declared type for COM Interface declaration for method {m.Name} with name {name}: \"{m.Arguments[I].Type}\""));
                        return null;
                    }
                }
                sb.Append(TypeNamesMapperV2.TransformVoidNameToCS(tg));
                sb.AppendFormatLine("> GEN_{0};", m.Name);
            } else {
                reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find declared type for COM Interface declaration for method {m.Name} with name {name}: \"{m.ReturnType}\""));
                return null;
            }

            return sb.ToString();
        }

        public static string GenerateDelegatePointerMethodDeclaration(DecodedCOMInterface.Method m, string name, TypeNamesMapperV2 mapper, DiagnosticReporter reporter)
        {
            string tg, t;
            System.Text.StringBuilder sb = new();

            tg = mapper.FullyQualify(m.ReturnType, out bool s);

            if (s) {
                sb.Append("delegate* unmanaged[Stdcall]<CCWComObjectDetails*,");

                for (int bound = m.Arguments.Length, I = 0; I < bound; I++)
                {
                    t = mapper.FullyQualify(m.Arguments[I].Type, out s);
                    if (s) {
                        sb.Append(TypeNamesMapperV2.TransformVoidNameToCS(t));
                        sb.Append(',');
                    } else {
                        reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find declared type for COM Interface declaration with name {name}: {m.Arguments[I].Type}"));
                        return null;
                    }
                }
                sb.Append(TypeNamesMapperV2.TransformVoidNameToCS(tg));
                sb.Append('>');
            } else {
                reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find declared type for COM Interface declaration with name {name}: {m.ReturnType}"));
                return null;
            }

            return sb.ToString();
        }

        public static DecodedCOMInterface[] GetAllImplementedInterfacesIncludingThis(DecodedCOMInterface current, TypeNamesMapperV2 mapper, DiagnosticReporter reporter)
        {
            Stack<DecodedCOMInterface> base_interfaces = new(2);

            DecodedCOMInterface ddc = current, t;

            while (ddc is not null)
            {
                if (ddc.BaseType is null) {
                    base_interfaces.Push(ddc);
                    break;
                } else if ((t = mapper.FindInterface(ddc.BaseType)) is not null) {
                    base_interfaces.Push(ddc);
                    ddc = t;
                } else {
                    reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find type named as {ddc.BaseType}."));
                    return null;
                }
            }

            return base_interfaces.ToArray();
        }
    }
}