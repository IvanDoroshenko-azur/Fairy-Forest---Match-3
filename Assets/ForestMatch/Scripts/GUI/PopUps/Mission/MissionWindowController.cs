using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

namespace Mkey
{
    public class MissionWindowController : PopUpsController
    {
        [SerializeField]
        private Text levelText;

        [Space(8)]       
        [Header ("Targets")]
        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private Text getScoreText;
        [SerializeField]
        private RectTransform targetsContainer;
        [SerializeField]
        private MissionTarget targetPrefab;

        [Space(8)]
        [Header("Boosters")]
        [SerializeField]
        private MissionBoosterHelper missionBoosterPrefab;
        [SerializeField]
        private RectTransform BoostersParent;
        [SerializeField]
        private int boostersCount = 3;

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        public MatchGUIController MGui { get { return MatchGUIController.Instance; } }

        public google_Mobile_Ads MobAds { get { return google_Mobile_Ads.Instance; } }

        public static MissionWindowController Instance;

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
            levelText.text = (MatchPlayer.CurrentLevel +1).ToString();
            getScoreText.gameObject.SetActive(MBoard.WinContr.HasScoreTarget);
            scoreText.text = MBoard.WinContr.ScoreTarget.ToString();
            CreateTargets();
            CreateBoostersPanel();
            base.RefreshWindow();
        }

        public void CreateTargets()
        {
            if (!targetsContainer) return;
            if (!targetPrefab) return;

            MissionTarget[] ts = targetsContainer.GetComponentsInChildren<MissionTarget>();
            foreach (var item in ts)
            {
                DestroyImmediate(item.gameObject);
            }

            foreach (var item in MBoard.Targets)
            {
                targetPrefab.SetIcon(MBoard.MatchSet.GetObject(item.Value.ID).GuiImage); // unity 2019 fix
                
                RectTransform t = Instantiate(targetPrefab, targetsContainer).GetComponent<RectTransform>();
                MissionTarget th = t.GetComponent<MissionTarget>();
                th.SetData(item.Value, true);
                th.SetIcon(MBoard.MatchSet.GetObject(item.Value.ID).GuiImage);
                th.gameObject.SetActive(item.Value.NeedCount > 0);
            }
        }

        public void Play_Click()
        {
            AnalyticsEvent.Custom("Play_Level", new Dictionary<string, object>
            {
               { "Level", MatchPlayer.CurrentLevel + 1 }
            });
            MobAds.DestroyBanner();//play_stat = 1;

            CloseWindow();
        }

        public void Show_SaleBoard()
        {
            MGui?.ShowSale(null);
        }

        public void ShowMarket()
        {
            MGui.ShowRealCoinsShop();
        }


        public void ShowFortune()
        {
            MGui.ShowFortune(null);
        }


        private void CreateBoostersPanel()
        {
            MissionBoosterHelper[] ms = BoostersParent.GetComponentsInChildren<MissionBoosterHelper>();
            foreach (MissionBoosterHelper item in ms)
            {
                DestroyImmediate(item.gameObject);
            }
            List<Booster> bList = new List<Booster>();
            List<Booster> bListToShop = new List<Booster>();

            bool selectFromAll = true;

            if (!selectFromAll)
            {
                foreach (var b in MPlayer.BoostHolder.Boosters)
                {
                    if (b.Count > 0) bList.Add(b);
                    else bListToShop.Add(b);
                }

                bList.Shuffle();
                int bCount = Mathf.Min(bList.Count, boostersCount);
                for (int i = 0; i < bCount; i++)
                {
                    Booster b = bList[i];
                    string id = b.bData.ID.ToString();
                    string name = b.bData.ObjectImage.name.ToString();
                    MissionBoosterHelper bM = MissionBoosterHelper.CreateMissionBooster(BoostersParent, missionBoosterPrefab, b, () => { MGui.ShowInGameShopBooster(id, name); });
                }

                int shopCount = boostersCount - bList.Count;
                if (shopCount > 0)
                {
                    shopCount = Mathf.Min(shopCount, bListToShop.Count);
                    bListToShop.Shuffle();

                    for (int i = 0; i < shopCount; i++)
                    {
                        Booster b = bListToShop[i];
                        string id = b.bData.ID.ToString();
                        string name = b.bData.ObjectImage.name.ToString();
                        MissionBoosterHelper bM = MissionBoosterHelper.CreateMissionBooster(BoostersParent, missionBoosterPrefab, b, () => { MGui.ShowInGameShopBooster(id, name); });
                    }
                }
            }
            else
            {
                foreach (var b in MPlayer.BoostHolder.Boosters)
                {
                    bList.Add(b);
                }

                bList.Shuffle();
                int bCount = Mathf.Min(bList.Count, boostersCount);
                for (int i = 0; i < bCount; i++)
                {
                    Booster b = bList[i];
                    string id = b.bData.ID.ToString();
                    string name = b.bData.ObjectImage.name.ToString();
                    MissionBoosterHelper bM = MissionBoosterHelper.CreateMissionBooster(BoostersParent, missionBoosterPrefab, b, () => { MGui.ShowInGameShopBooster(id, name); });
                }
            }
        }

        public void ToMap_Click()
        {
            MobAds.DestroyBanner();//  play_stat = 1;
            CloseWindow();
            SceneLoader.Instance.LoadScene(0);
        }
    }
}