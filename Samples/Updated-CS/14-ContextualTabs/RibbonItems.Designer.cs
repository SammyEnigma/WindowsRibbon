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
            public const uint cmdTabGroupTableTools = 1003;
            public const uint cmdTabDesign = 1004;
            public const uint cmdGroupDesign = 1006;
            public const uint cmdButtonDesign1 = 1010;
            public const uint cmdButtonDesign2 = 1011;
            public const uint cmdButtonDesign3 = 1012;
            public const uint cmdTabLayout = 1005;
            public const uint cmdGroupLayout = 1007;
            public const uint cmdButtonLayout1 = 1013;
            public const uint cmdButtonLayout2 = 1014;
            public const uint cmdTabMain = 1001;
            public const uint cmdGroupMain = 1002;
            public const uint cmdButtonSelect = 1008;
            public const uint cmdButtonUnselect = 1009;
        }

        // ContextPopup CommandName

        public Ribbon Ribbon { get; private set; }
        public RibbonTabGroup TabGroupTableTools { get; private set; }
        public RibbonTab TabDesign { get; private set; }
        public RibbonGroup GroupDesign { get; private set; }
        public RibbonButton ButtonDesign1 { get; private set; }
        public RibbonButton ButtonDesign2 { get; private set; }
        public RibbonButton ButtonDesign3 { get; private set; }
        public RibbonTab TabLayout { get; private set; }
        public RibbonGroup GroupLayout { get; private set; }
        public RibbonButton ButtonLayout1 { get; private set; }
        public RibbonButton ButtonLayout2 { get; private set; }
        public RibbonTab TabMain { get; private set; }
        public RibbonGroup GroupMain { get; private set; }
        public RibbonButton ButtonSelect { get; private set; }
        public RibbonButton ButtonUnselect { get; private set; }

        public RibbonItems(Ribbon ribbon)
        {
            if (ribbon == null)
                throw new ArgumentNullException(nameof(ribbon), "Parameter is null");
            this.Ribbon = ribbon;
            TabGroupTableTools = new RibbonTabGroup(ribbon, Cmd.cmdTabGroupTableTools);
            TabDesign = new RibbonTab(ribbon, Cmd.cmdTabDesign);
            GroupDesign = new RibbonGroup(ribbon, Cmd.cmdGroupDesign);
            ButtonDesign1 = new RibbonButton(ribbon, Cmd.cmdButtonDesign1);
            ButtonDesign2 = new RibbonButton(ribbon, Cmd.cmdButtonDesign2);
            ButtonDesign3 = new RibbonButton(ribbon, Cmd.cmdButtonDesign3);
            TabLayout = new RibbonTab(ribbon, Cmd.cmdTabLayout);
            GroupLayout = new RibbonGroup(ribbon, Cmd.cmdGroupLayout);
            ButtonLayout1 = new RibbonButton(ribbon, Cmd.cmdButtonLayout1);
            ButtonLayout2 = new RibbonButton(ribbon, Cmd.cmdButtonLayout2);
            TabMain = new RibbonTab(ribbon, Cmd.cmdTabMain);
            GroupMain = new RibbonGroup(ribbon, Cmd.cmdGroupMain);
            ButtonSelect = new RibbonButton(ribbon, Cmd.cmdButtonSelect);
            ButtonUnselect = new RibbonButton(ribbon, Cmd.cmdButtonUnselect);
        }

    }
}