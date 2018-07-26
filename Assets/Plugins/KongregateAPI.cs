// Kongregate API Version: 3.0.0
//
// This file is now only used for classes required for cross-platform support
// The other Kongregate API classes have been moved to KongregateAPI.dll
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine;

namespace Kongregate {
  public partial class InitializeTargetPlatform {
    public static void Initialize() {
#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        TargetPlatform.Current = TargetPlatforms.Mobile;
        Debug.Log("Kongregate target platform is mobile");
#elif UNITY_WEBPLAYER || UNITY_WEBGL
        TargetPlatform.Current = TargetPlatforms.Web;
        Debug.Log("Kongregate target platform is web");
#elif UNITY_STANDALONE
        TargetPlatform.Current = TargetPlatforms.Standalone;
        Debug.Log("Kongregate target platform is standalone");
#else
        TargetPlatform.Current = TargetPlatforms.Unknown;
        Debug.Log("Kongregate target platform is unknown");
#endif
    }
  }

#if UNITY_IOS || UNITY_IPHONE
  internal class IOSInternal : Internal {
    public IOSInternal(KongregateAPI api) : base(api) {
    }

    public static void Initialize(long gameId, string apiKey, string settingsJson) {
      _KongregateAPIInitializeJSON(gameId, apiKey, settingsJson);
    }

    public override bool IsReady() {
      return _KongregateAPIIsReady();
    }

    public override void MessageReceived(string eventName) {
      _KongregateAPIMessageReceived(eventName);
    }

    protected override void AssignEventListener(string gameObjectName, string functionName) {
      _KongregateAPISetEventListenter(gameObjectName, functionName);
    }

    protected override void AssignBundleEventListener(string gameObjectName, string functionName) {
      _KongregateAPISetEventBundleListener(gameObjectName, functionName);
    }

    [DllImport ("__Internal")]
    private static extern void _KongregateAPISetEventListenter(string gameObjectName, string functionName);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPISetEventBundleListener(string gameObjectName, string functionName);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMessageReceived(string eventName);
    [DllImport ("__Internal")]
    private static extern bool _KongregateAPIIsReady();
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIInitializeJSON(long gameId, string apiKey, string settingsJson);
  }

  internal class IOSAnalytics : Analytics {
    public IOSAnalytics(KongregateAPI api) : base(api) {
    }

    public override void SetSwrveButtonListener(string gameObjectName, string functionName) {
      _KongregateAPIAnalyticsSetSwrveButtonListener(gameObjectName, functionName);
    }

    public override void SetDeltaButtonListener(string gameObjectName, string functionName) {
      _KongregateAPIAnalyticsSetDeltaButtonListener(gameObjectName, functionName);
    }

    public override void SetDeltaParameterListener(string gameObjectName, string functionName) {
      _KongregateAPIAnalyticsSetDeltaParameterListener(gameObjectName, functionName);
    }

    public override void SetAutoEventListener(string gameObjectName, string functionName) {
      _KongregateAPIAnalyticsSetAutoEventListener(gameObjectName, functionName);
    }

    public override void AddFilterType(string filterType) {
      _KongregateAPIAnalyticsAddFilterType(filterType);
    }

    public override string GetAutoPropertiesJSON() {
      return _KongregateAPIAnalyticsGetAutoPropertiesJSON();
    }

    public override long GetAutoLongProperty(string field) {
      return _KongregateAPIAnalyticsGetAutoLongProperty(field);
    }

    public override string GetAutoStringProperty(string field) {
      return _KongregateAPIAnalyticsGetAutoStringProperty(field);
    }

    public override bool GetAutoBoolProperty(string field) {
      return _KongregateAPIAnalyticsGetAutoBoolProperty(field);
    }

    public override double GetAutoDoubleProperty(string field) {
      return _KongregateAPIAnalyticsGetAutoDoubleProperty(field);
    }

    public override int GetAutoIntProperty(string field) {
      return _KongregateAPIAnalyticsGetAutoIntProperty(field);
    }

    public override string GetAutoUTCProperty(string field) {
      return _KongregateAPIAnalyticsGetAutoUTCProperty(field);
    }

    public override void StartPurchase(string productID, int quantity, string gameFieldsJson) {
      UpdateCommonProps();
      _KongregateAPIAnalyticsStartPurchase(productID, quantity, gameFieldsJson);
    }

    public override void FinishPurchase(string resultCode, string transactionId, string gameFieldsJson, string dataSignature) {
      UpdateCommonProps();
      _KongregateAPIAnalyticsFinishPurchase(resultCode, transactionId, gameFieldsJson);
    }

    public override void FinishPurchaseWithProductId(string resultCode, string productID, string receipt, string gameFieldsJson) {
      _KongregateAPIAnalyticsFinishPurchaseWithProductId(resultCode, productID, receipt, gameFieldsJson);
    }

