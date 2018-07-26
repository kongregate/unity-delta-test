//
//  KongregateUnityWrapper.h
//
//  Created by Darcy Brown on 4/22/13.
//
//

#ifndef KONGREGATE_NATIVE_WRAPPER
#define KONGREGATE_NATIVE_WRAPPER

#import <KongregateSDK/KongregateSDK.h>
#import <UIKit/UIKit.h>
#import <objc/objc.h>

@interface KongregateNativeWrapper : NSObject
@end

static KongregateNativeWrapper* _wrapper;
static UIButton* _kongButton;
static int kongButtonTag = 444;
static int kongButtonX = 0;
static int kongButtonY = 0;
static int kongButtonSize = 64;
static int kongButtonRepositionBaseY = 1024;
static int kongButtonRepositionBaseX = 768;
static bool kongButtonReposition = true;
static bool useNativeRendering = true;
static bool windowPausesUnity = true;
static bool windowPausesSound = false;
static bool isReady = false;
void (^messageReceivedCallback)(void);
void (^kongSendApiEvent)(NSString* eventName,NSString* eventJSON);

#ifndef KongWrapperLog
#define KONG_WRAPPER_LOG_DEBUG 0
#define KONG_WRAPPER_LOG_WARN  1
#define KongWrapperLog(frmt, ... )  [KongregateNativeWrapper log:KONG_WRAPPER_LOG_DEBUG file:__FILE__ line:__LINE__ format:(frmt), ##__VA_ARGS__]
#define KongWrapperWarn(frmt, ... ) [KongregateNativeWrapper log:KONG_WRAPPER_LOG_WARN file:__FILE__ line:__LINE__ format:(frmt), ##__VA_ARGS__]
#endif


#ifdef KONGREGATE_NO_UNITY
void UnitySendMessage(const char* obj, const char* method, const char* msg);
#else
//Unity
extern "C" {
void UnitySendMessage(const char* obj, const char* method, const char* msg);
}
#endif

@implementation KongregateNativeWrapper

#ifdef KONGREGATE_NO_UNITY
-(void)unityPause:(bool)pause {}
-(void)unitySetAudioSessionActive:(bool)active {}
#else
-(void)unityPause:(bool)pause {
    Class helperClass = NSClassFromString(@"KongUnityUtils");
    if(!helperClass) {
        KongWrapperWarn(@"Unable to find KongUnityUtils class!");
        return;
    }

    if(![helperClass respondsToSelector:@selector(unityPause:)]) {
        KongWrapperWarn(@"Unable to find KongUnityUtils.unityPause method");
        return;
    }

    [helperClass performSelector:@selector(unityPause:) withObject:@(pause)];
}
-(void)unitySetAudioSessionActive:(bool)active {
    Class helperClass = NSClassFromString(@"KongUnityUtils");
    if(!helperClass) {
        KongWrapperWarn(@"Unable to find KongUnityUtils class!");
        return;
    }

    if(![helperClass respondsToSelector:@selector(unitySetAudioSessionActive:)]) {
        KongWrapperWarn(@"Unable to find KongUnityUtils.unitySetAudioSessionActive method");
        return;
    }

    [helperClass performSelector:@selector(unitySetAudioSessionActive:) withObject:@(active)];
}
#endif

-(UIWindow *)mainWindow {
    //    return UnityGetMainWindow(); --only Unity4 supports this
    return [[UIApplication sharedApplication] keyWindow];
}

