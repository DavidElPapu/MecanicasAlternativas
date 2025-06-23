using System;
using UnityEngine;

public class GeneralDefenseScript : MonoBehaviour
{
    public GameObject defenseModel;
    public Material previewMAT;
    public static event Action OnActivate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateDefense()
    {
        //el signo de interrogacion es para que no haya errores de null si nadie esta suscrito al evento
        OnActivate?.Invoke();
        ChangeModelMaterials(Color.white);
        defenseModel.GetComponent<BoxCollider>().isTrigger = false;
    }

    public void ChangeModelMaterials(Color color)
    {
        if (color != Color.white)
            color.a = 0.3f;
        previewMAT.color = color;
        foreach (MeshRenderer modelPartsRenderers in defenseModel.GetComponentsInChildren<MeshRenderer>())
        {
            modelPartsRenderers.material = previewMAT;
        }
    }
}
