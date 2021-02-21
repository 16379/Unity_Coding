using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistableObject
{
    public static Game Instance { get; private set; }
    const int saveVersion = 2;

    List<Shape> shapes;
    float creationProgress;
    float destructionProgress;
    int loadedLevelBuildIndex;
    [SerializeField]
    ShapeFactory shapeFactory;
    //public PersistableObject prefab;
    //public ShapeFactory shapeFactory;
    public PersistentStorage storage;
    //public SpawnZone spawnZone;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.F;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    public KeyCode destroyKey = KeyCode.X;

    public int levelCount;

    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }
    public SpawnZone SpawnZoneOfLevel { get; set; }

    private void OnEnable()
    {
        Instance = this;
    }
    private void Start()
    {
        shapes = new List<Shape>();
        if (Application.isEditor)
        {
            //Scene loadedLevel = SceneManager.GetSceneByName("Level 1");
            //if (loadedLevel.isLoaded)
            //{
            //    SceneManager.SetActiveScene(loadedLevel);
            //    return;
            //}
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level "))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
           
        }
        StartCoroutine(LoadLevel(1));
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            CreateShape();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey))
        {
            //Save();
            storage.Save(this,saveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            //Load();
            BeginNewGame();
            storage.Load(this);
        }
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
        else
        {
            for (int i = 1; i <= levelCount; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                }
            }
        }
        //自动创建
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }
        //自动删除
        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1)
        {
            destructionProgress -= 1;
            DestroyShape();
        }
    }

    void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        //t.localPosition = Random.insideUnitSphere * 5f;
        t.localPosition = SpawnZoneOfLevel.SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        shapes.Add(instance);
    }

    void BeginNewGame()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            //Destroy(shapes[i].gameObject);
            shapeFactory.Reclaim(shapes[i]);
        }
        shapes.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        //writer.Write(-saveVersion);
        writer.Write(shapes.Count);
        writer.Write(loadedLevelBuildIndex);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        int version = -reader.ReadInt();
        if(version > saveVersion)
        {
            Debug.LogError("Unsupported future save version" + version);
            return;
        }
        int count = version <= 0 ? -version : reader.ReadInt();
        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
        //int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId,materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

    void DestroyShape()
    {
        if(shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            //Destroy(shapes[index].gameObject);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }
    IEnumerator LoadLevel(int levelBuildIndex)
    {
        enabled = false;
        if(loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        //SceneManager.LoadScene("Level 1",LoadSceneMode.Additive);
        //yield return null;
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }


}
