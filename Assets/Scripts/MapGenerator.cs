using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mapPrefab;
    public GameObject redTreePrefab;
    // public GameObject blueTreePrefab;
    public GameObject redMinePrefab;
    // public GameObject blueMinePrefab;
    public GameObject pathEffectPrefab;
    public GameObject cannonPlacePrefab;
    public GameObject unicorn;
    public int minElementNum;
    public int maxElementNum;
    public int mapNum;
    public bool isInitialized { get; private set; }

    private int cannonPlaceNum;
    private int treesNum;
    private int minesNum;
    private int mapWidth;
    private int mapHeight;
    private List<List<Vector3>> paths;

    void Start()
    {
        // initialization
        mapWidth = 28;
        mapHeight = 14;
        cannonPlaceNum = 6;
        paths = new List<List<Vector3>>();

        for (int mapIdx = 0; mapIdx < mapNum; mapIdx++)
        {
            // create ground first
            Vector3 mapPos = new Vector3(mapIdx * mapWidth, 0f, 0f);
            Instantiate(mapPrefab, mapPos, Quaternion.identity);
            // generate path on the map
            GeneratePath(new Vector3(-13.5f + mapIdx * mapWidth, 0.5f, -6.5f));
            // generate elements on the map
            treesNum = Random.Range(minElementNum, maxElementNum);
            minesNum = Random.Range(minElementNum, maxElementNum);
            GenerateElement(minesNum, new Vector3(-13.5f + mapIdx * mapWidth, 0.5f, 6.5f), redMinePrefab);
            GenerateElement(treesNum, new Vector3(0.5f + mapIdx * mapWidth, 0.5f, 6.5f), redTreePrefab);
            // generate cannonPlace on the map
            GenerateCannonPlace(new Vector3(-13.5f + mapIdx * mapWidth, 0f, -0.5f));
        }
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateElement(int total, Vector3 offset, GameObject prefab)
    {
        HashSet<Vector3> elements = new HashSet<Vector3>();

        for (int i = 0; i < total; i++)
        {
            Vector3 point;
            do
            {
                point = new Vector3(Random.Range(0, 13), 0f, (-1) * Random.Range(0, 7));
            } while (elements.Contains(point));
            elements.Add(point);

            Vector3 position = point + offset;
            Instantiate(prefab, position, Quaternion.identity);
        }
    }

    private void GeneratePath(Vector3 offset)
    {
        // First, random each column position
        List<int> zPositions = new List<int>();
        int pathWidth = 2;
        int lines = 3;
        for (int i = 0; i < mapWidth - 2; i++)
        {
            int zPos = Random.Range(0, lines);
            zPositions.Add(zPos);
            for (int k = 1; k < pathWidth; k++)
            {
                zPositions.Add(zPos);
                if (i < mapWidth - 2) i++;
                else break;
            }
            // old version with pathWidth = 1
            // zPositions.Add(Random.Range(0, lines));
        }
        zPositions.Add(0);

        // direction
        // 1 -> up
        // 0 -> right
        // -1 -> down
        // Next, generate the path according to each column position
        int currentZPosition = 0;
        List<Vector3> path = new List<Vector3>();
        path.Add(offset);
        foreach (int p in zPositions)
        {
            Vector3 point = new Vector3(1f, 0f, 0f);
            offset += point;
            path.Add(offset);

            if (currentZPosition != p)
            {
                int step = (currentZPosition > p) ? -1 : 1;
                for (int i = 0; i < Mathf.Abs(currentZPosition - p); i++)
                {
                    Vector3 vec = (currentZPosition > p) ? new Vector3(0f, 0f, -1) : new Vector3(0f, 0f, 1);
                    offset += vec;
                    path.Add(offset);
                }
            }
            currentZPosition = p;
        }

        // Finally, load the effect on the determined path
        LoadEffect(path);
        paths.Add(path);
    }
    private void LoadEffect(List<Vector3> path)
    {
        foreach (Vector3 p in path)
        {
            Instantiate(pathEffectPrefab, p, Quaternion.identity);
        }
    }

    public List<Vector3> GetPath(int idx)
    {
        return paths[idx];
    }

    private void GenerateCannonPlace(Vector3 offset)
    {
        int lines = 4;
        // first, random the column position
        List<int> xPositions = new List<int>();
        for (int i=0; i<cannonPlaceNum; i++) {
            int xPos;
            do {
                xPos = Random.Range(0, mapWidth-1);
            } while (CheckIfOverlay(xPositions, xPos));
            xPositions.Add(xPos);
        }

        // next, place the cannonPlace according the column position
        // notice that there are 3 available z positions to place
        // so we need to decide it first (random)
        // Vector3 baseY = new Vector3(0f, 1f, 0f);
        for (int i = 0; i < cannonPlaceNum; i++)
        {
            int zPos = Random.Range(0, lines-1);
            Vector3 position = offset + new Vector3(xPositions[i], 1f, -zPos);
            Instantiate(cannonPlacePrefab, position, Quaternion.identity);
        }
    }

    private bool CheckIfOverlay(List<int> xPositions, int xPos) {
        // If there's already a cannonPlace in xPos-2, xPos-1, xPos, xPos+1, xPos+2 -> overlay
        // Else -> valid position to place the cannonPlace
        for (int i=-2; i<=2; i++) {
            if (xPositions.Contains(xPos+i)) return true;
        }
        return false;
    }

    public Vector3 ResetPlayerPosition()
    {
        // first, find out which map the player is at
        int mapIdx = Mathf.FloorToInt((14f + unicorn.transform.position.x) / mapWidth);
        mapIdx = Mathf.Max(mapIdx, 0);
        // Debug.Log(mapIdx);

        // next, set the player's position in the middle of the map
        Vector3 resetPosition = new Vector3(-0.5f + mapIdx * mapWidth, 2.0f, 1.5f);
        return resetPosition;
    }

}
