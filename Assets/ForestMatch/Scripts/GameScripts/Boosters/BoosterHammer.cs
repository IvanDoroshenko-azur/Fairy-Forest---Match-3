using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class BoosterHammer: BoosterFunc
    {
        [SerializeField]
        private float speed = 20f;

        #region override
        public override void InitStart ()
        {
            
        }

        public override void ApplyToGrid(GridCell gCell, BoosterObjectData bData, Action completeCallBack)
        {
            if (!gCell.Match)
            {
                Booster.ActiveBooster.DeActivateBooster();
                completeCallBack?.Invoke();
                return;
            }

            Booster b = Booster.ActiveBooster;
            b.AddCount(-1);

            ParallelTween par0 = new ParallelTween();
            TweenSeq bTS = new TweenSeq();
            CellsGroup area = GetArea(gCell);

            //move activeBooster
            Vector3 pos = transform.position;
            float dist = Vector3.Distance(transform.position, gCell.transform.position);
            Vector3 rotPivot = Vector3.zero;
            float rotRad = 6f;
            bTS.Add((callBack) =>
            {
                SimpleTween.Move(b.SceneObject, b.SceneObject.transform.position, gCell.transform.position, dist / speed).AddCompleteCallBack(() =>
                {
                    rotPivot = transform.position - new Vector3(0, rotRad, 0);
                    callBack();
                }).SetEase(EaseAnim.EaseInSine);
            });


            // back move
            bTS.Add((callBack) =>
            {
                SimpleTween.Value(gameObject, Mathf.Deg2Rad * 90f, Mathf.Deg2Rad * 180f, 0.25f).SetEase(EaseAnim.EaseInCubic). //
                 SetOnUpdate((float val) => { transform.position = new Vector3(rotRad * Mathf.Cos(val), rotRad * Mathf.Sin(val), 0) + rotPivot; }).
                 AddCompleteCallBack(() => { callBack(); });
            });
            //forward move
            bTS.Add((callBack) =>
            {

                SimpleTween.Value(gameObject, Mathf.Deg2Rad * 180f, Mathf.Deg2Rad * 100f, 0.2f).SetEase(EaseAnim.EaseOutBounce).
                    SetOnUpdate((float val) =>
                    {
                        transform.position = new Vector3(rotRad * Mathf.Cos(val), rotRad * Mathf.Sin(val), 0) + rotPivot;
                    }).
                    AddCompleteCallBack(() =>
                    {
                        MSound.PlayClip(0, bData.privateClip);
                        Destroy(gameObject, 0.25f);
                        Creator.InstantiateAnimPrefab(bData.animPrefab, gCell.transform, gCell.transform.position, SortingOrder.Booster + 1, true, callBack);
                    });

            });

            if (gCell.IsMatchable)
            {
                bTS.Add((callBack) =>
                {
                    gCell.CollectMatch(0, true, false, true, true, MBoard.showBombExplode, MBoard.dragExplode, MBoard.showWhirlWind, callBack);
                });
            }
            else
            {
                bTS.Add((callBack) =>
                {
                    gCell.SideHitOverlay(null);
                    callBack();
                });
            }

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
            List<GridCell> area = new NeighBors(hitGridCell, true).Cells;
            cG.Add(hitGridCell);
            foreach (var item in area)
            {
              if(hitGridCell.IsMatchObjectEquals(item) && item.IsMatchable) cG.Add(item);
            }

            return cG;
        }
        #endregion override
    }
}


///// <summary>
///// Aplly active hammer booster to gridcell
///// </summary>
///// <param name="gCell"></param>
//private bool ApplyBoosterHammerHandler(GridCell gCell)
//{
//    SaveUndoState();
//    gCell.ApplyHammerBooster(ActiveBooster, () =>
//        {
//            ActiveBooster = null;
//            MbState = MatchBoardState.Fill;
//        });
//    return true;
//}


///// <summary>
///// Apply Hammer to gridcell
///// </summary>
///// <param name="booster"></param>
///// <param name="completeCallBack"></param>
//internal void ApplyHammerBooster(Booster booster, Action completeCallBack)
//{
//    if (!Match)
//    {
//        completeCallBack?.Invoke();
//        return;
//    }

//    bool fullProt = Protected;
//    mObjectOld = Match;
//    UnparentDynamicObject();

//    TweenSeq bTS = new TweenSeq();
//    Vector3 rotPivot = Vector3.zero;
//    float rotRad = 6f;
//    booster.SceneObject.AddComponent<SpriteTrail>();
//    Vector3 pos = transform.position;

//    bTS.Add((callBack) =>
//    {
//        SimpleTween.Move(booster.SceneObject, booster.SceneObject.transform.position, transform.position, 1.0f).AddCompleteCallBack(() =>
//        {
//            rotPivot = booster.SceneObject.transform.position - new Vector3(0, rotRad, 0);
//            callBack();
//        }).SetEase(EaseAnim.EaseInSine);
//    });

//    bTS.Add((callBack) =>
//    {
//        SimpleTween.Value(booster.SceneObject, Mathf.Deg2Rad * 90f, Mathf.Deg2Rad * 180f, 0.25f).SetEase(EaseAnim.EaseInCubic). //
//         SetOnUpdate((float val) => { booster.SceneObject.transform.position = new Vector3(rotRad * Mathf.Cos(val), rotRad * Mathf.Sin(val), 0) + rotPivot; }).
//         AddCompleteCallBack(() => { callBack(); });
//    });

//    bTS.Add((callBack) =>
//    {

//        SimpleTween.Value(booster.SceneObject, Mathf.Deg2Rad * 180f, Mathf.Deg2Rad * 100f, 0.2f).SetEase(EaseAnim.EaseOutBounce).
//            SetOnUpdate((float val) =>
//            {
//                booster.SceneObject.transform.position = new Vector3(rotRad * Mathf.Cos(val), rotRad * Mathf.Sin(val), 0) + rotPivot;
//            }).
//            AddCompleteCallBack(() =>
//            {
//                DirectHitOverlay(null);
//                //  if (!fullProt) EffectsHolder.Instance.InstantiateAnimPrefabAtPositionWithCallBack(booster.mData.explodeAnimPrefab, transform, mObjectOld.transform.position, new Vector3(1, 1, 1), sortingLayer, SortingOrder.sortingOrderBoosterExplode, null, true);
//                callBack();
//            });

//    });

//    if (!fullProt)// hit main object
//    {
//        bTS.Add((callBack) =>
//        {
//            SimpleTween.Move(mObjectOld.gameObject, pos, MatchBoard.Instance.FlyTarget, .50f).SetEase(EaseAnim.EaseLinear).
//            AddCompleteCallBack(() =>
//            {
//                DestroyImmediate(mObjectOld.gameObject);
//                //TargetCollectEvent(mObjectOld.OData.ID);
//                completeCallBack();
//                callBack();
//            });
//        });
//    }
//    else
//    {
//        bTS.Add((callBack) =>
//        {
//            completeCallBack?.Invoke();
//            //   Match = mObjectOld;
//            callBack();
//        });
//    }
//    bTS.Start();
//}
