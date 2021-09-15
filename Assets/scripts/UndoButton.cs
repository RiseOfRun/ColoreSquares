using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UndoButton : MonoBehaviour
{
    public Image DisabledImage;

    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.Instance.CanBack)
        {
            button.enabled = false;
            button.interactable = false;
            return;
        }

        button.enabled = true;
        button.interactable = true;
    }
}
