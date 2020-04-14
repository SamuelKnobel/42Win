using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{
    GameObject WallElement;
    GameObject GroundElement;
    GameObject BinElement;

    [SerializeField]
    GameObject EnvironmentContainer;



    Vector2 WallElementSize = Vector2.zero;
    Vector2 GroundElementSize = Vector2.zero;
    Vector2 BinElementSize = Vector2.zero;



    // Start is called before the first frame update
    void Start()
    {
       

    }

    public void BuildEnvironemt()
    {
        EnvironmentContainer = GameObject.FindGameObjectWithTag("EnvironmentContainer");
        foreach (Transform child in EnvironmentContainer.transform)
        {
            if (child.gameObject.name == "Stack1"|| child.gameObject.name == "Stack2"|| child.gameObject.name == "FieldContainer")
            {             
            }
            else
                Destroy(child.gameObject);
        }

        GroundElement = (GameObject)Resources.Load("Prefabs/Environment/GroundElement");
        WallElement = (GameObject)Resources.Load("Prefabs/Environment/WallElement");
        BinElement = (GameObject)Resources.Load("Prefabs/Environment/BinElement");
        GameObject testWall = Instantiate(WallElement);
        WallElementSize = new Vector2(testWall.transform.localScale.x, testWall.transform.localScale.y);
        GameObject testGround = Instantiate(GroundElement);
        GroundElementSize = new Vector2(testGround.transform.localScale.x, testGround.transform.localScale.y);
        GameObject testBin = Instantiate(BinElement);
        BinElementSize = new Vector2(testBin.transform.localScale.x, testBin.transform.localScale.y);
        Destroy(testWall);
        Destroy(testGround);
        Destroy(testBin);


        BuildGround();
        BuildWalls();
        BuildBins();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildGround()
    {
        int width = (int)ConfigurationUtils.Width;

        float startpoint_x;
        float startpoint_y = -ConfigurationUtils.Height / 2;
        
        for (int i = 0; i < width; i++)
        {

            startpoint_x = -ConfigurationUtils.Width / 2  + i /*+ GroundElementSize.x/2*/;
            GameObject Ground = Instantiate(GroundElement,new Vector3(startpoint_x,startpoint_y,0), Quaternion.identity);
            Ground.transform.SetParent(EnvironmentContainer.transform);
        }
    }
    void BuildWalls()
    {
        int height = ConfigurationUtils.Height;

        float startpoint_x = -ConfigurationUtils.Width / 2;
        float startpoint_y;

        for (int i = 0; i < height; i++)
        {
            startpoint_y = -height / 2 + i + WallElementSize.y /2;
            GameObject WallLeft = Instantiate(WallElement, new Vector3(startpoint_x - GroundElementSize.x / 2, startpoint_y, 0), Quaternion.identity);
            GameObject WallRight;
            if (ConfigurationUtils.Width % 2!= 0)
                WallRight = Instantiate(WallElement, new Vector3(-startpoint_x + GroundElementSize.x / 2, startpoint_y, 0), Quaternion.identity);
            else
                WallRight = Instantiate(WallElement, new Vector3(-startpoint_x - GroundElementSize.x / 2, startpoint_y, 0), Quaternion.identity);

            WallLeft.transform.SetParent(EnvironmentContainer.transform);
            WallRight.transform.SetParent(EnvironmentContainer.transform);

        }
    }
    void BuildBins()
    {
        int height = ConfigurationUtils.Height;
        int width = ConfigurationUtils.Width;

        float startpoint_x;
        float startpoint_y;

        for (int i = 0; i < height; i++)
        {
            for (int i2 = 0; i2 < width-1; i2++)
            {
                startpoint_y = -ConfigurationUtils.Height / 2 + i + WallElementSize.y / 2;
                startpoint_x = -ConfigurationUtils.Width / 2 + i2 + GroundElementSize.x / 2;
                GameObject Bin = Instantiate(BinElement, new Vector3(startpoint_x, startpoint_y, 0), Quaternion.identity);
                Bin.transform.SetParent(EnvironmentContainer.transform);
            }
        }
    }
}
