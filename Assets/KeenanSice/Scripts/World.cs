using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile
{
    //KS - Don't expect to see this colour, but it is bright enough to show there is a problem.
    public Color mColour = Color.magenta;
    public GameObject mTileObject;
};

public class WallTile : WorldTile
{
    public WallTile()
    {
        mColour = Color.white;
    }
};

public class WalkableTile : WorldTile
{
    public float mSpeedModifier = 1.0f;

    public WalkableTile()
    {
        mColour = Color.black;
    }
};

public class HealthTile : WalkableTile
{
    public HealthTile()
    {
        mColour = Color.red;
    }
}

public class AmmoTile : WalkableTile
{
    public AmmoTile()
    {
        mColour = Color.blue;
    }
}

public class SpawnpointTile : WalkableTile
{
    public SpawnpointTile()
    {
        mColour = Color.green;
    }
}


public class World : MonoBehaviour
{
    public static Color[] playerColours = { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.cyan };

    public static Dictionary<Vector2Int, WorldTile> worldTiles = new Dictionary<Vector2Int, WorldTile>();

    public static Vector2Int worldTileDimensions = new Vector2Int(32, 32);

    public Vector2Int WorldDimensions = new Vector2Int(32, 32);

    //KS - Optional, will generate using perlin if not specified.
    public Texture2D worldGenerationTexture;

    //KS - Optional, leave as -1 if random is wanted.
    public int perlinSeed = -1;

    //KS - Defaulting to 1 till it's ready.
    public int totalAgentsOnATeam = 1;

    public GameObject baseTileObject;
    public GameObject baseAgentObject;

    public static List<SpawnpointTile> spawnpointTiles = new List<SpawnpointTile>();
    public static List<HealthTile> healthTiles = new List<HealthTile>();
    public static List<AmmoTile> ammoTiles = new List<AmmoTile>();
    public static List<AgentController> agents = new List<AgentController>();
    public int WorldSize;

    void Start()
    {
        GenerateWorld();
        SetupCamera();
        SetupAgents();
    }

    void SetupAgents()
    {
        Vector3 worldCenterPosition = new Vector3(worldTileDimensions.x, worldTileDimensions.y, 0) * 0.5f;

        float teamSpawnOffset = totalAgentsOnATeam * 0.5f;

        for (int x = 0; x < totalAgentsOnATeam; x++)
        {
            for (int i = 0; i < spawnpointTiles.Count; i++)
            {
                //KS - Don't spawn any more agents if we don't have the colours setup for it.
                if (i < playerColours.Length)
                {
                    GameObject agent = Instantiate(baseAgentObject);
                    agent.transform.position = spawnpointTiles[i].mTileObject.transform.position;

                    //KS - Make face center.
                    Vector3 centerLookDirection = Vector3.Normalize(worldCenterPosition - agent.transform.position);
                    float angle = Mathf.Atan2(centerLookDirection.y, centerLookDirection.x) * Mathf.Rad2Deg;
                    Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                    agent.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    agent.GetComponent<SpriteRenderer>().color = playerColours[i];
                    agent.GetComponent<AgentController>().teamNumber = i;
                    agents.Add(agent.GetComponent<AgentController>());
                }
            }
        }
    }


    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3(worldTileDimensions.x * 0.5f - 0.5f, worldTileDimensions.y * 0.5f - 0.5f, Camera.main.transform.position.z);
        Camera.main.orthographicSize = (worldTileDimensions.x > worldTileDimensions.y ? worldTileDimensions.x * 0.5f : worldTileDimensions.y * 0.5f) * 1.25f;
    }


    void GenerateWorld()
    {
        worldTiles.Clear();

        worldTileDimensions = WorldDimensions;

        if(worldGenerationTexture != null)
        {
            GenerateWorldFromImage();
            return;
        }
        else
        {
            GenerateWorldFromPerlin();
            return;
        }
    }

    void GenerateWorldFromPerlin()
    {
        if (perlinSeed > -1)
        {
            Random.InitState(perlinSeed);
        }

        float rn = Random.value;

        for (int y = 0; y < worldTileDimensions.y; y++)
        {
            for (int x = 0; x < worldTileDimensions.x; x++)
            {
                //KS - We can determine whether our map will have borders with Perlin, so lets generate some walls on the borders.
                if(x == 0 || y == 0 || x == worldTileDimensions.x - 1 || y == worldTileDimensions.y - 1)
                {
                    SetupTile(new WallTile(), x, y);

                    continue;
                }

                float a = Mathf.PerlinNoise(x * 0.1f + rn * 100.0f, y * 0.1f + rn * 100.0f);

                WorldTile newTile = new WorldTile();

                if (a >= 0.3f)
                {
                    newTile = new WalkableTile();
                }
                else
                {
                    newTile = new WallTile();
                }

                SetupTile(newTile, x, y);
            }
        }
    }

    void SetupTile(WorldTile newTile, int x, int y)
    {
        //KS - Quick optimisation, walkable tiles are black they don't need an actual GameObject.
        if (newTile.GetType() != typeof(WalkableTile))
        {
            newTile.mTileObject = Instantiate(baseTileObject);
            newTile.mTileObject.transform.position = new Vector3(x, y);
            newTile.mTileObject.transform.SetParent(transform);
            newTile.mTileObject.GetComponent<SpriteRenderer>().color = newTile.mColour;
        }

        if(newTile is WallTile)
        {
            newTile.mTileObject.AddComponent<BoxCollider2D>();
        }
        else if(newTile is AmmoTile)
        {
            newTile.mTileObject.layer = 8; //No Range Layer.
        }

        worldTiles.Add(new Vector2Int(x,y), newTile);
    }


    //KS - Make making levels great again!
    void GenerateWorldFromImage()
    {
        for(int y = 0; y < worldTileDimensions.y; y++)
        {
            float yPosPercent = y / (float)worldTileDimensions.y;

            for(int x = 0; x < worldTileDimensions.x; x++)
            {
                float xPosPercent = x / (float)worldTileDimensions.x;

                //KS - We'll use this colour to determine the tile.
                Color colourForTile = worldGenerationTexture.GetPixel((int)(xPosPercent * worldGenerationTexture.width), (int)(yPosPercent * worldGenerationTexture.height));

                WorldTile newTile = new WorldTile();

                if (colourForTile == Color.black)
                {
                    newTile = new WalkableTile();
                }
                else if(colourForTile == Color.white)
                {
                    newTile = new WallTile();
                }
                else if (colourForTile == Color.red)
                {
                    newTile = new HealthTile();
                    healthTiles.Add(newTile as HealthTile);
                }
                else if (colourForTile == Color.blue)
                {
                    newTile = new AmmoTile();
                    ammoTiles.Add(newTile as AmmoTile);
                }
                else if (colourForTile == Color.green)
                {
                    newTile = new SpawnpointTile();
                    spawnpointTiles.Add(newTile as SpawnpointTile);
                }

                SetupTile(newTile, x, y);
            }
        }
    }
    public static int WorldMaxSize
    {
        get
        {
            return worldTileDimensions.x * worldTileDimensions.y;
        }
    }

    public static bool IsPositionWalkable(Vector2Int position)
    {
        WorldTile tile;

        //KS - Dictionary look up for valid position.
        if (worldTiles.TryGetValue(position, out tile))
        {
            return (tile is WalkableTile);
        }

        return false;
    }
}
