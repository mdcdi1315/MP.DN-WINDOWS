
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace MP
{
    /// <summary>
    /// Using this control , the user can depict any color in it. Useful for color changing dialogs.
    /// </summary>
    [Docking(DockingBehavior.Ask)]
    [ToolboxItem(typeof(ColorBox))]
    [Designer(typeof(ColorBoxDesigner))]
    [DefaultProperty(nameof(DepictedColor))]
    [DefaultEvent(nameof(DepictedColorChanged))]
    [ToolboxItemFilter("Color", ToolboxItemFilterType.Require)]
    [System.Runtime.Versioning.SupportedOSPlatform("windows6.1")] // Windows 7
    [Description("Represents a small box that is painted given it's defined color.")]
    public partial class ColorBox : UserControl
    {
        private Color depcolor , fallbackcolor;

        private static void NullEventHandler(System.Object sender, Color e) { }

        /// <summary>
        /// Initialises a new ColorBox instance. Mostly used by the Windows Forms Designer.
        /// </summary>
        public ColorBox() : base()
        {
            fallbackcolor = Color.Black;
            depcolor = default;
            DepictedColorChanged = new(NullEventHandler);
            FallbackColorChanged = new(NullEventHandler);
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            LocationChanged += OnLocationChanged;
        }

        public ColorBox(Color color) : this()
        {
            depcolor = color;
        }

        public ColorBox(Color color , Color fallbackcolor) : this()
        {
            depcolor = color; 
            this.fallbackcolor = fallbackcolor;
        }

        private void OnSizeChanged(System.Object send , EventArgs e) => UpdateColor();

        private void OnLocationChanged(System.Object send , EventArgs e) => UpdateColor();

        /// <summary>
        /// Updates the color on the control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public void UpdateColor()
        {
            Graphics g = CreateGraphics();
            if (depcolor.IsEmpty)
            {
                g.Clear(fallbackcolor);
            } else
            {
                g.Clear(depcolor);
            }
        }

        /// <summary>
        /// Updates the fallback color on the control , if that is possble to happen.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public void UpdateFallbackColor()
        { 
            if (depcolor.IsEmpty)
            {
                CreateGraphics().Clear(fallbackcolor);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            UpdateColor();
            base.OnPaint(e);
        }

        /// <inheritdoc cref="Control.Invalidate()"/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new void Invalidate()
        {
            UpdateColor();
            base.Invalidate();
        }

        /// <summary>
        /// The color to show on the control.
        /// </summary>
        [Description("The color to show on the control.")]
        [Browsable(true)]
        [Category("General")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public System.Drawing.Color DepictedColor { get => depcolor; set { depcolor = value; DepictedColorChanged.Invoke(this, depcolor); UpdateColor(); } }

        /// <summary>
        /// The color that , if the <see cref="DepictedColor"/> property gets a value of <see cref="Color.Empty"/> , 
        /// the control will consult this value for showing a color. By default , it is set to <see cref="Color.Black"/>.
        /// </summary>
        [Description("The color that , if the DepictedColor property gets a value of System.Drawing.Color.Empty , " +
            "the control will consult this value for showing a color. By default , it is set to System.Drawing.Color.Black ." + 
            "Please be noted that the fallback color is not updated so much frequently , but you can explicitly update it " +
            "(If possible) using the UpdateFallbackColor method.")]
        [Browsable(true)]
        [Category("General")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public System.Drawing.Color FallbackColor { get => fallbackcolor; set { fallbackcolor = value; FallbackColorChanged.Invoke(this , fallbackcolor); } }

        /// <summary>Fired up when the <see cref="DepictedColor"/> property value has changed.</summary>
        [Description("The event that , when the depicted color is changed , is fired up.")]
        [Browsable(true)]
        [Category("Behavior")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event EventHandler<Color> DepictedColorChanged;

        /// <summary>Fired up when the <see cref="FallbackColor"/> property value has changed.</summary>
        [Description("The event that , when the fallback color is changed , is fired up.")]
        [Browsable(true)]
        [Category("Behavior")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event EventHandler<Color> FallbackColorChanged;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ColorBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ColorBox";
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SizeChanged -= OnSizeChanged;
                LocationChanged -= OnLocationChanged;
            }
            base.Dispose(disposing);
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.19041.0")]
    public class ColorBoxDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        private ColorBox cb;
        private Graphics designgraph;
        
        public ColorBoxDesigner() : base() { 
            cb = new();
            cb.ForeColor = Color.Aqua;
            cb.BackColor = Color.Black;
            cb.Size = new(10, 10);
            base.Initialize(cb);
            designgraph = Graphics.FromHwndInternal(cb.Handle);
            cb.DepictedColorChanged += Cb_DepictedColorChanged;
        }

        private void Cb_DepictedColorChanged(object sender, Color e)
        {
            designgraph?.DrawRectangle(new(e) , cb.Bounds);
        }

        public new void Dispose()
        {
            cb.Dispose();
            Dispose(true);
        }

        protected override void OnPaintAdornments(PaintEventArgs e)
        {
            if (cb.DepictedColor != System.Drawing.Color.Empty)
            {
                e.Graphics.Clear(cb.DepictedColor);
            }
            base.OnPaintAdornments(e);
        }

        protected override void Dispose(System.Boolean all)
        {
            cb.Dispose();
            base.Dispose(all);
        }
    }

}
