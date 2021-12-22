using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mapPrefab;
    public GameObject treePrefab;
    public GameObject minePrefab;
    public GameObject pathEffectPrefab;
    public GameObject turretPlacePrefab;
    public int minElementNum;
    public int maxElementNum;
    public int mapNum;
    
    private int turretPlaceNum;
    private int treesNum;
    private int minesNum;
    private int mapWidth;
    private int mapHeight;
    private List<List<int>> paths;
    private float offsetY;
    private int mapIdx;

    void Start()
    {
        // initialization
        mapWidth = 28;
        mapHeight = 14;
        offsetY = 0.5f;
        mapIdx = 0;
        turretPlaceNum = 6;
        paths = new List<List<int>>();

        while (mapIdx < mapNum)
        {
            // create ground first
            Vector3 mapPos = new Vector3(mapIdx * mapWidth, 0f, 0f);
            Instantiate(mapPrefab, mapPos, Quaternion.identity);
            // generate path on the map
            GeneratePath();
            // generate elements on the map
            treesNum = Random.Range(minElementNum, maxElementNum + 1);
            minesNum = Random.Range(minElementNum, maxElementNum + 1);
            GenerateElement(minesNum, "left", minePrefab);
            GenerateElement(treesNum, "right", treePrefab);
            // generate turretPlace on the map
            GenerateTurretPlace();
            mapIdx++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // assume that treePrefab is placed at the right half of the map
    // and the minePrefab is placed at the left half of the map 
    private void GenerateElement(int total, string region, GameObject _object)
    {
        List<int> elementIdx = new List<int>();
        // temporarily set the region size = 14 * 7
        int regionSize = 98;
        for (int i = 0; i < total; i++)
        {
            int idx;
            do
            {
                idx = Random.Range(0, regionSize);
            }
            while (elementIdx.Contains(idx));
            elementIdx.Add(idx);

            // translate the idx to coordinate
            Vector3 pos = idxToPos(idx, region);
            Instantiate(_object, pos, Quaternion.identity);
        }
    }

    private Vector3 idxToPos(int idx, string region)
    {
        int row = mapWidth / 2;
        int _x = idx % row;
        int _z = idx / row;
        int mapOffset = mapIdx * mapWidth;

        float offsetX, offsetZ;
        if (region == "left")
        {
            offsetX = -13.5f;
            offsetZ = 6.5f;
        }
        else
        {
            offsetX = 0.5f;
            offsetZ = 6.5f;
        }

        float x = offsetX + _x + mapOffset;
        float z = offsetZ - _z;
        return new Vector3(x, offsetY, z);
    }

    private void GeneratePath()
    {
        // First, random each column position
        List<int> pos = new List<int>();
        int lines = 4;
        for (int i = 0; i < mapWidth - 2; i++)
        {
            pos.Add(Random.Range(0, lines));
        }
        pos.Add(0);

        // direction
        // 1 -> up
        // 0 -> right
        // -1 -> down
        // Next, generate the path according to each column position
        int currentPos = 0;
        List<int> path = new List<int>();
        foreach (var p in pos)
        {
            path.Add(0);
            if (currentPos != p)
            {
                int step = (currentPos > p) ? -1 : 1;
                for (int i = 0; i < Mathf.Abs(currentPos - p); i++) path.Add(step);
            }
            currentPos = p;
        }

        // Finally, load the effect on the determined path
        LoadEffect(path);
        paths.Add(path);
    }

    private void LoadEffect(List<int> path)
    {
        int mapOffset = mapIdx * mapWidth;
        float startX = -13.5f + mapOffset;
        float startZ = -6.5f;

        Vector3 currentPos = new Vector3(startX, offsetY, startZ);
        Instantiate(pathEffectPrefab, currentPos, Quaternion.identity);
        foreach (var p in path)
        {
            // direction
            // 1 -> up
            // 0 -> right
            // -1 -> down
            if (p == -1 || p == 1)
            {
                currentPos.z += p;
            }
            else
            { // p = 0
                currentPos.x += 1;
            }
            Instantiate(pathEffectPrefab, currentPos, Quaternion.identity);
        }
    }

    public List<int> GetPath(int idx)
    {
        return paths[idx];
    }


    private void GenerateTurretPlace() {
        // for convenience, directly set the constant
        float x = -9.5f + mapIdx * mapWidth;
        float z = -1.5f;
        for (int i=0; i<turretPlaceNum; i++) {
            Vector3 vec = new Vector3(x + i*4f, 0f, z);
            Instantiate(turretPlacePrefab, vec, Quaternion.identity);
        }
    }

}
