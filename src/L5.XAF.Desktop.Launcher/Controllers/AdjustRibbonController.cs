﻿using System.Windows.Forms.VisualStyles;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

namespace AppifySheets.Desktop.Launcher.Controllers;
public class AdjustRibbonController : WindowController {
    ActionControlsSiteController? actionControlsSiteController;
    string[] needWordWrapActionsId = { "SaveAs", "SaveTo", "RichEdit_Open", "Task.MarkCompleted", "NextObject" };
    string[] beginGroupActionsId = { "Save", "Delete", "Cancel", "ResetView", "OpenObject", "ResetViewSettings", "ShowInReportV2", "ShowAllContexts" };
    public AdjustRibbonController() : base() { }
    protected override void OnActivated() {
        base.OnActivated();
        actionControlsSiteController = Frame.GetController<ActionControlsSiteController>();
        if(actionControlsSiteController != null) {
            actionControlsSiteController.CustomizeActionControl += ActionControlsSiteController_CustomizeActionControl;
            actionControlsSiteController.CustomAddActionControlToContainer += ActionControlsSiteController_CustomAddActionControlToContainer;
        }
    }
    protected override void OnDeactivated() {
        base.OnDeactivated();
        if(actionControlsSiteController != null) {
            actionControlsSiteController.CustomizeActionControl -= new EventHandler<ActionControlEventArgs>(ActionControlsSiteController_CustomizeActionControl);
            actionControlsSiteController.CustomAddActionControlToContainer -= ActionControlsSiteController_CustomAddActionControlToContainer;
        }
    }
    public BarItemLink? GetBarItemLinkForBarItem(IActionControlsSite actionControlsSite, VisualStyleElement.Menu.BarItem barItem) {
        BarItemLink? result = null;
        foreach(IActionControlContainer controlContainer in actionControlsSite.ActionContainers) {
            if(controlContainer is BarLinkActionControlContainer) {
                result = ((BarLinkActionControlContainer)controlContainer).BarContainerItem.ItemLinks.FirstOrDefault<BarItemLink>(x => x.Item == barItem);
                if(result != null) {
                    break;
                }
            }
        }
        return result;
    }

    void ActionControlsSiteController_CustomizeActionControl(object? sender, ActionControlEventArgs e) {
        if((Frame.Template is RibbonForm) && needWordWrapActionsId.Contains(e.ActionControl.ActionId) && (e.ActionControl.NativeControl is BarButtonItem)) {
            BarItemLink? barItemLink = GetBarItemLinkForBarItem((IActionControlsSite)Frame.Template, (VisualStyleElement.Menu.BarItem)e.ActionControl.NativeControl);
            if((barItemLink != null) && (!barItemLink.IsLinkInMenu)) {
                barItemLink.UserCaption = barItemLink.Item.Caption.Replace(' ', '\n');
            }
        }
        if(beginGroupActionsId.Contains(e.ActionControl.ActionId) && (e.ActionControl.NativeControl is VisualStyleElement.Menu.BarItem)) {
            BarItemLink? barItemLink = GetBarItemLinkForBarItem((IActionControlsSite)Frame.Template, (VisualStyleElement.Menu.BarItem)e.ActionControl.NativeControl);
            if(barItemLink != null) {
                barItemLink.BeginGroup = true;
            }
        }
    }

    void ActionControlsSiteController_CustomAddActionControlToContainer(object? sender, DevExpress.ExpressApp.Templates.ActionControls.CustomAddActionControlEventArgs e) {
        if(!(Frame.Template is RibbonForm)) {
            return;
        }
        if((e.Container is BarLinkActionControlContainer) && (e.Action.Id == ChangeVariantController.ChangeVariantActionId)) {
            BarLinkActionControlContainer barLinkControlContainer = (BarLinkActionControlContainer)e.Container;
            if(!barLinkControlContainer.IsMenuMode) {
                barLinkControlContainer.AddBarButtonItemSingleChoiceActionControl(ChangeVariantController.ChangeVariantActionId, DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsMode);
                e.Handled = true;
            }
        }
    }
}
