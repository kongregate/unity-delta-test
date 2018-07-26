#import <KongregateSDK/KongregateSDK.h>
#import "AppDelegateListener.h"
#if UNITY_VERSION >= 430
#import "KongAppDelegateListener.h"
#endif

@implementation KongAppDelegateListener

+(void)load
{
    NSLog(@"loading KongAppDelegateListener");
#if UNITY_VERSION >= 430
    UnityRegisterAppDelegateListener((id) [self listener]);
#endif
    
    [[NSNotificationCenter defaultCenter] addObserver:[self listener]
                                             selector:@selector(applicationDidFinishLaunching:)
                                                 name:UIApplicationDidFinishLaunchingNotification
                                               object:nil];

}

+ (KongAppDelegateListener*)listener
{
    static dispatch_once_t pred;
    static KongAppDelegateListener *_sharedInstance = nil;
    
    dispatch_once( &pred, ^{ _sharedInstance = [[self alloc] init]; } );
    return _sharedInstance;
}

- (void)onOpenURL:(NSNotification*)notification
{
    NSLog(@"KongAppDelegateListener onOpenURL: %@", notification);
    if (notification != nil && notification.userInfo != nil) {
        [KongregateAPI willOpenUrl:notification.userInfo[@"url"]];
    }
}

- (void)applicationDidFinishLaunching:(NSNotification*)notification
{
    NSLog(@"KongAppDelegateListener applicationDidFinishLaunching: %@", notification);
    if (notification != nil) {
        [KongregateAPI didFinishLaunchingWithOptions:notification.userInfo];
    }
}

- (void)didReceiveRemoteNotification:(NSNotification*)notification {
    NSLog(@"KongAppDelegateListener didReceiveRemoteNotification: %@", notification);
    if (notification != nil) {
        [KongregateAPI didReceiveRemoteNotification:notification.userInfo];
    }
}

@end