    public override string GetResourceNames() {
      return _KongregateAPIAnalyticsGetResourceNames();
    }

    public override string GetResourceAsString(string resourceId, string attributeId, string defValue) {
      return _KongregateAPIAnalyticsGetResourceAsString(resourceId, attributeId, defValue);
    }

    public override int GetResourceAsInt(string resourceId, string attributeId, int defValue) {
      return _KongregateAPIAnalyticsGetResourceAsInt(resourceId, attributeId, defValue);
    }

    public override float GetResourceAsFloat(string resourceId, string attributeId, float defValue) {
      return _KongregateAPIAnalyticsGetResourceAsFloat(resourceId, attributeId, defValue);
    }

    public override bool GetResourceAsBool(string resourceId, string attributeId, bool defValue) {
      return _KongregateAPIAnalyticsGetResourceAsBool(resourceId, attributeId, defValue);
    }

    public override void GameUserUpdate(string propsJSON) {
      _KongregateAPIAnalyticsGameUserUpdate(propsJSON);
    }

    public override void FinishPromoAward(string promoId) {
      _KongregateAPIAnalyticsFinishPromoAward(promoId);
    }

    public override void Start() {
      _KongregateAPIAnalyticsStart();
    }

    protected override void AddEvent(string collection, string jsonMap, string commonPropsJson) {
      _KongregateAPIAnalyticsAddEvent(collection, jsonMap, commonPropsJson);
    }

    protected override void UpdateCommonProps(string mapJson) {
      _KongregateAPIAnalyticsUpdateCommonProps(mapJson);
    }

    protected override void TrackPurchase(string productID, int quantity, String fieldsJSON) {
      _KongregateAPIAnalyticsTrackPurchase(productID, quantity, fieldsJSON);
    }

    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsSetSwrveButtonListener(string gameObjectName, string functionName);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsSetDeltaButtonListener(string gameObjectName, string functionName);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsSetDeltaParameterListener(string gameObjectName, string functionName);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsSetAutoEventListener(string gameObjectName, string functionName);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsAddEvent(string collection, string eventJson, string commonPropsJson);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsAddFilterType(string filterType);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsUpdateCommonProps(string mapJson);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsGameUserUpdate(string propsJson);
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIAnalyticsGetAutoPropertiesJSON();
    [DllImport ("__Internal")]
    private static extern long _KongregateAPIAnalyticsGetAutoLongProperty(string field);
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIAnalyticsGetAutoStringProperty(string field);
    [DllImport ("__Internal")]
    private static extern bool _KongregateAPIAnalyticsGetAutoBoolProperty(string field);
    [DllImport ("__Internal")]
    private static extern int _KongregateAPIAnalyticsGetAutoIntProperty(string field);
    [DllImport ("__Internal")]
    private static extern double _KongregateAPIAnalyticsGetAutoDoubleProperty(string field);
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIAnalyticsGetAutoUTCProperty(string field);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsStartPurchase(string productID, int quantity, string fieldsJson);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsFinishPurchase(string resultCode, string transactionId, string fieldsJson);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsFinishPurchaseWithProductId(string resultCode, string productId, string receipt, string fieldsJson);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsTrackPurchase(string productID, int quantity, string fieldsJson);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsStart();
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIAnalyticsGetResourceNames();
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIAnalyticsGetResourceAsString(string resourceId, string attributeId, string defValue);
    [DllImport ("__Internal")]
    private static extern bool _KongregateAPIAnalyticsGetResourceAsBool(string resourceId, string attributeId, bool defValue);
    [DllImport ("__Internal")]
    private static extern float _KongregateAPIAnalyticsGetResourceAsFloat(string resourceId, string attributeId, float defValue);
    [DllImport ("__Internal")]
    private static extern int _KongregateAPIAnalyticsGetResourceAsInt(string resourceId, string attributeId, int defValue);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIAnalyticsFinishPromoAward(string promoId);
  }

  internal class  IOSMobile : Mobile {
    public IOSMobile(KongregateAPI api) : base(api) {
    }

    public override void ButtonShow() {
      base.ButtonShow();
      _KongregateAPIMobileButtonShow();
    }

    public override void ButtonHide() {
      base.ButtonHide();
      _KongregateAPIMobileButtonHide();
    }

    public override void ButtonSetX(int x) {
      base.ButtonSetX(x);
      _KongregateAPIMobileButtonSetX(x);
    }

    public override void ButtonSetY(int y) {
      base.ButtonSetY(y);
      _KongregateAPIMobileButtonSetY(y);
    }

    public override void ButtonSetSize(int size) {
      base.ButtonSetSize(size);
      _KongregateAPIMobileButtonSetSize(size);
    }

    public override void ButtonSetNativeRendering(bool nativeRendering) {
      base.ButtonSetNativeRendering(nativeRendering);
      _KongregateAPIMobileButtonSetNativeRendering(nativeRendering);
    }

