using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class GridCell : MonoBehaviour, ICustomMessageTarget
    {
        #region debug
        private bool debug = false;
        #endregion debug

        #region stroke
        [SerializeField]
        private Transform LeftTopCorner;
        [SerializeField]
        private Transform RightTopCorner;
        [SerializeField]
        private Transform LeftBotCorner;
        [SerializeField]
        private Transform RightBotCorner;
        [SerializeField]
        private Sprite Left;
        [SerializeField]
        private Sprite Right;
        [SerializeField]
        private Sprite Top;
        [SerializeField]
        private Sprite Bottom;
        [SerializeField]
        private Sprite OutTopLeft;
        [SerializeField]
        private Sprite OutBotLeft;
        [SerializeField]
        private Sprite OutTopRight;
        [SerializeField]
        private Sprite OutBotRight;

        [SerializeField]
        private Sprite InTopLeft;
        [SerializeField]
        private Sprite InBotLeft;
        [SerializeField]
        private Sprite InTopRight;
        [SerializeField]
        private Sprite InBotRight;

        #endregion stroke

        #region row column
        public Column<GridCell> GColumn { get; private set; }
        public Row<GridCell> GRow { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public List<Row<GridCell>> Rows { get; private set; }
        #endregion row column

        public bool tutorialSet { get; set; }

        #region objects
        public GameObject DynamicObject
        {
            get
            {
                if (Match) return Match.gameObject;
                if (DynamicClickBomb) return DynamicClickBomb.gameObject;
                if (Falling) return Falling.gameObject;
                return null;
            }
        }
        public MatchObject Match { get { return   GetComponentInChildren<MatchObject>(); } }
        public FallingObject Falling { get { return GetComponentInChildren<FallingObject>(); } }
        public TutorialObject Tutorial { get; private set; }
        public OverlayObject Overlay { get; private set; }
        public BlockedObject Blocked { get; private set; }
        public UnderlayObject Underlay { get; private set; }
        public StaticMatchBombObject StaticMatchBomb { get; private set; }
        public DynamicMatchBombObject DynamicMatchBomb { get { if (Match) return Match.GetComponentInChildren<DynamicMatchBombObject>(); return null; } }
        public DynamicClickBombObject DynamicClickBomb { get { return GetComponentInChildren<DynamicClickBombObject>(); } }
        #endregion objects

        #region cache fields
        private BoxCollider2D coll2D;
        private SpriteRenderer sRenderer;
        #endregion cache fields

        #region events
        public Action<GridCell> PointerDownEvent;
        public Action<GridCell> DoubleClickEvent;
        public Action<GridCell> DragEnterEvent;
        #endregion events

        #region properties 
        /// <summary>
        /// Return true if mainobject and mainobject IsMatchedById || IsMatchedWithAny
        /// </summary>
        /// <returns></returns>
        public bool IsMatchable
        {
            get
            {
                if (!Overlay) return Match;
              //  if (Match || DynamicClickBomb) return true;
                  return (Match && !Overlay.BlockMatch);
               // return false;
            }
        }

        public bool IsMixable
        {
            get
            {
                if(Match || DynamicClickBomb) return true ;
                return false;
            }
        }

        public bool IsTopCell { get { return Row == 0; } }

        /// <summary>
        /// Return true if gridcell has no dynamic object
        /// </summary>
        public bool IsDynamicFree
        {
            get { return !DynamicObject; }
        }

        public bool IsDisabled
        {
            get; private set;
        }

        /// <summary>
        /// Return true if mainobject protected with overlay or underlay or self protected
        /// </summary>
        public bool Protected
        {
            get
            {
                if (Overlay && Overlay.Protection > 0) return true;
                if (Underlay && Underlay.Protection > 0) return true;
                return false;
            }
        }

        public bool HasBomb
        {
            get { return (StaticMatchBomb || DynamicMatchBomb || DynamicClickBomb); }
        }

        public bool PhysStep { get; private set; }

        public NeighBors Neighbors { get; private set; }

        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private GameObjectsSet GOSet { get { return MBoard.MatchSet; } }
        private TouchManager Touch { get { return TouchManager.Instance; } }
        #endregion properties 

        #region temp
        private List<GridCellState> undoStates;
        private TweenSeq collectSequence;
        private MatchObject mObjectOld;
        private TweenSeq tS;

        private GameMode gMode;

        public PFCell pfCell;
        [Header("Fill Path")]
        public List<GridCell> fillPath;
        public Spawner spawner;
        #endregion temp

        #region touchbehavior
        public void PointerDown(TouchPadEventArgs tpea)
        {
            if (MatchBoard.GMode == GameMode.Play)
            {
                //  PointerDownEvent?.Invoke(this);
                Touch.SetTarget(null);
                if (IsDraggable()) Touch.SetDraggable(this, (cBack) =>
                {
                    if (Match)
                    {
                        GrabDynamicObject(Match.gameObject, false, cBack);
                        Touch.SetTarget(null);
                        Touch.SetDraggable(null, null);
                    }
                    //////////////////
                    //else if (Overlay.CanDrag)
                    //{
                    //    GrabDynamicObject(Overlay.gameObject, false, cBack);
                    //    Touch.SetTarget(null);
                    //    Touch.SetDraggable(null, null);
                    //}
                    else if (DynamicClickBomb)
                    {
                        GrabDynamicObject(DynamicClickBomb.gameObject, false, cBack);
                        Touch.SetTarget(null);
                        Touch.SetDraggable(null, null);
                    }
                    /////////////////////
                });
                else
                {
                    Touch.SetDraggable(null, null);
                }
            }
            else
            {
              PointerDownEvent?.Invoke(this);
            }
        }

        public void Drag(TouchPadEventArgs tpea)
        {
            if (MatchBoard.GMode == GameMode.Play)
            {
            }
        }

        public void DragBegin(TouchPadEventArgs tpea)
        {
            if (MatchBoard.GMode == GameMode.Play)
            {
            }
        }

        public void DragDrop(TouchPadEventArgs tpea)
        {
            if (MatchBoard.GMode == GameMode.Play)
            {
            }
        }

        public void DragEnter(TouchPadEventArgs tpea)
        {
            if (MatchBoard.GMode == GameMode.Play)
            {
                if (IsDraggable())
                    Touch.SetTarget(this);
                DragEnterEvent?.Invoke(this);
            }
        }

        public bool CanSwap(GridCell gCellOther)
        {
            if (!gCellOther) return false;
            if (!gCellOther.IsDraggable()) return false;
            if (IsDraggable() && gCellOther.DynamicClickBomb) return true; 
            if (IsDraggable() && Neighbors.Contain(gCellOther)) return true;
            return false;
        }

        public void DragExit(TouchPadEventArgs tpea)
        {
            if (MatchBoard.GMode == GameMode.Play)
            {
                Touch.SetTarget(null);
                if (Touch.Draggable)
                {
                    StartCoroutine(WaitTarget(0.1f));
                }
            }
        }

        public void PointerUp(TouchPadEventArgs tpea)
        {
            if (MatchBoard.GMode == GameMode.Play)
            {
                if (IsDraggable())
                    PointerDownEvent?.Invoke(this);
                if (Touch.Draggable)
                {
                    Touch.ResetDrag(null);
                }
                
            }
        }

        private IEnumerator WaitTarget(float Waittime)
        {
            yield return new WaitForSeconds(Waittime);
            if (!Touch.Target && Touch.Draggable)
                Touch.ResetDrag(null);
        }

        public GameObject GetDataIcon()
        {
            return gameObject;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool IsDraggable()
        { 
            if(!Overlay)
            if (Match  || DynamicClickBomb) 
                return true;
            return false;
        }
        #endregion touchbehavior

        #region set mix grab clean
        internal void SetObject(int ID)
        {
            IsDisabled = false;
            MatchObjectData mOD = GOSet.GetMainObject(ID);
            if (mOD != null)
            {
                SetMatchObject(mOD);
                return;
            }

            OverlayObjectData oOD = GOSet.GetOverlayObject(ID);
            if (oOD != null)
            {
                SetOverlay(oOD);
                return;
            }

            UnderlayObjectData uOD = GOSet.GetUnderlayObject(ID);
            if (uOD != null)
            {
                SetUnderlay(uOD);
                return;
            }

            StaticMatchBombObjectData sOD = GOSet.GetStaticMatchBombObject(ID);
            if (sOD != null)
            {
                SetStaticMatchBomb(sOD);
                return;
            }

            DynamicMatchBombObjectData dOD = GOSet.GetDynamicMatchBombObject(ID);
            if (dOD != null)
            {
                SetDynamicMatchBomb(dOD);
                return;
            }

            DynamicClickBombObjectData cdOD = GOSet.GetDynamicClickBombObject(ID);
            if (cdOD != null)
            {
                SetDynamicClickBomb(cdOD);
                return;
            }
            
            if (ID == GOSet.FallingObject.ID)
            {
                SetFalling(GOSet.FallingObject);
                return;
            }

            TutorialObjectData tOD = GOSet.GetTurorialObject(ID);
            if (tOD != null && GameObjectsSet.IsTutorialObject(tOD.ID))
            {
                SetTutorialObject(tOD);
                return;
            }

            BaseObjectData bOD = GOSet.GetObject(ID);
            if (bOD != null && GameObjectsSet.IsDisabledObject(bOD.ID))
            {
                SetDisabledObject(bOD);
                return;
            }

            if (bOD != null && GameObjectsSet.IsBlockedObject(bOD.ID))
            {
                SetBlockedObject(bOD);
            }
        }

        internal void SetTutorialObject(TutorialObjectData tOD)
        {
            if (gMode == GameMode.Play)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (tOD == null || IsDisabled || Blocked) return;
                if (Tutorial)
                {
                    GameObject old = Tutorial.gameObject;
                    Destroy(old);
                }
                Tutorial = TutorialObject.Create(this, tOD);
            }
        }

        internal void SetDisabledObject(BaseObjectData bOD)
        {
            DestroyGridObjects();
            IsDisabled = true;
            if (gMode == GameMode.Play)
            {
                gameObject.SetActive(false);
            }
            else
            {
                sRenderer.sprite = bOD.ObjectImage;
            }
        }

        internal void SetBlockedObject(BaseObjectData bOD)
        {
            if (bOD == null || IsDisabled) { return; }
            DestroyGridObjects();
            Blocked = BlockedObject.Create(this, bOD, MBoard.TargetCollectEventHandler);  // sRenderer.sprite = bOD.ObjectImage;  Blocked = Creator.CreateSprite(transform, bOD.ObjectImage, transform.position, SortingOrder.Blocked).gameObject;
        }

        internal void SetMatchObject(MatchObjectData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) { return; }
            if (DynamicObject)
            {
                GameObject old = DynamicObject;
                DestroyImmediate(old);
            }
            MatchObject.Create(this, mObjectData, false, true, MBoard.TargetCollectEventHandler, MBoard.MatchScoreCollectHandler);
        }

        internal void SetOverlay(OverlayObjectData oData)
        {
            if (oData == null || IsDisabled || Blocked) return;
            if (Overlay)
            {
                GameObject old = Overlay.gameObject;
                Destroy(old);
            }
            Overlay = OverlayObject.Create(this, oData, MBoard.TargetCollectEventHandler);
        }

        internal void SetUnderlay(UnderlayObjectData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) return;
            if (Underlay)
            {
                GameObject old = Underlay.gameObject;
                Destroy(old);
            }
            Underlay = UnderlayObject.Create(this, mObjectData, MBoard.TargetCollectEventHandler);
        }

        internal void SetStaticMatchBomb(StaticMatchBombObjectData oData)
        {
            if (oData == null || IsDisabled || Blocked) return;
            if (StaticMatchBomb)
            {
                GameObject old = StaticMatchBomb.gameObject;
                Destroy(old);
            }
            StaticMatchBomb = StaticMatchBombObject.Create(this, oData, false, false, MBoard.TargetCollectEventHandler);
        }

        internal void SetDynamicMatchBomb(DynamicMatchBombObjectData oData)
        {
            if (oData == null || IsDisabled || Blocked) return;
            if (!Match) return;

            if (DynamicMatchBomb)
            {
                GameObject old = DynamicMatchBomb.gameObject;
                Destroy(old);
            }
            DynamicMatchBombObject.Create(Match, oData, false, false, MBoard.TargetCollectEventHandler);
        }

        internal void SetDynamicClickBomb(DynamicClickBombObjectData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) { return; }
          //  Debug.Log("set dynamic click bomb : " + mObjectData.ID);
            if (DynamicObject)
            {
                GameObject old = DynamicObject.gameObject;
                DestroyImmediate(old);
            }
            // DynamicObject =
            DynamicClickBombObject.Create(this, mObjectData, false, true, MBoard.TargetCollectEventHandler);//.gameObject;
        }

        internal void SetFalling(FallingObjectData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) { return; }
            Debug.Log("set falling: " + mObjectData.ID);
            if (DynamicObject)
            {
                GameObject old = DynamicObject;
                DestroyImmediate(old);
            }
            FallingObject.Create(this, mObjectData, false, true, MBoard.TargetCollectEventHandler);//.gameObject;
        }

        internal void MixJump(Vector3 pos, Action completeCallBack)
        {
            PhysStep = true;
            SimpleTween.Move(DynamicObject, transform.position, pos, 0.5f).AddCompleteCallBack(() =>
            {
                PhysStep = false;
                completeCallBack?.Invoke();
            }).SetEase(EaseAnim.EaseInSine);
        }

        internal void GrabDynamicObject(GameObject dObject, bool fast, Action completeCallBack)
        {
            if (dObject)
            {
                dObject.transform.parent = transform;
                //if (!fast)
                //    MoveTween(dObject, completeCallBack);
                //else
                    FastMoveTween(dObject, completeCallBack);
            }
            else
            {
                completeCallBack?.Invoke();
            }
        }

        /// <summary>
        /// Try to grab match object from fill path
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void FillGrab(Action completeCallBack)
        {
            GameObject mObject = null;
            GridCell gCell = null;

            //if (!MatchBoard.Instance.Spawned)
            //{
            //    completeCallBack?.Invoke();
            //    return;
            //}

            if (spawner) 
            {
                MatchObject mo = spawner.Get();
                mObject =(mo) ? mo.gameObject : null;
            }
            else
            {
                if(fillPath[0] == null)
                {
                    Debug.LogError("-==  Fill Path == (0) ==-");
                    completeCallBack?.Invoke();
                }
                gCell = fillPath[0];
                mObject = gCell.DynamicObject; 
            }
          //  if (mObject && gCell && (gCell.PhysStep)) return;

            GrabDynamicObject(mObject, (MBoard.fillType == FillType.Fast), completeCallBack);
        }

        /// <summary>
        ///  mainObject = null;
        /// </summary>
        public void UnparentDynamicObject()
        {
           if(DynamicObject)  DynamicObject.transform.parent = null;
        }
        #endregion set mix grab

        #region grid objects behavior
        private void FastMoveTween(GameObject mObject, Action completeCallBack)
        {
            PhysStep = true;
            tS = new TweenSeq();
            Vector3 scale = transform.localScale;
            float tweenTime = 0.07f;
            float distK = Vector3.Distance(mObject.transform.position, transform.position) / MatchBoard.MaxDragDistance;

            //move
            tS.Add((callBack) =>
            {
                SimpleTween.Move(mObject, mObject.transform.position, transform.position, tweenTime * distK).AddCompleteCallBack(() =>
                {
                    mObject.transform.position = transform.position;
                    PhysStep = false;
                    completeCallBack?.Invoke();
                    callBack();
                });
            });
            tS.Start();
        }

        private void MoveTween(GameObject mObject, Action completeCallBack)
        {
            PhysStep = true;
            tS = new TweenSeq();
            Vector3 scale = transform.localScale;
            float tweenTime = 0.07f;
            float distK = Vector3.Distance(mObject.transform.position, transform.position) / MatchBoard.MaxDragDistance;
            AnimationCurve scaleCurve = MatchBoard.Instance.arcCurve;

            Vector2 dPos = mObject.transform.position - transform.position;
            bool isVert = (Mathf.Abs(dPos.y) > Mathf.Abs(dPos.x));

            //move
            tS.Add((callBack) =>
            {
                SimpleTween.Move(mObject.gameObject, mObject.gameObject.transform.position, transform.position, tweenTime * distK).AddCompleteCallBack(() =>
                {
                    mObject.transform.position = transform.position;
                    callBack();
                }).SetEase(EaseAnim.EaseInSine);
            });

            //curve deform
            tS.Add((callBack) =>
            {
                SimpleTween.Value(mObject, 0.0f, 1f, 0.1f).SetEase(EaseAnim.EaseInSine).SetOnUpdate((float val) =>
                {
                    float t_scale = 1.0f + scaleCurve.Evaluate(val) * 0.1f;
                    mObject.transform.localScale = (isVert) ? new Vector3(t_scale, 2.0f - t_scale, 1) : new Vector3(2.0f - t_scale, t_scale, 1) ; //  mObject.SetLocalScaleX(t_scale); //  mObject.SetLocalScaleY(2.0f - t_scale);

                }).AddCompleteCallBack(() =>
                {
                    PhysStep = false;
                    completeCallBack?.Invoke();
                    callBack();
                });
            });

            tS.Start();
        }

        /// <summary>
        /// Show simple zoom sequence of main object
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void ZoomMatch(Action completeCallBack)
        {
            if (!Match)
            {
                completeCallBack?.Invoke();
                return;
            }

            Match.Zoom(completeCallBack);
        }

        /// <summary>
        /// Colect match object
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void CollectMatch(float delay, bool showPrefab, bool fly, bool hitProtection, bool sideHitProtection, bool showBombExplode, bool dragExplode, bool showWhirlWind, Action completeCallBack)
        {
            if (Overlay)
            {
                DirectHitOverlay(null, true);
                completeCallBack?.Invoke();
                if(!Overlay.CanSkip) return;
            }     

            if (HasBomb)
            {
                ExplodeBomb(delay, showBombExplode, showWhirlWind, true, dragExplode, true, false, completeCallBack);
                return;
            }

            if (!Match)
            {
                completeCallBack?.Invoke();
                return;
            }

            Match.Collect(this, delay, showPrefab, fly, hitProtection, sideHitProtection, completeCallBack);
  
        }

        /// <summary>
        /// Play explode animation and explode area
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void ExplodeBomb(float delay,
            bool playExplodeAnimation,
            bool whirlWind,
            bool sequenced,
            bool explodeDrag,
            bool showCollectPrefab, 
            bool collectFly,
            Action completeCallBack)
        {
            BombObject bomb = GetBomb();
            if (!bomb)
            {
                completeCallBack?.Invoke();
                return;
            }

            bomb.transform.parent = null;
            if (StaticMatchBomb) StaticMatchBomb = null;


            if (whirlWind)
            {
                //  delay = 0.4f;
                delay = 0.4f;
                switch (bomb.GetBombDir())
                {
                    case BombDir.Vertical:
                        GColumn.CreateNordWind(MBoard.wind.gameObject, transform.lossyScale, MBoard.transform, null);
                        break;
                    case BombDir.Horizontal:
                        GRow.CreateWestWind(MBoard.wind.gameObject, transform.lossyScale, MBoard.transform, null);
                        break;
                    case BombDir.Radial:
                        GColumn.CreateNordWind(MBoard.wind.gameObject, transform.lossyScale, MBoard.transform, null);
                        GRow.CreateWestWind(MBoard.wind.gameObject, transform.lossyScale, MBoard.transform, null);
                        break;
                }
            }
           
            if (playExplodeAnimation)
                bomb.PlayExplodeAnimation(this, delay, () =>
                {
                    bomb.ExplodeArea(this, 0, sequenced, explodeDrag, showCollectPrefab, collectFly, true, completeCallBack);
                });
            else
            {
                bomb.ExplodeArea(this, 0, sequenced, explodeDrag, showCollectPrefab, collectFly, true, completeCallBack);
            }

        }
      
        /// <summary>
        /// Move match to gridcell and collect
        /// </summary>
        /// <param name="bombCell"></param>
        /// <param name="completeCallBack"></param>
        internal void MoveMatchAndCollect(GridCell toCell, float delay, bool showPrefab, bool fly, bool hitProtection, bool showBombExplode, bool dragExplode, bool showWhirlWind, Action completeCallBack)
        {
            if (!Match)
            {
                completeCallBack?.Invoke();
                return;
            }

            if (hitProtection)
            {
                this.DirectHitUnderlay(null);
                this.DirectHitOverlay(null);
                this.Neighbors.Cells.ForEach((GridCell c) => { c.SideHitOverlay(null); });
            }

            MatchObject mo = this.Match;
           // Match.MoveMatchToBomb(this, toCell, delay, hitProtection , ()=> { 
            SimpleTween.Move(mo.gameObject, this.transform.position, toCell.transform.position, 0.15f).AddCompleteCallBack(completeCallBack).SetEase(EaseAnim.EaseInCirc).SetDelay(delay);
                                        //fly
            CollectMatch(0.15f, showPrefab, fly, false, false,  showBombExplode, dragExplode, showWhirlWind, completeCallBack);
        //  });
        }

        /// <summary>
        /// play donuts aroma
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void PlayIddle()
        {
            if (!Match)
            {
                return;
            }
            Creator.InstantiateAnimPrefab(Match.OData.iddleAnimPrefab, Match.transform, Match.transform.position,  SortingOrder.MainIddle, true, null);
        }

        /// <summary>
        /// Side hit neighbourn from collected
        /// </summary>
        /// 
        internal void DirectHitOverlay(Action completeCallBack)
        {
            if (Overlay && !Overlay.BlockMatch)// && !Overlay.SideHit)
            {
                Overlay.Hit(this, completeCallBack);
                return;
            }
            completeCallBack?.Invoke();
        }
        internal void DirectHitOverlay(Action completeCallBack, bool bomb)
        {
            if(Overlay && bomb)
            {
                Overlay.Hit(this, completeCallBack);
                return;
            }
            else if (Overlay && !Overlay.BlockMatch)// && !Overlay.SideHit)
            {
                Overlay.Hit(this,completeCallBack);
                return;
            }
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Side hit neighbourn from collected
        /// </summary>
        internal void SideHitOverlay(Action completeCallBack)
        {////////////////
            if (Overlay && Overlay.SideHit)
            {
                Overlay.Hit(this, completeCallBack);
                return;
            }
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Side hit neighbourn from collected
        /// </summary>
        internal void DirectHitUnderlay(Action completeCallBack)
        {
            if (Underlay)
            {
                Underlay.Hit(this, completeCallBack);
                return;
            }
            completeCallBack?.Invoke();
        }
        #endregion matchobject behavior

        /// <summary>
        ///  used by instancing for cache data
        /// </summary>
        internal void Init(int cellRow, int cellColumn, Column<GridCell> column, Row<GridCell> row, GameMode gMode)
        {
            IsDisabled = false;
            Row = cellRow;
            Column = cellColumn;
            GColumn = column;
            GRow = row;
            this.gMode = gMode;
            undoStates = new List<GridCellState>();
#if UNITY_EDITOR
            name = ToString();
#endif
            sRenderer = GetComponent<SpriteRenderer>();
            sRenderer.sortingOrder = SortingOrder.Base;
            Neighbors = new NeighBors(this, false);
        }

        /// <summary>
        ///  return true if  match objects of two cells are equal
        /// </summary>
        internal bool IsMatchObjectEquals(GridCell other)
        {
            if (other == null) return false; 
            if (Match == null) return false;
            return Match.Equals(other.Match);
        }

        /// <summary>
        ///  cancel any tween on main MainObject object
        /// </summary>
        internal void CancelTween()
        {
         //   Debug.Log("Cancel tween");
            if (DynamicObject)
            {
                SimpleTween.Cancel(DynamicObject, false);
                DynamicObject.transform.localScale = Vector3.one;
                DynamicObject.transform.position = transform.position;
            }
            if (Match)
            {
                Match.CancellTweensAndSequences();
                Match.ResetTween();
            }
            if (mObjectOld)
            {
                mObjectOld.CancellTweensAndSequences();
            }
        }

        /// <summary>
        ///  return true if Gridcell not blocked and not disabled
        /// </summary>
        internal bool CanFill()
        {
            if (IsDisabled || Blocked || DynamicObject) return false;

            return true;
        }

        /// <summary>
        /// Save current grid cell state to undo List
        /// </summary>
        internal void SaveUndoState()
        {
            if (undoStates.Count >= 5)
            {
                undoStates.RemoveAt(0);
            }
            GridCellState s = new GridCellState(this);
            undoStates.Add(s);
        }

        /// <summary>
        /// Return grid cell to previous grid cell state
        /// </summary>
        internal void PreviousUndoState()
        {
            if (undoStates == null || undoStates.Count == 0) return;
            GridCellState gCS = undoStates[undoStates.Count - 1];
            gCS.RestoreState(this);
            undoStates.RemoveAt(undoStates.Count - 1);
        }

        /// <summary>
        /// DestroyImeediate MainObject, OverlayProtector, UnderlayProtector
        /// </summary>
        internal void DestroyGridObjects()
        {
            if (DynamicObject) { DestroyImmediate(DynamicObject); } 
            if (Overlay) { DestroyImmediate(Overlay.gameObject); Overlay = null; }
            if (Underlay) { DestroyImmediate(Underlay.gameObject); Underlay = null; }
            if (StaticMatchBomb) { DestroyImmediate(StaticMatchBomb.gameObject); StaticMatchBomb = null; }
            if(Blocked) { { DestroyImmediate(Blocked.gameObject); Blocked = null; } }
        }

        public BombObject GetBomb()
        {
            if (StaticMatchBomb) return StaticMatchBomb;
            if (DynamicMatchBomb) return DynamicMatchBomb;
            if (DynamicClickBomb) return DynamicClickBomb;
            return null;
        } 

        public override string ToString()
        {
            return "cell : [ row: " + Row + " , col: " + Column + "]";
        }

        public bool HaveObjectWithID(int id)
        {
            if (Match && Match.GetID() == id) return true;
            if (Falling && Falling.GetID() == id) return true;
            if (Overlay && Overlay.GetID() == id) return true;
            if (Underlay && Underlay.GetID() == id) return true;
            if (StaticMatchBomb  && StaticMatchBomb.GetID() == id) return true;
            if (DynamicMatchBomb && DynamicMatchBomb.GetID() == id) return true;
            if (DynamicClickBomb && DynamicClickBomb.GetID() == id) return true;
            return false;
        }

        public void CreateBorder()
        {
            if(Left && LeftBotCorner)
            {
                if (!Neighbors.Left || Neighbors.Left.IsDisabled)
                {
                    SpriteRenderer srL = Creator.CreateSprite(transform, Left, new Vector3(LeftBotCorner.position.x, transform.position.y, transform.position.z), 1);
                    srL.name = "Left border: " + ToString();
                }
            }
            if (Right && RightBotCorner)
            {
                if (!Neighbors.Right || Neighbors.Right.IsDisabled)
                {
                    SpriteRenderer srR = Creator.CreateSprite(transform, Right, new Vector3(RightBotCorner.position.x, transform.position.y, transform.position.z), 1);
                    srR.name = "Right border: " + ToString();
                }
            }
            if (Top && RightTopCorner)
            {
                if (!Neighbors.Top || Neighbors.Top.IsDisabled)
                {
                    SpriteRenderer srT = Creator.CreateSprite(transform, Top, new Vector3(transform.position.x, RightTopCorner.position.y, transform.position.z), 1);
                    srT.name = "Top border: " + ToString();
                }
            }
            if (Bottom && RightBotCorner)
            {
                if (!Neighbors.Bottom || Neighbors.Bottom.IsDisabled)
                {
                    SpriteRenderer srB = Creator.CreateSprite(transform, Bottom, new Vector3(transform.position.x, RightBotCorner.position.y, transform.position.z), 1);
                    srB.name = "Bottom border: " + ToString();
                }
            }

            if(OutTopLeft && LeftTopCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && (!Neighbors.Top || Neighbors.Top.IsDisabled))
                {
                    SpriteRenderer srTL = Creator.CreateSprite(transform, OutTopLeft, LeftTopCorner.position, 1);
                    srTL.name = "OutTopLeft border: " + ToString(); 
                }
            }

            if (OutBotLeft && LeftBotCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && (!Neighbors.Bottom || Neighbors.Bottom.IsDisabled))
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, OutBotLeft, LeftBotCorner.position, 1);
                    sr.name = "OutBotLeft border: " + ToString();
                }
            }

            if (OutBotRight && RightBotCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && (!Neighbors.Bottom || Neighbors.Bottom.IsDisabled))
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, OutBotRight, RightBotCorner.position, 1);
                    sr.name = "OutBotLeft border: " + ToString();
                }
            }
            if (OutTopRight && RightTopCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && (!Neighbors.Top || Neighbors.Top.IsDisabled))
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, OutTopRight, RightTopCorner.position, 1);
                    sr.name = "OutBotLeft border: " + ToString();
                }
            }

            NeighBors n = new NeighBors(this, true);
            if (InTopLeft && LeftTopCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && n.TopLeft && !n.TopLeft.IsDisabled)
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, InTopLeft, LeftTopCorner.position, 2);
                    sr.name = "InTopLeft border: " + ToString();
                }
            }

            if (InBotLeft && LeftBotCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && n.BottomLeft && !n.BottomLeft.IsDisabled)
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, InBotLeft, LeftBotCorner.position, 2);
                    sr.name = "InBotLeft border: " + ToString();
                }
            }

            if (InTopRight && RightTopCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && n.TopRight && !n.TopRight.IsDisabled)
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, InTopRight, RightTopCorner.position, 2);
                    sr.name = "InTopRight border: " + ToString();
                }
            }

            if (InBotRight && RightBotCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && n.BottomRight && !n.BottomRight.IsDisabled)
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, InBotRight, RightBotCorner.position, 2);
                    sr.name = "InBotRight border: " + ToString();
                }
            }

        }
    }

    [Serializable]
    public class GridCellState
    {
        private MatchObjectData mainObjectData;
        private bool hasMainObject;

        private OverlayObjectData overlayObjectData;
        private bool hasOverlay;
        private int overlayProtection;

        private UnderlayObjectData underlayObjectData;
        private bool hasUnderlay;
        private int underlayProtection;

        private StaticMatchBombObjectData bombStaticData;
        private bool hasBombStatic;

        private DynamicMatchBombObjectData bombDynamicData;
        private bool hasBombDynamic;

        private  DynamicClickBombObjectData dynamicCickBombData;
        private bool hasClickBombDynamic;

        public GridCellState(GridCell c)
        {
            hasMainObject = c.Match;
            hasOverlay = c.Overlay;
            hasUnderlay = c.Underlay;
            hasBombStatic = c.StaticMatchBomb;
            hasBombDynamic = c.DynamicMatchBomb;
            hasClickBombDynamic = c.DynamicClickBomb;

            if (hasMainObject)
            {
                mainObjectData = c.Match.OData;
            }

            if (hasOverlay)
            {
                overlayObjectData = c.Overlay.OData;
                overlayProtection = c.Overlay.Protection;
            }

            if (hasUnderlay)
            {
                underlayObjectData = c.Underlay.OData;
                underlayProtection = c.Underlay.Protection;
            }

            if (hasBombStatic)
            {
                //  bombStaticData = c.BombStatic.;
            }
        }

        public void RestoreState(GridCell c)
        {
            c.DestroyGridObjects();

            if (hasMainObject)
            {
                c.SetMatchObject(mainObjectData);
            }

            if (hasOverlay)
            {
                c.SetOverlay(overlayObjectData);
                c.Overlay.SetProtection(overlayProtection);
            }

            if (hasUnderlay)
            {
                c.SetUnderlay(underlayObjectData);
                c.Underlay.SetProtection(underlayProtection);
            }

            if (hasBombStatic)
            {
                //  c.SetMatchData(bombStaticData);
            }
        }

    }
}