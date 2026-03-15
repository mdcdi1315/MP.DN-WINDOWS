
using System;
using Microsoft.CodeAnalysis;

namespace MP.AbstractionsLib.Analyzer.COMSourceGeneration
{
    public static class GeneratorCommon
    {
        private static bool GenerateNativeClassV2(System.Text.StringBuilder builder, DecodedCOMInterface[] interfaces, TypeNamesMapperV2 mapper, DiagnosticReporter reporter)
        {
            if (interfaces.Length == 0) { return false; }
            builder.AppendTabs(2);
            string friendly_name = interfaces[interfaces.Length - 1].FriendlyName;
            builder.AppendFormat("private sealed class Native : INativeCOMObject, {0}", friendly_name);
            builder.AppendLine();
            builder.AppendTabs(2);
            builder.AppendLine('{');

            builder.AppendTabs(3);
            builder.AppendLine("[StructLayout(LayoutKind.Sequential)]");
            builder.AppendTabs(3);
            builder.AppendLine("private struct VirtualTable");
            builder.AppendTabs(3);
            builder.AppendLine('{');
            builder.AppendTabs(4);
            builder.AppendLine("public IUnknownVirtualTable IUnknown_Table;");

            string t;
            foreach (DecodedCOMInterface dci in interfaces)
            {
                mapper.PushInspectingInterface(dci);
                try {
                    foreach (DecodedCOMInterface.Method m in dci.Methods)
                    {
                        builder.AppendTabs(4);
                        t = GeneratorUtils.GenerateDelegatePointerMethodFieldDeclaration(m, friendly_name, mapper, reporter);
                        if (t is null) { return false; }
                        builder.Append(t);
                    }
                } finally {
                    mapper.PopInspectingInterface();
                }
            }

            builder.AppendTabs(3);
            builder.AppendLine('}');
            
            builder.AppendLine();

            builder.AppendTabs(3);
            builder.AppendLine("[StructLayout(LayoutKind.Sequential)]");
            builder.AppendTabs(3);
            builder.AppendLine("private struct Native_Data { public VirtualTable* Table; }");
            builder.AppendLine();

            builder.AppendTabs(3);
            builder.AppendLine("private readonly Native_Data* p_object;");
            builder.AppendTabs(3);
            builder.AppendLine("public Native(void* p_object) => this.p_object = (Native_Data*)p_object;");
            builder.AppendTabs(3);
            builder.AppendLine("void* INativeCOMObject.Native => p_object;");
            
            DecodedCOMInterface.Method[] methods;

            foreach (DecodedCOMInterface c in interfaces)
            {
                mapper.PushInspectingInterface(c);
                try {
                    methods = c.Methods;
                    DecodedCOMInterface.MethodArgument[] args;
                    for (int I = 0; I < methods.Length; I++)
                    {
                        builder.AppendTabs(3);
                        t = GenerateMethodDeclaration(methods[I], friendly_name, mapper, reporter);
                        if (t is null) { return false; }
                        builder.Append(t);
                        bool isvoid;
                        t = TypeNamesMapperV2.TransformVoidNameToCS(mapper.FullyQualify(methods[I].ReturnType, out bool s), out isvoid);
                        if (!s) {
                            reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find declared type for COM Interface declaration for method {methods[I].Name} with name {friendly_name}: {methods[I].ReturnType}"));
                            return false;
                        }
                        builder.Append(isvoid ? " { " : " => ");
                        builder.AppendFormat("p_object->Table->GEN_{0}(p_object", methods[I].Name);
                        args = methods[I].Arguments;
                        int lenm1 = args.Length - 1;
                        if (args.Length > 0) { builder.Append(", "); }
                        for (int J = 0; J < args.Length; J++)
                        {
                            builder.Append(args[J].Name);
                            if (J != lenm1) { builder.Append(','); }
                        }
                        builder.Append(')');
                        if (isvoid) {
                            builder.Append("; }");
                        } else {
                            builder.Append(';');
                        }
                        builder.AppendLine();
                    }
                } finally {
                    mapper.PopInspectingInterface();
                }
            }

            builder.AppendTabs(2);
            builder.AppendLine('}');

            return true;
        }

        private static bool GenerateCCWDataV2(System.Text.StringBuilder builder , TypeNamesMapperV2 mapper, DiagnosticReporter reporter, DecodedCOMInterface[] interfaces)
        {
            System.Text.StringBuilder ccw_method_builder = new(), ccw_methods_builder = new();

            ccw_methods_builder.AppendLine();

            ccw_method_builder.AppendTabs(2);
            ccw_method_builder.AppendLine("public static void GenerateCCWVirtualTable([DisallowNull] VirtualTableBuilder builder)");
            ccw_method_builder.AppendTabs(2);
            ccw_method_builder.AppendLine('{');

            foreach (DecodedCOMInterface dc in interfaces)
            {
                mapper.PushInspectingInterface(dc);
                try {
                    foreach (DecodedCOMInterface.Method m in dc.Methods)
                    {
                        ccw_method_builder.AppendTabs(3);
                        ccw_method_builder.AppendFormatLine("builder.AddFunction(({0})&CCW_{1});", GeneratorUtils.GenerateDelegatePointerMethodDeclaration(m, dc.FriendlyName, mapper, reporter), m.Name);
                        ccw_methods_builder.AppendTabs(2);
                        ccw_methods_builder.AppendLine(GeneratorUtils.GenerateCCWWrapperFunction(m, mapper, reporter, dc.FullName));
                    }
                } finally {
                    mapper.PopInspectingInterface();
                }
            }

            ccw_method_builder.AppendTabs(2);
            ccw_method_builder.AppendLine('}');

            builder.Append(ccw_method_builder);
            builder.Append(ccw_methods_builder);

            return true;
        }

