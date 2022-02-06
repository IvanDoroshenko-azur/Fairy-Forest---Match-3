using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

namespace Mkey
{
    public class TutorialWindowController : PopUpsController
    {
        [Space(8)]
        [Header("Mask prefab")]

        [SerializeField]
        public GameObject prefabMaskObject;

        [SerializeField]
        public GameObject prefabMaskBoosterObject;

        [SerializeField]
        public GameObject parentMaskObjects;
       
        [Space(8)]
        [Header("Hand Obj")]

        [SerializeField]
        public GameObject prefabHandObject;

        [SerializeField]
        public Text textTutorial;

        public List<Vector2> boosterVectors;
        private int indBoost;
        private int boosterAnim = 0;
        private float periodAnim;
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        public MatchGUIController MGui { get { return MatchGUIController.Instance; } }

        public GameConstructSet GCSet { get { return MPlayer.gcSet; } }

        public LevelConstructSet LcSet
        {
            get { return GCSet.GetLevelConstructSet(MatchPlayer.CurrentLevel); }
        }

        private bool animEnd = true;
        private bool destroyAfter = false;
        private int startPointZero = 0;
        private float time = 0;
        private Vector2 pointStart = Vector2.zero;
        private Vector2 pointFinish = Vector2.zero;

        private GameObject obj_;

        public MatchGrid grid;
        //public override void RefreshWindow()
        //{        
        //    LevelConstructSet lcs = MBoard.LcSet;
        //    Sprite spr = lcs.imageTutorial;
        //    imageTutorial.sprite = spr;
        //    _animatorHand = handObj.GetComponent<Animator>();
        //    _animatorHand.SetInteger("IntHand", (int)lcs.numAnimation);
        //    textTutorial.text = lcs.textTutorial;
        //    base.RefreshWindow();
        //}

        public static TutorialWindowController Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            textTutorial.text = LcSet.textTutorial;

            for (int i = 0; i < LcSet.tutorialCells.Count; i++)
            {
                if (LcSet.tutorialCells[i].ID == GCSet.GOSet.toolTutorials[1].ID)
                    if (startPointZero == 0)
                    {
                        startPointZero = 1;
                        pointStart = LcSet.tutorialPos[i];
                    }
                    else
                    {
                        startPointZero = 2;
                        pointFinish = LcSet.tutorialPos[i];
                        break;
                    }
            }

            if (startPointZero == 0)
            {
                for (int i = 0; i < LcSet.tutorialCells.Count; i++)
                {
                    if ((LcSet.tutorialCells[i].ID >= GCSet.GOSet.toolTutorials[2].ID) && (LcSet.tutorialCells[i].ID <= GCSet.GOSet.toolTutorials[5].ID))
                        if (startPointZero == 0)
                        {
                            if (LcSet.tutorialCells[i].ID == GCSet.GOSet.toolTutorials[4].ID)
                                startPointZero = 1;
                            else
                                startPointZero = 3;
                            indBoost = LcSet.tutorialCells[i].ID - 202;
                            if(MPlayer.GetBooster(300000 + indBoost) == 0)
                                MPlayer.AddBooster(300000 + indBoost, 1);
                            pointStart = boosterVectors[indBoost];
                            pointFinish = LcSet.tutorialPos[i];
                            var obj = Instantiate(prefabMaskBoosterObject, boosterVectors[indBoost], Quaternion.identity);
                            obj.transform.SetParent(parentMaskObjects.transform, true);
                            grid = MBoard.grid;

                            for (int j = 0; j < grid.Cells.Count; j++)
                            {
                                if (!grid.Cells[j].IsDisabled && !grid.Cells[j].Blocked)
                                {
                                    var obj__ = Instantiate(prefabMaskObject, grid.Cells[j].transform.position, Quaternion.identity);
                                    obj__.transform.SetParent(parentMaskObjects.transform, true);
                                    obj__.transform.localScale *= LcSet.Scale;
                                }
                            }
                            break;
                        }
                }
            }
            else
            {
                for (int i = 0; i < LcSet.tutorialPos.Count; i++)
                {
                    var obj = Instantiate(prefabMaskObject, LcSet.tutorialPos[i], Quaternion.identity);
                    obj.transform.SetParent(parentMaskObjects.transform, true);
                    obj.transform.localScale *= LcSet.Scale;
                }
            }
        }

        private void Update()
        {
            if (startPointZero == 2)
            {
                if (animEnd)// && (time += Time.deltaTime) > 0.5f)
                {
                    animEnd = false;
                    obj_ = Instantiate(prefabHandObject, pointStart, Quaternion.identity);
                    obj_.transform.SetParent(parentMaskObjects.transform, true);

                    SimpleTween.Move(obj_.gameObject, pointStart, pointFinish, 0.9f).AddCompleteCallBack(() =>
                    {
                        destroyAfter = true;
                        time = 0;
                    });
                }

                if (destroyAfter && (time += Time.deltaTime) > 0.5f)
                {
                    Destroy(obj_.gameObject);
                    animEnd = true;
                    destroyAfter = false;
                }
            }
            else if (startPointZero == 1)
            {
                if (animEnd)
                {
                    animEnd = false;
                    obj_ = Instantiate(prefabHandObject, pointStart, Quaternion.identity);
                    obj_.transform.SetParent(parentMaskObjects.transform, true);
                }
                obj_.transform.GetChild(0).localScale = new Vector2(1 + Mathf.PingPong(Time.time, 0.5f), 1 + Mathf.PingPong(Time.time, 0.5f));
            }
            else if (startPointZero == 3)
            {
                if (animEnd)
                {
                    animEnd = false;
                    obj_ = Instantiate(prefabHandObject, pointStart, Quaternion.identity);
                    obj_.transform.SetParent(parentMaskObjects.transform, true);
                }

                if (boosterAnim == 0)
                {
                    periodAnim += Time.deltaTime;
                    obj_.transform.GetChild(0).localScale = new Vector2(1 + Mathf.PingPong(Time.time, 0.5f), 1 + Mathf.PingPong(Time.time, 0.5f));
                    if (periodAnim >= 1f)
                        boosterAnim = 1;
                }
                else if(boosterAnim == 1)
                {
                    boosterAnim = 5;
                    SimpleTween.Move(obj_.gameObject, pointStart, pointFinish, 0.9f).AddCompleteCallBack(() =>
                    {
                        boosterAnim = 2;
                        periodAnim = 0;
                    });
                }
                else if (boosterAnim == 2)
                {
                    periodAnim += Time.deltaTime;
                    obj_.transform.GetChild(0).localScale = new Vector2(1 + Mathf.PingPong(Time.time, 0.5f), 1 + Mathf.PingPong(Time.time, 0.5f));
                    if (periodAnim >= 1f)
                    {
                        animEnd = true;
                        periodAnim = 0;
                        boosterAnim = 0;
                        Destroy(obj_.gameObject);
                    }
                }
            }
        }

        private void AnimEnd()
        {
            
        }

        public void Clear_Click()
        {
            CloseWindow();
        }
    }
}