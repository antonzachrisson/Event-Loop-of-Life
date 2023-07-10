using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    private enum Action {Eat, Breed, Pursue, Wander };
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

    private Renderer rend;
    private Main main_;
    private GameObject eatTarget;
    private DistsC[] sheepDists;

    public GameObject wolf;

    private float timeLeft;
    private float currentHealth;
    private float breedCD;
    private float huntCD;
    private float moveSpeed;
    private float chaseSpeed;
    private float senseRange;
    private float maxHealth;
    private float healthGain;
    private int frameInterval;

    // Start is called before the first frame update
    void Start()
    {
        action = Action.Wander;
        rend = GetComponent<Renderer>();
        main_ = GameObject.Find("Main").GetComponent<Main>();
        rend.material.SetColor("_Color", Color.black);
        timeLeft = 2f;
        maxHealth = 200f;
        currentHealth = maxHealth / 2f;
        breedCD = 5f;
        huntCD = 3f;
        moveSpeed = 1f;
        chaseSpeed = 4f;
        senseRange = 30f;
        healthGain = 100f;
        frameInterval = 0;
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
            frameInterval = 10;
        }
        Act();
    }

    private void Sense()
    {
        List<GameObject> sheep_ = main_.getList("sheep");

        sheepDists = new DistsC[sheep_.Count];

        for (int i = 0; i < sheep_.Count; i++)
        {
            sheepDists[i] = new DistsC();
            sheepDists[i].dist = Vector3.Distance(this.transform.position, sheep_[i].transform.position);
            sheepDists[i].obj = sheep_[i];
        }
        System.Array.Sort(sheepDists);
    }

    private void Decide()
    {
        if (sheepDists.Length > 0 && currentHealth < maxHealth && huntCD <= 0f)
        {
            if (sheepDists[0].dist < 1f)
                action = Action.Eat;

            else if (sheepDists[0].dist <= senseRange)
                action = Action.Pursue;

            else
                action = Action.Wander;
        }

        else if (currentHealth > maxHealth && breedCD <= 0f)
        {
            action = Action.Breed;
        }

        else
            action = Action.Wander;
    }

    private void Die()
    {
        main_.remove(gameObject, "wolf");
        Destroy(gameObject);
    }

    private void Timer()
    {
        if (breedCD > 0f)
            breedCD -= Time.deltaTime;
        if (breedCD < 0f)
            breedCD = 0f;

        if (huntCD > 0f)
            huntCD -= Time.deltaTime;
        if (huntCD < 0f)
            huntCD = 0f;
    }

    private void Act()
    {
        switch (action)
        {
            case Action.Eat:
                huntCD = 6f;
                eatTarget = sheepDists[0].obj;
                Sheep temp = eatTarget.GetComponent<Sheep>();
                temp.Die();
                currentHealth += healthGain;
                action = Action.Wander;
                break;

            case Action.Breed:
                breedCD = 5f;
                currentHealth = maxHealth / 2f;
                main_.addWolf(wolf, transform.position.x + 2, transform.position.y, transform.position.z);
                action = Action.Wander;
                break;

            case Action.Pursue:
                if (sheepDists[0].obj == null)
                    break;

                float step = chaseSpeed * Time.deltaTime;

                Vector2 pos2 = new Vector2(transform.position.x, transform.position.z);
                Vector2 sheepPos2 = new Vector2(sheepDists[0].obj.transform.position.x, sheepDists[0].obj.transform.position.z);
                Vector2 newPos2 = Vector2.MoveTowards(pos2, sheepPos2, step);
                float Y = transform.position.y;

                Vector3 relativePos = new Vector3(sheepDists[0].obj.transform.position.x, Y, sheepDists[0].obj.transform.position.z) - transform.position;
                Quaternion rot = Quaternion.LookRotation(relativePos, Vector3.up);
                transform.rotation = rot;
                transform.position = new Vector3(newPos2.x, Y, newPos2.y);

                rend.material.SetColor("_Color", Color.red);
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
                rend.material.SetColor("_Color", Color.black);
                break;
        }

        currentHealth -= 0.05f;
        if (currentHealth <= 0f)
            Die();
    }
}
