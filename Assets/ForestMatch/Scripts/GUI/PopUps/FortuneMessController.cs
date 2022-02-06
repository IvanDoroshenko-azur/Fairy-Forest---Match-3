using UnityEngine.UI;
//using System;
using UnityEngine;

/*
 changes
    20.02.19 basic

 */
namespace Mkey
{
    public class FortuneMessController : PopUpsController
    {

        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private InGamePurchaser InGamePur { get { return InGamePurchaser.Instance; } }

        #region Prise setting
        [Space(8)]
        [Header("Prise setting")]
        public GameObject fonPrise;
        public GameObject txtPrise;
        public GameObject imgPrise;
        public GameObject butPrise;
        #endregion bomb setting

        #region Fortune setting
        [Space(8)]
        [Header("Fortune setting")]
        public AudioClip _clipWin;
        public RectTransform[] objFortune;
        public Sprite[] imageFortune;
        public int[] ID_Booster;
        private int[] _rand = {-1, -1, -1, -1, -1, -1};
        private string[] namePrise = { "Bomb Color", "Hammer", "Shuffle", "Coin", "Life", "Bomb"};
        private int[] array = { 1, 2, 3, 4, 5, 0 };
        public float Force = 10;
        public float offsetX = 5;
        public float dynamicScale = 0.01f;
        private float changeX = -1, oldchangeX = 0;

        public float speed = 2f;

        private int statFortune = 0;
        private GameObject tempObj;
        private AudioSource _audio;

        #endregion Fortune setting
        // Start is called before the first frame update
        private void Start()
        {
            _audio = transform.GetComponent<AudioSource>();

            for (int i = 0; i < objFortune.Length; i++)
            {
                Image img = objFortune[i].GetChild(0).GetComponent<Image>();
                int rand;
                rand = Random.Range(0, imageFortune.Length);

                int j = 0;              
                while(j < i)
                {
                    if (rand == _rand[j])
                    {
                        j = 0;
                        rand = Random.Range(0, imageFortune.Length);
                    }
                    else
                        j++;
                }
                _rand[i] = rand;
                img.sprite = imageFortune[rand];
                objFortune[i].transform.localPosition = new Vector3(offsetX * i - 500, objFortune[i].transform.localPosition.y, objFortune[i].transform.localPosition.z);
                objFortune[i].transform.localScale = new Vector3(objFortune[i].transform.localScale.x + Mathf.Sin(i / 10), objFortune[i].transform.localScale.y, objFortune[i].transform.localScale.z);
            }
        }

        private void Update()
        {
            for (int i = 0; i < objFortune.Length; i++)
            {
                float xTrain = objFortune[array[i]].transform.localPosition.x;
                //objFortune[i].transform.localPosition = new Vector2(xTrain += 0.001f, objFortune[i].transform.localPosition.y);
            }

                //for (int i = 0; i < objFortune.Length; i++)
                //{
                //    if (objFortune[i].transform.localPosition.x > 500)
                //        objFortune[i].transform.localPosition = new Vector3(objFortune[array[i]].transform.localPosition.x - offsetX, objFortune[i].transform.localPosition.y, objFortune[i].transform.localPosition.z);

                //    float _scale = 1.12f + Mathf.Cos(objFortune[i].transform.localPosition.x / 200) * dynamicScale;
                //    objFortune[i].transform.localScale = new Vector3(_scale, _scale, objFortune[i].transform.localScale.z);
                //}  

                if (statFortune == 2)
            {
                int fort = 0;
                string key = "mkmatch_" + "fortuna";

                fort = MPlayer.Fortune;
                PlayerPrefs.SetInt(key, fort - 1);
                MPlayer.Fortune -= 1;
                MPlayer.ChangeFortuneEvent?.Invoke(MPlayer.Fortune);

                //for (int i = 0; i < objFortune.Length; i++)
                //{
                //    Rigidbody2D rigi = objFortune[i].GetComponent<Rigidbody2D>();
                //    rigi.simulated = false;
                //}

                int indexPrise = 9;

                for (int i = 0; i < objFortune.Length; i++)
                {
                    if(objFortune[i].name == tempObj.name)
                    {
                        indexPrise = i;
                        break;
                    }
                }

                _audio.PlayOneShot(_clipWin);

                Image img = imgPrise.GetComponent<Image>();
                txtPrise.GetComponent<Text>().text = "x 1";

                if (namePrise[_rand[indexPrise]] == "Life")
                    MPlayer.AddLifes(1);
                if (namePrise[_rand[indexPrise]] == "Coin")
                {
                    txtPrise.GetComponent<Text>().text = "x 15";
                    MPlayer.AddCoins(15);
                }    
              
                   if (namePrise[_rand[indexPrise]] == "Bomb Color")
                    BoosterAdd(300000);
                   else if (namePrise[_rand[indexPrise]] == "Bomb")
                    BoosterAdd(300001);
                   else if (namePrise[_rand[indexPrise]] == "Shuffle")
                    BoosterAdd(300002);
                   else if (namePrise[_rand[indexPrise]] == "Hammer")
                    BoosterAdd(300003);
                       
                img.sprite = imageFortune[_rand[indexPrise]];
                fonPrise.gameObject.SetActive(true);
                imgPrise.gameObject.SetActive(true);
                txtPrise.gameObject.SetActive(true);
                butPrise.gameObject.SetActive(true);

                statFortune = 0;
            }
        }


        public void BoosterAdd(int id)
        {
            Booster b = MPlayer?.BoostHolder.GetBoosterById(id);
            if (b != null)
                b.AddCount(1);
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < objFortune.Length; i++)
            {
                //float xTrain = objFortune[array[i]].transform.localPosition.x;
                //objFortune[i].transform.localPosition = new Vector3(xTrain += 0.001f, objFortune[i].transform.localPosition.y, objFortune[i].transform.localPosition.z);
                if (statFortune == 1)
                {
                    objFortune[i].transform.Translate(transform.right * speed);
                    speed *= 0.998f;
                }

                if (objFortune[i].transform.localPosition.x > 500)
                    objFortune[i].transform.localPosition = new Vector3(objFortune[array[i]].transform.localPosition.x - offsetX, objFortune[i].transform.localPosition.y, objFortune[i].transform.localPosition.z);

                float _scale = 1.12f + Mathf.Cos(objFortune[i].transform.localPosition.x / 200) * dynamicScale;
                objFortune[i].transform.localScale = new Vector3(_scale, _scale, objFortune[i].transform.localScale.z);
            } 


            if (statFortune == 1)
            {
                changeX = objFortune[0].transform.localPosition.x;
                if (Mathf.Ceil(changeX) != Mathf.Ceil(oldchangeX))
                {
                    oldchangeX = changeX;
                }
                else
                {
                    statFortune = 2;
                    oldchangeX = 0;
                }
            }
        }

        public void Go_Fortune()
        {
            if (statFortune == 0)
            {
                statFortune = 1;
                //for (int i = 0; i < objFortune.Length; i++)
                //{
                //    Rigidbody2D rigi = objFortune[i].GetComponent<Rigidbody2D>();
                //    rigi.simulated = true;
                //    rigi.AddForce(new Vector3(Force + Random.Range(-100, 100), 0, 0));
                //}
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (statFortune == 2)
            {
                other.transform.localScale = new Vector3(3, 3, other.gameObject.transform.localScale.z);
            }

            if (statFortune == 1)
            {
                tempObj = other.gameObject;
                _audio.Play();
            }
        }
    }
}