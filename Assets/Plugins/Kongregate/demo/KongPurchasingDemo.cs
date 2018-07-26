
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;

namespace Kongregate
{

  // Demonstrate how to integrate Kongregate IAP purchase calls
  // with the Unity Purchasing API.
  public class KongPurchasingDemo : MonoBehaviour, IStoreListener
  {

    private static IStoreController mStoreController;
    private static IExtensionProvider mStoreExtensionProvider;
    private static Dictionary<string,Product> mPendingPurchases = new Dictionary<string,Product>();

    private static string kProductIDConsumable =    "consumable";                                                         // General handle for the consumable product.
    private static string kProductNameAppleConsumable =    "com.kongregate.mobile.games.angryBots.t05_coins";             // Apple App Store identifier for the consumable product.
    private static string kProductNameGooglePlayConsumable =    "com.kongregate.android.games.angrybots.t03_hard";        // Google Play Store identifier for the consumable product.

    void Start()
    {
      // If we haven't set up the Unity Purchasing reference
      if (mStoreController == null)
      {
        // set listener for receipt validation
        KongregateManager.eventReceiptVerificationComplete += OnReceiptVerificationComplete;

        // Begin to configure our connection to Purchasing
        InitializePurchasing();
      }
    }

    void OnGUI ()
    {
      if (!KongregateAPI.GetAPI().IsReady()) {
        // don't display purchase UI until Kongregate is ready
        return;
      }

      // Present a simple GUI to purchase an item
      KongDemoHelper.PrepareGUI();
      if (IsInitialized()) {
        if (GUI.Button (new Rect (1280 / 2 - 55, 100, 150, 40), "Unity Purchase", KongDemoHelper.buttonStyle)) {
          BuyConsumable ();
        }
      }
    }

    public void InitializePurchasing()
    {
      // If we have already connected to Purchasing ...
      if (IsInitialized())
      {
        // ... we are done here.
        return;
      }

      // Create a builder, first passing in a suite of Unity provided stores.
      var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

      // Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
      builder.AddProduct(kProductIDConsumable, ProductType.Consumable, new IDs(){{ kProductNameAppleConsumable,       AppleAppStore.Name },{ kProductNameGooglePlayConsumable,  GooglePlay.Name },});// Continue adding the non-consumable product.
      UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
      // Only say we are initialized if both the Purchasing references are set.
      return mStoreController != null && mStoreExtensionProvider != null;
    }

    public void BuyConsumable()
    {
      // Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
      BuyProductID(kProductIDConsumable);
    }

