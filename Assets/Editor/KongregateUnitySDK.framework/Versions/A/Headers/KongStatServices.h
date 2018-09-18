//
// Created by Ben Vinson on 5/7/13.
// Copyright (c) 2013 Kongregate. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>

/**
* This interface is used for submitting Kongregate statistics for scores
* and achievements.
*/
@interface KongStatServices : NSObject
-(id)init;

/**
* Submit a statistic to the server. Submissions will be queued locally and
* submitted to the server periodically in a background task.
*
* @param name The statistic name
* @param value The statistic value
*/
-(void)submit:(NSString*)name value:(int)value;
@end