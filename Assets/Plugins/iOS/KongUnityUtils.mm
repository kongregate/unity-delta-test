#import "KongUnityUtils.h"

#if UNITY_VERSION < 500
void UnityPause(bool pause);
void UnitySetAudioSessionActive(bool active);
#else
void UnityPause(int pause);
void UnitySetAudioSessionActive(int active);
#endif

@implementation KongUnityUtils

+(void)unityPause:(NSNumber*)shouldPause {
#if UNITY_VERSION < 500
    UnityPause([shouldPause boolValue]);
#else
    UnityPause([shouldPause intValue]);
#endif
}

+(void)unitySetAudioSessionActive:(NSNumber*)active {
#if UNITY_VERSION < 500
    UnitySetAudioSessionActive([active boolValue]);
#else 
    UnitySetAudioSessionActive([active intValue]);
#endif
}

@end
