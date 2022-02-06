using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
namespace Mkey
{
    public static class CollectionExt 
    {
        #region collections
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = (UnityEngine.Random.Range(0, n) % n);
                n--;
                T val = list[k];
                list[k] = list[n];
                list[n] = val;
            }
        }

        public static List<BaseObjectData> GetBaseList<T>(this IList<T> list) where T : BaseObjectData
        {
            List<BaseObjectData> resL = new List<BaseObjectData>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                resL.Add(list[i]);
            }
            return resL;
        }

        public static T GetRandomPos<T>(this IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        #endregion collections
    }
}
