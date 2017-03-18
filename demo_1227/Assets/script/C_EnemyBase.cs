using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_EnemyBase : MonoBehaviour {
    Rigidbody2D enemy_body;
    public LayerMask mask;
    Vector3 respawn_location_vec3;
    Animator enemy_animator;
    Transform t_attackarea, t_detect;
    RaycastHit2D ray_seeplayer,ray_detect;
    GameObject player;
    CircleCollider2D hit_area;
    bool b_toofar,b_attack, b_to_right = false;
    public int i_mode;
    float f_distance, f_ramble_left, f_ramble_right, f_ramble_wait;
    public float f_ramble_dis, f_speed, f_sight_dis,f_face_way;
    bool b_see_it,b_ramble_return;
    // Use this for initialization
    void Awake()
    {
        enemy_body = gameObject.GetComponent<Rigidbody2D>();
        respawn_location_vec3 = transform.position;
        enemy_animator = gameObject.GetComponent<Animator>();
        t_attackarea = gameObject.transform.GetChild(1);
        t_detect = gameObject.transform.GetChild(2);
        player = GameObject.Find("player");
        hit_area = gameObject.GetComponent<CircleCollider2D>();
        f_ramble_left = respawn_location_vec3.x - f_ramble_dis;
        f_ramble_right = respawn_location_vec3.x + f_ramble_dis;
        b_see_it = b_ramble_return = false;
        f_face_way = transform.localScale.x;
        f_ramble_wait = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!seePlay()) {
            behaviorMode();
        }
    }

    //追逐視野內玩家
    bool seePlay()
    {
        if (!b_see_it) b_see_it = ray_seeplayer = Physics2D.Raycast(transform.position, (new Vector3(-1 * transform.localScale.x, 0, 0) + transform.up * 0.3f), 6.0f, mask);
        Debug.DrawLine(transform.position, transform.position + (new Vector3(-1 * transform.localScale.x, 0, 0) + transform.up*0.3f).normalized * 6.0f);
        Vector3 walkto_vec3;
        ray_detect = Physics2D.Linecast(transform.position, t_detect.transform.position, 1 << LayerMask.NameToLayer("ground"));
        f_distance = Mathf.Abs(transform.position.x - respawn_location_vec3.x) ;
        if (f_distance > f_sight_dis) b_toofar = true;
        else b_toofar = false;
        if (b_see_it && ray_detect)
        {
            if (!b_toofar)
            {
                walkto_vec3 = new Vector3(ray_seeplayer.transform.position.x - transform.position.x, 0, 0);
                transform.localScale = new Vector3(1.0f * Mathf.Sign(transform.position.x - ray_seeplayer.transform.position.x), 1, 1);
                if (Mathf.Abs((ray_seeplayer.transform.position.x - transform.position.x)) < 1.5f)
                {
                    enemy_body.velocity = new Vector3(0, 0, 0);
                    enemy_animator.Play("EnemyAttack");
                    Debug.Log("attack");
                    return (true);
                }
                enemy_body.velocity = new Vector3(walkto_vec3.normalized.x * f_speed*1.4f, enemy_body.velocity.y,0);
                Debug.Log(transform.localScale + "yux" + walkto_vec3.normalized * f_speed * 1.4f);
                return true;
            }
            else {
                transform.localScale = new Vector3(1.0f *Mathf.Sign(transform.position.x - respawn_location_vec3.x) ,1,1);
                b_see_it = false;
                return false;
            } 
        }
        else {
            b_see_it = false;
            return false;
        }
    }

    void behaviorMode() {
        Vector3 walkto_vec3;
        Vector3 pos_vec3 = transform.position;
        switch (i_mode) {
            case 0:
                if (Mathf.Abs(respawn_location_vec3.x - pos_vec3.x) >0.5f)
                {
                    if (!Wait(ref f_ramble_wait, 1.2f)) {
                        enemy_body.velocity = new Vector3(0, enemy_body.velocity.y, 0);
                        return;
                    } 
                    transform.localScale = new Vector3(Mathf.Sign(pos_vec3.x - respawn_location_vec3.x) ,1,1);
                    walkto_vec3 = new Vector3(f_face_way, 0, 0);
                }
                else {
                    f_ramble_wait = 0.0f;
                    transform.localScale = new Vector3(f_face_way, 1, 1);
                    walkto_vec3 = Vector3.zero;
                } 
                enemy_body.velocity = new Vector3(walkto_vec3.normalized.x * f_speed, enemy_body.velocity.y, 0);
                break;

            case 1:
                if (pos_vec3.x < f_ramble_left)
                {
                    if (!(Wait(ref f_ramble_wait, 1.5f))) {
                        enemy_body.velocity = new Vector3(0,enemy_body.velocity.y,0);
                        return;
                    }
                    b_ramble_return = true;
                    transform.localScale = new Vector3(-1, 1, 1);
                    walkto_vec3 = new Vector3(1 , 0, 0);
                    b_to_right = true;
                }
                else if (pos_vec3.x > f_ramble_right)
                {
                    if ((!Wait(ref f_ramble_wait, 1.5f))) {
                        enemy_body.velocity = new Vector3(0,enemy_body.velocity.y,0);
                        return;
                    }
                    transform.localScale = new Vector3(1, 1, 1);
                    walkto_vec3 = new Vector3(-1, 0, 0);
                    b_to_right = false;
                }
                else {
                    f_ramble_wait = 00f;
                    if (b_to_right) {
                        walkto_vec3 = new Vector3(1, 0, 0);
                    }
                    else walkto_vec3 = new Vector3(-1, 0, 0);
                }
                enemy_body.velocity = new Vector3(walkto_vec3.normalized.x * f_speed, enemy_body.velocity.y, 0);
                break;
        }
    }

    bool Wait(ref float f_current_time, float f_total_time ) {
        if ( f_current_time < f_total_time)
        {
            f_current_time += Time.deltaTime;
            Debug.Log("Wait" + f_current_time);
            return false;
        }
        else {
            Debug.Log("done" + f_current_time);
            return true;
        } 
        
    }

    //攻擊範圍
    private bool attackjudgement(RaycastHit2D target)
    {
        if (!target) return false;
        float top, right, bottom, left;
        Vector3 right_t, right_b, left_t, left_b, playerpos;
        right_t = t_attackarea.transform.position + 3.5f * transform.up + 1.0f * transform.right;
        right_b = t_attackarea.transform.position + -0.3f * transform.up + 1.0f * transform.right;
        left_t = t_attackarea.transform.position + 3.5f * transform.up + -0.7f * transform.right;
        left_b = t_attackarea.transform.position + -0.3f * transform.up + -0.7f * transform.right;
        playerpos = target.transform.position;
        Debug.DrawLine(right_t, left_t, Color.white);
        Debug.DrawLine(right_t, right_b, Color.white);
        Debug.DrawLine(left_t, left_b, Color.white);
        Debug.DrawLine(right_b, left_b, Color.white);
        top = multiarea(playerpos, right_t, left_t);
        right = multiarea(playerpos, right_t, right_b);
        bottom = multiarea(playerpos, right_b, left_b);
        left = multiarea(playerpos, left_t, left_b);
        if (top * bottom <= 0 && right * left <= 0)
        {
            return true;
        }
        else
        {
            Debug.Log("enter but");
            return false;
        }
    }

    public void Attackarea(){
        hit_area.enabled = true;
        if (b_attack)
        {
            Debug.Log("success");
        }
        else Debug.Log("miss");
    }

    //四邊形面積
    private float multiarea(Vector3 point, Vector3 ver1, Vector3 ver2)
    {
        Vector3 tri1, tri2;
        float area;
        tri1 = ver1 - point;
        tri2 = ver2 - point;
        area = (tri1.x * tri2.y - tri1.y * tri2.x);
        return (area);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            b_attack = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            b_attack = false;
        }
    }
}
