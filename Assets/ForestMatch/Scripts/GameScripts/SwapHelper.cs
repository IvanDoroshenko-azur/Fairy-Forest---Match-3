using System;
using UnityEngine;

namespace Mkey
{
    public class SwapHelper 
    {
        public static GridCell Source;
        public static GridCell Target;

        public static Action<GridCell> SwapBeginEvent;
        public static Action<GridCell> SwapEndEvent;
        private static TouchManager Touch { get { return TouchManager.Instance; } }

        private static MatchBoard MBoard { get { return MatchBoard.Instance; } }

        public static void Swap()
        {
            Source = (Touch.Draggable) ? Touch.Source : null;
          //  Source = Touch.Source;

            Target = Touch.Target;
            MBoard.CellTarget = Touch.Target;

            Swap(Source, Target);
            Touch.SetDraggable(null, null);
            Touch.SetTarget(null);
        }

        public static void Swap(GridCell gc1, GridCell gc2)
        {
            Source = gc1;
            Target = gc2;

            if (Source && Source.CanSwap(Target))
            {
                SwapBeginEvent?.Invoke(Target);
                if (Source.Match || Source.DynamicClickBomb)
                {

                    if (Source.Match)
                    {
                        MatchObject dM = Source.Match;
                        Target.GrabDynamicObject(dM.gameObject, false, null);
                        //{
                        //    SwapEndEvent?.Invoke(Source);
                        //});
                    }
                    else
                    {
                        DynamicClickBombObject dM = Source.DynamicClickBomb;
                        Target.GrabDynamicObject(dM.gameObject, false, null);
                        //{
                        //    SwapEndEvent?.Invoke(Source);
                        //});
                    }

                    if (Target.Match)
                    {
                        MatchObject tM = Target.Match;
                        Source.GrabDynamicObject(tM.gameObject, false, () =>
                       {
                           SwapEndEvent?.Invoke(Target);
                       });
                    }
                    else
                    {
                        DynamicClickBombObject tM = Target.DynamicClickBomb;
                        Source.GrabDynamicObject(tM.gameObject, false, () =>
                       {
                           SwapEndEvent?.Invoke(Target);
                       });
                    }
                    ////////GameObject dM = Source.gameObject;
                    ////////GameObject tM = Target.gameObject;


                    //MatchObject dM = Source.Match;
                    //MatchObject tM = Target.Match;
                    //dM.SwapTime = Time.time;
                    //tM.SwapTime = Time.time;
                    //Source.GrabDynamicObject(tM.gameObject, false, null);
                    //Target.GrabDynamicObject(dM.gameObject, false, () =>
                    //{
                    //    SwapEndEvent?.Invoke(Target);
                    //});

                }
            }
            else Touch.ResetDrag(null);
        }

        public static void UndoSwap(Action callBack)
        {
            //if (Source.Match || Source.DynamicClickBomb)
            //{
            //    if (Source.Match)
            //    {
            //        MatchObject dM = Source.Match;
            //        Target.GrabDynamicObject(dM.gameObject, false, null);
            //        //{
            //        //    SwapEndEvent?.Invoke(Source);
            //        //});
            //    }
            //    else
            //    {
            //        DynamicClickBombObject dM = Source.DynamicClickBomb;
            //        Target.GrabDynamicObject(dM.gameObject, false, null);
            //        //{
            //        //    SwapEndEvent?.Invoke(Source);
            //        //});
            //    }

            //    if (Target.Match)
            //    {
            //        MatchObject tM = Target.Match;
            //        Source.GrabDynamicObject(tM.gameObject, false, () =>
            //        {
            //            callBack?.Invoke();
            //        });
            //    }
            //    else
            //    {
            //        DynamicClickBombObject tM = Target.DynamicClickBomb;
            //        Source.GrabDynamicObject(tM.gameObject, false, () =>
            //        {
            //            callBack?.Invoke();
            //        });
            //    }
            //}

            MatchObject dM = Source.Match;
            MatchObject tM = Target.Match;
            Source.GrabDynamicObject(tM.gameObject, false, null);
            Target.GrabDynamicObject(dM.gameObject, false, () =>
            {
                callBack?.Invoke();
            });
        }
    }
}