using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField]
    List<Light> RecolouredLights = new List<Light>();
    [SerializeField]
    Light ReangledLight;
    [SerializeField]
    AnimationCurve LightAngle;

    [SerializeField]
    Sprite LightColour;

    readonly float DayLength = 60f; // seconds
    float SimulatedTime = 0f;

    readonly float CurveOffset = -5f / 24f;

    private void Awake()
    {
        SimulatedTime = -CurveOffset;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SimulatedTime += Time.deltaTime;
        float percentage = Mathf.Clamp01((SimulatedTime % DayLength) / DayLength);
        var colour = LightColour.texture.GetPixel(Mathf.RoundToInt(percentage * (LightColour.texture.width - 1)), 0);
        foreach (Light l in RecolouredLights)
        {
            l.color = colour;
        }
        float modPercentage = (percentage + (1f + CurveOffset)) % 1f;
        float curveSample = LightAngle.Evaluate(modPercentage);
        Vector3 rot = new Vector3(curveSample * 360f, -90f, 0f);
        ReangledLight.transform.rotation = Quaternion.Euler(rot);
    }
}
