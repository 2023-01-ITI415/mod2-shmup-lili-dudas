using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S {get; private set;}//Singleton property

    [Header("Inscribed")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    [Header("Dynamic")] [Range(0,4)] [SerializeField]
    private float _shieldLevel = 1;
    [Tooltip( "This field holds a reference to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;

    void Awake()
    {
        if (S == null){
            S = this; //set the singleton only if it's null
        }
        else{
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }    
    }

    // Update is called once per frame
    void Update()
    {
        //pull in information from the Input class
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        //change transform.position based on axes
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        //rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(vAxis*pitchMult,hAxis*rollMult,0);
    }

    void onTriggerEnter(Collider other) {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        
        //make sure it's not the same triggering go as last time
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null) {
            shieldLevel --; //decrease the level of the shield by 1
            Destroy(go); //and destroy the enemy
        }
        else{
            Debug.LogWarning("Shield trigger hit by non-Enemy: "+ go.name);
        }
    }

    public float shieldLevel {
        get { return ( _shieldLevel);}
        private set {
            _shieldLevel = Mathf.Min(value, 4);
            //if the shield is going to be set to less than zero
            if (value < 0){
                Destroy(this.gameObject); //destroy the hero
                Main.HERO_DIED();
            }
        }
    }
}
