namespace Kongregate {
  using System;
  using System.Reflection;
  using Steamworks;
  using UnityEngine;

  /**
   * A Steam SDK Adapter for Steamworks.NET (http://steamworks.github.io/)
   * This file should go into your Assets/Scripts folder if you are using
   * Steamworks.NET for Steam SDK support. It also depends on their SteamManager
   * class, which you should initialize before setting up the Kong SDK.
   */
  public class SteamworksAdapter : ISteamAdapter {
    private const int MAX_TICKET_SIZE = 1024;
    private byte[] pendingAuthTicket;
    private HAuthTicket authTicketHandle;
    private Callback<GetAuthSessionTicketResponse_t> sessionTicketCallback;
    private AuthSessionTicketDelegate sessionTicketDelegate;

    public SteamworksAdapter() {
      sessionTicketCallback = Callback<GetAuthSessionTicketResponse_t>.Create(OnAuthSessionTicketResponse);
      Debug.Log("Kongregate: SteamworksAdapter startup, initialized=" + Initialized);
    }

    public virtual bool Initialized {
      get {
        return SteamManager.Initialized;
      }
    }

    public virtual string SteamID {
      get {
        if(!Initialized) return null;
        return SteamUser.GetSteamID().ToString();
      }
    }

    public virtual bool OverlayEnabled {
      get {
        if(!Initialized) return false;
        return SteamUtils.IsOverlayEnabled();
      }
    }

    public virtual string PersonaName {
      get {
        if(!Initialized) return null;
        var result = SteamFriends.GetPersonaName();
        return result == null ? null : result.ToString();
      }
    }

    public virtual void ActivateGameOverlayToWebPage(string url) {
      if(!Initialized) return;
      SteamFriends.ActivateGameOverlayToWebPage(url);
    }

    public virtual void GetAuthSessionTicket(AuthSessionTicketDelegate callback) {
      if(!Initialized) {
        Debug.Log("Kongregate: Ignoring GetAuthSessionTicket call, SteamManager is not initialized");
        return;
      }

      sessionTicketDelegate = callback;
      var ticket = new byte[MAX_TICKET_SIZE];
      uint length = 0;
      authTicketHandle = SteamUser.GetAuthSessionTicket(ticket, MAX_TICKET_SIZE, out length);
      pendingAuthTicket = new byte[length];
      Array.Copy(ticket, 0, pendingAuthTicket, 0, length);
      Debug.Log("Pending AuthTicket handle: " + authTicketHandle + ", length: " + length);
    }

    private void OnAuthSessionTicketResponse(GetAuthSessionTicketResponse_t response) {
      if(response.m_hAuthTicket == authTicketHandle) {
        if(sessionTicketDelegate != null) {
          Debug.Log("AuthSessionTicket result: " + response.m_eResult + ", firing callback");
          sessionTicketDelegate(response.m_eResult == EResult.k_EResultOK ? pendingAuthTicket : null);
          pendingAuthTicket = null;
          authTicketHandle = default(HAuthTicket);
        }
      } else {
        Debug.Log("Ignoring AuthSessionTicketResponse we did not request: " + response.m_hAuthTicket);
      }
    }
  }
}
