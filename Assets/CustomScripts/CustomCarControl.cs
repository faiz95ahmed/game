using System;
using UnityEngine;
using UnityEngine.UI;

using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof (CarController))]
public class CustomCarControl : MonoBehaviour {
    public GameManager gm;
    public Rigidbody rb;
    private CarController m_Car; // the car controller we want to use

    public int updateEvery = 0;
    public float radius = 10;

    public Text speedText;
    //danger monitors if an object is nearby within an angle of +-15 deg in front of the car
    public bool danger = false;

    bool waitingForCommands = false;
    PlayerCommands cmds;

    int frames = 0;
    int lastUpdate = 0;

    private void Start() {
        if(gm == null) {
            Debug.Log("Got no gm!");
        }
    }

    private void Awake() {
        // get the car controller
        m_Car = GetComponent<CarController>();
    }

    private void FixedUpdate() {
        frames++;
        if (gm.inter.HasCommands()) {
            waitingForCommands = false;
            cmds = gm.inter.GetCommands();
        }

        float h = Convert.ToSingle(cmds.right) - Convert.ToSingle(cmds.left);
        float v = Convert.ToSingle(cmds.forward) - Convert.ToSingle(cmds.backward);
        // We can also have handbrake controls if we want them:
        float handbrake = 0f;
        m_Car.Move(h, v, v, handbrake);

        if (!waitingForCommands && frames >= lastUpdate + updateEvery) {
            lastUpdate = frames;

            //radius of collision warning
            Collider[] nearby = Physics.OverlapSphere(rb.position, radius);
            danger = false;

            for (int i = 0; i < nearby.Length; i++) {
                float angle = Vector3.Angle(nearby[i].ClosestPointOnBounds(rb.position) - rb.position, transform.forward);

                if (angle <= 15f) {
                    danger = true;
                }
            }
            update_speed();

            PlayerData data;
            data.speed = rb.velocity.magnitude;
            data.danger = danger;
            gm.inter.NewData(data);
            waitingForCommands = true;
        }
    }

    public void EndLevel() {
        EndLevelData data;
        data.time = Mathf.RoundToInt(frames);// * Time.fixedDeltaTime);
        gm.inter.EndLevel(data);
    }

    void update_speed (){
        speedText.text = (rb.velocity.magnitude).ToString() + " m/s " + (danger ? "danger" : "");
    }
}