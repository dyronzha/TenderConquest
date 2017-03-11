using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_FarBackground : MonoBehaviour {

    float bk_f_x,bk_f_y;
    GameObject player;
    float limit_r, limit_l, player_r, player_l,trans_value;
    float limit_t, limit_b, player_t, player_b,trans_value_y;
    // Use this for initialization
    void Awake () {
        player = GameObject.Find("Player");
        limit_l = 15.5f; limit_r = 34.5f; player_l = 14.5f; player_r = 36.5f;
        limit_t = 16.5f; limit_b = 13.5f; player_t = 22.65f; player_b = 18.5f;
        trans_value = (limit_r - limit_l)/(player_r - player_l);
        bk_f_x = limit_l;
        bk_f_y = limit_b;
        trans_value_y = (limit_t - limit_b) / (player_t - player_b);
    }
	
	// Update is called once per frame
	void Update () {
        FarBackgroundMove();
	}

    void FarBackgroundMove()
    {
        if (player.transform.position.x <= player_l) bk_f_x = limit_l;
        else if (player.transform.position.x >= player_r) bk_f_x = limit_r;
        else
        {
                bk_f_x = limit_l + (player.transform.position.x - player_l) * trans_value;
        }

        if (player.transform.position.y <= player_b) bk_f_y = limit_b;
        else if (player.transform.position.y >= player_t) bk_f_y = limit_t;
        else
        {
            if(player.transform.localScale.y>0)bk_f_y = limit_b + (player.transform.position.y - player_b) * trans_value_y;
        }
        transform.position = new Vector3(bk_f_x, bk_f_y, transform.position.z);
    }
}
