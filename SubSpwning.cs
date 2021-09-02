using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSpwning : MonoBehaviour {
    [Header("Subs")]
    [SerializeField] private GameObject self;
    [SerializeField] private GameObject def;
    [SerializeField] private GameObject atk;
    [SerializeField] private GameObject spwnPt;

    private Movement2D playerMove;
    private MovementSub defMoveScript;
    private MovementSub atkMoveScript;

    [SerializeField] private bool defSpwned = false;
    [SerializeField] private bool atkSpwned = false;

    [SerializeField] private bool defNear = false;
    [SerializeField] private bool atkNear = false;

    private GameObject inRangeDef;
    private GameObject inRangeAtk;

    public float maxRange = 100;


    void Start() {
        playerMove = GetComponent<Movement2D>();
        self = gameObject;
        defMoveScript = def.GetComponent<MovementSub>();
        defMoveScript.isAtk = false;
        atkMoveScript = atk.GetComponent<MovementSub>();
        atkMoveScript.isAtk = true;



        //change so that only one sub can be spawned at once. Add max range for the sub. Despawn at infinite ranger with animation

    }

    // Update is called once per frame
    void Update() {

        if (defSpwned || atkSpwned) {

            playerCanMove(false);
        } else {
            playerCanMove(true);
        }

        if (Input.GetButtonDown("SpwnDef") && !defSpwned) {
            defSpwned = true;
            createSub(def);

        } else if (Input.GetButtonDown("SpwnDef") && defSpwned) {
            if (defNear) {

                Destroy(inRangeDef);
                defSpwned = false;
            }

        }

        if (Input.GetButtonDown("SpwnAtk") && !atkSpwned) {
            atkSpwned = true;
            createSub(atk);

        } else if (Input.GetButtonDown("SpwnAtk") && atkSpwned) {
            if (atkNear) {
                Destroy(inRangeAtk);
                atkSpwned = false;
            }
        }

    }


    /*
        private void OnTriggerEnter2D(Collider2D col) {

            if (col.gameObject.CompareTag("PlayerSub")) {
                MovementSub temp = col.gameObject.GetComponent<MovementSub>();
                if (temp.isAtk && atkSpwned) {
                    atkNear = true;
                } else if (!temp.isAtk && defSpwned) {
                    defNear = true;
                }


            }
        }*/


    private void OnTriggerStay2D(Collider2D col) {

        if (col.gameObject.CompareTag("PlayerSub")) {
            MovementSub temp = col.gameObject.GetComponent<MovementSub>();
            if (temp.isAtk && atkSpwned) {
                atkNear = true;

                inRangeAtk = col.gameObject;
            } else if (!temp.isAtk && defSpwned) {
                defNear = true;

                inRangeDef = col.gameObject;
            }
        } else {
            atkNear = false;
            defNear = false;
            inRangeAtk = null;
            inRangeDef = null;
        }

    }

    /*  private void OnTriggerExit2D(Collider2D col) {
          if (col.gameObject.CompareTag("PlayerSub")) {
              MovementSub temp = col.gameObject.GetComponent<MovementSub>();
              if (temp.isAtk && atkSpwned) {
                  atkNear = false;
              } else if (!temp.isAtk && defSpwned) {
                  defNear = false;
              }


          }
      }*/

    void playerCanMove(bool b) {
        playerMove.canMove = b;
    }

    void createSub(GameObject obj) {
        Instantiate(obj, spwnPt.gameObject.transform.position, Quaternion.identity);
        DrawLine temp = obj.GetComponent<DrawLine>();
        temp.player = gameObject;
    }
        
}
