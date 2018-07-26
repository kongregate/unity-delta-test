using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class KongregateConstants : MonoBehaviour {

  // prod
  public static long KONGREGATE_GAME_ID = 180037;
  public static string KONGREGATE_API_KEY = "2ee3bb8c-2629-45aa-b818-9d7aedf5a7ab";
  public static string KONGREGATE_API_DOMAIN = "m.kongregate.com";

  // dev
  // public static long KONGREGATE_GAME_ID = 1000000;
  // public static string KONGREGATE_API_KEY = "c8e1a04a-bb18-47dd-904b-d1dd2696d2bd";
  // public static string KONGREGATE_API_DOMAIN = "m.kongregatedev.com";

  public static string KONGREGATE_GOOGLE_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvtfYyxWC9hwmI+EHMqyQlj6+NqNwbszpBMtnei+OEEJiQWhf28cClOKBw5LBWt+RIRFUhQoeXnYzQxv8JA/eBA4pZ/oLZefz+IgN64spPtKjTR3gQxAJ/12O10CFJNl/jZDVTPzgfI9LMml4FcbCIHw641kXeQPrmRdL2gv8G3YdIFYp8w6UphtFYN+brQ0i+APWqkFwSRmX3q8p9EQqkgE4fYLezfwwQ98O4Gy1SGn9ouUlrlKTB/GYsqL2T7IBieYbXw0wlDA6ilOna2T/xoZnnhmdNeuTkt79Do5nC5WIq2b8knaiuTmszihlKBUS3mmk5dBm/xS1MmTcOr7twQIDAQAB";
  public static string KONGREGATE_KEEN_PROJECT_ID = "523766a000111c5fdb000005";
  public static string KONGREGATE_KEEN_WRITE_KEY = "3aa63169adb3ed1f352f42578568c9c305d84f34be9b881a119c825c576c6330d68f2f6e35c8903aed4f598dfa7ef98de19a5515b89d3e49b68a0ed179f17388f3d2b1181b3bdc550e6062b44073377cbfd8238c80dd1a74539a54a0521bf431cb8f45da871e6658b4c3863e02b61085";
  public static string KONGREGATE_APPLE_ID = "675200899";
  public static string KONGREGATE_ADX_ID = "ADX1010";
  public static string KONGREGATE_ADJUST_APP_TOKEN = "gsaeysxxfred";
#if UNITY_ANDROID
  public static int    KONGREGATE_SWRVE_APP_ID = 1615;
  public static string KONGREGATE_SWRVE_API_KEY = "htpXdmAxrqHnBiQFi9G";
  public static Dictionary<string,object> KONGREGATE_ADJUST_EVENT_TOKEN_MAP = new Dictionary<string,object>()  {
		{ KongregateAPI.ADJUST_SALE, "m08uvk" },
		{ KongregateAPI.ADJUST_SESSION, "nqv0bk" },
		{ KongregateAPI.ADJUST_INSTALL, "srfmix" }
	};
#elif UNITY_IPHONE
  public static int    KONGREGATE_SWRVE_APP_ID = 1837;
  public static string KONGREGATE_SWRVE_API_KEY = "ZDBHruT5fUXm1uxMhXu";
	public static Dictionary<string,object> KONGREGATE_ADJUST_EVENT_TOKEN_MAP = new Dictionary<string,object>()  {
		{ KongregateAPI.ADJUST_SALE, "lule3x" },
		{ KongregateAPI.ADJUST_SESSION, "azjqfw" },
		{ KongregateAPI.ADJUST_INSTALL, "r24t9p" }
	};
#elif UNITY_STANDALONE
  static KongregateConstants() {
    if(File.Exists("kong_domain.txt")) {
      KONGREGATE_API_DOMAIN = File.ReadAllText("kong_domain.txt").Trim();
      Debug.Log("Setting domain to: " + KONGREGATE_API_DOMAIN);
    }
  }

  public static int    KONGREGATE_SWRVE_APP_ID = 3528;
  public static string KONGREGATE_SWRVE_API_KEY = "bCoU1z8EY3lq5MyHCwZ";
  public static Dictionary<string,object> KONGREGATE_ADJUST_EVENT_TOKEN_MAP = new Dictionary<string,object>(){};
#else
  public static int    KONGREGATE_SWRVE_APP_ID = 0;
  public static string KONGREGATE_SWRVE_API_KEY = "";
  public static Dictionary<string,object> KONGREGATE_ADJUST_EVENT_TOKEN_MAP = new Dictionary<string,object>(){};
#endif
}
