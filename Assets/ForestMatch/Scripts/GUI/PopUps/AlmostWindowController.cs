using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class AlmostWindowController : PopUpsController
    {
        [SerializeField]
        private Text coinsText;
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private VideoPurchaser VidPurch { get { return VideoPurchaser.Instance; } }

        private MatchGUIController MGUICtrl { get { return MatchGUIController.Instance; } }

        private int  Coins{ get; set; }

        public GameObject VideoButton;

        public static AlmostWindowController Instance;
        void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (VidPurch.countVideoConti > 0) 
                VideoButton.gameObject.SetActive(true);
            else
                VideoButton.gameObject.SetActive(false);

            coinsText.text = (Mathf.RoundToInt(MPlayer.LevelScore / MPlayer.koeffCoinsScore).ToString());
        }
        public override void RefreshWindow()
        {
            base.RefreshWindow();
        }

        public void SetCoins(int coins)
        {
            Coins = coins;
            if (coinsText) coinsText.text = Coins.ToString();
        }

        public void Close_Click()
        {
            //play_stat = 1;
            CloseWindow();
            MPlayer.AddLifes(-1);
            MBoard.showAlmostMessage = false;
            MBoard.WinContr.CheckResult();
        }

        public void Continue_Coin()
        {
           // play_stat = 1;


            if (MPlayer.Coins > MPlayer.priceCoinsCont)
            {
                CloseWindow();
                MPlayer.AddCoins(- MPlayer.priceCoinsCont);
                MBoard.WinContr.AddMoves(5);
            }
            else
                MGUICtrl.ShowRealCoinsShop();
            
        }

        public void Play_Reward()
        {
           // play_stat = 1;
            VidPurch.ShowVideo(VidPurch.gameProducts[1].kProductID, VidPurch.gameProducts[1].name, 0, 0);
        }

        public void Reward_Continue()
        {
            CloseWindow();
            MBoard.WinContr.AddMoves(5);
        }

        public void Restart_Click()
        {
           // play_stat = 1;

            MBoard.match_stat = 1;

            CloseWindow();
            MGUICtrl.ShowFailed(null);
            MPlayer.AddLifes(-1);
            foreach (var item in MPlayer.BoostHolder.Boosters)
            {
                if (item.Use) item.ChangeUse();
            }
           

            //MatchBoard.showMission = false;
            //CloseWindow();
            //SceneLoader.Instance.LoadScene(1);
        }

    }
}
