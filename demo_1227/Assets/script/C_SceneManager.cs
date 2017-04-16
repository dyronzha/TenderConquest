﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SceneManager : MonoBehaviour {

    public static C_SceneManager SceneManger;
    public int i_save_point;
    public GameObject  O_show_plate21, O_enemy,O_plate10, O_show_plate11,O_record3,O_step_ice,O_step_enemy,O_barrier;
    GameObject O_camera;
    bool b_enemy_switch;

	// Use this for initialization
	void Awake () {
        SceneManger = this;
        i_save_point = 0;
        O_camera = GameObject.Find("Main Camera");
        b_enemy_switch = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (O_enemy == null&&!b_enemy_switch) {
            O_plate10.SetActive(true);
            O_show_plate21.SetActive(true);
            O_show_plate11.SetActive(true);
            O_record3.SetActive(true);
            b_enemy_switch = true;
        } 
	}

    void ChangeSavePoint() {
        i_save_point++;
        if (i_save_point == 2)
        {
            Destroy(O_barrier);
        }
        else if (i_save_point == 3) {
           
        }
    }

    void OnDetect() {
        if (i_save_point == 2)
        {
           
        }
        else if (i_save_point == 3) {
            O_camera.SendMessage("SetScreen", new Vector3(81.8f,17.5f,-10.0f));
            this.Invoke("step_enemy", 3.0f);
        }
        
    }

    void step_enemy() {
        O_step_enemy.SendMessage("change",new Vector3(82.3f,12.9f,0.5f));
        O_step_ice.SendMessage("change", new Vector3(80.0f,13.0f,0.5f));
        O_camera.GetComponent<C_CameraFollow>().Invoke("reset",1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

}