//If the symbol for iOS 8 isn't defined, define it.
#ifndef NSFoundationVersionNumber_iOS_8_0
#define NSFoundationVersionNumber_iOS_8_0 1134.10 //extracted with NSLog(@"%f", NSFoundationVersionNumber)
#endif
-(void)showButton {
    if([[self mainWindow] viewWithTag:_kongButton.tag] == nil) {
        // NOTE that Unity 0,0 comes across bottom,left here whereas it is set top,left
        // also, we have to re-orient ourselves to the screen which is assumed to be landscape
        // also need to take into account the repositioning
        // This no longer appears to apply on iOS8
        CGRect screenBounds = [[UIScreen mainScreen] bounds];
        [[[KongregateAPI instance] mobile] orientButtonToScreen];
        int translatedX = kongButtonX;
        int translatedY = kongButtonY;
        if (kongButtonReposition) {
            translatedX = (int)(kongButtonX*screenBounds.size.width  / kongButtonRepositionBaseX);
            translatedY = (int)(kongButtonY*screenBounds.size.height / kongButtonRepositionBaseY);
        }

        // iOS before version 8 used a wonky coordinate system
        if (floor(NSFoundationVersionNumber) < NSFoundationVersionNumber_iOS_8_0) {
            translatedX = (int)(screenBounds.size.width - translatedX); //top is bottom - assumes landscape
            _kongButton.frame = CGRectMake(translatedX-_kongButton.frame.size.height, translatedY, _kongButton.frame.size.width, _kongButton.frame.size.height);
        } else {
            _kongButton.frame = CGRectMake(translatedX, translatedY, _kongButton.frame.size.width, _kongButton.frame.size.height);
        }
        KongWrapperLog(@"setting translated button position %d %d", translatedX, translatedY);

        // this is a little weird because X is the screen width, which in landscape is our height, and our top is actually the bottom
        [[self mainWindow] addSubview:_kongButton];
        [[self mainWindow] bringSubviewToFront:_kongButton];
    } else {
        KongWrapperLog(@"Already added the button to the window");
    }
}

-(void)hideButton {
    if([[self mainWindow] viewWithTag:_kongButton.tag] != nil) {
        [_kongButton removeFromSuperview];
    } else {
        KongWrapperLog(@"Already removed the button from the window");
    }
}

-(void)webViewClosing {
    KongWrapperLog(@"KongUnityWrapper - Kong webView Closed");
    if(useNativeRendering) {
        [_wrapper showButton];
    }
    if (windowPausesSound) {
        [self unitySetAudioSessionActive:true];
    }
    if (windowPausesUnity) {
        [self unityPause:false];
    }
}

-(void)webViewOpening {
    KongWrapperLog(@"KongUnityWrapper - Kong webView Opened");
    if(useNativeRendering) {
        [_wrapper hideButton];
    }
    if (windowPausesSound) {
        [self unitySetAudioSessionActive:false];
    }
    if (windowPausesUnity) {
        [self unityPause:true];
    }
}

-(void)onKongregateApiEvent:(NSString *)event withData:(NSString*)dataJSON{
    BOOL hasCallback = (kongSendApiEvent != nil);
    if ([KONGREGATE_EVENT_READY isEqualToString:event]) {
        KongWrapperLog(@"Kongregate API Initialized");
        isReady = true;
    }
    if ([KONGREGATE_EVENT_OPENING isEqualToString:event]) {
        if (hasCallback) {
            // use a callback here so that we don't pause before they get a chance to process it
            messageReceivedCallback = ^{
                [self webViewOpening];
            };
        } else {
            [self webViewOpening];
        }
    }
    if ([KONGREGATE_EVENT_CLOSED isEqualToString:event]) {
        [self webViewClosing];
    }
    if (hasCallback) {
        kongSendApiEvent(event, dataJSON);
    }
}

+(void)log:(int)level file:(const char *)file line:(int)line format:(NSString *)format, ... {
    if ([KongregateAPI debug] || level == KONG_WRAPPER_LOG_WARN) {
        va_list args;
        if (format)
        {
            va_start(args, format);
            NSString *logMsg = [[NSString alloc] initWithFormat:format arguments:args];
            NSString *levelStr = (level == 0) ? @"DEBUG" : @"WARN";
            NSLog(@"KONG UNITY WRAPPER %@ <%@:(%d)> %@", levelStr, [[NSString stringWithUTF8String:file] lastPathComponent], line, logMsg );
            va_end(args);
        }
    }
}

+(NSDictionary*)parseJSON:(NSString*)json {
    if(!json || json.length == 0) {
        return (nil);
    }

    NSData* data = [json dataUsingEncoding:NSUTF8StringEncoding];
    NSError* error;
    NSDictionary* result = [NSJSONSerialization JSONObjectWithData:data options:(NSJSONReadingOptions) kNilOptions error:&error];
    if(result) {
        return (result);
    } else {
        KongWrapperLog(@"Error while parsing JSON: %@", error);
        return (nil);
    }
}