    void BuyProductID(string productId)
    {
      Dictionary<string,object> purchaseFields = new Dictionary<string,object>() {{ "type", "Gold Pack"}};

      // If the stores throw an unexpected exception, use try..catch to protect my logic here.
      try
      {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
          // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
          Product product = mStoreController.products.WithID(productId);

          // If the look up found a product for this device's store and that product is ready to be sold ...
          if (product != null && product.availableToPurchase)
          {
            // Notify Kongregate a purchase is starting
            KongregateAPI.GetAPI().Analytics.StartPurchase(product.definition.storeSpecificId, 1, purchaseFields);

            Debug.Log (string.Format("Purchasing product asychronously: '{0}'", product.definition.storeSpecificId));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
            mStoreController.InitiatePurchase(product);
          }
          // Otherwise ...
          else
          {
            // ... report the product look-up failure situation
            Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_FAIL,
              "product not found or available", purchaseFields);
          }
        }
        // Otherwise ...
        else
        {
          // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
          Debug.Log("BuyProductID FAIL. Not initialized.");
          KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_FAIL,
            "purchasing not initialized", purchaseFields);
        }
      }
      // Complete the unexpected exception handling ...
      catch (Exception e)
      {
        // ... by reporting any unexpected exception for later diagnosis.
        Debug.Log ("BuyProductID: FAIL. Exception during purchase. " + e);
        KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_FAIL,
          e.Message, purchaseFields);
      }
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
    public void RestorePurchases()
    {
      // If Purchasing has not yet been set up ...
      if (!IsInitialized())
      {
        // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
        Debug.Log("RestorePurchases FAIL. Not initialized.");
        return;
      }

      // If we are running on an Apple device ...
      if (Application.platform == RuntimePlatform.IPhonePlayer ||
      Application.platform == RuntimePlatform.OSXPlayer)
      {
        // ... begin restoring purchases
        Debug.Log("RestorePurchases started ...");

        // Fetch the Apple store-specific subsystem.
        var apple = mStoreExtensionProvider.GetExtension<IAppleExtensions>();
        // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
        apple.RestoreTransactions((result) => {
          // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
          Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
          });
      }
      // Otherwise ...
      else
      {
        // We are not running on an Apple device. No work is necessary to restore purchases.
        Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
      }
    }

    //
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
      // Purchasing has succeeded initializing. Collect our Purchasing references.
      Debug.Log("OnInitialized: PASS");

      // Overall Purchasing system, configured with products for this application.
      mStoreController = controller;
      // Store specific subsystem, for accessing device-specific store features.
      mStoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
      // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
      Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
      // A consumable product has been purchased by this user.
      if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable, StringComparison.Ordinal))
      {
        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.

        // The code below demonstrates how to use Kongregate's Receipt Validation
        // API. Only do this step if you're server does not have it's own receipt validation.
        // See OnReceiptVerificationComplete() for what to do after the receipt is validated
        // either on your server or through our API.

        #if UNITY_ANDROID

        // parse the receipt json, orderID and data signature
        Dictionary<string,object> receipt = Json.Deserialize(args.purchasedProduct.receipt) as Dictionary<string,object>;
        Dictionary<string,object> payload = Json.Deserialize(receipt["Payload"] as string) as Dictionary<string,object>;
        Dictionary<string,object> json = Kongregate.Json.Deserialize(payload["json"] as string) as Dictionary<string,object>;
        string googleReceiptJSON = payload["json"] as string;
        string googleSignature = payload["signature"] as string;
        string orderId = json.ContainsKey("orderId") ? json["orderId"] as string : string.Empty;

        // for Android we key the purchase using the orderID which is later used to retrieve
        // the verification status.
        mPendingPurchases[orderId] =  args.purchasedProduct;

        // ...and start the receipt validation request.
        KongregateAPI.GetAPI().Mtx.VerifyGoogleReceipt(googleReceiptJSON, googleSignature);

        #elif UNITY_IPHONE

        // for iOS we key purchases on the the transactionID which is later used to retrieve
        // verification status
        mPendingPurchases[args.purchasedProduct.transactionID] =  args.purchasedProduct;

        // for iOS, simply pass the transaction ID to our API
        KongregateAPI.GetAPI().Mtx.VerifyTransactionId(args.purchasedProduct.transactionID);

        #endif

        // return pending so the transaction is not completed until after we validate the receipt
        return PurchaseProcessingResult.Pending;
      }
      else {
        Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        KongregateAPI.GetAPI().Analytics.FinishPurchase(KongregateAPI.PURCHASE_FAIL,
          "Unrecognized product", new Dictionary<string,object>(){{ "type", "Gold Pack"}});

        // return complete to finish the transaction
        return PurchaseProcessingResult.Complete;
      }
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
      // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
      Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",product.definition.storeSpecificId, failureReason));
    }

    /**
     * Handle the KongregateAPI.KONGREGATE_EVENT_RECEIPT_VERIFICATION_COMPLETE event.
     */
    void OnReceiptVerificationComplete() {
      Debug.Log ("OnReceiptVerificationComplete");

      // dummy purchase field values to pass to the Kongregate SDK. replace
      // these with any non-auto fields from your schema for the
      // iap_transactions/iap_fails events.
      Dictionary<string,object> purchaseFields = new Dictionary<string,object> ()
      {{ "hard_currency_change",  5 },
       { "soft_currency_change", 10 },
       { "type", "Gold Pack" }};

      // Iterate over the list of pending transactions and resolve any that have
      // completed receipt validation.
      Dictionary<string,Product> stillInVerification = new Dictionary<string,Product> ();
      foreach (string transactionId in mPendingPurchases.Keys) {
        string status = KongregateAPI.GetAPI().Mtx.ReceiptVerificationStatus (transactionId);
        if (status != KongregateAPI.RECEIPT_VERIFICATION_STATUS_PROCESSING) {
          Debug.Log ("Verification for " + transactionId + " is " + status);
          Product purchasedProduct = mPendingPurchases[transactionId];
          string fullReceipt = purchasedProduct.receipt;

          #if UNITY_ANDROID
          // Parse the Google receipt to pass to the Kongregate API
          Dictionary<string,object> receipt = Json.Deserialize(fullReceipt) as Dictionary<string,object>;
          Dictionary<string,object> payload = Json.Deserialize(receipt["Payload"] as string) as Dictionary<string,object>;
          string googleReceiptJSON = payload["json"] as string;
          string googleSignature = payload["signature"] as string;
          if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_VALID) {
            KongregateAPI.GetAPI ().Analytics.FinishPurchase(KongregateAPI.PURCHASE_SUCCESS,
              googleReceiptJSON, purchaseFields, googleSignature);
          } else if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_INVALID) {
            KongregateAPI.GetAPI ().Analytics.FinishPurchase(KongregateAPI.PURCHASE_RECEIPT_FAIL,
              googleReceiptJSON, purchaseFields);
          }

          #elif UNITY_IPHONE
          // For iOS we simply need the transactionId
          if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_VALID) {
            KongregateAPI.GetAPI ().Analytics.FinishPurchase(KongregateAPI.PURCHASE_SUCCESS,
              transactionId, purchaseFields);
          } else if (status == KongregateAPI.RECEIPT_VERIFICATION_STATUS_INVALID) {
            KongregateAPI.GetAPI ().Analytics.FinishPurchase(KongregateAPI.PURCHASE_RECEIPT_FAIL,
              transactionId, purchaseFields);
          }
          #endif

          // Notify the Unity Purchasing API to finish/consume the purchase
          mStoreController.ConfirmPendingPurchase(purchasedProduct);
        } else {
          Debug.Log ("Still waiting on verification for " + transactionId);
          stillInVerification[transactionId] = mPendingPurchases[transactionId];
        }
      }
      mPendingPurchases = stillInVerification;
    }
  }
}

#else
namespace Kongregate
{
  // stubbed out version
  public class KongPurchasingDemo : MonoBehaviour
  {
  }
}
#endif
