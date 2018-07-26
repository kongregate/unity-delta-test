using UnityEngine;
using System;
using System.Collections;

namespace Kongregate
{
	public class KongPanelDemo : MonoBehaviour
	{

		GameObject steamGameObject;
		Texture2D _kongButtonTexture;
		Texture2D _notificationCountTexture;
		bool _unreadGuildChat;
		string _targetIdText = "";
		string[] _deepLinkTargets = new string[] {
			Mobile.TARGET_MESSAGES,
			Mobile.TARGET_MORE_GAMES,
			Mobile.TARGET_HIGH_SCORES,
			Mobile.TARGET_FORUMS,
			Mobile.TARGET_SUPPORT,
			Mobile.TARGET_OFFERS,
			Mobile.TARGET_REGISTRATION,
			Mobile.TARGET_TERMS,
			Mobile.TARGET_PRIVACY,
			Mobile.TARGET_TOPICS
		};

		// Use this for initialization
		void Start ()
		{
			_kongButtonTexture = (Texture2D)Resources.Load ("Kongregate/kongregate_button", typeof(Texture2D));
			KongregateManager.eventReady += OnReady;
			KongregateManager.eventNotificationCountUpdate += OnNotificationCountUpdate;
		}

		void OnEnable () {
			// Steam integration is provided by the SteamWorks.NET project.
			// We conditionally use it here to provide an example of how it works without
			// breaking projects that don't require Steam integration.
#if UNITY_STANDALONE && STEAMWORKS_SDK
			steamGameObject = new GameObject("SteamGameObject");
			steamGameObject.AddComponent(typeof(SteamManager));
#endif
		}

		void OnGUI ()
		{
			KongregateAPI kongregate = KongregateAPI.GetAPI ();
			if (!kongregate.IsReady ()) {
				// don't display purchase UI until Kongregate is ready
				return;
			}

			KongDemoHelper.PrepareGUI();

			// if our button is not hidden, we need to draw it
			Rect kButtonRect = new Rect (10, 10, 100, 100);

			// draw the K button
			if (GUI.Button (kButtonRect, _kongButtonTexture)) {
				Debug.Log ("You clicked the Kong button!");
				kongregate.Mobile.OpenKongregateWindow ();
			}
			if (_notificationCountTexture) {
				GUI.Label (new Rect (90, 20, 50, 50), _notificationCountTexture, KongDemoHelper.labelStyle);
			}

			// add buttons for the deep links
			Rect deepLinkButtonsRect = new Rect (kButtonRect.x, kButtonRect.x + kButtonRect.height + 20, 300, 200);
			Rect targetIdLabelRect = new Rect (deepLinkButtonsRect.x, deepLinkButtonsRect.y + deepLinkButtonsRect.height + 20, 100, 50);
			Rect targetIdTextRect = new Rect (targetIdLabelRect.x + targetIdLabelRect.width, targetIdLabelRect.y, 100, 50);
			GUI.Label (targetIdLabelRect, "TargetId:", KongDemoHelper.labelStyle);
			_targetIdText = GUI.TextField (targetIdTextRect, _targetIdText, KongDemoHelper.textFieldStyle);
			int deepLinkClick = GUI.SelectionGrid (deepLinkButtonsRect, -1, _deepLinkTargets, 2, KongDemoHelper.buttonStyle);
			if (deepLinkClick >= 0) {
				KongregateAPI.GetAPI ().Mobile.OpenKongregateWindow (_deepLinkTargets [deepLinkClick], _targetIdText);
			}

			// add guild chat button
			Rect guildChatRect = new Rect(kButtonRect.x + kButtonRect.width + 20, kButtonRect.y + 20, 200, 50);
			GUIStyle guildChatStyle = new GUIStyle(KongDemoHelper.buttonStyle);
			if (_unreadGuildChat) {
				guildChatStyle.fontStyle = FontStyle.Bold;
				guildChatStyle.normal.textColor = Color.red;
			}
			if (GUI.Button(guildChatRect, "Guild Chat", guildChatStyle)) {
				KongregateAPI.GetAPI().Mobile.OpenKongregateWindow(Mobile.TARGET_GUILD_CHAT);
			}
		}

		void OnReady()
		{
			Debug.Log ("OnReady");
			OnNotificationCountUpdate();

			// If you're implementing guild chat, you can update your character token at this time
			// if it is available:
			// kongregate.Services.SetCharacterToken("server-generated-token");
		}

		void OnNotificationCountUpdate()
		{
			Debug.Log("OnNotificationCountUpdate");

			// Draw the notification icon, if needed. Not currently supported by native rendering
			int notificationCount = KongregateAPI.GetAPI ().Services.GetNotificationCount ();
			if (notificationCount > 0) {
				_notificationCountTexture = (Texture2D)Resources.Load ("Kongregate/notification_" + Math.Min (notificationCount, 9), typeof(Texture2D));
			} else {
				_notificationCountTexture = null;
			}
			_unreadGuildChat = KongregateAPI.GetAPI ().Services.HasUnreadGuildMessages ();
		}
	}
}
