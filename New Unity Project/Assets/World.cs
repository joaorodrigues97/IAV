using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public static int chunkSize = 16;
    public static int radius = 1;
    public static ConcurrentDictionary<string, Chunk> chunkDict;
    public static List<string> toRemove = new List<string>();
    public GameObject player;
    Vector3 lastBuildPos;
    bool drawing;

    public static string CreateChunkName(Vector3 v)
    {
        return (int)v.x + " " + (int)v.y + " " + (int)v.z;
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
            if (c.Value.goChunk && Vector3.Distance(player.transform.position, c.Value.goChunk.transform.position) > chunkSize * radius)
            {
                toRemove.Add(c.Key);
            }
        }
        StartCoroutine(RemoveChunks());
        drawing = false;
    }

    void Drawing()
    {
        StartCoroutine(DrawChunks());
    }

    void Building(Vector3 chunkPos, int radius)
    {
        StartCoroutine(BuildRecursiveWorld(chunkPos, radius));
    }

    IEnumerator BuildRecursiveWorld(Vector3 chunkPos, int radius)
    {
       
        int x = (int)chunkPos.x;
        int y = (int)chunkPos.y;
        int z = (int)chunkPos.z;
        BuildChunkAt(chunkPos);
        yield return null;
        if (--radius < 0)
        {
            yield break;
        }
        Building(new Vector3(x, y, z + chunkSize), radius);
        yield return null;
        Building(new Vector3(x, y, z - chunkSize), radius);
        yield return null;
        Building(new Vector3(x+chunkSize, y, z), radius);
        yield return null;
        Building(new Vector3(x-chunkSize, y, z), radius);
        yield return null;
        Building(new Vector3(x, y+chunkSize, z), radius);
        yield return null;
        Building(new Vector3(x, y-chunkSize, z), radius);
        yield return null;
    }

    Vector3 whichChunk(Vector3 position)
    {
        Vector3 chunkPos = new Vector3();
        chunkPos.x = Mathf.Floor((position.x / chunkSize) * chunkSize);
        chunkPos.y = Mathf.Floor((int)(position.y / chunkSize) * chunkSize);
        chunkPos.z = Mathf.Floor((int)(position.z / chunkSize) * chunkSize);
        return chunkPos;
    }

    void Start()
    {
        chunkDict = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        Vector3 playerPos = player.transform.position;
        player.transform.position = new Vector3(playerPos.x,Utils.GenerateHeight(playerPos.x, playerPos.z), playerPos.z);
        lastBuildPos = player.transform.position;
        Building(whichChunk(lastBuildPos),radius);
        Drawing();
        player.SetActive(true);
    }

    void Update()
    {
        Vector3 movement = player.transform.position - lastBuildPos;
        if (movement.magnitude > chunkSize)
        {
            StopAllCoroutines();
            lastBuildPos = player.transform.position;
            Building(whichChunk(lastBuildPos), radius);
            Drawing();
        }
        if (!drawing)
        {
            Drawing();
        }
    }
}
