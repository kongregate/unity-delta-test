using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if PRIME31_STOREKIT
using Prime31;
#endif

public class KongregateGameObject : MonoBehaviour {

  string mAuthToken;
  long mUserId;
  string mUsername = null;
  bool mHasKongPlus = false;
  bool mGuest;
  bool mRecentPurchase;
  bool mPurchaseReady;
  bool mHasGun = false;
  bool mInventory = false;
  bool mKongregateWindowOpen = false;
  string mCustomStatId = "Score";
  string mCustomStatValue = "10";
  int mCustomAnalyticValue = 1;
  Dictionary<string,object> mTransactionsInVerification;
  System.Diagnostics.Stopwatch mPlayTimer;
  Texture2D mKongButtonTexture = null;
  Texture2D mNotificationCountTexture = null;
  string mTargetIdText = "";
  string[] mDeepLinkTargets = new string[] {
    Kongregate.Mobile.TARGET_MESSAGES,
    Kongregate.Mobile.TARGET_MORE_GAMES,
    Kongregate.Mobile.TARGET_HIGH_SCORES,
    Kongregate.Mobile.TARGET_FORUMS,
    Kongregate.Mobile.TARGET_SUPPORT,
    Kongregate.Mobile.TARGET_OFFERS,
    Kongregate.Mobile.TARGET_REGISTRATION,
    Kongregate.Mobile.TARGET_TERMS,
    Kongregate.Mobile.TARGET_PRIVACY,
    Kongregate.Mobile.TARGET_TOPICS,
    Kongregate.Mobile.TARGET_GUILD_CHAT
  };
  GameObject steamGameObject;
  float targetWidth = 1280.0f;
  float targetHeight = 720.04f;

