using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid_Board : GameObjectBase
{
    readonly Vector2 boardSize = new Vector2(0.7f, 0.7f);
    Vector2 idx;

    public void Init(Vector2 idx)
    {
        this.idx = idx;
        SetPosition();
    }

    void SetPosition()
    {
        float xOffSet = idx.y * (boardSize.x * 0.5f);
        cachedTransform.localPosition = new Vector3(idx.x * boardSize.x + xOffSet, idx.y * boardSize.y);
    }

    void OnClick()
    {
        Debug.Log("Baord Clicked");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                    Debug.Log(idx + ". Board Clicked");
            }
        }
    }
}
