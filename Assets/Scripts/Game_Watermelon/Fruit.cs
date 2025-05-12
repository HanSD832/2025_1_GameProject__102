using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int fruitType;
    public bool hasMerged = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //이미 합쳐진 과일은 무시
        if (hasMerged)
            return;

        //다른 과일과 충돌했는지 확인
        Fruit otherFruit=collision.gameObject.GetComponent<Fruit>();

        //충돌한 것이 과일이고 타입이 같다면
        if (otherFruit != null&& !otherFruit.hasMerged && otherFruit.fruitType==fruitType)
        {
            //합쳤다고 표시
            hasMerged = true;
            otherFruit.hasMerged = true;

            //두 과일의 중간 위치 계산
            Vector3 mergePosition=(transform.position+otherFruit.transform.position)/2f;

            //다음 단계 과일로 업그레이드
            FruitGame gameManager = FindObjectOfType<FruitGame>();
            if (gameManager != null)
            {
                gameManager.MergeFruits(fruitType, mergePosition);
            }

            //기존 과일 제거
            Destroy(otherFruit.gameObject);
            Destroy(gameObject);

        }
    }
}
