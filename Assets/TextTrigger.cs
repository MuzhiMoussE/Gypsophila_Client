using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextTrigger : MonoBehaviour
{
    public GameObject TextPanel;
    public Text textfield;
    public string Text;
    private AudioSource audioSource;
    private Animator animator;
    private void OnTriggerEnter(Collider other)
    {
        if(other != null && other.tag == Global.ItemTag.PLAYER)
        {
            textfield.text = Text;
            audioSource.Play();
            TextPanel.SetActive(true);
            animator.ResetTrigger("Disappear");
            animator.SetTrigger("Appear");
           
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = TextPanel.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
