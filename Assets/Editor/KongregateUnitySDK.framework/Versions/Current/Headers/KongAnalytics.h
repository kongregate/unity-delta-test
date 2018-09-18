//
//  KongAnalytics.h
//  KongregateSDK
//
//  Created by Russell Kinnicutt on 9/16/13.
//  Copyright (c) 2013 Kongregate. All rights reserved.
//

#import <Foundation/Foundation.h>

FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_AD_TRACKING;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_BUNDLE_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_CLIENT_OS_TYPE;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_CLIENT_OS_VERSION;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_PLATFORM;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_CLIENT_VERSION;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_COUNTRY_CODE;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_DATA_CONNECTION_TYPE;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_DAYS_RETAINED;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_DEV_CLIENT_VERSION;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_DEVICE_EVENT_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_DEVICE_TYPE;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_EVENT_TIME;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_EXTERNAL_IP_ADDRESS;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FB_USER_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FB_USERNAME;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_PUR_TIER;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FILTER_TYPE;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FIRST_CLIENT_VERSION;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FIRST_PLAY_TIME;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FIRST_PLAY_TIME_OFFSET;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FIRST_SDK_VERSION;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FIRST_SERVER_VERSION;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_GAMECENTER_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_GAMECENTER_ALIAS;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_IDFA;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_IP_ADDRESS;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_IS_FROM_BACKGROUND;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_KONG_PLUS;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_KONG_USER_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_KONG_USERNAME;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_LANG_CODE;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_NUM_PURCHASES;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_NUM_SESSIONS;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_PKG_SRC;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_PLAYER_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_PRIVACY_POLICY_VERSION;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_PRIVACY_POLICY_ACCEPTED_AT;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_PUSH_NOTIFICATIONS;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_SITE_VISITOR_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_SDK_VERSION;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_SESSION_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_TIME_SKEW;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_TOTAL_SPENT_IN_USD;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_EVENT_TIME_OFFSET;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_USD_SPENT_ON_KREDS;

FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_PLAYER_INFO_EVENT;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_TWITTER_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FB_USER_ID;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FB_USERNAME;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_FB_EMAIL;
FOUNDATION_EXPORT NSString* const KONG_ANALYTICS_EMAIL;

FOUNDATION_EXPORT NSString* const KONG_PURCHASE_SUCCESS;
FOUNDATION_EXPORT NSString* const KONG_PURCHASE_FAIL;
FOUNDATION_EXPORT NSString* const KONG_PURCHASE_RECEIPT_FAIL;

FOUNDATION_EXPORT NSString* const KONG_SWRVE_VIRTUAL_ECONOMY_EVENT_PURCHASE;
FOUNDATION_EXPORT NSString* const KONG_SWRVE_VIRTUAL_ECONOMY_EVENT_GIFT;
FOUNDATION_EXPORT NSString* const KONG_SWRVE_VIRTUAL_ECONOMY_PARAM_ITEM;
FOUNDATION_EXPORT NSString* const KONG_SWRVE_VIRTUAL_ECONOMY_PARAM_CURRENCY;
FOUNDATION_EXPORT NSString* const KONG_SWRVE_VIRTUAL_ECONOMY_PARAM_COST;
FOUNDATION_EXPORT NSString* const KONG_SWRVE_VIRTUAL_ECONOMY_PARAM_QUANTITY;
FOUNDATION_EXPORT NSString* const KONG_SWRVE_VIRTUAL_ECONOMY_PARAM_AMOUNT;


/**
* This interface wraps the functionality provided by the [Adjust](http://www.adjust.com)
* and [Swrve](http://www.swrve.com) SDKs. It also includeds legacy support for Keen. Ad-X
* has been removed from the Kongregate SDK.
*
* The primary purpose of this interface is to expose hooks for games to pass events
* to Swrve and Adjust. It also provides wrappers for some additional Swrve features
* such as User Properties, A/B Testing, and Push Notifications. See the
* [Mobile SDK Docs](/docs/analytics.html) for
* details of the various features.
*/
@interface KongAnalytics : NSObject

-(id)init;

/**
* Submits an event with the given collection name to Keen and Swrve.
*
* @param event The event properties (key/value pairs)
* @param eventCollection The name of the event/collection to submit
*/
-(void)addEvent:(NSDictionary*)event toCollection:(NSString*) eventCollection;

/**
* Helper method similar to addEvent:toCollection which only takes NSString arguments.
* This is helpful for writing plugins in languages which can't easily create an NSDictionary.
*
* @param collection The collection name
* @param eventJson A valid JSON object string with the event properties
* @param commonPropsJson A valid JSON object string containing common properties.
*/
-(void)addEvent:(NSString*)collection withEventJson:(NSString*)eventJson withCommonPropsJson:(NSString*) commonPropsJson;

