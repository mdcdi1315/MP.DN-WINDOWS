
using MP.ComInterop;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe class OleAut32
    {
        public enum VarAPIFlags : System.UInt32
        {
            None = 0,
            /*
             * The VarDateFromStr and VarBstrFromDate functions also accept the
             * VAR_TIMEVALUEONLY and VAR_DATEVALUEONLY flags
             */
            VAR_TIMEVALUEONLY = 0x00000001,
            VAR_DATEVALUEONLY = 0x00000002,
            /* VarDateFromUdate() only */
            VAR_VALIDDATE = 0x00000004,
            /* 
             * Accepted by all date & format APIs 
             * use Hijri calender
             */
            VAR_CALENDAR_HIJRI = 0x00000008,
            /* Booleans can optionally be accepted in localized form. Pass VAR_LOCALBOOL
              * into VarBoolFromStr and VarBstrFromBool to use localized boolean names
              */
            VAR_LOCALBOOL = 0x00000010,
            /* When passed into VarFormat and VarFormatFromTokens, prevents substitution
              * of formats in the case where a string is passed in that can not be
              * coverted into the desired type. (for ex, 'Format("Hello", "General Number")')
              */
            VAR_FORMAT_NOSUBSTITUTE = 0x00000020,
            /*
              * For VarBstrFromDate only - forces years to be 4 digits rather than shortening
              * to 2-digits when the years is in the date window.
              */
            VAR_FOURDIGITYEARS = 0x00000040,
            // SOUTHASIA START
            /* SOUTHASIA
              * For VarBstrFromDate only - forces years to be 4 digits rather than shortening
              * to 2-digits when the years is in the date window.
              */
            VAR_CALENDAR_THAI = 0x00000080,
            VAR_CALENDAR_GREGORIAN = 0x00000100
            // SOUTHASIA END
        }

        /*
         * Use NLS functions to format date, currency, time, and number.
         */
        public const System.Int32 LOCALE_USE_NLS = 0x10000000;

        public const System.Int32 VARCMP_LT = 0;
        public const System.Int32 VARCMP_EQ = 1;
        public const System.Int32 VARCMP_GT = 2;
        public const System.Int32 VARCMP_NULL = 3;

        [System.Diagnostics.StackTraceHidden]
        public static void OleAutomation_MapException(HRESULT hr)
        {
            if (hr.SUCCEEDED) { return; }
            throw (System.Int32)hr switch {
                CommonHResults.DISP_E_BADVARTYPE => new System.TypeLoadException("Bad variant type."),
                CommonHResults.DISP_E_OVERFLOW => new System.OverflowException("Operation resulted in an overflow."),
                CommonHResults.DISP_E_TYPEMISMATCH => new System.TypeLoadException("Type mismatch between input and output types"),
                CommonHResults.E_OUTOFMEMORY => new System.OutOfMemoryException("The operation could not be completed due to memory depletion."),
                CommonHResults.E_INVALIDARG => new System.ArgumentException("Invalid argument passed."),
                _ => hr.MappingException,
            };
        }

        // CY API

        [DllImport(Libraries.OleAut32 , EntryPoint = "VarCyFromR4", ExactSpelling = true)]
        private static extern HRESULT VarCyFromR4_Native(System.Single fltval , CURRENCY* pcy);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarCyFromR8", ExactSpelling = true)]
        private static extern HRESULT VarCyFromR8_Native(System.Double dblval, CURRENCY* pcy);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarR8FromCy", ExactSpelling = true)]
        private static extern HRESULT VarR8FromCy_Native(CURRENCY cy, System.Double* pdbl);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarR4FromCy", ExactSpelling = true)]
        private static extern HRESULT VarR4FromCy_Native(CURRENCY cy, System.Single* pflt);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarCyMul", ExactSpelling = true)]
        private static extern HRESULT VarCyMul_Native(CURRENCY c1 , CURRENCY c2 , CURRENCY* cout);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarCyMulI4", ExactSpelling = true)]
        private static extern HRESULT VarCyMulI4_Native(CURRENCY c, System.Int32 lg, CURRENCY* cout);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarCyMulI8", ExactSpelling = true)]
        private static extern HRESULT VarCyMulI8_Native(CURRENCY c, System.Int64 lg, CURRENCY* cout);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarCyNeg", ExactSpelling = true)]
        private static extern HRESULT VarCyNeg_Native(CURRENCY input, CURRENCY* pcy);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarCyAbs" , ExactSpelling = true)]
        private static extern HRESULT VarCyAbs_Native(CURRENCY input , CURRENCY* pcy);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarCyAdd", ExactSpelling = true)]
        private static extern HRESULT VarCyAdd_Native(CURRENCY c1, CURRENCY c2, CURRENCY* pcy);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarCySub", ExactSpelling = true)]
        private static extern HRESULT VarCySub_Native(CURRENCY c1, CURRENCY c2, CURRENCY* pcy);

        [DllImport(Libraries.OleAut32, EntryPoint = "VarCyFromStr" , ExactSpelling = true)]
        private static extern HRESULT VarCyFromStr_Native(/*LPCOLECHAR*/ System.Char* pstr, System.UInt32 LCID, VarAPIFlags flags, CURRENCY* pcy);
        
        [DllImport(Libraries.OleAut32, EntryPoint = "VarBstrFromCy", ExactSpelling = true)]
        private static extern HRESULT VarBstrFromCy_Native(CURRENCY cy, System.UInt32 LCID, VarAPIFlags flags, /* BSTR* -> OLECHAR** */ System.Char** bstr);

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern HRESULT VarCyCmp(CURRENCY c1, CURRENCY c2);

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern HRESULT VarCyCmpR8(CURRENCY c1, System.Double dblval);

        // End CY API

        // BSTR API

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern /* BSTR */ System.Char* SysAllocStringLen(/* OLECHAR* */ System.Char* pstring, System.UInt32 length);

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern void SysFreeString(/* BSTR */ System.Char* pstring);

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern System.UInt32 SysStringLen(/* BSTR */ System.Char* pstring);

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern BOOL SysReAllocStringLen(/* BSTR* */ System.Char** pinstring, /* OLECHAR* */ System.Char* olestringcpy, System.UInt32 length);

        // End BSTR API

        public static HRESULT VarCyFromR4(System.Single fltval , out CURRENCY currency)
        {
            CURRENCY ctt;
            HRESULT hr = VarCyFromR4_Native(fltval, &ctt);
            currency = ctt;
            return hr;
        }

        public static HRESULT VarCyFromR8(System.Double dblval, out CURRENCY currency)
        {
            CURRENCY ctt;
            HRESULT hr = VarCyFromR8_Native(dblval, &ctt);
            currency = ctt;
            return hr;
        }

        public static HRESULT VarCyMul(CURRENCY one , CURRENCY two , out CURRENCY result)
        {
            CURRENCY ctt;
            HRESULT hr = VarCyMul_Native(one, two, &ctt);
            result = ctt;
            return hr;
        }

        public static HRESULT VarCyMulI4(CURRENCY c, System.Int32 v , out CURRENCY result)
        {
            CURRENCY ctt;
            HRESULT hr = VarCyMulI4_Native(c, v, &ctt);
            result = ctt;
            return hr;
        }

        public static HRESULT VarCyMulI8(CURRENCY c, System.Int64 v, out CURRENCY result)
        {
            CURRENCY ctt;
            HRESULT hr = VarCyMulI8_Native(c, v, &ctt);
            result = ctt;
            return hr;
        }

        public static HRESULT VarCyNeg(CURRENCY ci, out CURRENCY result)
        {
            CURRENCY ctt;
            HRESULT hr = VarCyNeg_Native(ci, &ctt);
            result = ctt;
            return hr;
        }

        public static HRESULT VarCyAdd(CURRENCY one , CURRENCY two , out CURRENCY result)
        {
            CURRENCY ctt;
            HRESULT hr = VarCyAdd_Native(one, two, &ctt);
            result = ctt;
            return hr;
        }

        public static HRESULT VarCySub(CURRENCY one, CURRENCY two, out CURRENCY result)
        {
            CURRENCY ctt;
            HRESULT hr = VarCySub_Native(one, two, &ctt);
            result = ctt;
            return hr;
        }

        public static HRESULT VarCyAbs(CURRENCY c, out CURRENCY result)
        {
            CURRENCY ctt;
            HRESULT hr = VarCyAbs_Native(c, &ctt);
            result = ctt;
            return hr;
        }

        public static HRESULT VarR8FromCy(CURRENCY c, out System.Double result)
        {
            System.Double dbl;
            HRESULT hr = VarR8FromCy_Native(c, &dbl);
            result = dbl;
            return hr;
        }

        public static HRESULT VarR4FromCy(CURRENCY c, out System.Single result)
        {
            System.Single flt;
            HRESULT hr = VarR4FromCy_Native(c, &flt);
            result = flt;
            return hr;
        }

        public static HRESULT VarCyFromStr(System.String str , System.UInt32 lcid , VarAPIFlags flags , out CURRENCY result)
        {
            HRESULT hr;
            CURRENCY cr;
            fixed (System.Char* pstr = str) 
            {
                hr = VarCyFromStr_Native(pstr, lcid, flags, &cr);
            }
            result = cr;
            return hr;
        }

        public static HRESULT VarBstrFromCy(CURRENCY curr, System.UInt32 lcid , VarAPIFlags flags, out BSTR result)
        {
            result = null;
            System.Char* pbstr;
            HRESULT hr = VarBstrFromCy_Native(curr, lcid, flags, &pbstr);
            if (hr.SUCCEEDED) {
                result = new(pbstr);
            }
            return hr;
        }
    }
}