using UnityEngine;

public static class TerrainUtils
{
    public static Vector3 GetRandomPosition(Terrain terrain, float yOffset = 0f)
    {
        Vector3 terrainPos = terrain.transform.position;
        float width = terrain.terrainData.size.x;
        float length = terrain.terrainData.size.z;

        // pick random X and Z within those bounds
        float x = Random.Range(terrainPos.x, terrainPos.x + width);
        float z = Random.Range(terrainPos.z, terrainPos.z + length);

        // find the height (Y) at that specific X,Z point
        float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrainPos.y;
        float maxHeight = terrainPos.y + terrain.terrainData.size.y;

        // stop the yOffset from going above the terrain max height
        Vector3 randPosOnTerrain = new(x, Mathf.Clamp(y + yOffset, y, maxHeight), z);

        return randPosOnTerrain;
    }
}
