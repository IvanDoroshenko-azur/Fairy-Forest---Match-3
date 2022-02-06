using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace Mkey
{
    public class VideoPurchaser : MonoBehaviour
    {
        public ShopThingDataReal[] gameProducts;

        public int video_ads_mode = 0;
        public int video_ads_rew = 0;

        public int countVideoConti = 1;
        public int countVideoLifes = 3;
        public Action <int> VideoRewardComplete;
        public Action<int> VideoRewardCompleteLife;

        public static VideoPurchaser Instance;

        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        private AlmostWindowController AVContr { get { return AlmostWindowController.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }

        private bool result = false;
        private int countRew=0;
        private string prodIDRew, prodNameRew;

        //  private RewardedAd rewardedAd;
        private RewardedAd rewardedAd;
       // public int limitReward=2;

        void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        void Start()
        {        
            CreateAndLoadRewardedAd();

            InitializePurchasing();
        }

        /// <summary>
        /// Add for each button product clickEevnt
        /// </summary>
        private void InitializePurchasing()
        {
            if (gameProducts != null && gameProducts.Length > 0)
            {
                for (int i = 0; i < gameProducts.Length; i++)
                {
                    if (gameProducts[i] != null && !string.IsNullOrEmpty(gameProducts[i].kProductID))
                    {
                        string prodID = gameProducts[i].kProductID;
                        string prodName = gameProducts[i].name;
                        int count = gameProducts[i].thingCount;
                        int price = (int)gameProducts[i].thingPrice;

                        gameProducts[i].clickEvent.RemoveAllListeners();
                        gameProducts[i].clickEvent.AddListener(() => { ShowVideo(prodID, prodName, count, price); });
                    }
                }
            }
        }

        public void CreateAndLoadRewardedAd()
        {
#if UNITY_ANDROID
            //string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // test
            string adUnitId = "ca-app-pub-1984133919979899/3248167957";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            string adUnitId = "unexpected_platform";
#endif

            rewardedAd = new RewardedAd(adUnitId);

            // Called when an ad request has successfully loaded.
            rewardedAd.OnAdLoaded += HandleRewardedVideoLoaded;
            // Called when an ad request failed to load.
            //rewardedAd.OnAdFailedToLoad += HandleRewardedVideoFailedToLoad;
            // Called when an ad is shown.
            rewardedAd.OnAdOpening += HandleRewardedVideoOpened;
            // Called when an ad request failed to show.
            rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            // Called when the ad is closed.
            rewardedAd.OnAdClosed += HandleRewardedVideoClosed;

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded ad with the request.
            rewardedAd.LoadAd(request);
        }

        #region RewardedAd callback handlers

        public void HandleRewardedVideoLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedVideoLoaded event received");
        }

        public void HandleRewardedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            MonoBehaviour.print(
                "HandleRewardedVideoFailedToLoad event received with message: "
                                 );
        }

        public void HandleRewardedVideoOpened(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedVideoOpened event received");
        }


        public void HandleRewardedVideoClosed(object sender, EventArgs args)
        {
            CreateAndLoadRewardedAd();
        }

        public void HandleUserEarnedReward(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            //MonoBehaviour.print("HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);

            if (prodNameRew == "Add 1 life")
            {
                AddLife(countRew);
                GoodPurchaseMessage(prodIDRew, prodNameRew);
                if (countVideoLifes > 0)
                    countVideoLifes--;
                VideoRewardCompleteLife?.Invoke(countVideoLifes);
            }
            else
            {
                AVContr.Reward_Continue();
                if (countVideoConti > 0)
                    countVideoConti--;
                VideoRewardComplete?.Invoke(countVideoConti);
            }
        }

        #endregion

        public void VideoCountLifeAdd(int l)
        {
            countVideoLifes += l;
            if (countVideoLifes < 0)
                countVideoLifes = 0;
        }

        public void VideoCountAdd(int l)
        {
            countVideoConti += l;
            if (countVideoConti < 0)
                countVideoConti = 0;
        }

        private void ShowRewardedAd()
        {
            if (rewardedAd.IsLoaded())
            {
              rewardedAd.Show();
              Debug.Log("Show Video");
            }
            else
            {
              Debug.Log("Rewarded ad is not ready yet");
               FailedPurchaseMessage(prodIDRew, prodNameRew);
            }
        }

        /// <summary>
        /// Buy product, increase product count
        /// </summary>
        /// <param name="prodID"></param>
        /// <param name="prodName"></param>
        /// <param name="count"></param>
        /// <param name="price"></param>
        public void ShowVideo(string prodID, string prodName, int count, int price)
        {

            result = false;
            Debug.Log("----insert video start code---");
            countRew = count;
            prodIDRew = prodID;
            prodNameRew = prodName;
 
            ShowRewardedAd();
        }

        public void AddCoins(int count)
        {
            MPlayer.AddCoins(count);
        }

        public void SetInfiniteLife(int hours)
        {
            MPlayer.StartInfiniteLife(hours);
        }

        public void AddLife(int count)
        {
            MPlayer.AddLifes(count);
        }

        /// <summary>
        /// Show good purchase message
        /// </summary>
        /// <param name="prodId"></param>
        /// <param name="prodName"></param>
        private void GoodPurchaseMessage(string prodId, string prodName)
        {
            MGui?.ShowMessage("Succesfull!!!", "Added 1 life", 2, null);
        }

        /// <summary>
        /// Show failed purchase message
        /// </summary>
        /// <param name="prodId"></param>
        /// <param name="prodName"></param>
        private void FailedPurchaseMessage(string prodId, string prodName)
        {
            MGui?.ShowMessage("Sorry!", "Not received", 2, null);
        }

        /// <summary>
        /// Search in array gameProducts appropriate product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShopThingDataReal GetProductById(string id)
        {
            if (gameProducts != null && gameProducts.Length > 0)
                for (int i = 0; i < gameProducts.Length; i++)
                {
                    if (gameProducts[i] != null)
                        if (String.Equals(id, gameProducts[i].kProductID, StringComparison.Ordinal))
                            return gameProducts[i];
                }
            return null;
        }
    }
}