
using MP;
using System;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe class WinInet
    {
        public const System.UInt16 INTERNET_INVALID_PORT_NUMBER = 0;
        public const System.UInt16 INTERNET_DEFAULT_FTP_PORT = 21;
        public const System.UInt16 INTERNET_DEFAULT_HTTP_PORT = 80;
        public const System.UInt16 INTERNET_DEFAULT_HTTPS_PORT = 443;
        public const System.UInt16 INTERNET_DEFAULT_SOCKS_PORT = 1080;

        public enum InternetAccessType : System.UInt32
        {
            None = 0,
            INTERNET_OPEN_TYPE_PRECONFIG = 0,   // use registry configuration
            INTERNET_OPEN_TYPE_DIRECT = 1,   // direct to net
            INTERNET_OPEN_TYPE_PROXY = 3,   // via named proxy
            INTERNET_OPEN_TYPE_PRECONFIG_WITH_NO_AUTOPROXY = 4,   // prevent using java/script/INS
        }

        [Flags]
        public enum InternetOpenFlags : System.UInt32
        {
            None = 0,
            INTERNET_FLAG_ASYNC = 0x10000000,
            INTERNET_FLAG_FROM_CACHE = 0x01000000,  // use offline semantics
            INTERNET_FLAG_OFFLINE = INTERNET_FLAG_FROM_CACHE,
        }

        public enum InternetConnectService : System.UInt32
        {
            INTERNET_SERVICE_FTP = 1,
            INTERNET_SERVICE_HTTP = 3
        }

        [Flags]
        public enum InternetConnectFlags : System.UInt32
        {
            None = 0,
            INTERNET_FLAG_IDN_DIRECT = 0x00000001,  // IDN enabled for direct connections
            INTERNET_FLAG_IDN_PROXY = 0x00000002,  // IDN enabled for proxy
            INTERNET_FLAG_RELOAD = 0x80000000,  // retrieve the original item
            INTERNET_FLAG_RAW_DATA = 0x40000000,  // FTP/gopher find: receive the item as raw (structured) data
            INTERNET_FLAG_EXISTING_CONNECT = 0x20000000,  // FTP: use existing InternetConnect handle for server if possible
            INTERNET_FLAG_ASYNC = 0x10000000,  // this request is asynchronous (where supported)
            INTERNET_FLAG_PASSIVE = 0x08000000,  // used for FTP connections
            INTERNET_FLAG_NO_CACHE_WRITE  = 0x04000000,  // don't write this item to the cache
            INTERNET_FLAG_DONT_CACHE = INTERNET_FLAG_NO_CACHE_WRITE,
            INTERNET_FLAG_MAKE_PERSISTENT = 0x02000000,  // make this item persistent in cache
            INTERNET_FLAG_FROM_CACHE = 0x01000000,  // use offline semantics
            INTERNET_FLAG_OFFLINE = INTERNET_FLAG_FROM_CACHE,
            INTERNET_FLAG_SECURE = 0x00800000,  // use PCT/SSL if applicable (HTTP)
            INTERNET_FLAG_KEEP_CONNECTION = 0x00400000,  // use keep-alive semantics
            INTERNET_FLAG_NO_AUTO_REDIRECT = 0x00200000,  // don't handle redirections automatically
            INTERNET_FLAG_READ_PREFETCH = 0x00100000,  // do background read prefetch
            INTERNET_FLAG_NO_COOKIES = 0x00080000,  // no automatic cookie handling
            INTERNET_FLAG_NO_AUTH = 0x00040000,  // no automatic authentication handling
            INTERNET_FLAG_RESTRICTED_ZONE = 0x00020000,  // apply restricted zone policies for cookies, auth
            INTERNET_FLAG_CACHE_IF_NET_FAIL = 0x00010000,  // return cache file if net request fails
            INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTP = 0x00008000, // ex: https:// to http://
            INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTPS = 0x00004000, // ex: http:// to https://
            INTERNET_FLAG_IGNORE_CERT_DATE_INVALID = 0x00002000, // expired X509 Cert.
            INTERNET_FLAG_IGNORE_CERT_CN_INVALID = 0x00001000, // bad common name in X509 Cert.
            INTERNET_FLAG_RESYNCHRONIZE = 0x00000800,  // asking wininet to update an item if it is newer
            INTERNET_FLAG_HYPERLINK = 0x00000400,  // asking wininet to do hyperlinking semantic which works right for scripts
            INTERNET_FLAG_NO_UI = 0x00000200,  // no cookie popup
            INTERNET_FLAG_PRAGMA_NOCACHE = 0x00000100,  // asking wininet to add "pragma: no-cache"
            INTERNET_FLAG_CACHE_ASYNC = 0x00000080,  // ok to perform lazy cache-write
        }

        [Flags]
        public enum HttpQueryInfoFlags : System.UInt32
        {
            HTTP_QUERY_CONTENT_ID = 3,
            HTTP_QUERY_CONTENT_ENCODING = 29,
            HTTP_QUERY_CONTENT_LENGTH = 5,
            HTTP_QUERY_CONTENT_LOCATION = 51,
            HTTP_QUERY_CONTENT_TYPE = 1,
            HTTP_QUERY_DATE = 9,
            HTTP_QUERY_EXPIRES = 10,
            HTTP_QUERY_HOST = 55,
            HTTP_QUERY_LAST_MODIFIED = 11,
            HTTP_QUERY_STATUS_TEXT = 20,
            HTTP_QUERY_WARNING = 67,
            HTTP_QUERY_FLAG_SYSTEMTIME = 0x40000000,
            HTTP_QUERY_FLAG_NUMBER64 = 0x08000000,
            HTTP_QUERY_FLAG_NUMBER = 0x20000000,
        }

        [DllImport(Libraries.WinInet , ExactSpelling = true)]
        public static extern System.UInt32 InternetAttemptConnect(System.UInt32 RSVD = 0);

        [DllImport(Libraries.WinInet, ExactSpelling = true , SetLastError = true)]
        public static extern BOOL InternetCloseHandle(System.IntPtr hinternet);

        [DllImport(Libraries.WinInet , ExactSpelling = true , SetLastError = true , EntryPoint = "InternetGetLastResponseInfoW")]
        private static extern BOOL InternetGetLastResponseInfo_Native(System.UInt32* phttperror, System.Char* buffer, System.UInt32* bufferlength);

        public static System.UInt32 InternetGetLastResponseInfo(out System.String description)
        {
            description = new('\0', 512);
            System.UInt32 httperror , bufferlength = description.Length.ToUInt32();
            BOOL ret;
            System.Int32 error;
         g_retry:
            fixed (System.Char* pb = description) 
            {
                ret = InternetGetLastResponseInfo_Native(&httperror, pb, &bufferlength);
                error = Kernel32.GetLastError();
            }
            if (ret == BOOL.FALSE) {
                if (error == Errors.ERROR_INSUFFICIENT_BUFFER) {
                    description = new('\0', bufferlength.ToInt32());
                    goto g_retry;
                } else {
                    throw new MP.ExceptionSystem.NativeWindowsException(error);
                }
            }
            return httperror;
        }

        [DllImport(Libraries.WinInet , ExactSpelling = true , EntryPoint = "InternetOpenW" , SetLastError = true)]
        private static extern System.IntPtr InternetOpen_Native(System.Char* useragent, InternetAccessType type, System.Char* proxy, System.Char* proxybypass, InternetOpenFlags flags);

        public static System.IntPtr InternetOpen(System.String useragent, InternetAccessType accesstype, System.String proxy, System.String proxybypass, InternetOpenFlags flags) 
        {
            System.IntPtr ret;
            useragent += "\0";
            if (proxy is not null) { proxy += "\0"; }
            if (proxybypass is not null) { proxybypass += "\0"; }
            fixed (System.Char* ugp = useragent)
            fixed (System.Char* pprox = proxy)
            fixed (System.Char* pbprox = proxybypass)
            {
                ret = InternetOpen_Native(ugp, accesstype , pprox , pbprox , flags);
            }
            return ret;
        }

        [DllImport(Libraries.WinInet , ExactSpelling = true , EntryPoint = "InternetReadFile" , SetLastError = true)]
        private static extern BOOL InternetReadFile_Native(System.IntPtr hinternet, void* buffer, System.UInt32 nbytestoread, System.UInt32* pbytesread);

        public static BOOL InternetReadFile(System.IntPtr hinternet , System.Byte[] buffer , System.Int32 index , System.Int32 count , out System.Int32 bytesread)
        {
            System.UInt32 br;
            BOOL ret;
            fixed (System.Byte* pbuf = &buffer[index])
            {
                ret = InternetReadFile_Native(hinternet, pbuf, count.ToUInt32(), &br);
            }
            bytesread = br.ToInt32();
            return ret;
        }

        public static BOOL InternetReadFile(System.IntPtr hinternet , System.Span<System.Byte> buffer , out System.Int32 bytesread)
        {
            System.UInt32 br;
            BOOL ret;
            fixed (System.Byte* pbuf = buffer)
            {
                ret = InternetReadFile_Native(hinternet, pbuf, buffer.Length.ToUInt32(), &br);
            }
            bytesread = br.ToInt32();
            return ret;
        }

        [DllImport(Libraries.WinInet , EntryPoint = "InternetOpenUrlW" , ExactSpelling = true , SetLastError = true)]
        private static extern System.IntPtr InternetOpenUrl_Native(System.IntPtr hinternet , System.Char* url , System.Char* headers , System.UInt32 headerslen , InternetConnectFlags flags , void* userdata);

        public static System.IntPtr InternetOpenUrl(System.IntPtr hinternet , System.String url , System.String headers , InternetConnectFlags flags)
        {
            url += "\0";
            System.UInt32 phdlen = 0;
            if (headers is not null) { phdlen = headers.Length.ToUInt32(); }
            fixed (System.Char* pu = url)
            fixed (System.Char* phdr = headers)
            {
                return InternetOpenUrl_Native(hinternet, pu, phdr, phdlen, flags, null);
            }
        }

        [DllImport(Libraries.WinInet , ExactSpelling = true , EntryPoint = "InternetConnectW" , SetLastError = true)]
        private static extern System.IntPtr InternetConnect_Native(System.IntPtr hinternet, System.Char* url, System.UInt16 port, System.Char* username, System.Char* password, InternetConnectService service, InternetConnectFlags cfs, void* userdata);

        public static System.IntPtr InternetConnect(System.IntPtr hinternet , System.String url , System.UInt16 port , System.String username , System.String password , InternetConnectService serv , InternetConnectFlags flags)
        {
            url += "\0";
            if (username is not null) { username += "\0"; }
            if (password is not null) { password += "\0"; }
            fixed (System.Char* purl = url)
            fixed (System.Char* pusername = username)
            fixed (System.Char* ppassword = password)
            {
                return InternetConnect_Native(hinternet, purl, port, pusername, ppassword, serv, flags, null);
            }
        }

        [DllImport(Libraries.WinInet, EntryPoint = "HttpOpenRequestW", ExactSpelling = true, SetLastError = true)]
        private static extern System.IntPtr HttpOpenRequest_Native(System.IntPtr hinternet, System.Char* verb, System.Char* objname, System.Char* httpversion, System.Char* referrer, System.Char** accepttypes, InternetConnectFlags flags , void* context);

        public static System.IntPtr HttpOpenRequest(System.IntPtr hinternet  , System.String verb , System.String target , System.String httpversion , System.String referrer , System.String[] accepttypes, InternetConnectFlags flags)
        {
            verb += "\0";
            target += "\0";
            if (httpversion is not null) { httpversion += "\0"; }
            if (referrer is not null) { referrer += "\0"; }
            SafeLibcMemoryHandle memory = null;
            System.Char** acttypes = null;
            List<SafeLibcMemoryHandle> memhandles = null;
            if (accepttypes is not null && 
                accepttypes.Length > 0) {
                memory = new(sizeof(System.Char*) * (accepttypes.Length + 1));
                acttypes = (System.Char**)memory.MemoryPointer;
                memhandles = new(accepttypes.Length);
                for (System.Int32 I = 0; I < accepttypes.Length; I++) 
                {
                    memhandles.Add(accepttypes[I].ToNativeUnicodeString());
                    acttypes[I] = (System.Char*)memhandles[memhandles.Count - 1].MemoryPointer;
                }
                acttypes[accepttypes.Length] = null; // By the specification , this is required
            }
            System.IntPtr ret;
            fixed (System.Char* pv = verb)
            fixed (System.Char* phv = httpversion)
            fixed (System.Char* pt = target)
            fixed (System.Char* pr = referrer)
            {
                ret = HttpOpenRequest_Native(hinternet, pv, pt, phv, pr, acttypes, flags, null);
            }
            if (memhandles is not null) 
            {
                foreach (var str in memhandles) { str.Dispose(); }
                memhandles.Clear();
                memhandles = null;
                memory.Dispose();
                memory = null;
            }
            return ret;
        }

        [DllImport(Libraries.WinInet , EntryPoint = "HttpSendRequestW" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL HttpSendRequest_Native(System.IntPtr hinternet, System.Char* headers, System.UInt32 hdrlength, void* poptional, System.UInt32 optionallen);

        public static BOOL HttpSendRequest(System.IntPtr hinternet , System.String headers , System.Byte[] dataoptional = null)
        {
            if (headers is not null) { headers += "\0"; }
            System.UInt32 optlen = (dataoptional is null ? 0 : dataoptional.Length).ToUInt32() , 
                hdrlen = (headers is null ? 0 : headers.Length).ToUInt32();
            fixed (System.Char* phdr = headers)
            fixed (System.Byte* psrc = dataoptional)
            {
                return HttpSendRequest_Native(hinternet, phdr, hdrlen, psrc, optlen);
            }
        }

        [DllImport(Libraries.WinInet , EntryPoint = "HttpQueryInfoW" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL HttpQueryInfo_Native(System.IntPtr hinternet, HttpQueryInfoFlags flags, void* info, System.UInt32* infosize, System.UInt32* hdrindex);

        public static BOOL HttpQueryInfo_GetSYSTEMTIME(System.IntPtr hinternet , HttpQueryInfoFlags flg , out SYSTEMTIME info , ref System.UInt32 index)
        {
            SYSTEMTIME ifrt;
            System.UInt32 wb = sizeof(SYSTEMTIME).ToUInt32();
            fixed (System.UInt32* pidx = &index)
            {
                BOOL ret = HttpQueryInfo_Native(hinternet, flg | HttpQueryInfoFlags.HTTP_QUERY_FLAG_SYSTEMTIME, &ifrt, &wb, pidx);
                info = ifrt;
                return ret;
            }
        }

        public static BOOL HttpQueryInfo_GetUInt32(System.IntPtr hinternet , HttpQueryInfoFlags flags , out System.UInt32 number , ref System.UInt32 index)
        {
            System.UInt32 pret;
            System.UInt32 wb = sizeof(System.UInt32);
            BOOL ret;
            fixed (System.UInt32* pidx = &index)
            {
                ret = HttpQueryInfo_Native(hinternet, flags | HttpQueryInfoFlags.HTTP_QUERY_FLAG_NUMBER, &pret, &wb, pidx);
                number = pret;
                return ret;
            }
        }

        public static BOOL HttpQueryInfo_GetUInt64(System.IntPtr hinternet, HttpQueryInfoFlags flags, out System.UInt64 number, ref System.UInt32 index)
        {
            System.UInt64 pret;
            System.UInt32 wb = sizeof(System.UInt64);
            BOOL ret;
            fixed (System.UInt32* pidx = &index)
            {
                ret = HttpQueryInfo_Native(hinternet, flags | HttpQueryInfoFlags.HTTP_QUERY_FLAG_NUMBER64, &pret, &wb, pidx);
                number = pret;
                return ret;
            }
        }

        public static BOOL HttpQueryInfo_GetString(System.IntPtr hinternet, HttpQueryInfoFlags flags, out System.String strg, ref System.UInt32 index)
        {
            System.String str = new('\0', 512);
            System.UInt32 wb = (str.Length * sizeof(System.Char)).ToUInt32();
            BOOL ret;
            System.Int32 error;
        g_retry:
            fixed (System.Char* psrc = str)
            fixed (System.UInt32* pidx = &index)
            {
                ret = HttpQueryInfo_Native(hinternet, flags, psrc, &wb, pidx);
                error = Kernel32.GetLastError();
                if (ret == BOOL.FALSE) {
                    if (error == Errors.ERROR_INSUFFICIENT_BUFFER) {
                        str = new('\0', wb.ToInt32());
                        goto g_retry;
                    } else {
                        throw new MP.ExceptionSystem.NativeWindowsException(error);
                    }
                }
            }
            System.Int32 idx = str.IndexOf('\0');
            if (idx > -1) {
                strg = str.Remove(idx);
            } else {
                strg = str;
            }
            return ret;
        }

        [System.Diagnostics.StackTraceHidden]
        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        public static void ThrowAppropriateException(System.Int32 err = 0)
        {
            if (err == 0) { err= Kernel32.GetLastError(); }
            switch (err)
            {
                case Errors.ERROR_INTERNET_EXTENDED_ERROR:
                    // It represents an extended error , call GetLastResponseInfo
                    System.UInt32 httperr = InternetGetLastResponseInfo(out var desc);
                    throw new MP.ExceptionSystem.HttpFailureException(httperr.ToInt32(), desc);
                case Errors.ERROR_INTERNET_INTERNAL_ERROR:
                    throw new MP.ExceptionSystem.HttpFailureException("An internal failure has been occured.");
                case Errors.ERROR_INTERNET_INVALID_OPERATION:
                    throw new System.InvalidOperationException("Attempted to execute an invalid operation.");
                case Errors.ERROR_INTERNET_INVALID_URL:
                    throw new System.UriFormatException("The format of the URL was invalid.");
                case Errors.ERROR_INTERNET_NAME_NOT_RESOLVED:
                    throw new MP.ExceptionSystem.HttpFailureException("Cannot resolve the given URL.");
                case Errors.ERROR_INTERNET_OUT_OF_HANDLES:
                    throw new System.AggregateException("The Internet API cannot handle the request because it has run out of handles.");
                case Errors.ERROR_INTERNET_SERVER_UNREACHABLE:
                    throw new MP.ExceptionSystem.HttpFailureException("The server was unreachable.");
                case Errors.ERROR_INTERNET_TIMEOUT:
                    throw new System.TimeoutException("The connection timed out.");
                default:
                    throw new MP.ExceptionSystem.NativeWindowsException(err);
            }
        }
    }
}