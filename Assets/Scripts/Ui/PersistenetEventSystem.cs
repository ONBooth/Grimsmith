using UnityEngine;

public class PersistenetEventSystem : MonoBehaviour
{
    static PersistenetEventSystem instance;

    void Awake()
    {
        if (instance != null) {Destroy(gameObject); return;}

        instance = this;
        DontDestroyOnLoad(gameObject);

    }
}
