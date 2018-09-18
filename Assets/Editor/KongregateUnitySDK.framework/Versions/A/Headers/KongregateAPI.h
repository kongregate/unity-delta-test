//
// Created by Ben Vinson on 5/7/13.
// Copyright (c) 2013 Kongregate. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//

#define KongNSStringize_helper(x) #x
#define KongNSStringize(x) @KongNSStringize_helper(x)

#import <Foundation/Foundation.h>
#import "KongAnalytics.h"
#import "KongServices.h"
#import "KongStatServices.h"
#import "KongMobile.h"
#import "KongMtx.h"


/**
 * The block to be invoked when a Kongregate API event occurs.
 * @param eventName The name of the event
 * @param eventJSON The JSON data payload for the event or nil if no data is included
 *                  with the event.
 */
typedef void (^KongEventBundleListener)(NSString* event,NSString* jsonBundle);

@class KongMtx;

/**
 * Bundles a Kongregate Event with a JSON payload
 */
@interface KongEventBundle : NSObject
@property (nonatomic,retain) NSString* name;
@property (nonatomic,retain) NSString* jsonBundle;
@end

/**
* The global Kongregate API object, which is the main interface for interacting with the API.
* This header also contains constants for [API Events](/docs/events.html):
*
* * ```NSString* const KONGREGATE_EVENT_READY```
* * ```NSString* const KONGREGATE_EVENT_USER_CHANGED```
* * ```NSString* const KONGREGATE_EVENT_GAME_AUTH_CHANGED```
* * ```NSString* const KONGREGATE_EVENT_LOGIN_COMPLETE```
* * ```NSString* const KONGREGATE_EVENT_OPENING```
* * ```NSString* const KONGREGATE_EVENT_CLOSED```
* * ```NSString* const KONGREGATE_EVENT_USER_INVENTORY```
* * ```NSString* const KONGREGATE_EVENT_RECEIPT_VERIFICATION_COMPLETED```
* * ```NSString* const KONGREGATE_EVENT_SWRVE_RESOURCES_UPDATED```
* * ```NSString* const KONGREGATE_EVENT_SERVICE_UNAVAILABLE```
*/
@interface KongregateAPI : NSObject {
    KongServices* kongServices;
    KongStatServices* kongStats;
    KongMobile* kongMobile;
    KongMtx* kongMtx;
    KongAnalytics* kongAnalytics;
}

FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_USER_CHANGED;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_GAME_AUTH_CHANGED;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_LOGIN_COMPLETE;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_READY;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_OPENING;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_CLOSED;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_USER_INVENTORY;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_RECEIPT_VERIFICATION_COMPLETED;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_SWRVE_RESOURCES_UPDATED;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_NOTIFICATION_COUNT_UPDATED;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_CHARACTER_TOKEN_REQUEST;
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_OPEN_DEEP_LINK;

// Notifies the client that Kongregate is unavailable. This may be due to network connectivity
// or Kongregate could be experiencing a downtime. The client wil retry until the system is
// available.
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_SERVICE_UNAVAILABLE;

/**
 * Broadcast when a promotion event occurs and some item or currency should be awarded.
 * Promotional events occur through the Swrve in-app message system.
 * Parse the JSON in the event bundle to determine the award. The format of the JSON bundle will
 * be: { "promoId": "promo_ebf", "item": "super_punch", "currency": "gold", "amount": 100 }
 *
 * Once the award is complete, invoke {@link AnalyticsServices#finishPromoAward(String)}
 * which will set a Swrve User Property that may be used to prevent the promo from appearing again
 */
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_PROMO_AWARD;

/**
 * KONGREGATE_EVENT_PROMO_AWARD event parameter for the promoID.
 */
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_PARAM_PROMO_ID;

/**
 * KONGREGATE_EVENT_PROMO_AWARD event parameter for the item to award.
 */
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_PARAM_ITEM;

/**
 * KONGREGATE_EVENT_PROMO_AWARD event parameter for the currency type to award.
 */
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_PARAM_CURRENCY;

/**
 * KONGREGATE_EVENT_PROMO_AWARD event parameter for the amount of currency to award.
 */
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_PARAM_AMOUNT;

/**
 * KONGREGATE_EVENT_PROMO_AWARD event parameter for a link to open after giving an award.
 */
FOUNDATION_EXPORT NSString* const KONGREGATE_EVENT_PARAM_INSTALL_URL;

/**
 * KONG_SWRVE_EVENT_PREFIX used to exclude event from Kongregate Analytics ingestion
 * KONG_DELTA_EVENT_PREFIX used to exclude event from Kongregate Analytics ingestion
 */
