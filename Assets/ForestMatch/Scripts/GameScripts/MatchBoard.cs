using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
    public class MatchBoard : MonoBehaviour
    {

        #region bomb setting
        [Space(8)]
        [Header("Bomb setting")]
        public BombType bombType;
        public bool showBombExplode = true;
        public bool dragExplode = false;
        public bool  showWhirlWind = false;
        public Whirlwind wind;
        #endregion bomb setting

        #region settings 
        [Space(8)]
        [Header("Game settings")]
        public int[] rateLevel = new int[10] {8,16,24,32,40,48,56,64,72,80};
        public bool fastAuto = true;
        public bool victoryMessage = true;
        public static bool showMission = true;
        public bool showAlmostMessage = true;
        public int almostCoins = 100;
        public bool flyCollected = true;
        public FillType fillType;
        #endregion settings

        #region references
        [Header("Main references")]
        [Space(8)]
        public Transform GridContainer;
        [SerializeField]
        private RectTransform flyTarget;
        public SpriteRenderer backGround;
        public GameConstructor gConstructor;
        #endregion references

        #region spawn
        [Space(8)]
        [Header("Spawn")]
        public GameObject spawnerPrefab;
        public SpawnerStyle spawnerStyle;
        #endregion spawn

        public static float MaxDragDistance;

        public MatchGrid grid;

        #region curves
        public AnimationCurve explodeCurve;
        public AnimationCurve arcCurve;
        #endregion curves

        #region states
        public static GameMode GMode = GameMode.Play; // Play or Edit

        public MatchBoardState MbState { get; private set; }
        #endregion states

        #region properties
        public Vector3 FlyTarget
        {
            get { return flyTarget.transform.position; } //return Coordinats.CanvasToWorld(flyTarget.gameObject); }
        }

        public Sprite BackGround
        {
            get { return backGround.sprite; }
            set { if (backGround) backGround.sprite = value; }
        }

        public int AverageScore
        {
            get;
            private set;
        }

        private static TouchManager Touch { get { return TouchManager.Instance; } }

        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }

        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }

        public WinController WinContr { get; private set; }

        public Dictionary<int, TargetData> Targets { get; private set; }

        public MatchGUIController MGui { get { return MatchGUIController.Instance; } }

        private TutorialWindowController TWCtrl { get { return TutorialWindowController.Instance; } }

        private google_Mobile_Ads MobAds { get { return google_Mobile_Ads.Instance; } }
        #endregion properties

        #region sets
        public  GameConstructSet GCSet { get { return MPlayer.gcSet; } }

        public  GameObjectsSet MatchSet
        {
            get { return GCSet.GOSet; }
        }

        public  LevelConstructSet LcSet
        {
            get { return GCSet.GetLevelConstructSet(MatchPlayer.CurrentLevel); }
        }
        #endregion sets

        #region temp
        private List<GridCell> spRow = new List<GridCell>();
        private List<DataState> undoStates;
        public MatchGroupsHelper CollectGroups { get; private set; }
        public MatchGroupsHelper EstimateGroups { get;  set; }
        private bool testing = true;
        private float lastiEstimateShowed;
        private float lastPlayTime;
        private bool canPlay = false;
        private int collected = 0; // collected counter

        public GridCell CellTarget { get ; set; }

        public bool Spawned
        {
            get;
            private set;
        }
        #endregion temp

        #region debug
        [Header("Debug")]
        [SerializeField]
        private bool anySwap = false;
        #endregion debug
        public int match_stat = 0;

        public static MatchBoard Instance;

        #region regular
        private void Awake()
        {
            Application.targetFrameRate = 60; //FPS

            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            #region game sets 
            if (!GCSet)
            {
                Debug.Log("Game construct set not found!!!");
                return;
            }

            if (!LcSet)
            {
                Debug.Log("Level construct set not found!!! - " + MatchPlayer.CurrentLevel);
                return;
            }

            if (!MatchSet)
            {
                Debug.Log("MatcSet not found!!! - " + MatchPlayer.CurrentLevel);
                return;
            }
            #endregion game sets 

            #region targets
            Targets = new Dictionary<int, TargetData>();
            #endregion targets

            Spawned = true;

            if(MatchPlayer.CurrentLevel == 0)
                AnalyticsEvent.Custom("Start_Game", null);

            DestroyGrid();
            CreateGameBoard();
            MPlayer.StartLevel();

            if (GMode == GameMode.Edit)
            {
#if UNITY_EDITOR
                Debug.Log("start edit mode");
                foreach (var item in MatchSet.TargetObjects) // add all possible targets
                {
                    Targets.Add(item.ID, new TargetData(item.ID, LcSet.levelMission.GetTargetCount(item.ID)));
                }

                if (gConstructor)
                {
                    gConstructor.gameObject.SetActive(true);
                    gConstructor.InitStart(GCSet);
                }
                foreach (var item in MPlayer.BoostHolder.Boosters)
                {
                    if (item.Use) item.ChangeUse();
                }
#endif
            }

            else if (GMode == GameMode.Play)
            {
                Debug.Log("start play mode");

                if (gConstructor) DestroyImmediate(gConstructor.gameObject);

                WinContr = new WinController(LcSet.levelMission, this);                         //30                       //30  
            //  MPlayer.SetAverageScore(WinContr.IsTimeLevel? Mathf.Max(40, WinContr.MovesRest) * 60: WinContr.MovesRest * 60  );
                MPlayer.SetAverageScore(WinContr.IsTimeLevel? Mathf.Max(15, WinContr.MovesRest) * 60 : WinContr.MovesRest * 60);
                WinContr.TimerLeft30Event += () => { MGui.ShowMessageTimeLeft("Warning!", "30 seconds left", 2); };
                WinContr.MovesLeft5Event += () => { if (WinContr.Result != GameResult.WinAuto) MGui.ShowMessagMovesLeft("Warning!", "5 movesleft", 2); };
                WinContr.LevelWinEvent += () =>
                {
                    MobAds.ShowAds();
      
                    MGui.ShowWin(null);

                    MPlayer.PassLevel();
                    //Debug.Log("Zdesb Vstavitb Rate!!!!!!!!");

                    if (MPlayer.RateGetMove()==0)
                        for(int i=0; i < rateLevel.Length; i++)
                           if (MatchPlayer.CurrentLevel == (rateLevel[i]-1))
                                         MGui.ShowRate(null);


                    foreach (var item in MPlayer.BoostHolder.Boosters)
                    {
                        if (item.Use) item.ChangeUse();
                    }

                };

                WinContr.LevelPreLooseEvent += () =>
                {
                  //  MobAds.ShowBanner();
                    // if (MPlayer.Coins >= almostCoins)
                    // {
                    AnalyticsEvent.Custom("Lose_Level", new Dictionary<string, object>
                        {
                         { "Level", MatchPlayer.CurrentLevel  }
                        });

                        MGui.ShowAlmost(almostCoins);
                  //  }
                };

                WinContr.LevelLooseEvent += () =>
                {
                    AnalyticsEvent.Custom("Lose_Level", new Dictionary<string, object>
                    {
                        { "Level", MatchPlayer.CurrentLevel }
                    });

                    MGui.ShowFailed(null);
                    MPlayer.AddLifes(-1);
                    foreach (var item in MPlayer.BoostHolder.Boosters)
                    {
                        if(item.Use) item.ChangeUse();
                    }
                };
                WinContr.AutoWinEvent += () => 
                {
                    Spawned = false;
                    
                    if (victoryMessage)
                        MGui.ShowMessageAutoVictory("Congratulation!", "You win!", 2);
                  //////////////////////////
                  
                    //  WinContr.SetMoves(1);
                };

               // Debug.Log("Zdesb Vstavitb!!!!!!!!");
               // MGui.ShowRate(null);

                foreach (var item in MatchSet.TargetObjects)
                {
                    if (LcSet.levelMission.Targets.ContainObjectID(item.ID) && (LcSet.levelMission.Targets.CountByID(item.ID) > 0))
                        Targets.Add(item.ID, new TargetData(item.ID, LcSet.levelMission.GetTargetCount(item.ID)));
                }

                
                if (showMission)
                MGui.ShowMission(() =>
                {
                    if (WinContr.IsTimeLevel) WinContr.Timer.Start();
                    MbState = MatchBoardState.Fill;
                });
                else
                {
                    if (WinContr.IsTimeLevel) WinContr.Timer.Start();
                    MbState = MatchBoardState.Fill;
                }
                showMission = true;

                LevelConstructSet lcs = LcSet;
                if (lcs.tutorialCells.Count > 0)
                    MGui.ShowTutorial(null);

                #region matchgroups
                CollectGroups = new MatchGroupsHelper(grid);
                CollectGroups.QuadroCollect = QuadroCollectHandler;
                CollectGroups.HorizontalCollect5 = HorizontalCollectHandler5;
                CollectGroups.VerticalCollect5 = VerticalCollectHandler5;
                CollectGroups.HorizontalCollect4 = HorizontalCollectHandler4;
                CollectGroups.VerticalCollect4 = VerticalCollectHandler4;
                CollectGroups.CrossCollect4 = CrossCollectHandler;
                CollectGroups.ComboCollect = ComboCollectHandler;
                CollectGroups.Collect = CollectHandler;

                EstimateGroups = new MatchGroupsHelper(grid);
                #endregion matchgroups

                canPlay = true;    
            }

            grid.CalcObjects();
        }

        void Update()
        {
            if (!canPlay) return;
            if (WinContr.Result == GameResult.Win) return;
            if (WinContr.Result == GameResult.Loose) return;

            WinContr.Update(Time.time);

            // check board state
            switch (MbState)
            {
                case MatchBoardState.ShowEstimate: // Показываем варианты ходов 
                    ShowEstimateState();
                    break;

                case MatchBoardState.Fill: // Двигаем фишки на свободные клетки
                    FillState();
                    break;

                case MatchBoardState.Collect: // Собираем match группы
                    CollectState();
                    break;

                case MatchBoardState.Iddle: // Ждём действий от игрока
                    IddleState();
                    break;
                case MatchBoardState.Waiting:
                    break;
            }
        }
        #endregion regular

        

        #region grid construct
        public void CreateGameBoard()
        {
            Debug.Log("Create gameboard ");
            Debug.Log("level set: " + LcSet.name);
            Debug.Log("curr level: " + MatchPlayer.CurrentLevel);
            BackGround = MatchSet.BackGround(LcSet.BackGround);

            if (GMode == GameMode.Play)
            {
                grid = new MatchGrid(LcSet, MatchSet, GridContainer, SortingOrder.Base, GMode);

                // set cells delegates
                for (int i = 0; i < grid.Cells.Count; i++)
                {
                    SwapHelper.SwapEndEvent = MatchEndSwapHandler;
                    SwapHelper.SwapBeginEvent = MatchBeginSwapHandler;
                    grid.Cells[i].PointerDownEvent = MatchPointerDownHandler;
                    grid.Cells[i].DragEnterEvent = MatchDragEnterHandler;
                    grid.Cells[i].DoubleClickEvent = MatchDoubleClickHandler;
                }
                MaxDragDistance = Vector3.Distance(grid.Cells[0].transform.position, grid.Cells[1].transform.position);
                grid.FillGrid(true);
                
                // create spawners
                grid.Columns.ForEach((c) =>
                {
                    c.CreateTopSpawner(spawnerPrefab, spawnerStyle, GridContainer.lossyScale, transform);
                });

                // create pathes to spawners
                Map map = new Map(grid);
                PathFinder pF = new PathFinder();

                int ind = 0;
                foreach (var cl in grid.Cells)
                {
                    if (cl.spawner)
                    {
                        spRow.Add(grid.Cells[ind]);
                    }
                    ind++;
                }

                grid.Cells.ForEach((c)=>
                {
                    if (!c.Blocked && !c.IsDisabled && (c.Overlay ? c.Overlay.CanSkip : true))
                    {
                        int length = int.MaxValue;
                        List<GridCell> path = null;
                        grid.Columns.ForEach((col)=>
                        {
                            if (col.Spawn)
                            {
                                if (col.Spawn.gridCell != c)
                                {
                                    pF.CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell);
                                    if (pF.FullPath != null && pF.PathLenght < length) 
                                    { 
                                        path = pF.GCPath(); 
                                        length = pF.PathLenght; 
                                    }
                                }
                                else
                                {
                                    length = 0;
                                    path = new List<GridCell>();
                                }
                            }
                        });
                        c.fillPath = path;
                    }
                    c.CreateBorder();
                });

            }
            else
            {
#if UNITY_EDITOR
                if (grid != null)
                {
                    grid.Rebuild(LcSet, MatchSet, GMode);
                }
                else
                {
                    grid = new MatchGrid(LcSet, MatchSet, GridContainer, SortingOrder.Base, GMode);
                }

                // set cells delegates for constructor
                for (int i = 0; i < grid.Cells.Count; i++)
                {
                    grid.Cells[i].PointerDownEvent = (c) =>
                    {
                        if (c.Row < LcSet.VertSize) // don't using reserved rows
                            gConstructor.GetComponent<GameConstructor>().Cell_Click(c);
                    };

                    grid.Cells[i].DragEnterEvent = (c) =>
                    {
                        gConstructor.GetComponent<GameConstructor>().Cell_Click(c);
                    };
                }
#endif
            }

        }

         public void UpdatePath()
        {
            // create pathes to spawners
            Map map = new Map(grid);
            PathFinder pF = new PathFinder();

            foreach (var c in grid.Cells) // Перебираем все клетки в таблице
            {
                if (!c.Blocked && !c.IsDisabled && (c.Overlay ? c.Overlay.CanSkip : true)) // Если в данной клетке можно двигаться
                {
                    int length = int.MaxValue;
                    List<GridCell> path = null;

                    foreach (var col in spRow) // Перебираем клетки со спавном
                    {
                        if (col.spawner) // Если колонка со спавном
                        {
                            if (col.Column == c.Column) // Если мы в этой колонке
                            {
                                if (col != c) // Если в колонке со спавном клетка не равна нашей
                                {
                                    pF.CreatePath(map, c.pfCell, col.pfCell);
                                    if (pF.FullPath != null && pF.PathLenght < length)
                                    {
                                        path = pF.GCPath();
                                        length = pF.PathLenght;
                                        break;
                                    }
                                }
                                else
                                {
                                    length = 0;
                                    path = new List<GridCell>();
                                }
                            }
                            else
                            {
                                if (col != c) // Если в колонке со спавном клетка не равна нашей
                                {
                                    pF.CreatePath(map, c.pfCell, col.pfCell);
                                    if (pF.FullPath != null && pF.PathLenght < length)
                                    {
                                        path = pF.GCPath();
                                        length = pF.PathLenght;
                                    }
                                }
                                else
                                {
                                    length = 0;
                                    path = new List<GridCell>();
                                }
                            }
                        }
                    }
                    c.fillPath = path;
                }
            }
        }

        private void DestroyGrid()
        {
            GridCell[] gcs = gameObject.GetComponentsInChildren<GridCell>();
            for (int i = 0; i < gcs.Length; i++)
            {
                Destroy(gcs[i].gameObject);
            }
        }
        #endregion grid construct

        #region bombs
        private void SetStaticMatchBombHor(GridCell c, int ID)
        {
            StaticMatchBombObjectData b = MatchSet.GetStaticMatchBombObject(BombDir.Horizontal);
            c.SetStaticMatchBomb(b);
            c.SetObject(ID);
        }

        private void SetStaticMatchBombVert(GridCell c, int ID)
        {
            StaticMatchBombObjectData b = MatchSet.GetStaticMatchBombObject(BombDir.Vertical);
            c.SetStaticMatchBomb(b);
            c.SetObject(ID);
        }

        private void SetStaticMatchBombRadial(GridCell c, int ID)
        {
            StaticMatchBombObjectData b = MatchSet.GetStaticMatchBombObject(BombDir.Radial);
            c.SetStaticMatchBomb(b);
            c.SetObject(ID);
        }


        private void SetDynamicMatchBombHor(GridCell c, int ID)
        {
            DynamicMatchBombObjectData b = MatchSet.GetDynamicMatchBombObject(BombDir.Horizontal);
            c.SetObject(ID);
            c.SetDynamicMatchBomb(b);
        }

        private void SetDynamicMatchBombVert(GridCell c, int ID)
        {
            DynamicMatchBombObjectData b = MatchSet.GetDynamicMatchBombObject(BombDir.Vertical);
            c.SetObject(ID);
            c.SetDynamicMatchBomb(b);
        }

        private void SetDynamicMatchBombRadial(GridCell c, int ID)
        {
            DynamicMatchBombObjectData b = MatchSet.GetDynamicMatchBombObject(BombDir.Radial);
            c.SetObject(ID);
            c.SetDynamicMatchBomb(b);
        }

        private void SetDynamicClickBombHor(GridCell c)
        {
            DynamicClickBombObjectData b = MatchSet.GetDynamicClickBombObject(BombDir.Horizontal);
            c.SetDynamicClickBomb(b);
        }

        private void SetDynamicClickBombVert(GridCell c)
        {
            DynamicClickBombObjectData b = MatchSet.GetDynamicClickBombObject(BombDir.Vertical);
            c.SetDynamicClickBomb(b);
        }

        private void SetDynamicClickBombRadial(GridCell c)
        {
            DynamicClickBombObjectData b = MatchSet.GetDynamicClickBombObject(BombDir.Radial);
            c.SetDynamicClickBomb(b);
        }

        private void SetDynamicClickBombDinamit(GridCell c)
        {
            DynamicClickBombObjectData b = MatchSet.GetDynamicClickBombObject(BombDir.Dinamit);
            c.SetDynamicClickBomb(b);
        }

        private void SetDynamicClickBombColor(GridCell c)
        {
            DynamicClickBombObjectData b = MatchSet.GetDynamicClickBombObject(BombDir.Color);
            c.SetDynamicClickBomb(b);
        }

        private void SetDynamicClickBombButterfly(GridCell c)
        {
            DynamicClickBombObjectData b = MatchSet.GetDynamicClickBombObject(BombDir.Butterfly);
            c.SetDynamicClickBomb(b);
        }
        /// <summary>
        /// async collect matched objects in a group
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void ExplodeClickBomb(GridCell c)
        {
            if (!c.DynamicClickBomb)
            {
                return;
            }
            MbState = MatchBoardState.Waiting;
            SetControlActivity(false, false);
            c.ExplodeBomb(0.0f, showBombExplode, showWhirlWind, true, dragExplode, true, flyCollected, ()=>
            {
                MbState = MatchBoardState.Fill;
            });

        }

        #endregion bombs

        #region states
        private List<FallingObject> GetFalling()
        {
            List<GridCell> botCell = grid.GetBottomDynCells();
            List<FallingObject> res = new List<FallingObject>();
            foreach (var item in botCell)
            {
                if (item)
                {
                    FallingObject f = item.Falling;
                    if (f)
                    {
                        res.Add(f);
                    }
                }
            }
            return res;
        }

        private void CollectFalling(Action completeCallBack)
        {
         //   Debug.Log("collect falling " + GetFalling().Count);
            ParallelTween pt = new ParallelTween();
            foreach (var item in GetFalling())
            {
                pt.Add((callBack)=> 
                {
                    item.Collect(0, false, true, callBack);
                });
            }
            pt.Start(completeCallBack);
        }

        private void CollectState()
        {
            MbState = MatchBoardState.Waiting;
            collected = 0;

            if (grid.GetFreeCells().Count > 0)
            {
                MbState = MatchBoardState.Fill;
                return;
            }

            SetControlActivity(false, false); //Debug.Log("collect matches");

            CollectFalling(null);

            CollectGroups.CancelTweens();
            CollectGroups.CreateGroups(3, false);

            if (CollectGroups.Length == 0) // no matches
            {
                SetControlActivity(true, true);
                WinContr.CheckResult();
                if (grid.GetFreeCells().Count > 0)
                    MbState = MatchBoardState.Fill;
                else
                    MbState = MatchBoardState.ShowEstimate;
            }
            else
            {
                CollectGroups.CollectMatchGroups(() => { GreatingMessage(); MbState = MatchBoardState.Fill; });
            }
        }

        private void FillGridByStep(List<GridCell> freeCells, Action completeCallBack)
        {
            if (freeCells.Count == 0)
            {
                completeCallBack?.Invoke();
                return;
            }
            /////////////
            UpdatePath();
            //////////////
            ParallelTween tp = new ParallelTween();
            foreach (GridCell gc in freeCells)
            {
                tp.Add((callback) =>
                {
                    gc.FillGrab(callback);
                });
            }
            tp.Start(() =>
            {
                completeCallBack?.Invoke();
            });
        }

        private void FillState()
        {
            List<GridCell> gFreeCells = grid.GetFreeCells(); // Debug.Log("fill free count: " + gFreeCells.Count + " to collapse" );
            UpdatePath();
            if (gFreeCells.Count == 0)
            {      
                if (grid.NoPhys())
                {
                    MbState = MatchBoardState.Collect;
                } 
                //////////////////////////
                else
                    SetControlActivity(true, true);
                /////////////////////////
                return;
            }
            SetControlActivity(false, false);
            FillGridByStep(gFreeCells, null);
        }

        private void PlayIddleAnimRandomly()
        {
            if ((Time.time - lastPlayTime) < 5.3f) return;
            int randCell = UnityEngine.Random.Range(0, grid.Cells.Count);
            grid.Cells[randCell].PlayIddle();
            lastPlayTime = Time.time;
        }

        private void IddleState()
        {
            if (WinContr.Result == GameResult.PreLoose)
            {
                WinContr.Result = GameResult.None;
                WinContr.LevelPreLooseEvent?.Invoke();
            }

            PlayIddleAnimRandomly();
            if (Time.time - lastiEstimateShowed >= 5f)
            {
                lastiEstimateShowed = Time.time;
                MbState = MatchBoardState.ShowEstimate;
            }
        }

        private void ShowEstimateState()
        {
            if (!grid.NoPhys())
            {
                return;
            }

            MbState = MatchBoardState.Waiting;
            EstimateGroups.CancelTweens();
            EstimateGroups.CreateGroups(2, true);

            if (EstimateGroups.Length == 0)
            {
                MixGrid(null);
                return;
            }

            if (WinContr.Result != GameResult.WinAuto)
            {
                lastiEstimateShowed = Time.time; //  Debug.Log("show estimate");                                          

                EstimateGroups.ShowNextEstimateMatchGroups(() =>
                    {
                        MbState = MatchBoardState.Iddle; // go to iddle state
                    }); 
            }
            else
            {
                MakeStep(); // make auto step
            }
        }

        public void MixGrid(Action completeCallBack)
        {
            ParallelTween pT0 = new ParallelTween();
            ParallelTween pT1 = new ParallelTween();

            TweenSeq tweenSeq = new TweenSeq();
            List<GridCell> cellList = new List<GridCell>();
            List<GameObject> goList = new List<GameObject>();
            CollectGroups.CancelTweens();
            EstimateGroups.CancelTweens();

            grid.Cells.ForEach((c) => { if (c.IsMixable) { cellList.Add(c); goList.Add(c.DynamicObject); } });
            cellList.ForEach((c) => { pT0.Add((callBack) => { c.MixJump(transform.position, callBack); }); });

            cellList.ForEach((c) =>
            {
                int random = UnityEngine.Random.Range(0, goList.Count);
                GameObject m = goList[random];
                pT1.Add((callBack) => { c.GrabDynamicObject(m.gameObject, false, callBack); });
                goList.RemoveAt(random);
            });

            tweenSeq.Add((callBack) =>
            {
                pT0.Start(callBack);
            });

            tweenSeq.Add((callBack) =>
            {
                pT1.Start(() =>
                {
                    MbState = MatchBoardState.Fill;
                    completeCallBack?.Invoke();
                    callBack();
                });
            });
            tweenSeq.Start();
        }

        internal void SetControlActivity(bool activityGrid, bool activityMenu)
        {
            TouchManager.SetTouchActivity(activityGrid);
            HeaderGUIController.Instance.SetControlActivity(activityMenu);
            FooterGUIController.Instance.SetControlActivity(activityMenu);
        }
        #endregion states

        #region swap helper handlers
        private void MatchBeginSwapHandler(GridCell c)
        {
            if (GMode == GameMode.Play)
            {
                SetControlActivity(false, false);
                SaveUndoState();
            }
        }

  
        private void MatchEndSwapHandler(GridCell c) // Здесь проверяем, можно ли поменять местами фишки
        {
            if (GMode == GameMode.Play)
            {
                collected = 0; // reset collected count
                //Debug.Log("end swap");
                CollectGroups.CreateGroups(3, false);// = new MatchGroupsHelper(grid, 3, false);
                if ((CollectGroups.Length == 0) && (!c.DynamicClickBomb))   // if true to cansel swap
                {
                    if (!anySwap)
                        SwapHelper.UndoSwap(() =>
                        {
                            SetControlActivity(true, true);
                            MbState = MatchBoardState.Fill;
                        });

                    else
                    {
                        SetControlActivity(true, true);
                        WinContr.MakeMove();
                        MbState = MatchBoardState.Fill;
                    }
                }
                else
                {
                    if (c.DynamicClickBomb)
                        ExplodeClickBomb(c);
                    SetControlActivity(true, true);
                    WinContr.MakeMove();
                    if (TWCtrl != null)
                        TWCtrl.Clear_Click();
                    MbState = MatchBoardState.Fill; //    Debug.Log("end swap -> to fill");
                }
            }
        }
        #endregion swap helper handlers

        #region gridcell handlers
        private void MatchPointerDownHandler(GridCell c)
        {
            if (GMode == GameMode.Play)
            {
                if (grid.NoPhys())
                {
                    CollectGroups.CancelTweens();
                    EstimateGroups.CancelTweens();
                    ApplyBooster(c, ()=> {
                        if (TWCtrl != null)
                            TWCtrl.Clear_Click();
                    });
                    if (c.DynamicClickBomb)
                    {
                        WinContr.MakeMove();
                        ExplodeClickBomb(c);
                        if (TWCtrl != null)
                            TWCtrl.Clear_Click();
                    }
                }
            }
            else if (GMode == GameMode.Edit)
            {
#if UNITY_EDITOR
                gConstructor.GetComponent<GameConstructor>().selected = c;
#endif
            }
        }

        private void MatchDragEnterHandler(GridCell c)
        {
            if (GMode == GameMode.Play)
            {
                SwapHelper.Swap();
            }
        }

        private void MatchDoubleClickHandler(GridCell c)
        {

        }

        /// <summary>
        /// Raise for each collected matchobject
        /// </summary>
        /// <param name="gCell"></param>
        /// <param name="mData"></param>
        public void TargetCollectEventHandler(int id)
        {
            if (GMode == GameMode.Play)
            {
                if (Targets.ContainsKey(id)) Targets[id].IncCurrCount();
            }
        }

        public void MatchScoreCollectHandler()
        {
            collected += 1;
            if (collected <= 3) MPlayer.AddScore(10);
            else MPlayer.AddScore(20);
        }
        #endregion gridcell handlers

        #region match group handlers score counter
    
        private void QuadroCollectHandler(MatchGroup m)
        {
            SetDynamicClickBombButterfly(CellTarget ? CellTarget : m.lastMatchedCell);
            CellTarget = null;
            MSound.SoundPlayMakeBomb(0, null);
        }
        private void HorizontalCollectHandler5(MatchGroup m)
        {    
            SetDynamicClickBombColor(CellTarget ? CellTarget : m.lastMatchedCell);
            CellTarget = null;
            MSound.SoundPlayMakeBomb(0, null);
        }

        private void VerticalCollectHandler5(MatchGroup m)
        { 
            SetDynamicClickBombColor(CellTarget ? CellTarget : m.lastMatchedCell);
            CellTarget = null;
            MSound.SoundPlayMakeBomb(0, null);
        }
        private void HorizontalCollectHandler4(MatchGroup m)
        {
            //switch (bombType)
            //{
            //    case BombType.StaticMatch:
            //        SetStaticMatchBombVert(m.lastMatchedCell, m.lastMatchedID);
            //        break;
            //    case BombType.DynamicMatch:
            //        SetDynamicMatchBombVert(m.lastMatchedCell, m.lastMatchedID);
            //        break;
            //    case BombType.DynamicClick:
                    SetDynamicClickBombVert(CellTarget ? CellTarget : m.lastMatchedCell);// m.lastMatchedCell);
                    CellTarget = null;
            //        break;

            //}
            MSound.SoundPlayMakeBomb(0,null);
        }

        private void VerticalCollectHandler4(MatchGroup m)
        {
            //switch (bombType)
            //{
            //    case BombType.StaticMatch:
            //        SetStaticMatchBombHor(m.lastMatchedCell, m.lastMatchedID);
            //        break;
            //    case BombType.DynamicMatch:
            //        SetDynamicMatchBombHor(m.lastMatchedCell, m.lastMatchedID);
            //        break;
            //    case BombType.DynamicClick:
                    SetDynamicClickBombHor(CellTarget ? CellTarget : m.lastMatchedCell);//m.lastMatchedCell);
                    CellTarget = null;
            //        break;
            //}
            MSound.SoundPlayMakeBomb(0, null);
        }

        private void CrossCollectHandler(MatchGroup m)
        {
            //switch (bombType)
            //{
            //    case BombType.StaticMatch:
            //        SetStaticMatchBombRadial(m.lastMatchedCell, m.lastMatchedID);
            //        break;
            //    case BombType.DynamicMatch:
            //        SetDynamicMatchBombRadial(m.lastMatchedCell, m.lastMatchedID);
            //        break;
            //    case BombType.DynamicClick:
                    SetDynamicClickBombDinamit(CellTarget ? CellTarget : m.lastMatchedCell);//m.lastMatchedCell);
                    CellTarget = null;
            //        break;
            //}
            MSound.SoundPlayMakeBomb(0, null);
        }

        private void ComboCollectHandler(MatchGroupsHelper mgH)
        {
            // combo message
        }

        private void GreatingMessage()
        {
            if (WinContr.Result == GameResult.WinAuto) return; // EffectsHolder.Instance.InstantiateScoreFlyerAtPosition(s, scoreFlyerPos, f);
            int add = (collected - 3) * 10;
            int score = collected * 10 + Math.Max(0, add);
            if (score > 159 && score < 189)
            {
                Debug.Log("GOOD");
                MGui.ShowMessageGood("Good", null, 1f);
            }
            else if (score > 189 && score < 219)
            {
                Debug.Log("GREAT");
                MGui.ShowMessageGreat("Great", null, 1f);
            }
            else if (score > 219)
            {
                Debug.Log("EXCELLENT");
                MGui.ShowMessageExcellent("Excellent", null, 1f);
            }
        }

        /// <summary>
        /// async collect matched objects in a group
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void CollectHandler(CellsGroup m, Action completeCallBack)
        {
            float delay = 0;
            ParallelTween collectTween = new ParallelTween();
            if (m.Length > 3) // create bomb
            {
                foreach (GridCell c in m.Cells) // move and collect
                {
                    if (Targets.ContainsKey(m.lastMatchedID))                 //m.lastMatchedCell
                    {
                        collectTween.Add((callBack) => { c.MoveMatchAndCollect(CellTarget ? CellTarget : m.lastMatchedCell, 0, false, flyCollected, true, showBombExplode, dragExplode, showWhirlWind, callBack); });
                    }
                    else
                        collectTween.Add((callBack) => { c.MoveMatchAndCollect(CellTarget ? CellTarget : m.lastMatchedCell, 0, false, false, true, showBombExplode, dragExplode, showWhirlWind, callBack); });
                }
            }
            else
            {
                CellTarget = null;

                foreach (GridCell c in m.Cells)
                {
                    delay += 0.1f;
                    float d = delay;                       
                    if (Targets.ContainsKey(m.lastMatchedID))
                        collectTween.Add((callBack) => { c.CollectMatch(d, true, flyCollected, true, true, showBombExplode,  dragExplode, showWhirlWind, callBack); });
                    else
                        collectTween.Add((callBack) => { c.CollectMatch(d, true, false, true, true, showBombExplode, dragExplode, showWhirlWind, callBack); });
                }
            }
            collectTween.Start(completeCallBack);
        }
        #endregion match group handlers score counter

        private void MakeStep()
        {
            List<GridCell> bomb = grid.GetBombs();
            if (bomb.Count > 0)
            {
                SetControlActivity(false, false);
                MbState = MatchBoardState.Waiting;
                for(int i=0; i< bomb.Count; i++)
                  bomb[i].ExplodeBomb(0, true, false, true, dragExplode, false, flyCollected, () =>
                  {
                    WinContr.MakeMove();
                    MbState = MatchBoardState.Fill;
                    SetControlActivity(true, true);
                  });
                return;
            }

            if (!fastAuto)
            {
                if (EstimateGroups.Length > 0)
                {
                    EstimateGroups.SwapEstimate();
                }
            }
            else
            {
                SetControlActivity(false, false);
                int moves = WinContr.MovesRest;
                int bombsCount = Mathf.Min(moves, 3);
                List<GridCell> cells = grid.GetRandomMatch(bombsCount);

                MbState = MatchBoardState.Waiting;
                if (cells.Count > 2)
                    BombObject.ExplodeArea(cells[2].GColumn.cells, 0, true, false, true, true, true, () =>
                    {
                        WinContr.MakeMove();
                    });

                if (cells.Count > 1)                           // 0.05f
                    BombObject.ExplodeArea(cells[1].GRow.cells, 0, true, false, true, true, true, () =>
                   {
                       WinContr.MakeMove();
                   });

                BombObject.ExplodeArea(cells[0].GRow.cells, 0.15f, true, false, true, true, true, () =>
                {
                    // set 3 bombs
                    WinContr.MakeMove();
                    MbState = MatchBoardState.Fill; //    Debug.Log("end swap -> to fill");
                    SetControlActivity(true, true);
                });

            }
        }

        public bool NeedAlmostMessage()
        {
            return showAlmostMessage && (almostCoins <= MPlayer.Coins);
        }

        #region undo
        /// <summary>
        ///  Save Undo state of the match board
        /// </summary>
        internal void SaveUndoState()
        {
            if (undoStates == null) undoStates = new List<DataState>();
            if (undoStates.Count >= 5)
            {
                undoStates.RemoveAt(0);
            }
            DataState ds = new DataState(this, MPlayer);
            undoStates.Add(ds);
            // Debug.Log("save undo state" + undoStates.Count);
            grid.Cells.ForEach((ct) => { ct.SaveUndoState(); });
        }

        /// <summary>
        /// set the prev state on board
        /// </summary>
        public void PreviousState()
        {
            if (GMode == GameMode.Edit) return;
            if (Time.timeScale == 0) return;
            if (undoStates == null || undoStates.Count == 0) return;
            grid.Cells.ForEach((ct) => { ct.PreviousUndoState(); });
            DataState ds = undoStates[undoStates.Count - 1];
            ds.RestoreState(this, MPlayer);
            undoStates.RemoveAt(undoStates.Count - 1);
            HeaderGUIController.Instance.Refresh();
        }

        /// <summary>
        /// Set Time.timescale =(Time.timescale!=0)? 0 : 1
        /// </summary>
        public void Pause()
        {
            if (GMode == GameMode.Edit) return;
            if (Time.timeScale == 0.0f)
            {
                Time.timeScale = 1f;
                SetControlActivity(true, true);
            }

            else if (Time.timeScale > 0f)
            {
                Time.timeScale = 0f;
                SetControlActivity(false, true);
            }
        }
        #endregion undo

        #region boosters
        /// <summary>
        /// Aplly active booster to gridcell
        /// </summary>
        /// <param name="gCell"></param>
        private void ApplyBooster(GridCell gCell, Action complete)
        {
            collected = 0; // reset collected count
            if (Booster.ActiveBooster != null)
            {
                MbState = MatchBoardState.Waiting;
                SetControlActivity(false, false);
                Booster.ActiveBooster.Apply(gCell, ()=> { MbState = MatchBoardState.Fill; });
                complete?.Invoke();
            }
        }
        #endregion boosters
    }

    public class MatchGroupsHelper
    {
        private List<MatchGroup> mgList;
        private MatchGrid grid;

        public Action<MatchGroup> QuadroCollect;
        public Action<MatchGroup> HorizontalCollect5;
        public Action<MatchGroup> VerticalCollect5;
        public Action<MatchGroup> HorizontalCollect4;
        public Action<MatchGroup> VerticalCollect4;
        public Action<MatchGroup> CrossCollect4;
        public Action<MatchGroupsHelper> ComboCollect;
        public Action<MatchGroup, Action> Collect;

        public int Length
        {
            get { return mgList.Count; }
        }

        /// <summary>
        /// Find match croups on grid and estimate match groups
        /// </summary>
        public MatchGroupsHelper(MatchGrid grid )
        {
             mgList = new List<MatchGroup>();
            this.grid = grid;   
        }

        public void CreateGroups(int minMatches, bool estimate)
        {
            mgList = new List<MatchGroup>();
            if (!estimate)
            {
                grid.Rows.ForEach((br) =>
                {
                    List<MatchGroup> mgList_t = br.GetMatches(minMatches, false);
                    if (mgList_t != null && mgList_t.Count > 0)
                    {
                        AddRange(mgList_t);
                    }
                });

                grid.Columns.ForEach((bc) =>
                {
                    List<MatchGroup> mgList_t = bc.GetMatches(minMatches, false);
                    if (mgList_t != null && mgList_t.Count > 0)
                    {
                        AddRange(mgList_t);
                    }
                });
            }
            else
            {
                List<MatchGroup> mgList_t = new List<MatchGroup>();
                grid.Rows.ForEach((gr) =>
                {
                    mgList_t.AddRange(gr.GetMatches(minMatches, true));
                });
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, true, grid)) { AddEstimate(mg); } });

                mgList_t = new List<MatchGroup>();
                grid.Columns.ForEach((gc) =>
                {
                    mgList_t.AddRange(gc.GetMatches(minMatches, true));
                });
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, false, grid)) { AddEstimate(mg); } });
            }
        }

        /// <summary>
        /// Add new matchgroup and merge all intersections
        /// </summary>
        public void Add(MatchGroup mG)
        {
            List<MatchGroup> intersections = new List<MatchGroup>();

            for (int i = 0; i < mgList.Count; i++)
            {
                if (mgList[i].IsIntersectWithGroup(mG))
                {
                    intersections.Add(mgList[i]);
                }
            }
            // merge intersections
            if (intersections.Count > 0)
            {
                intersections.ForEach((ints) => { mgList.Remove(ints); });
                intersections.Add(mG);
                mgList.Add(Merge(intersections));
            }
            else
            {
                mgList.Add(mG);
            }
        }

        /// <summary>
        /// Add new estimate matchgroup
        /// </summary>
        public void AddEstimate(MatchGroup mGe)
        {
            for (int i = 0; i < mgList.Count; i++)
            {
                if (mgList[i].IsEqual(mGe))
                {
                    return;
                }
            }
            mgList.Add(mGe);
        }

        /// <summary>
        /// Add new matchgroup List and merge all intersections
        /// </summary>
        public void AddRange(List<MatchGroup> mGs)
        {
            for (int i = 0; i < mGs.Count; i++)
            {
                Add(mGs[i]);
            }
        }

        private MatchGroup Merge(List<MatchGroup> intersections)
        {
            MatchGroup mG = new MatchGroup();
            intersections.ForEach((ints) => { mG.Merge(ints); });
            return mG;
        }

        TweenSeq showSequence;
        public void ShowMatchGroupsSeq(Action completeCallBack)
        {
            showSequence = new TweenSeq();
            if (mgList.Count > 0)
            {
                Debug.Log("show match");
                foreach (MatchGroup mG in mgList)
                {
                    showSequence.Add((callBack) =>
                    {
                        mG.Show(callBack);
                    });
                }
            }
            showSequence.Add((callBack) =>
            {
                if (completeCallBack != null) completeCallBack();
                Debug.Log("show match ended");
                callBack();
            });
            showSequence.Start();
        }

        public void ShowMatchGroupsPar(Action completeCallBack)
        {
            showSequence = new TweenSeq();
            ParallelTween showTweenPar = new ParallelTween();

            if (mgList.Count > 0)
            {
                //  Debug.Log("show match");
                foreach (MatchGroup mG in mgList)
                {
                    showTweenPar.Add((callBack) =>
                    {
                        mG.Show(callBack);
                    });
                }
            }

            showSequence.Add((callBack) =>
            {
                showTweenPar.Start(callBack);
            });

            showSequence.Add((callBack) =>
            {
                if (completeCallBack != null) completeCallBack();
                // Debug.Log("show match ended");
                callBack();
            });
            showSequence.Start();
        }

        TweenSeq showEstimateSequence;
        public void ShowEstimateMatchGroupsSeq(Action completeCallBack)
        {
            showEstimateSequence = new TweenSeq();
            if (mgList.Count > 0)
            {
                foreach (MatchGroup mG in mgList)
                {
                    showEstimateSequence.Add((callBack) => { mG.Show(callBack); });
                }
            }
            showEstimateSequence.Add((callBack) =>
            {
                completeCallBack?.Invoke();
                callBack();
            });
            showEstimateSequence.Start();
        }

        static int next = 0;
        public void ShowNextEstimateMatchGroups(Action completeCallBack)
        {
            showEstimateSequence = new TweenSeq();
            next = (next < mgList.Count) ? next : 0;
            int n = next;
            if (mgList.Count > 0)
            {
                showEstimateSequence.Add((callBack) => { mgList[n].Show(callBack); });
            }
            showEstimateSequence.Add((callBack) =>
            {
                completeCallBack?.Invoke();
                callBack();
            });
            showEstimateSequence.Start();
            next++;
        }

        public void CollectMatchGroups (Action completeCallBack)
        {
             ParallelTween pt = new ParallelTween();

            if (mgList.Count == 0)
            {
                completeCallBack?.Invoke();
                return;
            }
            
            for (int i = 0; i < mgList.Count; i++)
            {
                if (mgList[i] != null)
                {
                    MatchGroup m = mgList[i];
                    pt.Add((callBack) =>
                    {
                        Collect(m, ()=> {
                            //Debug.Log(m.ToString() +  "collected");
                            CollectEventsRaise(m);
                            callBack();
                        });
                    });
                }
            }
            pt.Start(() =>
            {
                if (mgList.Count > 1)
                    ComboCollect?.Invoke(this);
                completeCallBack?.Invoke();
            });
        }

        private void CollectEventsRaise(MatchGroup m)
        {
            if (m.Length > 4 && m.IsVertical() && m.BombsCount == 0) //5 vertical bonus  - color bomb
            {
                VerticalCollect5?.Invoke(m);
                return;
            }
            else if (m.Length > 4 && m.IsHorizonal() && m.BombsCount == 0) //5 horizontal bonus - color bomb
            {
                HorizontalCollect5?.Invoke(m);
                return;
            }
            else if (m.Length > 3 && m.IsHorizonal() && m.BombsCount == 0) // horizontal bonus - hor bomb
            {
                HorizontalCollect4?.Invoke(m);
                return;
            }
            else if (m.Length > 3 && m.IsVertical() && m.BombsCount == 0) // vertical bonus  - ver bomb
            {
                VerticalCollect4?.Invoke(m);
                return;
            }
            //else if (m.Length > 3 && m.IsSide2() && m.BombsCount == 0) // quadro bonus  - butterfly bomb
            //{
            //    QuadroCollect?.Invoke(m);
            //    return;
            //}
            else if (m.Length > 3 && m.BombsCount == 0) // cross bonus - bomb
            {
               CrossCollect4?.Invoke(m);
            }
        }

        public override string ToString()
        {
            string s = "";
            mgList.ForEach((mg) => { s += mg.ToString(); });
            return s;
        }

        public void CancelTweens()
        {
            if (showSequence != null) { showSequence.Break(); showSequence = null; }
            if (showEstimateSequence != null) { showEstimateSequence.Break(); showEstimateSequence = null; }
            mgList.ForEach((mg) => { mg.CancelTween(); });
        }

        public void SwapEstimate()
        {
            mgList[0].SwapEstimate();
        }
    }

    public class MatchGroup : CellsGroup
    {
        private GridCell est1;
        private GridCell est2;

        public bool IsIntersectWithGroup(MatchGroup mGroup)
        {
            if (mGroup == null || mGroup.Length == 0) return false;
            for (int i = 0; i < Cells.Count; i++)
            {
                if (mGroup.Contain(Cells[i])) return true;
            }
            return false;
        }

        public void Merge(MatchGroup mGroup)
        {
            if (mGroup == null || mGroup.Length == 0) return;
            for (int i = 0; i < mGroup.Cells.Count; i++)
            {
                Add(mGroup.Cells[i]);
            }
        }

        public bool IsEqual(MatchGroup mGroup)
        {
            if (Length != mGroup.Length) return false;
            foreach (GridCell c in Cells)
            {
                if (!mGroup.Contain(c)) return false;
            }
            return true;
        }

        public bool IsEstimateMatch(int matchCount, bool horizontal, MatchGrid grid)
        {
            if (Length != matchCount) return false;
            if (horizontal)
            {
                GridCell L = GetLowermostX();
                GridCell T = GetTopmostX();

                // 3 estimate positions for l - cell (astrics)
                //   1 X X
                // 3 0 L T X
                //   2 X X
                int X0 = L.Column - 1; int Y0 = L.Row;
                GridCell c0 = grid[Y0, X0];

                if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 1))
                {
                    int X1 = X0; int Y1 = Y0 - 1;
                    GridCell c1 = grid[Y1, X1];
                    if ((c1 != null) && c1.IsMatchObjectEquals(L) && c1.IsDraggable())
                    {
                        Add(c1);
                        est1 = c0;
                        est2 = c1;
                        return true;
                    }

                    int X2 = X0; int Y2 = Y0 + 1;
                    GridCell c2 = grid[Y2, X2];
                    if ((c2 != null) && c2.IsMatchObjectEquals(L) && c2.IsDraggable())
                    {
                        Add(c2);
                        est1 = c0;
                        est2 = c2;
                        return true;
                    }

                    int X3 = X0 - 1; int Y3 = Y0;
                    GridCell c3 = grid[Y3, X3];
                    if ((c3 != null) && c3.IsMatchObjectEquals(L) && c3.IsDraggable())
                    {
                        Add(c3);
                        est1 = c0;
                        est2 = c3;
                        return true;
                    }
                }

                // 3 estimate positions for T - cell (astrics)
                //    X X 4
                //  X L T 0 6
                //    X X 5
                X0 = T.Column + 1; Y0 = T.Row;
                c0 = grid[Y0, X0];
                if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 1))
                {
                    int X4 = X0; int Y4 = Y0 - 1;
                    GridCell c4 = grid[Y4, X4];
                    if ((c4 != null) && c4.IsMatchObjectEquals(T) && c4.IsDraggable())
                    {
                        Add(c4);
                        est1 = c0;
                        est2 = c4;
                        return true;
                    }

                    int X5 = X0; int Y5 = Y0 + 1;
                    GridCell c5 = grid[Y5, X5];
                    if ((c5 != null) && c5.IsMatchObjectEquals(T) && c5.IsDraggable())
                    {
                        Add(c5);
                        est1 = c0;
                        est2 = c5;
                        return true;
                    }

                    int X6 = X0 + 1; int Y6 = Y0;
                    GridCell c6 = grid[Y6, X6];
                    if ((c6 != null) && c6.IsMatchObjectEquals(T) && c6.IsDraggable())
                    {
                        Add(c6);
                        est1 = c0;
                        est2 = c6;
                        return true;
                    }
                }

                // 2 estimate positions for L0T - horizontal
                //    X 7 X
                //  X L 0 T X
                //    X 8 X
                X0 = L.Column + 1; Y0 = L.Row;
                c0 = grid[Y0, X0];
                if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 2))
                {
                    int X7 = L.Column + 1; int Y7 = L.Row - 1;
                    GridCell c7 = grid[Y7, X7];
                    if (c7 != null && c7.IsMatchObjectEquals(L) && c7.IsDraggable())
                    {
                        Add(c7);
                        est1 = c0;
                        est2 = c7;
                        return true;
                    }

                    int X8 = L.Column + 1; int Y8 = L.Row + 1;
                    GridCell c8 = grid[Y8, X8];
                    if (c8 != null && c8.IsMatchObjectEquals(L) && c8.IsDraggable())
                    {
                        Add(c8);
                        est1 = c0;
                        est2 = c8;
                        return true;
                    }
                }
            }
            else
            {
                GridCell L = GetLowermostY();
                GridCell T = GetTopmostY();
                // 3 estimate positions for L - cell 
                //     
                //     X 
                //   X T X 
                //   X L X
                //   1 0 2
                //     3
                int X0 = L.Column; int Y0 = L.Row + 1;
                GridCell c0 = grid[Y0, X0];
                if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -1))
                {
                    int X1 = X0 - 1; int Y1 = Y0;
                    GridCell c1 = grid[Y1, X1];
                    if ((c1 != null) && c1.IsMatchObjectEquals(L) && c1.IsDraggable())
                    {
                        Add(c1);
                        est1 = c0;
                        est2 = c1;
                        return true;
                    }

                    int X2 = X0 + 1; int Y2 = Y0;
                    GridCell c2 = grid[Y2, X2];
                    if ((c2 != null) && c2.IsMatchObjectEquals(L) && c2.IsDraggable())
                    {
                        Add(c2);
                        est1 = c0;
                        est2 = c2;
                        return true;
                    }

                    int X3 = X0; int Y3 = Y0 + 1;
                    GridCell c3 = grid[Y3, X3];
                    if ((c3 != null) && c3.IsMatchObjectEquals(L) && c3.IsDraggable())
                    {
                        Add(c3);
                        est1 = c0;
                        est2 = c3;
                        return true;
                    }
                }

                // 3 estimate positions for T - cell
                //     6
                //   4 0 5
                //   X T X 
                //   X L X
                //     X 
                X0 = L.Column; Y0 = T.Row - 1;
                c0 = grid[Y0, X0];
             //   Debug.Log("c0: " + c0 + " : " + c0.IsDraggable() +" : " + ((T.Row - L.Row)));
                if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -1))
                {
                    int X4 = T.Column - 1; int Y4 = T.Row - 1;
                    GridCell c4 = grid[Y4, X4];
                    if ((c4 != null) && c4.IsMatchObjectEquals(L) && c4.IsDraggable())
                    {
                        Add(c4);
                        est1 = c0;
                        est2 = c4;
                        return true;
                    }

                    int X5 = T.Column + 1; int Y5 = T.Row - 1;
                    GridCell c5 = grid[Y5, X5];
                    if ((c5 != null) && c5.IsMatchObjectEquals(L) && c5.IsDraggable())
                    {
                        Add(c5);
                        est1 = c0;
                        est2 = c5;
                        return true;
                    }

                    int X6 = T.Column; int Y6 = T.Row - 2;
                    GridCell c6 = grid[Y6, X6];
                    if ((c6 != null) && c6.IsMatchObjectEquals(L) && c6.IsDraggable())
                    {
                        Add(c6);
                        est1 = c0;
                        est2 = c6;
                        return true;
                    }
                }

                // 2 estimate positions for T0L - vertical
                //      X
                //    X T X
                //    7 0 8 
                //    X L X
                //      X
                X0 = L.Column; Y0 = L.Row - 1;
                c0 = grid[Y0, X0];
                if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -2))
                {
                    int X7 = X0 - 1; int Y7 = Y0;
                    GridCell c7 = grid[Y7, X7];
                    if ((c7 != null) && c7.IsMatchObjectEquals(L) && c7.IsDraggable())
                    {
                        Add(c7);
                        est1 = c0;
                        est2 = c7;
                        return true;
                    }

                    int X8 = X0 + 1; int Y8 = Y0;
                    GridCell c8 = grid[Y8, X8];
                    if ((c8 != null) && c8.IsMatchObjectEquals(L) && c8.IsDraggable())
                    {
                        Add(c8);
                        est1 = c0;
                        est2 = c8;
                        return true;
                    }
                }
            }
            return false;
        }

        internal void SwapEstimate()
        {
            if (est1 && est2)
            {
                //Debug.Log("swap estimate");
                //est1.Swap(est2.Match);
                SwapHelper.Swap(est1, est2);
            }
        }
    }

    public class CellsGroup
    {
        public List<GridCell> Cells { get; private set; }
        public List<GridCell> Bombs { get; private set; }
        public int lastObjectOrderNumber;
        public int lastMatchedID { get; private set; }
        public GridCell lastAddedCell { get; private set; }
        public GridCell lastMatchedCell { get; private set; }

        public int MinYPos
        {
            get; private set;
        }

        public bool Contain(GridCell mCell)
        {
            return Cells.Contains(mCell);
        }

        public int Length
        {
            get { return Cells.Count; }
        }

        public CellsGroup()
        {
            Cells = new List<GridCell>();
            Bombs = new List<GridCell>();
            MinYPos = -1;
        }

        public void Add(GridCell mCell)
        {
            if (!Cells.Contains(mCell))
            {
                Cells.Add(mCell);
                MinYPos = (mCell.Row < MinYPos) ? mCell.Row : MinYPos;
                lastAddedCell = mCell;
                lastMatchedCell = (lastMatchedCell == null || lastMatchedCell.Match == null) ? mCell : lastMatchedCell;

                if (mCell.Match)
                {
                    lastObjectOrderNumber = mCell.Match.OData.ID;
                    lastMatchedCell = (lastMatchedCell.Match.SwapTime < mCell.Match.SwapTime) ? mCell : lastMatchedCell;
                    lastMatchedID = lastMatchedCell.Match.GetID();

                    if (mCell.HasBomb)
                    {
                        { Bombs.Add(mCell); }
                    }
                }
            }
        }

        public void AddRange(IEnumerable <GridCell> mCells)
        {
            if (mCells != null)
            {
                foreach (var item in mCells)
                {
                    Add(item);
                }
            }
        }

        public void CancelTween()
        {
            Cells.ForEach((c) => { c.CancelTween(); });
        }

        public override string ToString()
        {
            string s = "";
            Cells.ForEach((c) => { s += c.ToString(); });
            return s;
        }

        public GridCell GetLowermostX()
        {
            if (Cells.Count == 0) return null;
            GridCell l = Cells[0];
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Column < l.Column) l = Cells[i];
            }
            return l;
        }

        public GridCell GetTopmostX()
        {
            if (Cells.Count == 0) return null;
            GridCell t = Cells[0];
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Column > t.Column) t = Cells[i];
            }
            return t;
        }

        public GridCell GetLowermostY()
        {
            if (Cells.Count == 0) return null;
            GridCell l = Cells[0];
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Row > l.Row) l = Cells[i];
            }
            return l;
        }

        public GridCell GetTopmostY()
        {
            if (Cells.Count == 0) return null;
            GridCell t = Cells[0];
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Row < t.Row) t = Cells[i];
            }
            return t;
        }

        internal bool IsHorizonal()
        {
            if (Cells.Count < 2) return false;
            int row = Cells[0].Row;
            for (int i = 1; i < Cells.Count; i++)
            {
                if (row != Cells[i].Row) return false;
            }
            return true;
        }

        internal bool IsVertical()
        {
            if (Cells.Count < 2) return false;
            int column = Cells[0].Column;
            for (int i = 1; i < Cells.Count; i++)
            {
                if (column != Cells[i].Column) return false;
            }
            return true;
        }

        internal bool IsSide2()
        {
            if (Cells.Count < 2) return false;
            int row = Cells[0].Row;
            //for (int i = 1; i < Cells.Count; i++)
            //{
            //    if (row != Cells[i].Row) return false;
            //}

            int column = Cells[0].Column;
            //for (int i = 1; i < Cells.Count; i++)
            //{
            //    if (column != Cells[i].Column) return false;
            //}
            return (row == 2 || column == 2);
        }

        /// <summary>
        /// Scaling sequenced group
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void Show(Action completeCallBack)
        {
            ParallelTween showTween = new ParallelTween();
            foreach (GridCell gc in Cells)
            {
                showTween.Add((callBack) =>
                {
                    gc.ZoomMatch(callBack);
                });
            }
            showTween.Start(completeCallBack);
        }

        internal int BombsCount
        {
            get { return Bombs.Count; }
        }

        internal void Remove(GridCell mCell)
        {
            if (mCell == null) return;
            if (Contain(mCell))
            {
                Cells.Remove(mCell);
            }
        }

        internal void Remove(List<GridCell> mCells)
        {
            if (mCells == null) return;
            for (int i = 0; i < mCells.Count; i++)
            {
                if (Contain(mCells[i]))
                {
                    Cells.Remove(mCells[i]);
                }
            }
        }
    }

    public class Row<T> : CellArray<T> where T : GridCell
    {
        public Row(int size) : base(size) { }

        public void CreateWestWind(GameObject prefab, Vector3 scale, Transform parent, Action completeCallBack)
        {
            GameObject s = UnityEngine.Object.Instantiate(prefab, cells[0].transform.position, Quaternion.identity);
            s.transform.localScale = scale;
            s.transform.parent = parent;
          
            Vector3 dPos = new Vector3((cells[0].transform.localPosition - cells[1].transform.localPosition).x * 3.0f,0, 0);
            s.transform.localPosition += dPos;

            Vector3 endPos = cells[cells.Length - 1].transform.position - dPos * scale.x; 
            Whirlwind w = s.GetComponent<Whirlwind>();
            w.Create(endPos, completeCallBack);
        }

        /// <summary>
        /// Get right cells
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<T> GetRightCells(int index)
        {
            List<T> cs = new List<T>();
            if (ok(index))
            {
                int i = Length - 1;
                while (i > index)
                {
                    cs.Add(cells[i]);
                    i--;
                }
            }
            return cs;
        }

        /// <summary>
        /// Get right cell
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetRightCell(int index)
        {
            if (ok(index + 1))
            {
                return cells[index + 1];
            }
            return null;
        }

        /// <summary>
        /// Get left cells
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<T> GetLeftCells(int index)
        {
            List<T> cs = new List<T>();
            if (ok(index))
            {
                int i = 0;
                while (i < index)
                {
                    cs.Add(cells[i]);
                    i++;
                }
            }
            return cs;
        }

        /// <summary>
        /// Get left cell
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetLeftCell(int index)
        {
            if (ok(index - 1))
            {
                return cells[index - 1];
            }
            return null;
        }

        public T GetLeftDynamic()
        {
            return GetMinDynamic();
        }

        public T GetRightDynamic()
        {
            return GetMaxDynamic();
        }
    }

    public class Column<T> : CellArray<T> where T : GridCell
    {
        public Spawner Spawn { get; private set; }

        public Column(int size) : base(size) { }

        public void CreateTopSpawner(GameObject prefab, SpawnerStyle sStyle, Vector3 scale, Transform parent)
        {
            switch (sStyle)
            {
                case SpawnerStyle.AllEnabled:
                    GridCell gc = GetTopDynamic();
                    if (gc)
                    {
                        Vector3 pos = gc.transform.position;
                        GameObject s = UnityEngine.Object.Instantiate(prefab, cells[0].transform.position, Quaternion.identity);
                        s.transform.localScale = scale;
                        s.transform.parent = parent;
                        s.transform.localPosition += new Vector3(0, (cells[0].transform.position - cells[1].transform.position).y * 1.3f, 0);
                        Spawn = s.GetComponent<Spawner>();
                        Spawn.gridCell = gc;
                        gc.spawner = Spawn;
                    }
                    break;
                case SpawnerStyle.AllEnabledAlign:
                    GridCell c = GetTopDynamic();
                    if (c)
                    {
                        GameObject sa = UnityEngine.Object.Instantiate(prefab, GetTopDynamic().transform.position, Quaternion.identity);
                        sa.transform.localScale = scale;
                        sa.transform.parent = parent;
                        sa.transform.localPosition += new Vector3(0, (cells[0].transform.position - cells[1].transform.position).y * 1.3f, 0);
                        Spawn = sa.GetComponent<Spawner>();
                        Spawn.gridCell = c;
                        c.spawner = Spawn;
                    }

                    break;
                case SpawnerStyle.DisabledAligned:
                    if (!cells[0].Blocked && !cells[0].IsDisabled)// && !cells[0].Overlay)
                    {
                        GameObject sd = UnityEngine.Object.Instantiate(prefab, cells[0].transform.position, Quaternion.identity);
                        sd.transform.localScale = scale;
                        sd.transform.parent = parent;
                        sd.transform.localPosition += new Vector3(0, (cells[0].transform.position - cells[1].transform.position).y * 1.3f, 0);
                        Spawn = sd.GetComponent<Spawner>();
                        Spawn.gridCell = cells[0];
                        cells[0].spawner = Spawn;
                    }
                    break;
            }
           
        }

        public void CreateNordWind(GameObject prefab, Vector3 scale, Transform parent, Action completeCallBack)
        {
            GameObject s = UnityEngine.Object.Instantiate(prefab, cells[0].transform.position, Quaternion.identity);
            s.transform.localScale = scale;
            s.transform.parent = parent;
            s.transform.eulerAngles = new Vector3(0, 0, -90);
            Vector3 dPos = new Vector3( 0, (cells[0].transform.localPosition - cells[1].transform.localPosition).y * 3.0f, 0);
            s.transform.localPosition += dPos;

            Vector3 endPos = cells[cells.Length - 1].transform.position - dPos * scale.x;
            Whirlwind w = s.GetComponent<Whirlwind>();
            w.Create(endPos, completeCallBack);
        }

        public T GetTopDynamic()
        {
            return GetMinDynamic();
        }

        public T GetBottomDynamic()
        {
            return GetMaxDynamic();
        }

        /// <summary>
        /// Get cells at top
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<T> GetTopCells(int index)
        {
            List<T> cs = new List<T>();
            if (ok(index))
            {
                int i = 0;
                while (i < index)
                {
                    cs.Add(cells[i]);
                    i++;
                }
            }
            return cs;
        }

        /// <summary>
        /// Get cell at top
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetTopCell(int index)
        {
            if (ok(index - 1))
            {
                return cells[index - 1];
            }
            return null;
        }

        /// <summary>
        /// Get bottom cells
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<T> GetBottomCells(int index)
        {
            List<T> cs = new List<T>();
            if (ok(index))
            {
                int i = Length-1;
                while (i > index)
                {
                    cs.Add(cells[i]);
                    i--;
                }
            }
            return cs;
        }

        /// <summary>
        /// Get bottom cell
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetBottomCell(int index)
        {
            if (ok(index + 1))
            {
                return cells[index - 1];
            }
            return null;
        }
    }

    public class CellArray<T> : GenInd<T> where T : GridCell
    {
        public CellArray(int size) : base(size) { }

        public static bool AllMatchObjectsIsEqual(GridCell[] mcs)
        {
            if (mcs == null || !mcs[0] || mcs.Length < 2) return false;
            for (int i = 1; i < mcs.Length; i++)
            {
                if (!mcs[i]) return false;
                if (!mcs[0].IsMatchObjectEquals(mcs[i])) return false;
            }
            return true;
        }

        public List<MatchGroup> GetMatches4()
        {
            List<MatchGroup> mgList = new List<MatchGroup>();
            MatchGroup mg = new MatchGroup();
            mg.Add(cells[0]);
            for (int i = 1; i < cells.Length; i++)
            {
                int prev = mg.Length - 1;
                if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[prev]) && mg.Cells[prev].IsMatchable)
                {
                    mg.Add(cells[i]);
                    if (i == cells.Length - 1 && mg.Length >= 2)
                    {
                        mgList.Add(mg);
                        mg = new MatchGroup();
                    }
                }
                else // start new match group
                {
                    if (mg.Length >= 2)
                    {
                        mgList.Add(mg);
                    }
                    mg = new MatchGroup();
                    mg.Add(cells[i]);
                }
            }
            return mgList;
        }

            public List<MatchGroup> GetMatches(int minMatches, bool X0X)
        {
            List<MatchGroup> mgList = new List<MatchGroup>();
            MatchGroup mg = new MatchGroup();
            mg.Add(cells[0]);
            for (int i = 1; i < cells.Length; i++)
            {
                int prev = mg.Length - 1;
                if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[prev])  && mg.Cells[prev].IsMatchable)
                {
                    mg.Add(cells[i]);
                    if (i == cells.Length - 1 && mg.Length >= minMatches)
                    {
                        mgList.Add(mg);
                        mg = new MatchGroup();
                    }
                }
                else // start new match group
                {
                    if (mg.Length >= minMatches)
                    {
                        mgList.Add(mg);
                    }
                    mg = new MatchGroup();
                    mg.Add(cells[i]);
                }
            }

            if (X0X) // [i-2, i-1, i]
            {
                mg = new MatchGroup();

                for (int i = 2; i < cells.Length; i++)
                {
                    mg.Add(cells[i - 2]);
                    if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[0]) && !cells[i - 1].IsMatchObjectEquals(mg.Cells[0])  && mg.Cells[0].IsMatchable && cells[i - 1].IsDraggable())
                    {
                        mg.Add(cells[i]);
                        mgList.Add(mg);
                    }
                    mg = new MatchGroup();
                }
            } // end X0X
            return mgList;
        }

        public List<T> GetDynamicArea()
        {
            List<T> res = new List<T>(Length);
            for (int i = 0; i < Length; i++)
            {
              //  if (cells[i].DynamicObject)
                if(!cells[i].IsDisabled && !cells[i].Blocked)
                {
                   res.Add(cells[i]);
                }
            }
            return res;
        }

        public T GetMinDynamic()
        {
            for (int i = 0; i < Length; i++)
            {
                if (cells[i].DynamicObject || (!cells[i].Blocked && !cells[i].IsDisabled))
                {
                    return cells[i];
                }
            }
            return null;
        }

        public T GetMaxDynamic()
        {
            for (int i = Length - 1; i >= 0; i--)
            {
                if (cells[i].DynamicObject || (!cells[i].Blocked && !cells[i].IsDisabled))
                {
                    return cells[i];
                }
            }
            return null;
        }

        public Vector3 GetDynamicCenter()
        {
            T l = GetMinDynamic();
            T r = GetMaxDynamic();

            if (l && r) return (l.transform.position + r.transform.position) / 2f;
            else if (l) return l.transform.position;
            else if (r) return r.transform.position;
            else return Vector3.zero;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < cells.Length; i++)
            {
                s += cells[i].ToString();
            }
            return s;
        }

    }

    public class GenInd<T> where T : class
    {
        public T[] cells;
        public int Length;

        public GenInd(int size)
        {
            cells = new T[size];
            Length = size;
        }

        public T this[int index]
        {
            get { if (ok(index)) { return cells[index]; } else {  return null; } }
            set { if (ok(index)) { cells[index] = value; } else {  } }
        }

        protected bool ok(int index)
        {
            return (index >= 0 && index < Length);
        }

        public T GetMiddleCell()
        {
            int number = Length / 2;

            return cells[number];
        }
    }

    public class DataState
    {
        public int target0Count;
        public int target1Count;
        public int target2Count;

        public int score;

        public int moviesCount;
        int[] boostersCount;

        public DataState(MatchBoard matchBoard, MatchPlayer MPlayer)
        {
            //target0Count = matchBoard.target0.currCount;
            //target1Count = matchBoard.target1.currCount;
            //target2Count = matchBoard.target2.currCount;

            score = MPlayer.LevelScore;
            //  moviesCount = MatchBoard.MoviesCount;
            //boostersCount = new int[matchBoard.allBoosters.Count];
            //for (int i = 0; i < matchBoard.allBoosters.Count; i++)
            //{
            //    boostersCount[i] = matchBoard.allBoosters[i].Count;
            //}
        }

        public void RestoreState(MatchBoard matchBoard, MatchPlayer MPlayer)
        {
            //matchBoard.target0.currCount = target0Count;
            //matchBoard.target1.currCount = target1Count;
            //matchBoard.target2.currCount = target2Count;

            MPlayer.SetScore(score);
            // MatchBoard.MoviesCount = moviesCount;
            //for (int i = 0; i < matchBoard.allBoosters.Count; i++)
            //{
            //    matchBoard.allBoosters[i].Count = boostersCount[i];
            //}
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MatchBoard))]
    public class MatchBoardEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            //EditorGUILayout.Space();
            //EditorGUILayout.Space();
            //#region test
            //if (EditorApplication.isPlaying)
            //{
            //    MatchBoard tg = (MatchBoard)target;
            //    if (MatchBoard.GMode == GameMode.Play)
            //    {
            //        if (test = EditorGUILayout.Foldout(test, "Test"))
            //        {
            //            #region fill
            //            EditorGUILayout.BeginHorizontal("box");
            //            if (GUILayout.Button("Fill(remove matches)"))
            //            {
            //                tg.grid.FillGrid(true);
            //            }
            //            if (GUILayout.Button("Fill"))
            //            {
            //                tg.grid.FillGrid(false);
            //            }
            //            if (GUILayout.Button("Remove matches"))
            //            {
            //                tg.grid.RemoveMatches();
            //            }
            //            EditorGUILayout.EndHorizontal();
            //            #endregion fill

            //            #region mix
            //            EditorGUILayout.BeginHorizontal("box");
            //            if (GUILayout.Button("Mix"))
            //            {
            //                tg.MixGrid(null); 
            //            }

            //            if (GUILayout.Button("Mix"))
            //            {
            //                tg.MixGrid(null);
            //            }
            //            EditorGUILayout.EndHorizontal();
            //            #endregion mix

            //            #region matches
            //            EditorGUILayout.BeginHorizontal("box");
            //            if (GUILayout.Button("Estimate check"))
            //            {
            //                 tg.EstimateGroups.CreateGroups( 2, true);
            //                Debug.Log("Estimate Length:" + tg.EstimateGroups.Length);
            //            }

            //            if (GUILayout.Button("Get free cells"))
            //            {
            //                Debug.Log("Free cells: " + tg.grid?.GetFreeCells().Count);
            //            }
            //            EditorGUILayout.EndHorizontal();
            //            #endregion matches

            //        }

            //        EditorGUILayout.LabelField("Board state: " + tg.MbState);
            //        EditorGUILayout.LabelField("Estimate groups count: " + ((tg.EstimateGroups!=null)? tg.EstimateGroups.Length.ToString(): "none"));
            //        EditorGUILayout.LabelField("Collect groups count: " + ((tg.CollectGroups != null) ? tg.CollectGroups.Length.ToString() : "none"));
            //        EditorGUILayout.LabelField("Free cells count: " + ((tg.grid!= null) ? tg.grid.GetFreeCells().Count.ToString() : "none"));

            //        return;
            //    }
            //}
            //EditorGUILayout.LabelField("Goto play mode for test");
            //#endregion test
        }
    }
#endif
}


