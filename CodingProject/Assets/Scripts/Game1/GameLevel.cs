using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : PersistableObject
{
    public static GameLevel Current { get; private set; }

    [SerializeField]
    SpawnZone spawnZone;
    [SerializeField]
    PersistableObject[] persistentObjects;

    public Vector3 SpawnPoint
    {
        get
        {
            return spawnZone.SpawnPoint;
        }
    }

    private void OnEnable()
    {
        Current = this;
        if(persistentObjects == null)
        {
            persistentObjects = new PersistableObject[0];
        }
    }

    //// Start is called before the first frame update
    //void Start()
    //{
    //    Game.Instance.SpawnZoneOfLevel = spawnZone;        
    //}
    public override void Save(GameDataWriter writer)
    {
        writer.Write(persistentObjects.Length);
        for (int i = 0; i < persistentObjects.Length; i++)
        {
            persistentObjects[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++)
        {
            persistentObjects[i].Load(reader);
        }
    }
}
