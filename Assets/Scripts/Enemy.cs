using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int Health = 100;                                 //체력 선언(integer)
    public float Timer = 1.0f;                                 //타이머 선언(float)
    public int AttackPoint = 50;                                 //공격력 선언(integer)
    // 최초 프레임이 업데이트 되기 "전" 1회
    void Start()
    {
        Health += 100;                                         //Health를 100 더 올려준다
    }

    // 매 프레임
    void Update()
    {
        CharacterHealthUp();

        if (Input.GetKeyDown(KeyCode.Space))                      //스페이스 키를 눌렀을 때
        {
            Health -= AttackPoint;                               //체력을 공격력만큼 감소
        }

        CheckDeath();
    }

    public void CharacterHit(int Damage)                                 //대미지 받는 함수 선언
    {
        Health -= Damage;                                          //받은 공격력에 대한 체력을 감소시킨다
    }

    void CheckDeath()
    {
        if (Health <= 0)                                         //체력이 0이라면
        {
            Destroy(gameObject);                                 //이 오브젝트 파괴
        }
    }

    void CharacterHealthUp()
    {
        Timer-= Time.deltaTime;
        if (Timer <= 0)
        {
            Timer = 1;
            Health += 10;
        }
    }

    
}