/**
* Sets the common properties callback as a block which returns an NSDictionary. This block
* will be called each time an event is submitted to populate the common fields for that event.
*
* @param block A block that returns an NSDictionary of properties
*/
-(void)setCommonPropertiesBlock:(NSDictionary* (^) (void)) block;

/**
* Sets a listener for analytic events the Kongregate SDK automatically fires. Most games will
* not need to listen for these events. It's useful if you wish to echo the events to your own
* analytics collection system.
*
* @param block A block to be notified when auto events are fired
*/
-(void)setAutoEventListener:(void (^) (NSString*, NSDictionary*)) block;

/**
* Sets the common properties to the given JSON string. Useful for native
* bindings to other languages where the callback method can't be used.
*
* @param json A valid JSON object string with the common properties.
*/
-(void)setCommonPropertiesJSON:(NSString*)json;

/**
* Sets the common properties to the given NSDictionary. Useful for native
* bindings to other languages where the callback method can't be used.
*
* @param commonProps A valid JSON object string with the common properties.
*/
-(void)setCommonProperties:(NSDictionary*)commonProps;

/**
* @deprecated see finishPurchase
* @param productID the product ID
*/
-(void)trackPurchase:(NSString*)productID;

/**
* @deprecated see finishPurchase
* @param productID The product ID
* @param quantity The quantity of items purchased
*/
-(void)trackPurchase:(NSString*)productID withQuantity:(int)quantity;

/**
* @deprecated see finishPurchase
* @param productID The product ID
* @param quantity The quantity of items purchased
* @param jsonFields An optional JSON object string with custom event fields for the Keen event
*/
-(void)trackPurchase:(NSString *)productID withQuantity:(int)quantity withGameFieldsJson:(NSString*)jsonFields;

/**
* @deprecated see finishPurchase
* @param productID The product ID
* @param quantity The quantity of items purchased
* @param fields An optional dictionary containing custom event fields to pass along
*/
-(void)trackPurchase:(NSString *)productID withQuantity:(int)quantity withGameFields:(NSDictionary*)fields;

/**
* Creates an iap_attemps event. The SKU must match the
* one set up in the iTunes Connect portal, and must contain the pricing
* tier, such as: t05_hard
*
* @param productID The product ID
* @param quantity The quantity of items purchased
* @param fields An optional dictionary containing custom event fields to pass along
*/
-(void)startPurchase:(NSString*)productID withQuantity:(int)quantity withGameFields:(NSDictionary*)fields;

/**
 * Creates either iap_transactions or iap_fails purchase events depending on arguments and
 * state of the transaction. Invoke this method after the transcation completed, but before
 * it is finished.
 *
 * The resultCode parameter should be one of:
 *  KONG_PURCHASE_SUCCESS: if the transaction was successfully completed
 *  KONG_PURCHASE_FAIL: if the transaction failed
 *  KONG_PURCHASE_RECEIPT_FAIL: if the transaction was successfull, but reciept verification failed.
 *
 * @param resultCode one of KONG_PURCHASE_SUCCESS, KONG_PURCHASE_FAIL, KONG_PURCAHSE_RECEIPT_FAIL
 * @param transactionId the SKUPurchase.transactionIdentifier
 * @param gameFields game fields to be passed along with the event
 */
-(void)finishPurchase:(NSString*) resultCode
    withTransactionId:(NSString*) transactionId
       withGameFields:(NSDictionary*) gameFields;

/**
 * Creates either iap_transactions or iap_fails purchase events depending on arguments and
 * state of the transaction. Invoke this method after the transcation completed, but before
 * it is finished.
 *
 * The resultCode parameter should be one of:
 *  KONG_PURCHASE_SUCCESS: if the transaction was successfully completed
 *  KONG_PURCHASE_FAIL: if the transaction failed
 *  KONG_PURCHASE_RECEIPT_FAIL: if the transaction was successfull, but reciept verification failed.
 *
 * @param resultCode one of KONG_PURCHASE_SUCCESS, KONG_PURCHASE_FAIL, KONG_PURCAHSE_RECEIPT_FAIL
 * @param transactionId the SKUPurchase.transactionIdentifier
 * @param gameFieldsJson game fields to be passed along with the event
 */
-(void)finishPurchase:(NSString*) resultCode
    withTransactionId:(NSString*) transactionId
   withGameFieldsJson:(NSString*) gameFieldsJson;

