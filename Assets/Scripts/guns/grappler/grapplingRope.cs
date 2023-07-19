using UnityEngine;

public class grapplingRope : MonoBehaviour
{
    [Header("General Refernces:")]
    public grappler grapplingGun;
    public LineRenderer lineRenderer;

    int precision = 40;
    float straightenLineSpeed = 5;
    float waveSize = 2;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 3.5f;

    float moveTime = 0;

    [HideInInspector] public bool isGrappling = true;

    bool straightLine = true;

    private void OnEnable() {
        moveTime = 0;
        lineRenderer.positionCount = precision;
        waveSize = 2;
        straightLine = false;

        LinePointsToFirePoint();

        lineRenderer.enabled = true;
    }

    private void OnDisable() {
        lineRenderer.enabled = false;
        isGrappling = false;
    }

    private void LinePointsToFirePoint() {
        for (int i = 0; i < precision; i++)
        {
            lineRenderer.SetPosition(i, grapplingGun.firePoint.position);
        }
    }

    private void Update() {
        moveTime += Time.deltaTime;
        DrawRope();
    }

    void DrawRope()
    {
        if (!straightLine) {
            if (lineRenderer.GetPosition(precision - 1).x == grapplingGun.grapplePoint.x)
                straightLine = true;
            else
                DrawRopes();
        }
        else
        {
            if (!isGrappling) {
                grapplingGun.Grapple();
                isGrappling = true;
            }
            if (waveSize > 0) {
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopes();
            }
            else {
                waveSize = 0;
                if (lineRenderer.positionCount != 2)  
                    lineRenderer.positionCount = 2; 

                lineRenderer.SetPosition(0, grapplingGun.firePoint.position);
                lineRenderer.SetPosition(1, grapplingGun.grapplePoint);
            }
        }
    }

    void DrawRopes()
    {
        for (int i = 0; i < precision; i++) {
            float delta = (float)i / ((float)precision - 1f);
            Vector2 targetPosition = Vector2.Lerp(grapplingGun.firePoint.position, grapplingGun.grapplePoint, delta);
            Vector2 currentPosition = Vector2.Lerp(grapplingGun.firePoint.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);
            lineRenderer.SetPosition(i, currentPosition);
        }
    }

}