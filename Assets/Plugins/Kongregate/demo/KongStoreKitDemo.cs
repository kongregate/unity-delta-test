using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if PRIME31_STOREKIT
using Prime31;
#endif

namespace Kongregate
{
  public class KongStoreKitDemo : MonoBehaviour
  {
    public string _GooglePublicKey;
    #if PRIME31_STOREKIT
    bool mRecentPurchase;
    bool mPurchaseReady;
    Dictionary<string,object> mTransactionsInVerification;

    // Use this for initialization
    void Start ()
    {
      ConfigureStoreKitManager();

      KongregateManager.eventReady += delegate() {
        SetupPurchases ();
        if (mTransactionsInVerification.Count > 0) {
          // verify any transactions that were finished before the SDK finished initializing
          foreach (var transactionId in mTransactionsInVerification.Keys) {
            Debug.Log("KongStoreKitDemo:: verifying pending transactions: " + transactionId);
            KongregateAPI.GetAPI().Mtx.VerifyTransactionId(transactionId);
          }
        }
      };
      KongregateManager.eventReceiptVerificationComplete += OnReceiptVerificationComplete;
    }

    void OnGUI ()
    {
      KongregateAPI kongregate = KongregateAPI.GetAPI();
      if (!kongregate.IsReady()) {
        // don't display purchase UI until Kongregate is ready
        return;
      }
      KongDemoHelper.PrepareGUI();

      if (mPurchaseReady) {
        if (GUI.Button (new Rect (1280 / 2 - 100, 50, 240, 80), "P31 Purchase", KongDemoHelper.buttonStyle)) {
          StartPurchase ();
        }
      }
    }

    void ConfigureStoreKitManager() {

      //queue of transactions needing to be verified
      mTransactionsInVerification = new Dictionary<string,object> ();

      #if UNITY_IPHONE

      //don't autoconfirm transactions until we have verified the receipt
      StoreKitBinding.enableHighDetailLogs(true);
      StoreKitManager.autoConfirmTransactions=false;
      StoreKitBinding.setShouldSendTransactionUpdateEvents(true);

      StoreKitManager.productListReceivedEvent += allProducts => {
        Debug.Log( "KongStoreKitDemo:: received total products: " + allProducts.Count );
        mPurchaseReady = true;
      };

      // the following handles receipt verification
      StoreKitManager.productPurchaseAwaitingConfirmationEvent += transaction => {
        Debug.Log("KongStoreKitDemo: productPurchaseAwaitingConfirmationEvent: " + transaction.transactionState);
        mTransactionsInVerification[transaction.transactionIdentifier] = transaction; //add this transaction to the queue
        if (KongregateAPI.GetAPI() != null && KongregateAPI.GetAPI().IsReady()) {
          Debug.Log("KongStoreKitDemo: verifiying: " + transaction);
          KongregateAPI.GetAPI().Mtx.VerifyTransactionId(transaction.transactionIdentifier);
        } else {
          Debug.Log("KongStoreKitDemo: delay verify until READY event: " + transaction);
        }
      };

      StoreKitManager.restoreTransactionsFailedEvent += errorMsg => {
        Debug.Log("KongStoreKitDemo: restoreTransactionsFailedEvent: " + errorMsg);
      };

      StoreKitManager.restoreTransactionsFinishedEvent += () => {
        Debug.Log("KongStoreKitDemo: restoreTransactionsFinishedEvent");
      };

      StoreKitManager.purchaseSuccessfulEvent += transaction => {
        Debug.Log("KongStoreKitDemo: purchaseSuccessfulEvent: " + transaction.transactionIdentifier);
      };

      StoreKitManager.purchaseFailedEvent += errorMsg => {
        Debug.Log("KongStoreKitDemo: purchase failed: " + errorMsg);
        // pass null, if you do not have the transaction id.
        KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_FAIL, null,
          getPurchaseFields());
        mPurchaseReady = true;
      };

      StoreKitManager.purchaseCancelledEvent += errorMsg => {
        Debug.Log("KongStoreKitDemo: purchase cancelled: " + errorMsg);
        // pass null, if you do not have the transaction id.
        KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_FAIL, null,
          getPurchaseFields());
        mPurchaseReady = true;
      };

