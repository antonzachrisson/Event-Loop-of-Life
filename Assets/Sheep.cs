using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    private enum Action { Evade, Eat, Breed, Find, Wander };
    Action action;

    public class DistsC : System.IComparable
    {
        public float dist;
        public GameObject obj;

        public int CompareTo(object obj_)
        {
            DistsC otherClass = (DistsC)obj_;

            return this.dist.CompareTo(otherClass.dist);
        }
    }

    private Main main_;
    private DistsC[] grassDists;
    private DistsC[] wolfDists;
    private GameObject eatTarget;
    private float timeLeft;
    private float currentHealth;
    private float breedCD;
    private Renderer rend;
    private float senseRange;
    private float moveSpeed;
    private float maxHealth;
    private float healthGain;
    private int frameInterval;

    public GameObject sheep;
    
    // Start is called before the first frame update
    void Start()
    {
        action = Action.Wander;
        main_ = GameObject.Find("Main").GetComponent<Main>();
        timeLeft = 2f;
        maxHealth = 100f;
        currentHealth = maxHealth / 2f;
        rend = GetComponent<Renderer>();
        breedCD = 0f;
        senseRange = 10f;
        moveSpeed = 1.35f;
        healthGain = 20f;
        frameInterval = 10;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Timer();
        frameInterval++;
        if (frameInterval > 10)
        {
            Sense();
            Decide();
            frameInterval = 0;
        }
        Act();
    }

    private void Sense()
    {
        List<GameObject> wolfs_ = main_.getList("wolf");
        List<GameObject> grass_ = main_.getList("grass");

        wolfDists = new DistsC[wolfs_.Count];
        grassDists = new DistsC[grass_.Count];

        for (int i = 0; i < grass_.Count; i++)
        {
            grassDists[i] = new DistsC();
            grassDists[i].dist = Vector3.Distance(this.transform.position, grass_[i].transform.position);
            grassDists[i].obj = grass_[i];
        }

        for (int i = 0; i < wolfs_.Count; i++)
        {
            wolfDists[i] = new DistsC();
            wolfDists[i].dist = Vector3.Distance(this.transform.position, wolfs_[i].transform.position);
            wolfDists[i].obj = wolfs_[i];
        }
        System.Array.Sort(grassDists);
        System.Array.Sort(wolfDists);
    }

    private void Decide()
    {
        if (wolfDists.Length > 0 && wolfDists[0].dist <= senseRange)
        {
            action = Action.Evade;
        }
        else
        {
            if (grassDists.Length > 0 && currentHealth < maxHealth)
            {
                if (grassDists[0].dist < 1f)
                    action = Action.Eat;

                else if (grassDists[0].dist <= senseRange)
                    action = Action.Find;

                else
                    action = Action.Wander;
            }

            else if (currentHealth >= maxHealth && breedCD <= 0f)
            {
                action = Action.Breed;
            }

            else
                action = Action.Wander;
        }
    }

    private void Timer()
    {
        if (breedCD > 0f)
            breedCD -= Time.deltaTime;
        if (breedCD < 0f)
            breedCD = 0f;
    }

    public void Die()
    {
        main_.remove(gameObject, "sheep");
        Destroy(gameObject);
    }

    private void Act()
    {
        switch (action)
        {
            case Action.Evade:
                rend.material.SetColor("_Color", Color.blue);

                Vector3 newDir = new Vector3(0f, 0f, 0f);
                for (int i = 0; i < wolfDists.Length; i++)
                {
                    newDir += -(wolfDists[i].obj.transform.position - transform.position).normalized;
                }
                newDir = newDir.normalized;
                transform.rotation = Quaternion.LookRotation(newDir);
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
                break;

            case Action.Eat:
                eatTarget = grassDists[0].obj;
                Grass temp = eatTarget.GetComponent<Grass>();
                temp.Die();
                currentHealth += healthGain;
                action = Action.Wander;
                break;

            case Action.Breed:
                breedCD = 8f;
                currentHealth = maxHealth / 5f;
                main_.addSheep(sheep, transform.position.x + 1, transform.position.y, transform.position.z);
                action = Action.Wander;
                break;

            case Action.Find:
                if (grassDists[0].obj == null)
                    break;

                float step = moveSpeed * Time.deltaTime;

                Vector2 pos2 = new Vector2(transform.position.x, transform.position.z);
                Vector2 grassPos2 = new Vector2(grassDists[0].obj.transform.position.x, grassDists[0].obj.transform.position.z);
                Vector2 newPos2 = Vector2.MoveTowards(pos2, grassPos2, step);
                float Y = transform.position.y;

                Vector3 relativePos = new Vector3(grassDists[0].obj.transform.position.x, Y, grassDists[0].obj.transform.position.z) - transform.position;
                Quaternion rot = Quaternion.LookRotation(relativePos, Vector3.up);
                transform.rotation = rot;
                transform.position = new Vector3(newPos2.x, Y, newPos2.y);

                rend.material.SetColor("_Color", Color.green);
                break;
            
            case Action.Wander:
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0f)
                {
                    Quaternion randRot = Quaternion.Euler(0, Random.Range(0, 259), 0);
                    transform.rotation = randRot;
                    timeLeft = 2f;
                }
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
                rend.material.SetColor("_Color", Color.white);
                break;
        }

        currentHealth -= 0.05f;
        if (currentHealth <= 0f)
            Die();
    }
}
