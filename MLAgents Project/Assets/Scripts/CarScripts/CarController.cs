using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CarController : MonoBehaviour
{
    
    public bool PlayerUseButtonsON;

    //public float EnginePower = 100;

    [SerializeField]
    private float Speed;
    public float SpeedAdder = 8;
    public float SpeedRemover = 5;
    public float MaxSpeed = 25;  // (max forward speed)
    public float MinSpeed = -10;  // (max backward speed)

    public float TurnSpeed = 100;


    private float horizontalInput;
    private float forwardInput;

    private Rigidbody carRigidbody;



    public TextMeshProUGUI speedText;

    private AudioSource audioSourceEngine;
    private AudioSource audioSourceTurn;
    public AudioClip CarCrashAudioClip;

    private CarTireTrailScript carTireTrail;




    [SerializeField]
    private float CarEngineSoundMin = 0.1f;
    [SerializeField]
    private float CarEngineSoundMax = 0.4f;
    [SerializeField]
    private float CarTurnSoundMax = 0.4f;



    void Start()
    {
        audioSourceEngine = GetComponents<AudioSource>()[0];
        audioSourceTurn = GetComponents<AudioSource>()[1];
        carRigidbody = transform.GetComponent<Rigidbody>();
        carTireTrail = transform.GetComponent<CarTireTrailScript>();
        StartReset();
        Application.targetFrameRate = (int)( 1 / Time.fixedTime);
    }




    public void StartReset()
    {
        audioSourceEngine.volume = CarEngineSoundMin;
        speedText.text = "0";
        Speed = 0;
        transform.position = new Vector3(Random.Range(-7, -5), 0, Random.Range(14, 20));
        transform.rotation = Quaternion.Euler(0, -90, 0);
    }




    void Update()
    {

        Vector3 direction = transform.forward * forwardInput;
        direction.y = 0;
        direction.Normalize();

        Vector3 final = transform.forward;
        final.y = 0;
        transform.forward = final.normalized;

        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");        

        //  player movement from buttons (if turned on)
        if (PlayerUseButtonsON)
        {
            if (forwardInput > 0) accelerate();
            else if (forwardInput < 0) brake();
            else applyFriction();            

            if (horizontalInput > 0) turnRight();
            else if (horizontalInput < 0) turnLeft();
        }

        speedText.text = Speed.ToString();
        audioSourceEngine.volume = CarEngineSoundMin + (CarEngineSoundMax-CarEngineSoundMin) * System.Math.Abs(Speed) / MaxSpeed ;

        if (audioSourceTurn.volume >= 0.1f)
        {
            audioSourceTurn.volume -= 0.1f;

            if (audioSourceTurn.volume > CarTurnSoundMax - 0.1f)
            {
                carTireTrail.startEmitting();
            }
            else
            {
                carTireTrail.stopEmitting();
            }
        }

    }

    private void FixedUpdate()
    {
        carRigidbody.Move(transform.position + transform.forward * Speed * Time.deltaTime, transform.rotation);
    }




    public void turnRight()
    {
        transform.Rotate(Vector3.up, TurnSpeed * Time.deltaTime, Space.World);
        if (audioSourceTurn.volume <= CarTurnSoundMax)
        {
            audioSourceTurn.volume += 0.1f;
        }
    }


    public void turnLeft()
    {
        transform.Rotate(Vector3.up, - TurnSpeed * Time.deltaTime, Space.World);
        if (audioSourceTurn.volume <= CarTurnSoundMax)
        {
            audioSourceTurn.volume += 0.1f;
        }
    }




    public void accelerate()
    {
        if (Speed < MaxSpeed - SpeedAdder)
        {
            Speed += SpeedAdder * Time.deltaTime;
        }
        else
        {
            Speed = MaxSpeed;
        }
    }


    public void brake()
    {
        if (Speed > MinSpeed + SpeedAdder)
        {
            Speed -= SpeedAdder * Time.deltaTime;
        }
        else
        {
            Speed = MinSpeed;
        }
    }


    public void applyFriction()
    {
        //  reduce absolute value of speed bcs of road friction
        if (Speed > 0)
        {
            if (Speed > SpeedRemover) Speed -= SpeedRemover * Time.deltaTime;
            else Speed = 0;
        }
        else if (Speed < 0)
        {
            if (-Speed > SpeedRemover) Speed += SpeedRemover * Time.deltaTime;
            else Speed = 0;
        }
    }






    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("wall"))
        {
            Speed = Speed / 3;

            audioSourceTurn.PlayOneShot(CarCrashAudioClip, 1.0f);


            //Debug.Log(Vector3.Angle(-collision.contacts[0].normal, carDir));

            if (Speed > 5)
            {
                Vector3 fwd = transform.forward;
                Vector3 carDir = transform.forward * Mathf.Sign(Speed);

                Vector3 reflection = Vector3.Reflect(carDir, -collision.contacts[0].normal).normalized;

                transform.LookAt(transform.position + reflection);


                if (Vector3.Angle(-collision.contacts[0].normal, carDir) < 45)
                {
                    //  If angle is facing towards the wall then set original rotation and lower speed
                    Speed = Speed / 5;
                    transform.LookAt(transform.position + fwd);
                }
                else if (Speed < 0)
                {
                    transform.Rotate(0, 180, 0);
                }
            }

            //Debug.DrawRay(transform.position + Vector3.up * 0.9f, carDir * 5, Color.green, 2);
            //Debug.DrawRay(transform.position + Vector3.up * 0.9f, -collision.contacts[0].normal * 5, Color.blue, 2);
            //Debug.DrawRay(transform.position + Vector3.up * 1.1f, reflection * 5, Color.magenta, 2);
        }

    }



    public float getSpeed()
    {
        return Speed;
    }



}


