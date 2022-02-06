using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class InventoryWindowController : PopUpsController
    {
        [Space(8)]
        [Header("Boosters")]
        [SerializeField]
        private MissionBoosterHelper missionBoosterPrefab;
        [SerializeField]
        private RectTransform BoostersParent;

        private Sprite defaultAvatar;

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }

        public override void RefreshWindow()
        {
            CreateBoostersPanel();
            base.RefreshWindow();
        }

        private void CreateBoostersPanel()
        {
            MissionBoosterHelper[] ms = BoostersParent.GetComponentsInChildren<MissionBoosterHelper>();
            foreach (MissionBoosterHelper item in ms)
            {
                DestroyImmediate(item.gameObject);
            }
            List<Booster> bList = new List<Booster>();

            foreach (var b in MPlayer.BoostHolder.Boosters)
            {
               // if (b.Count > 0)
                    bList.Add(b);
            }

          //  bList.Shuffle();
            for (int i = 0; i < bList.Count; i++)
            {
                Booster b = bList[i];
                string id = b.bData.ID.ToString();
                MissionBoosterHelper bM = MissionBoosterHelper.CreateProfileBooster(BoostersParent, missionBoosterPrefab, b, () => { MGui.ShowInGameShopBooster(id); });
            }
        }
    }
}