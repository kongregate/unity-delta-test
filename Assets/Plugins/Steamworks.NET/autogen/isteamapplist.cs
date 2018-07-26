// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2016 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

// This file is automatically generated.
// Changes to this file will be reverted when you update Steamworks.NET

using System;
using System.Runtime.InteropServices;

namespace Steamworks {
	public static class SteamAppList {
		public static uint GetNumInstalledApps() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamAppList_GetNumInstalledApps();
		}

		public static uint GetInstalledApps(AppId_t[] pvecAppID, uint unMaxAppIDs) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamAppList_GetInstalledApps(pvecAppID, unMaxAppIDs);
		}

		/// <summary>
		/// <para> returns -1 if no name was found</para>
		/// </summary>
		public static int GetAppName(AppId_t nAppID, out string pchName, int cchNameMax) {
			InteropHelp.TestIfAvailableClient();
			IntPtr pchName2 = Marshal.AllocHGlobal(cchNameMax);
			int ret = NativeMethods.ISteamAppList_GetAppName(nAppID, pchName2, cchNameMax);
			pchName = ret != -1 ? InteropHelp.PtrToStringUTF8(pchName2) : null;
			Marshal.FreeHGlobal(pchName2);
			return ret;
		}

		/// <summary>
		/// <para> returns -1 if no dir was found</para>
		/// </summary>
		public static int GetAppInstallDir(AppId_t nAppID, out string pchDirectory, int cchNameMax) {
			InteropHelp.TestIfAvailableClient();
			IntPtr pchDirectory2 = Marshal.AllocHGlobal(cchNameMax);
			int ret = NativeMethods.ISteamAppList_GetAppInstallDir(nAppID, pchDirectory2, cchNameMax);
			pchDirectory = ret != -1 ? InteropHelp.PtrToStringUTF8(pchDirectory2) : null;
			Marshal.FreeHGlobal(pchDirectory2);
			return ret;
		}

		/// <summary>
		/// <para> return the buildid of this app, may change at any time based on backend updates to the game</para>
		/// </summary>
		public static int GetAppBuildId(AppId_t nAppID) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamAppList_GetAppBuildId(nAppID);
		}
	}
}