using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK }
    public enum BlockType { GRASS, DIRT, STONE, TREEWOOD, LEAF, LAVA, AIR};
    Material material;
    public BlockType bType;
    Chunk owner;
    Vector3 pos;

    bool isSolid;

    static Vector2 GrassSide_LBC = new Vector2(3f, 15f) / 16;
    static Vector2 GrassTop_LBC = new Vector2(2f, 6f) / 16;
    static Vector2 Dirt_LBC = new Vector2(2f, 15f) / 16;
    static Vector2 Stone_LBC = new Vector2(1f, 15f) / 16;
    static Vector2 Wood_LBC = new Vector2(4f, 8f) / 16;
    static Vector2 Leaf_LBC = new Vector2(6f, 12f) / 16;
    static Vector2 Lava_LBC = new Vector2(15f, 0f) / 16;

    Vector2[,] blockUVs =
    {
        {GrassTop_LBC, GrassTop_LBC + new Vector2(1f, 0f)/16, GrassTop_LBC + new Vector2(0f, 1f)/16, GrassTop_LBC + new Vector2(1f, 1f)/16 },
        {GrassSide_LBC, GrassSide_LBC + new Vector2(1f, 0f)/16, GrassSide_LBC + new Vector2(0f, 1f)/16, GrassSide_LBC + new Vector2(1f, 1f)/16 },
        {Dirt_LBC, Dirt_LBC + new Vector2(1f, 0f)/16, Dirt_LBC + new Vector2(0f, 1f)/16, Dirt_LBC + new Vector2(1f, 1f)/16 },
        {Stone_LBC, Stone_LBC + new Vector2(1f, 0f)/16, Stone_LBC + new Vector2(0f, 1f)/16, Stone_LBC + new Vector2(1f, 1f)/16 },
         {Wood_LBC, Wood_LBC + new Vector2(1f, 0f)/16, Wood_LBC + new Vector2(0f, 1f)/16, Wood_LBC + new Vector2(1f, 1f)/16 },
        {Leaf_LBC, Leaf_LBC + new Vector2(1f, 0f)/16, Leaf_LBC + new Vector2(0f, 1f)/16, Leaf_LBC + new Vector2(1f, 1f)/16 },
        {Lava_LBC, Lava_LBC + new Vector2(1f, 0f)/16, Lava_LBC + new Vector2(0f, 1f)/16, Lava_LBC + new Vector2(1f, 1f)/16 }
    };

    public Block(BlockType bType, Vector3 pos, Chunk owner, Material material)
    {
        this.bType = bType;
        this.owner = owner;
        this.pos = pos;
        this.material = material;
        if (bType == BlockType.AIR)
        {
            isSolid = false;
        }
        else
        {
            isSolid = true;
        }
    }

    void CreateQuad(Cubeside side)
    {
        Mesh mesh = new Mesh();

        Vector3 v0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 v1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 v2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 v3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 v4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 v5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 v6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 v7 = new Vector3(-0.5f, 0.5f, -0.5f);

        Vector2 uv00 = new Vector2(0, 0);
        Vector2 uv01 = new Vector2(0, 1);
        Vector2 uv10 = new Vector2(1, 0);
        Vector2 uv11 = new Vector2(1, 1);

        if (bType == BlockType.GRASS && side == Cubeside.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }else if (bType == BlockType.GRASS && side == Cubeside.BOTTOM)
        {
            uv00 = blockUVs[2, 0];
            uv10 = blockUVs[2, 1];
            uv01 = blockUVs[2, 2];
            uv11 = blockUVs[2, 3];
        }
        else
        {
            
            uv00 = blockUVs[(int)(bType + 1), 0];
            uv10 = blockUVs[(int)(bType + 1), 1];
            uv01 = blockUVs[(int)(bType + 1), 2];
            uv11 = blockUVs[(int)(bType + 1), 3];
            
        }

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        int[] triangles = new int[6];
        Vector2[] uv = new Vector2[4];
        switch (side)
        {
            case Cubeside.FRONT:
                vertices = new Vector3[] { v4, v5, v1, v0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                break;
            case Cubeside.BOTTOM:
                vertices = new Vector3[] { v0, v1, v2, v3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                break;
            case Cubeside.TOP:
                vertices = new Vector3[] { v7, v6, v5, v4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                break;
            case Cubeside.LEFT:
                vertices = new Vector3[] { v7, v4, v0, v3 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                break;
            case Cubeside.RIGHT:
                vertices = new Vector3[] { v5, v6, v2, v1 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                break;
            case Cubeside.BACK:
                vertices = new Vector3[] { v6, v7, v3, v2 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                break;
        }
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };
        uv = new Vector2[] { uv11, uv01, uv00, uv10 };

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.position = this.pos;
        quad.transform.parent = owner.goChunk.transform;

        MeshFilter mf = quad.AddComponent<MeshFilter>();
        mf.mesh = mesh;
    }

    int convertToLocalIndex(int i)
    {
        if (i == -1)
        {
            return World.chunkSize - 1;
        }
        if (i == World.chunkSize)
        {
            return 0;
        }
        return i;
    }

    bool HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] chunkdata = owner.chunkData;

        if (x < 0 || x >= World.chunkSize || y < 0 || y >= World.chunkSize || z < 0 || z >= World.chunkSize)
        {
            Vector3 neighChunkPos = owner.goChunk.transform.position + new Vector3((x-(int)pos.x)*World.chunkSize, (y - (int)pos.y) * World.chunkSize, (z - (int)pos.z) * World.chunkSize);
            string chunkName = World.CreateChunkName(neighChunkPos);

            x = convertToLocalIndex(x);
            y = convertToLocalIndex(y);
            z = convertToLocalIndex(z);

            Chunk c;
            if(World.chunkDict.TryGetValue(chunkName, out c))
            {
                chunkdata = c.chunkData;
            }
            else
            {
                return false;
            }
        }
        else
        {
            chunkdata = owner.chunkData;
        }
        try
        {
            return chunkdata[x, y, z].isSolid;
        }
        catch (System.IndexOutOfRangeException ex) { }
        return false;
    }

    public void Draw()
    {
        if(bType == BlockType.AIR)
        {
            return;
        }

        if (!HasSolidNeighbour((int)pos.x - 1, (int)pos.y, (int)pos.z))
        {
            CreateQuad(Cubeside.LEFT);
        }
        if (!HasSolidNeighbour((int)pos.x + 1, (int)pos.y, (int)pos.z))
        {
            CreateQuad(Cubeside.RIGHT);
        }
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y - 1, (int)pos.z))
        {
            CreateQuad(Cubeside.BOTTOM);
        }
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y + 1, (int)pos.z))
        {
            CreateQuad(Cubeside.TOP);
        }
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z - 1))
        {
            CreateQuad(Cubeside.BACK);
        }
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z + 1))
        {
            CreateQuad(Cubeside.FRONT);
        }
    }
}