/**
 * Creates either iap_transactions or iap_fails purchase events depending on the result code. If
 * transaction ID is available, use the finishPurchase:withTransactionId varients, since
 * they will automatically pull information from the transaction queue. This method is provided for
 * compatibility with plugins or APIs that don't provide access to the transaction ID.
 *
 * The resultCode parameter should be one of:
 *  KONG_PURCHASE_SUCCESS: if the transaction was successfully completed
 *  KONG_PURCHASE_FAIL: if the transaction failed
 *  KONG_PURCHASE_RECEIPT_FAIL: if the transaction was successfull, but reciept verification failed.
 *
 * @param resultCode one of KONG_PURCHASE_SUCCESS, KONG_PURCHASE_FAIL, KONG_PURCAHSE_RECEIPT_FAIL
 * @param productId the SKUPurchase.transactionIdentifier
 * @param receipt a Base64 encoded receipt
 * @param gameFields game fields to be passed along with the event
 */
-(void)finishPurchase:(NSString*) resultCode
    withProductId:(NSString *)productId
          withReceipt:(NSString *)receipt
       withGameFields:(NSDictionary *)gameFields;

/**
 * Creates either iap_transactions or iap_fails purchase events depending on the result code. If
 * transaction ID is available, use the finishPurchase:withTransactionId varients, since
 * they will automatically pull information from the transaction queue. This method is provided for
 * compatibility with plugins or APIs that don't provide access to the transaction ID.
 *
 * The resultCode parameter should be one of:
 *  KONG_PURCHASE_SUCCESS: if the transaction was successfully completed
 *  KONG_PURCHASE_FAIL: if the transaction failed
 *  KONG_PURCHASE_RECEIPT_FAIL: if the transaction was successfull, but receipt verification failed.
 *
 * @param resultCode one of KONG_PURCHASE_SUCCESS, KONG_PURCHASE_FAIL, KONG_PURCAHSE_RECEIPT_FAIL
 * @param productId the SKUPurchase.transactionIdentifier
 * @param receipt a Base64 encoded receipt
 * @param gameFieldsJson game fields to be passed along with the event
 */
-(void)finishPurchase:(NSString*) resultCode
        withProductId:(NSString *)productId
          withReceipt:(NSString *)receipt
   withGameFieldsJson:(NSString *)gameFieldsJson;

/**
* Creates a start purchase event. The SKU must match the
* one set up in the iTunes Connect portal, and must contain the pricing
* tier, such as: t05_hard
*
* @param productID The product ID
* @param quantity The quantity of items purchased
* @param jsonFields An optional JSON object string with custom event fields for the Keen event
*/
-(void)startPurchase:(NSString*)productID withQuantity:(int)quantity withGameFieldsJson:(NSString*)jsonFields;

/**
 * Update game specific user properties with Swrve. The Kongregaet SDK automatically updates
 * a set of user properties with Swrve as well. This method is to be used for game specific
 * user segmentation.
 * @param props Dictionary of user state attributes.
 */
-(void)gameUserUpdate:(NSDictionary*)props;

/**
 * Update game specific user properties with Swrve. The Kongregaet SDK automatically updates
 * a set of user properties with Swrve as well. This method is to be used for game specific
 * user segmentation.
 * @param propsJson A JSON formatted string object of user state attributes
 */
-(void)gameUserUpdateJSON:(NSString*)propsJson;

/**
 * Retrieve the complete set of automatic properties Kongregate tracks as a JSON 
 * formatted string.
 */
-(NSString*)getAutoPropertiesJSON;

/**
* Get a String automatic property
* @param field The field name
* @return The prop value
*/
-(NSString*)getAutoStringProperty:(NSString*)field;

/**
* Get a 64-bit integer automatic property
* @param field The field name
* @return The prop value
*/
-(int64_t)getAutoLongLongProperty:(NSString*)field;

/**
* Get a boolean automatic property
* @param field The field name
* @return The prop value
*/
-(BOOL)getAutoBoolProperty:(NSString*)field;

/**
* Get an integer automatic property
* @param field The field name
* @return The prop value
*/
-(int)getAutoIntProperty:(NSString*)field;

/**
* Get a UTC timestamp automatic property
* @param field The field name
* @return The prop value
*/
-(NSString*)getAutoUTCProperty:(NSString*)field;

/**
* Get a double automatic property
* @param field The field name
* @return The prop value
*/
-(double)getAutoDoubleProperty:(NSString*)field;

/**
 * @deprecated use addFilterType instead
 * Set a custom filter type to be sent with every event
 * @param filterType the prop value
 */
