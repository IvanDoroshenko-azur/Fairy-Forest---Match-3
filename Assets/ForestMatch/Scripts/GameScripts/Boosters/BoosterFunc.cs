using UnityEngine;
using System;

namespace Mkey
{
    public class BoosterFunc : MonoBehaviour
    {
        protected MatchBoard MBoard { get { return MatchBoard.Instance; } }
        protected MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        protected MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        protected MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }

        protected Action<GameObject, float, Action> delayAction = (g, del, callBack) => { SimpleTween.Value(g, 0, 1, del).AddCompleteCallBack(callBack); };

        public virtual void InitStart ()
        {
            Debug.Log("base init start");
        }

        public virtual void ApplyToGrid(GridCell hitGridCell, BoosterObjectData bData,  Action completeCallBack)
        {
            Debug.Log("base apply to grid booster");
            completeCallBack?.Invoke();
        }

        public virtual bool  ActivateApply(Booster b)
        {
            Debug.Log("base activate apply booster");
            return false;
        }

        public virtual CellsGroup GetArea(GridCell hitGridCell)
        {
            Debug.Log("base get shoot area");
            CellsGroup cG = new CellsGroup();
            return cG;
        }
    }
}