using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] private GameObject _foundPanel;
    
    [SerializeField] private TMP_Text _playerWhoFoundText;

    private void Start()
    {
        _foundPanel.SetActive(false);
    }

    private void Update()
    {
        
    }

    public void OpenCloseFoundPanel(bool isOpen)
    {
        _foundPanel.SetActive(isOpen);
    }

    public void PlayerNameWhoFound(string playerName)
    {
        _playerWhoFoundText.text = "You have been found by: " + playerName;
        OpenCloseFoundPanel(isOpen: true);
    }
}
