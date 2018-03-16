﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    public Rigidbody rb;
    public float drive_force;
    bool waitingForCommands = false;
    int frames = 0;
    public Text speedText;
    //danger monitors if an object is nearby within an angle of +-15 deg in front of the car
    public bool danger = false;

    GameManager gm;

    // Use this for initialization
    void Start() {
        drive_force *= Time.fixedDeltaTime;
        update_speed();

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gm == null) {
            Debug.Log("Cannot find GM");
        }
    }

    void FixedUpdate() {
        frames++;
        if (gm.inter.HasCommands()) {
            waitingForCommands = false;

            PlayerCommands cmds = gm.inter.GetCommands();

            if (cmds.forward) {
                rb.AddForce(drive_force*Vector3.forward);
            }
            if (cmds.backward) {
                rb.AddForce(drive_force*Vector3.back);
            }
            if (cmds.left) {
                rb.AddForce(drive_force*Vector3.left);
            }
            if (cmds.right) {
                rb.AddForce(drive_force*Vector3.right);
            }

            //radius of collision warning
            float radius = 10;
            Collider[] nearby = Physics.OverlapSphere(rb.position, radius);
            danger = false;

            for (int i = 0; i < nearby.Length; i++) {
                float angle = Vector3.Angle(nearby[i].ClosestPointOnBounds(rb.position) - rb.position, Vector3.forward);

                if (angle <= 15f) {
                    danger = true;
                }
            }
            update_speed();
        }

        if (!waitingForCommands) {
            PlayerData data;
            data.drive_force = drive_force;
            gm.inter.NewData(data);
            waitingForCommands = true;
        }
    }

    public void EndLevel() {
        EndLevelData data;
        data.time = Mathf.RoundToInt(frames * Time.fixedDeltaTime);
        gm.inter.EndLevel(data);
    }

    void update_speed (){
        speedText.text = (rb.velocity.magnitude).ToString() + " m/s " + (danger ? "danger" : "");
    }
}
