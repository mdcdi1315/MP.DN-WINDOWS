
using System;
using MP.ExtSystemApi;
using System.Runtime.CompilerServices;

namespace MP
{
    internal sealed class PlayerWindowUIState
    {
        [Flags]
        private enum Booleans : System.Byte
        {
            Enabled = 1,
            ClosingAssert = 2,
            OptPanel = 4,
            DDFMon = 8,
            UIShutdown = 16,
            HardFailureRecorded = 32,
        }

        private System.Int32 lvwindex;
        private System.Int16 optpanelidx;
        private System.Byte selectedchannel;
        private volatile Booleans booloptions;
        private MainMenuButtonRegistrationInfo[] mainmenubuttondata , rightclickbuttondata;

        public PlayerWindowUIState() 
        {
            rightclickbuttondata = mainmenubuttondata = System.Array.Empty<MainMenuButtonRegistrationInfo>();
            optpanelidx = 0;
            lvwindex = -1;
            selectedchannel = 0;
            booloptions = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private System.Boolean HasBooleanOption(Booleans b) => (booloptions & b) == b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBooleanOption(Booleans b , System.Boolean value)
        {
            if (value) {
                booloptions |= b;
            } else {
                booloptions &= ~b;
            }
        }

        public void Reset() 
        { 
            lvwindex = 0; 
            selectedchannel = 0; 
        }

        public void ChangeSelectedChannel()
        {
            const System.Byte MAXCHANNELS = 2;
            selectedchannel++;
            if (selectedchannel >= MAXCHANNELS)
            {
                selectedchannel = 0;
            }
        }

        public MainMenuButtonRegistrationInfo[] AdditionalPlaylistModeRightClickButtons
        {
            get => rightclickbuttondata;
            set => rightclickbuttondata = value;
        }

        public MainMenuButtonRegistrationInfo[] AdditionalMainMenuButtons
        {
            get => mainmenubuttondata;
            set => mainmenubuttondata = value;
        }

        public System.Boolean DDFWindowIsActive
        {
            get => HasBooleanOption(Booleans.DDFMon);
            set => SetBooleanOption(Booleans.DDFMon , value);
        }

        public System.Int32 ListViewIndex 
        { 
            get => lvwindex; 
            set => lvwindex = value; 
        }

        public System.Int16 OptionsPanelIndex 
        { 
            get => optpanelidx; 
            set => optpanelidx = value; 
        }

        public System.Byte SelectedChannel => selectedchannel;

        public System.Boolean Enabled 
        { 
            get => HasBooleanOption(Booleans.Enabled);
            set => SetBooleanOption(Booleans.Enabled, value); 
        }

        public System.Boolean AssertClosing 
        { 
            get => HasBooleanOption(Booleans.ClosingAssert); 
            set => SetBooleanOption(Booleans.ClosingAssert, value); 
        }

        public System.Boolean OptionsPanelMode 
        { 
            get => HasBooleanOption(Booleans.OptPanel); 
            set => SetBooleanOption(Booleans.OptPanel, value); 
        }

        public System.Boolean UIShutdown
        {
            get => HasBooleanOption(Booleans.UIShutdown);
            set => SetBooleanOption(Booleans.UIShutdown, value);
        }

        public System.Boolean HardFailureOccured
        {
            get => HasBooleanOption(Booleans.HardFailureRecorded);
            set => SetBooleanOption(Booleans.HardFailureRecorded , value);
        }
    }
}