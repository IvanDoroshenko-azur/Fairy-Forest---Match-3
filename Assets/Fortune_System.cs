using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fortune_System : MonoBehaviour
{
    public AudioClip _clipWin;
    public RectTransform[] objFortune;

    #region Prise setting
    [Space(8)]
    [Header("Prise setting")]
    public GameObject fonPrise;
    public GameObject textPrise;
    public GameObject imgPrise;
    public GameObject butPrise;
    #endregion bomb setting

    #region Fortune setting
    [Space(8)]
    [Header("Fortune setting")]
    public Sprite[] imageFortune;
    private int[] _rand = new int[10];
    private string[] namePrise = {"Bomb Shadow", "Movement", "Explosive", "Coin", "Life", "Magnet", "Wand", "Shuffle", "Hammer", "Bomb"};
    private int[] array = {1,2,3,4,5,6,7,8,9,0};
    public float Force = 10;
    public float offsetX = 5;
    public float dynamicScale = 0.01f;
    private float changeX=-1, oldchangeX=0;
    private enum StatFortune { iddleFortune =0, runFortune, stopFortune};
    private int statFortune = 0;
    #endregion Fortune setting

    private GameObject tempObj;

    private AudioSource _audio;
    // Start is called before the first frame update
    void Start()
    {
        _audio = transform.GetComponent<AudioSource>();

      for(int i=0; i < objFortune.Length; i++)
        {
            //  Instantiate(objFortune[i], new Vector3(offsetX * i - 5, transform.localPosition.y, transform.localPosition.z), Quaternion.identity);
            Image img = objFortune[i].GetChild(0).GetComponent<Image>();
            int rand;
            rand = Random.Range(0, imageFortune.Length - 1);
            for (int j = 0; j < i; j++)
            {
                if (rand == _rand[j])
                {
                    j = 0;
                    rand = Random.Range(0, imageFortune.Length - 1);
                }
            }
            _rand[i] = rand;
            img.sprite = imageFortune[rand]; 
            objFortune[i].transform.localPosition = new Vector3(offsetX * i -500, objFortune[i].transform.localPosition.y, objFortune[i].transform.localPosition.z);
            objFortune[i].transform.localScale = new Vector3(objFortune[i].transform.localScale.x + Mathf.Sin(i/10), objFortune[i].transform.localScale.y,  objFortune[i].transform.localScale.z);
            Rigidbody2D rigi = objFortune[i].GetComponent<Rigidbody2D>();
           // rigi.AddForce(new Vector3(Force, 0, 0));
 
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < objFortune.Length; i++)
        {
            if(objFortune[i].transform.localPosition.x > 1000)
                objFortune[i].transform.localPosition = new Vector3(objFortune[array[i]].transform.localPosition.x - offsetX, objFortune[i].transform.localPosition.y, objFortune[i].transform.localPosition.z);
            
            float _scale = 1.12f + Mathf.Cos(objFortune[i].transform.localPosition.x / 200) * dynamicScale;
            objFortune[i].transform.localScale = new Vector3(_scale, _scale, objFortune[i].transform.localScale.z);
            //Rigidbody2D rigi = objFortune[i].GetComponent<Rigidbody2D>();
            // rigi.AddForce(new Vector3(10, 0, 0));
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
                for (int i = 0; i < objFortune.Length; i++)
                {
                    Rigidbody2D rigi = objFortune[i].GetComponent<Rigidbody2D>();
                    rigi.simulated = false;
                }
                int indexPrise=-1;
              //  Debug.Log(tempObj.name);
                if      (tempObj.name == "FortuneBoosterPrefab"   ) indexPrise = 0;
                else if (tempObj.name == "FortuneBoosterPrefab(1)") indexPrise = 1;
                else if (tempObj.name == "FortuneBoosterPrefab(2)") indexPrise = 2;
                else if (tempObj.name == "FortuneBoosterPrefab(3)") indexPrise = 3;
                else if (tempObj.name == "FortuneBoosterPrefab(4)") indexPrise = 4;
                else if (tempObj.name == "FortuneBoosterPrefab(5)") indexPrise = 5;
                else if (tempObj.name == "FortuneBoosterPrefab(6)") indexPrise = 6;
                else if (tempObj.name == "FortuneBoosterPrefab(7)") indexPrise = 7;
                else if (tempObj.name == "FortuneBoosterPrefab(8)") indexPrise = 8;
                else if (tempObj.name == "FortuneBoosterPrefab(9)") indexPrise = 9;

              //  Debug.Log(namePrise[_rand[indexPrise]]);

                _audio.PlayOneShot(_clipWin);

                Image img = imgPrise.GetComponent<Image>();
                img.sprite = imageFortune[_rand[indexPrise]];
                fonPrise.gameObject.SetActive(true);
                imgPrise.gameObject.SetActive(true);
                textPrise.gameObject.SetActive(true);
                butPrise.gameObject.SetActive(true);

                statFortune = 2;
                oldchangeX = 0;
            }
         //   Debug.Log(changeX + " = " + oldchangeX);
        }

    }

    public void Go_Fortune()
    {
        statFortune = 1;
        for (int i = 0; i < objFortune.Length; i++)
        {
            Rigidbody2D rigi = objFortune[i].GetComponent<Rigidbody2D>();
            rigi.simulated = true;
            rigi.AddForce(new Vector3(Force, 0, 0));
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

       // Debug.Log("Klac!!!");
    }

}
