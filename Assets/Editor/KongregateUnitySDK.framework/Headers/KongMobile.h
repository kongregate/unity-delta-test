//
//  KongMobile.h
//  KongregateSDK
//
//  Created by Darcy Brown on 6/5/13.
//  Copyright (c) 2013 Kongregate. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

/**
 * valid targets for openKongregateWindow:target
 */
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_MORE_GAMES;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_HIGH_SCORES;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_SUPPORT;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_FORUMS;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_OFFERS;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_REGISTRATION;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_TERMS;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_PRIVACY;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_TOPICS;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_MESSAGES;
FOUNDATION_EXPORT NSString* const KONG_MOBILE_TARGET_GUILD_CHAT;

/**
 * valid mobile panel events for KONGREGATE_OPTION_SUPPORTED_PANEL_EVENTS option
 */
FOUNDATION_EXPORT NSString* const KONG_MOBILE_PANEL_EVENT_GO_TO_GUILDS;

// supported custom panel transition animations for KONGREGATE_OPTION_DEFAULT_PANEL_TRANSITION
FOUNDATION_EXPORT NSString* const KONG_PANEL_TRANSITION_SLIDE_FROM_LEFT;
FOUNDATION_EXPORT NSString* const KONG_PANEL_TRANSITION_SLIDE_FROM_RIGHT;

/**
 * Trigger ({@link #trigger(String)}) to notify the Kongregate SDK
 * to display the GDPR Alert dialog, if needed
 */
FOUNDATION_EXPORT NSString* const KONG_MOBILE_GDPR_ALERT_TRIGGER;

/**
* This interface handles mobile-specific issues such as UI and opening
* the Kongregate panel.
*/
@interface KongMobile : NSObject
-(id)init;

/**
* Attempts to orient the Kongregate button to the screen. This can be useful
* after detecting a rotation.
*/
-(void)orientButtonToScreen;

/**
* Opens the Kongregate window
*/
-(void)openKongregateWindow;

/**
 * Opens the Kongregate window with a target
 *
 * @param target The target to open the window with. (e.g. KONG_MOBILE_TARGET_FORUMS)
 */
-(void)openKongregateWindow:(NSString*)target;

/**
 * Opens the kongregate window with a target and index. For example,
 * a specific topic.
 * 
 * @param target The target page to open the panel to (e.g. KONG_MOBILE_TARGET_TOPICS)
 * @param targetId The ID to open within the target (e.g. 1234)
 */
-(void)openKongregateWindow:(NSString*)target withId:(NSString*)targetId;

/**
 * Close webview
 */
-(void)closeKongregateWindow;

/**
 * Check if the Kongregate Window is currently open
 */
-(BOOL)isKongregateWindowOpen;

/**
* Gets an instance of the Kongregate button to add to your layout. The size will
* be either 32, 48, or 64 pixels depending on the device.
*
* @return The button
*/
-(UIButton*)getButton;

/**
* Gets an instance of the Kongregate button to add to your layout. Valid sizes
* are 32, 48, and 64.
*
* @param size The size of the button (32/48/64)
* @return The button
*/
-(UIButton*)getButton:(int)size;

/**
* @deprecated see KongAnalytics finishPurchase
* @param productID the product ID
* @param quantity the quantity
*/
-(void)trackPurchase:(NSString*)productID withQuantity:(int)quantity;


/**
* @deprecated see KongAnalytics finishPurchase
* @param productID the product ID
*/
-(void)trackPurchase:(NSString*)productID;


/**
 * Return the URL used to open the app when launched or brought to the foreground using a deep link.
 * The {@link KongregateAPI#KONGREGATE_EVENT_DEEP_LINK} will be fired as well. Will return
 * nil for standard app launches.
 */
-(NSURL*)getOpenURL;

/**
 * Triggers an event from the game to the Kongregate SDK
 * @param name
 */
-(void)trigger:(NSString*)name;

@end
