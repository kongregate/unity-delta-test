using UnityEngine;
using System.Collections;

namespace Kongregate
{
	public class KongUserStatsInventoryDemo : MonoBehaviour
	{

		string _username = "Guest";
		long _userId;
		bool _hasPlus;
		string _gameAuthToken;
		bool _inventory;
		bool _hasGun;
		string _statId = "";
		string _statValue = "";

		// Use this for initialization
		void Start ()
		{
			KongregateManager.eventUserChanged += OnUserChanged;
			KongregateManager.eventGameAuthChanged += OnGameAuthChanged;
			KongregateManager.eventUserInventory += OnUserInventory;
			KongregateManager.eventReady += () => {
				// request the user's items
				KongregateAPI.GetAPI().Mtx.RequestUserItemList();
			};
		}

		// Update is called once per frame
		void OnGUI ()
		{
			KongDemoHelper.PrepareGUI();

			KongregateAPI kongregate = KongregateAPI.GetAPI();

			GUI.Label(new Rect(1000, 10, 200, 20), _username + (_hasPlus ? " (K+)" : "") + " (" + _userId + ")", KongDemoHelper.labelStyle);

			if(GUI.Button (new Rect(1080, 40, 100, 40), "Inventory", KongDemoHelper.buttonStyle)) {
				_inventory = false;
				kongregate.Mtx.RequestUserItemList();
			}

			if(_inventory) {
				GUI.Label(new Rect(1000, 120, 250, 80), "Has AWESOME GUN: " + _hasGun, KongDemoHelper.labelStyle);
			} else {
				GUI.Label(new Rect(1000, 120, 250, 80), "Requesting inventory...", KongDemoHelper.labelStyle);
			}

			//  a simple button to demonstrate submitting stats
			if(GUI.Button (new Rect (Screen.width - 200,150,150,40), "Submit Win", KongDemoHelper.buttonStyle)) {
				kongregate.Stats.Submit("Wins", 1);
			}

			GUI.Label(new Rect(1000, 220, 50, 40), "Stat:", KongDemoHelper.labelStyle);
			_statId = GUI.TextField(new Rect(1120, 200, 80, 40), _statId);
			GUI.Label(new Rect(1000, 260, 100, 40), "Value:", KongDemoHelper.labelStyle);
			_statValue = GUI.TextField(new Rect(1120, 250, 80, 40), _statValue);
			if(GUI.Button (new Rect(1120, 300, 80, 40), "Submit", KongDemoHelper.buttonStyle)) {
				long value = 0;
				if (long.TryParse (_statValue, out value)) {
					kongregate.Stats.Submit(_statId, long.Parse(_statValue));
				}
			}
		}

		void OnUserChanged() {
			Debug.Log ("OnUserChange");
			// update user details
			KongregateAPI kongregate = KongregateAPI.GetAPI();
			if (kongregate.Services.IsGuest()) {
				_username = "Guest";
			} else {
				_username = kongregate.Services.GetUsername();
			}
			_userId = kongregate.Services.GetUserId();
			_hasPlus = kongregate.Services.HasKongPlus();
		}

		void OnGameAuthChanged() {
			// use the auth token to valid the user using our REST API
			_gameAuthToken = KongregateAPI.GetAPI().Services.GetGameAuthToken();
			Debug.Log ("OnGameAuthToken: " + _gameAuthToken);
		}

		void OnUserInventory() {
			_inventory = true;
			_hasGun = KongregateAPI.GetAPI().Mtx.HasItem("promo-gun");
			Debug.Log("Kongregate inventory received, has gun: " + _hasGun);
		}
	}
}
