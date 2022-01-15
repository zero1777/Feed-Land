using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// TODO
// check the monster/unicorn api 
// let the map can be horizontal or vertical

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mapPrefab;
    public GameObject mapPrefabWithSanta;
    public GameObject mapPrefabWithWinter;
    public GameObject pathEffectPrefab;
    public GameObject cannonPlacePrefab;
    public GameObject unicorn;
    public GameObject[] treePrefab;
    public GameObject[] minePrefab;
    public GameObject snowEffect;

    public int[] elementNum;
    public int level;
    public int initMapNum;
    public bool isInitialized { get; private set; }
    private int currentMapIdx;
    private List<List<Vector3>> specialPrefabCoord;
    private List<mapType> mapTypeSeq;
    private List<List<int>> specialPrefabPathZPos;
    private mapType currentMapType;

    private int cannonPlaceNum;
    private int[] treesNum;
    private int[] minesNum;
    private int mapWidth;
    private int mapHeight;
    private int prevZPoint;
    private List<List<Vector3>> paths;

    private enum mapType : int
    {
        Santa,
        Winter,
        Origin
    }

    void Start()
    {
        // initialization
        mapWidth = 28;
        mapHeight = 14;
        currentMapIdx = 0;
        prevZPoint = mapHeight - 1;

        specialPrefabCoord = new List<List<Vector3>>();
        specialPrefabPathZPos = new List<List<int>>();
        paths = new List<List<Vector3>>();
        mapTypeSeq = new List<mapType>() { mapType.Origin, mapType.Origin, mapType.Santa, mapType.Origin, mapType.Origin, mapType.Winter };

        LoadSpecialPrefabPos();
        LoadSpecialPrefabPathZPos();

        for (int i = 0; i < initMapNum; i++)
        {
            GenerateMap();
        }
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (unicorn != null)
        {
            int unicornMapIdx = FindUnicornPosition();
            if (unicornMapIdx == currentMapIdx - 1) GenerateMap();
        }
    }

    public int GetCurrentMapIdx()
    {
        return currentMapIdx;
    }

    private void GenerateMap()
    {
        // elementPositions = new List<Vector3>();
        List<System.Tuple<Vector3, GameObject>> elementPositions;
        List<Vector3> cannonPlacePositions;
        List<Vector3> path;
        List<Vector3> specialPrefabPathPositions;

        // get mapType from currentMapIdx
        currentMapType = mapTypeSeq[currentMapIdx % mapTypeSeq.Count];

        // create ground first
        // according to the mapTypeSeq
        Vector3 mapPos = new Vector3(currentMapIdx * mapWidth, 0f, 0f);

        switch (currentMapType)
        {
            case mapType.Origin:
                Instantiate(mapPrefab, mapPos, Quaternion.identity);
                break;
            case mapType.Santa:
                Instantiate(mapPrefabWithSanta, mapPos, Quaternion.identity);
                break;
            case mapType.Winter:
                Instantiate(mapPrefabWithWinter, mapPos, Quaternion.identity);
                break;
        }

        // generate specialPrefab Positions on the map
        specialPrefabPathPositions = GenerateSpecialPrefabPosition(new Vector3(-13.5f + currentMapIdx * mapWidth, 0.5f, 6.5f));
        // Debug.Log($"[GenerateSpecialPrefabPosition] spp-size: {specialPrefabPathZPos[1].Count}");

        // Initialize some variables
        List<int> total = new List<int>();
        List<GameObject> prefab = new List<GameObject>();
        cannonPlaceNum = Random.Range(5, 8);

        for (int j = 0; j < level; j++)
        {
            switch (currentMapType)
            {
                case mapType.Origin:
                    prefab.Add(treePrefab[j]);
                    total.Add(elementNum[j]);

                    prefab.Add(minePrefab[j]);
                    total.Add(elementNum[j]);
                    break;
                case mapType.Santa:
                case mapType.Winter:
                    prefab.Add(treePrefab[j]);
                    total.Add(elementNum[j]);

                    prefab.Add(minePrefab[j]);
                    total.Add(elementNum[j]);
                    break;
            }

        }
        // Debug.Log($"[Initialize] spp-size: {specialPrefabPathZPos[1].Count}");

        // generate elementsPositions on the map
        elementPositions = GenerateElementPosition(total, new Vector3(-13.5f + currentMapIdx * mapWidth, 0.5f, 6.5f), prefab);
        // Debug.Log($"[GenerateElementPosition] spp-size: {specialPrefabPathZPos[1].Count}");

        // generate path on the map
        path = GeneratePath(new Vector3(-13.5f + currentMapIdx * mapWidth, 0.5f, 6.5f));
        // Debug.Log($"[GeneratePath] spp-size: {specialPrefabPathZPos[1].Count}");

        // generate cannonPlace on the map
        cannonPlacePositions = GenerateCannonPlace(cannonPlaceNum, new Vector3(-13.5f + currentMapIdx * mapWidth, 0.5f, 6.5f), path, specialPrefabPathPositions);
        // Debug.Log($"[GenerateCannonPlace] spp-size: {specialPrefabPathZPos[1].Count}");

        // generate elements on the map
        GenerateElementsPrefab(elementPositions, path, cannonPlacePositions, specialPrefabPathPositions);
        // Debug.Log($"[GenerateElementsPrefab] spp-size: {specialPrefabPathZPos[1].Count}");

        // generate snowEffect on the map when in the special mapType
        GenerateSnowEffect(new Vector3(0.5f + currentMapIdx * mapWidth, 11f, 1.5f));
        // Debug.Log($"[GenerateSnowEffect] spp-size: {specialPrefabPathZPos[1].Count}");

        currentMapIdx++;
    }

    private void Shuffle(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector3 currentIndex = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = currentIndex;
        }
    }

    // offset is at the left-top of map
    private List<System.Tuple<Vector3, GameObject>> GenerateElementPosition(List<int> total, Vector3 offset, List<GameObject> prefab)
    {
        List<System.Tuple<Vector3, GameObject>> elementPositions = new List<System.Tuple<Vector3, GameObject>>();


        for (int idx = 0; idx < total.Count; idx++)
        {
            HashSet<Vector3> topLeftPositions = new HashSet<Vector3>();
            // first, random generate the position from the whole map
            for (int i = 0; i < total[idx]; i++)
            {
                Vector3 point;
                do
                {
                    point = new Vector3(Random.Range(0, mapWidth - 2), 0f, -(Random.Range(0, mapHeight - 2)));
                } while (CheckIfElementOverlay(point + offset, elementPositions));
                topLeftPositions.Add(point);
            }

            // next, place the element according to the column position
            // notice that we will reserved 3*3 place for that kind of elements
            // random the number of each 3*3 place for diversity
            List<Vector3> box = new List<Vector3>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    box.Add(new Vector3(i, 0f, -j));
                }
            }

            foreach (Vector3 tfpos in topLeftPositions)
            {
                int num = Random.Range(5, 7);
                Vector3 position = offset + tfpos;
                Shuffle(box);

                for (int i = 0; i < num; i++)
                {
                    System.Tuple<Vector3, GameObject> tp = new System.Tuple<Vector3, GameObject>(position + box[i], prefab[idx]);
                    elementPositions.Add(tp);
                }
            }
        }

        return elementPositions;
    }

    private bool CheckIfElementOverlay(Vector3 position, List<System.Tuple<Vector3, GameObject>> elementPositions)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                foreach (System.Tuple<Vector3, GameObject> tp in elementPositions)
                {
                    Vector3 vec = position + new Vector3(x, 0f, -z);
                    if (tp.Item1 == vec)
                    {
                        return true;
                    }
                }
                // bool contains = elementPositions.Any(m => m.Item1 == (position + new Vector3(x, 0f, -z)));
                // if (contains) return true;
            }
        }
        return false;
    }

    // offset is at the left-top of map
    private List<Vector3> GeneratePath(Vector3 offset)
    {
        // Debug.Log($"[Before] spp-size: {specialPrefabPathZPos[1].Count}");
        // First, random each column position
        List<int> zPositions = new List<int>();
        switch (currentMapType)
        {
            case mapType.Origin:
                int pathWidth = 5;
                zPositions.Add(prevZPoint);

                for (int i = 0; i < mapWidth - 1;)
                {
                    int zPos = Random.Range(0, mapHeight);
                    for (int k = 0; k < pathWidth; k++)
                    {
                        if (i == mapWidth - 2) prevZPoint = zPos;
                        zPositions.Add(zPos);
                        i++;
                        if (i >= mapWidth - 1) break;
                    }
                }
                Debug.Log($"Origin zPos size: {zPositions.Count}");
                break;
            case mapType.Santa:
                zPositions = specialPrefabPathZPos[(int)mapType.Santa];
                zPositions.Insert(0, prevZPoint);
                prevZPoint = mapHeight / 2;
                Debug.Log($"Santa zPos size: {zPositions.Count}");
                break;
            case mapType.Winter:

                zPositions = specialPrefabPathZPos[(int)mapType.Winter];
                // Debug.Log($"[After] spp-size: {specialPrefabPathZPos[1].Count}");
                zPositions.Prepend(prevZPoint);
                prevZPoint = mapHeight / 2;
                Debug.Log($"Winter zPos size: {zPositions.Count}");
                break;
        }


        // direction
        // 1 -> up
        // 0 -> right
        // -1 -> down
        // Next, generate the path according to each column position
        List<Vector3> path = new List<Vector3>();
        int currentZPosition = zPositions[0];
        zPositions.Remove(0);
        offset += new Vector3(0f, 0f, -currentZPosition);
        path.Add(offset);
        foreach (int p in zPositions)
        {
            Vector3 point = new Vector3(1f, 0f, 0f);
            offset += point;
            path.Add(offset);

            if (currentZPosition != p)
            {
                for (int i = 0; i < Mathf.Abs(currentZPosition - p); i++)
                {
                    Vector3 vec = (p > currentZPosition) ? new Vector3(0f, 0f, -1) : new Vector3(0f, 0f, 1);
                    offset += vec;
                    path.Add(offset);
                }
            }
            currentZPosition = p;
        }

        // Finally, load the effect on the determined path
        LoadEffect(path);
        paths.Add(path);

        return path;
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

    // offset is at the left-top of map
    private List<Vector3> GenerateCannonPlace(int total, Vector3 offset, List<Vector3> path, List<Vector3> specialPrefabPathPositions)
    {
        List<Vector3> cannonPlacePositions = new List<Vector3>();
        // first, random the column position
        switch (currentMapType)
        {
            case mapType.Origin:
            case mapType.Santa:
                Vector3 point;
                for (int i = 0; i < total; i++)
                {
                    do
                    {
                        point = new Vector3(Random.Range(1, mapWidth - 2), 0f, -(Random.Range(1, mapHeight - 2)));
                    } while (CheckIfCannonPlaceOverlay(offset + point, cannonPlacePositions, path, specialPrefabPathPositions));

                    // Generate cannonPlace prefab accroding to the point
                    Vector3 position = offset + point;
                    Instantiate(cannonPlacePrefab, position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);

                    // add the occupied point to the cannonPlacePositions
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            cannonPlacePositions.Add(position + new Vector3(x, 0f, -z));
                        }
                    }
                }
                break;
            case mapType.Winter:
                // cannonPlace will be built in two rows
                // first row with 5 cannonPlace
                // and second with 2 cannonPlace

                // first row
                int xIdx = 8;
                int zIdx = 3;
                Vector3 pt;
                for (int i = 0; i < 5; i++, xIdx += 3)
                {
                    // Generate cannonPlace prefab accroding to the point
                    pt = new Vector3(xIdx, 0f, -zIdx);
                    Vector3 position = offset + pt;
                    Instantiate(cannonPlacePrefab, position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);

                    // add the occupied point to the cannonPlacePositions
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            cannonPlacePositions.Add(position + new Vector3(x, 0f, -z));
                        }
                    }
                }

                // second row
                xIdx = 8;
                zIdx = 6;
                for (int i = 0; i < 2; i++, xIdx += 12)
                {
                    // Generate cannonPlace prefab accroding to the point
                    pt = new Vector3(xIdx, 0f, -zIdx);
                    Vector3 position = offset + pt;
                    Instantiate(cannonPlacePrefab, position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);

                    // add the occupied point to the cannonPlacePositions
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            cannonPlacePositions.Add(position + new Vector3(x, 0f, -z));
                        }
                    }
                }
                break;
        }

        return cannonPlacePositions;
    }

    private bool CheckIfCannonPlaceOverlay(Vector3 point, List<Vector3> cannonPlacePositions, List<Vector3> path, List<Vector3> specialPrefabPathPositions)
    {
        // If there's already a path or a cannonPlace in the position -> overlay
        // Else -> valid position to place the cannonPlace
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector3 position = point + new Vector3(x, 0f, -z);
                if (cannonPlacePositions.Contains(position)) return true;
                if (path.Contains(position)) return true;
                if (currentMapType == mapType.Santa)
                {
                    if (specialPrefabPathPositions.Contains(position)) return true;
                }
            }
        }
        return false;
    }

    private void GenerateElementsPrefab(List<System.Tuple<Vector3, GameObject>> elementPositions, List<Vector3> path, List<Vector3> cannonPlacePositions, List<Vector3> specialPrefabPathPositions)
    {
        // only create the prefab where the position isn't occupied by cannonPlace or path
        foreach (System.Tuple<Vector3, GameObject> tp in elementPositions)
        {
            bool isPathOverlay = false;
            for (int x = -1; x <= 1 && !isPathOverlay; x++)
            {
                for (int z = -1; z <= 1 && !isPathOverlay; z++)
                {
                    if (path.Contains(tp.Item1 + new Vector3(x, 0f, z)))
                    {
                        isPathOverlay = true;
                    }
                }
            }

            bool isSpecialPrefabOverlay = false;
            if (currentMapType != mapType.Origin && specialPrefabPathPositions.Contains(tp.Item1)) isSpecialPrefabOverlay = true;
            // if (!isPathOverlay && !cannonPlacePositions.Contains(tp.Item1)) Debug.Log($"position: {tp.Item1}");
            if (!isPathOverlay && !cannonPlacePositions.Contains(tp.Item1) && !isSpecialPrefabOverlay) Instantiate(tp.Item2, tp.Item1, Quaternion.identity);
        }
    }

    public Vector3 ResetPlayerPosition()
    {
        // first, find out which map unicorn is at
        int mapIdx = FindUnicornPosition();
        // Debug.Log(mapIdx);

        // next, set the player's position in the middle of the map
        Vector3 resetPosition = new Vector3(-0.5f + mapIdx * mapWidth, 2.0f, 1.5f);
        return resetPosition;
    }

    private int FindUnicornPosition()
    {
        // Debug.Log(unicorn.name);
        int idx = Mathf.FloorToInt((14f + unicorn.transform.position.x) / mapWidth);
        idx = Mathf.Max(idx, 0);
        return idx;
    }

    // for special prefab position and function
    private void LoadSpecialPrefabPos()
    {
        // santa Prefab
        List<Vector3> santaList = new List<Vector3>();
        for (int x = 11; x <= 18; x++)
        {
            for (int z = 0; z <= 3; z++)
            {
                santaList.Add(new Vector3(x, 0f, -z));
            }
        }
        specialPrefabCoord.Add(santaList);

        // winter Prefab
        List<Vector3> winterList = new List<Vector3>();
        for (int x = 10; x <= 19; x++)
        {
            for (int z = 5; z <= 9; z++)
            {
                winterList.Add(new Vector3(x, 0f, -z));
            }
        }
        specialPrefabCoord.Add(winterList);
    }

    private void LoadSpecialPrefabPathZPos()
    {
        // santa map path
        // |===|    |===|
        // |   ======   |
        // |            | 
        List<int> santaZPositions = new List<int>();
        for (int x = 0; x < mapWidth; x++)
        {
            // 1. ===
            if (x >= 1 && x <= 9) santaZPositions.Add(0);
            // 2. =====
            else if (x >= 10 && x <= 18) santaZPositions.Add(4);
            // 3. ===
            else if (x >= 19 && x <= mapWidth - 3) santaZPositions.Add(0);
            else if (x >= mapWidth - 2 && x <= mapWidth - 1) santaZPositions.Add(mapHeight / 2);
        }
        specialPrefabPathZPos.Add(santaZPositions);

        // winter map path
        //    |======|
        //    |      |
        // ===       ===
        List<int> winterZPositions = new List<int>();
        for (int x = 0; x < mapWidth; x++)
        {
            // 1. ===
            if (x >= 1 && x <= 3) winterZPositions.Add(mapHeight / 2);
            // 2. =====
            else if (x >= 4 && x <= 23) winterZPositions.Add(1);
            // 3. ===
            else if (x >= 24 && x <= mapWidth - 1) winterZPositions.Add(mapHeight / 2);
        }
        specialPrefabPathZPos.Add(winterZPositions);
    }

    private List<Vector3> GenerateSpecialPrefabPosition(Vector3 offset)
    {
        switch (currentMapType)
        {
            case mapType.Origin:
                return null;
            case mapType.Santa:
                List<Vector3> santaPos = new List<Vector3>();
                foreach (Vector3 spp in specialPrefabCoord[((int)mapType.Santa)])
                {
                    santaPos.Add(offset + spp);
                }
                return santaPos;
            case mapType.Winter:
                List<Vector3> winterPos = new List<Vector3>();
                foreach (Vector3 spp in specialPrefabCoord[((int)mapType.Winter)])
                {
                    winterPos.Add(offset + spp);
                }
                return winterPos;
        }
        return null;
    }

    private void GenerateSnowEffect(Vector3 offset)
    {
        GameObject snow = Instantiate(snowEffect, offset, Quaternion.identity);
        snow.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        Destroy(snow, 30f);
    }
}
