using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class StartMapGuiController : MonoBehaviour
    {
        [SerializeField]
        private Text lifesText;
        [SerializeField]
        private Text coinsText;
        [SerializeField]
        private Image infiniteIcon;
        [SerializeField]
        private Text timerText;
        [SerializeField]
        private Text fortuneText;

        #region temp
        private float restHours = 0;
        private float restMinutes = 0;
        private float restSeconds = 0;
        #endregion temp

        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MapController MContr { get { return MapController.Instance; } }

        public static StartMapGuiController Instance;

        #region regular
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            MPlayer.ChangeCoinsEvent += RefreshCoins;
            MPlayer.ChangeFortuneEvent += RefreshFortuna;
            MPlayer.ChangeLifeEvent += RefreshLife;
            MPlayer.StartInfiniteLifeEvent += RefreshInfiniteLife;
            MPlayer.EndInfiniteLifeEvent += RefreshInfiniteLife;
            if (timerText) timerText.text = restMinutes.ToString("00") + ":" + restSeconds.ToString("00");
            Refresh();
        }

        void OnGUI()
        {
            RefreshTimerText();
        }

        private void OnDestroy()
        {
            MPlayer.ChangeCoinsEvent -= RefreshCoins;
            MPlayer.ChangeFortuneEvent -= RefreshFortuna;
            MPlayer.ChangeLifeEvent -= RefreshLife;
            MPlayer.StartInfiniteLifeEvent -= RefreshInfiniteLife;
            MPlayer.EndInfiniteLifeEvent -= RefreshInfiniteLife;
        }
        #endregion regular 

        private void RefreshTimerText()
        {
            LifeIncTimer lifeIncTimer = LifeIncTimer.Instance;
            InfiniteLifeTimer infiniteLifeTimer = InfiniteLifeTimer.Instance;
            if (timerText)
            {
                if (infiniteLifeTimer && infiniteLifeTimer.IsWork)
                {
                    if (restHours != infiniteLifeTimer.RestHours || restMinutes != infiniteLifeTimer.RestMinutes || restSeconds != infiniteLifeTimer.RestSeconds)
                    {
                        restHours = infiniteLifeTimer.RestHours;
                        restMinutes = infiniteLifeTimer.RestMinutes;
                        restSeconds = infiniteLifeTimer.RestSeconds;
                        timerText.text = restHours.ToString("00") + ":" + restMinutes.ToString("00"); // + ":" + restSeconds.ToString("00");
                    }
                    if (lifesText && lifesText.gameObject.activeSelf) lifesText.gameObject.SetActive(false);
                    if (infiniteIcon && !infiniteIcon.gameObject.activeSelf) infiniteIcon.gameObject.SetActive(true);
                    return;
                }

                if (lifeIncTimer)
                {
                    if (restMinutes != lifeIncTimer.RestMinutes || restSeconds != lifeIncTimer.RestSeconds)
                    {
                        restMinutes = lifeIncTimer.RestMinutes;
                        restSeconds = lifeIncTimer.RestSeconds;
                        timerText.text = restMinutes.ToString("00") + ":" + restSeconds.ToString("00");
                    }
                    if (lifesText && !lifesText.gameObject.activeSelf) lifesText.gameObject.SetActive(true);
                    if (infiniteIcon && infiniteIcon.gameObject.activeSelf) infiniteIcon.gameObject.SetActive(false);
                }
            }
        }

        private void Refresh()
        {////////////////////////////////////
            RefreshLife(MPlayer.Life);
            RefreshCoins(MPlayer.Coins);
            RefreshFortuna(MPlayer.Fortune);
            RefreshTimerText();
        }

        /// <summary>
        /// Set all interactable as activity
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }


        #region event handlers
        private void RefreshCoins(int coins)
        {
            if (coinsText) coinsText.text = coins.ToString();
        }

        private void RefreshFortuna(int fortuns)
        {
            if (fortuneText) fortuneText.text = fortuns.ToString();
        }

        private void RefreshLife(int life)
        {
            if (lifesText) lifesText.text = life.ToString();
        }

        private void RefreshInfiniteLife()
        {
            if (infiniteIcon) infiniteIcon.gameObject.SetActive(MPlayer.HasInfiniteLife());
            if (lifesText) lifesText.enabled = !MPlayer.HasInfiniteLife();
        }
        #endregion event handlers 

        public void ShowBoosterShop_Click()
        {
            MGui?.ShowInGameShopBooster(null);
        }

        public void ShowRealCoinsShop_Click()
        {
            MGui?.ShowRealCoinsShop();
        }

        public void ShowRealLifeShop_Click()
        {
            MGui?.ShowLifeShop();
        }

        public void ShowSettings_Click()
        {
            MGui?.ShowSettings();
        }

        public void ShowGame_Click()
        {
            MContr?.ClickStartGame();
        }

        public void ShowRate_Click()
        {
            MGui?.ShowRate(null);
        }

        public void ShowInventory_Click()
        {
            MGui?.ShowInventory(null);
        }

        public void ShowSale_Click()
        {
            MGui?.ShowSale(null);
        }


        public void ShowFortune_Click()
        {
            string key = "mkmatch_" + "fortuna"; // current fortunes
            int fort = 0;
            if (PlayerPrefs.HasKey(key)) 
                 fort = PlayerPrefs.GetInt(key);
            if (fort > 0)
                MGui?.ShowFortune(null);
            else
                MGui?.ShowNonFortune(null);
        }
    }
}