+(NSString*)toJSONString:(id)dictionaryOrArrayToOutput {
    NSError* error;
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:dictionaryOrArrayToOutput
                                                       options:0
                                                         error:&error];
    if(!jsonData) {
        KongWrapperLog(@"Got an error: %@", error);
        return @"{}"; // empty json object
    }
    NSString* jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return jsonString;
}

@end

NSString* CreateNSString(const char* string) {
    return [NSString stringWithUTF8String:string ? string : ""];
}

char* MakeStringCopy(const char* string) {
    if(string == NULL)return NULL;
#ifdef KONGREGATE_AIR
    FREObject res;
    FRENewObjectFromUTF8((uint32_t)(strlen(string)+1), (const uint8_t*)string, &res);
    return((char*)res);
#else
    if(string == NULL) return NULL;
    char* res = (char*) malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
#endif
}

extern "C" {

void _KongregateAPIInitializeJSON(int64_t gameId, char* apiKey, char* settingsJSON) {
    NSLog(@"_KongregateAPIInitializeJSON");
    _wrapper = [[KongregateNativeWrapper alloc] init];
    NSString* apiKeyString = CreateNSString(apiKey);
    NSString* settingsJSONString = CreateNSString(settingsJSON);

    [KongregateAPI initialize:gameId apiKey:apiKeyString withSettingsJSON:settingsJSONString];
    [[KongregateAPI instance] setApiEventBundleListener:^(NSString *event, NSString *jsonBundle) {
        [_wrapper onKongregateApiEvent:event withData:jsonBundle];
    }];
    kongButtonReposition = [[KongregateAPI instance] getBoolSetting:KONGREGATE_OPTION_AUTO_REPOSITION withDefault:NO];
    windowPausesSound = [[KongregateAPI instance] getBoolSetting:KONGREGATE_OPTION_UNITY_PANEL_PAUSE_SOUND withDefault:NO];
    windowPausesUnity = [[KongregateAPI instance] getBoolSetting:KONGREGATE_OPTION_UNITY_PANEL_PAUSE_APP withDefault:NO];
    _kongButton = [[[KongregateAPI instance] mobile] getButton];
    _kongButton.tag = kongButtonTag;
}

char* _KongregateAPIServicesGetUsername() {
    return MakeStringCopy([[[[KongregateAPI instance] services] getUsername] UTF8String]);
}

bool _KongregateAPIServicesIsGuest() {
    return [[[KongregateAPI instance] services] isGuest];
}

bool _KongregateAPIServicesHasKongPlus() {
    return [[[KongregateAPI instance] services] hasKongPlus];
}

char* _KongregateAPIMobileGetOpenURL() {
    NSURL* url = [[[KongregateAPI instance] mobile] getOpenURL];
    if (url == nil) {
        return NULL;
    }
    NSString* urlString = [url absoluteString];
    if (urlString.length == 0) {
        return NULL;
    }

    return MakeStringCopy([urlString UTF8String]);
}

int32_t _KongregateAPIServicesGetNotificationCount() {
    return [[[KongregateAPI instance] services] getNotificationCount];
}

bool _KongregateAPIServicesHasUnreadGuildMessages() {
    return [[[KongregateAPI instance] services] hasUnreadGuildMessages];
}

void _KongregateAPIServicesSetCharacterToken(char* token) {
    NSString* string = [NSString stringWithFormat:@"%s", token];
    [[[KongregateAPI instance] services] setCharacterToken:string];
}

void _KongregateAPIStatsSubmit(char* statisticName, int statisticValue) {
    NSString* string = [NSString stringWithFormat:@"%s", statisticName];
    KongWrapperLog(@"recieved stat from unity: %@ %d", string, statisticValue);
    [[[KongregateAPI instance] stats] submit:string value:statisticValue];
}

void _KongregateAPIAnalyticsSetSwrveButtonListener(char* gameObjectName, char* functionName) {
    NSString* gameObjectNameStr = CreateNSString(gameObjectName);
    NSString* functionNameStr = CreateNSString(functionName);
    KongWrapperLog(@"API: setting swrve button callback %@ %@", gameObjectNameStr, functionNameStr);
    [[[KongregateAPI instance] analytics] setSwrveButtonCallback:^(NSString *action, NSString *value) {
        // Process the custom action. For example:
        NSString* actionValueStr = [NSString stringWithFormat:@"%@ %@", action, value];
        KongWrapperLog(@"forwarding swrve action: %@", action);
        UnitySendMessage([gameObjectNameStr UTF8String], [functionNameStr UTF8String], [actionValueStr UTF8String]);
    }];
}
    
void _KongregateAPIAnalyticsSetDeltaButtonListener(char* gameObjectName, char* functionName) {
    NSString* gameObjectNameStr = CreateNSString(gameObjectName);
    NSString* functionNameStr = CreateNSString(functionName);
    KongWrapperLog(@"API: setting delta button callback %@ %@", gameObjectNameStr, functionNameStr);
    [[[KongregateAPI instance] analytics] setDeltaButtonCallback:^(NSString *action, NSString *value) {
        // Process the custom action. For example:
        NSString* actionValueStr = [NSString stringWithFormat:@"%@ %@", action, value];
        KongWrapperLog(@"forwarding delta action: %@", action);
        UnitySendMessage([gameObjectNameStr UTF8String], [functionNameStr UTF8String], [actionValueStr UTF8String]);
    }];
}
    
void _KongregateAPIAnalyticsSetDeltaParameterListener(char* gameObjectName, char* functionName) {
    NSString* gameObjectNameStr = CreateNSString(gameObjectName);
    NSString* functionNameStr = CreateNSString(functionName);
    KongWrapperLog(@"API: setting delta parameter callback %@ %@", gameObjectNameStr, functionNameStr);
    [[[KongregateAPI instance] analytics] setDeltaParameterCallback:^(NSDictionary *fields) {
        NSString* fieldsJSON = [KongregateNativeWrapper toJSONString:fields];
        KongWrapperLog(@"forwarding delta action: %@", fieldsJSON);
        UnitySendMessage([gameObjectNameStr UTF8String], [functionNameStr UTF8String], [fieldsJSON UTF8String]);
    }];
}

void _KongregateAPIAnalyticsSetAutoEventListener(char* gameObjectName, char* methodName) {
    NSString* gameObjectNameStr = CreateNSString(gameObjectName);
    NSString* methodNameStr = CreateNSString(methodName);
    KongWrapperLog(@"API: setting auto event callback %@ %@", gameObjectNameStr, methodNameStr);
    [[[KongregateAPI instance] analytics] setAutoEventListener:^(NSString *eventName, NSDictionary *fields) {
        NSString* fieldsJSON = [KongregateNativeWrapper toJSONString:fields];
        NSString* eventMessage = [NSString stringWithFormat:@"%@ %@", eventName, fieldsJSON];
        KongWrapperLog(@"forwarding event bundle: %@", eventName);
        UnitySendMessage([gameObjectNameStr UTF8String], [methodNameStr UTF8String], [eventMessage UTF8String]);
    }];
}

void _KongregateAPIAnalyticsAddEvent(char* collectionParam, char* eventJsonParam, char* commonPropsJsonParam) {
    NSString* collection = CreateNSString(collectionParam);
    NSString* eventJson = CreateNSString(eventJsonParam);
    NSString* commonPropsJson = CreateNSString(commonPropsJsonParam);

    [[[KongregateAPI instance] analytics] addEvent:collection withEventJson:eventJson withCommonPropsJson:commonPropsJson];

}

void _KongregateAPIAnalyticsAddFilterType(char* filterTypeParam) {
    NSString* filterType = CreateNSString(filterTypeParam);
    [[[KongregateAPI instance] analytics] addFilterType:filterType];
}

void _KongregateAPIAnalyticsGameUserUpdate(char* propsJsonParam) {
    NSString* propsJson = CreateNSString(propsJsonParam);
    [[[KongregateAPI instance] analytics] gameUserUpdateJSON:propsJson];
}

char* _KongregateAPIAnalyticsGetAutoPropertiesJSON() {
    NSString* json = [[[KongregateAPI instance] analytics] getAutoPropertiesJSON];
    json = json != nil ? json : @"";
    return MakeStringCopy([json UTF8String]);
}

char* _KongregateAPIAnalyticsGetAutoStringProperty(char* fieldParam) {
    NSString* fieldName = CreateNSString(fieldParam);
    NSString *value = [[[KongregateAPI instance] analytics] getAutoStringProperty:fieldName];
    return MakeStringCopy([value UTF8String]);
}

int64_t _KongregateAPIAnalyticsGetAutoLongProperty(char* fieldParam) {
    NSString* fieldName = CreateNSString(fieldParam);
    return [[[KongregateAPI instance] analytics] getAutoLongLongProperty:fieldName];
}

BOOL _KongregateAPIAnalyticsGetAutoBoolProperty(char* fieldParam) {
    NSString* fieldName = CreateNSString(fieldParam);
    return [[[KongregateAPI instance] analytics] getAutoBoolProperty:fieldName];
}

int _KongregateAPIAnalyticsGetAutoIntProperty(char* fieldParam) {
    NSString* fieldName = CreateNSString(fieldParam);
    return [[[KongregateAPI instance] analytics] getAutoIntProperty:fieldName];
}

double _KongregateAPIAnalyticsGetAutoDoubleProperty(char* fieldParam) {
    NSString* fieldName = CreateNSString(fieldParam);
    return [[[KongregateAPI instance] analytics] getAutoDoubleProperty:fieldName];
}

char* _KongregateAPIAnalyticsGetAutoUTCProperty(char* fieldParam) {
    NSString* fieldName = CreateNSString(fieldParam);
    NSString *value = [[[KongregateAPI instance] analytics] getAutoUTCProperty:fieldName];
    return MakeStringCopy([value UTF8String]);
}

void _KongregateAPIAnalyticsUpdateCommonProps(char* mapJsonParam) {
    NSString* mapJson = CreateNSString(mapJsonParam);
    [[[KongregateAPI instance] analytics] setCommonPropertiesJSON:mapJson];
}

void _KongregateAPIAnalyticsStart() {
    [[[KongregateAPI instance] analytics] start];
}

void _KongregateAPIMtxRequestUserItemList() {
    [[[KongregateAPI instance] mtx] requestUserItemList];
}

bool _KongregateAPIMtxHasItem(char* string) {
    NSString* identifier = [NSString stringWithFormat:@"%s", string];
    return [[[KongregateAPI instance] mtx] hasItem:identifier];
}

void _KongregateAPIMtxVerifyTransactionId(char* transactionId) {
    [[[KongregateAPI instance] mtx] verifyTransactionId:CreateNSString(transactionId)];
}

char* _KongregateAPIMtxReceiptVerificationStatus(char* transactionId) {
    NSString *status = [[[KongregateAPI instance] mtx] receiptVerificationStatus:CreateNSString(transactionId)];
    return MakeStringCopy([status UTF8String]);
}

void _KongregateAPIMobileOpenKongregateWindow(char* targetParam, char* targetIdParam) {
    NSString *target = CreateNSString(targetParam);
    NSString *targetId = CreateNSString(targetIdParam);
    KongWrapperLog(@"_OpenKongregate: %@ : %@", target, targetId);
    [[[KongregateAPI instance] mobile] openKongregateWindow:target withId:targetId];
    KongWrapperLog(@"-> complete.");
}

void _KongregateAPIMobileCloseKongregateWindow() {
    KongWrapperLog(@"_CloseKongregate");
    [[[KongregateAPI instance] mobile] closeKongregateWindow];
}

void _KongregateAPIMobileButtonHide() {
    KongWrapperLog(@"API: hide button");
    if(useNativeRendering) {
        [_wrapper hideButton];
    }
}

void _KongregateAPIMobileButtonShow() {
    KongWrapperLog(@"API: show button");
    if(useNativeRendering) {
        [_wrapper showButton];
    }
}

const char* _KongregateAPIMobileButtonGetTexture() {
    KongWrapperLog(@"rendering Kong button in Unity");
    UIImage* myUIImage = [_kongButton backgroundImageForState:UIControlStateNormal];
    NSData* imageData = UIImagePNGRepresentation(myUIImage);
    NSArray* paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString* documentsDirectory = [paths objectAtIndex:0];
    NSString* filePath = [documentsDirectory stringByAppendingPathComponent:@"kong.png"]; //Add the file name
    KongWrapperLog(@"filePath %@", filePath);
    [imageData writeToFile:filePath atomically:YES];
    return MakeStringCopy([filePath UTF8String]);
}

void _KongregateAPIMobileButtonSetNativeRendering(bool nativeRendering) {
    KongWrapperLog(@"API: set native %d", nativeRendering);
    useNativeRendering = nativeRendering;
}

void _KongregateAPIMobileButtonSetX(int x) {
    KongWrapperLog(@"API: set x %d", x);
    kongButtonX = x;
}

void _KongregateAPIMobileButtonSetY(int y) {
    KongWrapperLog(@"API: set y %d", y);
    kongButtonY = y;
}

void _KongregateAPIMobileButtonSetSize(int size) {
    KongWrapperLog(@"API: set size %d", size);
    kongButtonSize = size;
}

void _KongregateAPIMobileTrigger(char* nameParam) {
    NSString* name = CreateNSString(nameParam);
    KongWrapperLog(@"API: trigger %@", name);
    [KongregateAPI.instance.mobile trigger:name];
}
    
void _KongregateAPIAnalyticsStartPurchase(char* productIDParam, int qty, char* fieldsJsonParam) {
    NSString* productId = CreateNSString(productIDParam);
    NSString* fieldsJson = CreateNSString(fieldsJsonParam);
    KongWrapperLog(@"API: mobile start purchase productID: %@  qty: %d", productId, qty);
    [KongregateAPI.instance.analytics startPurchase:productId
                                       withQuantity:qty
                                 withGameFieldsJson:fieldsJson];
}

void _KongregateAPIAnalyticsFinishPurchase(char* resultCodeParam, char* transactionIdParam, char* fieldsJsonParam) {
    NSString* resultCode = CreateNSString(resultCodeParam);
    NSString* transactionId = CreateNSString(transactionIdParam);
    NSString* fieldsJson = CreateNSString(fieldsJsonParam);
    KongWrapperLog(@"API: mobile finish purchase: %@  result: %@", transactionId, resultCode);
    [KongregateAPI.instance.analytics finishPurchase:resultCode
                                   withTransactionId:transactionId
                                  withGameFieldsJson:fieldsJson];
}

void _KongregateAPIAnalyticsFinishPurchaseWithProductId(char* resultCodeParam, char* productIdParam, char* receiptParam, char* fieldsJsonParam) {
    NSString* resultCode = CreateNSString(resultCodeParam);
    NSString* productId = CreateNSString(productIdParam);
    NSString* receipt = CreateNSString(receiptParam);
    NSString* fieldsJson = CreateNSString(fieldsJsonParam);
    KongWrapperLog(@"API: mobile finish purchase: %@  result: %@", productId, resultCode);
    [KongregateAPI.instance.analytics finishPurchase:resultCode
                                       withProductId:productId
                                         withReceipt:receipt
                                  withGameFieldsJson:fieldsJson];
}

const char* _KongregateAPIAnalyticsGetResourceNames() {
    NSString* resourceNames = [KongregateAPI.instance.analytics getResourceNamesAsJson];
    return MakeStringCopy([resourceNames UTF8String]);
}
    
void _KongregateAPIAnalyticsFinishPromoAward(char* promoIdParam) {
    NSString* promoId = CreateNSString(promoIdParam);
    [KongregateAPI.instance.analytics finishPromoAward:promoId];
}

const char* _KongregateAPIAnalyticsGetResourceAsString(char* resourceIdParam, char* attributeIdParam, char* defValueParam) {
    NSString* resourceId = CreateNSString(resourceIdParam);
    NSString* attributeId = CreateNSString(attributeIdParam);
    NSString* defValue = CreateNSString(defValueParam);
    NSString* result = [KongregateAPI.instance.analytics getResourceAsString:resourceId
                                                                    withAttr:attributeId
                                                                 withDefault:defValue];
    return MakeStringCopy([result UTF8String]);
}

bool _KongregateAPIAnalyticsGetResourceAsBool(char* resourceIdParam, char* attributeIdParam, bool defValue) {
    NSString* resourceId = CreateNSString(resourceIdParam);
    NSString* attributeId = CreateNSString(attributeIdParam);
    bool result = [KongregateAPI.instance.analytics getResourceAsBool:resourceId
                                                             withAttr:attributeId
                                                          withDefault:defValue];
    return result;

}


float _KongregateAPIAnalyticsGetResourceAsFloat(char* resourceIdParam, char* attributeIdParam, float defValue) {
    NSString* resourceId = CreateNSString(resourceIdParam);
    NSString* attributeId = CreateNSString(attributeIdParam);
    float result = [KongregateAPI.instance.analytics getResourceAsFloat:resourceId
                                                               withAttr:attributeId
                                                            withDefault:defValue];
    return result;

}

int _KongregateAPIAnalyticsGetResourceAsInt(char* resourceIdParam, char* attributeIdParam, int defValue) {
    NSString* resourceId = CreateNSString(resourceIdParam);
    NSString* attributeId = CreateNSString(attributeIdParam);
    int result = [KongregateAPI.instance.analytics getResourceAsInt:resourceId
                                                           withAttr:attributeId
                                                        withDefault:defValue];
    return result;

}


void _KongregateAPIAnalyticsTrackPurchase(char* productID, int qty, char* fieldsJsonParam) {
    NSString* skuStr = CreateNSString(productID);
    NSString* fieldsJson = CreateNSString(fieldsJsonParam);
    KongWrapperLog(@"API: mobile track purchase productID: %@  qty: %d", skuStr, qty);
    [KongregateAPI.instance.analytics trackPurchase:skuStr withQuantity:qty withGameFieldsJson:fieldsJson];
}

void _KongregateAPISetEventListenter(char* gameObjectName, char* functionName) {
    NSString* gameObjectNameStr = CreateNSString(gameObjectName);
    NSString* functionNameStr = CreateNSString(functionName);
    KongWrapperLog(@"API: set event listener %@ %@", gameObjectNameStr, functionNameStr);
    kongSendApiEvent = ^(NSString* eventName, NSString* jsonBundle) {
        KongWrapperLog(@"forwarding event: %@", eventName);
        UnitySendMessage([gameObjectNameStr UTF8String], [functionNameStr UTF8String], [eventName UTF8String]);
    };
}

void _KongregateAPISetEventBundleListener(char* gameObjectName, char* functionName) {

    NSString* gameObjectNameStr = CreateNSString(gameObjectName);
    NSString* functionNameStr = CreateNSString(functionName);
    KongWrapperLog(@"API: setting event bundle listener %@ %@", gameObjectNameStr, functionNameStr);
    kongSendApiEvent = ^(NSString* eventName, NSString *jsonBundle) {
        NSString* event;
        if (jsonBundle == nil) {
            event = eventName;
        } else {
            event = [NSString stringWithFormat:@"%@ %@", eventName, jsonBundle];
        }
        KongWrapperLog(@"forwarding event bundle: %@", event);
        UnitySendMessage([gameObjectNameStr UTF8String], [functionNameStr UTF8String], [event UTF8String]);
    };
}

void _KongregateAPIMessageReceived(char * eventName) {
    KongWrapperLog(@"API: confirm message recieved for %@", CreateNSString(eventName));
    if (messageReceivedCallback) {
        KongWrapperLog(@"running callback");
        messageReceivedCallback();
        messageReceivedCallback = nil;
    }
}

bool _KongregateAPIIsReady() {
    return isReady;
}

int64_t _KongregateAPIServicesGetUserId() {
    return [[[KongregateAPI instance] services] getUserId];
}

const char* _KongregateAPIServicesGetGameAuthToken() {
    return MakeStringCopy([[[[KongregateAPI instance] services] getGameAuthToken] UTF8String]);
}
}

#endif