  void OnEnable () {
    // Enable Web API integration
    KongregateAPI.Settings.WebEnabled = true;

    KongregateAPI.Settings.Debug = true;
    KongregateAPI.Settings.ButtonCalcScreenCoordinates = true; // if the SDK should handle recalculating the button coordinates for different screen sizes (based on 1024/768)
    KongregateAPI.Settings.ApiDomain = GetAPIDomain(); //this should default to m.kongregate.com, you can remove this line all together

    // You can use the following flag to enable/disable automatic starting of the
    // analytics system. False is the default, which means that the analytics
    // system is started automatically. If you set this to true, you need to
    // manually call KongregateAPI.Analytics.Start() after Initializing the SDK.
    // KongregateAPI.Settings.DeferAnalytics = true;

    // Optional to support swrve analytics tracking
    KongregateAPI.Settings.SwrveAppId = GetSwrveAppId();
    KongregateAPI.Settings.SwrveApiKey = GetSwrveApiKey();

    // You may override SWRVE config options like so
	  //  KongregateAPI.Settings.SwrveConfig = new Dictionary<string,object>() {
		// 	{ KongregateAPI.KONGREGATE_SWRVE_AUTO_DOWNLOAD, false },
		// 	{ KongregateAPI.KONGREGATE_SWRVE_LANGUAGE, "gb" },
		// 	{ KongregateAPI.KONGREGATE_SWRVE_APP_STORE, "amazon" }
		// };
    KongregateAPI.Settings.SwrveConfig = new Dictionary<string,object>() {
      { KongregateAPI.KONGREGATE_SWRVE_SENDER_ID, "648571852960" }
    };

    // Initialize Adjust Event Token map
    KongregateAPI.Settings.AdjustAppToken = GetAdjustAppToken();
    KongregateAPI.Settings.AdjustEnvironment = "sandbox"; // be sure to set to "production" for release builds
    KongregateAPI.Settings.AdjustEventTokenMap = GetAdjustEventTokenMap();

    // Optionally filter analytics events and properties
    // KongregateAPI.Settings.AutoAnalyticsFilter = Kongregate.Analytics.FIELD_IP_ADDRESS + "," + Kongregate.Analytics.EVENT_SESSION_START;

    // Other optional settings
    // KongregateAPI.Settings.WindowPausesSound = true;
    // KongregateAPI.Settings.WindowPausesUnity = false;
    // KongregateAPI.Settings.GuildChat = true;
    KongregateAPI.Settings.CrashlyticsLogging = true;
    KongregateAPI.Settings.CrashlyticsUserKeys = true;

    // Uncomment any panel events your game supports.
    KongregateAPI.Settings.SupportedPanelEvents = new string[] {
      // Kongregate.Mobile.PANEL_EVENT_GO_TO_GUILDS
    };

    // Uncomment to specify a custom panel transition animation
    // KongregateAPI.Settings.DefaultPanelTransition = Kongregate.Mobile.PANEL_TRANSITION_SLIDE_FROM_LEFT;

    // Steam integration is provided by the SteamWorks.NET project.
    // We conditionally use it here to provide an example of how it works without
    // breaking projects that don't require Steam integration.
#if UNITY_STANDALONE && STEAMWORKS
    KongregateAPI.Settings.StandaloneEnabled = true;
    KongregateAPI.Settings.BundleID = "com.kongregate.mobile.angrybots.steam";
    KongregateAPI.Settings.AppVersion = "1.0.0";
    steamGameObject = new GameObject("SteamGameObject");
    steamGameObject.AddComponent(typeof(SteamManager));
#endif

    // by default, the API will be enabled on platforms that it supports
    // when using the API on platforms that it doesn't support, it will simply ignore calls to it to fail silently
    // therefore you can keep calls throughout your code in place such as 'kongregate.Stats.Submit("Wins", 1);'
    //
    // if for some reason you do not want Kongregate API to run on a platform that it does support, you can set
    // the following flag to false to have it behave as if it did not support it:
    //
    // KongregateAPI.Enabled = false;

    KongregateAPI kongregate = KongregateAPI.Initialize(GetGameId(), GetAPIKey());
    kongregate.SetEventBundleListener(gameObject, (string eventName, string eventJSON) => {
      Debug.Log ("Kongregate API Event Bundle name: " + eventName + ", params: " + eventJSON);
      HandleKongregateEvent(eventName);
      if (KongregateAPI.KONGREGATE_EVENT_PROMO_AWARD.Equals(eventName)) {
        Debug.Log("Award Promo Stuff: " + eventJSON);
        // TODO: Parse the promoId from the JSON and pass to the finishPromoAward method
        // Example includes are hard coded promoId so we don't add a dependency to a specific
        // JSON parser.
        kongregate.Analytics.FinishPromoAward("tpt_angry_promo");
      }
    });

    // Optional: set a listener to be notified when automatic analytic events are fired. Most games
    // won't need this. It's useful if you'd like to echo the analytics to your own system.
    kongregate.Analytics.SetAutoEventListener(gameObject, (string eventName, string fieldsJson) => {
      Debug.Log ("Kongregate API auto event name: " + eventName);
    });

    // set the common properties evalutor callback
    // TODO: replace the fields below with information specific to your game
    kongregate.Analytics.SetCommonPropsCallback(() => {
      Dictionary<string,object> commonProps = new Dictionary<string, object>()
      {
        { "game_username", "joe" },
        { "server_version", "1.0.0.0" },
        { "custom_val", ++mCustomAnalyticValue}
        // ...
      };
      return commonProps;
    });

    // Manually start the analytics subsystem. Has no effect if DeferAnalytics is false (default)
    //kongregate.Analytics.Start();

    // You can start adding analytic events after initializing our SDK. You
    // do not need to wait for KONGREGATE_EVENT_READY. If deferred analytics
    // is enabled, you can start adding analytic events after starting the
    // Analytics subsystem.
    // kongregate.Analytics.AddEvent("pre_ready_event",
    //   new Dictionary<string,object>() {{ "stub_field", 1}});

    //configure up our button
    kongregate.Mobile.ButtonSetNativeRendering(false); // true by default, false means render in Unity rather than native
    if (kongregate.Mobile.ButtonIsNativeRendering()) {
      // Set position and show button. Only needed when using native rendering.
      kongregate.Mobile.ButtonSetX(10);
      kongregate.Mobile.ButtonSetY(10);
      kongregate.Mobile.ButtonSetSize(48);
      kongregate.Mobile.ButtonShow();
      // native rendering does not support the notificaiton count icon.
    } else {
      // Not using native rendering, so we load assets from here /Assets/Plugins/Kongregate/Resources and display the
      // button programmatically along with a notification icon. You may do the same, move the asset into the scene,
      // or manage however you wish. Be sure to also render the notification icon (see below).
      mKongButtonTexture = (Texture2D) Resources.Load("Kongregate/kongregate_button", typeof(Texture2D));
    }

    // this method demonstrates how to use Swrve in-app messages
    DemoSetupInAppMessageHandle();

    // this method demonstrates how to use our Analytics API with Prime31s IAP plugins
    DemoSetupPurchases();
  }

  void OnGUI () {
    //write your GUI elements for 1280x720
    GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(Screen.width / targetWidth, Screen.height / targetHeight, 1));

    KongregateAPI kongregate = KongregateAPI.GetAPI();

