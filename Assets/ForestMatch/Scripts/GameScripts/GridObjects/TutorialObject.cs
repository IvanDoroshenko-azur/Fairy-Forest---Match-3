using System;
using UnityEngine;

namespace Mkey
{
    public class TutorialObject : GridObject
    {
        #region properties
        public TutorialObjectData OData { get; private set; }
        #endregion properties

        #region create
        internal virtual void SetData(TutorialObjectData mData)
        {
            OData = mData;
            SetToFront(false);
        }

        /// <summary>
        /// Create new OverlayObject for gridcell
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="oData"></param>
        /// <param name="addCollider"></param>
        /// <param name="radius"></param>
        /// <param name="isTrigger"></param>
        /// <returns></returns>
        public static TutorialObject Create(GridCell parent, TutorialObjectData oData)
        {
            if (!parent || oData == null) return null;
            GameObject gO = null;
            SpriteRenderer sR = null;
            TutorialObject gridObject = null;

            sR = Creator.CreateSprite(parent.transform, oData.ObjectImage, parent.transform.position);
            gO = sR.gameObject;

            gridObject = gO.GetOrAddComponent<TutorialObject>();
#if UNITY_EDITOR
            gO.name = "tutorial " + parent.ToString();
#endif
            gridObject.SetData(oData);
            gridObject.SRenderer = sR;
            return gridObject;
        }
        #endregion create

        #region override
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
                SRenderer.sortingOrder = SortingOrder.OverToFront;
            else
                SRenderer.sortingOrder = SortingOrder.Over;
        }

        public override string ToString()
        {
            return "Overlay: " + GetID();
        }
        #endregion override
    }
}
