//*****************************************************************************
//
//  File:       Ribbon.cs
//
//  Contents:   Class which is used as a façade for the Windows Ribbon 
//              Framework. In charge of initialization and communication with 
//              the Windows Ribbon Framework.
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using RibbonLib.Interop;
using System.Resources;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using MethodInvoker = System.Windows.Forms.MethodInvoker;
using RibbonLib.Controls;

namespace RibbonLib
{
    /// <summary>
    /// Main class for using the windows ribbon in a .NET application
    /// </summary>
    [ToolboxBitmap(typeof(Ribbon), "Ribbon16.bmp")]
    public class Ribbon : Control, IUICommandHandler
    {
        private const string uriFile = "file://";

        private IUIImageFromBitmap _imageFromBitmap;
        private UIImage _uiImage;
        private RibbonUIApplication _application;
        private Dictionary<uint, BaseRibbonControl> _mapRibbonControls = new Dictionary<uint, BaseRibbonControl>();
        private IntPtr _loadedDllHandle = IntPtr.Zero;

        private const string DefaultResourceName = "APPLICATION_RIBBON";
        private const string ResNameExtension = "_RIBBON";

        private RibbonShortcutTable _ribbonShortcutTable;

        private static readonly object EventRibbonEventException = new object();
        private static readonly object EventViewCreated = new object();
        private static readonly object EventViewDestroy = new object();
        private static readonly object EventRibbonHeight = new object();

        //@ Size for designer
        /// <summary>
        /// The default height is 147, but here we have to use default height - Top Non Client Area (31)
        /// </summary>
        protected override Size DefaultSize => new Size(base.DefaultSize.Width, 116);

        /// <summary>
        /// Get EventLogger object which implements IUIEventLogger.
        /// Only available in Windows 8, 10. Can be null.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventLogger EventLogger { get; private set; }

        private string _shortcutTableResourceName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="specified"></param>
        /// <inheritdoc cref="SetBoundsCore"/>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (Util.DesignMode)
                height = DefaultSize.Height;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        /// is a reference to an embedded resource file
        /// in the application assembly. The (xml)-file contains
        /// shortcut keys.
        /// </summary>
        [Description("The embedded resource (xml)-file contains shortcut keys.")]
        public string ShortcutTableResourceName
        {
            get { return _shortcutTableResourceName; }
            set
            {
                _shortcutTableResourceName = value;
                CheckInitialize();
            }
        }

        void TryCreateShortcutTable(Assembly assembly)
        {
            _ribbonShortcutTable = null;

            if (string.IsNullOrEmpty(this.ShortcutTableResourceName))
                return;

            _ribbonShortcutTable = Util.DeserializeEmbeddedResource<RibbonShortcutTable>(
                this.ShortcutTableResourceName, assembly);
            if (_ribbonShortcutTable != null)
            {
                var form = this.FindForm();
                form.KeyPreview = true;
                form.KeyUp += new KeyEventHandler(Form_KeyUp);
            }
            else
            {
                throw new ArgumentException(string.Format("Embedded resource not found '{0}'", nameof(ShortcutTableResourceName)));
            }
        }

        void Form_KeyUp(object sender, KeyEventArgs e)
        {
            var commandId = _ribbonShortcutTable.HitTest(e.KeyData);
            if (commandId == 0)
                return;

            ((IUICommandHandler)this).Execute(commandId, ExecutionVerb.Execute, null, null, null);

            e.SuppressKeyPress = false;
            e.Handled = true;
        }

        /// <summary>
        /// Initializes a new instance of the Ribbon
        /// </summary>
        public Ribbon()
        {
            base.Dock = DockStyle.Top;

            if (Util.DesignMode)
                return;

            this.SetStyle(ControlStyles.UserPaint, false);
            this.SetStyle(ControlStyles.Opaque, true);

            this.ParentChanged += new EventHandler(Ribbon_ParentChanged);
            this.HandleCreated += new EventHandler(RibbonControl_HandleCreated);
            this.HandleDestroyed += new EventHandler(Ribbon_HandleDestroyed);
        }

        #region Form Windows State change bug workaround

        Form _form;
        FormWindowState _previousWindowState;
        int _previousNormalHeight;
        int _preserveHeight;

        void Ribbon_ParentChanged(object sender, EventArgs e)
        {
            var parent = this.Parent;
            if (parent == null)
            {
                RegisterForm(null);
                return;
            }
            var form = parent as Form;
            if (form == null)
                throw new ApplicationException("Parent of Ribbon does not derive from Form class.");

            RegisterForm(form);
        }

