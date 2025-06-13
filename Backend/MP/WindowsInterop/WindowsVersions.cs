namespace MP.WindowsInterop
{
    /// <summary>
    /// Defines different Windows OS versioning constants. <br />
    /// The constants are extracted from sdkddkver.h header file.
    /// </summary>
    public static class WindowsVersions
    {
        //
        // _WIN32_WINNT version constants.
        // Note that these are exported as strings so that we can mark with SupportedOSPlatform family of System.Runtime.Versioning attributes.
        //
        public const System.String _WIN32_WINNT_NT4 = "windows4.0";
        public const System.String _WIN32_WINNT_WIN2K = "windows5.0";
        public const System.String _WIN32_WINNT_WINXP = "windows5.1";
        public const System.String _WIN32_WINNT_WS03 = "windows5.2";
        public const System.String _WIN32_WINNT_WIN6 = "windows6.0";
        public const System.String _WIN32_WINNT_VISTA = _WIN32_WINNT_WIN6;
        public const System.String _WIN32_WINNT_WS08 = _WIN32_WINNT_WIN6;
        public const System.String _WIN32_WINNT_LONGHORN = _WIN32_WINNT_WIN6;
        public const System.String _WIN32_WINNT_WIN7 = "windows6.1";
        public const System.String _WIN32_WINNT_WIN8 = "windows6.2";
        public const System.String _WIN32_WINNT_WINBLUE = "windows6.3";
        public const System.String _WIN32_WINNT_WINTHRESHOLD = "windows10.0"; /* ABRACADABRA_THRESHOLD */
        public const System.String _WIN32_WINNT_WIN10 = "windows10.0"; /* ABRACADABRA_THRESHOLD */

        // NTDDI version constants are even more specific, touching down the entire OS string.

        //
        // NTDDI version constants
        //
        // Note that up to Windows 8.1 MSFT was using 4 digits for service packs.
        public const System.String NTDDI_WIN2K = "windows05.0.0000.0";
        public const System.String NTDDI_WIN2KSP1 = "windows05.0.1000.0";
        public const System.String NTDDI_WIN2KSP2 = "windows05.0.2000.0";
        public const System.String NTDDI_WIN2KSP3 = "windows05.0.3000.0";
        public const System.String NTDDI_WIN2KSP4 = "windows05.0.4000.0";

        public const System.String NTDDI_WINXP = "windows05.1.0000.0";
        public const System.String NTDDI_WINXPSP1 = "windows05.1.1000.0";
        public const System.String NTDDI_WINXPSP2 = "windows05.1.2000.0";
        public const System.String NTDDI_WINXPSP3 = "windows05.1.3000.0";
        public const System.String NTDDI_WINXPSP4 = "windows05.1.4000.0";

        public const System.String NTDDI_WS03 = "windows05.2.0000.0";
        public const System.String NTDDI_WS03SP1 = "windows05.2.1000.0";
        public const System.String NTDDI_WS03SP2 = "windows05.2.2000.0";
        public const System.String NTDDI_WS03SP3 = "windows05.2.3000.0";
        public const System.String NTDDI_WS03SP4 = "windows05.2.4000.0";

        public const System.String NTDDI_WIN6       = "windows06.0.0000.0";
        public const System.String NTDDI_WIN6SP1 = "windows06.0.1000.0";
        public const System.String NTDDI_WIN6SP2 = "windows06.0.2000.0";
        public const System.String NTDDI_WIN6SP3 = "windows06.0.3000.0";
        public const System.String NTDDI_WIN6SP4 = "windows06.0.4000.0";

        public const System.String NTDDI_VISTA = NTDDI_WIN6;
        public const System.String NTDDI_VISTASP1 = NTDDI_WIN6SP1;
        public const System.String NTDDI_VISTASP2 = NTDDI_WIN6SP2;
        public const System.String NTDDI_VISTASP3 = NTDDI_WIN6SP3;
        public const System.String NTDDI_VISTASP4 = NTDDI_WIN6SP4;

        public const System.String NTDDI_LONGHORN = NTDDI_VISTA;

        public const System.String NTDDI_WS08 = NTDDI_WIN6SP1;
        public const System.String NTDDI_WS08SP2 = NTDDI_WIN6SP2;
        public const System.String NTDDI_WS08SP3 = NTDDI_WIN6SP3;
        public const System.String NTDDI_WS08SP4 = NTDDI_WIN6SP4;

        public const System.String NTDDI_WIN7 = "windows06.1.0000.0";
        public const System.String NTDDI_WIN8 = "windows06.2.0000.0";
        public const System.String NTDDI_WINBLUE = "windows06.3.0000.0"; // Aka 'Windows 8.1'.
        public const System.String NTDDI_WINTHRESHOLD = "windows10.0.00000.0";  /* ABRACADABRA_THRESHOLD */
        public const System.String NTDDI_WIN10 = "windows10.0.00000.0";  /* ABRACADABRA_THRESHOLD */
        public const System.String NTDDI_WIN10_TH2 = "windows10.0.11000.0";  /* ABRACADABRA_WIN10_TH2 */
        public const System.String NTDDI_WIN10_RS1 = "windows10.0.12000.0";  /* ABRACADABRA_WIN10_RS1 */
        public const System.String NTDDI_WIN10_RS2 = "windows10.0.13000.0";  /* ABRACADABRA_WIN10_RS2 */
        public const System.String NTDDI_WIN10_RS3 = "windows10.0.14000.0";  /* ABRACADABRA_WIN10_RS3 */
        public const System.String NTDDI_WIN10_RS4 = "windows10.0.15000.0";  /* ABRACADABRA_WIN10_RS4 */
        public const System.String NTDDI_WIN10_RS5 = "windows10.0.16000.0";  /* ABRACADABRA_WIN10_RS5 */
        public const System.String NTDDI_WIN10_19H1 = "windows10.0.17000.0";  /* ABRACADABRA_WIN10_19H1*/
        public const System.String NTDDI_WIN10_VB = "windows10.0.18000.0";  /* ABRACADABRA_WIN10_VB */
        public const System.String NTDDI_WIN10_MN = "windows10.0.19000.0";
        // Windows 11 from now on!!!!!!
        public const System.String NTDDI_WIN10_FE = "windows10.0.20000.0";
        // Windows 11 23H2
        public const System.String NTDDI_WIN10_CO = "windows10.0.21000.0";
        // Windows 11 24H2
        public const System.String NTDDI_WIN10_NI = "windows10.0.22000.0";

    }
}