    // isReady() is true if we are on iOS platform and the API is initalized properly
    if (kongregate.IsReady()) {
#if UNITY_STANDALONE
      if(GUI.Button (new Rect (targetWidth/2 - 170 - 5,10,80,40), "Reset Data")) {
        Kongregate.DataStore dataStore = Kongregate.DataStore.Get("analytics");
        string playerId = dataStore.GetString("player_id", "");
        Debug.Log("PlayerID was: " + playerId);
        dataStore.DeleteAll();
        if(!string.IsNullOrEmpty(playerId)) {
          dataStore.SetString("player_id", playerId);
        }
      }
#endif

      // add a simple button to demonstrate hidding and showing the K button
      if(GUI.Button (new Rect (targetWidth/2 - 80 - 5,10,80,40), "Toggle K")) {
        if (kongregate.Mobile.ButtonIsHidden()) {
          kongregate.Mobile.ButtonShow();
        } else {
          kongregate.Mobile.ButtonHide();
        }
      }

      // add a simple button to demonstrate submitting stats
      if(GUI.Button (new Rect (targetWidth/2 + 5,10,80,40), "Submit Win")) {
        kongregate.Stats.Submit("Wins", 1);
      }

      Dictionary<string,object> fields = new Dictionary<string,object>()
      {
        { "type", "Gold Pack" },
        { "discount_percent", 12.5 },
        { "context_of_offer", "StoreFront"}
      };

      if ( mPurchaseReady ) {
        if(GUI.Button (new Rect (targetWidth/2 - 170 -5,60,80,40), "Purchase")) {
          mPurchaseReady = false; // disable button when clicked
#if UNITY_IPHONE && PRIME31_STOREKIT
          kongregate.Analytics.StartPurchase( "com.kongregate.mobile.games.angryBots.t05_coins", 1, fields );
          StoreKitBinding.purchaseProduct( "com.kongregate.mobile.games.angryBots.t05_coins", 1 );
#elif UNITY_ANDROID && PRIME31_STOREKIT
          kongregate.Analytics.StartPurchase("com.kongregate.android.games.angrybots.t03_hard", 1, fields);
          GoogleIAB.purchaseProduct( "com.kongregate.android.games.angrybots.t03_hard" );
#elif UNITY_WEBPLAYER || UNITY_WEBGL
          kongregate.Analytics.StartPurchase("com.kongregate.web.angrybots.t05_hard", 1, fields);

          // Use the Javascript API to initiate the purchase. Notice in this flow we call finishPurchase via
          // the Javascript API after the purchase is completed by serializing the game fields needed. You
          // could also save the game fields into a variable in your Unity script and then call
          // kongregate.Analytics.FinishPurchase(...) in the Unity callback as well.
          Application.ExternalEval(string.Format(
            @"kongregate.mtx.purchaseItems(['com.kongregate.web.angrybots.t05_hard'], function(result){{
                var status = result.success ? 'SUCCESS' : 'FAIL';
                var data = result.success ? '{0}' : '{1}';
                kongregate.analytics.finishPurchase(status, result.purchase_id, data);

                // Fire the callback in the Unity code
                kongregateUnitySupport.getUnityObject().SendMessage('Kongregate', 'OnKredPurchaseResult', status);
            }});"
          , Kongregate.Analytics.toAnalyticEventJSON(getPurchaseFields()), Kongregate.Analytics.toAnalyticEventJSON(fields)));
#elif UNITY_STANDALONE
          kongregate.Analytics.StartPurchase("com.kongregate.mobile.games.angryBots.t05_hard", 1, fields);
#endif
        }

#if UNITY_IPHONE && PRIME31_STOREKIT
        if(GUI.Button (new Rect (targetWidth/2 + 5,60,80,40), "Restore")) {
          StoreKitBinding.restoreCompletedTransactions();
        }
#endif
        if (mRecentPurchase) {
          GUI.Box (new Rect (targetWidth/2 - 70,targetHeight/2 - 30,140,60), "Purchase Successful!");
          if (GUI.Button (new Rect (targetWidth/2 - 20,targetHeight/2,40,20), "OK")) {
            mRecentPurchase = false;
          }
        }
      }

#if UNITY_STANDALONE
      if(!mPurchaseReady) {
        if(GUI.Button (new Rect (targetWidth/2 - 170 -5,60,80,40), "Finish")) {
          kongregate.Analytics.FinishPurchase("SUCCESS", null, getPurchaseFields());
          mPurchaseReady = true;
        }

        if(GUI.Button (new Rect (targetWidth/2 - 80 -5,60,80,40), "Cancel")) {
          kongregate.Analytics.FinishPurchase("FAIL", null, fields);
          mPurchaseReady = true;
        }
      }
