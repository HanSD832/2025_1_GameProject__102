using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("기본 이동 설정")]
    public float moveSpeed = 5.0f;  //이동 속도 변수 설정
    public float jumpForce = 7.0f;  //점프의 힘 값을 준다
    public float turnSpeed = 10f;   //회전 속도

    [Header("점프 개선 설정")]
    public float fallMultiplier = 2.5f; //하강 중력 배울
    public float lowJumpMultiplier = 2.0f;  //짧은 점프 배율

    [Header("지면 감지 설정")]
    public float coyoteTime = 0.15f;
    public float coyoteTimeCounter;
    public bool realGrounded = true;

    [Header("글라이더 설정")]
    public GameObject gliderObject;
    public float gliderFallspeed = 1.0f;
    public float gliderMoveSpeed = 7.0f;
    public float gliderMaxTime = 5.0f;
    public float gliderTimeLeft;
    public bool isGliding = false;

    public bool isGrounded = true;  //땅에 있는지 체크하는 변수 (true/false)

    public int coinCount = 0;   //코인 획득 변수 선언
    public int totalCoins = 50; //총 코인 획득 필요 변수 선언

    public Rigidbody rb;    //플레이어 강체를 선언
    // Start is called before the first frame update
    void Start()
    {
        //글라이더 오브젝트 초기화
        if (gliderObject != null)
        {
            gliderObject.SetActive(false);
        }

        gliderTimeLeft = gliderMaxTime;

        coyoteTimeCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //지면 감지 활성화
        UpdateGroundedState();

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //이동 방향 벡터
        Vector3 movement=new Vector3(moveHorizontal,0,moveVertical);

        if(movement.magnitude>0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        //g로 글라이딩
        if(Input.GetKey(KeyCode.G) && !isGrounded && gliderTimeLeft> 0)
        {
            if(!isGliding)
            {
                //글라이더 활성화 함수
                EnableGlider();
            }

            gliderTimeLeft-= Time.deltaTime;    //글라이더 사용시간 감소

            if (gliderTimeLeft<=0)
            {
                //글라이더 비활성화 함수
                DisableGlider();
            }
        }
        else if(isGliding)
        {
            //g키를떼면 글라이더 비활성화
            DisableGlider();
        }

        if (isGliding)
        {
            ApplyGliderMovement(moveHorizontal,moveVertical);
        }
        else
        {
            //속도값으로 직접 이동
            rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);
            //착지 점프 높이 구현
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        //속도값으로 직접 이동
        rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);
        //착지 점프 높이 구현
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            realGrounded = false;
            coyoteTimeCounter = 0;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGrounded = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Coin"))
        {
            coinCount++;
            Destroy(other.gameObject);
            Debug.Log($"코인 수집 : {coinCount}/{totalCoins}");
        }

        if(other.CompareTag("Door")&&coinCount>=totalCoins)
        {
            Debug.Log("게임 클리어");
        }
    }

    void UpdateGroundedState()
    {
        if(realGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            isGrounded=true;
        }
        else
        {
            if (coyoteTimeCounter > 0)
            {
                coyoteTimeCounter-=Time.deltaTime;
                isGrounded=true;
            }
            else
            {
                isGrounded = false;
            }
        }
    }
    void EnableGlider() //글라이더 활성화
    {
        isGliding = true;
        if (gliderObject != null)
        {
            gliderObject.SetActive(true);
        }

        rb.velocity=new Vector3(rb.velocity.x,gliderFallspeed,rb.velocity.z);
    }

    void DisableGlider() //글라이더 비활성화
    {
        isGliding=false;

        if (gliderObject != null)
        {
            gliderObject.SetActive(false);
        }

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    void ApplyGliderMovement(float horizontal,float vertical)
    {
        Vector3 gliderVelocity=new Vector3(horizontal*gliderMoveSpeed, -gliderFallspeed, vertical*gliderMoveSpeed);

        rb.velocity = gliderVelocity;
    }
}
