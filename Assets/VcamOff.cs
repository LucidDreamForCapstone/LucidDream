using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VcamOff : MonoBehaviour
{
    [SerializeField] GameObject _vcam;
    
    public void SetOnTheVcam() {
        _vcam.SetActive(true);
    }
    public void SetoffTheVcam() {
        _vcam.SetActive(false); 
    }
}
