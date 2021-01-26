using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField]
    Shape[] prefabs;
    [SerializeField]
    Material[] materials;

    public Shape Get(int shapeId)
    {
        Shape instance = Instantiate(prefabs[shapeId]);
        instance.ShapeId = shapeId;
        return instance;
    }
    public Shape GetRandom()
    {
        return Get(Random.Range(0, prefabs.Length));
    }

    public int MaterialId { get; private set; }

    public void SetMaterial(Material material,int materialId)
    {
        GetComponent<MeshRenderer>().material = material;
        MaterialId = materialId;
    }

}
