using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class BoosterWand: BoosterFunc
    {
        [SerializeField]
        private float speed = 20f;

        #region override
        public override void InitStart ()
        {
            
        }

        public override void ApplyToGrid(GridCell gCell, BoosterObjectData bData, Action completeCallBack)
        {
            if (!gCell.IsMatchable)
            {
                Booster.ActiveBooster.DeActivateBooster();
                completeCallBack?.Invoke();
                return;
            }

            Booster b = Booster.ActiveBooster;
            b.AddCount(-1);

            ParallelTween par0 = new ParallelTween();
            ParallelTween par1 = new ParallelTween();
            TweenSeq bTS = new TweenSeq();
            CellsGroup area = GetArea(gCell);

            float dist = Vector3.Distance(transform.position, gCell.transform.position);
            List<GameObject> dupBoost = new List<GameObject>();
            dupBoost.Add(b.SceneObject);

            //move activeBooster
            bTS.Add((callBack) =>
            {
                SimpleTween.Move(gameObject, transform.position, gCell.transform.position, dist / speed).AddCompleteCallBack(() =>
                {
                    MSound.PlayClip(0, bData.privateClip);
                    callBack();
                }).SetEase(EaseAnim.EaseInSine);
            });

            // duplicate and move
            foreach (var c in area.Cells)
            {
                if (c != gCell)
                    par0.Add((callBack) =>
                    {
                        GameObject boost = b.DuplicateGameobjet();
                       
                        dupBoost.Add(boost);
                        SimpleTween.Move(boost.gameObject, gCell.transform.position, c.transform.position, 0.25f).AddCompleteCallBack(() =>
                        {
                            callBack();
                        }).SetEase(EaseAnim.EaseInSine);
                    });
            }
            
            //apply effect for each cell parallel
            float delay = 0.0f;
            foreach (var c in area.Cells)
            {
                delay += 0.15f;
                float d = delay;
                par1.Add((callBack) =>
                {
                    delayAction(gameObject, d,
                        () =>
                        {
                            Creator.InstantiateAnimPrefab(bData.animPrefab, c.transform, c.transform.position, SortingOrder.Booster + 1, true, callBack);
                        }
                        );

                });
            }
           
            // destroy boosters
            foreach (var db in dupBoost)
            {
                delay += 0.15f;
                float d = delay;
                par1.Add((callBack) =>
                {
                    delayAction(gameObject, d,
                        () =>
                        {
                            Destroy(db);
                        }
                        );
                    callBack();
                });
            }

            // collect match objects
            delay = 0.15f;
            foreach (var c in area.Cells)
            {
                delay += 0.15f;
                float d = delay;
                par1.Add((callBack) =>
                {
                    c.CollectMatch(d, true, false, true, true, MBoard.showBombExplode, MBoard.dragExplode, MBoard.showWhirlWind, callBack);
                });
            }

            bTS.Add((callback) =>
            {
                par0.Start(() =>
                {
                    callback();
                });
            });

            bTS.Add((callback) =>
            {
                par1.Start(() =>
                {
                    callback();
                });
            });

            bTS.Add((callback) =>
            {
                Booster.ActiveBooster.DeActivateBooster();
                completeCallBack?.Invoke();
                callback();
            });

            bTS.Start();
        }

        public override CellsGroup GetArea(GridCell hitGridCell)
        {
            CellsGroup cG = new CellsGroup();
            List<GridCell> area = MBoard.grid.GetAllByID(hitGridCell.Match.GetID());
            cG.Add(hitGridCell);
            foreach (var item in area)
            {
                if (hitGridCell.IsMatchObjectEquals(item) && item.IsMatchable) cG.Add(item);
            }

            return cG;
        }
        #endregion override
    }
}


