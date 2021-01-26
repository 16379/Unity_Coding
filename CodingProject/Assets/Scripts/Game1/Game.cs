﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject
{
    const int saveVersion = 1;

    List<Shape> shapes;
    //public PersistableObject prefab;
    public ShapeFactory shapeFactory;
    public PersistentStorage storage;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.F;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    
    private void Awake()
    {
        shapes = new List<Shape>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            CreateObject();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey))
        {
            //Save();
            storage.Save(this);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            //Load();
            BeginNewGame();
            storage.Load(this);
        }
    }

    void CreateObject()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        shapes.Add(instance);
    }

    void BeginNewGame()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            Destroy(shapes[i].gameObject);
        }
        shapes.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(-saveVersion);
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeId);
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
            Shape instance = shapeFactory.Get(shapeId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

}
