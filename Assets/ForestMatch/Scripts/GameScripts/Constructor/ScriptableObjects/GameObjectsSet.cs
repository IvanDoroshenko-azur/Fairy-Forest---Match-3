using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mkey
{
    [CreateAssetMenu]
    public class GameObjectsSet : BaseScriptable, ISerializationCallbackReceiver
    {

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }

        public Sprite[] backGrounds;

        public GameObject gridCellOdd;
        public GameObject gridCellEven;

        [Tooltip("Used only for constructor, make visible disabled cells")]
        public Sprite gridCellDisabledSprite;

        [SerializeField]
        [HideInInspector]
        private BaseObjectData disabledObject; // flag for disabled gridcell

        [SerializeField]
        private List<BaseObjectData> blockedObjects;

        [SerializeField]
        public List<TutorialObjectData> toolTutorials;  

        [SerializeField]
        private List<MatchObjectData> matchObjects;

        [SerializeField]
        private List<OverlayObjectData> overlayObjects;

        [SerializeField]
        private List<UnderlayObjectData> underlayObjects;

        [Header("Static Match Bombs")]
        [SerializeField]
        private StaticMatchBombObjectData staticMatchBombObjectVertical;

        [SerializeField]
        private StaticMatchBombObjectData staticMatchBombObjectHorizontal;

        [SerializeField]
        private StaticMatchBombObjectData staticMatchBombObjectRadial;

        [Header("Dynamic Match Bombs")]
        [Space(8)]
        [SerializeField]
        private DynamicMatchBombObjectData dynamicMatchBombObjectVertical;

        [SerializeField]
        private DynamicMatchBombObjectData dynamicMatchBombObjectHorizontal;

        [SerializeField]
        private DynamicMatchBombObjectData dynamicMatchBombObjectRadial;

        [Header("Dynamic Click Bombs")]
        [Space(8)]
        [SerializeField]
        private DynamicClickBombObjectData dynamicClickBombObjectVertical;

        [SerializeField]
        private DynamicClickBombObjectData dynamicClickBombObjectHorizontal;

        [SerializeField]
        private DynamicClickBombObjectData dynamicClickBombObjectRadial;

        [SerializeField]
        private DynamicClickBombObjectData dynamicClickBombObjectDinamit;

        [SerializeField]
        private DynamicClickBombObjectData dynamicClickBombObjectColor;
        [SerializeField]
        private DynamicClickBombObjectData dynamicClickBombObjectButterfly;

        [SerializeField]
        private List<BoosterObjectData> boosterObjects;

        [SerializeField]
        private FallingObjectData fallingObject; 

        private List<BaseObjectData> targetObjects;

        #region properties

        public IList<MatchObjectData> MatchObjects
        {
            get {return matchObjects.AsReadOnly(); }
        }

        public IList<TutorialObjectData> TutorialObjects
        {
            get { return  toolTutorials.AsReadOnly(); }
        }

        public BaseObjectData Disabled // flag for disabled gridcell
        {
            get { return disabledObject; }
        }

        public IList<BaseObjectData> BlockedObjects
        {
            get { return blockedObjects.AsReadOnly(); }
        }

        public IList<OverlayObjectData> OverlayObjects
        {
            get { return overlayObjects.AsReadOnly(); }
        }

        public IList<UnderlayObjectData> UnderlayObjects
        {
            get { return underlayObjects.AsReadOnly(); }
        }

        public IList<BoosterObjectData> BoosterObjects { get {return boosterObjects.AsReadOnly(); } }

        public IList<BaseObjectData> TargetObjects { get { CreateTargets(); return targetObjects.AsReadOnly(); } }

        public IList<StaticMatchBombObjectData> StaticMatchBombObjects { get { return new List<StaticMatchBombObjectData>(new StaticMatchBombObjectData [] { staticMatchBombObjectVertical, staticMatchBombObjectHorizontal, staticMatchBombObjectRadial }).AsReadOnly(); } }

        public IList<DynamicMatchBombObjectData> DynamicMatchBombObjects { get { return new List<DynamicMatchBombObjectData>(new DynamicMatchBombObjectData[] { dynamicMatchBombObjectVertical, dynamicMatchBombObjectHorizontal, dynamicMatchBombObjectRadial }).AsReadOnly(); } }

        public IList<DynamicClickBombObjectData> DynamicClickBombObjects { get { return new List<DynamicClickBombObjectData>(new DynamicClickBombObjectData[] { dynamicClickBombObjectVertical, dynamicClickBombObjectHorizontal, dynamicClickBombObjectRadial, dynamicClickBombObjectDinamit, dynamicClickBombObjectColor }).AsReadOnly(); } }


        /*
        [Serializable]
        public class OffMatch_ID
        {
            [SerializeField]
            private int id;
            [SerializeField]
            private int count;

            public int ID { get { return id; } }
            public int Count { get { return count; } }

            public OffMatch_ID(int id, int count)
            {
                this.count = count;
                this.id = id;
            }
        }
        */

        public FallingObjectData FallingObject 
        {
            get { return fallingObject; }
        }

        public int RegularLength
        {
            get { return MatchObjects.Count; }
        }
        #endregion properties

        #region serialization
        public void OnBeforeSerialize()
        {
            disabledObject = new MatchObjectData();
            disabledObject.ObjectImage = gridCellDisabledSprite;

            // set ids for game objects
            Disabled.Enumerate(1);
            EnumerateArray(blockedObjects, 100);
            EnumerateArray(toolTutorials, 200);
            EnumerateArray(matchObjects, 1000);
            EnumerateArray(overlayObjects, 100000);
            EnumerateArray(underlayObjects, 200000);
            EnumerateArray(boosterObjects, 300000);
           

            staticMatchBombObjectVertical.bombType = BombDir.Vertical;
            staticMatchBombObjectHorizontal.bombType = BombDir.Horizontal;
            staticMatchBombObjectRadial.bombType = BombDir.Radial;
            staticMatchBombObjectVertical.Enumerate(400000);
            staticMatchBombObjectHorizontal.Enumerate(400001);
            staticMatchBombObjectRadial.Enumerate(400002);

            dynamicMatchBombObjectVertical.bombType = BombDir.Vertical;
            dynamicMatchBombObjectHorizontal.bombType = BombDir.Horizontal;
            dynamicMatchBombObjectRadial.bombType = BombDir.Radial;
            dynamicMatchBombObjectVertical.Enumerate(400010);
            dynamicMatchBombObjectHorizontal.Enumerate(400011);
            dynamicMatchBombObjectRadial.Enumerate(400012);

            dynamicClickBombObjectVertical.bombType = BombDir.Vertical;
            dynamicClickBombObjectHorizontal.bombType = BombDir.Horizontal;
            dynamicClickBombObjectRadial.bombType = BombDir.Radial;
            dynamicClickBombObjectDinamit.bombType = BombDir.Dinamit;
            dynamicClickBombObjectColor.bombType = BombDir.Color;
            dynamicClickBombObjectButterfly.bombType = BombDir.Butterfly;
            dynamicClickBombObjectVertical.Enumerate(400020);
            dynamicClickBombObjectHorizontal.Enumerate(400021);
            dynamicClickBombObjectRadial.Enumerate(400022);
            dynamicClickBombObjectDinamit.Enumerate(400023);
            dynamicClickBombObjectColor.Enumerate(400024);
            dynamicClickBombObjectButterfly.Enumerate(400025);

            fallingObject.Enumerate(500000);
        }

        public void OnAfterDeserialize()
        {
            //   Debug.Log("deserialized ");
        }
        #endregion serialization

        #region get object
        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public MatchObjectData GetMainObject(int id)
        {
            foreach (var item in MatchObjects)
            {
                if (id == item.ID) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns random objects array.
        /// </summary>
        /// <returns>Reference to char array</returns>
        public List<MatchObjectData> GetMainRandomObjects(int count)
        {
            List<MatchObjectData> r = new List<MatchObjectData>(count);
            List<MatchObjectData> source = matchObjects;
           
            int[] temp = new int[10];
            LevelConstructSet lcs = MBoard.LcSet;

            int j = 0;

         //   for (int k = 0; k < 2; k++)
           // {
                for(int i = 0; i < source.Count; i++)
                {
                    if((source[i].ID != lcs.OffMatch_ID[0])&&
                       (source[i].ID != lcs.OffMatch_ID[1])&&
                       (source[i].ID != lcs.OffMatch_ID[2])&&
                       (source[i].ID != lcs.OffMatch_ID[3])&&
                       (source[i].ID != lcs.OffMatch_ID[4])&&
                       (source[i].ID != lcs.OffMatch_ID[5])&&
                       (source[i].ID != lcs.OffMatch_ID[6]))
                    {
                        temp[j] = i;
                        j++;
                    }
                }
          //  }
                              //count
            for (int i = 0; i < count; i++)
            {
               //   int rndNumber = UnityEngine.Random.Range(0, source.Count);
               //   r.Add(source[rndNumber]);
                int rndNumber = UnityEngine.Random.Range(0, j);
                r.Add(source[temp[rndNumber]]);
            }
            return r;
        }

        /// <summary>
        /// Returns random MainObjectData array without "notInclude" list featured objects .
        /// </summary>
        public List<MatchObjectData> GetMainRandomObjects(int count, List<BaseObjectData> notInclude)
        {
            List<MatchObjectData> r = new List<MatchObjectData>(count);
            List<MatchObjectData> source = new List<MatchObjectData>(matchObjects);

            if (notInclude != null)
                for (int i = 0; i < notInclude.Count; i++)
                {
                    source.RemoveAll((mOD) => { return mOD.ID == notInclude[i].ID; });
                }

            for (int i = 0; i < count; i++)
            {
                int rndNumber = UnityEngine.Random.Range(0, source.Count);
                r.Add(source[rndNumber]);
            }
            return r;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public TutorialObjectData GetTurorialObject(int id)
        {
            foreach (var item in toolTutorials)
            {
                if (id == item.ID) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public OverlayObjectData GetOverlayObject(int id)
        {
            foreach (var item in overlayObjects)
            {
                if (id == item.ID) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public BaseObjectData GetBlockedObject(int id)
        {
            foreach (var item in blockedObjects)
            {
                if (id == item.ID) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public UnderlayObjectData GetUnderlayObject(int id)
        {
            foreach (var item in underlayObjects)
            {
                if (id == item.ID) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public BoosterObjectData GetBoosterObject(int id)
        {
            foreach (var item in boosterObjects)
            {
                if (id == item.ID) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public StaticMatchBombObjectData GetStaticMatchBombObject(int id)
        {
            foreach (var item in StaticMatchBombObjects)
            {
                if (id == item.ID) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public StaticMatchBombObjectData GetStaticMatchBombObject(BombDir bType)
        {
            switch (bType)
            {
                case BombDir.Vertical:
                    return staticMatchBombObjectVertical;
                case BombDir.Horizontal:
                    return staticMatchBombObjectHorizontal;
                case BombDir.Radial:
                    return staticMatchBombObjectRadial;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public DynamicMatchBombObjectData GetDynamicMatchBombObject(int id)
        {
            foreach (var item in DynamicMatchBombObjects)
            {
                if (id == item.ID) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public DynamicMatchBombObjectData GetDynamicMatchBombObject(BombDir bType)
        {
            switch (bType)
            {
                case BombDir.Vertical:
                    return dynamicMatchBombObjectVertical;
                case BombDir.Horizontal:
                    return dynamicMatchBombObjectHorizontal;
                case BombDir.Radial:
                    return dynamicMatchBombObjectRadial;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public DynamicClickBombObjectData GetDynamicClickBombObject(int id)
        {
            foreach (var item in DynamicClickBombObjects)
            {
                if (id == item.ID) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public DynamicClickBombObjectData GetDynamicClickBombObject(BombDir bType)
        {
            switch (bType)
            {
                case BombDir.Vertical:
                    return dynamicClickBombObjectVertical;
                case BombDir.Horizontal:
                    return dynamicClickBombObjectHorizontal;
                case BombDir.Radial:
                    return dynamicClickBombObjectRadial;
                case BombDir.Dinamit:
                    return dynamicClickBombObjectDinamit;
                case BombDir.Color:
                    return dynamicClickBombObjectColor;
                case BombDir.Butterfly:
                    return dynamicClickBombObjectButterfly;
            }
            return null;
        }

        /// <summary>
        /// Returns reference  object from set.
        /// </summary>
        /// <returns>Reference letter</returns>
        public BaseObjectData GetObject(int id)
        {
            if (id == 1) return Disabled;

        //    foreach (var item in imageTutorial)
        //    {
        //        if (id == item.ID) return item;
       //     }

            foreach (var item in matchObjects)
            {
                if (id == item.ID) return item;
            }

            foreach (var item in  toolTutorials)
            {
                if (id == item.ID) return item;
            }

            foreach (var item in overlayObjects)
            {
                if (id == item.ID) return item;
            }

            foreach (var item in underlayObjects)
            {
                if (id == item.ID) return item;
            }

            foreach (var item in blockedObjects)
            {
                if (id == item.ID) return item;
            }

            foreach (var item in StaticMatchBombObjects)
            {
                if (id == item.ID) return item;
            }

            foreach (var item in DynamicMatchBombObjects)
            {
                if (id == item.ID) return item;
            }

            foreach (var item in DynamicClickBombObjects)
            {
                if (id == item.ID) return item;
            }

            foreach (var item in boosterObjects)
            {
                if (id == item.ID) return item;
            }
            if (id == fallingObject.ID) return fallingObject;
            return null;
        }
        #endregion get object 

        public bool ContainID(int id)
        {
            return
                (
                   ContainMatchID(id)
                || ContainOverlayID(id)
                || ContainUnderlayID(id)
                || ContainBoosterID(id)
                || ContainDynamicClickBombID(id)
                || ContainDynamicMatchBombID(id)
                || ContainStaticMatchBombID(id)
                || ContainBlockedID(id)
                || IsDisabledObject(id)
                || IsFallingObjectID(id)
                || IsTutorialObject(id)
                ) ;
        }

        public bool ContainMatchID(int id)
        {
            return ContainID(MatchObjects, id);
        }

        public bool ContainBlockedID(int id)
        {
            return ContainID(BlockedObjects, id);
        }

        public bool ContainBoosterID(int id)
        {
            return ContainID(BoosterObjects, id);
        }

        public bool ContainOverlayID(int id)
        {
            return ContainID(OverlayObjects, id);
        }

        public bool ContainUnderlayID(int id)
        {
            return ContainID(UnderlayObjects, id);
        }

        public bool ContainStaticMatchBombID(int id)
        {
            return ContainID(StaticMatchBombObjects, id);
        }

        public bool ContainDynamicMatchBombID(int id)
        {
            return ContainID(DynamicMatchBombObjects, id);
        }

        public bool ContainDynamicClickBombID(int id)
        {
            return ContainID(DynamicClickBombObjects, id);
        }

        public bool ContainTargetID(int id)
        {
            return ContainID(TargetObjects, id);
        }

        public Sprite BackGround(int index)
        {
            index = (int)Mathf.Repeat(index, backGrounds.Length);
            return backGrounds[index];
        }

        public int BackGroundsCount
        {
            get { return backGrounds.Length; }
        }


        public Sprite DisabledCellSprite
        {
            get { return Disabled.ObjectImage; }
        }

        private void CreateTargets()
        {
            targetObjects = new List<BaseObjectData>();

            if(overlayObjects!=null)
            foreach (var item in overlayObjects)
            {
                if (item.canUseAsTarget) targetObjects.Add(item);
            }

            if(underlayObjects!=null)
            foreach (var item in underlayObjects)
            {
                if (item.canUseAsTarget) targetObjects.Add(item);
            }

            if(matchObjects!=null)
            foreach (var item in matchObjects)
            {
                if (item.canUseAsTarget) targetObjects.Add(item);
            }

            if (fallingObject.canUseAsTarget) targetObjects.Add(fallingObject);
        }

        public static bool IsDisabledObject(int id)
        {
            return id == 1;
        }

        public static bool IsBlockedObject(int id)
        {
            return (id >=100) && (id < 200);
        }

        public static bool IsTutorialObject(int id)
        {
            return (id >= 200) && (id < 1000);
        }

        public static bool IsFallingObjectID(int id)
        {
          return  (id == 500000);
        }

        #region utils
        private void EnumerateArray<T>(ICollection<T> a, int startIndex) where T : BaseObjectData
        {
            if (a != null && a.Count > 0)
            {
                foreach (var item in a)
                {
                    item.Enumerate(startIndex++);
                }
            }
        }

        private bool ContainID<T>(ICollection<T> a, int id) where T : BaseObjectData
        {
            if (a == null || a.Count == 0) return false;
            foreach (var item in a)
            {
                if (item.ID == id) return true;
            }
            return false;
        }
        #endregion utils
    }

    [Serializable]
    public class MatchObjectData : BaseObjectData
    {
        public GameObject guiTargetPrefab;
        public GameObject iddleAnimPrefab;
        public GameObject collectAnimPrefab;

        [Space(8)]
        public Sprite HorizontalGift;
        public Sprite VerticalGift;
        public Sprite CrossGift;

        [Space(8)]
        [Tooltip("This object will be used as target")]
        #region construct object properties
        public bool canUseAsTarget;
        #endregion construct object properties
    }

    [Serializable]
    public class TutorialObjectData : BaseObjectData
    {
        public Sprite[] protectionStateImages;
        public GameObject hitAnimPrefab;

        [Space(8)]
        [Tooltip("Only if you collect it. In this version we can use only one object to collect")]
        #region construct object properties
        public bool canUseAsTarget;
        #endregion construct object properties
    }

    [Serializable]
    public class BoosterObjectData : BaseObjectData
    {
        public BoosterFunc prefab;
        public GameObject animPrefab;
    }

    // The object blocks dragging the main object and even blocks match
    [Serializable]
    public class OverlayObjectData : BaseObjectData
    {
        public bool blockMatch = true;
        public bool sideHit = false;
        public bool canDrag = true;
        public bool canSkip = true;
        public Sprite[] protectionStateImages;

        public GameObject hitAnimPrefab;

        [Space(8)]
        [Tooltip("Only if you collect it. In this version we can use only one object to collect")]
        #region construct object properties
        public bool canUseAsTarget;
        #endregion construct object properties

    }

    [Serializable]
    public class UnderlayObjectData : BaseObjectData
    {
        public Sprite[] protectionStateImages;
        public GameObject hitAnimPrefab;

        [Space(8)]
        [Tooltip("Only if you collect it. In this version we can use only one object to collect")]
        #region construct object properties
        public bool canUseAsTarget;
        #endregion construct object properties
    }

    [Serializable]
    public class FallingObjectData : BaseObjectData
    {
        public GameObject iddleAnimPrefab;
        public GameObject collectAnimPrefab;

        [Space(8)]
        [Tooltip("Only if you collect it. In this version we can use only one object to collect")]
        #region construct object properties
        public bool canUseAsTarget;
        #endregion construct object properties
    }


    [Serializable]
    public class StaticMatchBombObjectData : BaseObjectData
    {
        [HideInInspector]
        [SerializeField]
        public BombDir bombType;
        public GameObject iddleAnimPrefab;
        public GameObject explodeAnimPrefab;
    }

    [Serializable]
    public class DynamicMatchBombObjectData : BaseObjectData
    {
        public BombDir bombType;
        public GameObject iddleAnimPrefab;
        public GameObject explodeAnimPrefab;
    }

    [Serializable]
    public class DynamicClickBombObjectData : BaseObjectData
    {
        public BombDir bombType;
        public GameObject iddleAnimPrefab;
        public GameObject explodeAnimPrefab;
        public bool canDrag = true;
    }

    [Serializable]
    public class BaseObjectData
    {
        [HideInInspector]
        [SerializeField]
        private string name;

        [HideInInspector]
        [SerializeField]
        private int id;

        [Space(8)]
        [Tooltip("Picture that is used on bubble board")]
        [Header("Sprites")]
        public Sprite ObjectImage;
        [Tooltip("Picture that is used on gui")]
        public Sprite GuiImage;
        [Tooltip("Picture that is used on gui")]
        public Sprite GuiImageHover;
        [Tooltip("Picture that is used on gui")]
        public Sprite IconSprite;

        [Space(8)]
        [Header("Addit properties")]

        public AudioClip privateClip;

        #region properties
        public int ID
        {
            get { return id; }
            private set { id = value; }
        }

        public string Name
        {
            get { return name; }
        }
        #endregion properties

        public void Enumerate(int id)
        {
            this.id = id;
            name = (ObjectImage == null) ? "null" + "-" + id : ObjectImage.name + "; ID : " + id;
        }
    }

    [Serializable]
    public class CellData
    {
        [SerializeField]
        private int id;
        [SerializeField]
        private int row;
        [SerializeField]
        private int column;

        public int ID { get { return id; } }
        public int Row { get { return row; } }
        public int Column { get { return column; } }

        public CellData(int id, int row, int column)
        {
            this.row = row;
            this.column = column;
            this.id = id;
        }
    }

    /// <summary>
    /// Helper serializable class object with the equal ID
    /// </summary>
    [Serializable]
    public class ObjectsSetData
    {
        public Action <int> ChangeEvent;

        [SerializeField]
        private int id;
        [SerializeField]
        private int count;

        public int ID { get { return id; } }
        public int Count { get { return count; } }

        public ObjectsSetData(int id, int count)
        {
            this.id = id;
            this.count = count;
        }

        public Sprite GetImage(GameObjectsSet mSet)
        {
            return mSet.GetMainObject(id).GuiImage;
        }

        public void IncCount()
        {
            SetCount(count + 1);
        }

        public void DecCount()
        {
            SetCount(count - 1);
        }

        public void SetCount(int newCount)
        {
            newCount = Mathf.Max(0, newCount);
            bool changed = (Count != newCount);
            count = newCount;
            if(changed)  ChangeEvent?.Invoke(count);
        }
    }

    /// <summary>
    /// Helper class that contains list of object set data 
    /// </summary>
    [Serializable]
    public class ObjectSetCollection
    {
        [SerializeField]
        private List<ObjectsSetData> list;

        public IList<ObjectsSetData> ObjectsList { get { return list.AsReadOnly(); } }

        public ObjectSetCollection()
        {
            list = new List<ObjectsSetData>();
        }

        public uint Count
        {
            get { return (list == null) ? 0 : (uint)list.Count; }
        }

        public void AddById(int id, int count)
        {

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ID == id)
                {
                    list[i].SetCount(list[i].Count + count);
                    return;
                }
            }
            list.Add(new ObjectsSetData(id, count));
        }

        public void RemoveById(int id, int count)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ID == id)
                {
                    list[i].SetCount(list[i].Count - count);
                    return;
                }
            }
        }

        public void SetCountById(int id, int count)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ID == id)
                {
                    list[i].SetCount(count);
                    return;
                }
            }
            list.Add(new ObjectsSetData(id, count));
        }

        public void CleanById(int id)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ID == id)
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }

        public int CountByID(int id)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ID == id)
                    return list[i].Count;
            }
            return 0;
        }

        public bool ContainObjectID(int id)
        {
            return CountByID(id) > 0;
        }

    }
  
}