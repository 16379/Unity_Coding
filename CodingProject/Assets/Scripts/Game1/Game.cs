using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject
{
    const int saveVersion = 1;

    List<Shape> shapes;
    float creationProgress;
    float destructionProgress;

    //public PersistableObject prefab;
    public ShapeFactory shapeFactory;
    public PersistentStorage storage;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.F;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    public KeyCode destroyKey = KeyCode.X;

    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }

    private void Awake()
    {
        shapes = new List<Shape>();
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

        creationProgress += Time.deltaTime * CreationSpeed;
        Debug.Log("creationProgress这个值是:" + creationProgress);
        while (creationProgress >= 1f)
        {
            Debug.LogError("创建一个");
            creationProgress -= 1f;
            CreateShape();
        }
        destructionProgress += Time.deltaTime * DestructionSpeed;
        Debug.Log("destructionProgress这个值是:" + destructionProgress);
        while (destructionProgress >= 1)
        {
            Debug.LogError("删除一个");
            destructionProgress -= 1;
            DestroyShape();
        }
    }

    void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
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

}
