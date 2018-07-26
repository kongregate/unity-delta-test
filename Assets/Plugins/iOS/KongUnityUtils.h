#pragma once
#import <Foundation/Foundation.h>
#import "UnityAppController.h"

@interface KongUnityUtils : NSObject
+(void)unityPause:(NSNumber*)shouldPause;
+(void)unitySetAudioSessionActive:(NSNumber*)active;
@end

@interface KongregateAppController : UnityAppController
-(BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions;
-(void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo;
@end
