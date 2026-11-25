using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Npcs : MonoBehaviour
{
    public StaffID ID;
    public TMP_Text IDnumber;
    public TMP_Text JobTitle;
    public TMP_Text name;
    public TMP_Text  Date;
    public TMP_Text Expries;
    public Image photoID;
    

    public GameObject Canvas;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowID()
    {
        Canvas.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().npc = this;

            //////Canvas.SetActive(true);
            name.text = ID.Name;
            Date.text = ID.Date;
            Expries.text = ID.Expries;
            IDnumber.text = ID.IDnumber;
            JobTitle.text = ID.JobTitle;
            photoID.sprite = ID.photoID;
            Debug.Log(ID.Name);
        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().npc = null;
            Canvas.SetActive(false);
            name.text = ID.Name;
            Date.text= ID.Date;
            Expries.text= ID.Expries;
            JobTitle.text= ID.JobTitle;
            photoID.sprite = ID.photoID;
            Debug.Log(ID.Name);
        }
    }
}