#endif

      // add text fields to demonstrate submitting generic stat types
      GUI.Label(new Rect(targetWidth/2 + 95, 10, 50, 20), "Stat:");
      mCustomStatId = GUI.TextField(new Rect(targetWidth/2 + 140, 10, 80, 20), mCustomStatId);
      GUI.Label(new Rect(targetWidth/2 + 95, 35, 50, 20), "Value:");
      mCustomStatValue = GUI.TextField(new Rect(targetWidth/2 + 140, 35, 80, 20), mCustomStatValue);
      if(GUI.Button (new Rect(targetWidth/2 + 230, 10, 80, 40), "Submit")) {
        long value = 0;
        if (long.TryParse (mCustomStatValue, out value)) {
              kongregate.Stats.Submit(mCustomStatId, long.Parse(mCustomStatValue));
        }
      }

      if(GUI.Button (new Rect(targetWidth/2 + 430, 10, 100, 40), "Inventory")) {
        mInventory = false;
        kongregate.Mtx.RequestUserItemList();
      }

      GUI.Box(new Rect(5, 105, 200, 60), "");

      if(mInventory) {
        GUI.Label(new Rect(10, 110, 200, 20), "Has AWESOME GUN: " + mHasGun);
      } else {
        GUI.Label(new Rect(10, 110, 200, 20), "Requesting inventory...");
      }

      if (mUsername != null) {
        GUI.Label(new Rect(10,125,200,20), "Username: " + mUsername + ", id: " + mUserId);
        GUI.Label(new Rect(10,140,200,20), "KongPlus: " + mHasKongPlus);
      }

      if(GUI.Button (new Rect(targetWidth/2 + 535, 10, 100, 40), "FBPost")) {
        Dictionary<string,object> parameters = new Dictionary<string,object>()
        {
          { "link", "http://www.kongregate.com" },
          { "name", "AngryBots" },
          { "caption", "Testing 123" },
          { "description", "Test Post"}
        };
#if UNITY_ANDROID && PRIME31_SOCIAL
        FacebookAndroid.showFacebookShareDialog(parameters);
#elif UNITY_IPHONE && PRIME31_SOCIAL
        FacebookBinding.showFacebookShareDialog(parameters);
#endif
      }

      if(GUI.Button (new Rect(targetWidth/2 + 535, 60, 100, 40), "More Games")) {
        kongregate.Mobile.OpenKongregateWindow(Kongregate.Mobile.TARGET_MORE_GAMES);
      }
      if(GUI.Button (new Rect(targetWidth/2 + 535, 110, 100, 40), "High Scores")) {
        kongregate.Mobile.OpenKongregateWindow(Kongregate.Mobile.TARGET_HIGH_SCORES);
      }
      if(GUI.Button (new Rect(targetWidth/2 + 535, 160, 100, 40), "Support")) {
        kongregate.Mobile.OpenKongregateWindow(Kongregate.Mobile.TARGET_SUPPORT);
      }
      if(GUI.Button (new Rect(targetWidth/2 + 535, 210, 100, 40), "Forums")) {
        kongregate.Mobile.OpenKongregateWindow(Kongregate.Mobile.TARGET_FORUMS);
      }

      // add a simple button to demonstrate submitting analytics
      mPlayTimer = new System.Diagnostics.Stopwatch();
      mPlayTimer.Start();
      if(GUI.Button (new Rect(targetWidth/2 + 330, 10, 80, 40), "Play Ends")) {
        DemoAddAnalyticsEvent();
      }

      // if our button is not hidden, we need to draw it
      Rect kButtonRect = new Rect(10,10,100,100);
      if (kongregate.Mobile.ButtonIsNativeRendering()) {
        //nothing to do, rendering is handled in native SDK
        // NOTE: notification count is not supported by native rendering
      } else if(!kongregate.Mobile.ButtonIsHidden()) {
        // draw the K button
        if (GUI.Button(kButtonRect, mKongButtonTexture)) {
          Debug.Log ("You clicked the Kong button!");
          kongregate.Mobile.OpenKongregateWindow();
        }
        if (mNotificationCountTexture) {
          GUI.Label(new Rect(90,20,50,50), mNotificationCountTexture);
        }
      }

      // add buttons for the deep links
      Rect deepLinkButtonsRect = new Rect(kButtonRect.x, kButtonRect.y + kButtonRect.height + 60, 200, 400);
      Rect targetIdLabelRect = new Rect(deepLinkButtonsRect.x, deepLinkButtonsRect.y + deepLinkButtonsRect.height + 20, 100, 50);
      Rect targetIdTextRect = new Rect(targetIdLabelRect.x + targetIdLabelRect.width, targetIdLabelRect.y, 100, 50);
      GUI.Label(targetIdLabelRect, "TargetId:");
      mTargetIdText = GUI.TextField(targetIdTextRect, mTargetIdText);
      int deepLinkClick = GUI.SelectionGrid(deepLinkButtonsRect, -1, mDeepLinkTargets, 2);
      if (deepLinkClick >= 0) {
        KongregateAPI.GetAPI().Mobile.OpenKongregateWindow(mDeepLinkTargets[deepLinkClick], mTargetIdText);
      }

    }// IsReady
  }// OnGUI

  void UpdateNotificationCount() {
    // Draw the notification icon, if needed. Not currently supported by native rendering
    int notificationCount = KongregateAPI.GetAPI().Services.GetNotificationCount();
    if (notificationCount > 0) {
      mNotificationCountTexture = (Texture2D) Resources.Load ("Kongregate/notification_" + Math.Min(notificationCount,9), typeof(Texture2D));
    } else {
      mNotificationCountTexture = null;
    }

    Debug.Log("has unread guild messages: " + KongregateAPI.GetAPI().Services.HasUnreadGuildMessages());
  }


  // Demonstrate how to setup  a listener for Swrve in-app messages
  void DemoSetupInAppMessageHandle() {

    Debug.Log("setting swrve button listener");
    KongregateAPI.GetAPI().Analytics.SetSwrveButtonListener("Kongregate", "HandleSwrveAction"); //GameObjectName, methodName)
  }

  void HandleSwrveAction(String action) {
    Debug.Log("swrve action: " + action);
  }

  // Demonstrate how to add analytic events
  void DemoAddAnalyticsEvent() {

    KongregateAPI kongregate = KongregateAPI.GetAPI();

    // send a Swrve specific custom event
    mPlayTimer.Stop();
    long playTime = mPlayTimer.ElapsedMilliseconds / 1000;
    mPlayTimer.Reset();
    System.Random random = new System.Random();
    Dictionary<string,object> swrveMap = new Dictionary<string,object>()
    {
      { "play_time", playTime },
      { "bullets",  random.Next(1000) },
      { "kills",  random.Next(100) },
      { "floor", random.Next(10) }
    };

    // send a Swrve only event
    kongregate.Analytics.AddEvent("swrve.game_plays", swrveMap);

    // send an adjust only event
    kongregate.Analytics.AddEvent("adjust.nnclzg", (string) null);

    Dictionary<string,object> purchasePayload = new Dictionary<string,object>()
    {
      { Kongregate.Analytics.SWRVE_VIRTUAL_ECONOMY_PARAM_ITEM, "an_item" },
      { Kongregate.Analytics.SWRVE_VIRTUAL_ECONOMY_PARAM_CURRENCY, "hard_currency_change" },
      { Kongregate.Analytics.SWRVE_VIRTUAL_ECONOMY_PARAM_COST, 100 },
      { Kongregate.Analytics.SWRVE_VIRTUAL_ECONOMY_PARAM_QUANTITY, 5 }
    };
    kongregate.Analytics.AddEvent(Kongregate.Analytics.SWRVE_VIRTUAL_ECONOMY_EVENT_PURCHASE, purchasePayload);

    Dictionary<string,object> giftPayload = new Dictionary<string,object>()
    {
      { Kongregate.Analytics.SWRVE_VIRTUAL_ECONOMY_PARAM_CURRENCY, "soft_currency_change" },
      { Kongregate.Analytics.SWRVE_VIRTUAL_ECONOMY_PARAM_AMOUNT, 4.99 }
    };
    kongregate.Analytics.AddEvent(Kongregate.Analytics.SWRVE_VIRTUAL_ECONOMY_EVENT_GIFT, giftPayload);


    // a complex event map demonstrating nested objects and arrays
    Dictionary<string,object> eventMap = new Dictionary<string,object>()
    {
      { "test_number_25", 54 },
      { "test_bool_true", true },
      { "test_bool_false", false },
      { "test_str_array", new object[] { "Demon Card", "Goblin Card", "Angel Card" } },
    };
    kongregate.Analytics.AddEvent("play_ends", eventMap);

    // You may access analytic field values as well
    string osVersion = kongregate.Analytics.GetAutoStringProperty(Kongregate.Analytics.FIELD_CLIENT_OS_VERSION);
    Debug.Log("Client OS Version is: " + osVersion);
    long kongUid = kongregate.Analytics.GetAutoLongProperty(Kongregate.Analytics.FIELD_KONG_USER_ID);
    Debug.Log("Kongregate UID is: " + kongUid);
    bool fromBackground = kongregate.Analytics.GetAutoBoolProperty(Kongregate.Analytics.FIELD_IS_FROM_BACKGROUND);
    Debug.Log("Is from background: " + fromBackground);
    string firstPlay = kongregate.Analytics.GetAutoUTCProperty(Kongregate.Analytics.FIELD_FIRST_PLAY_TIME);
    Debug.Log("First play time: " + firstPlay);

    // You may ge the install referrer, like so, if needed (Android only)
    Debug.Log("Install Referrer is: " + kongregate.Analytics.GetInstallReferrer());
  }

  // Demonstrate how you might setup purchases using the Prime31 Plugin, and notify our SDK of the results.
  void DemoSetupPurchases() {
    mTransactionsInVerification = new Dictionary<string,object>(); //queue of transactions needing to be verified
#if UNITY_IPHONE && PRIME31_STOREKIT
    StoreKitManager.productListReceivedEvent += allProducts => {
      Debug.Log( "received total products: " + allProducts.Count );
      mPurchaseReady = true;
    };

    // the following handles receipt verification
    StoreKitManager.autoConfirmTransactions=false; //don't autoconfirm transactions until we have verified the receipt
    StoreKitManager.productPurchaseAwaitingConfirmationEvent += transaction => {
      mTransactionsInVerification[transaction.transactionIdentifier] = transaction; //add this transaction to the queue
      KongregateAPI.GetAPI().Mtx.VerifyTransactionId(transaction.transactionIdentifier);
    };

    StoreKitManager.purchaseFailedEvent += errorMsg => {
      Debug.Log("purchase failed: " + errorMsg);
      // pass null, if you do not have the transaction id.
      KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_FAIL, null,
        getPurchaseFields());
      mPurchaseReady = true;
    };

    StoreKitManager.purchaseCancelledEvent += errorMsg => {
      Debug.Log("purchase cancelled: " + errorMsg);
      // pass null, if you do not have the transaction id.
      KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_FAIL, null,
        getPurchaseFields());
      mPurchaseReady = true;
    };

    StoreKitBinding.requestProductData( new string[] { "com.kongregate.mobile.games.angryBots.t05_coins" } );
