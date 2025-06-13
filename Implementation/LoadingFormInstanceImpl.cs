using System.Windows.Forms;

namespace MP
{
    public sealed class LoadingFormInstanceImpl : Dialogs.StartupLoadingDialogBase
    {
        public LoadingFormInstanceImpl() 
        {
            NextAnimationDelay = 210;
        }

        protected override Form GetFormInstance()
        {
            LoadingForm lf = new();
            AnimationCode = lf.ThreadCode;
            return lf;
        }
    }
}