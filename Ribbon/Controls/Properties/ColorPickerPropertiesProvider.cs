//*****************************************************************************
//
//  File:       ColorPickerPropertiesProvider.cs
//
//  Contents:   Definition for color picker properties provider 
//
//*****************************************************************************

using System;
using System.Drawing;
using RibbonLib.Interop;

namespace RibbonLib.Controls.Properties
{
    /// <summary>
    /// Definition for color picker properties provider interface
    /// </summary>
    public interface IColorPickerPropertiesProvider
    {
        /// <summary>
        /// Automatic color label property
        /// </summary>
        string AutomaticColorLabel { get; set; }

        /// <summary>
        /// Color property
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Color type property
        /// </summary>
        SwatchColorType ColorType { get; set; }

        /// <summary>
        /// More colors label property
        /// </summary>
        string MoreColorsLabel { get; set; }

        /// <summary>
        /// No color label property
        /// </summary>
        string NoColorLabel { get; set; }

        /// <summary>
        /// Recent colors category label property
        /// </summary>
        string RecentColorsCategoryLabel { get; set; }

        /// <summary>
        /// Standard colors property
        /// </summary>
        Color[] StandardColors { get; set; }

        /// <summary>
        /// Standard colors category label property
        /// </summary>
        string StandardColorsCategoryLabel { get; set; }

        /// <summary>
        /// Standard colors tooltips property
        /// </summary>
        string[] StandardColorsTooltips { get; set; }

        /// <summary>
        /// Theme colors property
        /// </summary>
        Color[] ThemeColors { get; set; }

        /// <summary>
        /// Theme colors category label property
        /// </summary>
        string ThemeColorsCategoryLabel { get; set; }

        /// <summary>
        /// Theme colors tooltips property
        /// </summary>
        string[] ThemeColorsTooltips { get; set; }
    }

    /// <summary>
    /// Implementation of IColorPickerPropertiesProvider
    /// </summary>
    public class ColorPickerPropertiesProvider : BasePropertiesProvider, IColorPickerPropertiesProvider
    {
        /// <summary>
        /// ColorPickerPropertiesProvider ctor
        /// </summary>
        /// <param name="ribbon">parent ribbon</param>
        /// <param name="commandId">ribbon control command id</param>
        public ColorPickerPropertiesProvider(Ribbon ribbon, uint commandId)
            : base(ribbon, commandId)
        {
            // add supported properties
            _supportedProperties.Add(RibbonProperties.AutomaticColorLabel);
            _supportedProperties.Add(RibbonProperties.Color);
            _supportedProperties.Add(RibbonProperties.ColorType);
            _supportedProperties.Add(RibbonProperties.MoreColorsLabel);
            _supportedProperties.Add(RibbonProperties.NoColorLabel);
            _supportedProperties.Add(RibbonProperties.RecentColorsCategoryLabel);
            _supportedProperties.Add(RibbonProperties.StandardColors);
            _supportedProperties.Add(RibbonProperties.StandardColorsCategoryLabel);
            _supportedProperties.Add(RibbonProperties.StandardColorsTooltips);
            _supportedProperties.Add(RibbonProperties.ThemeColors);
            _supportedProperties.Add(RibbonProperties.ThemeColorsCategoryLabel);
            _supportedProperties.Add(RibbonProperties.ThemeColorsTooltips);
        }

        private string _automaticColorLabel;
        private Color? _color;
        private SwatchColorType? _colorType;
        private string _moreColorsLabel;
        private string _noColorLabel;
        private string _recentColorsCategoryLabel;
        private Color[] _standardColors;
        private string _standardColorsCategoryLabel;
        private string[] _standardColorsTooltips;
        private Color[] _themeColors;
        private string _themeColorsCategoryLabel;
        private string[] _themeColorsTooltips;

