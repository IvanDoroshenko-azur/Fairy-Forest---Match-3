using System;
using System.Collections.Generic;

namespace Mkey
{
    [Serializable]
    /// <summary>
    /// Get neighborns for gridcell 
    /// </summary>
    public class NeighBors
    {
        public GridCell Main { get; private set; }
        public GridCell Left { get; private set; }
        public GridCell Right { get; private set; }
        public GridCell Top { get; private set; }
        public GridCell Bottom { get; private set; }

        public GridCell TopLeft { get; private set; }
        public GridCell BottomLeft { get; private set; }
        public GridCell TopRight { get; private set; }
        public GridCell BottomRight { get; private set; }

        public List<GridCell> Cells { get; private set; }

        /// <summary>
        /// Create NeighBorns  cells
        /// </summary>
        /// <param name="main"></param>
        /// <param name="id"></param>
        public NeighBors(GridCell main, bool addDiag)
        {
            Main = main;
            Left = main.GRow[main.Column - 1];
            Right = main.GRow[main.Column + 1];
            Top = main.GColumn[main.Row - 1];
            Bottom = main.GColumn[main.Row + 1];

            Cells = new List<GridCell>();
            if (Top) Cells.Add(Top);
            if (Bottom) Cells.Add(Bottom);
            if (Left) Cells.Add(Left);
            if (Right) Cells.Add(Right);

            if (addDiag)
            {
                TopLeft = (Top) ? Top.GRow[Top.Column - 1] : null;
                BottomLeft = (Bottom) ? Bottom.GRow[Bottom.Column - 1] : null; 
                TopRight = (Top) ? Top.GRow[Top.Column + 1] : null;
                BottomRight = (Bottom) ? Bottom.GRow[Bottom.Column + 1] : null;

                Cells = new List<GridCell>();
                if (Top) Cells.Add(Top);
                if (TopLeft) Cells.Add(TopLeft);
                if (Left) Cells.Add(Left);
                if (BottomLeft) Cells.Add(BottomLeft);

                if (Bottom) Cells.Add(Bottom);
                if (BottomRight) Cells.Add(BottomRight);
                if (Right) Cells.Add(Right);
                if (TopRight) Cells.Add(TopRight);
            }
        }

        public bool Contain(GridCell gCell)
        {
            return Cells.Contains(gCell);
        }

        public override string ToString()
        {
            return ("All cells : " + ToString(Cells));
        }

        public static string ToString(List<GridCell> list)
        {
            string res = "";
            foreach (var item in list)
            {
                res += item.ToString();
            }
            return res;
        }

        public List<PFCell> GetNeighBorsPF()
        {
            List<PFCell> res = new List<PFCell>();
            foreach (var item in Cells)
            {
                res.Add(item.pfCell);
            }

            return res;
        }

        public List<GridCell> GetMatchIdCells(int id, bool matchable)
        {
            List<GridCell> res = new List<GridCell>();
            foreach (var item in Cells)
            {
                MatchObject m = item.Match;
                if(m &&(m.GetID() == id) && (item.IsMatchable == matchable))
                {
                    res.Add(item);
                }
            }
            return res;
        }
    }
}