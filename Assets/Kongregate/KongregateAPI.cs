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
using DeltaDNA;

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

  internal class IOSAnalytics : UnityAnalytics {
    public IOSAnalytics(KongregateAPI api) : base(api) {
      if (!KongregateAPI.Settings.DeferAnalytics) {
        Start();
      }
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
      Dictionary<string, object> gameFields =  Json.Deserialize(gameFieldsJson) as Dictionary<string,object>;
      base.addIAPEvent(Analytics.EVENT_IAP_ATTEMPTS, productID, gameFields, null, null, null);
    }

    public override void FinishPurchase(string resultCode, string transactionId, string gameFieldsJson, string dataSignature) {
      UpdateCommonProps();
      _KongregateAPIAnalyticsFinishPurchase(resultCode, transactionId, gameFieldsJson);

      Dictionary<string, object> gameFields =  Json.Deserialize(gameFieldsJson) as Dictionary<string,object>;
      string transactionJSON = _KongregateAPIAnalyticsGetTransactionDetails(transactionId, resultCode);
      Dictionary<string, object> transactionFields =  Json.Deserialize(transactionJSON) as Dictionary<string,object>;

      FinishPurchase(resultCode, null, transactionFields, gameFields, dataSignature);
    }

    public override void FinishPurchaseWithProductId(string resultCode, string productID, string receipt, string gameFieldsJson) {
      _KongregateAPIAnalyticsFinishPurchaseWithProductId(resultCode, productID, receipt, gameFieldsJson);

      Dictionary<string, object> gameFields =  Json.Deserialize(gameFieldsJson) as Dictionary<string,object>;

      if (Analytics.KONG_PURCHASE_FAIL.Equals(resultCode) || Analytics.KONG_PURCHASE_RECEIPT_FAIL.Equals(resultCode)) {
          base.iapFails("", null, gameFields);
      } else if (Analytics.KONG_PURCHASE_SUCCESS.Equals(resultCode)) {
          base.iapTransaction(productID, null, gameFields, "", receipt);
      }
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
      base.Start();

      Boolean autoCollect = KongregateAPI.Settings.AutoCollectDeviceToken;
      if (autoCollect) {
          DDNA.Instance.IosNotifications.RegisterForPushNotifications();
      }
    }

    protected override void AddEvent(string collection, string jsonMap, string commonPropsJson) {
      _KongregateAPIAnalyticsAddEvent(collection, jsonMap, commonPropsJson);

      string kongProps = _KongregateAPIAnalyticsGetKongProperties();
      base.AddEvent(collection, jsonMap, commonPropsJson, kongProps);
    }

    protected override void UpdateCommonProps(string mapJson) {
      _KongregateAPIAnalyticsUpdateCommonProps(mapJson);
    }

    protected override void TrackPurchase(string productID, int quantity, String fieldsJSON) {
      _KongregateAPIAnalyticsTrackPurchase(productID, quantity, fieldsJSON);
    }

    protected void FinishPurchase(string resultCode, string responseInfo,
                                  Dictionary<string, object> transactionFields,
                                  Dictionary<string, object> gameFields,
                                  string dataSignture) {

        Dictionary<string, object> iapFields = new Dictionary<string, object>();
    		Debug.LogWarning("IAP FLOW STEP: finishPurchase(): " + resultCode);
        // check for case where we do not have a transaction id (Unity)
        if (Analytics.KONG_PURCHASE_FAIL.Equals(resultCode)) {
          object failTransactionId;
          transactionFields.TryGetValue("transactionId", out failTransactionId);
          if (failTransactionId != null) {
              object failReason;
              transactionFields.TryGetValue("failReason", out failReason);

              iapFields.Add(Analytics.RECEIPT_ID, failTransactionId.ToString());

              base.iapFails(failReason.ToString(), iapFields, gameFields);
          }
          return;
        }

           object state;
           if (transactionFields.TryGetValue("SKPaymentTransactionStateRestored", out state) ||
               transactionFields.TryGetValue("SKPaymentTransactionStatePurchasing", out state)) {
               return;
           }

           object transactionIdentifier;
           transactionFields.TryGetValue("transactionId", out transactionIdentifier);
           iapFields.Add(Analytics.RECEIPT_ID, transactionIdentifier);

           // fire the approriate event given the transaction state and purchase code.
           if (Analytics.KONG_PURCHASE_SUCCESS.Equals(resultCode)) {
              if (transactionFields.TryGetValue("SKPaymentTransactionStatePurchased", out state)) {
                   // fire iap_transcation event
                   object productId;
                   transactionFields.TryGetValue("productId", out productId);

                   base.iapTransaction(productId.ToString(), iapFields, gameFields, null, dataSignture);
               }
           } else if (Analytics.KONG_PURCHASE_FAIL.Equals(resultCode) || Analytics.KONG_PURCHASE_RECEIPT_FAIL.Equals(resultCode)) {
               if (transactionFields.TryGetValue("SKPaymentTransactionStateFailed", out state)) {
                   // fire iap_fail event with the error code from the payment transaction as reason
                   object failReason;
                   transactionFields.TryGetValue("productId", out failReason);

                   base.iapFails(failReason.ToString(), iapFields, gameFields);
               }
           }
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
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIAnalyticsGetKongProperties();
    [DllImport ("__Internal")]
    private static extern string _KongregateAPIAnalyticsGetTransactionDetails(string transactionId, string resultCode);

  }
#endif

#if UNITY_ANDROID
internal class AndroidAnalytics : UnityAnalytics {
  private IAndroidWrapper androidApi;

  public AndroidAnalytics(KongregateAPI api, IAndroidWrapper androidApi) : base(api) {
    this.androidApi = androidApi;
    if (!KongregateAPI.Settings.DeferAnalytics) {
      Start();
    }
  }

  public override void SetAutoEventListener(string gameObjectName, string functionName) {
    androidApi.Call("KongregateAPIAnalyticsSetAutoEventListener", gameObjectName, functionName);
  }

  public override void AddFilterType(string filterType) {
    androidApi.Call("KongregateAPIAnalyticsAddFilterType", filterType);
  }

  public override string GetInstallReferrer() {
    return androidApi.Call<string>("KongregateAPIAnalyticsGetInstallReferrer");
  }

  public override string GetAutoPropertiesJSON() {
    return androidApi.Call<string>("KongregateAPIAnalyticsGetAutoPropertiesJSON");
  }

  public override long GetAutoLongProperty(string field) {
    return androidApi.Call<long>("KongregateAPIAnalyticsGetAutoLongProperty", field);
  }

  public override string GetAutoStringProperty(string field) {
    return androidApi.Call<string>("KongregateAPIAnalyticsGetAutoStringProperty", field);
  }

  public override bool GetAutoBoolProperty(string field) {
    return androidApi.Call<bool>("KongregateAPIAnalyticsGetAutoBoolProperty", field);
  }

  public override double GetAutoDoubleProperty(string field) {
    return androidApi.Call<double>("KongregateAPIAnalyticsGetAutoDoubleProperty", field);
  }

  public override int GetAutoIntProperty(string field) {
    return androidApi.Call<int>("KongregateAPIAnalyticsGetAutoIntProperty", field);
  }

  public override string GetAutoUTCProperty(string field) {
    return androidApi.Call<string>("KongregateAPIAnalyticsGetAutoUTCProperty", field);
  }

  public override void StartPurchase(string productID, int quantity, string gameFieldsJson) {
    if (quantity != 1) {
      api.LogWarn("Android doesn't support purchase of more than 1 instance of a productID. Tracking a single attempt.");
    }
    UpdateCommonProps();
    androidApi.Call("KongregateAPIAnalyticsStartPurchase", productID, quantity, gameFieldsJson);

    Dictionary<string, object> gameFields =  Json.Deserialize(gameFieldsJson) as Dictionary<string,object>;
    base.addIAPEvent(Analytics.EVENT_IAP_ATTEMPTS, productID, gameFields, null, null, null);
  }

  public override void FinishPurchase(string resultCode, string transactionId, string gameFieldsJson, string dataSignature) {
    UpdateCommonProps();
    androidApi.Call("KongregateAPIAnalyticsFinishPurchase", resultCode, transactionId, gameFieldsJson, dataSignature);

    Dictionary<string, object> gameFields =  Json.Deserialize(gameFieldsJson) as Dictionary<string,object>;
    IabResultType result = (IabResultType)Enum.Parse(typeof(IabResultType), resultCode);
    FinishPurchase(result, transactionId, gameFields, dataSignature);
  }

  public override string GetResourceNames() {
    return androidApi.Call<string>("KongregateAPIAnalyticsGetResourceNames");
  }

  public override string GetResourceAsString(string resourceId, string attributeId, string defValue) {
    return androidApi.Call<string>("KongregateAPIAnalyticsGetResourceAsString", resourceId, attributeId, defValue);
  }

  public override int GetResourceAsInt(string resourceId, string attributeId, int defValue) {
    return androidApi.Call<int>("KongregateAPIAnalyticsGetResourceAsInt", resourceId, attributeId, defValue);
  }

  public override float GetResourceAsFloat(string resourceId, string attributeId, float defValue) {
    return androidApi.Call<float>("KongregateAPIAnalyticsGetResourceAsFloat", resourceId, attributeId, defValue);
  }

  public override bool GetResourceAsBool(string resourceId, string attributeId, bool defValue) {
    return androidApi.Call<bool>("KongregateAPIAnalyticsGetResourceAsBool", resourceId, attributeId, defValue);
  }

  public override void GameUserUpdate(string propsJSON) {
    androidApi.Call("KongregateAPIAnalyticsGameUserUpdate", propsJSON);
  }

  public override void FinishPromoAward(string promoId) {
    androidApi.Call("KongregateAPIAnalyticsFinishPromoAward", promoId);
  }

  public override void Start() {
    androidApi.WithActivity((activity) => {
      androidApi.Call("KongregateAPIAnalyticsStart", activity);
    });
    base.Start();

    Boolean autoCollect = KongregateAPI.Settings.AutoCollectDeviceToken;
    if (autoCollect) {
        DDNA.Instance.AndroidNotifications.RegisterForPushNotifications();
    }
  }

  protected override void AddEvent(string collection, string jsonMap, string commonPropsJson) {
    androidApi.Call("KongregateAPIAnalyticsAddEvent", collection, jsonMap, commonPropsJson);

    string kongProps = androidApi.Call<string>("KongregateAPIAnalyticsGetKongProperties");
    base.AddEvent(collection, jsonMap, commonPropsJson, kongProps);
  }

  protected override void UpdateCommonProps(string mapJson) {
    androidApi.Call("KongregateAPIAnalyticsUpdateCommonProps", mapJson);
  }

  protected override void TrackPurchase(string productID, int quantity, String fieldsJSON) {
    if (quantity != 1) {
      api.LogWarn("Android doesn't support purchase of more than 1 instance of a productID. Tracking a single purchase.");
    }
    androidApi.Call("KongregateAPIAnalyticsTrackPurchase", productID, fieldsJSON);
  }

  protected void FinishPurchase(IabResultType resultCode, string responseInfo, Dictionary<string, object> gameFields, string dataSignture) {
  		Dictionary<string, object> iapFields = new Dictionary<string, object>();
  		Debug.LogWarning("IAP FLOW STEP: finishPurchase(): " + resultCode);

      // parse json info if success or receipt fail. when value is fail, response info will just be
      // an error message string.
      Dictionary<string, object> purchaseInfoJson = null;
      string resultReason = null;
      if (IabResultType.SUCCESS.Equals(resultCode) || IabResultType.RECEIPT_FAIL.Equals(resultCode)) {
          purchaseInfoJson = Json.Deserialize(responseInfo) as Dictionary<string,object>;
          string orderId = optStringWarn(purchaseInfoJson, "orderId", "", "unable to parse orderId from responseInfo in finishPurchase()");
          iapFields.Add(Analytics.RECEIPT_ID, orderId);
      } else {
          // receipt ids aren't available for failed purchases
  	      iapFields.Add(Analytics.RECEIPT_ID, null);
      }

      // fire appropriate event based on result
      if (IabResultType.SUCCESS.Equals(resultCode)) {
          String productId = optStringWarn(purchaseInfoJson, "productId", "",
                  "unable to parse productId from responseInfo in finishPurchase()");
  			  if (!String.IsNullOrEmpty(resultReason)) {
                  // use to see how many success do to bad connections come in (hopefully, it'll stay low)
  			       iapFields.Add(Analytics.SUCCESS_REASON, resultReason);
          }
          base.iapTransaction(productId, iapFields, gameFields, responseInfo, dataSignture);
      } else if (IabResultType.RECEIPT_FAIL.Equals(resultCode)) {
          base.iapFails(resultReason, iapFields, gameFields);
      } else if (IabResultType.FAIL.Equals(resultCode)) {
          base.iapFails(responseInfo, iapFields, gameFields);
      } else {
  			  Debug.LogWarning("invalid result code passed to finishPurchase: " + resultCode);
      }
    }
}
#endif

internal class UnityAnalytics : Analytics {

  public UnityAnalytics(KongregateAPI api) : base(api) {
  }

  public override void Start() {
    DDNA.Instance.StartSDK(new Configuration() {
          environmentKeyDev = KongregateAPI.Settings.DeltaEnvironmentKey,
          environmentKeyLive = KongregateAPI.Settings.DeltaEnvironmentKey,
          collectUrl = KongregateAPI.Settings.DeltaCollectUrl,
          engageUrl = KongregateAPI.Settings.DeltaEngageUrl,
          useApplicationVersion = true
      });
  }

  protected void AddEvent(string collection, string jsonMap, string commonPropsJson, string kongPropsJSON) {
    Dictionary<string, object> fields =  Json.Deserialize(jsonMap) as Dictionary<string,object>;
    Dictionary<string, object> commonFields = Json.Deserialize(commonPropsJson) as Dictionary<string,object>;
    Dictionary<string, object> kongFields = Json.Deserialize(kongPropsJSON) as Dictionary<string,object>;

    if (commonFields != null) {
		    foreach (string key in commonFields.Keys) {
			       fields.Add (key, commonFields [key]);
		    }
	  }
	  if (kongFields != null) {
		    foreach (string key in kongFields.Keys) {
			       fields.Add (key, kongFields [key]);
		    }
	  }

    AddDeltaEvent(collection, fields);
  }

  private void AddDeltaEvent(string collection, Dictionary<string, object> fields) {
    if (!DDNA.Instance.HasStarted) {
      Debug.LogWarning("Attempting to record " + collection + " event before starting DeltaDNA.");
      return;
    }

    if (collection.StartsWith(KongregateAPI.KONGREGATE_SWRVE_PREFIX) ||
      collection.StartsWith(KongregateAPI.KONGREGATE_DELTA_PREFIX)) {
        Debug.Log("Sending Delta only event: " + collection);
        String qualifiedEventName = collection.Replace(KongregateAPI.KONGREGATE_SWRVE_PREFIX, "");
        qualifiedEventName = qualifiedEventName.Replace(KongregateAPI.KONGREGATE_DELTA_PREFIX, "");
        collection = qualifiedEventName;
    }

    String eventName = KongDeltaUtils.eventNameToDeltaName(collection);
    if (KongDeltaUtils.deltaEventFilter(eventName) /*|| KongDeltaUtils.deltaRemoteFilterList(eventName)*/) {
        Debug.Log("Delta Filtered Event: " + collection);
        return;
    }

    Dictionary<string, object> eventPayload = KongDeltaUtils.fieldsToDeltaFields(fields, eventName);

    DDNA.Instance.RecordEvent(eventName, eventPayload)
      .Add(new GameParametersHandler(gameParameters => {
        if (_paramListener != null)
          _paramListener(gameParameters);
      }))
      .Add(new ImageMessageHandler(DDNA.Instance, imageMessage => {
      Debug.Log("Delta Image Message Received");
      // the image message is already prepared so it will show instantly
      imageMessage.Show();
      imageMessage.OnAction += eventArgs => {
        if (_buttonListener != null)
          _buttonListener(eventArgs.ID, eventArgs.ActionType, eventArgs.ActionValue);
      };
      }))
      .Run();
  }

  protected void addIAPEvent(string collection, string productId,
                       Dictionary<string,object> gameFields,
                       Dictionary<string,object> iapFields,
                       string receipt,
                       string receiptSignature) {

          Dictionary<string,object> eventMap = new Dictionary<string,object>();
          double usdCost = KongDeltaUtils.parseItemPrice(productId);
          eventMap.Add(Analytics.USD_COST, usdCost);
          /* Ignore Local Price for now
          ProductInfoCache.Product product = mProductInfoCache.getProductInfo(productId);
  		    eventMap.Add(Analytics.LOCAL_CURRENCY_COST, product != null ? product.getLocalCost() : null);
  		    eventMap.Add(Analytics.LOCAL_CURRENCY_TYPE, product != null ? product.getLocalCurrency() : null);
          */
  		    eventMap.Add(Analytics.PRODUCT_ID, productId);
          if (gameFields != null) {
  			       foreach (string key in gameFields.Keys) {
  				           eventMap.Add (key, gameFields [key]);
  			       }
          }
          if (iapFields != null) {
  			       foreach (string key in iapFields.Keys) {
  				           eventMap.Add (key, iapFields [key]);
  			       }
          }
          AddDeltaEvent(collection, eventMap);

          if (Analytics.EVENT_IAP_TRANSACTIONS.Equals(collection)) {
              iapEvent(productId, usdCost, eventMap, receipt, receiptSignature);
        			Debug.Log("IAP FLOW STEP: completed: " + collection);
          }
      }

      public enum IabResultType {
    		SUCCESS,
    		FAIL,
    		RECEIPT_FAIL
    	};

	protected void iapTransaction(string productID, Dictionary<string,object> iapFields,
			Dictionary<string,object> gameFields, string data, string dataSignature) {

			if (iapFields != null || (gameFields != null && gameFields.Count > 0)) {
          Debug.Log("iapTransaction: " + productID);
          if (iapFields != null) {
	             iapFields.Add(Analytics.RECEIPT_DATA, data);
	             iapFields.Add(Analytics.RECEIPT_SIGNATURE, dataSignature);
          }
          addIAPEvent(EVENT_IAP_TRANSACTIONS, productID, iapFields, gameFields, data, dataSignature);
      }
    }

	protected void iapFails(string failReason, Dictionary<string,object> iapFields, Dictionary<string,object> gameFields) {
		    Dictionary<string,object> eventMap = new Dictionary<string,object>(iapFields);
		    eventMap.Add(Analytics.FAIL_REASON, failReason);
        if (gameFields != null) {
			       foreach (string key in gameFields.Keys) {
				           eventMap.Add (key, gameFields [key]);
			       }
        }
        AddDeltaEvent(EVENT_IAP_FAILS, eventMap);
		    Debug.LogWarning("IAP FLOW STEP: completed: " + EVENT_IAP_FAILS + " : " + failReason);
  }

  private static string[] VIRTUAL_CURRENCY_FIELDS = new string[] {
    "soft_currency_change",
    "hard_currency_change"
  };

  private void iapEvent(String productId, double price, Dictionary<string,object> eventMap,
                           String receipt, String receiptSignature) {

          Product rewards = new Product();
          rewards.AddItem(productId, productId, 1);
          foreach (string field in VIRTUAL_CURRENCY_FIELDS) {
  			       object val;
  			       eventMap.TryGetValue(field, out val);
  			       long num;
  			       if (long.TryParse(val.ToString(), out num)) {
  				           rewards.AddVirtualCurrency(field, field, num);
              }
          }
  		    if (eventMap.ContainsKey("type")) {
            object type;
            eventMap.TryGetValue("type", out type);
            if (type != null) {
              rewards.AddItem(type.ToString(), type.ToString(), 1);
            }
          }

          Product productSpent = new Product();
  		    productSpent.SetRealCurrency("USD", (int)price);

  		    Transaction ddnaEvent = new Transaction("transaction", "transaction", rewards, productSpent);
  		    ddnaEvent.SetProductId(productId);
  		    if (!String.IsNullOrEmpty(receipt)) {
  			       ddnaEvent.SetReceipt(receipt);
          }
  		    DDNA.Instance.RecordEvent(ddnaEvent);
      }

      protected static String optStringWarn(Dictionary<string, object> json, string key, string defaultValue, string warningMessage) {
    		if (json == null || json.Keys.Count == 0) {
    			Debug.LogWarning("key " + key + " not found in null or empty json object: " + warningMessage);
    			return defaultValue;
    		}
    		object result;
    		json.TryGetValue(key, out result);
    		if (result == null) {
    			Debug.LogWarning("key " + key + " not found in json object: " + warningMessage);
    			return defaultValue;
    		} else {
    			return result.ToString();
    		}
    	}

}
}