#elif UNITY_ANDROID && PRIME31_STOREKIT
    GoogleIABManager.billingSupportedEvent += () => {
      Debug.Log ("billing is supported");
      mPurchaseReady = true;
      var skus = new string[] { "com.kongregate.android.games.angrybots.t03_hard" };
      GoogleIAB.queryInventory( skus );
    };
    GoogleIABManager.queryInventorySucceededEvent += (purchases, skus) => {
      foreach(GooglePurchase purchase in purchases) {
        verifyGoogleReceipt(purchase);
      }
    };
    GoogleIABManager.purchaseSucceededEvent += purchase => {
      Debug.Log( "Google IAB callback - purchased product: " + purchase);
      verifyGoogleReceipt(purchase);
    };
    GoogleIABManager.purchaseFailedEvent += message => {
      Debug.Log("Google IAB callback - purchase failed: " + message);
      KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_FAIL,
        message, getPurchaseFields());
      mPurchaseReady = true;
    };
    GoogleIABManager.consumePurchaseSucceededEvent += purchase => {
      Debug.Log("Google IAB callback - consume succeeded: " + purchase);
    };
    GoogleIABManager.consumePurchaseFailedEvent += message => {
      Debug.Log("Google IAB callback - consume failed: " + message);
    };
    GoogleIAB.init( GetGooglePublicKey() );
