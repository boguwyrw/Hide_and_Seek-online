using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private GameObject _loadingPanel;

    [SerializeField] private TMP_Text _loadingText;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
