
using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// #pragma warning disable RS1035 // Do not use APIs banned for analyzers

namespace MP.AbstractionsLib.Analyzer.COMSourceGeneration
{
    public static class COMInformationSyntaxProvider
    {
        public static bool COMGeneratorInterfaceDeclPred(SyntaxNode node, CancellationToken ct) => node is InterfaceDeclarationSyntax;

        public static DecodedCOMInterface COMGeneratorGatherInterface(GeneratorAttributeSyntaxContext gsc, CancellationToken ct)
        {
            try {
                if (gsc.TargetNode is not InterfaceDeclarationSyntax ids) {
                    return null;
                } else {
                    DecodedCOMInterface dci = null;

                    if (gsc.SemanticModel.GetDeclaredSymbol(ids) is ITypeSymbol decl)
                    {
                        dci = DecodedCOMInterface.FromType(decl);
                    }
                    
                    return dci;
                }
            } catch (Exception)
            {
                // System.Console.WriteLine("FAIL: " + ex);
                return null;
            }
        }
    }
}