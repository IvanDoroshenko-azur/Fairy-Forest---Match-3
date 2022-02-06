using System;
using UnityEngine;
using System.Collections.Generic;

namespace Mkey
{
    public class DynamicClickBombObject : BombObject
    {
        private GameObjectsSet GOSet { get { return MBoard.MatchSet; } }

        #region properties
        public DynamicClickBombObjectData OData { get; private set; }
        #endregion properties

        #region events
        private Action<int> TargetCollectEvent;
        #endregion events

        public bool CanDrag
        {
            get { return OData.canDrag; }
        }

        private int radius = 2;

        private int randMatchID = 0;

        #region create
        internal virtual void SetData(DynamicClickBombObjectData oData)
        {
            SRenderer = GetComponent<SpriteRenderer>();
            if (SRenderer) SRenderer.sprite = (oData != null) ? oData.ObjectImage : null;
            OData = oData;
#if UNITY_EDITOR
            gameObject.name = (oData != null) ? "DynamicClickBomb: " + GetID() + "(" + SRenderer.sprite.name + ")" : "none";
#endif
            SetToFront(false);
        }

        /// <summary>
        /// Create new MainObject for gridcell
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="oData"></param>
        /// <param name="addCollider"></param>
        /// <param name="radius"></param>
        /// <param name="isTrigger"></param>
        /// <returns></returns>
        
        public static DynamicClickBombObject Create(GridCell parent, DynamicClickBombObjectData oData, bool addCollider, bool isTrigger, Action<int> TargetCollectEvent)
        {
            if (!parent || oData == null) return null;
            SpriteRenderer sR = Creator.CreateSprite(parent.transform, oData.ObjectImage, parent.transform.position);
            GameObject gO = sR.gameObject;
            DynamicClickBombObject gridObject = gO.AddComponent<DynamicClickBombObject>();

            

            if (addCollider)
            {
                BoxCollider2D cC = gridObject.gameObject.GetOrAddComponent<BoxCollider2D>();
                cC.isTrigger = isTrigger;
            }

            if (oData.iddleAnimPrefab)
                Creator.InstantiatePrefab(oData.iddleAnimPrefab, gridObject.transform, gridObject.transform.position, 0, SortingOrder.DynamicMatchBombIddleAnim);

            gridObject.SRenderer = sR;
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.SetData(oData);
            return gridObject;
        }
        #endregion create

