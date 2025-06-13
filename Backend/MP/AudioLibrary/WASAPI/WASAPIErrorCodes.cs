
using MP.ComInterop;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Defines WAS API <see cref="HRESULT"/> codes.
    /// </summary>
    public static class WASAPIErrorCodes
    {
        public const System.Int32 AUDCLNT_E_NOT_INITIALIZED = unchecked((System.Int32)0x88890001);

        public const System.Int32 AUDCLNT_E_ALREADY_INITIALIZED = unchecked((System.Int32)0x88890002);

        public const System.Int32 AUDCLNT_E_WRONG_ENDPOINT_TYPE = unchecked((System.Int32)0x88890003);

        public const System.Int32 AUDCLNT_E_DEVICE_INVALIDATED = unchecked((System.Int32)0x88890004);

        public const System.Int32 AUDCLNT_E_NOT_STOPPED = unchecked((System.Int32)0x88890005);

        public const System.Int32 AUDCLNT_E_BUFFER_TOO_LARGE = unchecked((System.Int32)0x88890006);

        public const System.Int32 AUDCLNT_E_OUT_OF_ORDER = unchecked((System.Int32)0x88890007);

        public const System.Int32 AUDCLNT_E_UNSUPPORTED_FORMAT = unchecked((System.Int32)0x88890008);

        public const System.Int32 AUDCLNT_E_INVALID_SIZE = unchecked((System.Int32)0x88890009);

        public const System.Int32 AUDCLNT_E_DEVICE_IN_USE = unchecked((System.Int32)0x8889000a);

        public const System.Int32 AUDCLNT_E_BUFFER_OPERATION_PENDING = unchecked((System.Int32)0x8889000b);

        public const System.Int32 AUDCLNT_E_THREAD_NOT_REGISTERED = unchecked((System.Int32)0x8889000c);

        public const System.Int32 AUDCLNT_E_EXCLUSIVE_MODE_NOT_ALLOWED = unchecked((System.Int32)0x8889000e);

        public const System.Int32 AUDCLNT_E_ENDPOINT_CREATE_FAILED = unchecked((System.Int32)0x8889000f);

        public const System.Int32 AUDCLNT_E_SERVICE_NOT_RUNNING = unchecked((System.Int32)0x88890010);

        public const System.Int32 AUDCLNT_E_EVENTHANDLE_NOT_EXPECTED = unchecked((System.Int32)0x88890011);

        public const System.Int32 AUDCLNT_E_EXCLUSIVE_MODE_ONLY = unchecked((System.Int32)0x88890012);

        public const System.Int32 AUDCLNT_E_BUFDURATION_PERIOD_NOT_EQUAL = unchecked((System.Int32)0x88890013);

        public const System.Int32 AUDCLNT_E_EVENTHANDLE_NOT_SET = unchecked((System.Int32)0x88890014);

        public const System.Int32 AUDCLNT_E_INCORRECT_BUFFER_SIZE = unchecked((System.Int32)0x88890015);

        public const System.Int32 AUDCLNT_E_BUFFER_SIZE_ERROR = unchecked((System.Int32)0x88890016);

        public const System.Int32 AUDCLNT_E_CPUUSAGE_EXCEEDED = unchecked((System.Int32)0x88890017);

        public const System.Int32 AUDCLNT_E_BUFFER_ERROR = unchecked((System.Int32)0x88890018);

        public const System.Int32 AUDCLNT_E_BUFFER_SIZE_NOT_ALIGNED = unchecked((System.Int32)0x88890019);

        public const System.Int32 AUDCLNT_E_INVALID_DEVICE_PERIOD = unchecked((System.Int32)0x88890020);

        public const System.Int32 AUDCLNT_E_INVALID_STREAM_FLAG = unchecked((System.Int32)0x88890021);

        public const System.Int32 AUDCLNT_E_ENDPOINT_OFFLOAD_NOT_CAPABLE = unchecked((System.Int32)0x88890022);

        public const System.Int32 AUDCLNT_E_OUT_OF_OFFLOAD_RESOURCES = unchecked((System.Int32)0x88890023);

        public const System.Int32 AUDCLNT_E_OFFLOAD_MODE_ONLY = unchecked((System.Int32)0x88890024);

        public const System.Int32 AUDCLNT_E_NONOFFLOAD_MODE_ONLY = unchecked((System.Int32)0x88890025);

        public const System.Int32 AUDCLNT_E_RESOURCES_INVALIDATED = unchecked((System.Int32)0x88890026);

        public const System.Int32 AUDCLNT_E_RAW_MODE_UNSUPPORTED = unchecked((System.Int32)0x88890027);

        public const System.Int32 AUDCLNT_E_ENGINE_PERIODICITY_LOCKED = unchecked((System.Int32)0x88890028);

        public const System.Int32 AUDCLNT_E_ENGINE_FORMAT_LOCKED = unchecked((System.Int32)0x88890029);

        public const System.Int32 AUDCLNT_E_HEADTRACKING_ENABLED = unchecked((System.Int32)0x88890030);

        public const System.Int32 AUDCLNT_E_HEADTRACKING_UNSUPPORTED = unchecked((System.Int32)0x88890040);

        public const System.Int32 AUDCLNT_S_BUFFER_EMPTY = 0x8890001;

        public const System.Int32 AUDCLNT_S_THREAD_ALREADY_REGISTERED = 0x8890002;

        public const System.Int32 AUDCLNT_S_POSITION_STALLED = 0x8890003;
    }
}