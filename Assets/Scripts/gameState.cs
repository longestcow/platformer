using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameState : MonoBehaviour
{
    int[] maxLevels = new int[]{2,3,2,1,5};

    public int shotgun, grappler, boost;
    public int phase, level;

    GameObject shotgunObj, grapplerObj, boostObj; 
    Shotty shotgunScript; grappler grapplerScript; boost boostScript; public Player player;

    public List<Color> loadColors = new List<Color>();
    public List<GameObject> currentguns = new List<GameObject>();
    public List<List<GameObject>> spawnPoints = new List<List<GameObject>>(), workArounds = new List<List<GameObject>>(), confiners = new List<List<GameObject>>(), snows = new List<List<GameObject>>();
    //lists - positions for spawnPoints and workarounds, gameObjects of colliders
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        shotgun = 0;
        grappler = 0;
        boost = 0 ;

        phase = 0;
        level = 0;

        currentguns.Add(shotgunObj = GameObject.Find("shotty"));
        currentguns.Add(grapplerObj = GameObject.Find("grappler"));
        currentguns.Add(boostObj = GameObject.Find("boost"));

        shotgunScript = shotgunObj.GetComponent<Shotty>();
        grapplerScript = grapplerObj.GetComponent<grappler>();
        boostScript = boostObj.GetComponent<boost>();

        yellow_setup();
        player.Noise(0,0);

        player.cam.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingShape2D = confiners[phase][level].GetComponent<Collider2D>();
        player.transform.position = spawnPoints[phase][level].transform.position;
        snows[phase][level].GetComponent<ParticleSystem>().Play();

    }

    void Update()
    {
        if(shotgun < 1 && currentguns.Contains(shotgunObj))
            currentguns.Remove(shotgunObj);
        if(shotgun > 0 && !currentguns.Contains(shotgunObj))
            currentguns.Add(shotgunObj);

        if(grappler < 1 && currentguns.Contains(grapplerObj))
            currentguns.Remove(grapplerObj);
        if(grappler > 0 && !currentguns.Contains(grapplerObj))
            currentguns.Add(grapplerObj);

        if(boost < 1 && currentguns.Contains(boostObj))
            currentguns.Remove(boostObj);
        if(boost > 0 && !currentguns.Contains(boostObj))
            currentguns.Add(boostObj);

        if(!grapplerScript.enabled && !shotgunScript.enabled && !boostScript.enabled){
            if(currentguns.Contains(shotgunObj))
                shotgunScript.enabled=true;
            else if(currentguns.Contains(grapplerObj))
                grapplerScript.enabled=true;
            else if(currentguns.Contains(boostObj))
                boostScript.enabled=true;
        }




        // if(!grapplerScript.enabled) {
        //     foreach(GameObject obj in GameObject.FindObjectsOfType<hitPoint>())
        //         Destroy(obj);
        //     boostObj.GetComponentInChildren<LineRenderer>().enabled=false;
        // }

    }

    public void nextLevel() {
        transform.position=new Vector3(-20,20,0);
        snows[phase][level].GetComponent<ParticleSystem>().Stop();
        level+=1;
        if(level>=maxLevels[phase]){
            phase=0;
            level=0;
            player.dialogueChange("yeah thats all there is", 3);
        }
        snows[phase][level].GetComponent<ParticleSystem>().Play();
        GameObject.Find("rope").GetComponent<grapplingRope>().enabled=false;
        Quaternion the = Quaternion.Euler(0,0,Random.Range(40, 80));
        Instantiate(player.load, new Vector3(0,0,10), the, GameObject.Find("cam").transform);
        reset();
        // spawnPoint, confiner, and workAround are handled in loadScript 
    }
    
    public float mapfunc(float val, float from1, float to1, float from2, float to2){
        return (((val - from1) * (to2 - from2)) / (to1-from1)) + from2;
    }

    void yellow_setup(){
        loadColors.Add(new Color(mapfunc(183,0,255,0,1), mapfunc(153,0,255,0,1), 0)); //yellow


        List<GameObject> spawnPoin = new List<GameObject>(), workAroun = new List<GameObject>(), confiner = new List<GameObject>(), snow = new List<GameObject>();
        spawnPoin.Add(GameObject.Find("level 1/spawnPoint"));
        spawnPoin.Add(GameObject.Find("level 2/spawnPoint"));
        spawnPoints.Add(spawnPoin);

        workAroun.Add(GameObject.Find("level 1/loadWorkaround"));
        workAroun.Add(GameObject.Find("level 2/loadWorkaround"));
        workArounds.Add(workAroun);

        confiner.Add(GameObject.Find("level 1/confiner"));
        confiner.Add(GameObject.Find("level 2/confiner"));
        confiners.Add(confiner);

        snow.Add(GameObject.Find("level 1/Snow"));
        snow.Add(GameObject.Find("level 2/Snow"));
        snows.Add(snow);

        

    }

    public void reset(){
        shotgun=0;
        grappler=0;
        boost=0;
        player.dNums = 0;
        player.jNums = 0;
    }


}