      StoreKitManager.transactionUpdatedEvent += transaction => {
        Debug.Log("KongStoreKitDemo: transactionUpdated: " + transaction.transactionState);
      };

      Debug.Log("KongStoreKitDemo: listeners configured");

      #endif
    }

    void SetupPurchases ()
    {
      Debug.Log ("KongStoreKitDemo: Setting up purchases...");
      #if UNITY_IPHONE
      Debug.Log("KongStoreKitDemo: requesting products");
      StoreKitBinding.requestProductData( new string[] { "com.kongregate.mobile.games.angryBots.t05_coins" } );
      #elif UNITY_ANDROID
      GoogleIABManager.billingSupportedEvent += () => {
        Debug.Log ("KongStoreKitDemo: billing is supported query inventory");
        mPurchaseReady = true;
        var skus = new string[] { "com.kongregate.android.games.angrybots.t03_hard" };
        GoogleIAB.queryInventory (skus);
      };
      GoogleIABManager.queryInventorySucceededEvent += (purchases, skus) => {
        Debug.Log("KongStoreKitDemo: queryInventory succeeded");
        foreach (GooglePurchase purchase in purchases) {
          Debug.Log("KongStoreKitDemo: queryInventory succeeded: " + purchase.productId);
          verifyGoogleReceipt (purchase);
        }
      };
      GoogleIABManager.queryInventoryFailedEvent += (sku) => {
        Debug.Log("KongStoreKitDemo: queryInventoryFailedEvent: " + sku);
      };
      GoogleIABManager.purchaseSucceededEvent += purchase => {
        Debug.Log ("Google IAB callback - purchased product: " + purchase);
        verifyGoogleReceipt (purchase);
      };
      GoogleIABManager.purchaseFailedEvent += message => {
        Debug.Log ("Google IAB callback - purchase failed: " + message);
        KongregateAPI.GetAPI ().Analytics.FinishPurchase (KongregateAPI.PURCHASE_FAIL,
                                                      message, getPurchaseFields ());
        mPurchaseReady = true;
      };
      GoogleIABManager.consumePurchaseSucceededEvent += purchase => {
        Debug.Log ("KongStoreKitDemo: Google IAB callback - consume succeeded: " + purchase);
      };
      GoogleIABManager.consumePurchaseFailedEvent += message => {
        Debug.Log ("Google IAB callback - consume failed: " + message);
      };
      GoogleIAB.init (_GooglePublicKey);
      #elif UNITY_WEBPLAYER || UNITY_WEBGL
    mPurchaseReady = true;
      #endif
    }

  #if UNITY_ANDROID
    void verifyGoogleReceipt (GooglePurchase purchase)
    {
      mTransactionsInVerification [purchase.orderId != null ? purchase.orderId : string.Empty] = purchase;
      KongregateAPI.GetAPI ().Mtx.VerifyGoogleReceipt (purchase.originalJson, purchase.signature);
    }
  #elif UNITY_WEBPLAYER || UNITY_WEBGL
  void OnKredPurchaseResult(string result) {
    Debug.Log("Kred Purchase Result: " + result);
    mPurchaseReady = true;
    // This is when you would inform your server to check the player's Kongregate inventory if successful
  }
  #endif

    Dictionary<string,object> getPurchaseFields ()
    {
      Dictionary<string,object> fields = new Dictionary<string,object> ()
    {
      { "hard_currency_change",  5 },
      { "soft_currency_change", 10 },
      { "type", "Gold Pack" },
      { "discount_percent", 12.5 },
      { "context_of_offer","StoreFront"},
      { "resources_change", new object[] { "rank=1", "type=win" } },
    };
      return fields;
    }

    void StartPurchase ()
    {
      mPurchaseReady = false; // disable button when clicked
      KongregateAPI kongregate = KongregateAPI.GetAPI ();
      Dictionary<string,object> fields = new Dictionary<string,object> ()
    {
      { "type", "Gold Pack" },
      { "discount_percent", 12.5 },
      { "context_of_offer", "StoreFront"}
    };
      #if UNITY_IPHONE
    kongregate.Analytics.StartPurchase( "com.kongregate.mobile.games.angryBots.t05_coins", 1, fields );
    StoreKitBinding.purchaseProduct( "com.kongregate.mobile.games.angryBots.t05_coins", 1 );
      #elif UNITY_ANDROID
      kongregate.Analytics.StartPurchase ("com.kongregate.android.games.angrybots.t03_hard", 1, fields);
      GoogleIAB.purchaseProduct ("com.kongregate.android.games.angrybots.t03_hard");
      #elif UNITY_WEBPLAYER || UNITY_WEBGL
    kongregate.Analytics.StartPurchase("com.kongregate.web.angrybots.t05_hard", 1, fields);

    // Use the Javascript API to initiate the purchase. Notice in this flow we call finishPurchase via
    // the Javascript API after the purchase is completed by serializing the game fields needed. You
    // could also save the game fields into a variable in your Unity script and then call
    // kongregate.Analytics.FinishPurchase(...) in the Unity callback as well.
    Application.ExternalEval(string.Format(
      @"kongregate.mtx.purchaseItems(['com.kongregate.web.angrybots.t05_hard'], function(result){{
                var status = result.success ? 'SUCCESS' : 'FAIL';
                var data = result.success ? '{0}' : '{1}';
                kongregate.analytics.finishPurchase(status, result.purchase_id, data);

                // Fire the callback in the Unity code
                kongregateUnitySupport.getUnityObject().SendMessage('Kongregate', 'OnKredPurchaseResult', status);
            }});"
			, Analytics.toAnalyticEventJSON(getPurchaseFields()), Analytics.toAnalyticEventJSON(fields)));
			#endif
		}

		void OnReceiptVerificationComplete() {
			Debug.Log ("OnReceiptVerificationComplete");
			KongregateAPI kongregate = KongregateAPI.GetAPI();
			// we keep a list of transactions here so that we don't run into race conditions
			Dictionary<string,object> stillInVerification = new Dictionary<string,object> ();
			foreach (var transactionId in mTransactionsInVerification.Keys) {
				string status = kongregate.Mtx.ReceiptVerificationStatus (transactionId);
				if (status != KongregateAPI.RECEIPT_VERIFICATION_STATUS_PROCESSING) {
					Debug.Log ("Verification for " + transactionId + " is " + status);
					#if UNITY_IPHONE
					if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_VALID) {
						StoreKitTransaction transaction = (StoreKitTransaction)mTransactionsInVerification[transactionId];
						KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_SUCCESS,
						                                                transactionId, getPurchaseFields());

						// Below is an alternative iOS FinishPurchase() call. This version may be used if your plugin
						// does not make the transaction Id available or finishes the transaction before passing control
						// back to Unity. FinishPurchase() is preferred because purchase details may be pulled directly from
						// the transaction.

						// KongregateAPI.GetAPI().Analytics.FinishPurchaseWithProductId(KongregateAPI.PURCHASE_SUCCESS,
						//  transaction.productIdentifier, transaction.base64EncodedTransactionReceipt, getPurchaseFields());
					} else if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_INVALID) {
						KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_RECEIPT_FAIL,
						                                                transactionId, getPurchaseFields());
					}
					// call to finish the transaction regardless of it we are valid or not
					StoreKitBinding.finishPendingTransaction(transactionId);
					#elif UNITY_ANDROID
					GooglePurchase transaction = (GooglePurchase)mTransactionsInVerification [transactionId];
					if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_VALID) {
						KongregateAPI.GetAPI ().Analytics.FinishPurchase (KongregateAPI.PURCHASE_SUCCESS,
						                                                transaction.originalJson, getPurchaseFields (), transaction.signature);
					} else if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_INVALID) {
						KongregateAPI.GetAPI ().Analytics.FinishPurchase (KongregateAPI.PURCHASE_RECEIPT_FAIL,
						                                                transaction.originalJson, getPurchaseFields ());
					}
					// call to finish transaction regardless of it we are valid or not
					GoogleIAB.consumeProduct (transaction.productId);
					#endif
					mPurchaseReady = true;
				} else {
					Debug.Log ("Still waiting on verification for " + transactionId);
					stillInVerification [transactionId] = mTransactionsInVerification [transactionId];
				}
			}
			mTransactionsInVerification = stillInVerification;
		}
#endif

  }
}