FOUNDATION_EXPORT NSString* const KONG_SWRVE_EVENT_PREFIX;
FOUNDATION_EXPORT NSString* const KONG_DELTA_EVENT_PREFIX;

/**
 * Set a block to echo Kong Log messages to. Useful for crash reporting
 * tools that upload console logs with reports
 * @param block the block to be invoked when the SDK logs a debug message
 */
+(void)setLogEcho:(void (^) (NSString*)) block;

/**
* Initializes the API with the default settings.
* @param gameId Your game id
* @param apiKey Your mobile API key
*/
+(void)initialize:(int64_t)gameId apiKey:(NSString*)apiKey;

/**
* Initializes the API with custom settings.
* @param gameId Your game id
* @param apiKey Your mobile API key
* @param settings A dictionary with settings for controlling API behavior
*/
+(void)initialize:(int64_t)gameId apiKey:(NSString*)apiKey withSettings:(NSDictionary*)settings;

/**
 * Initializes the API with custom JSON formatted settings.
 * @param gameId Your game id
 * @param apiKey Your mobile API key
 * @param settingsJSON A JSON formatted string with settings for controlling API behavior
 */
+(void)initialize:(int64_t)gameId apiKey:(NSString *)apiKey withSettingsJSON:(NSString*)settingsJSON;

/**
* Retrieves the global API instance. Must not be called until after initialize.
* @returns The global KongregateAPI object
*/
+(KongregateAPI*)instance;

/**
* Call this from your AppDelegate when the application launches. This is used to
* track push notifications through swrve.
* @param launchOptions the launch options passed to your AppDelegate's didFinishLaunchingWithOptions methodRe
*/
+(void)didFinishLaunchingWithOptions:(NSDictionary*)launchOptions;

/**
* Call this from your AppDelegate when a push notification is received. This is used
* for tracking push notifications through swrve.
* @param userInfo the userInfo passed to your AppDelegate's didReceiveRemoteNotification
*/
+(void)didReceiveRemoteNotification:(NSDictionary*)userInfo;

/**
* Call this from your AppDelegate's openUrl method which is invoked when the app is opened from a
* deep link. This is used for Adjust deep link reattributions.
*
* It is OK invoke this method prior to initializing the Kongregate SDK.
*/
+(void)willOpenUrl:(NSURL*)url;

/**
* Sets the listener function that will be notified of Kongregate events
* @param observer The observer object
* @param selector The selector to call on the observer
*/
-(void)setApiEventListener:(id)observer selector:(SEL)selector;

/**
 * Sets the listener function that will be notified of Kongregate events. This
 * differs from setApiEventListener in that the events may contain a JSON payload.
 * @param block a ^(NSString* eventName, NSString* eventJSON) block to be invoked
 * on Kongregate events
 */
-(void)setApiEventBundleListener:(KongEventBundleListener) block;

/**
* For environments where it isn't possible to set a listener object, you
* can use this method to poll for an array of pending events and iterate
* through them
* @return An array of NSString pending events
*/
-(NSArray *)pollEvents;

/**
 * For environments where it isn't possible to set a listener object, you
 * can use this method to poll for an array of pending events and iterate
 * through them
 * @return An array of pending KongEventBundle events
 */
-(NSArray *)pollEventBundles;

/**
* Retrieves the global KongServices object.
* @return The KongServices object
*/
-(KongServices*)services;

/**
* Retrieves the global KongStats object.
* @return The KongStats object
*/
-(KongStatServices*)stats;

/**
* Retrieves the global KongMobile object.
* @return The KongMobile object
*/
-(KongMobile*)mobile;

/**
* Retrieves the global KongMtx object.
* @return The KongMobile object
*/
-(KongMtx*)mtx;

/**
* Retrieves the global KongAnalytics object.
* @return The KongAnalytics object
*/
-(KongAnalytics*)analytics;

// Internal methods
+(bool)ready;
+(bool)debug;
+(int64_t)gameId;
+(NSString*)apiKey;
+(NSString*)kongDomain;
+(NSString*)kongURL;
+(NSURL*)openURL;
+(bool)webViewDebug;
+(NSString*)panelOrientationOverride;
+(NSString*)panelTransitionOverride;
+(bool)isTestGDPRAlert;
+(bool)isDeferGDPRAlert;
+(void)setCrashReportUserObject:(id)value forKey:(NSString*)key;
-(id)init;
-(void)broadcastApiEventBundle:(KongEventBundle*) apiEvent;
-(void)broadcastApiEvent:(NSString*) apiEvent;
-(void)broadcastApiEvent:(NSString*) eventName withData:(NSString*) eventJSON;
-(bool)getBoolSetting:(NSString*) key withDefault:(bool)defaultValue;
-(void)dealloc;

@end
