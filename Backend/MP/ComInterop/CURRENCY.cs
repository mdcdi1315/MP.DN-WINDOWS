
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

// CS0659: 'CURRENCY' overrides Object.Equals(object o) but does not override Object.GetHashCode()
// CS0661: 'CURRENCY' defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning disable 0659 , 0661

namespace MP.ComInterop
{ 
    /// <summary>
    /// A simplified version of the CY structure for Ole Automation, plus some operations for creating and getting currencies from a <see cref="PROPVARIANT"/>. <br />
    /// A <see cref="CURRENCY"/> structure is used to save economical values. <br />
    /// Unlike other floating-point implementations, the structure allows to save only up to 4 decimal digits, and any numeric operations
    /// happening on them with more than 4 digits rounds those to the closest fraction part with 4 decimal digits. <br />
    /// It provides specialized operators for working with currencies, plus performance operators for working also with doubles altogether. <br />
    /// Among these, it also supports negation and has a specialized method for retrieveing an absolute <see cref="CURRENCY"/> value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CURRENCY : 
        IUnaryNegationOperators<CURRENCY , CURRENCY>,
        IAdditionOperators<CURRENCY , CURRENCY , CURRENCY>,
        ISubtractionOperators<CURRENCY , CURRENCY , CURRENCY>,
        IMultiplyOperators<CURRENCY , CURRENCY , CURRENCY>,
        IMultiplyOperators<CURRENCY, System.Int32, CURRENCY>,
        IMultiplyOperators<CURRENCY, System.Int64, CURRENCY>,
        IComparisonOperators<CURRENCY , CURRENCY , System.Boolean>,
        IComparisonOperators<CURRENCY, System.Double, System.Boolean>,
        IEquatable<CURRENCY>
    {
        private readonly System.Int64 Raw;

        public static CURRENCY FromDouble(System.Double dbl)
        {
            Interop.OleAut32.OleAutomation_MapException(Interop.OleAut32.VarCyFromR8(dbl, out CURRENCY cy));
            return cy;
        }

        public static CURRENCY FromSingle(System.Single flt)
        {
            Interop.OleAut32.OleAutomation_MapException(Interop.OleAut32.VarCyFromR4(flt, out CURRENCY cy));
            return cy;
        }

        public static CURRENCY FromCultureDependentString(System.String str)
        {
            Interop.OleAut32.OleAutomation_MapException(
                Interop.OleAut32.VarCyFromStr(
                    str, 
                    Interop.OleAut32.LOCALE_USE_NLS, 
                    Interop.OleAut32.VarAPIFlags.None, 
                    out var c)
            );
            return c;
        }

        public static CURRENCY Absolute(CURRENCY current)
        {
            Interop.OleAut32.VarCyAbs(current, out var ret);
            return ret;
        }

        public static implicit operator CURRENCY(System.Double dbl) => FromDouble(dbl);

        public static implicit operator CURRENCY(System.Single flt) => FromSingle(flt);

        public static explicit operator System.Double(CURRENCY c)
        {
            Interop.OleAut32.OleAutomation_MapException(Interop.OleAut32.VarR8FromCy(c, out var ret));
            return ret;
        }

        public static explicit operator System.Single(CURRENCY c)
        {
            Interop.OleAut32.OleAutomation_MapException(Interop.OleAut32.VarR4FromCy(c, out var ret));
            return ret;
        }

        public static System.Boolean operator ==(CURRENCY c1 , CURRENCY c2) => Interop.OleAut32.VarCyCmp(c1, c2) == Interop.OleAut32.VARCMP_EQ;

        public static System.Boolean operator !=(CURRENCY c1, CURRENCY c2) => Interop.OleAut32.VarCyCmp(c1, c2) != Interop.OleAut32.VARCMP_EQ;

        public static System.Boolean operator <(CURRENCY c1, CURRENCY c2) => Interop.OleAut32.VarCyCmp(c1, c2) == Interop.OleAut32.VARCMP_LT;

        public static System.Boolean operator >(CURRENCY c1, CURRENCY c2) => Interop.OleAut32.VarCyCmp(c1, c2) == Interop.OleAut32.VARCMP_GT;

        public static System.Boolean operator ==(CURRENCY c, System.Double dbl) => Interop.OleAut32.VarCyCmpR8(c, dbl) == Interop.OleAut32.VARCMP_EQ;

        public static System.Boolean operator !=(CURRENCY c, System.Double dbl) => Interop.OleAut32.VarCyCmpR8(c, dbl) != Interop.OleAut32.VARCMP_EQ;

        public static System.Boolean operator >(CURRENCY c , System.Double dbl) => Interop.OleAut32.VarCyCmpR8(c, dbl) == Interop.OleAut32.VARCMP_GT;

        public static System.Boolean operator <(CURRENCY c1, System.Double dbl) => Interop.OleAut32.VarCyCmpR8(c1, dbl) == Interop.OleAut32.VARCMP_LT;

        public static System.Boolean operator >=(CURRENCY c1, CURRENCY c2)
        {
            System.Int32 ret = Interop.OleAut32.VarCyCmp(c1, c2);
            return ret == Interop.OleAut32.VARCMP_GT || ret == Interop.OleAut32.VARCMP_EQ;
        }

        public static System.Boolean operator <=(CURRENCY c1, CURRENCY c2)
        {
            System.Int32 ret = Interop.OleAut32.VarCyCmp(c1, c2);
            return ret == Interop.OleAut32.VARCMP_LT || ret == Interop.OleAut32.VARCMP_EQ;
        }

        public static System.Boolean operator >=(CURRENCY c, System.Double dbl)
        {
            System.Int32 ret = Interop.OleAut32.VarCyCmpR8(c, dbl);
            return ret <= Interop.OleAut32.VARCMP_GT && ret >= Interop.OleAut32.VARCMP_EQ;
        }

        public static System.Boolean operator <=(CURRENCY c, System.Double dbl)
        {
            System.Int32 ret = Interop.OleAut32.VarCyCmpR8(c, dbl);
            return ret >= Interop.OleAut32.VARCMP_LT && ret <= Interop.OleAut32.VARCMP_EQ;
        }

        public static CURRENCY operator -(CURRENCY c)
        {
            Interop.OleAut32.VarCyNeg(c, out var ret);
            return ret;
        }

        public static CURRENCY operator -(CURRENCY c1 , CURRENCY c2)
        {
            Interop.OleAut32.OleAutomation_MapException(Interop.OleAut32.VarCySub(c1, c2, out var ret));
            return ret;
        }

        public static CURRENCY operator +(CURRENCY c1, CURRENCY c2)
        {
            Interop.OleAut32.OleAutomation_MapException(Interop.OleAut32.VarCyAdd(c1, c2, out var ret));
            return ret;
        }

        public static CURRENCY operator *(CURRENCY c1, CURRENCY c2)
        {
            Interop.OleAut32.OleAutomation_MapException(Interop.OleAut32.VarCyMul(c1, c2, out var ret));
            return ret;
        }

        public static CURRENCY operator *(CURRENCY c, System.Int32 intv)
        {
            Interop.OleAut32.OleAutomation_MapException(Interop.OleAut32.VarCyMulI4(c, intv, out var ret));
            return ret;
        }

        public static CURRENCY operator *(CURRENCY c, System.Int64 lgv)
        {
            Interop.OleAut32.OleAutomation_MapException(Interop.OleAut32.VarCyMulI8(c, lgv, out var ret));
            return ret;
        }

        public System.Boolean Equals(CURRENCY other) => Interop.OleAut32.VarCyCmp(this, other) == Interop.OleAut32.VARCMP_EQ;

        public override bool Equals([NotNullWhen(true)] object obj)
            => obj switch {
                System.Double dbl => Interop.OleAut32.VarCyCmpR8(this, dbl) == Interop.OleAut32.VARCMP_EQ,
                CURRENCY other => Equals(other),
                _ => false,
            };

        public override System.String ToString()
        {
            Interop.OleAut32.OleAutomation_MapException(
                Interop.OleAut32.VarBstrFromCy(this, 
                Interop.OleAut32.LOCALE_USE_NLS, 
                Interop.OleAut32.VarAPIFlags.None, 
                out BSTR bstr)
            );
            try {
                return bstr.ToString();
            } finally {
                bstr.Dispose();
            }
        }
    }
    
}