        /// <summary>
        /// Handles IUICommandHandler.UpdateProperty function for the supported properties
        /// </summary>
        /// <param name="key">The Property Key to update</param>
        /// <param name="currentValue">A pointer to the current value for key. This parameter can be null</param>
        /// <param name="newValue">When this method returns, contains a pointer to the new value for key</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise</returns>
        public override HRESULT UpdateProperty(ref PropertyKey key, PropVariantRef currentValue, ref PropVariant newValue)
        {
            if (key == RibbonProperties.AutomaticColorLabel)
            {
                if (_automaticColorLabel != null)
                {
                    newValue.SetString(_automaticColorLabel);
                }
            }
            else if (key == RibbonProperties.Color)
            {
                if (_color.HasValue)
                {
                    newValue.SetUInt((uint)ColorTranslator.ToWin32(_color.Value));
                }
            }
            else if (key == RibbonProperties.ColorType)
            {
                if (_colorType.HasValue)
                {
                    newValue.SetUInt((uint)_colorType.Value);
                }
            }
            else if (key == RibbonProperties.MoreColorsLabel)
            {
                if (_moreColorsLabel != null)
                {
                    newValue.SetString(_moreColorsLabel);
                }
            }
            else if (key == RibbonProperties.NoColorLabel)
            {
                if (_noColorLabel != null)
                {
                    newValue.SetString(_noColorLabel);
                }
            }
            else if (key == RibbonProperties.RecentColorsCategoryLabel)
            {
                if (_recentColorsCategoryLabel != null)
                {
                    newValue.SetString(_recentColorsCategoryLabel);
                }
            }
            else if (key == RibbonProperties.StandardColors)
            {
                if (_standardColors != null)
                {
                    int[] intStandardColors = Array.ConvertAll<Color, int>(_standardColors, new Converter<Color, int>(ColorTranslator.ToWin32));
                    uint[] uintStandardColors = Array.ConvertAll<int, uint>(intStandardColors, new Converter<int, uint>(Convert.ToUInt32));
                    newValue.SetUIntVector(uintStandardColors);
                }
            }
            else if (key == RibbonProperties.StandardColorsCategoryLabel)
            {
                if (_standardColorsCategoryLabel != null)
                {
                    newValue.SetString(_standardColorsCategoryLabel);
                }
            }
            else if (key == RibbonProperties.StandardColorsTooltips)
            {
                if (_standardColorsTooltips != null)
                {
                    newValue.SetStringVector(_standardColorsTooltips);
                }
            }
            else if (key == RibbonProperties.ThemeColors)
            {
                if (_themeColors != null)
                {
                    int[] intThemeColors = Array.ConvertAll<Color, int>(_themeColors, new Converter<Color, int>(ColorTranslator.ToWin32));
                    uint[] uintThemeColors = Array.ConvertAll<int, uint>(intThemeColors, new Converter<int, uint>(Convert.ToUInt32));
                    newValue.SetUIntVector(uintThemeColors);
                }
            }
            else if (key == RibbonProperties.ThemeColorsCategoryLabel)
            {
                if (_themeColorsCategoryLabel != null)
                {
                    newValue.SetString(_themeColorsCategoryLabel);
                }
            }
            else if (key == RibbonProperties.ThemeColorsTooltips)
            {
                if (_themeColorsTooltips != null)
                {
                    newValue.SetStringVector(_themeColorsTooltips);
                }
            }
             
            return HRESULT.S_OK;
        }

        #region IColorPickerPropertiesProvider Members

