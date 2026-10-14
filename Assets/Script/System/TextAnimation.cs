using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    //ダメージや回復時に表示されるテキストの表示時間設定処理

    //テキスト表示時間
    private float lifeTime = 0.5f;

    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