    public override void OpenKongregateWindow(string target, string id) {
      _KongregateAPIMobileOpenKongregateWindow(target, id);
    }

    public override void CloseKongregateWindow() {
      _KongregateAPIMobileCloseKongregateWindow();
    }

    public override string GetOpenURL() {
      return _KongregateAPIMobileGetOpenURL();
    }

    public override void Trigger(string name) {
      _KongregateAPIMobileTrigger(name);
    }

    protected override Texture2D CreateButtonTexture() {
      Texture2D texture2D = new Texture2D (buttonSize, buttonSize);
      string path = _KongregateAPIMobileButtonGetTexture();
      MethodInfo info = typeof(File).GetMethod("ReadAllBytes");
      byte[] imageBytes = (byte[])info.Invoke(null, new object[]{ path });
      texture2D.LoadImage(imageBytes);
      return(texture2D);
    }

    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMobileButtonHide();
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMobileButtonShow();
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIMobileButtonGetTexture();
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMobileButtonSetNativeRendering(bool nativeRendering);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMobileButtonSetX(int x);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMobileButtonSetY(int y);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMobileButtonSetSize(int size);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMobileOpenKongregateWindow(string target, string id);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMobileCloseKongregateWindow();
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIMobileGetOpenURL();
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMobileTrigger(string name);
  }

  internal class IOSMtx : Mtx {
    public IOSMtx(KongregateAPI api) : base(api) {
    }

    public override void VerifyTransactionId(string transactionIdentifier) {
      base.VerifyTransactionId(transactionIdentifier);
      _KongregateAPIMtxVerifyTransactionId(transactionIdentifier);
    }

    public override void VerifyGoogleReceipt(string receipt, string signature) {
      base.VerifyGoogleReceipt(receipt, signature);
      throw new System.InvalidOperationException("VerifyGoogleReceipt is not supported on iOS." +
        " Use VerifyTransactionId(transactionIdentifier)");
    }

    public override string ReceiptVerificationStatus(string transactionIdentifier) {
      return _KongregateAPIMtxReceiptVerificationStatus(transactionIdentifier);
    }

    protected override void PerformUserItemListRequest() {
      _KongregateAPIMtxRequestUserItemList();
    }

    protected override bool PerformHasItemQuery(string identifier) {
      return _KongregateAPIMtxHasItem(identifier);
    }

    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMtxRequestUserItemList();
    [DllImport ("__Internal")]
    private static extern bool _KongregateAPIMtxHasItem(string identifier);
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIMtxVerifyTransactionId(string transactionIdentifier);
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIMtxReceiptVerificationStatus(string transactionIdentifier);
  }

  internal class IOSServices : Services {
    public IOSServices(KongregateAPI api) : base(api) {

    }

    public override bool IsGuest() {
      return _KongregateAPIServicesIsGuest();
    }

    public override string GetUsername() {
      return _KongregateAPIServicesGetUsername();
    }

    public override string GetGameAuthToken() {
      return _KongregateAPIServicesGetGameAuthToken();
    }

    public override long GetUserId() {
      return _KongregateAPIServicesGetUserId();
    }

    public override bool HasKongPlus() {
      return _KongregateAPIServicesHasKongPlus();
    }

    public override int GetNotificationCount() {
      return _KongregateAPIServicesGetNotificationCount();
    }

    public override bool HasUnreadGuildMessages() {
      return _KongregateAPIServicesHasUnreadGuildMessages();
    }

    public override void SetCharacterToken(string characterToken) {
      _KongregateAPIServicesSetCharacterToken(characterToken);
    }

    [DllImport ("__Internal")]
    private static extern string _KongregateAPIServicesGetUsername();
    [DllImport ("__Internal")]
    private static extern bool _KongregateAPIServicesHasKongPlus();
    [DllImport ("__Internal")]
    private static extern int _KongregateAPIServicesGetNotificationCount();
    [DllImport ("__Internal")]
    private static extern bool _KongregateAPIServicesHasUnreadGuildMessages();
    [DllImport ("__Internal")]
    private static extern long _KongregateAPIServicesGetUserId();
    [DllImport ("__Internal")]
    private static extern void _KongregateAPIServicesSetCharacterToken(string token);
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIServicesGetGameAuthToken();
    [DllImport ("__Internal")]
    private static extern bool _KongregateAPIServicesIsGuest();
  }

  internal class IOSStats : Stats {
    public IOSStats(KongregateAPI api) : base(api) {
    }

    public override void Submit(string name, long value) {
      _KongregateAPIStatsSubmit(name, value);
    }

    [DllImport ("__Internal")]
    private static extern void _KongregateAPIStatsSubmit(string statisticName, long statisticValue);
  }
#endif
}
