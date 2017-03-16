using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_EnemyBase : MonoBehaviour {
    Rigidbody2D enemy_body;
    public LayerMask mask;
    Vector3 respawn_location_vec3;
    Animator enemy_animator;
    Transform t_attackarea;
    RaycastHit2D ray_seeplayer;
    GameObject player;
    CircleCollider2D hit_area;
    bool b_toofar,b_attack = false;
    public int i_mode;
    float f_distance, f_ramble_left, f_ramble_right;
    public float f_ramble_dis;

    // Use this for initialization
    void Awake()
    {
        enemy_body = gameObject.GetComponent<Rigidbody2D>();
        respawn_location_vec3 = transform.position;
        enemy_animator = gameObject.GetComponent<Animator>();
        t_attackarea = gameObject.transform.GetChild(1);
        player = GameObject.Find("player");
        hit_area = gameObject.GetComponent<CircleCollider2D>();
        i_mode = 0;
        f_ramble_left = respawn_location_vec3.x - f_ramble_dis;
        f_ramble_right = respawn_location_vec3.x + f_ramble_right;
    }

    // Update is called once per frame
    void Update()
    {
        sight();
        //attackjudgement(ray_seeplayer);

        if (!seePlay()) {
            
        }
        else behaviorMode();

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("change");
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
        }
        if (Input.GetMouseButtonDown(0)) Debug.Log(mask);
    }

    //追逐視野內玩家
    void sight()
    {
        ray_seeplayer = Physics2D.Raycast(transform.position, ((-transform.right) * transform.localScale.x + transform.up * 0.3f), 6.0f,mask);
        Debug.DrawLine(transform.position, transform.position + (Vector3)((-transform.right * transform.localScale.x) + transform.up * 0.3f) * 6.0f);
        Vector3 walkto_vec3, record_vec3;
         f_distance = Vector3.Distance(transform.position, respawn_location_vec3);
        if (f_distance > 6.0f) b_toofar = true;
        if (ray_seeplayer)
        {
            if (!b_toofar)
            {
                walkto_vec3 = new Vector3(ray_seeplayer.transform.position.x - transform.position.x, 0, 0).normalized;
                if (Mathf.Abs((ray_seeplayer.transform.position.x - transform.position.x)) < 1.8f)
                {
                    enemy_animator.Play("EnemyAttack");
                    Debug.Log("attack");
                    return;
                }
                enemy_body.velocity = walkto_vec3 * 3.0f;
            }
            else
            {
                walkto_vec3 = new Vector3(respawn_location_vec3.x - transform.position.x, 0, 0).normalized;
                record_vec3 = walkto_vec3;
            }
        }
        else
        {
            //walkto_vec3 = Vector3.zero;
            b_toofar = false;
            if (Mathf.Abs((f_distance)) < 1.0f)
            {
                enemy_body.velocity = Vector2.zero;
                walkto_vec3 = Vector3.zero;
            }
            else walkto_vec3 = new Vector3(respawn_location_vec3.x - transform.position.x, 0, 0).normalized;
        }
        enemy_body.velocity = walkto_vec3 * 3.0f;
    }

    bool seePlay()
    {
        ray_seeplayer = Physics2D.Raycast(transform.position, ((-transform.right) * transform.localScale.x + transform.up * 0.3f), 6.0f, mask);
        Debug.DrawLine(transform.position, transform.position + (Vector3)((-transform.right * transform.localScale.x) + transform.up * 0.3f) * 6.0f);
        Vector3 walkto_vec3, record_vec3;
        f_distance = Vector3.Distance(transform.position, respawn_location_vec3);
        if (f_distance > 6.0f) b_toofar = true;
        if (ray_seeplayer)
        {
            if (!b_toofar)
            {
                walkto_vec3 = new Vector3(ray_seeplayer.transform.position.x - transform.position.x, 0, 0).normalized;
                if (Mathf.Abs((ray_seeplayer.transform.position.x - transform.position.x)) < 1.8f)
                {
                    enemy_animator.Play("EnemyAttack");
                    Debug.Log("attack");
                    return (true);
                }
                enemy_body.velocity = walkto_vec3 * 3.0f;
                return true;
            }
            else return false;
        }
        else {
            return false;
        }
    }

    void behaviorMode() {
        Vector3 walkto_vec3;
        switch (i_mode) {
            case 0:
                if (Vector2.Distance(respawn_location_vec3, transform.position) > 0.1f)
                {
                    walkto_vec3 = (respawn_location_vec3 - transform.position).normalized;
                }
                else walkto_vec3 = Vector3.zero;
                break;

            case 1:
                if (transform.position.x > f_ramble_left)
                {
                    walkto_vec3 = new Vector3(transform.position.x - f_ramble_left, 0, 0);
                }
                else if (transform.position.x < f_ramble_right)
                {
                    walkto_vec3 = new Vector3(f_ramble_right - transform.position.x, 0, 0);
                }
                else {
                    
                }
                break;
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
