using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject grass;
    public GameObject wolf;
    public GameObject sheep;
    private List<GameObject> allGrass;
    private List<GameObject> allSheep;
    private List<GameObject> allWolf;

    // Start is called before the first frame update
    void Start()
    {
        allGrass = new List<GameObject>();
        allSheep = new List<GameObject>();
        allWolf = new List<GameObject>();
        addGrass(grass, 0f, 0f);

        for (float i = 1; i < 20f; i += 1f)
        {
            addGrass(grass, i, i);
            addGrass(grass, -i, i);
            addGrass(grass, i, -i);
            addGrass(grass, -i, -i);
            addGrass(grass, i, 0);
            addGrass(grass, -i, 0);
            addGrass(grass, 0, i);
            addGrass(grass, 0, -i);
        }

        addSheep(sheep, 15f, 0.6f, 7f);
        addSheep(sheep, 15f, 0.6f, 5f);
        addSheep(sheep, 15f, 0.6f, 9f);
        addSheep(sheep, 15f, 0.6f, 11f);
        addSheep(sheep, 15f, 0.6f, 3f);

        addWolf(wolf, -20f, 0.6f, -20f);
    }

    public List<GameObject> getList(string type)
    {
        switch (type)
        {
            case "grass":
                return allGrass;
                break;
            case "sheep":
                return allSheep;
                break;
            case "wolf":
                return allWolf;
                break;
            default:
                return null;
                break;
        }
    }

    public void addGrass(GameObject grass_, float x, float z)
    {
        allGrass.Add((GameObject)Instantiate(grass_, new Vector3(x , 0, z), Quaternion.identity));
    }

    public void addSheep(GameObject sheep_, float x, float y, float z)
    {
        allSheep.Add((GameObject)Instantiate(sheep_, new Vector3(x, y, z), Quaternion.identity));
    }

    public void addWolf(GameObject wolf_, float x, float y, float z)
    {
        allWolf.Add((GameObject)Instantiate(wolf_, new Vector3(x, y, z), Quaternion.identity));
    }

    public void remove(GameObject temp, string type)
    {
        switch (type)
        {
            case "grass":
                allGrass.Remove(temp);
                break;
            case "sheep":
                allSheep.Remove(temp);
                break;
            case "wolf":
                allWolf.Remove(temp);
                break;
            default:
                return;
                break;
        }
    }
}
