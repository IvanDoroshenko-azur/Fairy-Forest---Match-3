using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

namespace Mkey
{
    public class SaleWindowController : PopUpsController
    {
        [SerializeField]
        private Text levelText;

        [Space(8)]
        [Header("Targets")]
        int a;

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        public MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        private Purchaser MPurch { get { return Purchaser.Instance; } }

        public int play_stat = 0;

        public static SaleWindowController Instance;

        void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }
        public override void RefreshWindow()
        {        //" Level #"
         //   levelText.text = (MatchPlayer.CurrentLevel +1).ToString();
            base.RefreshWindow();
        }


        public void Play_Click()
        {
            AnalyticsEvent.Custom("Play_Level", new Dictionary<string, object>
            {
               { "Level", MatchPlayer.CurrentLevel + 1 }
            });
            play_stat = 1;

            CloseWindow();
        }


        public void ToMap_Click()
        {
            play_stat = 1;
            CloseWindow();
            SceneLoader.Instance.LoadScene(0);
        }

        public void Buy_Begin_Click()
        {
          //  MPurch.BuyBeginPack();
        }

        public void Buy_Medium_Click()
        {
          //  MPurch.BuyMediumPack();
        }

        public void Buy_Profy_Click()
        {
          //  MPurch.BuyProfyPack();
        }
    }
}