#else
    mPurchaseReady = true;
#endif
  }

#if UNITY_ANDROID && PRIME31_STOREKIT
  void verifyGoogleReceipt(GooglePurchase purchase) {
    // Sandbox purchases may have a blank orderId
    mTransactionsInVerification[purchase.orderId == null ? string.Empty : purchase.orderId] = purchase;
    KongregateAPI.GetAPI().Mtx.VerifyGoogleReceipt(purchase.originalJson, purchase.signature);
  }
#elif UNITY_WEBPLAYER || UNITY_WEBGL
  void OnKredPurchaseResult(string result) {
    Debug.Log("Kred Purchase Result: " + result);
    mPurchaseReady = true;
    // This is when you would inform your server to check the player's Kongregate inventory if successful
  }
#endif

  Dictionary<string,object> getPurchaseFields() {
    Dictionary<string,object> fields = new Dictionary<string,object>()
    {
      { "hard_currency_change",  5 },
      { "soft_currency_change", 10 },
      { "type", "Gold Pack" },
      { "discount_percent", 12.5 },
      { "context_of_offer","StoreFront"},
      { "resources_change", new object[] { "rank=1", "type=win" } },
    };
    return fields;
  }

  void OnApplicationPause(bool pausing) {
    Debug.Log("OnApplicationPause: " + pausing);
    KongregateAPI kongregate = KongregateAPI.GetAPI();
    if (pausing) {
      kongregate.OnPause();
    } else {
      kongregate.OnResume();
      if (mKongregateWindowOpen) {
#if UNITY_IPHONE
        // Android closes the panel when the app goes to the background, so iOS only.
        Debug.Log("Kongregate Panel is still open, game should remain paused");
        Time.timeScale = 0;
        AudioListener.pause = true;
#endif
      }
    }
  }


  // Demonstarte how to handle Kongregate Events
  void HandleKongregateEvent(string eventName) {
    Debug.Log("HandleKongregateEvent: " + eventName);
    KongregateAPI kongregate = KongregateAPI.GetAPI();
    switch(eventName) {
      case KongregateAPI.KONGREGATE_EVENT_READY:
        // Kongregate API is ready to go
        // You may now access the current Kongregate User, submit stats, etc.
        // You may also check to see if the game auth token is available. If this is the first time
        // the user lauched your game, they may not have an AuthToken yet. A
        // KONGEGATE_EVENT_GAME_AUTH_CHANGED will follow (if they have an internet connection). You only
        // need the Game Auth Token, if you plan to perform server-side verification.
        Debug.Log("Kongregate API is ready");
        Debug.Log("Kongregate Auto props as JSON: " + kongregate.Analytics.GetAutoPropertiesJSON());
        UpdateNotificationCount();

        // If you're implementing guild chat, you can update your character token at this time
        // if it is available:
        // kongregate.Services.SetCharacterToken("server-generated-token");
        DemoRequestUserItems();
        DemoUpdateGameUserData();
        Debug.Log("app opened using deep link: " + kongregate.Mobile.GetOpenURL());
        break;
      case KongregateAPI.KONGREGATE_EVENT_USER_CHANGED:
        // this could be one of:
        // 1) user was a guest and then logged in/registered
        // 2) user was logged in and then logged out and is now guest
        // 3) user was logged in and then logged out and is now a different user
        // Note that the user id will come back for Guests as well. To check for a
        // guest, use the IsGuest() method.
        // if you are also concerned about the Auth Token, you should instead listen
        // for KONGREGATE_EVENT_USER_AUTH_CHANGED as it is a superset of this event
        mUserId = kongregate.Services.GetUserId();
        mUsername = kongregate.Services.GetUsername();
        mGuest = kongregate.Services.IsGuest();
        mHasKongPlus = kongregate.Services.HasKongPlus();
        Debug.Log("user "+ mUserId+" "+(mGuest ? "is" : "is not")+" a guest");
        DemoRequestUserItems();
        break;
      case KongregateAPI.KONGREGATE_EVENT_GAME_AUTH_CHANGED:
        // User's game auth token has changed, either by way of login, logout, or if their password changes.
        // This event will always follow KONGREGATE_EVENT_USER_CHANGED (if they have intenet access).
        // Note that Guests will also have an Auth Token and will cause this to event to fire as well.
        // Guest token is the same for all Guests. you can check if the user is a Guest using the
        // isGuest() method. Their is no need to perform server-side verification for the Guests token.
        // If you want to listen for a single event that covers all the scenarios of a user change or
        // an auth token change, this is the event to listen for
        mUserId = kongregate.Services.GetUserId();
        mAuthToken = kongregate.Services.GetGameAuthToken();
        mGuest = kongregate.Services.IsGuest();
        Debug.Log("user "+ mUserId+" "+(mGuest ? "is" : "is not")+" a guest");
        Debug.Log("game auth has changed to:"+mAuthToken);
        break;
      case KongregateAPI.KONGREGATE_EVENT_OPEN_DEEP_LINK:
        Debug.Log("app opened using deep link: " + kongregate.Mobile.GetOpenURL());
        break;
      case KongregateAPI.KONGREGATE_EVENT_OPENING:
        // Kongregate Window is opening
        Debug.Log("Kongregate Window is opening - pausing logic and sound");
        mKongregateWindowOpen = true;
#if UNITY_IPHONE
        // Android closes the panel when the app goes to the background, so iOS only.
        Time.timeScale = 0;
        AudioListener.pause = true;
#endif
        break;
      case KongregateAPI.KONGREGATE_EVENT_CLOSED:
        // Kongregate Window is clsoed
        Debug.Log("Kongregate Window is closed - resuming logic and sound");
        mKongregateWindowOpen = false;
#if UNITY_IPHONE
        // Android closes the panel when the app goes to the background, so iOS only.
        Time.timeScale = 1;
        AudioListener.pause = false;
#endif
        break;
      case KongregateAPI.KONGREGATE_EVENT_USER_INVENTORY:
        // User inventory is now available, we can call hasItem to check for promotional items
        mInventory = true;
        DemoGetUserItems();
        break;
      case KongregateAPI.KONGREGATE_EVENT_SERVICE_UNAVAILABLE:
        // A request to Kongregate failed. This will typically occur when the user does not
        // have a network connection or Kongregate is experiencing a scheduled downtime. Requests
        // will be stored locally until the service becomes available again. Game clients
        // should only need to handle this event if they require server side auth,
        // do not have a game auth token yet, and wish to message to the user that they are unable
        // to validate them.
        Debug.Log("Kongregate Service Unavailable");
        break;
      case KongregateAPI.KONGREGATE_EVENT_RECEIPT_VERIFICATION_COMPLETE:
        // A purchase receipt has been verified, we can now check the result for our transaction
        Debug.Log("A receipt has finished verification");
        // we keep a list of transactions here so that we don't run into race conditions
        Dictionary<string,object> stillInVerification = new Dictionary<string,object>();
        foreach(var transactionId in mTransactionsInVerification.Keys) {
          string status = kongregate.Mtx.ReceiptVerificationStatus(transactionId);
          if (status != KongregateAPI.RECEIPT_VERIFICATION_STATUS_PROCESSING) {
            Debug.Log("Verification for " + transactionId + " is "+ status);
#if UNITY_IPHONE && PRIME31_STOREKIT
            if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_VALID) {
              StoreKitTransaction transaction = (StoreKitTransaction)mTransactionsInVerification[transactionId];
              KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_SUCCESS,
                transactionId, getPurchaseFields());

              // Below is an alternative iOS FinishPurchase() call. This version may be used if your plugin
              // does not make the transaction Id available or finishes the transaction before passing control
              // back to Unity. FinishPurchase() is preferred because purchase details may be pulled directly from
              // the transaction.

              // KongregateAPI.GetAPI().Analytics.FinishPurchaseWithProductId(KongregateAPI.PURCHASE_SUCCESS,
              //  transaction.productIdentifier, transaction.base64EncodedTransactionReceipt, getPurchaseFields());
            } else if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_INVALID) {
              KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_RECEIPT_FAIL,
                transactionId, getPurchaseFields());
            }
            // call to finish the transaction regardless of it we are valid or not
            StoreKitBinding.finishPendingTransaction(transactionId);
