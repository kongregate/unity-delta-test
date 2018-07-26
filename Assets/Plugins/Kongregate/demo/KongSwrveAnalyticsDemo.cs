using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kongregate
{
	public class KongSwrveAnalyticsDemo : MonoBehaviour
	{

		// Use this for initialization
		void Awake ()
		{
      		KongregateAPI kongregate = KongregateAPI.GetAPI();
			if (kongregate != null) {
				SetupAnalyticsCallbacks();
			} else {
				KongregateManager.eventInitializing += SetupAnalyticsCallbacks;
			}

			KongregateManager.eventSwrveResourceUpdate += OnSwrveResourcesUpdated;
		}

		void SetupAnalyticsCallbacks() {
			// The following listeners can only be set after Initialize is invoked, though it is not necessary
			// to wait for the ready event.
			KongregateAPI kongregate = KongregateAPI.GetAPI();

			// Set the callback that will include fields sent with all events
			kongregate.Analytics.SetCommonPropsCallback(CommonPropsCallback);

			// NOTE: if deferred analytics is enabled, you may only SetSwrveButtonListener after Analytics.Start()
			// is invoked
			kongregate.Analytics.SetSwrveButtonListener(gameObject.name, "HandleSwrveAction");
		}

		void OnGUI() {
			KongregateAPI kongregate = KongregateAPI.GetAPI();
			if (!kongregate.IsReady()) {
				// don't display purchase UI until Kongregate is ready
				return;
			}

			KongDemoHelper.PrepareGUI();

			if(GUI.Button (new Rect(1280/2 - 150, 150, 300, 40), "Send Custom play_ends Events", KongDemoHelper.buttonStyle)) {
				SendCustomEvents();
			}
		}

		Dictionary<string,object> CommonPropsCallback ()
		{
			Debug.Log ("common props callback invoked");
			return new Dictionary<string,object> () {
				{ "game_username", "fred" },
				{ "server_version", "1.2.3" },
#if UNITY_STANDALONE
				{ "client_version", "4.5.6" }
#endif
			};
		}

		void HandleSwrveAction(string action) {
			Debug.Log ("HandleSwrveAction: " + action);

			// implement the custom Swrve action handler. You may implement this to direct the user to the in
			// game store or some other screen you wish to promote through in-app messages.
		}

		void SendCustomEvents() {

			KongregateAPI kongregate = KongregateAPI.GetAPI();

			// Send a custom event. Event will be sent through Swrve as Kongregate.RawData.play_ends and
			// extracted through Kongregate's ETL system. Since the event is also sent to Swrve, it may
			// also be used for Swrve features such as in-app messages and A/B tests.
			Dictionary<string,object> eventMap = new Dictionary<string,object>()
			{
				{ "play_id", "550e8400-e29b-41d4-a716-446655440000" },
				{ "pve_energy_used", 24 },
				{ "is_pvp", true },
				{ "resources_change", new string[] { "cards +1", "PvE energy +20" } },
			};
			kongregate.Analytics.AddEvent("play_ends", eventMap);

			// Sends a custom event to Swrve only. It will not go through Kongregate ETL, but may be used
			// for Swrve features such as in-app messages and A/B tests.
			Dictionary<string,object> swrveEventMap = new Dictionary<string,object>()
			{
				{ "play_time", 60 },
				{ "mode", "Mission" }
			};
			kongregate.Analytics.AddEvent("swrve.game_over", swrveEventMap);

			// Update Swrve user properties which may be used to create user segments to
			// target A/B tests, in-app messages, and Push Notifications. The Kongregate SDK
			// automatically updates a set of user properties, you may use this method to add
			// additional custom user properties.
			Dictionary<string,object> customUserProps = new Dictionary<string,object>
			{
				{ "level", 4 },
				{ "type", "theif" }
			};
			kongregate.Analytics.GameUserUpdate(customUserProps);
		}

		/**
		 * Handle the KONGREGATE_EVENT_SWRVE_RESOURCES_UPDATES update which is fired by the Swrve SDK when
		 * resources setup for A/B testing are ready to be loaded.
		 */
		void OnSwrveResourcesUpdated ()
		{
			// A handful of accessors are supported to retrieve resources and values
			Debug.Log ("Got Swrve resources: " + KongregateAPI.GetAPI ().Analytics.GetResourceNames ());
			Debug.Log ("abtest string value: " + KongregateAPI.GetAPI ().Analytics.GetResourceAsString ("test_resource", "string_attr", "n/a"));
			Debug.Log ("   abtest int value: " + KongregateAPI.GetAPI ().Analytics.GetResourceAsInt ("test_resource", "int_attr", 0));
			Debug.Log (" abtest float value: " + KongregateAPI.GetAPI ().Analytics.GetResourceAsFloat ("test_resource", "float_attr", 0));
			Debug.Log ("  abtest bool value: " + KongregateAPI.GetAPI ().Analytics.GetResourceAsBool ("test_resource", "bool_attr", false));
		}
	}
}
