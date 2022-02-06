﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class FailedWindowController : PopUpsController
    {
        [SerializeField]
        private Text levelText;
        [SerializeField]
        private GameObject scoreComplete;
        [SerializeField]
        private GameObject scoreUnComplete;

        [Space(8)]
        [Header("Targets")]
        [SerializeField]
        private Text coinsText;
        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private GUIObjectTargetHelper targetPrefab;
        [SerializeField]
        private RectTransform targetsContainer;

        [Space(8)]
        [Header("Boosters")]
        [SerializeField]
        private MissionBoosterHelper missionBoosterPrefab;
        [SerializeField]
        private RectTransform BoostersParent;
        [SerializeField]
        private int boostersCount = 3;
        [SerializeField]
        private bool useAds = true;

        private static int failsCounter = 0;

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        private google_Mobile_Ads MobAds { get { return google_Mobile_Ads.Instance; } }

        public int play_stat = 0;

        public static FailedWindowController Instance;
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
            coinsText.text = (Mathf.RoundToInt(MPlayer.LevelScore / MPlayer.koeffCoinsScore).ToString());

            failsCounter++;
            if (useAds && failsCounter % 2 == 0)
            {
                Debug.Log("show ads");
               // AdsControl.Instance.ShowAdInt();
            }
        }

        public override void RefreshWindow()
        {
            CreateTargets();
            if (levelText) levelText.text = (MatchPlayer.CurrentLevel + 1).ToString();
          //  RefreshCoins();
            //RefreshScore();
            CreateBoostersPanel();
            base.RefreshWindow();
        }

        private void RefreshCoins()
        {
            //int coins = Mathf.RoundToInt(MPlayer.LevelScore / MPlayer.koeffCoinsScore);
            //Debug.Log("coins: " + coins + " : " + MBoard.WinContr.ScoreTarget.ToString());
            //if (coinsText) scoreText.text = coins.ToString();
            //scoreComplete?.SetActive(MBoard.WinContr.HasScoreTarget && (coins >= MBoard.WinContr.ScoreTarget));
            //scoreUnComplete?.SetActive(MBoard.WinContr.HasScoreTarget && (coins < MBoard.WinContr.ScoreTarget));
        }

        private void RefreshScore()
        {
            int score = MPlayer.LevelScore;
            Debug.Log("score: " + score + " : " + MBoard.WinContr.ScoreTarget.ToString());
            if (scoreText) scoreText.text = score.ToString();
            scoreComplete?.SetActive(MBoard.WinContr.HasScoreTarget && (score >= MBoard.WinContr.ScoreTarget));
            scoreUnComplete?.SetActive(MBoard.WinContr.HasScoreTarget && (score < MBoard.WinContr.ScoreTarget));
        }

        public void Map_Click()
        {
          //  MobAds.DestroyBanner();
           // MobAds.ShowAds();
            FortuneShow();
           // CloseWindow();
           // SceneLoader.Instance.LoadScene(0);
           //  MGui.ShowMessage("Look in the chest!", "You can find a lot of bonuses", 1.5f, () => { SceneLoader.Instance.LoadScene(0, ()=> { StartMapGuiController.Instance?.ShowFortune_Click(); }); });
        }

        public void FortuneShow()
        {
            if (MPlayer.Fortune > 0)
                MGui.ShowFortune(null);
            else
                MGui.ShowNonFortune(null);
        }
        

        public void Replay_Click()
        {
            MobAds.ShowAds();

            if (MPlayer.Life > 0 || MPlayer.HasInfiniteLife())
            {
               // MPlayer.AddLifes(-1);
             //   MatchBoard.showMission = false;
                CloseWindow();
                SceneLoader.Instance.LoadScene(1);
            }
            else
            {
                // CloseWindow();
                MGui.ShowMessage("Sorry!", "You have no lifes.", 1.5f, () => { MGui.ShowLifeShop(); });  //SceneLoader.Instance.LoadScene(0); });
            }
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
                    MissionBoosterHelper bM = MissionBoosterHelper.CreateMissionBooster(BoostersParent, missionBoosterPrefab, b, () => { MGui.ShowInGameShopBooster(id); });
                }
            }
        }

        private void CreateTargets()
        {
            if (!targetsContainer) return;
            if (!targetPrefab) return;

            GUIObjectTargetHelper[] ts = targetsContainer.GetComponentsInChildren<GUIObjectTargetHelper>();
            foreach (var item in ts)
            {
                Destroy(item.gameObject);
            }

            foreach (var item in MBoard.Targets)
            {
                RectTransform t = Instantiate(targetPrefab, targetsContainer).GetComponent<RectTransform>();
                GUIObjectTargetHelper th = t.GetComponent<GUIObjectTargetHelper>();
                th.SetData(item.Value, false, true);
                th.SetIcon(MBoard.MatchSet.GetObject(item.Value.ID).GuiImage);
                item.Value.ChangeCountEvent += (td) => { if (this) { th.SetData(td, false); th.gameObject.SetActive(td.NeedCount > 0); } };
                th.gameObject.SetActive(item.Value.NeedCount > 0);
            }
        }
    }
}
