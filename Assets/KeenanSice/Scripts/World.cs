using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile 
{
    //KS - Don't expect to see this colour, but it is bright enough to show there is a problem.
    public Color mColour = Color.magenta;
    public GameObject mTileObject;

    public virtual void Initialise() { }
};

public class WallTile : WorldTile
{
    public WallTile()
    {
        mColour = Color.white;
    }

    public override void Initialise()
    {
        base.Initialise();
        mTileObject.AddComponent<BoxCollider2D>();
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

//KS - Base class for pickup tiles.
public class PickupTile : WalkableTile
{
    public BasePickup pickupComponent;

    public bool Pickup(Controller instigator)
    {
        return pickupComponent.Pickup(instigator);
    }
}

public class HealthTile : PickupTile
{
    public HealthTile()
    {
        mColour = Color.red;
    }

    public override void Initialise()
    {
        base.Initialise();
        pickupComponent = mTileObject.AddComponent<HealthPickup>();
    }
}

public class AmmoTile : PickupTile
{
    public AmmoTile()
    {
        mColour = Color.blue;
    }

    public override void Initialise()
    {
        base.Initialise();
        pickupComponent = mTileObject.AddComponent<AmmoPickup>();
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

    public static Vector2Int worldTileDimensions = Vector2Int.zero;

    //KS - Only need to change this if using perlin for map generation, else the map texture dimensions override.
    public Vector2Int WorldDimensions = new Vector2Int(32, 32);

    //KS - Optional, will generate using perlin if not specified.
    public Texture2D worldGenerationTexture;

    //KS - Optional, leave as -1 if random is wanted.
    public int perlinSeed = -1;

    public GameObject baseTileObject;
    public GameObject baseAgentObject;

    public static List<SpawnpointTile> spawnpointTiles = new List<SpawnpointTile>();
    public static List<HealthTile> healthTiles = new List<HealthTile>();
    public static List<AmmoTile> ammoTiles = new List<AmmoTile>();

    public static List<List<Controller>> agentTeams = new List<List<Controller>>();

    public int totalHumanPlayers = 1;
    public int totalAgentsOnATeam = 1;

    //KS - Raycasts should only hit walls.
    public const int enemyAttackLayerMask = ~(1 << 10);

    public static int AgentCount
    {
        get
        {
            int count = 0;
            foreach (List<Controller> team in agentTeams)
            {
                count += team.Count;
            }
            return count;
        }
    }

    public static int WorldMaxSize
    {
        get
        {
            return worldTileDimensions.x * worldTileDimensions.y;
        }
    }

    void Start()
    {
        GenerateWorld();
        SetupCamera();
        SetupAgents();
    }

    //KS - Handle setting up agents for AI and Humans to use ingame.
    void SetupAgents()
    {
        Vector3 worldCenterPosition = new Vector3(worldTileDimensions.x, worldTileDimensions.y, 0) * 0.5f;

        for (int x = 0; x < totalAgentsOnATeam; x++)
        {
            for (int i = 0; i < spawnpointTiles.Count; i++)
            {
                //KS - Don't spawn any more agents if we don't have the colours setup for it.
                if (i < playerColours.Length)
                {
                    //KS - [Start] Spawn prefab agent at position facing the center of the map.
                    Vector3 spawnPosition = spawnpointTiles[i].mTileObject.transform.position;

                    //KS - Make face center.
                    Vector3 centerLookDirection = Vector3.Normalize(worldCenterPosition - spawnPosition);
                    float angle = Mathf.Atan2(centerLookDirection.y, centerLookDirection.x) * Mathf.Rad2Deg;
                    Quaternion spawnRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    GameObject agent = Instantiate(baseAgentObject, spawnPosition, spawnRotation);
                    //KS - [End] Spawn prefab agent at position facing the center of the map.

                    //KS - Change agent colour to match team colour.
                    agent.GetComponent<SpriteRenderer>().color = playerColours[i];

                    bool isLeaderOfTeam = x == 0;

                    //KS - Determine which controller the agent needs, if human players are playing they will always be the leader and have AI teammates.
                    Controller controller = isLeaderOfTeam && i < totalHumanPlayers ? (Controller)agent.AddComponent<PlayerController>() : agent.AddComponent<AgentController>();
                    controller.teamNumber = i;

                    if (isLeaderOfTeam)
                    {
                        agentTeams.Add(new List<Controller>());
                    }

                    agentTeams[i].Add(controller);
                    SetupTeamLeader(i);
                }
            }
        }
    }

    //KS - Move camera to center of map, and adjust orthographic to fit map onto screen.
    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3(worldTileDimensions.x * 0.5f - 0.5f, worldTileDimensions.y * 0.5f - 0.5f, Camera.main.transform.position.z);
        Camera.main.orthographicSize = (worldTileDimensions.x > worldTileDimensions.y ? worldTileDimensions.x * 0.5f : worldTileDimensions.y * 0.5f) * 1.25f;
    }


    //KS - Controllers need to be removed when killed, this could result in a new team leader being established.
    public static void RemoveControllerFromTeam(Controller controller)
    {
        agentTeams[controller.teamNumber].Remove(controller);
        SetupTeamLeader(controller.teamNumber);
    }

    
    //KS - Team leaders that are AI controlled need to be given a behavior to play.
    private static void SetupTeamLeader(int teamNumber)
    {
        //KS - Check if team has any members left alive.
        if (agentTeams[teamNumber].Count > 0)
        {
            //KS - The leader will always be the first element in the array.
            Controller leader = agentTeams[teamNumber][0];

            //KS - If they are not a human player and have not already been given a behaviour, then we do it here.
            if (!(leader is PlayerController) && !agentTeams[teamNumber][0].GetComponent<AgentBehaviour>())
            {
                agentTeams[teamNumber][0].gameObject.AddComponent<AgentBehaviour>();
            }
        }
    }

    //KS - Route the world generation depending on if a map was specified.
    void GenerateWorld()
    {
        //KS - Might as well clear, so this function can be used as a regenerate.
        worldTiles.Clear();

        if(worldGenerationTexture != null)
        {
            worldTileDimensions = new Vector2Int(worldGenerationTexture.width, worldGenerationTexture.height);
            GenerateWorldFromImage();
            return;
        }
        else
        {
            worldTileDimensions = WorldDimensions;
            GenerateWorldFromPerlin();
            return;
        }
    }

    //KS - Mainly for initial debugging, this allows us to create a procedurally generated map using Perlin Noise.
    void GenerateWorldFromPerlin()
    {
        //KS - If the seed is greater than -1, then the user must have a specified a seed, else it will be random.
        if (perlinSeed > -1)
        {
            Random.InitState(perlinSeed);
        }

        float rn = Random.value;

        //KS - Iterate x and y to generate map tiles.
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

                //KS - Generate a value from 0 to 1, we'll use this to determine what tile is being generating.
                float a = Mathf.PerlinNoise(x * 0.1f + rn * 100.0f, y * 0.1f + rn * 100.0f);

                const float wallChance = 0.3f;
                WorldTile newTile = a < wallChance ? (WorldTile)new WallTile() : new WalkableTile();

                SetupTile(newTile, x, y);
            }
        }
    }