        void RegisterForm(Form form)
        {
            if (_form != null)
                _form.SizeChanged -= new EventHandler(_form_SizeChanged);

            _form = form;

            if (_form == null)
                return;

            _form.SizeChanged += new EventHandler(_form_SizeChanged);
        }

        void _form_SizeChanged(object sender, EventArgs e)
        {
            if (_previousWindowState != FormWindowState.Normal
                && _form.WindowState == FormWindowState.Normal
                && _previousNormalHeight != 0)
            {
                _preserveHeight = _previousNormalHeight;
                _form.BeginInvoke(new MethodInvoker(RestoreHeight));
            }

            if (_form.WindowState == FormWindowState.Normal)
                _previousNormalHeight = _form.Height;
            _previousWindowState = _form.WindowState;
        }

        void RestoreHeight()
        {
            _form.Height = _preserveHeight;
        }
        #endregion

        void Ribbon_HandleDestroyed(object sender, EventArgs e)
        {
            DestroyFramework();
        }

        void RibbonControl_HandleCreated(object sender, EventArgs e)
        {
            CheckInitialize();
        }

        /// <summary>
        /// only Dock.Top possible
        /// </summary>
        [DefaultValue(typeof(DockStyle), "Top")]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
            }
        }

        /// <summary>
        /// Don't use
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// This is the Name parameter used for the UICC Compiler
        /// Default value is APPLICATION or leave it empty.
        /// </summary>
        [Description("This is the Name parameter used for the UICC Compiler. Default value is APPLICATION or leave it empty.")]
        public string ResourceIdentifier
        {
            get;
            set;
        }

        string _resourceName;

        /// <summary>
        /// is a reference to an embedded resource file
        /// in the application assembly. The RibbonMarkup.ribbon file.
        /// </summary>
        [Description("Is a reference to an embedded resource file in the application assembly. The RibbonMarkup.ribbon file.")]
        public string ResourceName
        {
            get { return _resourceName; }
            set
            {
                _resourceName = value;
                CheckInitialize();
            }
        }

        /// <summary>
        /// Don't use
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IntPtr WindowHandle
        {
            get
            {
                var form = this.Parent as Form;
                return form.Handle;
            }
        }

        void CheckInitialize()
        {
            if (Util.DesignMode)
                return;

            if (Initialized)
                return;

            if (string.IsNullOrEmpty(ResourceName))
                throw new ApplicationException(string.Format("'{0}' not set", nameof(ResourceName)));
            //return;

            var form = this.Parent as Form;
            if (form == null)
                return;

            if (!form.IsHandleCreated)
                return;

            var assembly = form.GetType().Assembly;
            InitFramework(this.ResourceName, assembly);
            TryCreateShortcutTable(assembly);
        }

        /// <summary>
        /// Draws only in Design mode
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            ControlPaint.DrawContainerGrabHandle(e.Graphics, this.ClientRectangle);
        }

        /// <summary>
        /// Check if ribbon framework has been initialized
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Initialized
        {
            get
            {
                return (Framework != null);
            }
        }

        /// <summary>
        /// Get ribbon framework object
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IUIFramework Framework { get; private set; }

        string _tempDllFilename;

        /// <summary>
        /// Embedded resource based Ribbon Dll
        /// </summary>
        byte[] GetLocalizedRibbon(string ribbonResource, Assembly ribbonAssembly)
        {
            byte[] data = null;
            bool found = false;

            // try to get from current current culture satellite assembly first
            var culture = Thread.CurrentThread.CurrentUICulture;
            Assembly satelliteAssembly = null;
            TryGetSatelliteAssembly(culture, ribbonAssembly, ref satelliteAssembly);

            if (satelliteAssembly != null)
            {
                found = TryGetRibbon(ribbonResource, satelliteAssembly, ref data);
                if (found)
                    return data;
            }

            // try to get from current current culture fallback satellite assembly
            Assembly fallbackAssembly = null;
            if (culture.Parent != null)
                TryGetSatelliteAssembly(culture.Parent, ribbonAssembly, ref fallbackAssembly);

            if (fallbackAssembly != null)
            {
                found = TryGetRibbon(ribbonResource, fallbackAssembly, ref data);
                if (found)
                    return data;
            }

            // try to get from current current culture fallback satellite assembly
            found = TryGetRibbon(ribbonResource, ribbonAssembly, ref data);
            if (!found)
                throw new ArgumentException(string.Format("ResourceName resource '{0}' not found in assembly '{1}'.", ribbonResource, ribbonAssembly.Location));

            return data;
        }

        bool TryGetSatelliteAssembly(CultureInfo culture, Assembly mainAssembly, ref Assembly satelliteAssembly)
        {
            try
            {
                satelliteAssembly = mainAssembly.GetSatelliteAssembly(culture);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool TryGetRibbon(string ribbonResource, Assembly assembly, ref byte[] data)
        {
            try
            {
                var buffer = Util.GetEmbeddedResource(ribbonResource, assembly);
                data = buffer;
                if (buffer == null)
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// File based Ribbon Dll method
        /// </summary>
        private byte[] GetLocalizedRibbonFileData(string ribbonResource)
        {
            string path = null;
            string localizedPath = String.Empty;
            string fileName;
            string directoryName = null;
            var culture = Thread.CurrentThread.CurrentUICulture;
            try
            {
                int start = ribbonResource.IndexOf('{'); //Mark for Special Folder like LocalApplicationData
                int last = -1;
                if (start > 0)
                {
                    last = ribbonResource.IndexOf('}');
                    if (last < start)
                    {
                        throw new ArgumentException("} not found in " + nameof(ResourceName));
                    }
                    string specialFolder = ribbonResource.Substring(start + 1, last - start - 1);
                    Environment.SpecialFolder enumSpecial = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), specialFolder);
                    string specialFolderPath = Environment.GetFolderPath(enumSpecial);
                    path = ribbonResource.Substring(0, start) + specialFolderPath + ribbonResource.Substring(last + 1);
                }
                else
                {
                    path = uriFile + Path.GetFullPath(ribbonResource.Substring(uriFile.Length));
                }
                path = new Uri(path).LocalPath;
                localizedPath = path;
                if (File.Exists(path))
                {
                    fileName = Path.GetFileName(path);
                    directoryName = Path.GetDirectoryName(path);
                    string cultureName = culture.Name;
                    string localPath;
                    if ((localPath = TryGetCultureFile(directoryName, fileName, cultureName, true)) == null)
                    {
                        if ((localPath = TryGetCultureFile(directoryName, fileName, cultureName, false)) == null)
                        {
                            if (culture.Parent != null)
                            {
                                cultureName = culture.Parent.Name;
                                if ((localPath = TryGetCultureFile(directoryName, fileName, cultureName, true)) == null)
                                {
                                    localPath = TryGetCultureFile(directoryName, fileName, cultureName, false);
                                }
                            }
                        }
                    }
                    if (localPath != null)
                    {
                        localizedPath = localPath;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Ribbon ResourceName is invalid", ex);
            }
            if (String.IsNullOrEmpty(localizedPath))
                return null;
            else
                return File.ReadAllBytes(localizedPath);
        }

        private static string TryGetCultureFile(string directoryName, string fileName, string cultureName, bool root)
        {
            string localizedPath = null;
            string localizedDirectory;
            if (root)
                localizedDirectory = directoryName;
            else
                localizedDirectory = Path.Combine(directoryName, cultureName);

            if (Directory.Exists(localizedDirectory))
            {
                string tmpPath = Path.Combine(localizedDirectory, fileName);
                if (File.Exists(tmpPath) && !root)
                {
                    localizedPath = tmpPath;
                }
                else
                {
                    tmpPath = Path.Combine(localizedDirectory, Path.GetFileNameWithoutExtension(fileName) + "." + cultureName + Path.GetExtension(fileName));
                    if (File.Exists(tmpPath))
                    {
                        localizedPath = tmpPath;
                    }
                }
            }
            return localizedPath;
        }

        /// <summary>
        /// Initalize ribbon framework
        /// </summary>
        /// <param name="ribbonResource">Name of ribbon dll</param>
        /// <param name="ribbonAssembly">Assembly where ribbon should reside</param>
        void InitFramework(string ribbonResource, Assembly ribbonAssembly)
        {
            string path = Path.GetTempPath();
            _tempDllFilename = Path.Combine(path, Path.GetTempFileName());
            byte[] buffer = null;
            if (ribbonResource.ToLowerInvariant().StartsWith(uriFile))
            {
                buffer = GetLocalizedRibbonFileData(ribbonResource);
            }
            else
                buffer = GetLocalizedRibbon(ribbonResource, ribbonAssembly);

            File.WriteAllBytes(_tempDllFilename, buffer);

            // if ribbon dll exists, use it
            if (File.Exists(_tempDllFilename))
            {
                string resourceIdentifier;
                if (string.IsNullOrEmpty(ResourceIdentifier))
                    resourceIdentifier = DefaultResourceName;
                else
                    resourceIdentifier = ResourceIdentifier + ResNameExtension;
                // load ribbon from ribbon dll resource
                InitFramework(resourceIdentifier, _tempDllFilename);
            }
        }

        /// <summary>
        /// Initalize ribbon framework
        /// </summary>
        /// <param name="resourceName">Identifier of the ribbon resource</param>
        /// <param name="ribbonDllName">Dll name where to find ribbon resource</param>
        void InitFramework(string resourceName, string ribbonDllName)
        {
            // dynamically load ribbon library
            _loadedDllHandle = PInvoke.LoadLibraryEx(ribbonDllName, IntPtr.Zero,
                                                            LOAD_LIBRARY_FLAGS.DONT_RESOLVE_DLL_REFERENCES |
                                                            LOAD_LIBRARY_FLAGS.LOAD_IGNORE_CODE_AUTHZ_LEVEL |
                                                            LOAD_LIBRARY_FLAGS.LOAD_LIBRARY_AS_DATAFILE |
                                                            LOAD_LIBRARY_FLAGS.LOAD_LIBRARY_AS_IMAGE_RESOURCE);

            if (_loadedDllHandle == IntPtr.Zero)
            {
                FreeLibrary();
                throw new ApplicationException("Ribbon resource DLL exists but could not be loaded.");
            }
            uint imageSize = 0;
            IntPtr hrSRC = PInvoke.FindResource(_loadedDllHandle, resourceName, "UIFILE");
            if (hrSRC != IntPtr.Zero)
            {
                imageSize = PInvoke.SizeofResource(_loadedDllHandle, hrSRC);
            }
            if (imageSize == 0)
            {
                FreeLibrary();
                throw new ApplicationException("Ribbon resource DLL not valid, " + nameof(ResourceIdentifier) + "?");
            }

            InitFramework(resourceName, _loadedDllHandle);
        }

        private void FreeLibrary()
        {
            if (_loadedDllHandle != IntPtr.Zero)
            {
                // free dynamic library
                PInvoke.FreeLibrary(_loadedDllHandle);
                _loadedDllHandle = IntPtr.Zero;
            }
            if (!string.IsNullOrEmpty(_tempDllFilename))
            {
                try
                {
                    File.Delete(_tempDllFilename);
                    _tempDllFilename = null;
                }
                catch { }
            }
        }

        /// <summary>
        /// Initialize ribbon framework
        /// </summary>
        /// <param name="resourceName">Identifier of the ribbon resource</param>
        /// <param name="hInstance">Pointer to HINSTANCE of module where we can find ribbon resource</param>
        void InitFramework(string resourceName, IntPtr hInstance)
        {
            // create ribbon framework object
            Framework = CreateRibbonFramework();
            _imageFromBitmap = CreateImageFromBitmapFactory();
            _uiImage = new UIImage(_imageFromBitmap);

            // create ribbon application object
            _application = new RibbonUIApplication(this, this);

            // init ribbon framework
            HRESULT hr = Framework.Initialize(this.WindowHandle, _application);
            if (hr.Failed)
            {
                Marshal.ThrowExceptionForHR((int)hr);
            }

            // load ribbon ui
            hr = Framework.LoadUI(hInstance, resourceName);

            if (!(Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor <= 1 || Environment.OSVersion.Version.Major < 6))
            {
                IUIEventingManager eventingManager = Framework as IUIEventingManager;
                if (eventingManager != null)
                {
                    EventLogger = new EventLogger(eventingManager);
                }
            }

            if (hr.Failed)
            {
                Marshal.ThrowExceptionForHR((int)hr);
            }
        }

        /// <summary>
        /// Destroy ribbon framework
        /// </summary>
        void DestroyFramework()
        {

            if (Initialized)
            {
                if (EventLogger != null)
                    EventLogger.Destroy();
                // destroy ribbon framework
                Framework.Destroy();
                Marshal.ReleaseComObject(Framework);

                // remove reference to framework object
                Framework = null;
            }

            // Unregister event handlers
            RegisterForm(null);

            FreeLibrary();

            if (_imageFromBitmap != null)
            {
                _uiImage?.Dispose();
                // remove reference to imageFromBitmap object
                _imageFromBitmap = null;
            }

            if (_application != null)
            {
                // remove reference to application object
                _application = null;
            }

            // remove references to ribbon controls
            _mapRibbonControls.Clear();
        }

        /// <summary>
        /// Change ribbon background, highlight and text colors
        /// </summary>
        /// <param name="background">new background color</param>
        /// <param name="highlight">new highlight color</param>
        /// <param name="text">new text color</param>
        public void SetColors(Color background, Color highlight, Color text)
        {
            if (!Initialized)
            {
                return;
            }

            // convert colors to proper color format
            uint backgroundColor = ColorHelper.ColorToHSB(background);
            uint highlightColor = ColorHelper.ColorToHSB(highlight);
            uint textColor = ColorHelper.ColorToHSB(text);

            IPropertyStore propertyStore = (IPropertyStore)Framework;

            PropVariant backgroundColorProp = PropVariant.FromObject(backgroundColor);
            PropVariant highlightColorProp = PropVariant.FromObject(highlightColor);
            PropVariant textColorProp = PropVariant.FromObject(textColor);

            // set ribbon colors
            propertyStore.SetValue(ref RibbonProperties.GlobalBackgroundColor, ref backgroundColorProp);
            propertyStore.SetValue(ref RibbonProperties.GlobalHighlightColor, ref highlightColorProp);
            propertyStore.SetValue(ref RibbonProperties.GlobalTextColor, ref textColorProp);

            propertyStore.Commit();
        }

        /// <summary>
        /// Change ribbon background color
        /// </summary>
        /// <param name="background">new background color</param>
        public void SetBackgroundColor(Color background)
        {
            if (!Initialized)
            {
                return;
            }

            // convert color to proper color format
            uint color = ColorHelper.ColorToHSB(background);

            IPropertyStore propertyStore = (IPropertyStore)Framework;

            PropVariant colorProp = PropVariant.FromObject(color);

            // set ribbon color
            propertyStore.SetValue(ref RibbonProperties.GlobalBackgroundColor, ref colorProp);

            propertyStore.Commit();
        }

        /// <summary>
        /// Change ribbon highlight color
        /// </summary>
        /// <param name="highlight">new highlight color</param>
        public void SetHighlightColor(Color highlight)
        {
            if (!Initialized)
            {
                return;
            }

            // convert color to proper color format
            uint color = ColorHelper.ColorToHSB(highlight);

            IPropertyStore propertyStore = (IPropertyStore)Framework;

            PropVariant colorProp = PropVariant.FromObject(color);

            // set ribbon color
            propertyStore.SetValue(ref RibbonProperties.GlobalHighlightColor, ref colorProp);

            propertyStore.Commit();
        }

        /// <summary>
        /// Change ribbon text color
        /// </summary>
        /// <param name="text">new text color</param>
        public void SetTextColor(Color text)
        {
            if (!Initialized)
            {
                return;
            }

            // convert color to proper color format
            uint color = ColorHelper.ColorToHSB(text);

            IPropertyStore propertyStore = (IPropertyStore)Framework;

            PropVariant colorProp = PropVariant.FromObject(color);

            // set ribbon color
            propertyStore.SetValue(ref RibbonProperties.GlobalTextColor, ref colorProp);

            propertyStore.Commit();
        }

        /// <summary>
        /// Get the three Colors of the Ribbon
        /// </summary>
        /// <returns>Ribbon Colors class or null</returns>
        public RibbonColors GetColors()
        {
            RibbonColors colors = null;
            if (!Initialized)
            {
                return colors;
            }
            IPropertyStore propertyStore = (IPropertyStore)this.Framework;
            PropVariant backgroundColorProp;
            PropVariant highlightColorProp;
            PropVariant textColorProp;

            // get ribbon colors
            propertyStore.GetValue(ref RibbonProperties.GlobalBackgroundColor, out backgroundColorProp);
            propertyStore.GetValue(ref RibbonProperties.GlobalHighlightColor, out highlightColorProp);
            propertyStore.GetValue(ref RibbonProperties.GlobalTextColor, out textColorProp);
            uint background = (uint)backgroundColorProp.Value;
            uint highlight = (uint)highlightColorProp.Value;
            uint text = (uint)textColorProp.Value;
            colors = new RibbonColors(ColorHelper.HSBtoColor(background), ColorHelper.HSBtoColor(highlight), ColorHelper.HSBtoColor(text));
            return colors;
        }

        /// <summary>
        /// Wraps a Bitmap object with IUIImage interface
        /// </summary>
        /// <param name="bitmap">Bitmap object to wrap</param>
        /// <returns>IUIImage wrapper</returns>
        public IUIImage ConvertToUIImage(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            if (_imageFromBitmap == null)
            {
                return null;
            }

            IUIImage uiImage;
            _imageFromBitmap.CreateImage(bitmap.GetHbitmap(), Ownership.Transfer, out uiImage);

            return uiImage;
        }

        /// <summary>
        /// Set current application modes
        /// </summary>
        /// <param name="modesArray">array of modes to set</param>
        /// <remarks>Unlisted modes will be unset</remarks>
        public void SetModes(params byte[] modesArray)
        {
            if (modesArray == null || modesArray.Length == 0)
                throw new ArgumentNullException(nameof(modesArray));
            // check that ribbon is initialized
            if (!Initialized)
            {
                return;
            }

            // calculate compact modes value
            int compactModes = 0;
            for (int i = 0; i < modesArray.Length; ++i)
            {
                if (modesArray[i] >= 32)
                {
                    throw new ArgumentException("Modes should range between 0 to 31.");
                }

                compactModes |= (1 << modesArray[i]);
            }

            // set modes
            Framework.SetModes(compactModes);
        }

        /// <summary>
        /// Shows a predefined context popup in a specific location
        /// </summary>
        /// <param name="contextPopupID">commandId for the context popup</param>
        /// <param name="x">X in screen coordinates</param>
        /// <param name="y">Y in screen coordinates</param>
        public void ShowContextPopup(uint contextPopupID, int x, int y)
        {
            // check that ribbon is initialized
            if (!Initialized)
            {
                return;
            }

            object contextualUIObject;
            Guid contextualUIGuid = new Guid(RibbonIIDGuid.IUIContextualUI);
            HRESULT hr = Framework.GetView(contextPopupID, ref contextualUIGuid, out contextualUIObject);
            if (hr.Succeeded)
            {
                IUIContextualUI contextualUI = contextualUIObject as IUIContextualUI;
                contextualUI.ShowAtLocation(x, y);
                Marshal.ReleaseComObject(contextualUI);
            }
            else
            {
                Marshal.ThrowExceptionForHR((int)hr);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ApplicationButtonColor
        {
            get
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return default(Color);
                }

                IPropertyStore propertyStore = (IPropertyStore)this.Framework;
                HRESULT hr = propertyStore.GetValue(RibbonProperties.ApplicationButtonColor, out PropVariant propVariant);
                if (hr.Succeeded)
                {
                    uint result = (uint)propVariant.Value;
                    Color color = ColorHelper.HSBtoColor(result);
                    return color;
                }
                return default(Color);
            }
            set
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return;
                }
                uint hsb = ColorHelper.ColorToHSB(value);
                IPropertyStore propertyStore = (IPropertyStore)this.Framework;
                PropVariant propVariant = PropVariant.FromObject(hsb);
                HRESULT hr = propertyStore.SetValue(RibbonProperties.ApplicationButtonColor, propVariant);
                if (hr.Succeeded)
                    hr = propertyStore.Commit();
            }
        }

        /// <summary>
        /// Get or Set the DarkModeRibbon PropertyKey
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DarkModeRibbon
        {
            get
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return false;
                }

                IPropertyStore propertyStore = (IPropertyStore)this.Framework;
                HRESULT hr = propertyStore.GetValue(RibbonProperties.DarkModeRibbon, out PropVariant propDarkMode);
                if (hr.Succeeded)
                {
                    bool result = (bool)propDarkMode.Value;
                    return result;
                }
                return false;
            }
            set
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return;
                }

                IPropertyStore propertyStore = (IPropertyStore)this.Framework;
                PropVariant propDarkMode = PropVariant.FromObject(value);
                HRESULT hr = propertyStore.SetValue(RibbonProperties.DarkModeRibbon, propDarkMode);
                if (hr.Succeeded)
                    hr = propertyStore.Commit();
            }
        }

        /// <summary>
        /// Specifies whether the ribbon is in a collapsed or expanded state
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Minimized
        {
            get
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return false;
                }

                IPropertyStore propertyStore = _application.UIRibbon as IPropertyStore;
                PropVariant propMinimized;
                HRESULT hr = propertyStore.GetValue(ref RibbonProperties.Minimized, out propMinimized);
                return (bool)propMinimized.Value;
            }
            set
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return;
                }

                IPropertyStore propertyStore = _application.UIRibbon as IPropertyStore;
                PropVariant propMinimized = PropVariant.FromObject(value);
                HRESULT hr = propertyStore.SetValue(ref RibbonProperties.Minimized, ref propMinimized);
                hr = propertyStore.Commit();
            }
        }

        /// <summary>
        /// Specifies whether the ribbon user interface (UI) is in a visible or hidden state
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Viewable
        {
            get
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return false;
                }

                IPropertyStore propertyStore = _application.UIRibbon as IPropertyStore;
                PropVariant propViewable;
                HRESULT hr = propertyStore.GetValue(ref RibbonProperties.Viewable, out propViewable);
                return (bool)propViewable.Value;
            }
            set
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return;
                }

                IPropertyStore propertyStore = _application.UIRibbon as IPropertyStore;
                PropVariant propViewable = PropVariant.FromObject(value);
                HRESULT hr = propertyStore.SetValue(ref RibbonProperties.Viewable, ref propViewable);
                hr = propertyStore.Commit();
            }
        }

        /// <summary>
        /// Specifies whether the quick access toolbar is docked at the top or at the bottom
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ControlDock QuickAccessToolbarDock
        {
            get
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return ControlDock.Top;
                }

                IPropertyStore propertyStore = _application.UIRibbon as IPropertyStore;
                PropVariant propQuickAccessToolbarDock;
                HRESULT hr = propertyStore.GetValue(ref RibbonProperties.QuickAccessToolbarDock, out propQuickAccessToolbarDock);
                return (ControlDock)(uint)propQuickAccessToolbarDock.Value;
            }
            set
            {
                // check that ribbon is initialized
                if (!Initialized)
                {
                    return;
                }

                IPropertyStore propertyStore = _application.UIRibbon as IPropertyStore;
                PropVariant propQuickAccessToolbarDock = PropVariant.FromObject((uint)value);
                HRESULT hr = propertyStore.SetValue(ref RibbonProperties.QuickAccessToolbarDock, ref propQuickAccessToolbarDock);
                hr = propertyStore.Commit();
            }
        }

        /// <summary>
        /// The SaveSettingsToStream method is useful for persisting ribbon state, such as Quick Access Toolbar (QAT) items, across application instances.
        /// </summary>
        /// <param name="stream"></param>
        public void SaveSettingsToStream(Stream stream)
        {
            if (!Initialized)
            {
                return;
            }

            StreamAdapter streamAdapter = new StreamAdapter(stream);
            HRESULT hr = _application.UIRibbon.SaveSettingsToStream(streamAdapter);
        }

        /// <summary>
        /// The LoadSettingsFromStream method is useful for persisting ribbon state, such as Quick Access Toolbar (QAT) items, across application instances.
        /// </summary>
        /// <param name="stream"></param>
        public void LoadSettingsFromStream(Stream stream)
        {
            if (!Initialized)
            {
                return;
            }

            StreamAdapter streamAdapter = new StreamAdapter(stream);
            HRESULT hr = _application.UIRibbon.LoadSettingsFromStream(streamAdapter);
        }

        /// <summary>
        /// Create ribbon framework object
        /// </summary>
        /// <returns>ribbon framework object</returns>
        private static IUIFramework CreateRibbonFramework()
        {
            try
            {
                return new UIRibbonFramework() as IUIFramework;
            }
            catch (COMException exception)
            {
                throw new PlatformNotSupportedException("The ribbon framework couldn't be found on this system.", exception);
            }
        }

        /// <summary>
        /// Create image-from-bitmap factory object
        /// </summary>
        /// <returns>image-from-bitmap factory object</returns>
        private static IUIImageFromBitmap CreateImageFromBitmapFactory()
        {
            return new UIRibbonImageFromBitmapFactory() as IUIImageFromBitmap;
        }

        /// <summary>
        /// Adds a ribbon control to the internal map
        /// </summary>
        /// <param name="ribbonControl">ribbon control to add</param>
        internal void AddRibbonControl(BaseRibbonControl ribbonControl)
        {
            _mapRibbonControls.Add(ribbonControl.CommandID, ribbonControl);
        }

        internal bool OnRibbonEventException(object sender, ThreadExceptionEventArgs args)
        {
            EventHandler<ThreadExceptionEventArgs> eh = Events[EventRibbonEventException] as EventHandler<ThreadExceptionEventArgs>;
            if (eh != null)
            {
                eh(sender, args);
                return true;
            }
            return false;
        }

        /// <summary>
        /// User can handle untrapped Exceptions in the other events of the Ribbon
        /// </summary>
        public event EventHandler<ThreadExceptionEventArgs> RibbonEventException
        {
            add
            {
                Events.AddHandler(EventRibbonEventException, value);
            }
            remove
            {
                Events.RemoveHandler(EventRibbonEventException, value);
            }
        }

        #region Implementation of IUICommandHandler

        /// <summary>
        /// Implementation of IUICommandHandler.Execute
        /// Responds to execute events on Commands bound to the Command handler
        /// </summary>
        /// <param name="commandID">the command that has been executed</param>
        /// <param name="verb">the mode of execution</param>
        /// <param name="key">the property that has changed</param>
        /// <param name="currentValue">the new value of the property that has changed</param>
        /// <param name="commandExecutionProperties">additional data for this execution</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise</returns>
        /// <remarks>This method is used internally by the Ribbon class and should not be called by the user.</remarks>
        HRESULT IUICommandHandler.Execute(uint commandID, ExecutionVerb verb, PropertyKeyRef key, PropVariantRef currentValue, IUISimplePropertySet commandExecutionProperties)
        {
#if DEBUG
            Debug.WriteLine(string.Format("Execute verb: {0} for command {1}", verb, commandID));
#endif

            if (TryGetRibbonControlById(commandID, out BaseRibbonControl control))
            {
                IRibbonControl item = control as IRibbonControl;
                return item.Execute(verb, key, currentValue, commandExecutionProperties);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        /// Implementation of IUICommandHandler.UpdateProperty
        /// Responds to property update requests from the Windows Ribbon (Ribbon) framework. 
        /// </summary>
        /// <param name="commandID">The ID for the Command, which is specified in the Markup resource file</param>
        /// <param name="key">The Property Key to update</param>
        /// <param name="currentValue">A pointer to the current value for key. This parameter can be null</param>
        /// <param name="newValue">When this method returns, contains a pointer to the new value for key</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise</returns>
        /// <remarks>This method is used internally by the Ribbon class and should not be called by the user.</remarks>
        HRESULT IUICommandHandler.UpdateProperty(uint commandID, ref PropertyKey key, PropVariantRef currentValue, ref PropVariant newValue)
        {
#if DEBUG
            Debug.WriteLine(string.Format("UpdateProperty key: {0} for command {1}", RibbonProperties.GetPropertyKeyName(ref key), commandID));
#endif

            if (TryGetRibbonControlById(commandID, out BaseRibbonControl control))
            {
                IRibbonControl item = control as IRibbonControl;
                return item.UpdateProperty(ref key, currentValue, ref newValue);
            }

            return HRESULT.S_OK;
        }

        #endregion

        /// <summary>
        /// Event fires when the View is created
        /// </summary>
        public event EventHandler ViewCreated
        {
            add
            {
                Events.AddHandler(EventViewCreated, value);
            }
            remove
            {
                Events.RemoveHandler(EventViewCreated, value);
            }
        }

        internal void OnViewCreated()
        {
            EventHandler eh = Events[EventViewCreated] as EventHandler;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event fires when the View is in destroy
        /// </summary>
        public event EventHandler ViewDestroy
        {
            add
            {
                Events.AddHandler(EventViewDestroy, value);
            }
            remove
            {
                Events.RemoveHandler(EventViewDestroy, value);
            }
        }

        internal void OnViewDestroy()
        {
            EventHandler eh = Events[EventViewDestroy] as EventHandler;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event fires when the Ribbon height changed
        /// </summary>
        public event EventHandler RibbonHeightChanged
        {
            add
            {
                Events.AddHandler(EventRibbonHeight, value);
            }
            remove
            {
                Events.RemoveHandler(EventRibbonHeight, value);
            }
        }

        internal void OnRibbonHeightChanged()
        {
            EventHandler eh = Events[EventRibbonHeight] as EventHandler;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns the Dll Handle for the culture specific RibbonMarkup.ribbon file.
        /// One can use this handle to get Strings, Bitmaps.
        /// </summary>
        /// <returns>The Dll Handle of the Ribbon resource</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IntPtr MarkupHandle
        {
            get
            {
                return _loadedDllHandle;
            }
        }

        /// <summary>
        /// Get the control by commandId
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns>IRibbonControl</returns>
        /// <exception cref="ArgumentException"></exception>
        public IRibbonControl GetRibbonControlById(uint commandId)
        {
            bool result = _mapRibbonControls.TryGetValue(commandId, out BaseRibbonControl item);
            if (result)
                return item;
            throw new ArgumentException("Not found", nameof(commandId));
        }

        internal bool TryGetRibbonControlById(uint commandId, out BaseRibbonControl item)
        {
            bool result = _mapRibbonControls.TryGetValue(commandId, out item);
            return result;
        }
    }
}
