using UnityEngine;

public abstract class DefenseClass : MonoBehaviour
{
    [SerializeField] public DefensesSO defenseSO;
    [SerializeField] protected GameObject defenseModel;
    [SerializeField] protected Material previewMAT;

    public virtual void OnPlacing()
    {
        ChangeModelMaterials(Color.white);
        defenseModel.GetComponent<BoxCollider>().isTrigger = false;
    }

    public abstract void OnDeleting();
    public virtual void ChangeModelMaterials(Color color)
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
