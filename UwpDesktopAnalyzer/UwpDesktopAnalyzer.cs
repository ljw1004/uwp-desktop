using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;


[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public class UwpDesktopAnalyzer : DiagnosticAnalyzer
{
    public static DiagnosticDescriptor Rule = new DiagnosticDescriptor("UWP003", "UWP-only", "Type '{0}' can only be used in UWP apps, not Desktop or Centennial", "Safety", DiagnosticSeverity.Warning, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, CS.SyntaxKind.InvocationExpression, CS.SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, VB.SyntaxKind.InvocationExpression, VB.SyntaxKind.ObjectCreationExpression);
        // Once Dev15 rolls around it would be nicer to change this to context.RegisterOperationAction(OperationKind.InvocationExpression) ...
    }

    void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        bool cs = (context.Node.IsKind(CS.SyntaxKind.InvocationExpression) || context.Node.IsKind(CS.SyntaxKind.ObjectCreationExpression));
        bool inv = (context.Node.IsKind(CS.SyntaxKind.InvocationExpression) || context.Node.IsKind(VB.SyntaxKind.InvocationExpression));
        string method, type;
        if (inv)
        {
            var symbolInfo = cs
                ? context.SemanticModel.GetSymbolInfo((context.Node as CS.Syntax.InvocationExpressionSyntax).Expression)
                : context.SemanticModel.GetSymbolInfo((context.Node as VB.Syntax.InvocationExpressionSyntax).Expression);
            if (symbolInfo.Symbol == null || !symbolInfo.Symbol.IsStatic || symbolInfo.Symbol.ContainingType == null) return;
            type = symbolInfo.Symbol.ContainingType.ToDisplayString();
            method = type + "." + symbolInfo.Symbol.Name;
        }
        else
        {
            var symbolInfo = cs
                ? context.SemanticModel.GetSymbolInfo((context.Node as CS.Syntax.ObjectCreationExpressionSyntax).Type)
                : context.SemanticModel.GetSymbolInfo((context.Node as VB.Syntax.ObjectCreationExpressionSyntax).Type);
            if (symbolInfo.Symbol == null) return;
            type = symbolInfo.Symbol.ToDisplayString();
            method = type + "..ctor";
        }

        var loc = context.Node.GetLocation();
        if (!loc.IsInSource) return;
        if (!ForbiddenStaticMethods.Contains(method)) return;
        context.ReportDiagnostic(Diagnostic.Create(Rule, loc, type));
    }


    private static HashSet<string> _forbiddenStaticMethods;

    public static HashSet<string> ForbiddenStaticMethods
    {
        get
        {
            if (_forbiddenStaticMethods != null) return _forbiddenStaticMethods;
            _forbiddenStaticMethods = new HashSet<string>()
            {
"Windows.Foundation.Diagnostics.AsyncCausalityTracer.TraceOperationCreation",
"Windows.Foundation.Diagnostics.AsyncCausalityTracer.TraceOperationCompletion",
"Windows.Foundation.Diagnostics.AsyncCausalityTracer.TraceOperationRelation",
"Windows.Foundation.Diagnostics.AsyncCausalityTracer.TraceSynchronousWorkStart",
"Windows.Foundation.Diagnostics.AsyncCausalityTracer.TraceSynchronousWorkCompletion",
"Windows.Graphics.Display.DisplayInformation.GetForCurrentView",
"Windows.Storage.Search.ContentIndexer.GetIndexer",
"Windows.Storage.Search.ContentIndexer.GetIndexer",
"Windows.Storage.Search.ValueAndLanguage..ctor",
"Windows.Storage.Search.IndexableContent..ctor",
"Windows.System.Power.Diagnostics.BackgroundEnergyDiagnostics.ComputeTotalEnergyUsage",
"Windows.System.Power.Diagnostics.BackgroundEnergyDiagnostics.ResetTotalEnergyUsage",
"Windows.System.Power.Diagnostics.ForegroundEnergyDiagnostics.ComputeTotalEnergyUsage",
"Windows.System.Power.Diagnostics.ForegroundEnergyDiagnostics.ResetTotalEnergyUsage",
"Windows.System.MemoryManager.TrySetAppMemoryUsageLimit",
"Windows.System.MemoryManager.GetAppMemoryReport",
"Windows.System.MemoryManager.GetProcessMemoryReport",
"Windows.Storage.Search.QueryOptions..ctor",
"Windows.Storage.Search.QueryOptions..ctor",
"Windows.Storage.Search.QueryOptions..ctor",
"Windows.ApplicationModel.UserDataAccounts.UserDataAccountManager.RequestStoreAsync",
"Windows.ApplicationModel.UserDataAccounts.UserDataAccountManager.ShowAddAccountAsync",
"Windows.ApplicationModel.UserDataAccounts.UserDataAccountManager.ShowAccountSettingsAsync",
"Windows.ApplicationModel.UserDataAccounts.UserDataAccountManager.ShowAccountErrorResolverAsync",
"Windows.ApplicationModel.UserDataAccounts.SystemAccess.DeviceAccountConfiguration..ctor",
"Windows.ApplicationModel.UserDataAccounts.SystemAccess.UserDataAccountSystemAccessManager.AddAndShowDeviceAccountsAsync",
"Windows.ApplicationModel.Store.ProductPurchaseDisplayProperties..ctor",
"Windows.ApplicationModel.Store.ProductPurchaseDisplayProperties..ctor",
"Windows.ApplicationModel.Store.CurrentApp.GetCustomerPurchaseIdAsync",
"Windows.ApplicationModel.Store.CurrentApp.GetCustomerCollectionsIdAsync",
"Windows.ApplicationModel.Store.CurrentApp.GetAppPurchaseCampaignIdAsync",
"Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationByProductIdsAsync",
"Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationByKeywordsAsync",
"Windows.ApplicationModel.Store.CurrentApp.ReportProductFulfillment",
"Windows.ApplicationModel.Store.CurrentApp.ReportConsumableFulfillmentAsync",
"Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync",
"Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync",
"Windows.ApplicationModel.Store.CurrentApp.GetUnfulfilledConsumablesAsync",
"Windows.ApplicationModel.Store.CurrentApp.RequestAppPurchaseAsync",
"Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync",
"Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationAsync",
"Windows.ApplicationModel.Store.CurrentApp.GetAppReceiptAsync",
"Windows.ApplicationModel.Store.CurrentApp.GetProductReceiptAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.GetAppPurchaseCampaignIdAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.LoadListingInformationByProductIdsAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.LoadListingInformationByKeywordsAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.ReportConsumableFulfillmentAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.RequestProductPurchaseAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.RequestProductPurchaseAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.GetUnfulfilledConsumablesAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.RequestAppPurchaseAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.RequestProductPurchaseAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.LoadListingInformationAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.GetAppReceiptAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.GetProductReceiptAsync",
"Windows.ApplicationModel.Store.CurrentAppSimulator.ReloadSimulatorAsync",
"Windows.Devices.Enumeration.DeviceAccessInformation.CreateFromId",
"Windows.Devices.Enumeration.DeviceAccessInformation.CreateFromDeviceClassId",
"Windows.Devices.Enumeration.DeviceAccessInformation.CreateFromDeviceClass",
"Windows.Devices.Geolocation.Geofencing.Geofence..ctor",
"Windows.Devices.Geolocation.Geofencing.Geofence..ctor",
"Windows.Devices.Geolocation.Geofencing.Geofence..ctor",
"Windows.Devices.Geolocation.Geofencing.Geofence..ctor",
"Windows.Devices.SmartCards.SmartCardProvisioning.RequestAttestedVirtualSmartCardCreationAsync",
"Windows.Devices.SmartCards.SmartCardProvisioning.RequestAttestedVirtualSmartCardCreationAsync",
"Windows.Devices.SmartCards.SmartCardProvisioning.FromSmartCardAsync",
"Windows.Devices.SmartCards.SmartCardProvisioning.RequestVirtualSmartCardCreationAsync",
"Windows.Devices.SmartCards.SmartCardProvisioning.RequestVirtualSmartCardCreationAsync",
"Windows.Devices.SmartCards.SmartCardProvisioning.RequestVirtualSmartCardDeletionAsync",
"Windows.Devices.SmartCards.SmartCardPinPolicy..ctor",
"Windows.Foundation.Diagnostics.RuntimeBrokerErrorSettings..ctor",
"Windows.Foundation.Diagnostics.ErrorDetails.CreateFromHResultAsync",
"Windows.Graphics.Printing.OptionDetails.PrintTaskOptionDetails.GetFromPrintTaskOptions",
"Windows.Graphics.Printing.PrintManager.GetForCurrentView",
"Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync",
"Windows.Media.Effects.AudioEffectsManager.CreateAudioRenderEffectsManager",
"Windows.Media.Effects.AudioEffectsManager.CreateAudioRenderEffectsManager",
"Windows.Media.Effects.AudioEffectsManager.CreateAudioCaptureEffectsManager",
"Windows.Media.Effects.AudioEffectsManager.CreateAudioCaptureEffectsManager",
"Windows.Networking.Proximity.PeerFinder.CreateWatcher",
"Windows.Networking.Proximity.PeerFinder.Start",
"Windows.Networking.Proximity.PeerFinder.Start",
"Windows.Networking.Proximity.PeerFinder.Stop",
"Windows.Networking.Proximity.PeerFinder.FindAllPeersAsync",
"Windows.Networking.Proximity.PeerFinder.ConnectAsync",
"Windows.Networking.Vpn.VpnRoute..ctor",
"Windows.Networking.Vpn.VpnNamespaceInfo..ctor",
"Windows.Networking.Vpn.VpnInterfaceId..ctor",
"Windows.Networking.Vpn.VpnRouteAssignment..ctor",
"Windows.Networking.Vpn.VpnNamespaceAssignment..ctor",
"Windows.Networking.Vpn.VpnPacketBuffer..ctor",
"Windows.Networking.Vpn.VpnChannel.ProcessEventAsync",
"Windows.Networking.Vpn.VpnDomainNameAssignment..ctor",
"Windows.Networking.Vpn.VpnTrafficFilterAssignment..ctor",
"Windows.Networking.Vpn.VpnAppId..ctor",
"Windows.Networking.Vpn.VpnDomainNameInfo..ctor",
"Windows.Networking.Vpn.VpnTrafficFilter..ctor",
"Windows.Networking.Vpn.VpnCustomEditBox..ctor",
"Windows.Networking.Vpn.VpnCustomPromptTextInput..ctor",
"Windows.Networking.Vpn.VpnCustomComboBox..ctor",
"Windows.Networking.Vpn.VpnCustomPromptOptionSelector..ctor",
"Windows.Networking.Vpn.VpnCustomTextBox..ctor",
"Windows.Networking.Vpn.VpnCustomPromptText..ctor",
"Windows.Networking.Vpn.VpnCustomCheckBox..ctor",
"Windows.Networking.Vpn.VpnCustomPromptBooleanInput..ctor",
"Windows.Networking.Vpn.VpnCustomErrorBox..ctor",
"Windows.Networking.Vpn.VpnPlugInProfile..ctor",
"Windows.Networking.Vpn.VpnNativeProfile..ctor",
"Windows.Networking.Vpn.VpnManagementAgent..ctor",
"Windows.System.LauncherOptions..ctor",
"Windows.System.FolderLauncherOptions..ctor",
"Windows.System.Launcher.LaunchFolderAsync",
"Windows.System.Launcher.LaunchFolderAsync",
"Windows.System.Launcher.LaunchUriForResultsAsync",
"Windows.System.Launcher.LaunchUriForResultsAsync",
"Windows.System.Launcher.LaunchUriAsync",
"Windows.System.Launcher.QueryUriSupportAsync",
"Windows.System.Launcher.QueryUriSupportAsync",
"Windows.System.Launcher.QueryFileSupportAsync",
"Windows.System.Launcher.QueryFileSupportAsync",
"Windows.System.Launcher.FindUriSchemeHandlersAsync",
"Windows.System.Launcher.FindUriSchemeHandlersAsync",
"Windows.System.Launcher.FindFileHandlersAsync",
"Windows.System.Launcher.LaunchFileAsync",
"Windows.System.Launcher.LaunchFileAsync",
"Windows.System.Launcher.LaunchUriAsync",
"Windows.System.Launcher.LaunchUriAsync",
"Windows.Media.ContentRestrictions.RatedContentDescription..ctor",
"Windows.Media.ContentRestrictions.RatedContentRestrictions..ctor",
"Windows.Media.ContentRestrictions.RatedContentRestrictions..ctor",
"Windows.UI.Notifications.TileFlyoutNotification..ctor",
"Windows.UI.Notifications.TileFlyoutUpdateManager.CreateTileFlyoutUpdaterForApplication",
"Windows.UI.Notifications.TileFlyoutUpdateManager.CreateTileFlyoutUpdaterForApplication",
"Windows.UI.Notifications.TileFlyoutUpdateManager.CreateTileFlyoutUpdaterForSecondaryTile",
"Windows.UI.Notifications.TileFlyoutUpdateManager.GetTemplateContent",
"Windows.ApplicationModel.Resources.Management.ResourceIndexer..ctor",
"Windows.ApplicationModel.Resources.Management.ResourceIndexer..ctor",
"Windows.Management.Orchestration.CurrentAppOrchestration.GetForCurrentView",
"Windows.Web.Http.Diagnostics.HttpDiagnosticProvider.CreateFromProcessDiagnosticInfo"            };
            return _forbiddenStaticMethods;
        }
    }
}

