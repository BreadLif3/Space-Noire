using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControls : MonoBehaviour
{
    public float speed=5f;
    public float turnSmoothTime = 0.1f;
    public float jumpSpeed;

    public Rigidbody rb;

    float turnSmoothVelocity;
    public CharacterController controller;
    public Transform cam; 
    private float ySpeed;
    private float originalStepOffset;
    private bool haveShoes=false;
    private bool haveGlider=false;


  void Start(){
        rb= GetComponent<Rigidbody>();
        originalStepOffset=controller.stepOffset;
    }

    // Update is called once per frame

    void Update(){
        float moveHorizontal = Input.GetAxis("Horizontal")* Time.deltaTime*speed;
        float moveVertical = Input.GetAxis("Vertical")* Time.deltaTime*speed;

        Vector3 movement = new  Vector3(moveHorizontal, 0f, moveVertical).normalized;
        
        ySpeed += Physics.gravity.y*Time.deltaTime;

        if (controller.isGrounded){
            ySpeed= -0.5f;
            if (Input.GetButtonDown("Jump")){
                ySpeed=jumpSpeed;
            }
        }
        else{
            controller.stepOffset= originalStepOffset;
        }

        Vector3 velocity= movement * movement.magnitude;
        velocity.y= ySpeed;
        controller.Move(velocity* Time.deltaTime);

        if (movement.magnitude >= 0.1f) // all for movement adjusting to camera
        {
            float targetAngle=Mathf.Atan2(movement.x,movement.z)* Mathf.Rad2Deg + cam.eulerAngles.y;// used to rotate the character towards way hes walking
            float angle=Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); // smoothen numbers
            transform.rotation=Quaternion.Euler(0f, targetAngle, 0f);
            
            Vector3 moveDir= Quaternion.Euler(0f, targetAngle, 0f) *Vector3.forward; // redirects forward the way the camera is facing 
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftShift)){
            if (haveGlider==true){
                rb.useGravity=false;
                float targetAngle=Mathf.Atan2(movement.x,movement.z)* Mathf.Rad2Deg + cam.eulerAngles.y;// used to rotate the character towards way hes walking
                Vector3 jumpExtender= Quaternion.Euler(0f, targetAngle, 0f) * new Vector3 (0f,0f,20f); 
                controller.Move(jumpExtender.normalized * speed * Time.deltaTime);
            }
        }
       else{
            rb.useGravity=true;   
       } 

        RaycastHit hit;
        Ray wallDetection=new Ray(transform.position, transform.forward);
        if (Physics.Raycast(wallDetection, out hit, 1)){
            if (hit.collider.tag=="Climbable" && haveShoes==true){
                if (Input.GetKey(KeyCode.W)){
                    Vector3 climb=new Vector3(0,0.85f,0);
                    transform.position+=climb;
                }
            }
        }
    }
     void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("item")){ 
            other.gameObject.SetActive(false);
            haveShoes=true;
        }

        if (other.gameObject.CompareTag("Glider")){ 
            other.gameObject.SetActive(false);
            haveGlider=true;
        }
    }
}
