//
//  KongregateSDK.h
//  KongregateSDK
//
//  Created by Darcy Brown on 4/17/13.
//  Copyright (c) 2013 Darcy Brown. All rights reserved.
//

#ifndef __IPHONE_8_0
#pragma message "Building with an SDK older than iOS8 is now deprecated. Please upgrade to the latest SDK. "
#endif

#import <Foundation/Foundation.h>
#import <libkern/OSAtomic.h>
#import <StoreKit/StoreKit.h>
#import "KongregateAPI.h"
#import "KongStatServices.h"
#import "KongServices.h"
#import "KongMobile.h"
#import "KongMtx.h"

FOUNDATION_EXPORT NSString* const KONG_API_VERSION;

FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_DOMAIN;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_ANALYTICS_DOMAIN;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_DEBUG;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_ENABLED;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_KEEN_PROJECT_ID;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_KEEN_WRITE_KEY;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_AUTO_ANALYTICS_MODE;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_AUTO_ANALYTICS_FILTER;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_MANAGE_SHARED_CACHE;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_SWRVE_APP_ID;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_SWRVE_API_KEY;

FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_DELTA_ENVIRONMENT_KEY;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_DELTA_COLLECT_URL;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_DELTA_ENGAGE_URL;

FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_KONG_ANALYTICS_ID;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_KONG_ANALYTICS_KEY;

FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_DEBUG_WEBVIEW;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_DEFER_ANALYTICS;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_PERSISTENT_WEBVIEW;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_GUILD_CHAT;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_PANEL_ORIENTATION_OVERRIDE;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_CRASHLYTICS_USER_KEYS;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_SUPPORTED_PANEL_EVENTS;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_DEFAULT_PANEL_TRANSITION;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_CUSTOM_PLAYER_ID;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_DEFER_GDPR_ALERT;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_TEST_GDPR_ALERT;

// deprecated AdX options. No longer used
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_ADX_CLIENT_ID;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_ADX_ID;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_APPLE_ID;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_ADX_UPGRADE;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_ADX_QA_UNTIL;

// Unity only options
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_AUTO_REPOSITION;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_UNITY_PANEL_PAUSE_SOUND;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_UNITY_PANEL_PAUSE_APP;

// These SWRVE options will pass through to the swrve config object when set.
FOUNDATION_EXPORT NSString* const KONGREGATE_SWRVE_PREFIX;
FOUNDATION_EXPORT NSString* const KONGREGATE_SWRVE_AUTO_COLLECT_DEVICE_TOKEN;
FOUNDATION_EXPORT NSString* const KONGREGATE_SWRVE_PUSH_ENABLED;
FOUNDATION_EXPORT NSString* const KONGREGATE_SWRVE_PUSH_NOTIFICATION_EVENTS;
FOUNDATION_EXPORT NSString* const KONGREGATE_SWRVE_TALK_ENABLED;
FOUNDATION_EXPORT NSString* const KONGREGATE_SWRVE_MAX_CONCURRENT_DOWNLOADS;
FOUNDATION_EXPORT NSString* const KONGREGATE_SWRVE_APP_VERSION;
FOUNDATION_EXPORT NSString* const KONGREGATE_SWRVE_LINK_TOKEN;
FOUNDATION_EXPORT NSString* const KONGREGATE_SWRVE_LANGUAGE;

// Adjust options
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_ADJUST_APP_TOKEN;
FOUNDATION_EXPORT NSString* const KONGREGATE_OPTION_ADJUST_ENVIRONMENT;
FOUNDATION_EXPORT NSString* const KONGREGATE_ADJUST_PREFIX;
FOUNDATION_EXPORT NSString* const KONGREGATE_ADJUST_SALE_EVENT_TOKEN;
FOUNDATION_EXPORT NSString* const KONGREGATE_ADJUST_SESSION_EVENT_TOKEN;
FOUNDATION_EXPORT NSString* const KONGREGATE_ADJUST_INSTALL_EVENT_TOKEN;

FOUNDATION_EXPORT int64_t const KONG_GUEST_USER_ID;
FOUNDATION_EXPORT NSString* const KONG_GUEST_GAME_AUTH_TOKEN;

FOUNDATION_EXPORT NSString* const KONG_AUTO_ANALYTICS_MODE_ENABLE_ALL;
FOUNDATION_EXPORT NSString* const KONG_AUTO_ANALYTICS_MODE_DISABLE_ALL;
FOUNDATION_EXPORT NSString* const KONG_AUTO_ANALYTICS_MODE_CLOUD;

FOUNDATION_EXPORT NSString* const KONG_RECEIPT_VERIFICATION_STATUS_UNKNOWN;
FOUNDATION_EXPORT NSString* const KONG_RECEIPT_VERIFICATION_STATUS_PROCESSING;
FOUNDATION_EXPORT NSString* const KONG_RECEIPT_VERIFICATION_STATUS_VALID;
FOUNDATION_EXPORT NSString* const KONG_RECEIPT_VERIFICATION_STATUS_INVALID;

FOUNDATION_EXPORT NSString* const KONG_ORIENTATION_PORTRAIT;
FOUNDATION_EXPORT NSString* const KONG_ORIENTATION_PORTRAIT_SENSOR;
FOUNDATION_EXPORT NSString* const KONG_ORIENTATION_LANDSCAPE;
FOUNDATION_EXPORT NSString* const KONG_ORIENTATION_LANDSCAPE_SENSOR;
