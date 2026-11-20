using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "StaffID", menuName = "Scriptable Objects/StaffID")]
public class StaffID : ScriptableObject
{
    [Header("Staff ID")]
    public string ID;
    public string JobTitle;
    [Space]
    public string Name;
    public string Date;
    public string Expries;
    public string IDnumber;

    public Sprite photoID;
    
}
