using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameHandler : MonoBehaviour
{
    public PanelManager pm;

    public CameraFollow camFollow;
    public Transform targetTransform;

    public float zoom;
    public float zoomAmount;
    public float zoomMin;
    public float zoomMax;
    public float scrollAmount;
    public float scrollEdgeSize;

    public static UnityEvent SwitchToFreeCamera;
    public static UnityEvent SwitchToFocusCamera;

    public enum GameState
    {
        MainScreen,
        Starting,
        Paused,
        Playing, 
        GameOver
    }

    public GameState currentState;

    public GameObject pausePanel;
    public GameObject startPanel;
    public GameObject loadingPanel;

    public static float gameDeltaTime;

    public static bool physicsOn { get; private set; }

    public void InitializeGame()
    {
        currentState = GameState.Starting;
        pm.HidePanel("Start");
        pm.ShowPanel("Loading");

        StartCoroutine("LoadGameStuff");

    }

    public IEnumerator LoadGameStuff()
    {
        yield return new WaitForSeconds(5);
        pm.ClearPanels();
        currentState = GameState.Playing;
        yield return null;
    }

    private void Start()
    {
       // pm = new PanelManager();
        pm.AddPanel("Pause", pausePanel);
        pm.AddPanel("Start", startPanel);
        pm.AddPanel("Loading", loadingPanel);

        pm.ShowPanel("Start");
   

        camFollow.Setup(() => targetTransform.position, () => zoom);
        camFollow.SetCameraFollowPosition(targetTransform.position);
        camFollow.SetCameraFollowObject(targetTransform);
        camFollow.SetCameraZoom(100);

        //Initialize Players
        //Initialize BattleOrder

        currentState = GameState.MainScreen;      
        physicsOn = false;
    }

    void HandleManualMovement()
    {
        
        if (Input.GetKey(KeyCode.D))
        {
            CameraFollow.ScrollCamera.Invoke(scrollAmount * gameDeltaTime, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            CameraFollow.ScrollCamera.Invoke(-1 * scrollAmount * gameDeltaTime, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            CameraFollow.ScrollCamera.Invoke(0, scrollAmount * gameDeltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            CameraFollow.ScrollCamera.Invoke(0, -1 * scrollAmount * gameDeltaTime);
        }
        if (Input.GetMouseButtonUp(1))
        {
            camFollow.SetCameraFollowPosition(targetTransform.position);
            CameraFollow.SwitchToFocusCamera.Invoke();
        }
    }

    private void PlayingUpdate()
    {
        gameDeltaTime = Time.deltaTime;

        HandleManualMovement();
        HandleEdgeScrolling();

        HandleZoom();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
            physicsOn = false;
            currentState = GameState.Paused;
        }
    }

    private void PausedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(false);
            physicsOn = true;
            currentState = GameState.Playing;
        }
    }

    void HandleEdgeScrolling()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (mousePos.x > Screen.width - scrollEdgeSize)
        {
            float scrollFactor = (CalcScrollFactor(mousePos.x, Screen.width, scrollEdgeSize)) * scrollAmount ;
            CameraFollow.ScrollCamera.Invoke(scrollFactor * gameDeltaTime, 0);
        }
        if (mousePos.x < scrollEdgeSize)
        {
            float scrollFactor = (CalcScrollFactor(mousePos.x, 0, scrollEdgeSize)) * scrollAmount;
            CameraFollow.ScrollCamera.Invoke(-1 * scrollFactor * gameDeltaTime, 0);
        }
        if (mousePos.y > Screen.height - scrollEdgeSize)
        {
            float scrollFactor = (CalcScrollFactor(mousePos.y, Screen.height, scrollEdgeSize)) * scrollAmount;
            CameraFollow.ScrollCamera.Invoke(0, scrollFactor * gameDeltaTime);
        }
        if (mousePos.y < scrollEdgeSize)
        {
            float scrollFactor = (CalcScrollFactor(mousePos.y, 0, scrollEdgeSize)) * scrollAmount;
            CameraFollow.ScrollCamera.Invoke(0, -1 * scrollFactor * gameDeltaTime);
        }
    }

    float  CalcScrollFactor(float mouse, float edgeValue, float edgeAmount)
    {
        float result = 0;        
        float adjustedMouse;
        float adjustedEdge;

        if (edgeValue == 0)
        {
            adjustedEdge = edgeAmount;
            adjustedMouse = adjustedEdge - mouse;
        }
        else
        {
            adjustedEdge = edgeValue - edgeAmount;
            adjustedMouse = mouse - adjustedEdge;
            adjustedEdge = edgeAmount;        
        }

        result = adjustedMouse / adjustedEdge;

        return result;
    }
    void HandleZoom()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            ZoomIn();
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            ZoomOut();
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            ZoomIn();
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            ZoomOut();
        }

        CameraFollow.ZoomCamera.Invoke(zoom);
    }
    private void Update()
    {
        switch (currentState)
        {
            case GameState.MainScreen:
                {
                    // Display main screen
                    break;
                }

            case GameState.Starting:
                {
                    // Set up world
                    break;
                }

            case GameState.Playing:
                {
                    // Active state of play
                    PlayingUpdate();
                    break;
                }

            case GameState.Paused:
                {
                    // Do you really need to be told?
                    PausedUpdate();
                    break;
                }
            default:
                {
                    // When in doubt, go to pause menu.
                    currentState = GameState.Paused;
                    break;
                }
        }
    }

    private void ZoomIn()
    {
        zoom -= zoomAmount * gameDeltaTime ;
        if (zoom < zoomMin) zoom = zoomMin;

    }
    private void ZoomOut()
    {
        zoom += zoomAmount * gameDeltaTime;
        if (zoom > zoomMax) zoom = zoomMax;
    }
}
