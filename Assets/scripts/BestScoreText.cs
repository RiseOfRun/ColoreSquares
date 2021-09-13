using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestScoreText : MonoBehaviour
{
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = GameController.Instance.BestScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = GameController.Instance.BestScore.ToString();
    }
}
