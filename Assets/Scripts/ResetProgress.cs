using UnityEngine;

public class ResetProgress : MonoBehaviour
{
    [ContextMenu("Reset All Level Progress")]
    private void Reset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Progress direset!");
    }
}