        /// <summary>
        /// Automatic color label property
        /// </summary>
        public string AutomaticColorLabel
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant automaticColorLabel;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.AutomaticColorLabel, out automaticColorLabel);
                    if (NativeMethods.Succeeded(hr))
                    {
                        string result = (string)automaticColorLabel.Value;
                        PropVariant.Clear(ref automaticColorLabel);
                        return result;
                    }
                }

                return _automaticColorLabel;
            }
            set
            {
                _automaticColorLabel = value;
                if (_ribbon.Initialized)
                {
                    PropVariant automaticColorLabel;
                    if ((_automaticColorLabel == null) || (_automaticColorLabel.Trim() == string.Empty))
                    {
                        automaticColorLabel = PropVariant.Empty;
                    }
                    else
                    {
                        automaticColorLabel = PropVariant.FromObject(_automaticColorLabel);
                    }
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.AutomaticColorLabel, ref automaticColorLabel);
                    PropVariant.Clear(ref automaticColorLabel);
                }
            }
        }

        /// <summary>
        /// Color property
        /// </summary>
        public Color Color
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant color;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.Color, out color);
                    if (NativeMethods.Succeeded(hr))
                    {
                        return ColorTranslator.FromWin32((int)(uint)color.Value);
                    }
                }

                return _color.GetValueOrDefault();
            }
            set
            {
                _color = value;
                if (_ribbon.Initialized)
                {
                    PropVariant color = PropVariant.FromObject((uint)ColorTranslator.ToWin32(value));
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.Color, ref color);
                }
            }
        }

        /// <summary>
        /// Color type property
        /// </summary>
        public SwatchColorType ColorType
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant colorType;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.ColorType, out colorType);
                    if (NativeMethods.Succeeded(hr))
                    {
                        return (SwatchColorType)colorType.Value;
                    }
                }

                return _colorType.GetValueOrDefault();
            }
            set
            {
                _colorType = value;
                if (_ribbon.Initialized)
                {
                    PropVariant colorType = PropVariant.FromObject((uint)value);
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.ColorType, ref colorType);
                }
            }
        }

        /// <summary>
        /// More colors label property
        /// </summary>
        public string MoreColorsLabel
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant moreColorsLabel;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.MoreColorsLabel, out moreColorsLabel);
                    if (NativeMethods.Succeeded(hr))
                    {
                        string result = (string)moreColorsLabel.Value;
                        PropVariant.Clear(ref moreColorsLabel);
                        return result;
                    }
                }

                return _moreColorsLabel;
            }
            set
            {
                _moreColorsLabel = value;
                if (_ribbon.Initialized)
                {
                    PropVariant moreColorsLabel;
                    if ((_moreColorsLabel == null) || (_moreColorsLabel.Trim() == string.Empty))
                    {
                        moreColorsLabel = PropVariant.Empty;
                    }
                    else
                    {
                        moreColorsLabel = PropVariant.FromObject(_moreColorsLabel);
                    }
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.MoreColorsLabel, ref moreColorsLabel);
                    PropVariant.Clear(ref moreColorsLabel);
                }
            }
        }

        /// <summary>
        /// No color label property
        /// </summary>
        public string NoColorLabel
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant noColorLabel;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.NoColorLabel, out noColorLabel);
                    if (NativeMethods.Succeeded(hr))
                    {
                        string result = (string)noColorLabel.Value;
                        PropVariant.Clear(ref noColorLabel);
                        return result;
                    }
                }

                return _noColorLabel;
            }
            set
            {
                _noColorLabel = value;
                if (_ribbon.Initialized)
                {
                    PropVariant noColorLabel;
                    if ((_noColorLabel == null) || (_noColorLabel.Trim() == string.Empty))
                    {
                        noColorLabel = PropVariant.Empty;
                    }
                    else
                    {
                        noColorLabel = PropVariant.FromObject(_noColorLabel);
                    }
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.NoColorLabel, ref noColorLabel);
                    PropVariant.Clear(ref noColorLabel);
                }
            }
        }

        /// <summary>
        /// Recent colors category label property
        /// </summary>
        public string RecentColorsCategoryLabel
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant recentColorsCategoryLabel;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.RecentColorsCategoryLabel, out recentColorsCategoryLabel);
                    if (NativeMethods.Succeeded(hr))
                    {
                        string result = (string)recentColorsCategoryLabel.Value;
                        PropVariant.Clear(ref recentColorsCategoryLabel);
                        return result;
                    }
                }

                return _recentColorsCategoryLabel;
            }
            set
            {
                _recentColorsCategoryLabel = value;
                if (_ribbon.Initialized)
                {
                    PropVariant recentColorsCategoryLabel;
                    if ((_recentColorsCategoryLabel == null) || (_recentColorsCategoryLabel.Trim() == string.Empty))
                    {
                        recentColorsCategoryLabel = PropVariant.Empty;
                    }
                    else
                    {
                        recentColorsCategoryLabel = PropVariant.FromObject(_recentColorsCategoryLabel);
                    }
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.RecentColorsCategoryLabel, ref recentColorsCategoryLabel);
                    PropVariant.Clear(ref recentColorsCategoryLabel);
                }
            }
        }

        /// <summary>
        /// Standard colors property
        /// </summary>
        public Color[] StandardColors
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant standardColors;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.StandardColors, out standardColors);
                    if (NativeMethods.Succeeded(hr))
                    {
                        uint[] uintStandardColors = (uint[])standardColors.Value;
                        int[] intStandardColors = Array.ConvertAll<uint, int>(uintStandardColors, new Converter<uint, int>(Convert.ToInt32));
                        Color[] colorStandardColors = Array.ConvertAll<int, Color>(intStandardColors, new Converter<int, Color>(ColorTranslator.FromWin32));
                        PropVariant.Clear(ref standardColors);
                        return colorStandardColors;
                    }
                }

                return _standardColors;
            }
            set
            {
                _standardColors = value;
                if (_ribbon.Initialized)
                {
                    int[] intStandardColors = Array.ConvertAll<Color, int>(_standardColors, new Converter<Color, int>(ColorTranslator.ToWin32));
                    uint[] uintStandardColors = Array.ConvertAll<int, uint>(intStandardColors, new Converter<int, uint>(Convert.ToUInt32));

                    PropVariant standardColors = PropVariant.FromObject(uintStandardColors);
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.StandardColors, ref standardColors);
                    PropVariant.Clear(ref standardColors);
                }
            }
        }

        /// <summary>
        /// Standard colors category label property
        /// </summary>
        public string StandardColorsCategoryLabel
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant standardColorsCategoryLabel;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.StandardColorsCategoryLabel, out standardColorsCategoryLabel);
                    if (NativeMethods.Succeeded(hr))
                    {
                        string result = (string)standardColorsCategoryLabel.Value;
                        PropVariant.Clear(ref standardColorsCategoryLabel);
                        return result;
                    }
                }

                return _standardColorsCategoryLabel;
            }
            set
            {
                _standardColorsCategoryLabel = value;
                if (_ribbon.Initialized)
                {
                    PropVariant standardColorsCategoryLabel;
                    if ((_standardColorsCategoryLabel == null) || (_standardColorsCategoryLabel.Trim() == string.Empty))
                    {
                        standardColorsCategoryLabel = PropVariant.Empty;
                    }
                    else
                    {
                        standardColorsCategoryLabel = PropVariant.FromObject(_standardColorsCategoryLabel);
                    }
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.StandardColorsCategoryLabel, ref standardColorsCategoryLabel);
                    PropVariant.Clear(ref standardColorsCategoryLabel);
                }
            }
        }

        /// <summary>
        /// Standard colors tooltips property
        /// </summary>
        public string[] StandardColorsTooltips
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant standardColorsTooltips;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.StandardColorsTooltips, out standardColorsTooltips);
                    if (NativeMethods.Succeeded(hr))
                    {
                        string[] result = (string[])standardColorsTooltips.Value;
                        PropVariant.Clear(ref standardColorsTooltips);
                        return result;
                    }
                }

                return _standardColorsTooltips;
            }
            set
            {
                _standardColorsTooltips = value;
                if (_ribbon.Initialized)
                {
                    PropVariant standardColorsTooltips = PropVariant.FromObject(value);
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.StandardColorsTooltips, ref standardColorsTooltips);
                    PropVariant.Clear(ref standardColorsTooltips);
                }
            }
        }

        /// <summary>
        /// Theme colors property
        /// </summary>
        public Color[] ThemeColors
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant themeColors;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.ThemeColors, out themeColors);
                    if (NativeMethods.Succeeded(hr))
                    {
                        uint[] uintThemeColors = (uint[])themeColors.Value;
                        int[] intThemeColors = Array.ConvertAll<uint, int>(uintThemeColors, new Converter<uint, int>(Convert.ToInt32));
                        Color[] colorThemeColors = Array.ConvertAll<int, Color>(intThemeColors, new Converter<int, Color>(ColorTranslator.FromWin32));
                        PropVariant.Clear(ref themeColors);
                        return colorThemeColors;
                    }
                }

                return _themeColors;
            }
            set
            {
                _themeColors = value;
                if (_ribbon.Initialized)
                {
                    int[] intThemeColors = Array.ConvertAll<Color, int>(_themeColors, new Converter<Color, int>(ColorTranslator.ToWin32));
                    uint[] uintThemeColors = Array.ConvertAll<int, uint>(intThemeColors, new Converter<int, uint>(Convert.ToUInt32));

                    PropVariant themeColors = PropVariant.FromObject(uintThemeColors);
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.ThemeColors, ref themeColors);
                    PropVariant.Clear(ref themeColors);
                }
            }
        }

        /// <summary>
        /// Theme colors category label property
        /// </summary>
        public string ThemeColorsCategoryLabel
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant themeColorsCategoryLabel;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.ThemeColorsCategoryLabel, out themeColorsCategoryLabel);
                    if (NativeMethods.Succeeded(hr))
                    {
                        string result = (string)themeColorsCategoryLabel.Value;
                        PropVariant.Clear(ref themeColorsCategoryLabel);
                        return result;
                    }
                }

                return _themeColorsCategoryLabel;
            }
            set
            {
                _themeColorsCategoryLabel = value;
                if (_ribbon.Initialized)
                {
                    PropVariant themeColorsCategoryLabel;
                    if ((_themeColorsCategoryLabel == null) || (_themeColorsCategoryLabel.Trim() == string.Empty))
                    {
                        themeColorsCategoryLabel = PropVariant.Empty;
                    }
                    else
                    {
                        themeColorsCategoryLabel = PropVariant.FromObject(_themeColorsCategoryLabel);
                    }
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.ThemeColorsCategoryLabel, ref themeColorsCategoryLabel);
                    PropVariant.Clear(ref themeColorsCategoryLabel);
                }
            }
        }

        /// <summary>
        /// Theme colors tooltips property
        /// </summary>
        public string[] ThemeColorsTooltips
        {
            get
            {
                if (_ribbon.Initialized)
                {
                    PropVariant themeColorsTooltips;
                    HRESULT hr = _ribbon.Framework.GetUICommandProperty(_commandID, ref RibbonProperties.ThemeColorsTooltips, out themeColorsTooltips);
                    if (NativeMethods.Succeeded(hr))
                    {
                        string[] result = (string[])themeColorsTooltips.Value;
                        PropVariant.Clear(ref themeColorsTooltips);
                        return result;
                    }
                }

                return _themeColorsTooltips;
            }
            set
            {
                _themeColorsTooltips = value;
                if (_ribbon.Initialized)
                {
                    PropVariant themeColorsTooltips = PropVariant.FromObject(value);
                    HRESULT hr = _ribbon.Framework.SetUICommandProperty(_commandID, ref RibbonProperties.ThemeColorsTooltips, ref themeColorsTooltips);
                    PropVariant.Clear(ref themeColorsTooltips);
                }
            }
        }
       
        #endregion
    }
}