        private static string GenerateBaseTypeName(DecodedCOMInterface.ParsedPartialTypeInformation p, TypeNamesMapperV2 mapper, DiagnosticReporter reporter)
        {
            if (p is null) {
                return String.Empty;
            } else {
                DecodedCOMInterface d = mapper.FindInterface(p);
                if (d is null) {
                    reporter(Diagnostic.Create(MPCOMGenerator.DESC_3, null, p));
                    return String.Concat(": ", p.ToString());
                } else {
                    return String.Concat(": ", p.QualifiedName = d.FullName);
                }
            }
        }

        public static string GenerateInterfaceV2(DecodedCOMInterface com_interface, TypeNamesMapperV2 mapper, DiagnosticReporter reporter)
        {
            System.Text.StringBuilder sb = new();

            sb.AppendLine();
            sb.AppendLine("// <auto-generated>");
            sb.AppendFormatLine("// MP Windows COM Interface wrappers implementation for interface {0}.", com_interface.FriendlyName);
            sb.AppendLine("// This file is created by the COM source generator for providing the wrappers implementations for the COM Interop to work.");
            sb.AppendLine("// </auto-generated>");
            sb.AppendLine();
            sb.AppendLine("#pragma warning disable CS0105");
            sb.AppendLine();
            sb.AppendLine("using MP;");
            sb.AppendLine("using System;");
            sb.AppendLine("using MP.Annotations;");
            sb.AppendLine("using MP.NativeInterop;");
            sb.AppendLine("using MP.NativeInterop.Windows;");
            sb.AppendLine("using MP.NativeInterop.Windows.COM;");
            sb.AppendLine("using System.Runtime.InteropServices;");
            sb.AppendLine("using System.Runtime.CompilerServices;");
            sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
            sb.AppendLine();
            sb.AppendLine();

            if (com_interface.NamespacedImports is not null)
            {
                int idx;
                foreach (string ni in com_interface.NamespacedImports)
                {
                    if ((idx = ni.LastIndexOf('.')) > -1)
                    {
                        sb.AppendFormatLine("using {0};", ni.Remove(idx));
                    }
                }
            }

            sb.AppendLine();

            if (com_interface.EnclosedNamespace is not null)
            {
                sb.AppendFormatLine("namespace {0};", com_interface.EnclosedNamespace);
                sb.AppendLine();
            }

            sb.AppendFormatLine("[COMInterface(\"{0}\", typeof(DispatchProvider))]", com_interface.IID);
            sb.AppendFormatLine("{0} interface {1} {2}",
                com_interface.NamespacedImports is null ? "unsafe partial" : "public unsafe",
                com_interface.FriendlyName,
                GenerateBaseTypeName(com_interface.BaseType, mapper, reporter)
            );
            sb.AppendLine('{');
            sb.AppendLine();

            if (com_interface.NamespacedImports is not null)
            {
                String t;
                // We need to append the abstract methods as well.
                foreach (DecodedCOMInterface.Method m in com_interface.Methods)
                {
                    sb.AppendTabs(1);
                    t = GenerateMethodDeclaration(m, com_interface.FriendlyName, mapper, reporter);
                    if (t is null) {
                        return null;
                    }
                    sb.Append(t);
                    sb.AppendLine(';');
                }
                sb.AppendLine();
                sb.AppendLine();
            }

            sb.AppendTabs(1);
            sb.AppendLine("private unsafe sealed class DispatchProvider : ICOMDispatchServicesProvider");
            sb.AppendTabs(1);
            sb.AppendLine('{');

            DecodedCOMInterface[] all_interfaces = GeneratorUtils.GetAllImplementedInterfacesIncludingThis(com_interface, mapper, reporter);

            if (all_interfaces is null) { return null; }

            if (GenerateNativeClassV2(sb, all_interfaces, mapper, reporter)) {
                sb.AppendLine();
                sb.AppendTabs(2);
                sb.AppendLine("public static INativeCOMObject CreateNativeObject([DisallowNull] void* p_native) => new Native(p_native);");
                sb.AppendLine();
                if (!GenerateCCWDataV2(sb, mapper, reporter, all_interfaces)) {
                    return null;
                }
            } else {
                return null;
            }

            sb.AppendTabs(1);
            sb.AppendLine('}');

            sb.AppendLine('}');

            return sb.ToString();
        }

        private static string GenerateMethodDeclaration(DecodedCOMInterface.Method m, string name, TypeNamesMapperV2 mapper, DiagnosticReporter reporter)
        {
            string tg, t;
            System.Text.StringBuilder sb = new();

            tg = mapper.FullyQualify(m.ReturnType, out bool s);

            if (s) {
                sb.AppendFormat("public {0}", TypeNamesMapperV2.TransformVoidNameToCS(tg));
                sb.AppendFormat(" {0}(", m.Name);

                for (int bound = m.Arguments.Length, I = 0; I < bound; I++)
                {
                    t = mapper.FullyQualify(m.Arguments[I].Type, out s);
                    if (s) {
                        sb.Append(TypeNamesMapperV2.TransformVoidNameToCS(t));
                        sb.AppendFormat(" {0}{1}", m.Arguments[I].Name, I == bound - 1 ? String.Empty : ",");
                    } else {
                        reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find declared type for COM Interface declaration for method {m.Name} with name {name}: {m.Arguments[I].Type}"));
                        return null;
                    }
                }
                sb.Append(')');
            } else {
                reporter(Diagnostic.Create(MPCOMGenerator.DESC_2, null, $"Cannot find declared type for COM Interface declaration for method {m.Name} with name {name}: {m.ReturnType}"));
                return null;
            }

            return sb.ToString();
        }
    }
}