        #region override
        internal override void PlayExplodeAnimation (GridCell gCell, float delay,  Action completeCallBack)
        {
            if (!gCell || OData == null) completeCallBack?.Invoke();

            Row<GridCell> r = gCell.GRow;
            Column<GridCell> c = gCell.GColumn;
            Debug.Log(gCell);

            TweenSeq anim = new TweenSeq();
            GameObject g = null;
            GameObject g1 = null;

            List<GridCell> area; 
            CellsGroup cG = new CellsGroup();
            ParallelTween par0 = new ParallelTween();

          //  delay += 0.05f;

            if (delay > 0)
            {
                anim.Add((callBack) => {
                    SimpleTween.Value(gameObject, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }

            anim.Add((callBack) =>
            {
                MSound.PlayClip(0, OData.privateClip);
                callBack();
            });

            anim.Add((callBack) =>
            {
                MSound.PlayClip(0, OData.privateClip);
                callBack();
            });

            if (OData.bombType == BombDir.Horizontal)
            {
                anim.Add((callBack) =>
                {
                    int rowCount = r.GetRightDynamic().Column - r.GetLeftDynamic().Column;
                    Vector3 pos = r.GetDynamicCenter();
                    float scale = (rowCount > 0) ? rowCount / 6.0f : 1f;
                    g = Creator.InstantiateAnimPrefab(OData.explodeAnimPrefab, transform, pos, SortingOrder.MainExplode, false, callBack);
                    if (g) g.transform.localScale = new Vector3(g.transform.localScale.x * scale, g.transform.localScale.y, 1);
                });
            }

            else if (OData.bombType == BombDir.Vertical)
            {
                anim.Add((callBack) =>
                {
                    int colCount = c.GetBottomDynamic().Row - c.GetTopDynamic().Row;
                    Vector3 pos = c.GetDynamicCenter();
                    float scale = (colCount > 0) ? colCount / 6.0f : 1f;
                    g = Creator.InstantiateAnimPrefab(OData.explodeAnimPrefab, gCell.transform, pos, SortingOrder.MainExplode, false, callBack);
                    if (g)
                    {
                        g.transform.localScale = new Vector3(g.transform.localScale.x * scale, g.transform.localScale.y, 1);
                        g.transform.eulerAngles = new Vector3(0, 0, 90);
                    }
                });
            }

            else if (OData.bombType == BombDir.Radial)
            {
                anim.Add((callBack) =>
                {
                    int rowCount = r.GetRightDynamic().Column - r.GetLeftDynamic().Column;
                    Vector3 pos = r.GetDynamicCenter();
                    float scale = (rowCount > 0) ? rowCount / 6.0f : 1f;
                    g = Creator.InstantiateAnimPrefab(OData.explodeAnimPrefab, gCell.transform, pos, SortingOrder.MainExplode, false, null);
                    if (g) g.transform.localScale = new Vector3(g.transform.localScale.x * scale, g.transform.localScale.y, 1);

                    int colCount = c.GetBottomDynamic().Row - c.GetTopDynamic().Row;
                    pos = c.GetDynamicCenter();
                    scale = (colCount > 0) ? colCount / 6.0f : 1f;
                    g1 = Creator.InstantiateAnimPrefab(OData.explodeAnimPrefab, gCell.transform, pos, SortingOrder.MainExplode, false, callBack);
                    if (g1)
                    {
                        g1.transform.localScale = new Vector3(g1.transform.localScale.x * scale, g1.transform.localScale.y, 1);
                        g1.transform.eulerAngles = new Vector3(0, 0, 90);
                    }
                });
            }

            else if (OData.bombType == BombDir.Dinamit)
            {
                anim.Add((callBack) =>
                {
                    area = GetExplodeArea(gCell).Cells;

                    //apply effect for each cell parallel
                    int i = 0;
                    foreach (var cl in area)
                    {
                        delay += 0.1f;
                          if(i == 0)
                            Creator.InstantiateAnimPrefab(OData.explodeAnimPrefab, cl.transform, cl.transform.position, SortingOrder.Booster + 1, true, callBack);
                          else
                            Creator.InstantiateAnimPrefab(OData.explodeAnimPrefab, cl.transform, cl.transform.position, SortingOrder.Booster + 1, true, null);
                        i++;
                    }
                });
            }

            else if (OData.bombType == BombDir.Color)
            {
                anim.Add((callBack) =>
                {
                    randMatchID = (GOSet.GetMainRandomObjects(1))[0].ID;
                    area = GetExplodeArea(gCell).Cells;

                    //apply effect for each cell parallel
                   delay = 0.0f;
                    int i = 0;
                    foreach (var cl in area)
                    {
                        delay += 0.1f;
                
                        if (i == 0)
                            Creator.InstantiateAnimPrefab(OData.explodeAnimPrefab, cl.transform, cl.transform.position, SortingOrder.Booster + 1, true, callBack);
                        else
                            Creator.InstantiateAnimPrefab(OData.explodeAnimPrefab, cl.transform, cl.transform.position, SortingOrder.Booster + 1, true, null);
                        i++;
                    }
                });
            }

            anim.Add((callBack) =>
            {
                if (g) Destroy(g);
                if (g1) Destroy(g1);
                TargetCollectEvent?.Invoke(GetID());
                completeCallBack?.Invoke();
                callBack();
            });

            anim.Start();
        }

        public override CellsGroup GetExplodeArea(GridCell gCell)
        {
            List<GridCell> area = MBoard.grid.GetAroundArea(gCell, radius).Cells;
            CellsGroup cG = new CellsGroup();
            Debug.Log(gCell);
            GameObjectsSet goSet;
            if (!gCell) return cG;
            switch (OData.bombType)
            {
                case BombDir.Vertical:
                    cG.AddRange(gCell.GColumn.GetDynamicArea());
                    break;
                case BombDir.Horizontal:
                    cG.AddRange(gCell.GRow.GetDynamicArea());
                    break;
                case BombDir.Radial:
                    cG.AddRange(gCell.GColumn.GetDynamicArea());
                    cG.AddRange(gCell.GRow.GetDynamicArea());
                    break;
                case BombDir.Dinamit:
                    area = MBoard.grid.GetAroundArea(gCell, radius).Cells;
                    cG.Add(gCell);
                    foreach (var item in area)
                         if(!item.IsDisabled && !item.Blocked) 
                            cG.Add(item);
                    break;
                case BombDir.Color:
                    area = MBoard.grid.GetAllByID(randMatchID);
                    cG.Add(gCell);
                    foreach (var item in area)
                        if (item.IsMatchable) cG.Add(item);
                    break;
            }
            return cG;
        }


        private int GetIdNearBorn(GridCell gc)
        {
            if(gc.Row > 0)
            if (gc.GColumn[gc.Row - 1].Match != null)
         return gc.GColumn[gc.Row - 1].Match.GetID();

            if (gc.Column < gc.GRow.Length)
                if (gc.GRow[gc.Column + 1].Match != null)
         return gc.GRow[gc.Column + 1].Match.GetID();

            if (gc.Row < gc.GColumn.Length)
                if (gc.GColumn[gc.Row + 1].Match != null)
          return gc.GColumn[gc.Row + 1].Match.GetID();

            if (gc.Column > 0)
                if (gc.GRow[gc.Column - 1].Match != null)
         return gc.GRow[gc.Column - 1].Match.GetID();

            return 0;
        }
        public override void ExplodeArea(GridCell gCell, float delay, bool sequenced, bool explodeDrag, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            Destroy(gameObject);
            ParallelTween pt = new ParallelTween();
            TweenSeq expl = new TweenSeq();
            List<GridCell> area;

            bool sideHitProtection = false;
            if (OData.bombType == BombDir.Color)
                sideHitProtection = true;

            if (delay > 0)
            {
                expl.Add((callBack) => {
                    SimpleTween.Value(gameObject, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }

            float incDelay = 0;
            area = GetExplodeArea(gCell).Cells;

            foreach (GridCell mc in area) //parallel explode all cells
            {
               // if (sequenced) 
                    incDelay += 0.05f;
                float t = incDelay;
                pt.Add((callBack) => { ExplodeCell(mc, t, explodeDrag, showPrefab, fly, hitProtection, sideHitProtection, callBack); });
            }

            expl.Add((callBack) => { pt.Start(callBack); });
            expl.Add((callBack) => { completeCallBack?.Invoke(); callBack(); });

            expl.Start();
        }

        public override int GetID()
        {
            return (OData != null) ? OData.ID : Int32.MinValue;
        }

        public override void CancellTweensAndSequences()
        {
            base.CancellTweensAndSequences();
        }

        public override void SetToFront(bool set)
        {
            if (!SRenderer) SRenderer = GetComponent<SpriteRenderer>();
            if (set)
                SRenderer.sortingOrder = SortingOrder.DynamicMatchBombToFront;
            else
                SRenderer.sortingOrder = SortingOrder.DynamicMatchBomb;
        }

        public override string ToString()
        {
            return "DynamicClickBomb: " + GetID();
        }

        internal void InstantiateScoreFlyerAtPosition(GameObject scoreFlyerPrefab, int score, Vector3 position)
        {
            if (!scoreFlyerPrefab) return;
            GameObject flyer = Instantiate(scoreFlyerPrefab);
            ScoreFlyer sF = flyer.GetComponent<ScoreFlyer>();
            sF.StartFly(score.ToString(), position);
            flyer.transform.localScale = transform.lossyScale;
        }

        public override BombDir GetBombDir()
        {
            return OData.bombType;
        }
        #endregion override
    }
}