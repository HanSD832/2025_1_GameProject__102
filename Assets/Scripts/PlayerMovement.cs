using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("�⺻ �̵� ����")]
    public float moveSpeed = 5.0f;  //�̵� �ӵ� ���� ����
    public float jumpForce = 7.0f;  //������ �� ���� �ش�
    public float turnSpeed = 10f;   //ȸ�� �ӵ�

    [Header("���� ���� ����")]
    public float fallMultiplier = 2.5f; //�ϰ� �߷� ���
    public float lowJumpMultiplier = 2.0f;  //ª�� ���� ����

    [Header("���� ���� ����")]
    public float coyoteTime = 0.15f;
    public float coyoteTimeCounter;
    public bool realGrounded = true;

    [Header("�۶��̴� ����")]
    public GameObject gliderObject;
    public float gliderFallspeed = 1.0f;
    public float gliderMoveSpeed = 7.0f;
    public float gliderMaxTime = 5.0f;
    public float gliderTimeLeft;
    public bool isGliding = false;

    public bool isGrounded = true;  //���� �ִ��� üũ�ϴ� ���� (true/false)

    public int coinCount = 0;   //���� ȹ�� ���� ����
    public int totalCoins = 50; //�� ���� ȹ�� �ʿ� ���� ����

    public Rigidbody rb;    //�÷��̾� ��ü�� ����
    // Start is called before the first frame update
    void Start()
    {
        //�۶��̴� ������Ʈ �ʱ�ȭ
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
        //���� ���� Ȱ��ȭ
        UpdateGroundedState();

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //�̵� ���� ����
        Vector3 movement=new Vector3(moveHorizontal,0,moveVertical);

        if(movement.magnitude>0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        //g�� �۶��̵�
        if(Input.GetKey(KeyCode.G) && !isGrounded && gliderTimeLeft> 0)
        {
            if(!isGliding)
            {
                //�۶��̴� Ȱ��ȭ �Լ�
                EnableGlider();
            }

            gliderTimeLeft-= Time.deltaTime;    //�۶��̴� ���ð� ����

            if (gliderTimeLeft<=0)
            {
                //�۶��̴� ��Ȱ��ȭ �Լ�
                DisableGlider();
            }
        }
        else if(isGliding)
        {
            //gŰ������ �۶��̴� ��Ȱ��ȭ
            DisableGlider();
        }

        if (isGliding)
        {
            ApplyGliderMovement(moveHorizontal,moveVertical);
        }
        else
        {
            //�ӵ������� ���� �̵�
            rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);
            //���� ���� ���� ����
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        //�ӵ������� ���� �̵�
        rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);
        //���� ���� ���� ����
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
            Debug.Log($"���� ���� : {coinCount}/{totalCoins}");
        }

        if(other.CompareTag("Door")&&coinCount>=totalCoins)
        {
            Debug.Log("���� Ŭ����");
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
    void EnableGlider() //�۶��̴� Ȱ��ȭ
    {
        isGliding = true;
        if (gliderObject != null)
        {
            gliderObject.SetActive(true);
        }

        rb.velocity=new Vector3(rb.velocity.x,gliderFallspeed,rb.velocity.z);
    }

    void DisableGlider() //�۶��̴� ��Ȱ��ȭ
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
