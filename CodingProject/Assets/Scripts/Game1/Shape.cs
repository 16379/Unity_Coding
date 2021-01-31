using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharePropertyBlock;

    Color color;
    int shapeId = int.MinValue;
    MeshRenderer meshRenderer;
    public int ShapeId
    {
        get { return shapeId; }
        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapeId");
            }
        }
    }

    public int MaterialId { get; private set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material, int materialId)
    {
        meshRenderer.material = material;
        MaterialId = materialId;
    }

    public void SetColor(Color color)
    {
        this.color = color;
        //meshRenderer.material.color = color;
        //var propertyBlock = new MaterialPropertyBlock();
        if (sharePropertyBlock == null)
        {
            sharePropertyBlock = new MaterialPropertyBlock();
        }
        sharePropertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(sharePropertyBlock);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        //reader.ReadColor();
    }

}
