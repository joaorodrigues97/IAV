using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Block[,,] chunkData;
    public GameObject goChunk;
    public enum chunkStatus { DRAW, DONE };
    Material material;
    public chunkStatus status;
    public List<Vector3> treePos;

    public Chunk(Vector3 pos, Material material)
    {
        goChunk = new GameObject(World.CreateChunkName(pos));
        goChunk.transform.position = pos;
        this.material = material;
        treePos = new List<Vector3>();
        BuildChunk();
    }

    void BuildChunk()
    {
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)goChunk.transform.position.x + x;
                    int worldY = (int)goChunk.transform.position.y + y;
                    int worldZ = (int)goChunk.transform.position.z + z;
                    int h = Utils.GenerateHeight(worldX, worldZ);
                    int hs = Utils.GenerateStoneHeight(worldX, worldZ);

                    if (worldY <= hs)
                    {
                        if (Utils.fBM3D(worldX, worldY, worldZ, 1, 0.5f) < 0.51f)
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, this, material);
                        }
                        else
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);
                        }
                    }
                    else if (worldY == h)
                    {

                        if ((Random.Range(0f, 1f) < 0.005f) && x < 15 && z < 15 && x > 0 && z > 0)
                        {
                            
                             chunkData[x, y, z] = new Block(Block.BlockType.TREEWOOD, pos, this, material);
                             treePos.Add(new Vector3(x, y, z));
                            
                        }
                        else
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, this, material);
                        }

                    }
                    else if (worldY < h)
                    {

                        chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, this, material);
                    }
                    else
                    {
                        /*if (chunkData[x, y - 1,z].bType == Block.BlockType.TREEWOOD)
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.TREEWOOD, pos, this, material);

                        }
                        else
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);
                        }*/

                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);

                    }
                }
            }
            
            status = chunkStatus.DRAW;
        }
        
    }

    public void GenerateTrees()
    {
        foreach(Vector3 pos in treePos){
            int random = Random.Range(4,5);
            for (int i = 1; i < random; i++)
            {
                if (pos.y+i < 15)
                {
                    chunkData[(int)pos.x, (int)pos.y + i, (int)pos.z] = new Block(Block.BlockType.TREEWOOD, new Vector3((int)pos.x, (int)pos.y + i, (int)pos.z), this, material);
                    if (i >= random / 2)
                    {
                        chunkData[(int)pos.x, (int)pos.y + i, (int)pos.z + 1] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x, (int)pos.y + i, (int)pos.z + 1), this, material);
                        chunkData[(int)pos.x + 1, (int)pos.y + i, (int)pos.z + 1] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x + 1, (int)pos.y + i, (int)pos.z + 1), this, material);
                        chunkData[(int)pos.x + 1, (int)pos.y + i, (int)pos.z] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x + 1, (int)pos.y + i, (int)pos.z), this, material);
                        chunkData[(int)pos.x + 1, (int)pos.y + i, (int)pos.z - 1] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x + 1, (int)pos.y + i, (int)pos.z - 1), this, material);
                        chunkData[(int)pos.x, (int)pos.y + i, (int)pos.z - 1] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x, (int)pos.y + i, (int)pos.z - 1), this, material);
                        chunkData[(int)pos.x - 1, (int)pos.y + i, (int)pos.z - 1] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x - 1, (int)pos.y + i, (int)pos.z - 1), this, material);
                        chunkData[(int)pos.x - 1, (int)pos.y + i, (int)pos.z + 1] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x - 1, (int)pos.y + i, (int)pos.z + 1), this, material);
                        chunkData[(int)pos.x - 1, (int)pos.y + i, (int)pos.z] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x - 1, (int)pos.y + i, (int)pos.z), this, material);
                    }

                    if (i == random - 1 && pos.y + i + 1 <= 15)
                    {
                        chunkData[(int)pos.x, (int)pos.y + i + 1, (int)pos.z] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x, (int)pos.y + i + 1, (int)pos.z), this, material);
                    }
                }
                else if(pos.y+i == 15)
                {
                    chunkData[(int)pos.x, (int)pos.y + i , (int)pos.z] = new Block(Block.BlockType.LEAF, new Vector3((int)pos.x, (int)pos.y + i , (int)pos.z), this, material);
                }
                
            }
        }
    }

    public void DrawChunk()
    {
        GenerateTrees();
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    
                    chunkData[x, y, z].Draw();
                }
            }
        }
        CombineQuads();
        MeshCollider collider = goChunk.AddComponent<MeshCollider>();
        collider.sharedMesh = goChunk.GetComponent<MeshFilter>().mesh;
        status = chunkStatus.DONE;
    }




    void CombineQuads()
    {
        MeshFilter[] meshFilters = goChunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        MeshFilter mf = (MeshFilter)goChunk.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);

        MeshRenderer renderer = goChunk.AddComponent<MeshRenderer>();
        renderer.material = material;

        foreach (Transform quad in goChunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }

    public void DrawChunkAfter(MeshCollider collider,MeshFilter mf, MeshRenderer mr, Chunk chunk)
    {
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    chunk.chunkData[x, y, z].Draw();
                }
            }
        }
        CombineQuadsAfter(mf,mr);
        //MeshCollider collider = goChunk.AddComponent<MeshCollider>();
        collider.sharedMesh = goChunk.GetComponent<MeshFilter>().mesh;
        status = chunkStatus.DONE;
    }




    void CombineQuadsAfter(MeshFilter mf, MeshRenderer renderer)
    {
        MeshFilter[] meshFilters = goChunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //MeshFilter mf = (MeshFilter)goChunk.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mf.mesh.CombineMeshes(combine);

        //MeshRenderer renderer = goChunk.AddComponent<MeshRenderer>();
        renderer.material = material;

        foreach (Transform quad in goChunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
}
