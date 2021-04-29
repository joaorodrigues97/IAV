using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject player;
    public Material material;
    public Camera cam;
    public static int chunkSize = 16;
    public static int radius = 3;
    public static ConcurrentDictionary<string, Chunk> chunkDict;
    public static List<string> toRemove = new List<string>();
    Vector3 lastBuildPos;
    bool drawing;

    public static string CreateChunkName(Vector3 v)
    {
        return (int)v.x + " " + (int)v.y + " " + (int)v.z;
    }

    IEnumerator BuildRecursiveWorld(Vector3 chunkPos, int rad)
    {
        int x = (int)chunkPos.x;
        int y = (int)chunkPos.y;
        int z = (int)chunkPos.z;

        BuildChunkAt(chunkPos);
        yield return null;

        if (--rad < 0) yield break;
        Building(new Vector3(x, y, z + chunkSize), rad);
        yield return null;
        Building(new Vector3(x, y, z - chunkSize), rad);
        yield return null;
        Building(new Vector3(x + chunkSize, y, z), rad);
        yield return null;
        Building(new Vector3(x - chunkSize, y, z), rad);
        yield return null;
        Building(new Vector3(x, y + chunkSize, z), rad);
        yield return null;
        Building(new Vector3(x, y - chunkSize, z), rad);
    }

    void BuildChunkAt(Vector3 chunkPos)
    {
        string name = CreateChunkName(chunkPos);
        Chunk c;
        if (!chunkDict.TryGetValue(name, out c))
        {
            c = new Chunk(chunkPos, material);
            c.goChunk.transform.parent = this.transform;
            chunkDict.TryAdd(c.goChunk.name, c);
        }
    }

    IEnumerator RemoveChunks()
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            string name = toRemove[i];
            Chunk c;
            if (chunkDict.TryGetValue(name, out c))
            {
                Destroy(c.goChunk);
                chunkDict.TryRemove(name, out c);
                toRemove.Remove(name);
                yield return null;
            }
        }
        toRemove.Clear();
    }

    IEnumerator DrawChunks()
    {
        drawing = true;
        foreach (KeyValuePair<string, Chunk> c in chunkDict)
        {
            if (c.Value.status == Chunk.chunkStatus.DRAW)
            {
                c.Value.DrawChunk();
                yield return null;
            }
            /*if(c.Value.goChunk && Vector3.Distance(player.transform.position, c.Value.goChunk.transform.position) > chunkSize * radius)
            {
                toRemove.Add(c.Key);
            }*/
            Vector3 chunkPos = c.Value.goChunk.transform.position;
            Vector3 playerChunkPos = whichChunk(player.transform.position);
            /*if (c.Value.goChunk && Vector3.Distance(getChunkCenter(playerChunkPos), getChunkCenter(chunkPos)) > chunkSize * radius)
            {
                toRemove.Add(c.Key);
            }*/
            
            if (Mathf.Abs(playerChunkPos.x - int.Parse(c.Key.Split(' ')[0])) > 16*radius || Mathf.Abs(playerChunkPos.z - int.Parse(c.Key.Split(' ')[2])) > 16 * radius || Mathf.Abs(playerChunkPos.y - int.Parse(c.Key.Split(' ')[1])) > 16 * radius)
            {
                //Debug.Log("PLAYER X: "+getChunkCenter(playerChunkPos).x);
                //Debug.Log("PLAYER Z: " + getChunkCenter(playerChunkPos).z);
                //Debug.Log("PLAYER X: " + int.Parse(c.Key.Split(' ')[0]));
                //Debug.Log("PLAYER Z: " + int.Parse(c.Key.Split(' ')[2]));
                //Debug.Log("RESULTADO FINAL X: "+Mathf.Abs(getChunkCenter(playerChunkPos).x - int.Parse(c.Key.Split(' ')[0])));
                //Debug.Log("RESULTADO FINAL Y: " + Mathf.Abs(getChunkCenter(playerChunkPos).z - int.Parse(c.Key.Split(' ')[2])));
                toRemove.Add(c.Key);
            }
        }
        StartCoroutine(RemoveChunks());
        drawing = false;
    }

    void Building(Vector3 chunkPos, int rad)
    {
        StartCoroutine(BuildRecursiveWorld(chunkPos, rad));
    }

    void Drawing()
    {
        StartCoroutine(DrawChunks());
    }

    Vector3 whichChunk(Vector3 position)
    {
        Vector3 chunkPos = new Vector3();
        chunkPos.x = Mathf.Floor(position.x / chunkSize) * chunkSize;
        chunkPos.y = Mathf.Floor(position.y / chunkSize) * chunkSize;
        chunkPos.z = Mathf.Floor(position.z / chunkSize) * chunkSize;

        return chunkPos;
    }

    Vector3 getChunkCenter(Vector3 pos)
    {
        pos.x = whichChunk(pos).x / 2;
        pos.x = whichChunk(pos).y;
        pos.z = whichChunk(pos).z / 2;
        return new Vector3(pos.x, pos.y, pos.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        player.SetActive(false);
        chunkDict = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        Vector3 ppos = player.transform.position;
        player.transform.position = new Vector3(ppos.x, Utils.GenerateHeight(ppos.x, ppos.z) + 1, ppos.z);
        lastBuildPos = player.transform.position;
        Building(whichChunk(lastBuildPos), radius);
        Drawing();
        player.SetActive(true);
    }

    private void Update()
    {
        Vector3 movement = player.transform.position - lastBuildPos;
        if (movement.magnitude > chunkSize)
        {
            StopAllCoroutines();
            lastBuildPos = player.transform.position;
            Building(whichChunk(lastBuildPos), radius);
            Drawing();
        }
        if (!drawing) Drawing();

        bool rightClick = Input.GetMouseButtonDown(1);
        if (rightClick)
        {
            
            RaycastHit hitInfo;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, 4f))
            {
                
                Vector3 pointInTargetBlock = hitInfo.point + cam.transform.forward * .01f;
            
                Debug.Log(pointInTargetBlock);
               
                int chunkPosX = Mathf.FloorToInt(pointInTargetBlock.x / 16f) * 16;
                int chunkPosY = Mathf.FloorToInt(pointInTargetBlock.y / 16f) * 16;
                int chunkPosZ = Mathf.FloorToInt(pointInTargetBlock.z / 16f) * 16;
                

                foreach (KeyValuePair<string, Chunk> c in chunkDict)
                {
                    if (int.Parse(c.Key.Split(' ')[0]) == whichChunk(pointInTargetBlock).x && int.Parse(c.Key.Split(' ')[1]) == whichChunk(pointInTargetBlock).y && int.Parse(c.Key.Split(' ')[2]) == whichChunk(pointInTargetBlock).z)
                    {
                       
                        int bix = Mathf.RoundToInt(pointInTargetBlock.x) - chunkPosX;
                        int biy = Mathf.RoundToInt(pointInTargetBlock.y) -chunkPosY;
                        int biz = Mathf.RoundToInt(pointInTargetBlock.z) -chunkPosZ;
                        
                        if (rightClick)
                        {
                            c.Value.chunkData[bix, biy, biz] = new Block(Block.BlockType.AIR, new Vector3(bix, biy, biz), c.Value, material);

                            
                            MeshCollider mesh = c.Value.goChunk.GetComponent<MeshCollider>();
                            MeshFilter meshf = c.Value.goChunk.GetComponent<MeshFilter>();
                            MeshRenderer meshr = c.Value.goChunk.GetComponent<MeshRenderer>();

                            c.Value.DrawChunkAfter(mesh, meshf, meshr, c.Value);
                        }
                    }
                }

                

            }

        }
    }
}
