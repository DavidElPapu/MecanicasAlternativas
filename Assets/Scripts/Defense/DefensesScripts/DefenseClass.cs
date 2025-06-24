using UnityEngine;

public abstract class DefenseClass : MonoBehaviour
{
    public DefensesSO defenseSO;
    [SerializeField] protected int currentLevel;
    [SerializeField] protected GameObject defenseModel;
    [SerializeField] protected Material previewMAT;

    public virtual void Awake()
    {
        currentLevel = 0;
    }

    public virtual void OnPlacing()
    {
        //cuando se ponga, mejor que cambie a otro material, no el preview
        ChangeModelMaterials(Color.white);
        defenseModel.GetComponent<BoxCollider>().isTrigger = false;
        currentLevel = 1;
    }

    public virtual void OnDeleting()
    {
        Destroy(this.gameObject);
    }

    public virtual void OnUpgrading()
    {
        currentLevel++;
        //Debug.Log("Subi al nivel" + currentLevel);
    }

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

    public virtual int GetCurrentLevel()
    {
        return currentLevel;
    }
}
