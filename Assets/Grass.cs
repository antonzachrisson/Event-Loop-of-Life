using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    private enum Action { loseHealth, grow, spread, nothing };
    Action action;

    public GameObject grass;
    private Main main_;

    private bool trampled;
    private bool hasTrampled;
    private float health;
    private float spreadCD;
    private float maxHealth;
    private int decideInterval;
    
    // Start is called before the first frame update
    void Start()
    {
        trampled = false;
        hasTrampled = false;
        health = maxHealth / 10f;
        action = Action.nothing;
        spreadCD = 0f;
        main_ = GameObject.Find("Main").GetComponent<Main>();
        transform.localScale += new Vector3(0f, -1f, 0f);
        maxHealth = 200f;
        decideInterval = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Timer();
        Size();
        decideInterval++;
        if (decideInterval > 10)
        {
            Decide();
            decideInterval = 0;
        }
        Act();
    }

    //Sense
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Sheep" || other.gameObject.tag == "Wolf")
        {
            trampled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Sheep" || other.gameObject.tag == "Wolf")
        {
            trampled = false;
            hasTrampled = false;
        }
    }

    private void Size()
    {
        Vector3 scaleChange = new Vector3(0f, (health - maxHealth) / maxHealth, 0f);

        transform.localScale += new Vector3(0f, 1f - transform.localScale.y, 0f);
        transform.localScale += scaleChange;
    }

    private void Timer()
    {
        if (spreadCD > 0f)
            spreadCD -= Time.deltaTime;
        if (spreadCD < 0f)
            spreadCD = 0f;
    }

    private void Decide()
    {
        if (trampled)
            action = Action.loseHealth;

        else
        {
            if (health < maxHealth)
                action = Action.grow;

            else if (health >= maxHealth && spreadCD <= 0f)
                action = Action.spread;

            else
                action = Action.nothing;
        }
    }

    private void Act()
    {
        switch (action)
        {
            case Action.loseHealth:
                if (!hasTrampled)
                {
                    health /= 2f;
                    hasTrampled = true;
                }
                break;
            case Action.grow:
                health += 0.2f;
                break;
            case Action.spread:
                main_.addGrass(grass, this.transform.position.x + Random.Range(-3f, 3f), this.transform.position.z + Random.Range(-3f, 3f));
                //spreadCD = 2f;
                spreadCD = 15f;
                action = Action.nothing;
                break;
            case Action.nothing:
                if (health > maxHealth)
                    health = maxHealth;
                break;
        }
    }

    float getHealth()
    {
        return health;
    }

    //temporary
    public void Die()
    {
        main_.remove(gameObject, "grass");
        Destroy(gameObject);
    }
}