-(void)setFilterType:(NSString*)filterType;

/**
 * Adds a custom filter type to be sent with every event
 * @param filterType the prop value
 */
-(void)addFilterType:(NSString*)filterType;

/**
* When using KONGREGATE_OPTION_DEFER_ANALYTICS, this allows you to start
* the analytics subsystem. You should attempt to call this function as quickly as
* possible after starting up, or you may miss events.
*/
-(void)start;

/**
 * Get all Swrve resources. 
 * @retruns a dictionary of resource names to SwrveResource objects
 */
-(NSDictionary*)getResources;

/**
 * Get all Swrve resource names
 * @returns an array of NSString resource names.
 */
-(NSArray*)getResourceNames;

/**
 * Get all Swrve resource names as a JSON Array
 * @return a JSON Array formatted NSString of all Swrve resource names.
 */
-(NSString*)getResourceNamesAsJson;

/*
 * Retrieve a Swrve A/B test resource as a NString.
 * @param resourceId the ID of the Swrve resource
 * @param attrId the ID of the attribute
 * @param defValue a devault value if the resource/attribute is not found.
 * @return the value of the attribute in the resource, or the default alue if not found.
 */
-(NSString*)getResourceAsString:(NSString*)resourceId withAttr:(NSString*)attrId withDefault:(NSString*)defValue;

/**
 * Retrieve a Swrve A/B test resource as an int.
 * @param resourceId the ID of the Swrve resource
 * @param attrId the ID of the attribute
 * @param defValue a devault value if the resource/attribute is not found.
 * @return the value of the attribute in the resource, or the default alue if not found.
 */
-(int)getResourceAsInt:(NSString*)resourceId withAttr:(NSString*)attrId withDefault:(int)defValue;

/**
 * Retrieve a Swrve A/B test resource as a float.
 * @param resourceId the ID of the Swrve resource
 * @param attrId the ID of the attribute
 * @param defValue a devault value if the resource/attribute is not found.
 * @return the value of the attribute in the resource, or the default alue if not found.
 */
-(float)getResourceAsFloat:(NSString*)resourceId withAttr:(NSString*)attrId withDefault:(float)defValue;

/**
 * Retrieve a Swrve A/B test resource as a boolean.
 * @param resourceId the ID of the Swrve resource
 * @param attrId the ID of the attribute
 * @param defValue a devault value if the resource/attribute is not found.
 * @return the value of the attribute in the resource, or the default alue if not found.
 */
-(BOOL)getResourceAsBool:(NSString*)resourceId withAttr:(NSString*)attrId withDefault:(BOOL)defValue;

/**
 * Sets the callback listener for Delta in-app messages [Deprecated]
 * Will be invoked when a deep link is selected as the message Action.
 * @param callback to be invoked
 */
-(void)setSwrveButtonCallback:(void (^) (NSString*, NSString*)) callback;

/**
 * Sets the callback listener for Delta in-app messages
 * Will be invoked when a deep link is selected as the message Action.
 * @param callback to be invoked
 */
-(void)setDeltaButtonCallback:(void (^) (NSString*, NSString*)) callback;


/**
 * Sets the callback listener for Delta parameters
 * Will be invoked when parameters are returned in response to an event
 * @param callback to be invoked
 */
-(void)setDeltaParameterCallback:(void (^)(NSDictionary *))callback;

/**
 * Notify the Kongregate SDK that a promo award has been handled. This will update
 * a Swrve User Property that may be used as a filter for additional Swrve messaging.
 *
 * It's a good idea, particularly if your game has a server component, to also track awarded
 * promoIds and to display a message if your game is asked to award a promo that has already
 * been awarded. This will handle an edge case where a user can trick the client into showing
 * an in-app message to perform the award again.
 *
 * NOTE: you may also simply pass the promoID for the event, rather than the complete payload.
 * When only the promoID is passed, the user property will be updated. When the full event
 * payload is included, the SDK will parse the payload and may automatically preform other actions
 * such as open an install URL.
 *
 * @param eventPayload the event payload for the PROMO_AWARD event.
 */
-(void)finishPromoAward:(NSString*)eventPayload;

/**
 * Returns the common analytics properties collected by the Kongregate SDK
 */
-(NSString *)getKongregatePropertiesJSON;

/**
 * Returns a json string of the Apple transaction details for the given transaction
 *
 * @param transactionId The transaction identifier; if null or empty, attempts to retrieve last transaction details from queue
 * @param resultCode The result of transaction
 */
-(NSString *)getTransactionDetailsJSON:(NSString *)transactionId withResult:(NSString *)resultCode;

@end
