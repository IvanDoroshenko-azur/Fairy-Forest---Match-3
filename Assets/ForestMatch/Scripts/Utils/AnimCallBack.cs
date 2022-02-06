using UnityEngine;

public class AnimCallBack: MonoBehaviour
{
    private System.Action cBack;

    public void EndCallBack()
    {
        if (cBack != null) cBack();
    }

    public void SetEndCallBack(System.Action cBack)
    {
        this.cBack = cBack;
    }
}