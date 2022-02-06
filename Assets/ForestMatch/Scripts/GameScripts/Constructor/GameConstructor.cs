using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mkey
{
    public class GameConstructor : MonoBehaviour
    {
#if UNITY_EDITOR

     //   private List<RectTransform> openedPanels;
     //   private List<MatchObjectData> matchObjects;


        [SerializeField]
        private Text editModeText;

        #region selected brush
        [Space(8, order = 0)]
        [Header("Grid Brushes", order = 1)]

        [SerializeField]
        private Image tutorialBrushImage;
        [SerializeField]
        private Image selectedTutorialBrushImage;
        [SerializeField]
        private PanelContainerController TutorialBrushContainer;
        private BaseObjectData tutorialBrush;

        [Space(8)]
        [SerializeField]
        private Image matchedBrushImage;
        [SerializeField]
        private Image selectedMatchedBrushImage;
        [SerializeField]
        private PanelContainerController MatchedBrushContainer;
        private BaseObjectData matchedBrush;

        [Space(8)]
        [SerializeField]
        private Image bombsBrushImage;
        [SerializeField]
        private Image selectedBombsBrushImage;
        [SerializeField]
        private PanelContainerController BombsBrushContainer;
        private BaseObjectData bombsBrush;

        [Space(8)]
        [SerializeField]
        private Image mainBrushImage;
        [SerializeField]
        private Image selectedMainBrushImage;
        [SerializeField]
        private PanelContainerController MainBrushContainer;
        private BaseObjectData mainBrush;

        [Space(8)]
        [SerializeField]
        private Image overBrushImage;
        [SerializeField]
        private Image selectedOverBrushImage;
        [SerializeField]
        private PanelContainerController OverBrushContainer;
        private BaseObjectData overBrush;

        [Space(8)]
        [SerializeField]
        private Image underBrushImage;
        [SerializeField]
        private Image selectedUnderBrushImage;
        [SerializeField]
        private PanelContainerController UnderBrushContainer;
        private BaseObjectData underBrush;

        [Space(8)]
        [SerializeField]
        private Image disabledBrushImage;
        [SerializeField]
        private Image selectedDisabledBrushImage;
        private BaseObjectData disabledBrush;

        [Space(8)]
        [SerializeField]
        private Image blockedBrushImage;
        [SerializeField]
        private Image selectedBlockedBrushImage;
        [SerializeField]
        private PanelContainerController BlockedBrushContainer;
        private BaseObjectData blockedBrush;

        [Space(8)]
        [SerializeField]
        private Image fallingBrushImage;
        [SerializeField]
        private Image selectedFallingBrushImage;
        [SerializeField]
        private PanelContainerController FallingBrushContainer;
        private BaseObjectData fallingBrush;
        #endregion selected brush


        #region gift
        //[Space(8, order = 0)]
        //[Header("Gift", order = 1)]
        //[SerializeField]
        //private PanelContainerController GiftPanelContainer;
        [SerializeField]
        private IncDecInputPanel IncDecPanelPrefab;
        #endregion gift

        #region match
        [Space(8, order = 0)]
        [Header("Match", order = 1)]
        [SerializeField]
        private PanelContainerController MatchPanelContainer;
        [SerializeField]
        private Image MatchImage;
        [SerializeField]
        private IncDecInputPanel TogglePanelMatchPrefab;
        [SerializeField]
        private Sprite MatchDarkImage;

        //  [SerializeField]
        //   private Image underMatchImage;
        //  [SerializeField]
        //    private BaseObjectData underMatch;
        #endregion match

        #region mission
        [Space(8, order = 0)]
        [Header("Mission", order = 1)]
        [SerializeField]
        private PanelContainerController MissionPanelContainer;
        [SerializeField]
        private IncDecInputPanel InputTextPanelMissionPrefab;
        [SerializeField]
        private IncDecInputPanel IncDecTogglePanelMissionPrefab;
        [SerializeField]
        private IncDecInputPanel TogglePanelMissionPrefab;
        #endregion mission

        #region grid construct
        [Space(8, order = 0)]
        [Header("Grid", order = 1)]
        [SerializeField]
        private PanelContainerController GridPanelContainer;
        [SerializeField]
        private IncDecInputPanel IncDecGridPrefab;
        #endregion grid construct

        #region game construct
        [Space(8, order = 0)]
        [Header("Game construct", order = 0)]
        [SerializeField]
        private GameObject levelButtonPrefab;
        [SerializeField]
        private GameObject smallButtonPrefab;
        [SerializeField]
        private GameObject constructPanel;
        [SerializeField]
        private Button openConstructButton;
        [SerializeField]
        private ScrollRect LevelButtonsContainer;
        #endregion game construct

        #region private
        private LevelConstructSet lcSet;
        private MissionConstruct levelMission;
        private Dictionary<int, TargetData> targets;
        private GameConstructSet gcSet;
        private GameObjectsSet goSet;
        #endregion private

        public GridCell selected;
        public int selectedTarget = 0;

        //resource folders
        private string levelConstructSetSubFolder = "LevelConstructSets";

        private int minVertSize = 5;
        private int maxVertSize = 15;
        private int minHorSize = 5;
        private int maxHorSize = 15;

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }

        public void InitStart(GameConstructSet gcSet)
        {
            if (MatchBoard.GMode == GameMode.Edit)
            {
                if (!MBoard) return;
                if (!MPlayer) return;

                Debug.Log("gc init");
                this.gcSet = gcSet;
                if (!gcSet)
                {
                    Debug.Log("Game construct set not found!!!");
                    return;
                }
                if (!gcSet.GOSet)
                {
                    Debug.Log("GameObjectSet not found!!! - ");
                    return;
                }

                tutorialBrush = gcSet.GOSet.Disabled;
                tutorialBrushImage.sprite = tutorialBrush.ObjectImage;

                matchedBrush = gcSet.GOSet.Disabled;
                matchedBrushImage.sprite = matchedBrush.ObjectImage;

                bombsBrush = gcSet.GOSet.Disabled;
                bombsBrushImage.sprite = bombsBrush.ObjectImage;

                mainBrush = gcSet.GOSet.Disabled;
                mainBrushImage.sprite = mainBrush.ObjectImage;
                SelectMainBrush();

                overBrush = gcSet.GOSet.Disabled;
                overBrushImage.sprite = overBrush.ObjectImage;

                underBrush = gcSet.GOSet.Disabled;
                underBrushImage.sprite = underBrush.ObjectImage;

                disabledBrush = gcSet.GOSet.Disabled;
                disabledBrushImage.sprite = disabledBrush.ObjectImage;

                blockedBrush = gcSet.GOSet.Disabled;
                blockedBrushImage.sprite = blockedBrush.ObjectImage;

                fallingBrush = gcSet.GOSet.Disabled;
                fallingBrushImage.sprite = fallingBrush.ObjectImage;

                if (editModeText) editModeText.text = "EDIT MODE" + '\n' + "Level " + (MatchPlayer.CurrentLevel + 1);
                ShowLevelData(false);

                CreateLevelButtons();
                ShowConstructMenu(true);
            }
        }

        private void ShowLevelData()
        {
            ShowLevelData(true);
        }

        private void ShowLevelData(bool rebuild)
        {
            lcSet = MBoard.LcSet;
            goSet = gcSet.GOSet;
            MBoard.GCSet.Clean();
            lcSet.Clean(goSet);
            Debug.Log("Show level data: " + (MatchPlayer.CurrentLevel));
            if (rebuild) MBoard.CreateGameBoard();

            levelMission = lcSet.levelMission;
            targets = MBoard.Targets;
            foreach (var item in targets)
            {
                item.Value.SetCurrCount(0);
                int iCount = levelMission.Targets.CountByID(item.Key);
                if (iCount > 0)
                    item.Value.SetNeedCount(levelMission.Targets.CountByID(item.Key));
                else
                    item.Value.SetNeedCount(0);
            }

            LevelButtonsRefresh();
            if (editModeText) editModeText.text = "EDIT MODE" + '\n' + "Level " + (MatchPlayer.CurrentLevel + 1);
            if (HeaderGUIController.Instance)
            {
                HeaderGUIController.Instance.RefreshTimeMoves();
                HeaderGUIController.Instance.RefreshScore(levelMission.ScoreTarget);
                HeaderGUIController.Instance.RefreshLevel();
            }
        }

        #region construct menus +
        bool openedConstr = false;

        public void OpenConstructPanel()
        {
            SetConstructControlActivity(false);
            RectTransform rt = constructPanel.GetComponent<RectTransform>();//Debug.Log(rt.offsetMin + " : " + rt.offsetMax);
            float startX = (!openedConstr) ? 0 : 1f;
            float endX = (!openedConstr) ? 1f : 0;

            SimpleTween.Value(constructPanel, startX, endX, 0.2f).SetEase(EaseAnim.EaseInCubic).
                               SetOnUpdate((float val) =>
                               {
                                   rt.transform.localScale = new Vector3(val, 1, 1);
                               // rt.offsetMax = new Vector2(val, rt.offsetMax.y);
                           }).AddCompleteCallBack(() =>
                           {
                               SetConstructControlActivity(true);
                               openedConstr = !openedConstr;
                               LevelButtonsRefresh();
                           });
        }

        private void SetConstructControlActivity(bool activity)
        {
            Button[] buttons = constructPanel.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        private void ShowConstructMenu(bool show)
        {
            constructPanel.SetActive(show);
            openConstructButton.gameObject.SetActive(show);
        }

        public void CreateLevelButtons()
        {
            GameObject parent = LevelButtonsContainer.content.gameObject;
            Button[] existButtons = parent.GetComponentsInChildren<Button>();
            for (int i = 0; i < existButtons.Length; i++)
            {
                DestroyImmediate(existButtons[i].gameObject);
            }

            for (int i = 0; i < MBoard.GCSet.levelSets.Count; i++)
            {
                GameObject buttonGO = Instantiate(levelButtonPrefab, Vector3.zero, Quaternion.identity);
                buttonGO.transform.SetParent(parent.transform);
                buttonGO.transform.localScale = Vector3.one;
                Button b = buttonGO.GetComponent<Button>();
                b.onClick.RemoveAllListeners();
                int level = i + 1;
                b.onClick.AddListener(() =>
                {
                    MatchPlayer.CurrentLevel = level - 1; // = level;
                CloseOpenedPanels();
                    ShowLevelData();
                });
                buttonGO.GetComponentInChildren<Text>().text = "" + level.ToString();
            }
        }

        public void RemoveLevel()
        {
            Debug.Log("Click on Button <Remove level...> ");
            if (MBoard.GCSet.LevelCount < 2)
            {
                Debug.Log("Can't remove the last level> ");
                return;
            }
            MBoard.GCSet.RemoveLevel(MatchPlayer.CurrentLevel);
            CreateLevelButtons();
            MatchPlayer.CurrentLevel = (MatchPlayer.CurrentLevel <= MBoard.GCSet.LevelCount - 1) ? MatchPlayer.CurrentLevel : MatchPlayer.CurrentLevel - 1;
                ShowLevelData();
        }

        public void InsertBefore()
        {
            Debug.Log("Click on Button <Insert level before...> ");
            LevelConstructSet lcs = ScriptableObjectUtility.CreateResourceAsset<LevelConstructSet>(levelConstructSetSubFolder, "", " " + 1.ToString());
            MBoard.GCSet.InsertBeforeLevel(MatchPlayer.CurrentLevel, lcs);
            CreateLevelButtons();
            ShowLevelData();
        }

        public void InsertAfter()
        {
            Debug.Log("Click on Button <Insert level after...> ");
            LevelConstructSet lcs = ScriptableObjectUtility.CreateResourceAsset<LevelConstructSet>(levelConstructSetSubFolder, "", " " + 1.ToString());
            MBoard.GCSet.InsertAfterLevel(MatchPlayer.CurrentLevel, lcs);
            CreateLevelButtons();
            MatchPlayer.CurrentLevel += 1;
            ShowLevelData();
        }

        private void LevelButtonsRefresh()
        {
            Button[] levelButtons = LevelButtonsContainer.content.gameObject.GetComponentsInChildren<Button>();
            for (int i = 0; i < levelButtons.Length; i++)
            {
                SelectButton(levelButtons[i], (i == MatchPlayer.CurrentLevel));
            }
        }

        private void SelectButton(Button b, bool select)
        {
            b.GetComponent<Image>().color = (select) ? new Color(0.5f, 0.5f, 0.5f, 1) : new Color(1, 1, 1, 1);
        }
        #endregion construct menus

        #region grid settings
        private void ShowLevelSettingsMenu(bool show)
        {
            constructPanel.SetActive(show);
            openConstructButton.gameObject.SetActive(show);
        }

        bool openedSettings = false;
        public void OpenSettingsPanel_Click()
        {
            Debug.Log("open grid settings click");
            MatchGrid grid = MBoard.grid;

            ScrollPanelController sRC = GridPanelContainer.ScrollPanel;
            if (sRC) // 
            {
                if (sRC) sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = GridPanelContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Grid panel";

                //create  vert size block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "VertSize", grid.Rows.Count.ToString(),
                    () => { IncVertSize(); },
                    () => { DecVertSize(); },
                    (val) => { },
                    () => { return grid.Rows.Count.ToString(); },
                    null);

                //create hor size block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "HorSize", lcSet.HorSize.ToString(),
                    () => { IncHorSize(); },
                    () => { DecHorSize(); },
                    (val) => { },
                    () => { return lcSet.HorSize.ToString(); },
                    null);

                //create background block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "BackGrounds", lcSet.BackGround.ToString(),
                    () => { IncBackGround(); },
                    () => { DecBackGround(); },
                    (val) => { },
                    () => { return lcSet.BackGround.ToString(); },
                    null);

                //create dist X block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "Dist X", lcSet.DistX.ToString(),
                    () => { IncDistX(); },
                    () => { DecDistX(); },
                    (val) => { },
                    () => { return lcSet.DistX.ToString(); },
                    null);

                //create dist Y block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "Dist Y", lcSet.DistY.ToString(),
                    () => { IncDistY(); },
                    () => { DecDistY(); },
                    (val) => { },
                    () => { return lcSet.DistY.ToString(); },
                    null);

                //create scale block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "Scale", lcSet.Scale.ToString(),
                    () => { IncScale(); },
                    () => { DecScale(); },
                    (val) => { },
                    () => { return lcSet.Scale.ToString(); },
                    null);

                sRC.OpenScrollPanel(null);
            }




            //SetSettingsControlActivity(false);

            //RectTransform rt = levelSettingsPanel.GetComponent<RectTransform>();//Debug.Log(rt.offsetMin + " : " + rt.offsetMax);
            //float startX = (!openedSettings) ? 0 : 1f;
            //float endX = (!openedSettings) ? 1f : 0;
            //MatchBoard.GCSet.GetLevelConstructSet(MatchPlayer.CurrentLevel).SelectLevel();
            //SimpleTween.Value(levelSettingsPanel, startX, endX, 0.2f).SetEase(EaseAnim.EaseInCubic).
            //                   SetOnUpdate((float val) =>
            //                   {
            //                       rt.transform.localScale = new Vector3(val, 1, 1);
            //                   }).AddCompleteCallBack(() =>
            //                   {
            //                       SetSettingsControlActivity(true);
            //                       openedSettings = !openedSettings;
            //                       MatchButtonsRefresh();
            //                   });
        }

        public void IncVertSize()
        {
            Debug.Log("Click on Button <VerticalSize...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            int vertSize = lcs.VertSize;
            vertSize = (vertSize < maxVertSize) ? ++vertSize : maxVertSize;
            lcs.VertSize = vertSize;
            ShowLevelData();
        }

        public void DecVertSize()
        {
            LevelConstructSet lcs = MBoard.LcSet;
            int vertSize = lcs.VertSize;
            vertSize = (vertSize > minVertSize) ? --vertSize : minVertSize;
            lcs.VertSize = vertSize;
            ShowLevelData();
        }

        public void IncHorSize()
        {
            Debug.Log("Click on Button <HorizontalSize...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            int horSize = lcs.HorSize;
            horSize = (horSize < maxHorSize) ? ++horSize : maxHorSize;
            lcs.HorSize = horSize;
            ShowLevelData();
        }

        public void DecHorSize()
        {
            Debug.Log("Click on Button <HorizontalSize...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            int horSize = lcs.HorSize;
            horSize = (horSize > minHorSize) ? --horSize : minHorSize;
            lcs.HorSize = horSize;
            ShowLevelData();
        }

        public void IncDistX()
        {
            Debug.Log("Click on Button <DistanceX...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float dist = lcs.DistX;
            dist += 0.05f;
            lcs.DistX = (dist > 1f) ? 1f : dist;
            ShowLevelData();
        }

        public void DecDistX()
        {
            Debug.Log("Click on Button <DistanceX...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float dist = lcs.DistX;
            dist -= 0.05f;
            lcs.DistX = (dist > 0f) ? dist : 0f;
            ShowLevelData();
        }

        public void IncDistY()
        {
            Debug.Log("Click on Button <DistanceY...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float dist = lcs.DistY;
            dist += 0.05f;
            lcs.DistY = (dist > 1f) ? 1f : dist;
            ShowLevelData();
        }

        public void DecDistY()
        {
            Debug.Log("Click on Button <DistanceY...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float dist = lcs.DistY;
            dist -= 0.05f;
            lcs.DistY = (dist > 0f) ? dist : 0f;
            ShowLevelData();
        }

        public void DecScale()
        {
            Debug.Log("Click on Button <Scale...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float scale = lcs.Scale;
            scale -= 0.05f;
            lcs.Scale = (scale > 0f) ? scale : 0f;
            ShowLevelData();
        }

        public void IncScale()
        {
            Debug.Log("Click on Button <Scale...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float scale = lcs.Scale;
            scale += 0.05f;
            lcs.Scale = scale;
            ShowLevelData();
        }

        public void IncBackGround()
        {
            Debug.Log("Click on Button <BackGround...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            lcs.IncBackGround();
            ShowLevelData();
        }

        public void DecBackGround()
        {
            Debug.Log("Click on Button <BackGround...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            lcs.DecBackGround();
            ShowLevelData();
        }
        #endregion grid settings

        #region grid brushes
        public void Cell_Click(GridCell cell)
        {
            Debug.Log("Click on cell <" + cell.ToString() + "...> ");
            LevelConstructSet lcs = MBoard.LcSet;

            if (selectedDisabledBrushImage.enabled)
            {
                Debug.Log("disabled brush enabled");
                if (cell.IsDisabled) lcSet.RemoveDisabledCell(new CellData(disabledBrush.ID, cell.Row, cell.Column));
                else lcSet.AddDisabledCell(new CellData(disabledBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedTutorialBrushImage.enabled)
            {
                Debug.Log("tutorial brush enabled");
                if (!GameObjectsSet.IsDisabledObject(tutorialBrush.ID))
                {
                    lcs.AddTutorialCell(cell.transform.position, new CellData(tutorialBrush.ID, cell.Row, cell.Column));
                }
                else lcs.RemoveTutorialCell(cell.transform.position, new CellData(tutorialBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedMatchedBrushImage.enabled)
            {
                Debug.Log("matched brush enabled");
                if (!GameObjectsSet.IsDisabledObject(matchedBrush.ID)) lcs.AddMatchedCell(new CellData(matchedBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveMatchedCell(new CellData(matchedBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedBombsBrushImage.enabled)
            {
                Debug.Log("matched brush enabled");
                if (!GameObjectsSet.IsDisabledObject(bombsBrush.ID)) lcs.AddBombsCell(new CellData(bombsBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveBombsCell(new CellData(bombsBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedBlockedBrushImage.enabled)
            {
                Debug.Log("blocked brush enabled");
                if (!GameObjectsSet.IsDisabledObject(blockedBrush.ID)) lcs.AddBlockedCell(new CellData(blockedBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveBlockedCell(new CellData(blockedBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedMainBrushImage.enabled)
            {
                Debug.Log("main brush enabled");
                if (!GameObjectsSet.IsDisabledObject(mainBrush.ID)) lcs.AddFeaturedCell(new CellData(mainBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveFeaturedCell(new CellData(mainBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedFallingBrushImage.enabled)
            {
                Debug.Log("falling brush enabled");
                if (!GameObjectsSet.IsDisabledObject(fallingBrush.ID)) lcs.AddFallingCell(new CellData(fallingBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveFallingCell(new CellData(fallingBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedOverBrushImage.enabled)
            {
                Debug.Log("over brush enabled");
                if (!GameObjectsSet.IsDisabledObject(overBrush.ID)) lcs.AddOverlayCell(new CellData(overBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveOverlayCell(new CellData(overBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedUnderBrushImage.enabled)
            {
                Debug.Log("under brush enabled");
                if (!GameObjectsSet.IsDisabledObject(underBrush.ID)) lcs.AddUnderlayCell(new CellData(underBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveUnderlayCell(new CellData(underBrush.ID, cell.Row, cell.Column));
            }

            CloseOpenedPanels();
            ShowLevelData();
        }

        public void OpenTutorialBrushPanel_Click()
        {
            Debug.Log("open tutorial brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = TutorialBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = TutorialBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Tutorial brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.TutorialObjects != null)
                    foreach (var item in gcSet.GOSet.TutorialObjects)
                    {
                        mData.Add(item);
                    }

                //create main bubbles brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        tutorialBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetObject(mD.ID) : gcSet.GOSet.Disabled;
                        tutorialBrushImage.sprite = tutorialBrush.ObjectImage;
                        SelectTutorialBrush();
                    });
                }

                //description input field
                IncDecInputPanel.Create(sRC.scrollContent, InputTextPanelMissionPrefab, "", levelMission.Description,
                null,
                null,
                (val) => { lcs.textTutorial = val.ToString(); levelMission.SetDescription(val); },//
               // (val) => { levelMission.SetDescription(val); },
                () => { return levelMission.Description; },
                null);

                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenMatchedBrushPanel_Click()
        {
            Debug.Log("open matched brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = MatchedBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = MatchedBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Matched brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.MatchObjects != null)
                    foreach (var item in gcSet.GOSet.MatchObjects)
                    {
                        mData.Add(item);
                    }

                //create main bubbles brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        matchedBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetObject(mD.ID) : gcSet.GOSet.Disabled;
                        matchedBrushImage.sprite = matchedBrush.ObjectImage;
                        SelectMatchedBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenBombsBrushPanel_Click()
        {
            Debug.Log("open bombs brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = BombsBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = BombsBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Bombs brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.DynamicClickBombObjects != null)
                    foreach (var item in gcSet.GOSet.DynamicClickBombObjects)
                    {
                        mData.Add(item);
                    }

                //create main bubbles brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        bombsBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetObject(mD.ID) : gcSet.GOSet.Disabled;
                        bombsBrushImage.sprite = bombsBrush.ObjectImage;
                        SelectBombsBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }
        public void OpenBlockedBrushPanel_Click()
        {
            Debug.Log("open blocked brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = BlockedBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = BlockedBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Blocked brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.BlockedObjects != null)
                    foreach (var item in gcSet.GOSet.BlockedObjects)
                    {
                        mData.Add(item);
                    }

                //create main bubbles brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        blockedBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetObject(mD.ID) : gcSet.GOSet.Disabled;
                        blockedBrushImage.sprite = blockedBrush.ObjectImage;
                        SelectBlockedBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenFallingBrushPanel_Click()
        {
            Debug.Log("open blocked brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = FallingBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = FallingBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Falling brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.FallingObject != null)
                  //  foreach (var item in gcSet.GOSet.BlockedObjects)
                    {
                        mData.Add(gcSet.GOSet.FallingObject);
                    }

                //create brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        fallingBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetObject(mD.ID) : gcSet.GOSet.Disabled;
                        fallingBrushImage.sprite = fallingBrush.ObjectImage;
                        SelectFallingBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenMainBrushPanel_Click()
        {
            Debug.Log("open main brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = MainBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = MainBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Main brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.MatchObjects != null)
                    foreach (var item in gcSet.GOSet.MatchObjects)
                    {
                        mData.Add(item);
                    }

                //create main bubbles brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        mainBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetMainObject(mD.ID) : gcSet.GOSet.Disabled;
                        mainBrushImage.sprite = mainBrush.ObjectImage;
                        SelectMainBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenOverBrushPanel_Click()
        {
            Debug.Log("open over brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = OverBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = OverBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Over brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.OverlayObjects != null) mData.AddRange(gcSet.GOSet.OverlayObjects.GetBaseList());

                //create over brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        overBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetOverlayObject(mD.ID) : gcSet.GOSet.Disabled;
                        overBrushImage.sprite = overBrush.ObjectImage;
                        SelectOverBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenUnderBrushPanel_Click()
        {
            Debug.Log("open over brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = UnderBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = UnderBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Over brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.UnderlayObjects != null) mData.AddRange(gcSet.GOSet.UnderlayObjects.GetBaseList());

                //create over brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        underBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetUnderlayObject(mD.ID) : gcSet.GOSet.Disabled;
                        underBrushImage.sprite = underBrush.ObjectImage;
                        SelectUnderBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        private void CloseOpenedPanels()
        {
            ScrollPanelController[] sRCs = GetComponentsInChildren<ScrollPanelController>();
            foreach (var item in sRCs)
            {
                item.CloseScrollPanel(true, null);
            }

        }

        private void SetSpriteControlActivity(RectTransform panel, bool activity)
        {
            Button[] buttons = panel.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        public void SelectTutorialBrush()
        {
            DeselectAllBrushes();
            selectedTutorialBrushImage.enabled = true;
        }

        public void SelectMatchedBrush()
        {
            DeselectAllBrushes();
            selectedMatchedBrushImage.enabled = true;
        }

        public void SelectBombsBrush()
        {
            DeselectAllBrushes();
            selectedBombsBrushImage.enabled = true;
        }

        public void SelectMainBrush()
        {
            DeselectAllBrushes();
            selectedMainBrushImage.enabled = true;
        }

        public void SelectOverBrush()
        {
            DeselectAllBrushes();
            selectedOverBrushImage.enabled = true;
        }

        public void SelectUnderBrush()
        {
            DeselectAllBrushes();
            selectedUnderBrushImage.enabled = true;
        }

        public void SelectUnderMatch()
        {
         //   underMatchImage.enabled = true;
        }

        public void SelectDisabledBrush()
        {
            DeselectAllBrushes();
            selectedDisabledBrushImage.enabled = true;
        }

        public void SelectBlockedBrush()
        {
            DeselectAllBrushes();
            selectedBlockedBrushImage.enabled = true;
        }

        public void SelectFallingBrush()
        {
            DeselectAllBrushes();
            selectedFallingBrushImage.enabled = true;
        }

        private void DeselectAllBrushes()
        {
            selectedTutorialBrushImage.enabled = false;
            selectedMatchedBrushImage.enabled = false;
            selectedBombsBrushImage.enabled = false;
            selectedBlockedBrushImage.enabled = false;
            selectedDisabledBrushImage.enabled = false;
            selectedMainBrushImage.enabled = false;
            selectedOverBrushImage.enabled = false;
            selectedUnderBrushImage.enabled = false;
            selectedFallingBrushImage.enabled = false;
        }
        #endregion grid brushes


        public void OffOn_MatchObj(int id)
        {
            Debug.Log("Click on Button <Off/On object Match...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            int[] offMatch_ID = lcs.OffMatch_ID;
            bool estb = false;

            for (int i = 0; i < offMatch_ID.Length; i++)
            {
                if (offMatch_ID[i] == id)
                {
                    offMatch_ID[i] = 0;
                    estb = true;
                }
            }

            if (!estb)
                for (int i = 0; i < offMatch_ID.Length; i++)
                    if (offMatch_ID[i] == 0)
                    {
                        offMatch_ID[i] = id;
                        break;
                    }
 
                 //   vertSize = (vertSize < maxVertSize) ? ++vertSize : maxVertSize;
            lcs.OffMatch_ID = offMatch_ID;

            ShowLevelData();       
        }


        #region match
        public void OpenMatchPanel_Click()
        {
            Debug.Log("open match click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;
            int[] offMatch_ID = lcs.OffMatch_ID;

            ScrollPanelController sRC = MatchPanelContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = MatchPanelContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Match object panel";

                List<MatchObjectData> mData = new List<MatchObjectData>();
                //   mData.Add(GOSet.Disabled);
                //  if (gcSet.GOSet.UnderlayObjects != null) mData.AddRange(gcSet.GOSet.UnderlayObjects.GetBaseList());


             
            //    mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.MatchObjects != null)
                    foreach (var item in gcSet.GOSet.MatchObjects)
                    {
                        mData.Add(item);
                    }
                    
                //create over brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    MatchObjectData mD = mData[i];
                    Image a;
                    bool estb = false;
                    //a = smallButtonPrefab.AddComponent<Image>();

                    for (int j = 0; j < mData.Count; j++)
                    {
                        if (offMatch_ID[j] == mD.ID)
                        {
                            // a.color = new Vector4(0.5F, 1, 0.5F, 1);
                            CreateButton(smallButtonPrefab, sRC.scrollContent, MatchDarkImage, () =>
                            {
                                Debug.Log("Click on Button <" + mD.ID + "...> ");
                                OffOn_MatchObj(mD.ID);
                            });
                            estb = true;
                            break;
                        }
                    }

                    if(!estb)
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        OffOn_MatchObj(mD.ID);
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }
        #endregion match


        #region mission
        public void OpenMissionPanel_Click()
        {
            Debug.Log("open mission click");
            MatchGrid grid = MBoard.grid;

            ScrollPanelController sRC = MissionPanelContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = MissionPanelContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Mission panel";


                IncDecInputPanel movesPanel = null;

                //create time constrain
                IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Time", levelMission.TimeConstrain.ToString(),
                () => { levelMission.AddTime(1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                () => { levelMission.AddTime(-1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                (val) => { int res; bool good = int.TryParse(val, out res); if (good) { levelMission.SetTime(res); HeaderGUIController.Instance.RefreshTimeMoves(); } },
                () => { movesPanel?.gameObject.SetActive(!levelMission.IsTimeLevel); return levelMission.TimeConstrain.ToString(); },
                null);

                //create mission moves constrain
                movesPanel = IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Moves", levelMission.MovesConstrain.ToString(),
                    () => { levelMission.AddMoves(1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                    () => { levelMission.AddMoves(-1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                    (val) => { int res; bool good = int.TryParse(val, out res); if (good) { levelMission.SetMovesCount(res); HeaderGUIController.Instance.RefreshTimeMoves(); } },
                    () => { return levelMission.MovesConstrain.ToString(); },
                    null);
                movesPanel.gameObject.SetActive(!levelMission.IsTimeLevel);

                //description input field
                IncDecInputPanel.Create(sRC.scrollContent, InputTextPanelMissionPrefab, "Description", levelMission.Description,
                null,
                null,
                (val) => { levelMission.SetDescription(val); },
                () => { return levelMission.Description; },
                null);

                //create score target
                IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab,"Score", levelMission.ScoreTarget.ToString(),
                () => { levelMission.AddScoreTarget(1); HeaderGUIController.Instance.RefreshScore(levelMission.ScoreTarget); },
                () => { levelMission.AddScoreTarget(-1); HeaderGUIController.Instance.RefreshScore(levelMission.ScoreTarget); },
                (val) => { int res; bool good = int.TryParse(val, out res); if (good) { levelMission.SetScoreTargetCount(res); HeaderGUIController.Instance.RefreshScore(levelMission.ScoreTarget); } },
                () => {   return levelMission.ScoreTarget.ToString(); },
                null);

                //create object targets
                foreach (var item in targets)
                {
                    int id = item.Key;
                    IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Target", levelMission.GetTargetCount(id).ToString(),
                    false,
                    () => { levelMission.AddTarget(id, 1); item.Value?.IncNeedCount(1); },
                    () => { levelMission.RemoveTarget(id, 1); item.Value?.IncNeedCount(-1); },
                    (val) => { int res; bool good = int.TryParse(val, out res); if (good) { levelMission.SetTargetCount(id, res); item.Value?.SetNeedCount(res); } },
                    null,
                    () => { return levelMission.GetTargetCount(id).ToString(); }, // grid.GetObjectsCountByID(id).ToString()); },
                    item.Value.GetImage(goSet));
                }

                sRC.OpenScrollPanel(null);
            }
        }
        #endregion mission

        #region load assets
        void LoadGameConstructAsset(string gameConstructSetSubFolder)
        {
            if (MBoard.GCSet != null)
            {
                return;
            }
            GameConstructSet[] os = LoadResourceAssets<GameConstructSet>(gameConstructSetSubFolder);
            if (os.Length > 0)
            {
                // MatchBoard.GCSet = os[0];
            }
            else
            {
                // MatchBoard.Instance.GCSet = ScriptableObjectUtility.CreateAsset<GameConstructSet>(gameConstructSetSubFolder, ""," "+ 1.ToString());
            }
        }

        List<GameObjectsSet> LoadMatchSetAssets(string matchSetSubFolder)
        {
            List<GameObjectsSet> MatchSets = new List<GameObjectsSet>(LoadResourceAssets<GameObjectsSet>(matchSetSubFolder));
            if (MatchSets == null || MatchSets.Count == 0)
            {
                MatchSets = new List<GameObjectsSet>();
                MatchSets.Add(ScriptableObjectUtility.CreateResourceAsset<GameObjectsSet>(matchSetSubFolder, "", " " + 1.ToString()));
                Debug.Log("New MatchSet created: " + MatchSets[0].ToString());
            }
            return MatchSets;
        }

        List<LevelConstructSet> LoadLevelConstructAssets()
        {
            List<GameObjectsSet> goSets = null;
            List<LevelConstructSet> LevelConstructSets = new List<LevelConstructSet>(LoadResourceAssets<LevelConstructSet>(levelConstructSetSubFolder));
            if (LevelConstructSets == null || LevelConstructSets.Count == 0)
            {
                LevelConstructSets = new List<LevelConstructSet>();
                LevelConstructSets.Add(ScriptableObjectUtility.CreateResourceAsset<LevelConstructSet>(levelConstructSetSubFolder, "", " " + 1.ToString()));
                Debug.Log("New LevelConstructSet created: " + LevelConstructSets[0].ToString());
            }
            // all empty level MatchSets - set to default
            // LevelConstructSets.ForEach((l)=> { if (!l.mSet) l.mSet = goSets[0]; });
            return LevelConstructSets;
        }

        T[] LoadResourceAssets<T>(string subFolder) where T : BaseScriptable
        {
            T[] t = Resources.LoadAll<T>(subFolder);
            if (t != null && t.Length > 0)
            {
                string s = "";
                foreach (var m in t)
                {
                    s += m.ToString() + "; ";
                }
                Debug.Log("Scriptable assets loaded," + typeof(T).ToString() + ", count: " + t.Length + "; sets : " + s);
            }
            else
            {
                Debug.Log("Scriptable assets " + typeof(T).ToString() + " not found!!!");
            }
            return t;
        }

        #endregion load assets

        #region utils
        private void DestroyGoInChildrenWithComponent<T>(Transform parent) where T : Component
        {
            T[] existComp = parent.GetComponentsInChildren<T>();
            for (int i = 0; i < existComp.Length; i++)
            {
                DestroyImmediate(existComp[i].gameObject);
            }
        }

        private Button CreateButton(GameObject prefab, Transform parent, Sprite sprite, System.Action listener)
        {
            GameObject buttonGO = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            buttonGO.transform.SetParent(parent);
            buttonGO.transform.localScale = new Vector3(1, 1, 1);
            Button b = buttonGO.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.GetComponent<Image>().sprite = sprite;
            if (listener != null) b.onClick.AddListener(() =>
            {
                listener();
            });

            return b;
        }

        private void SelectButton(Button b)
        {
            Text t = b.GetComponentInChildren<Text>();
            if (!t) return;
            t.enabled = true;
            t.gameObject.SetActive(true);
            t.text = "selected";
            t.color = Color.black;
        }

        private void DeselectButton(Button b)
        {
            Text t = b.GetComponentInChildren<Text>();
            if (!t) return;
            t.enabled = true;
            t.gameObject.SetActive(true);
            t.text = "";
        }
        #endregion utils

#endif
    }

#if UNITY_EDITOR
    public static class ScriptableObjectUtility //http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAsset<T>(string subFolder, string namePrefix, string nameSuffics) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            Debug.Log(path);
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/Resources/" + subFolder + "/" + namePrefix + typeof(T).ToString() + nameSuffics + ".asset");
            Debug.Log(assetPathAndName);
            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files in Resource/Subfolder .
        /// </summary>
        public static T CreateResourceAsset<T>(string subFolder, string namePrefix, string nameSuffics) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string path = "Assets/ForestMatch/Resources/";
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + subFolder + "/" + namePrefix + typeof(T).ToString() + nameSuffics + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files in Resource/Subfolder .
        /// </summary>
        public static void DeleteResourceAsset(UnityEngine.Object o)
        {
            string path = AssetDatabase.GetAssetPath(o);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

    }
#endif
}
/*
 *TODO

 */