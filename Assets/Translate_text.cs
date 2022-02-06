using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translate_text : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text text = gameObject.GetComponent<Text>();
        if (text) text.text = Translator.Translate(text.text);
    }

    // Update is called once per frame
}