    /// KS Mini Documentation [Start]
    /// 
    /// These maps are based on a pixel level, 1 Unity unit is equal to one pixel.
    /// Red pixels (255,0,0) will generate health tiles which agents can pickup health from.
    /// Green pixels (0,255,0) will generate spawnpoints which determines where teams will spawn from. 1 spawnpoint per team.
    /// Blue pixels (0,0,255) will generate ammo tiles which agents can pickup ammo from.
    /// 
    /// White pixels (255,255,255) will generate walls that the agents cannot move through.
    /// Black pixels (0,0,0) will blank areas, treat these as walking spaces or just air.
    /// 
    /// Maps should have a height and width that are power of two, this is to avoid problems with compression/subpixels, and also for performance reasons.
    /// 
    /// KS Mini Documentation [End]
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

    void SetupTile(WorldTile newTile, int x, int y)
    {
        //KS - Quick optimisation, walkable tiles are black so they do not need an actual GameObject.
        if (newTile.GetType() != typeof(WalkableTile))
        {
            newTile.mTileObject = Instantiate(baseTileObject);
            newTile.mTileObject.transform.position = new Vector3(x, y);
            newTile.mTileObject.transform.SetParent(transform);
            newTile.mTileObject.GetComponent<SpriteRenderer>().color = newTile.mColour;
            newTile.mTileObject.layer = 8;
        }

        newTile.Initialise();
        worldTiles.Add(new Vector2Int(x, y), newTile);
    }
}
