using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Mkey
{
    public enum NumAnimation : int { toLeft = 0, toUP, toRight, none };

    [CreateAssetMenu]
    public class LevelConstructSet : BaseScriptable
    {
        //[SerializeField]
        //public Sprite imageTutorial;
        //[SerializeField]
        //public NumAnimation numAnimation;
        
        [TextArea(3, 10)] [SerializeField] public string textTutorial = String.Empty;

        [SerializeField]
        public int[] OffMatch_ID = new int[7];

        [SerializeField]
        private int vertSize = 8;
        [SerializeField]
        private int horSize = 8;
        [SerializeField]
        private float distX = 0.0f;
        [SerializeField]
        private float distY = 0.0f;

        [SerializeField]
        private float scale = 0.9f;
        [SerializeField]
        private int backGroundNumber = 0;

        
        public int BackGround
        {
            get { return backGroundNumber; }

        }

        public MissionConstruct levelMission;

        public int VertSize
        {
            get { return vertSize; }
            set
            {
                if (value < 1) value = 1;
                vertSize = value;
                SetAsDirty();
            }
        }

        public int HorSize
        {
            get { return horSize; }
            set
            {
                if (value < 1) value = 1;
                horSize = value;
                SetAsDirty();
            }
        }

        public float DistX
        {
            get { return distX; }
            set
            {
                distX = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        public float DistY
        {
            get { return distY; }
            set
            {
                distY = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                if (value < 0) value = 0;
                scale = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        [SerializeField]
        public List<Vector2> tutorialPos;
        [SerializeField]
        public List<CellData> tutorialCells;
        [SerializeField]
        public List<CellData> matchedCells;
        [SerializeField]
        public List<CellData> bombsCells;
        [SerializeField]
        public List<CellData> blockedCells;
        [SerializeField]
        public List<CellData> disabledCells;
        [HideInInspector]
        [SerializeField]
        public List<CellData> featuredCells;
        [SerializeField]
        public List<CellData> fallingCells;
        [SerializeField]
        public List<CellData> overlayCells;
        [SerializeField]
        public List<CellData> underlayCells;

        #region regular
        void OnEnable()
        {
            // Debug.Log("onenable " + ToString());
            if (levelMission == null) levelMission = new MissionConstruct();
            levelMission.SaveEvent = SetAsDirty;

        }

        void Awake()
        {
            // Debug.Log("awake " + ToString());
            //if (levelMission == null) levelMission = new MissionConstruct();
            //levelMission.SaveEvent = SetAsDirty;

        }
        #endregion regular

        public void AddTutorialCell(Vector2 pos, CellData cd)
        {
            if (blockedCells == null) blockedCells = new List<CellData>();
            tutorialPos.Add(pos);
            tutorialCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveTutorialCell(Vector2 pos, CellData cd)
        {
            if (blockedCells == null) blockedCells = new List<CellData>();
            if (tutorialPos != null) tutorialPos.RemoveAll((c) => { return ((pos.x == c.x) && (pos.y == c.y)); });
            RemoveCellData(tutorialCells, cd);
            SetAsDirty();
        }

        public void AddFeaturedCell(CellData cd)
        {
            if (featuredCells == null) featuredCells = new List<CellData>();
            RemoveCellData(featuredCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            RemoveCellData(fallingCells, cd);
            featuredCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveFeaturedCell(CellData cd)
        {
            if (featuredCells == null) featuredCells = new List<CellData>();
            RemoveCellData(featuredCells, cd);
            SetAsDirty();
        }

        public void AddFallingCell(CellData cd)
        {
            if (fallingCells == null) fallingCells = new List<CellData>();
            RemoveCellData(featuredCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            RemoveCellData(fallingCells, cd);
            fallingCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveFallingCell(CellData cd)
        {
            if (fallingCells == null) fallingCells = new List<CellData>();
            RemoveCellData(fallingCells, cd);
            SetAsDirty();
        }

        public void AddDisabledCell(CellData cd)
        {
            if (disabledCells == null) disabledCells = new List<CellData>();
            RemoveCellData(cd);
            disabledCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveDisabledCell(CellData cd)
        {
            if (disabledCells == null) disabledCells = new List<CellData>();
            RemoveCellData(disabledCells, cd);
            SetAsDirty();
        }


        public void AddMatchedCell(CellData cd)
        {
            if (matchedCells == null) matchedCells = new List<CellData>();
            RemoveCellData(matchedCells, cd);
            matchedCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveMatchedCell(CellData cd)
        {
            if (matchedCells == null) matchedCells = new List<CellData>();
            RemoveCellData(matchedCells, cd);
            SetAsDirty();
        }

        public void AddBombsCell(CellData cd)
        {
            if (bombsCells == null) bombsCells = new List<CellData>();
            RemoveCellData(bombsCells, cd);
            bombsCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveBombsCell(CellData cd)
        {
            if (bombsCells == null) bombsCells = new List<CellData>();
            RemoveCellData(bombsCells, cd);
            SetAsDirty();
        }

        public void AddBlockedCell(CellData cd)
        {
            if (blockedCells == null) blockedCells = new List<CellData>();
            RemoveCellData(cd);
            blockedCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveBlockedCell(CellData cd)
        {
            if (blockedCells == null) blockedCells = new List<CellData>();
            RemoveCellData(blockedCells, cd);
            SetAsDirty();
        }

        public void AddOverlayCell(CellData cd)
        {
            if (overlayCells == null) overlayCells = new List<CellData>();
            RemoveCellData(overlayCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            overlayCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveOverlayCell(CellData cd)
        {
            if (overlayCells == null) overlayCells = new List<CellData>();
            RemoveCellData(overlayCells, cd);
            SetAsDirty();
        }

        public void AddUnderlayCell(CellData cd)
        {
            if (underlayCells == null) underlayCells = new List<CellData>();
            RemoveCellData(underlayCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            underlayCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveUnderlayCell(CellData cd)
        {
            if (underlayCells == null) underlayCells = new List<CellData>();
            RemoveCellData(underlayCells, cd);
            SetAsDirty();
        }

        /// <summary>
        /// Remove all non-existent cells data from board
        /// </summary>
        /// <param name="gOS"></param>
        public void Clean(GameObjectsSet gOS)
        {
            Action<List<CellData>> cAction = (arr) => 
            {
                if (arr != null)
                {
                    arr.RemoveAll((c) =>
                    {
                        return ((c.Column >= horSize) && (c.Row >= vertSize));
                    });

                    if(gOS)
                    arr.RemoveAll((c) =>
                    {
                        return (!gOS.ContainID(c.ID));
                    });
                }
            };
            cAction(matchedCells);
            cAction(featuredCells);
            cAction(blockedCells);
            cAction(overlayCells);
            cAction(disabledCells);
            cAction(underlayCells);
            SetAsDirty();
        }

        public void IncBackGround()
        {
            backGroundNumber++;
            //  if (backGroundNumber >= mSet.BackGroundsCount || backGroundNumber < 0) backGroundNumber = 0;
            Save();
        }

        public void DecBackGround()
        {
            backGroundNumber--;
            //   if (backGroundNumber >= mSet.BackGroundsCount) backGroundNumber = 0;
            //   else if (backGroundNumber < 0) backGroundNumber = mSet.BackGroundsCount - 1;
            Save();
        }

        private float RoundToFloat(float val, float delta)
        {
            int vi = Mathf.RoundToInt(val / delta);
            return (float)vi * delta;
        }

        private void RemoveCellData(List<CellData> cdl, CellData cd)
        {
            if (cdl != null) cdl.RemoveAll((c) => { return ((cd.Column == c.Column) && (cd.Row == c.Row)); });
        }

     

        /// <summary>
        /// Remove celldata overlay -> disabled
        /// </summary>
        /// <param name="cd"></param>
        public void RemoveCellData(CellData cd)
        {
            RemoveCellData(overlayCells, cd);
            RemoveCellData(featuredCells, cd);
            RemoveCellData(underlayCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            RemoveCellData(fallingCells, cd);
        }

        private bool ContainCellData(List<CellData> lcd, CellData cd)
        {
            if (lcd == null || cd == null) return false;
            foreach (var item in lcd)
            {
                if ((item.Row == cd.Row) && (item.Column == cd.Column)) return true; 
            }
            return false;
        }
    }
}



