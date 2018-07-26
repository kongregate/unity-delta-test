//
// Created by Ben Vinson on 9/24/13.
// Copyright (c) 2013 Kongregate. All rights reserved.
//


#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

typedef void (^KongUserItemCallback)(bool, __int64_t, NSArray *);
typedef void (^KongVerifyCompletionHandler)(BOOL success);

/**
* Interface for transaction APIs including Kongregate micro-transactions, user item management,
* and App Store reciept validation. 
*
* This API can be used to query the server to determine if a Kongregate user owns a particular item.
* Currently, the functionality provided by this API is used for cross-promotion between games.
* For example, a bonus item in one game can be awarded by obtaining a badge in another.
*/
@interface KongMtx : NSObject {
    NSMutableDictionary* _inventories;
}
-(id)init;

/**
* Call this function to request the user's inventory from the remote
* server. Once the list of items has been retrieved, a KONGREGATE_EVENT_USER_INVENTORY
* event will be broadcast, at which point you can call hasItem: to
* determine if the user owns a specific item.
*/
-(void)requestUserItemList;

/**
* Used to check if a user owns an item with the given identifier. Only valid
* after a call to requestUserItemList generates the KONGREGATE_EVENT_USER_INVENTORY
* event. It is recommended you persist the value of this call to disk once it returns
* true so that repeated requests to the server don't need to be made.
*
* @param identifier The Kongregate item identifier
* @return True if the user has the specified identifier in their inventor
*/
-(bool)hasItem:(NSString*)identifier;

/**
* Call this function to verify a transaction from the App Store. If you have access to the
* transaction object this is the method to use. When verification is complete, the callback
* will contain a boolean result to continue your processing. If you don't have access to 
* transaction object, or can not use a callback, you can use the verifyTransactionId method.
*
* @param transaction The SKPaymentTransaction to verify
* @param completionHandler The callback function and boolean result for when complete
*/
- (void)verifyTransaction:(SKPaymentTransaction *)transaction completionHandler:(KongVerifyCompletionHandler)completionHandler;

/**
* Call this function to verify a transaction id that you have received. When the verification
* has finished, you will receive the KONGREGATE_EVENT_RECEIPT_VERIFICATION_COMPLETED event.
* Once you have received that event, you can find the status of this verification by using the
* receiptVerificationStatus method. Note that you can use verifyTransaction instead of this
* if you have the SKPaymentTransaction object and can handle a callback. Note that you should
* NOT finish the transaction with Apple until verification is complete.
*
* @param transactionId The transacationIdentifier from the transaction to verify
*/
- (void)verifyTransactionId:(NSString *)transactionId;

/**
* This method is to be used in conjunction with verifyTransactionId to check on the status
* of the verification process. The status will be one of the following:
*
* - KONG_RECEIPT_VERIFICATION_STATUS_UNKNOWN: the transaction id is unknown
* - KONG_RECEIPT_VERIFICATION_STATUS_PROCESSING: the verification process is still running
* - KONG_RECEIPT_VERIFICATION_STATUS_VALID: the transaction is valid
* - KONG_RECEIPT_VERIFICATION_STATUS_INVALID: the transaction is invalid
*
* Once you have a valid or invalid result, you can now finish the transaction. Note that
* you should still finish the transaction with Apple regardless of if it is invalid or not.
*
* @param transactionId The transacationIdentifier to check the status of
* @return the status of the verification.
*/
- (NSString*)receiptVerificationStatus:(NSString*)transactionId;

@end