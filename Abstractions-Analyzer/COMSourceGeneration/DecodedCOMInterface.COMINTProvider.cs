
using System;
using Microsoft.CodeAnalysis.Text;

namespace MP.AbstractionsLib.Analyzer.COMSourceGeneration
{
    partial class DecodedCOMInterface
    {
        private static void VerifyTypeName(string type_name)
        {
            System.Char c;
            int len = type_name.Length, lenm1 = len - 1;
            for (int I = 0; I < len; I++)
            {
                c = type_name[I];
                if (
                    ((c < 'a' && c > 'z') &&
                    (c < 'A' && c > 'Z') &&
                    (c != '_') &&
                    (c != '.')) &&
                    (I == lenm1 && c != '*')
                ) {
                    throw new InvalidOperationException($"String {type_name} is invalid because a banned character was found in it: {c}");
                }
            }
            if (type_name[type_name.Length-1] == '.') {
                throw new InvalidOperationException($"String {type_name} is not allowed to end with a trailing dot character.");
            }
        }

        private static void VerifyName(string name)
        {
            foreach (System.Char c in name)
            {
                if (
                    (c < 'a' && c > 'z') &&
                    (c < 'A' && c > 'Z') && 
                    (c != '_')
                ) {
                    throw new InvalidOperationException($"String {name} is invalid because a banned character was found in it: {c}");
                }
            }
        }

        // Decodes a stored COM Interface declaration from COM Interface Declaration JSON files.
        // These files are passed as additional files to the compiler with the .comint file extension.
        public static DecodedCOMInterface FromSourceText(SourceText text)
        {
            DecodedCOMInterface dci = new();
            if (text is null) {
                return null;
            } else {
                System.Text.Json.JsonElement elm;
                using System.Text.Json.JsonDocument jdt = System.Text.Json.JsonDocument.Parse(text.ToString(), new System.Text.Json.JsonDocumentOptions() { CommentHandling = System.Text.Json.JsonCommentHandling.Skip, MaxDepth = 5, AllowTrailingCommas = false });
                if (jdt.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object) {
                    throw new InvalidOperationException("COM INT JSON root element must be a JSON object.");
                } else {
                    string temp;
                    if (jdt.RootElement.TryGetProperty("imports", out elm))
                    {
                        switch (elm.ValueKind)
                        {
                            case System.Text.Json.JsonValueKind.Array:
                                System.Text.Json.JsonElement e_entry;
                                dci.NamespacedImports = new string[elm.GetArrayLength()];
                                for (int I = 0; I < dci.NamespacedImports.Length; I++) {
                                    e_entry = elm[I];
                                    if (e_entry.ValueKind != System.Text.Json.JsonValueKind.String) {
                                        throw new InvalidOperationException("Import directive must be a dot-delimited string.");
                                    }
                                    temp = e_entry.GetString();
                                    VerifyTypeName(temp);
                                    dci.NamespacedImports[I] = temp;
                                }
                                break;
                            case System.Text.Json.JsonValueKind.String:
                                temp = elm.GetString();
                                VerifyTypeName(temp);
                                dci.NamespacedImports = new string[] { temp };
                                break;
                        }
                    }

                    elm = jdt.RootElement.GetProperty("name");
                    if (elm.ValueKind != System.Text.Json.JsonValueKind.String) {
                        throw new InvalidOperationException("The interface name must be a string.");
                    }
                    VerifyName(dci.FriendlyName = elm.GetString());

                    elm = jdt.RootElement.GetProperty("iid");
                    if (elm.ValueKind != System.Text.Json.JsonValueKind.String) {
                        throw new InvalidOperationException("The interface name must be a string.");
                    }

                    if (GeneratorUtils.HasInvalidIID(dci.IID = elm.GetString())) {
                        throw new InvalidOperationException($"The interface named as {dci.FriendlyName} specified invalid interface ID: {dci.IID}");
                    }

                    if (jdt.RootElement.TryGetProperty("namespace", out elm))
                    {
                        if (elm.ValueKind != System.Text.Json.JsonValueKind.String) {
                            throw new InvalidOperationException("The interface's namespace must be a string.");
                        } else {
                            VerifyTypeName(dci.EnclosedNamespace = elm.GetString());
                        }
                    }

                    if (jdt.RootElement.TryGetProperty("base_type" , out elm)) {
                        if (elm.ValueKind != System.Text.Json.JsonValueKind.String) {
                            throw new InvalidOperationException("The interface's namespace must be a string.");
                        } else {
                            dci.BaseType = new(elm.GetString());
                            if (dci.BaseType.PointerIndirections > 0) {
                                throw new InvalidOperationException("Pointer types are not allowed as the base interface of a COM interface.");
                            }
                        }
                    } else {
                        dci.BaseType = null;
                    }

                    elm = jdt.RootElement.GetProperty("methods");

                    if (elm.ValueKind != System.Text.Json.JsonValueKind.Array) {
                        throw new InvalidOperationException("Methods field must be an array of JSON objects.");
                    }

                    Method m;
                    dci.Methods = new Method[elm.GetArrayLength()];

                    System.Text.Json.JsonElement elm_2, elm_3;

                    for (int I = 0; I < dci.Methods.Length; I++) {
                        elm_2 = elm[I];
                        if (elm_2.ValueKind != System.Text.Json.JsonValueKind.Object) {
                            throw new InvalidOperationException("Methods field must be an array of JSON objects.");
                        } else {
                            m = new();

                            elm_3 = elm_2.GetProperty("name");

                            if (elm_3.ValueKind != System.Text.Json.JsonValueKind.String) {
                                throw new InvalidOperationException("Method's name must be a string.");
                            } else {
                                VerifyName(m.Name = elm_3.GetString());
                            }

                            elm_3 = elm_2.GetProperty("return_type");

                            if (elm_3.ValueKind != System.Text.Json.JsonValueKind.String) {
                                throw new InvalidOperationException("Method's return type must be a string.");
                            } else {
                                m.ReturnType = new(elm_3.GetString());
                            }

                            elm_3 = elm_2.GetProperty("arguments");
                            m.Arguments = DecodeMethodArgs(elm_3);

                            dci.Methods[I] = m;
                        }
                    }

                    return dci;
                }
            }
        }

        private static MethodArgument[] DecodeMethodArgs(System.Text.Json.JsonElement element)
        {
            if (element.ValueKind != System.Text.Json.JsonValueKind.Array) {
                throw new InvalidOperationException("The method's parameter declarations must be a JSON array.");
            } else {
                MethodArgument[] args = new MethodArgument[element.GetArrayLength()];
                for (int I = 0; I < args.Length; I++) {
                    args[I] = DecodeMethodArg(element[I]);
                }
                return args;
            }
        }

        private static MethodArgument DecodeMethodArg(System.Text.Json.JsonElement element)
        {
            if (element.ValueKind != System.Text.Json.JsonValueKind.Object) {
                throw new InvalidOperationException("The method's parameter must be a JSON object.");
            } else {
                MethodArgument arg = new();
                System.Text.Json.JsonElement elm = element.GetProperty("name");
                if (elm.ValueKind != System.Text.Json.JsonValueKind.String) {
                    throw new InvalidOperationException("The method's parameter name must be a string.");
                } else {
                    VerifyName(arg.Name = elm.GetString());
                    elm = element.GetProperty("type");
                    if (elm.ValueKind != System.Text.Json.JsonValueKind.String) {
                        throw new InvalidOperationException("The method's parameter type must be a string.");
                    } else {
                        arg.Type = new(elm.GetString());
                    }
                    return arg;
                }
            }
        }
    }
}