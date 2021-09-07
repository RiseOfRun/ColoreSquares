using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class Square : MonoBehaviour
{
    public TextMeshPro text;
    public Color[] colors;
    public bool CanMerge = true;
    public int Weight
    {
        get => weight;
        set
        {
            weight = value;
            text.text = weight > 0 ? Mathf.Pow(2, weight).ToString() : "";
            
        }
    }

    public float speed = 5;
    private int weight;
    public Vector2Int TargetPosition { get; set; }
    public Square MergeAfterReachingTarget { get; set; }

    public static Square Merge(Square from, Square to)
    {
        from.Weight++;
        Destroy(to.gameObject);
        from.CanMerge = false;
        from.SetColor();
        return from;
    }

    void SetColor()
    {
        var sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = weight>colors.Length-1 ? colors.Last() : colors[weight];
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        SetColor();
        for (float i = 0; i < 1; i+=Time.deltaTime/0.3f)
        {
            float fade = Mathf.Lerp(0, 1, i);
            fade = 1 - (1 - fade) * (1 - fade) * (1 - fade);
            transform.localScale = new Vector3(fade,fade,fade);
            yield return null;
        }
        
        transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nextPosition = Vector2.MoveTowards(transform.localPosition, TargetPosition, Time.deltaTime * speed);
        transform.localPosition = nextPosition;

        if ((Vector2) transform.localPosition == (Vector2) TargetPosition)
        {
            if (MergeAfterReachingTarget!=null)
            {
                Merge(this, MergeAfterReachingTarget);
                MergeAfterReachingTarget = null;
            }
        }
    }
}
