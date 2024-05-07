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
            public const uint cmdHelpButton = 1016;
            public const uint cmdTabMain = 1011;
            public const uint cmdGroupFileActions = 1013;
            public const uint cmdButtonNew = 1001;
            public const uint cmdButtonOpen = 1002;
            public const uint cmdButtonSave = 1003;
            public const uint cmdGroupExit = 1014;
            public const uint cmdButtonExit = 1004;
            public const uint cmdTabDrop = 1012;
            public const uint cmdGroupDrop = 1015;
            public const uint cmdButtonDropA = 1008;
            public const uint cmdButtonDropB = 1009;
            public const uint cmdButtonDropC = 1010;
        }

        // ContextPopup CommandName

        public Ribbon Ribbon { get; private set; }
        public RibbonHelpButton HelpButton { get; private set; }
        public RibbonTab TabMain { get; private set; }
        public RibbonGroup GroupFileActions { get; private set; }
        public RibbonButton ButtonNew { get; private set; }
        public RibbonButton ButtonOpen { get; private set; }
        public RibbonButton ButtonSave { get; private set; }
        public RibbonGroup GroupExit { get; private set; }
        public RibbonButton ButtonExit { get; private set; }
        public RibbonTab TabDrop { get; private set; }
        public RibbonGroup GroupDrop { get; private set; }
        public RibbonButton ButtonDropA { get; private set; }
        public RibbonButton ButtonDropB { get; private set; }
        public RibbonButton ButtonDropC { get; private set; }

        public RibbonItems(Ribbon ribbon)
        {
            if (ribbon == null)
                throw new ArgumentNullException(nameof(ribbon), "Parameter is null");
            this.Ribbon = ribbon;
            HelpButton = new RibbonHelpButton(ribbon, Cmd.cmdHelpButton);
            TabMain = new RibbonTab(ribbon, Cmd.cmdTabMain);
            GroupFileActions = new RibbonGroup(ribbon, Cmd.cmdGroupFileActions);
            ButtonNew = new RibbonButton(ribbon, Cmd.cmdButtonNew);
            ButtonOpen = new RibbonButton(ribbon, Cmd.cmdButtonOpen);
            ButtonSave = new RibbonButton(ribbon, Cmd.cmdButtonSave);
            GroupExit = new RibbonGroup(ribbon, Cmd.cmdGroupExit);
            ButtonExit = new RibbonButton(ribbon, Cmd.cmdButtonExit);
            TabDrop = new RibbonTab(ribbon, Cmd.cmdTabDrop);
            GroupDrop = new RibbonGroup(ribbon, Cmd.cmdGroupDrop);
            ButtonDropA = new RibbonButton(ribbon, Cmd.cmdButtonDropA);
            ButtonDropB = new RibbonButton(ribbon, Cmd.cmdButtonDropB);
            ButtonDropC = new RibbonButton(ribbon, Cmd.cmdButtonDropC);
        }

    }
}
