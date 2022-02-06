using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds.Api;

namespace Mkey
{
    public class google_Mobile_Ads : MonoBehaviour
    {
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MissionWindowController MWContr { get { return MissionWindowController.Instance; } }
        private AlmostWindowController AWContr { get { return AlmostWindowController.Instance; } }
        private FailedWindowController FWContr { get { return FailedWindowController.Instance; } }

        private BannerView bannerView;
        private InterstitialAd interstitial;

        public int freqInters = 2;
        private int tryLevelCount;

        private static string savePrefix = "mkmatch_";
        private string saveStatPurKey = savePrefix + "status_pur"; // status purchase

        private int stat = 0;

        public static google_Mobile_Ads Instance;

        String testDeviceId = "6B81F84A75ECB91C";

        void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }
        public void Start()
        {
            MobileAds.Initialize(initStatus => {
                Debug.Log("-== Admob Initialize ==-");
            });

            if (PlayerPrefs.HasKey(saveStatPurKey))
                stat = PlayerPrefs.GetInt(saveStatPurKey);

           if(stat == 0) 
                RequestInterstitial();
        }

        public void ShowAds()
        {
            if (stat != 0)
                return;

            if (MatchPlayer.CurrentLevel > 4)
            {
                tryLevelCount++;
                 if (tryLevelCount % freqInters == 0)
                 {
                        RequestBanner();
                        InterstitialAd();
                 }
                 else
                        RequestBanner();
            }
        }
        public void PurchDoNotAds()
        {
            if (PlayerPrefs.HasKey(saveStatPurKey))
                stat = PlayerPrefs.GetInt(saveStatPurKey);
            if (stat < 1)
                PlayerPrefs.SetInt(saveStatPurKey, 1);
        }

        /// //*** Banner *******
        /// 

        public void DestroyBanner()
        {
            if (bannerView != null)
                this.bannerView.Destroy();
        }

        public void ShowBanner()
        {
            RequestBanner();
        }
        private void RequestBanner()
        {
#if UNITY_ANDROID
         //   string adUnitId = "cca-app-pub-3940256099942544/6300978111"; //test
            string adUnitId = "ca-app-pub-1984133919979899/5747389811";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

            // Clean up banner ad before creating a new one.
            if (this.bannerView != null)
            {
                this.bannerView.Destroy();
            }

          //  AdSize adaptiveSize =
         //       AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
              AdSize adSize = new AdSize(320, 100);//AdSize.Banner
            bannerView = new BannerView(adUnitId, adSize, AdPosition.Bottom);

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().
                                 ////AddTestDevice(AdRequest.TestDeviceSimulator).
                                 ////AddTestDevice("6B81F84A75ECB91C").
                                 Build();

            // Load a banner ad.
            bannerView.LoadAd(request);

        }


        ////*********  Interstitial ********************
        private void RequestInterstitial()
        {
#if UNITY_ANDROID
          //  string adUnitId = "ca-app-pub-3940256099942544/1033173712";//test
            string adUnitId = "ca-app-pub-1984133919979899/1286057168";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
            // Clean up interstitial before using it
            if (this.interstitial != null)
            {
                this.interstitial.Destroy();
            }

            // Initialize an InterstitialAd.
            this.interstitial = new InterstitialAd(adUnitId);
            this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
            this.interstitial.OnAdFailedToLoad += (sender, args) =>
            {
                Instance.Invoke(nameof(RequestInterstitial), 10);
            };

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().
                                 ////AddTestDevice(AdRequest.TestDeviceSimulator).
                                 ////AddTestDevice("6B81F84A75ECB91C").
                                 Build();

            // Load the interstitial with the request.
            this.interstitial.LoadAd(request);
        }
        public void HandleInterstitialClosed(object sender, EventArgs args)
        {
            RequestInterstitial();
        }
        public void InterstitialAd()
        {
            if (this.interstitial.IsLoaded()) 
                this.interstitial.Show();
        }
    }
}
