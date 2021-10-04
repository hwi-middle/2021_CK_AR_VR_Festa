using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeUIText : MonoBehaviour
{
    private string script = "안녕하세요. 담배 하나만 주시겠어요?";
    private Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<Text>();
        StartCoroutine(TypeText(0.1f));
    }

    IEnumerator TypeText(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var t in script)
        {
            _text.text += t;
            yield return new WaitForSeconds(delay);
        }
    }
}
