using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Events;

public class ProximityPromptScript : MonoBehaviour
{
    public float MaxDistance = 10f;
    public float MinDot = 0.8f;
    public float time = 1f;
    public float WaitTime = 3f;
    private float WaitedTime;
    private float ReamingTime;
    public string keybind = "e";
    public string Object = "Door";
    public string Actions;

    public Transform Camera;
    public Transform Canvas;
    public TMP_Text Text;

    public GameObject TextField;
    private bool IsActive = false;
    private bool Debounce = true;

    public bool IsVisible = false;

    [SerializeField] private AudioSource ProceduralSFX;

    public UnityEvent IsTriggered;

    private static readonly List<ProximityPromptScript> allPrompts = new List<ProximityPromptScript>();
    private static ProximityPromptScript bestPrompt;
    private static int lastEvaluatedFrame = -1;

    private void OnEnable()
    {
        if (!allPrompts.Contains(this))
        {
            allPrompts.Add(this);
        }
    }

    private void OnDisable()
    {
        allPrompts.Remove(this);
    }

    private void Start()
    {
        ReamingTime = time;
        WaitedTime = WaitTime;
    }

    private void Update()
    {
        if (Time.frameCount != lastEvaluatedFrame)
        {
            EvaluateBestPrompt();
            lastEvaluatedFrame = Time.frameCount;
        }

        IsActive = (bestPrompt == this);

        if (IsActive && Debounce)
        {
            Canvas.LookAt(Camera.position);
            Canvas.Rotate(0, 180, 0);
            TextField.SetActive(true);

            if (Input.GetKey(keybind))
            {
                if (ReamingTime > 0)
                {
                    ReamingTime -= Time.deltaTime;
                    Text.text = Object + " - " + Actions + " (" +
                    math.round(ReamingTime * 10f) / 10f + ")";

                    StartProcenduralSFX();
                }
                else
                {
                    Debounce = false;
                    IsTriggered?.Invoke();
                    
                    StopProcenduralSFX();
                }
            }
            else
            {
                ReamingTime = time;
                Text.text = Object + " - " + Actions + " (" +
                keybind.ToUpper() + ")";

                StopProcenduralSFX();
            }
        }
        else
        {
            TextField.SetActive(false);
            StopProcenduralSFX();
        }

        if (!Debounce)
        {
            if (WaitedTime > 0)
                WaitedTime -= Time.deltaTime;
            else if (!Input.GetKey(keybind))
            {
                Debounce = true;
                WaitedTime = WaitTime;
            }
        }
    }

    private static void EvaluateBestPrompt()
    {
        bestPrompt = null;
        float highestDot = -1f;

        foreach (var prompt in allPrompts)
        {
            if (!prompt.IsVisible || prompt.Camera == null || prompt.Canvas == null) continue;

            float distance = Vector3.Distance(prompt.Camera.position, prompt.Canvas.position);
            if (distance > prompt.MaxDistance) continue;

            Vector3 camFront = prompt.Camera.forward;
            Vector3 shouldLook = prompt.Canvas.position - prompt.Camera.position;
            float dot = Vector3.Dot(camFront.normalized, shouldLook.normalized);

            if (dot > prompt.MinDot && dot > highestDot)
            {
                highestDot = dot;
                bestPrompt = prompt;
            }
        }
    }

    private void StartProcenduralSFX()
    {
        if (ProceduralSFX != null)
        {
            if (!ProceduralSFX.isPlaying)
            {
                ProceduralSFX.Play();
            }
        }
    }

    private void StopProcenduralSFX()
    {
        if (ProceduralSFX != null)
        {
            if (ProceduralSFX.isPlaying)
            {
                ProceduralSFX.Stop();
                ProceduralSFX.time = 0;
            }
        }
    }
}