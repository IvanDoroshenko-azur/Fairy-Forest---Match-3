using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class BombObject : GridObject
    {
        public static void ExplodeCell(GridCell gCell, float delay, bool explodeDrag, bool showPrefab, bool fly, bool hitProtection, bool sideHitProtection, Action completeCallBack)
        {
            if (gCell.GetBomb())
            {                                       //true
                gCell.ExplodeBomb(delay, true, true, true, true, true, false, completeCallBack);
                return;
            }

            if (gCell.Overlay && !sideHitProtection) // only hit overlay
            {
                gCell.DirectHitOverlay(null, true);
                completeCallBack?.Invoke();
                return;
            }

            if (gCell.Underlay && !gCell.Match) // only hit underlay
            {
                gCell.DirectHitUnderlay(null);
                completeCallBack?.Invoke();
                return;
            }

            if (!gCell.Match)
            {
                completeCallBack?.Invoke();
                return;
            }

            fly = false; hitProtection = true; delay = 0;///////////////////////////////
             gCell.Match.Explode(gCell, explodeDrag, showPrefab, fly, hitProtection, sideHitProtection,  delay, completeCallBack);
        }

        public static void ExplodeArea(IEnumerable<GridCell> area, float delay, bool sequenced, bool explodeDrag, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            ParallelTween pt = new ParallelTween();
            TweenSeq expl = new TweenSeq();
            GameObject temp = new GameObject();
            if (delay > 0)
            {
                expl.Add((callBack) => {
                    SimpleTween.Value(temp, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }
            float incDelay = 0;
            foreach (GridCell mc in area) //parallel explode all cells
            {
                if (sequenced) incDelay += 0.05f;
                float t = incDelay;
                pt.Add((callBack) => { ExplodeCell(mc, t, explodeDrag, showPrefab, fly, hitProtection, hitProtection, callBack); });
            }

            expl.Add((callBack) => { pt.Start(callBack); });
            expl.Add((callBack) =>
            {
                Destroy(temp);
                completeCallBack?.Invoke(); callBack();
            });

            expl.Start();
        }

        #region virtual
        internal virtual void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }

        public virtual CellsGroup GetExplodeArea(GridCell gCell)
        {
            return new CellsGroup();
        }

        public virtual void ExplodeArea(GridCell gCell, float delay, bool sequenced, bool explodeDrag, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }
       
        public virtual BombDir GetBombDir()
        {
            return BombDir.Horizontal;
        }
        #endregion virtual
    }
}