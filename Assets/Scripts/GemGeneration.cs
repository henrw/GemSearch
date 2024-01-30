using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGeneration : MonoBehaviour
{
    public GameObject gemPrefab;
    private Vector3 minBounds = new Vector3(0f, 3f, -9f);
    private Vector3 maxBounds = new Vector3(33f, 0f, 9f);
    public int numTargets = 5;
    public float minDistance = 1.0f;
    private float scaleMin = 1.0f;
    private float scaleMax = 1.0f;
    public GameObject targetGameObject;
    public bool loadFromFile = true;
    public TextAsset csvFile;

    public float collisionWaitTime = 3.0f;
    private float collisionSphereRadius = 1.0f;

    public Vector3[] gemPositions; // Store gems' coordinates

    private Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(minBounds.x, maxBounds.x),
            UnityEngine.Random.Range(minBounds.y, maxBounds.y),
            UnityEngine.Random.Range(minBounds.z, maxBounds.z)
        );
        return randomPosition;
    }

    private float GetRandomScale()
    {
        return UnityEngine.Random.Range(scaleMin, scaleMax);
    }

    private IEnumerator CreateGem(Vector3 position, float gemScale, int gemId, Action<bool> callback)
    {
        GameObject gem = Instantiate(gemPrefab, position, Quaternion.identity);
        gem.transform.localScale = new Vector3(gem.transform.localScale.x, gem.transform.localScale.y, gem.transform.localScale.z) * gemScale;
        gem.name = "Gem"+gemId.ToString();

        yield return CheckValidity(gem, gemId + 1, result =>
        {
            if (!result)
            {
                Destroy(gem);
                //Debug.Log("destroy: Gem" + gemId.ToString());
            }
            callback(result);
        });

    }

    private void CreateGemFromFile(Vector3 position, float gemScale, int gemId)
    {
        GameObject gem = Instantiate(gemPrefab, position, Quaternion.identity);
        gem.transform.localScale = new Vector3(gem.transform.localScale.x, gem.transform.localScale.y, gem.transform.localScale.z) * gemScale;
        gem.name = "Gem" + gemId.ToString();
    }

    // Check whether (1) the gems are too close to each other and (2) the gems locate within the canvas (TODO)
    private IEnumerator CheckValidity(GameObject gem, int num, Action<bool> callback)
    {
        yield return new WaitForSeconds(collisionWaitTime);

        //Debug.Log("wake " + (num - 1).ToString());
        for (int i = 0; i < num; ++i)
        {
            if (Vector3.Distance(gemPositions[i], gem.transform.position) < minDistance)
            {
                callback(false);
                yield break;
            }
        }

        Collider[] colliders = Physics.OverlapSphere(gem.transform.position, collisionSphereRadius);
        if (colliders.Length == 1) // Only itself
        {
            callback(false);
            yield break;
        }

        callback(true);
    }


    public List<Vector4> ReadCoordinates()
    {
        List<Vector4> coordinates = new List<Vector4>();

        if (csvFile != null)
        {
            string[] lines = csvFile.text.Split('\n');
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 4)
                {
                    float x = float.Parse(parts[0]);
                    float y = float.Parse(parts[1]);
                    float z = float.Parse(parts[2]);
                    float w = float.Parse(parts[3]);
                    coordinates.Add(new Vector4(x, y, z, w));
                }
            }
        }
        return coordinates;
    }

    private IEnumerator GenerateGems()
    {
        for (int i = 0; i < numTargets; i++)
        {
            Vector3 randomPosition = GetRandomPosition();
            float randomRadius = GetRandomScale();
            yield return StartCoroutine(CreateGem(randomPosition, randomRadius, i, success => {
                if (!success)
                {
                    i--;
                    //Debug.Log("regenerate");
                }
                else
                {
                    gemPositions[i] = GameObject.Find("Gem"+i.ToString()).transform.position;
                    //Debug.Log(gemPositions[i]);
                }
            }));
        }
    }

    private void LoadGemsFromFile()
    {
        List<Vector4> coordinates = ReadCoordinates();
        for (int i = 0; i < coordinates.Count; i++)
        {
            Vector3 position = new Vector3(coordinates[i].x, coordinates[i].y, coordinates[i].z);
            float radius = coordinates[i].w;
            CreateGemFromFile(position, radius, i);
        }
    }

    void Start()
    {
        gemPositions = new Vector3[numTargets];
        if (loadFromFile == true)
        {
            LoadGemsFromFile();
        }
        else
        {
            StartCoroutine(GenerateGems());
        }
    }

    void Update()
    {

    }
}
