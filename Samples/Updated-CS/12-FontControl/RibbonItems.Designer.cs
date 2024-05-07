//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using RibbonLib;
using RibbonLib.Controls;

namespace RibbonLib.Controls
{
    partial class RibbonItems
    {
        private static class Cmd
        {
            public const uint cmdTabMain = 1001;
            public const uint cmdGroupRichFont = 1002;
            public const uint cmdRichFont = 1003;
        }

        // ContextPopup CommandName

        public Ribbon Ribbon { get; private set; }
        public RibbonTab TabMain { get; private set; }
        public RibbonGroup GroupRichFont { get; private set; }
        public RibbonFontControl RichFont { get; private set; }

        public RibbonItems(Ribbon ribbon)
        {
            if (ribbon == null)
                throw new ArgumentNullException(nameof(ribbon), "Parameter is null");
            this.Ribbon = ribbon;
            TabMain = new RibbonTab(ribbon, Cmd.cmdTabMain);
            GroupRichFont = new RibbonGroup(ribbon, Cmd.cmdGroupRichFont);
            RichFont = new RibbonFontControl(ribbon, Cmd.cmdRichFont);
        }

    }
}
