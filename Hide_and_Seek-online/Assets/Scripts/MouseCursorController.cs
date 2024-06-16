using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseCursorController : MonoBehaviour
{
    private int _sceneNumber = -1;

    private void Start()
    {
        _sceneNumber = SceneManager.GetActiveScene().buildIndex;
        CursorManagement();
    }

    private void Update()
    {
        CursorManagement();
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void RestoreCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowCursor();
        }
        else if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            HideCursor();
        }
    }

    private void CursorManagement()
    {
        switch (_sceneNumber)
        {
            case 0:
                ShowCursor();
                break;
            case 1:
                RestoreCursor();
                break;
            default:
                break;
        }
    }
}
