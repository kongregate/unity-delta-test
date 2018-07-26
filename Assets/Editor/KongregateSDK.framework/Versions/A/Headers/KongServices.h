//
// Created by Ben Vinson on 5/7/13.
// Copyright (c) 2013 Kongregate. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>

/**
* This class is used to get general information about the Kongregate environment
* and the user. These methods should not be relied upon until the READY event has
* been received.
*/
@interface KongServices : NSObject
-(id)init;

/**
* The Kongregate username for the current user.
* @return The username
*/
-(NSString*)getUsername;

/**
* The Kongregate user ID of the current user.
* @return The user id, or 0 if the user is a guest
*/
-(int64_t)getUserId;

/**
* Determines if the user is a guest on Kongregate.
* @return True if the user is a guest
*/
-(bool)isGuest;

/**
* Determines if the user has a Kong+ account.
* @return True if the user has a Kong+ account
*/
-(bool)hasKongPlus;

/**
* Retrieves the game authentication token for the Kongregate user.
* @return The token
* @note This will be a stub value for guests
*/
-(NSString*)getGameAuthToken;

/**
 * Retrieve the count of unread notifications for the current user
 * @return number of unread notifications
 */
-(int32_t)getNotificationCount;

/**
 * Check whether the user has unread guild messages
 * @return true if the user has unread guild messages
 */
-(bool)hasUnreadGuildMessages;

/**
* Updates the character token used for guild chat authentication. This should be
* generated on your server and signed using JWT.
* @param characterToken The JWT signed token
*/
-(void)setCharacterToken:(NSString*)characterToken;

@end