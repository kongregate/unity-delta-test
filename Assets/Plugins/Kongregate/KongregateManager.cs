using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kongregate
{
	/**
	 * Helper class to manage interactions with the KongregateAPI. This class
	 * along with example files included in the /Assets/Plugins/Kongregate/demo directory
	 * effectively replace the KongregateGameObject-Example.cs.
   *
	 * You may use this class in your code by simply attaching the script to a
	 * game object that will persist for the life of your game. You may use
	 * the Unity Inspector to set the various API Keys and IDs required by the
	 * Kongregate API. You may also simply initialize them in script.
	 *
	 * Public variables are provided for the common settings required by most
	 * games. Legacy settings are not included here to keep the API as simple as possible for new games.
   * If you wish to initialize with legacy settings you may extend this class
	 * and override GetLegacySettings or GetAPISettings.
	 *
	 * For detailed examples of how to use this class open KongregateDemoScene.unity.
	 * Note that the KongStoreKitDemo script depends on Prime31s StoreKit plugin. If you
	 * are using a different plugin, you'll need to adapt this script (or simply delete)
	 * to build the demo scene.
	 */
	public class KongregateManager : MonoBehaviour
	{
		/**
		 * Set if this is release candidate build. When set the various production
		 * keys and values will be used. When not set, Sandbox and Test values
		 * are used.
		 */
		public bool rcBuild;

		/** The Kongregate Game ID */
		public long kongregateGameID;

		/** The Kongregate API Key */
		public string kongregateAPIKey;

		/** Kongregate Analytics Test app ID */
		public string kongAnalyticsTestId;

		/** Kongregate Analytics Test API key */
		public string kongAnalyticsTestKey;

		/** Kongregate Analytics Live app ID */
		public string kongAnalyticsLiveId;

		/** Kongregate Analytics Live API key */
		public string kongAnalyticsLiveKey;

        /** See KongregateAPI.KONGREGATE_OPTION_DEBUG */
		public bool debugLogging;

		/** Production Android KongregateAPI.KONGREGATE_OPTION_SWRVE_APP_ID */
		public int androidSwrveLiveAppId;

		/** Production Android KongregateAPI.KONGREGATE_OPTION_SWRVE_API_KEY */
		public string androidSwrveLiveAPIKey;

		/** Test Android KongregateAPI.KONGREGATE_OPTION_SWRVE_APP_ID */
		public int androidSwrveTestAppId;

		/** Test Android KongregateAPI.KONGREGATE_OPTION_SWRVE_API_KEY */
		public string androidSwrveTestAPIKey;

        /** Production Android KongregateAPI.KONGREGATE_OPTION_DELTA_ENVIRONMENT_KEY */
		public string androidDeltaLiveEnvironmentKey;
        
        /** Production Android KongregateAPI.KONGREGATE_OPTION_DELTA_COLLECT_URL */
		public string androidDeltaLiveCollectUrl;
        
        /** Production Android KongregateAPI.KONGREGATE_OPTION_DELTA_ENGAGE_URL */
		public string androidDeltaLiveEngageUrl;
        
        /** Test Android KongregateAPI.KONGREGATE_OPTION_DELTA_ENVIRONMENT_KEY */
		public string androidDeltaTestEnvironmentKey;
        
        /** Test Android KongregateAPI.KONGREGATE_OPTION_DELTA_COLLECT_URL */
		public string androidDeltaTestCollectUrl;
        
        /** Test Android KongregateAPI.KONGREGATE_OPTION_DELTA_ENGAGE_URL */
		public string androidDeltaTestEngageUrl;
        
		/** Android KongregateAPI.KONGREGATE_OPTION_ADJUST_APP_TOKEN */
		public string androidAdjustAppToken;

		/** Android KongregateAPI.KONGREGATE_ADJUST_SALE_EVENT_TOKEN */
		public string androidAdjustSaleToken;

		/** Android KongregateAPI.KONGREGATE_ADJUST_INSTALL_EVENT_TOKEN */
		public string androidAdjustInstallToken;

		/** Android KongregateAPI.KONGREGATE_ADJUST_SESSION_EVENT_TOKEN */
		public string androidAdjustSessionToken;

		/** Production iOS KongregateAPI.KONGREGATE_OPTION_SWRVE_APP_ID */
		public int iOSSwrveLiveAppId;

		/** Production iOS KongregateAPI.KONGREGATE_OPTION_SWRVE_API_KEY */
		public string iOSSwrveLiveAPIKey;

		/** Test iOS KongregateAPI.KONGREGATE_OPTION_SWRVE_APP_ID */
		public int iOSSwrveTestAppId;

		/** Test iOS KongregateAPI.KONGREGATE_OPTION_SWRVE_API_KEY */
		public string iOSSwrveTestAPIKey;

        /** Production iOS KongregateAPI.KONGREGATE_OPTION_DELTA_ENVIRONMENT_KEY */
		public string iOSDeltaLiveEnvironmentKey;
        
        /** Production iOS KongregateAPI.KONGREGATE_OPTION_DELTA_COLLECT_URL */
		public string iOSDeltaLiveCollectUrl;
        
        /** Production iOS KongregateAPI.KONGREGATE_OPTION_DELTA_ENGAGE_URL */
		public string iOSDeltaLiveEngageUrl;
        
        /** Test iOS KongregateAPI.KONGREGATE_OPTION_DELTA_ENVIRONMENT_KEY */
		public string iOSDeltaTestEnvironmentKey;
        
        /** Test iOS KongregateAPI.KONGREGATE_OPTION_DELTA_COLLECT_URL */
		public string iOSDeltaTestCollectUrl;
        
        /** Test iOS KongregateAPI.KONGREGATE_OPTION_DELTA_ENGAGE_URL */
		public string iOSDeltaTestEngageUrl;
        
		/** iOS KongregateAPI.KONGREGATE_OPTION_ADJUST_APP_TOKEN */
		public string iOSAdjustAppToken;

		/** iOS KongregateAPI.KONGREGATE_ADJUST_SALE_EVENT_TOKEN */
		public string iOSAdjustSaleToken;

		/** iOS KongregateAPI.KONGREGATE_ADJUST_INSTALL_EVENT_TOKEN */
		public string iOSAdjustInstallToken;

		/** iOS KongregateAPI.KONGREGATE_ADJUST_SESSION_EVENT_TOKEN */
		public string iOSAdjustSessionToken;

		/** Production Standalone KongregateAPI.KONGREGATE_OPTION_SWRVE_APP_ID */
		public int standaloneSwrveLiveAppId;

		/** Production Standalone KongregateAPI.KONGREGATE_OPTION_SWRVE_API_KEY */
		public string standaloneSwrveLiveAPIKey;

		/** Test Standalone KongregateAPI.KONGREGATE_OPTION_SWRVE_APP_ID */
		public int standaloneSwrveTestAppId;

		/** Test Standalone KongregateAPI.KONGREGATE_OPTION_SWRVE_API_KEY */
		public string standaloneSwrveTestAPIKey;

		/** Set to true to enable support for WebPlayer and WebGL builds */
		public bool webAPIEnabled;

		/** Set to true to enable support for the Standalone player */
		public bool standaloneAPIEnabled;

		/** Set to true to if your game uses the Guild Chat APIs */
		public bool guildChatEnabled;

		// Setup listeners for corresponding kongregate events

		/** Fired when the KongregateManager begins initializing the Kongregate API */
		public static event Action eventInitializing;

		/** See KongregateAPI.KONGREGATE_EVENT_READY */
		public static event Action eventReady;

		/** See KongregateAPI.KONGREGATE_EVENT_USER_CHANGED */
		public static event Action eventUserChanged;

		/** See KongregateAPI.KONGREGATE_EVENT_GAME_AUTH_CHANGED */
		public static event Action eventGameAuthChanged;

		/** See KongregateAPI.KONGREGATE_EVENT_LOGIN_COMPLETE */
		public static event Action eventLoginComplete;

		/** See KongregateAPI.KONGREGATE_EVENT_OPENING */
		public static event Action eventOpenPanel;

		/** See KongregateAPI.KONGREGATE_EVENT_CLOSED */
		public static event Action eventClosePanel;

		/** See KongregateAPI.KONGREGATE_EVENT_USER_INVENTORY */
		public static event Action eventUserInventory;

		/** See KongregateAPI.KONGREGATE_EVENT_SERVICE_UNAVAILABLE */
		public static event Action eventServiceUnavailable;

		/** See KongregateAPI.KONGREGATE_EVENT_RECEIPT_VERIFICATION_COMPLETE */
		public static event Action eventReceiptVerificationComplete;

		/** See KongregateAPI.KONGREGATE_EVENT_SWRVE_RESOURCES_UPDATES */
		public static event Action eventSwrveResourceUpdate;

		/** See KongregateAPI.KONGREGATE_EVENT_NOTIFICATION_COUNT_UPDATED */
		public static event Action eventNotificationCountUpdate;

		/**
		 * Event fired through the Kongregate Panel. Since the panel is hosted
		 * on Kongregate.com, events of this type may be added outside of SDK
		 * updates and do not have corresponding constants defined in our API
		 * package.
		 */
		public static event Action<string,string> eventPanelMessage;

		/**
		 * The KongregateAPI will be initialized when the associated game object
		 * is enabled.
		 */
		void OnEnable ()
		{
			InitializeKongregate();
		}

		/**
		 * Initialize the Kongregate SDK if it isn't already initialized.
		 */
		void InitializeKongregate() {
			Debug.Log ("KongregateManager.InitializeKongregate");
			if (KongregateAPI.GetAPI() != null) {
				Debug.Log ("KongregateAPI already initialized");
				return;
			}

			ConfigureAPISettings();
			KongregateAPI kongregate = KongregateAPI.Initialize (kongregateGameID, kongregateAPIKey);
			if (eventInitializing != null) {
				// not an official Kongregate API event, but used here to notify other components that initialize
				// has been invoked.
				eventInitializing();
			}
			kongregate.SetEventBundleListener (gameObject, HandleKongregateEvent);
		}

		/**
		 * Override this method if you prefer to set the API settings in code
		 * rather than the Unity inspector, or if you need add legacy
		 * settings not included as inspector variables.
		 */
		protected void ConfigureAPISettings() {

			// Settings shared across platforms
			KongregateAPI.Settings.WebEnabled = webAPIEnabled;
			KongregateAPI.Settings.StandaloneEnabled = standaloneAPIEnabled;
			KongregateAPI.Settings.Debug = debugLogging;
			KongregateAPI.Settings.GuildChat = guildChatEnabled;
			KongregateAPI.Settings.AdjustEnvironment = rcBuild ? "production" : "sandbox";
			KongregateAPI.Settings.KongAnalyticsId = rcBuild ? kongAnalyticsLiveId : kongAnalyticsTestId;
			KongregateAPI.Settings.KongAnalyticsKey = rcBuild ? kongAnalyticsLiveKey : kongAnalyticsTestKey;

			// Environment specific settings below
			#if UNITY_IPHONE
			KongregateAPI.Settings.SwrveAppId = rcBuild ? iOSSwrveLiveAppId : iOSSwrveTestAppId;
			KongregateAPI.Settings.SwrveApiKey = rcBuild ? iOSSwrveLiveAPIKey : iOSSwrveTestAPIKey;
            KongregateAPI.Settings.DeltaEnvironmentKey = rcBuild ? iOSDeltaLiveEnvironmentKey : iOSDeltaTestEnvironmentKey;
            KongregateAPI.Settings.DeltaCollectUrl = rcBuild ? iOSDeltaLiveCollectUrl : iOSDeltaTestCollectUrl;
            KongregateAPI.Settings.DeltaEngageUrl = rcBuild ? iOSDeltaLiveEngageUrl : iOSDeltaTestEngageUrl;
			KongregateAPI.Settings.AdjustAppToken = iOSAdjustAppToken;
			KongregateAPI.Settings.AdjustEventTokenMap = new Dictionary<string,object>() {
				{ KongregateAPI.ADJUST_INSTALL, iOSAdjustInstallToken },
				{ KongregateAPI.ADJUST_SESSION, iOSAdjustSessionToken },
				{ KongregateAPI.ADJUST_SALE, iOSAdjustSaleToken }};
			#elif UNITY_ANDROID
			KongregateAPI.Settings.SwrveAppId = rcBuild ? androidSwrveLiveAppId : androidSwrveTestAppId;
			KongregateAPI.Settings.SwrveApiKey = rcBuild ? androidSwrveLiveAPIKey : androidSwrveTestAPIKey;
            KongregateAPI.Settings.DeltaEnvironmentKey = rcBuild ? androidDeltaLiveEnvironmentKey : androidDeltaTestEnvironmentKey;
            KongregateAPI.Settings.DeltaCollectUrl = rcBuild ? androidDeltaLiveCollectUrl : androidDeltaTestCollectUrl;
            KongregateAPI.Settings.DeltaEngageUrl = rcBuild ? androidDeltaLiveEngageUrl : androidDeltaTestEngageUrl;
			KongregateAPI.Settings.AdjustAppToken = androidAdjustAppToken;
			KongregateAPI.Settings.AdjustEventTokenMap = new Dictionary<string,object>() {
				{ KongregateAPI.ADJUST_INSTALL, androidAdjustInstallToken },
				{ KongregateAPI.ADJUST_SESSION, androidAdjustSessionToken },
				{ KongregateAPI.ADJUST_SALE, androidAdjustSaleToken }};
			#elif UNITY_STANDALONE || UNITY_EDITOR
			KongregateAPI.Settings.SwrveAppId = rcBuild ? standaloneSwrveLiveAppId : standaloneSwrveTestAppId;
			KongregateAPI.Settings.SwrveApiKey = rcBuild ? standaloneSwrveLiveAPIKey : standaloneSwrveTestAPIKey;
			#endif
		}

		/**
		 * Kongregate API Event Listener implemtation that simply forwards
		 * events to corresponding Action listeners.
		 */
		void HandleKongregateEvent (string eventName, string eventJSON)
		{
			Debug.Log ("HandleKongregateEvent: " + eventName + " : " + eventJSON);
			switch (eventName) {
			case KongregateAPI.KONGREGATE_EVENT_READY:
				if (eventReady != null) {
					eventReady ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_USER_CHANGED:
				if (eventUserChanged != null) {
					eventUserChanged ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_GAME_AUTH_CHANGED:
				if (eventGameAuthChanged != null) {
					eventGameAuthChanged ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_LOGIN_COMPLETE:
				if (eventLoginComplete != null) {
					eventLoginComplete ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_OPENING:
				if (eventOpenPanel != null) {
					eventOpenPanel ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_CLOSED:
				if (eventClosePanel != null) {
					eventClosePanel ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_USER_INVENTORY:
				if (eventUserInventory != null) {
					eventUserInventory ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_SERVICE_UNAVAILABLE:
				if (eventServiceUnavailable != null) {
					eventServiceUnavailable ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_RECEIPT_VERIFICATION_COMPLETE:
				if (eventReceiptVerificationComplete != null) {
					eventReceiptVerificationComplete ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_SWRVE_RESOURCES_UPDATES:
				if (eventSwrveResourceUpdate != null) {
					eventSwrveResourceUpdate ();
				}
				break;
			case KongregateAPI.KONGREGATE_EVENT_NOTIFICATION_COUNT_UPDATED:
				if (eventNotificationCountUpdate != null) {
					eventNotificationCountUpdate ();
				}
				break;
			default:
				if (eventPanelMessage != null) {
					eventPanelMessage(eventName, eventJSON);
				}
				break;
			}
		}
	}
}
