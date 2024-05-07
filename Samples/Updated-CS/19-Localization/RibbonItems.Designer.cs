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
            public const uint cmdTab = 1012;
            public const uint cmdGroup = 1015;
            public const uint cmdButtonOne = 1008;
            public const uint cmdButtonTwo = 1009;
            public const uint cmdButtonThree = 1010;
        }

        // ContextPopup CommandName

        public Ribbon Ribbon { get; private set; }
        public RibbonTab Tab { get; private set; }
        public RibbonGroup Group { get; private set; }
        public RibbonButton ButtonOne { get; private set; }
        public RibbonButton ButtonTwo { get; private set; }
        public RibbonButton ButtonThree { get; private set; }

        public RibbonItems(Ribbon ribbon)
        {
            if (ribbon == null)
                throw new ArgumentNullException(nameof(ribbon), "Parameter is null");
            this.Ribbon = ribbon;
            Tab = new RibbonTab(ribbon, Cmd.cmdTab);
            Group = new RibbonGroup(ribbon, Cmd.cmdGroup);
            ButtonOne = new RibbonButton(ribbon, Cmd.cmdButtonOne);
            ButtonTwo = new RibbonButton(ribbon, Cmd.cmdButtonTwo);
            ButtonThree = new RibbonButton(ribbon, Cmd.cmdButtonThree);
        }

    }
}