#elif UNITY_ANDROID && PRIME31_STOREKIT
            GooglePurchase transaction = (GooglePurchase)mTransactionsInVerification[transactionId];
            if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_VALID) {
              KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_SUCCESS,
                transaction.originalJson, getPurchaseFields(), transaction.signature);
            } else if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_INVALID) {
              KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_RECEIPT_FAIL,
                transaction.originalJson, getPurchaseFields());
            }
            // call to finish transaction regardless of it we are valid or not
            GoogleIAB.consumeProduct(transaction.productId);
#endif
            mPurchaseReady = true;
          } else {
            Debug.Log("Still waiting on verification for " + transactionId);
            stillInVerification[transactionId] = mTransactionsInVerification[transactionId];
          }
        }
        mTransactionsInVerification = stillInVerification;
        break;
      case KongregateAPI.KONGREGATE_EVENT_SWRVE_RESOURCES_UPDATES:
        DemoSwrveResources();
        break;
      case KongregateAPI.KONGREGATE_EVENT_NOTIFICATION_COUNT_UPDATED:
        Debug.Log("notification count updated: " + kongregate.Services.GetNotificationCount());
        UpdateNotificationCount();
        break;
    }
    // the following should be called to let the API know processing is complete
    // this is particularly important for when pausing the game via iOS (default)
    kongregate.MessageReceived(eventName);
  }

  // Demonstrate how to request the user items
  void DemoRequestUserItems() {
    KongregateAPI.GetAPI().Mtx.RequestUserItemList();
  }

  // Demostrate how to retreive User items after KONGREGATE_EVENT_USER_INVENTORY is retrieved
  void DemoGetUserItems() {
    mHasGun = KongregateAPI.GetAPI().Mtx.HasItem("promo-gun");
    Debug.Log("Kongregate inventory received, has gun: " + mHasGun);
  }

  // Demonstrates how to update game user data in SWRVE.
  void DemoUpdateGameUserData() {

    // you may specify game user data as either a dictonary of string values
    Dictionary<string,object> gameUserProps = new Dictionary<string, object>()
    {
      { "user_prop_1", "one" },
      { "user_prop_2", "two" }
    };
    KongregateAPI.GetAPI().Analytics.GameUserUpdate(gameUserProps);

    // or a json string
    KongregateAPI.GetAPI().Analytics.GameUserUpdate("{ \"json_prop_1\" : \"json_value_1\", \"json_prop_2\" : \"json_value_2\" }");
  }

  // Demonstrate how to retrieve Swrve resources
  void DemoSwrveResources() {
    // the following resources should be configured in teh Swrve dashboard
    Debug.Log("Got Swrve resources: " + KongregateAPI.GetAPI().Analytics.GetResourceNames());
    Debug.Log("abtest string value: " + KongregateAPI.GetAPI().Analytics.GetResourceAsString("test_resource", "string_attr", "n/a"));
    Debug.Log("   abtest int value: " + KongregateAPI.GetAPI().Analytics.GetResourceAsInt("test_resource", "int_attr", 0));
    Debug.Log(" abtest float value: " + KongregateAPI.GetAPI().Analytics.GetResourceAsFloat("test_resource", "float_attr", 0));
    Debug.Log("  abtest bool value: " + KongregateAPI.GetAPI().Analytics.GetResourceAsBool("test_resource", "bool_attr", false));
  }

  // The following uses reflection to find the Game Id from a Constants file,
  // Replace all of this with your GameID
  long GetGameId() {
    System.Type type = System.Type.GetType("KongregateConstants");
    if ( type == null ) {
      return 0;
    }
    return (long)type.GetField("KONGREGATE_GAME_ID").GetValue(null);
  }

  // The following uses reflection to find the API Key from a Constants file,
  // Replace all of this with your API Key
  string GetAPIKey() {
    System.Type type = System.Type.GetType("KongregateConstants");
    if ( type == null ) {
      return "";
    }
    return (string)type.GetField("KONGREGATE_API_KEY").GetValue(null);
  }

  // The following uses reflection to find the API Domain from a Constants file,
  // Replace all of this with your GameID
  string GetAPIDomain() {
    System.Type type = System.Type.GetType("KongregateConstants");
    if ( type == null ) {
      return "m.kongregate.com";
    }
    return (string)type.GetField("KONGREGATE_API_DOMAIN").GetValue(null);
  }

  // The following returns public key for google services. You may use your own
  // for testing. Kongregate will need to provide you with the public key that
  // will be used once we upload it to the Google play store.
  string GetGooglePublicKey() {
    System.Type type = System.Type.GetType("KongregateConstants");
    if ( type == null ) {
      return "your-public-key";
    }
    return (string)type.GetField("KONGREGATE_GOOGLE_PUBLIC_KEY").GetValue(null);
  }

  // use reflection to find the Apple Id from a constant file.
  // Replace with the Apple Id for your game.
  string GetAppleId() {
    System.Type type = System.Type.GetType("KongregateConstants");
    if (type == null) {
      return "";
    }
    return (string)type.GetField("KONGREGATE_APPLE_ID").GetValue (null);
  }

  // use reflection to find the Swrve App Id from a constant file.
  // Replace with the Swrve App Id for your game.
  int GetSwrveAppId() {
    System.Type type = System.Type.GetType("KongregateConstants");
    if (type == null) {
      return 0;
    }
    return (int)type.GetField("KONGREGATE_SWRVE_APP_ID").GetValue (null);
  }

  // use reflection to find the Swrve Api Key from a constant file.
  // Replace with the Swrve API Key for your game.
  string GetSwrveApiKey() {
    System.Type type = System.Type.GetType("KongregateConstants");
    if (type == null) {
      return "";
    }
    return (string)type.GetField("KONGREGATE_SWRVE_API_KEY").GetValue (null);
  }

  // use reflection to find the Adjust App Token from a constant file.
  // Replace with the Adjust App Token for your game.
  string GetAdjustAppToken() {
    System.Type type = System.Type.GetType("KongregateConstants");
    if (type == null) {
      return "";
    }
    return (string)type.GetField("KONGREGATE_ADJUST_APP_TOKEN").GetValue (null);
  }

  // use reflection to find the Adjust event token map from a constant file.
  // Replace with the Adjust event token map for your game.
  // For exampe:
  // return new Dictionary<string,object>() { { KongregateAPI.ADJUST_SALE, "abc123" } };
  Dictionary<string,object> GetAdjustEventTokenMap() {
    System.Type type = System.Type.GetType("KongregateConstants");
    if (type == null) {
      return new Dictionary<string,object>();
    }

    Dictionary<string,object> map = (Dictionary<string,object>)type.GetField("KONGREGATE_ADJUST_EVENT_TOKEN_MAP").GetValue(null);
    if(map == null) {
      map = new Dictionary<string,object>();
    }
    return(map);
  }

}
