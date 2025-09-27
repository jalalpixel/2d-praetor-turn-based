using UnityEngine;

public class ObjectRotateOverTime : MonoBehaviour
{
    public float rotationSpeed = 360f; // Speed of rotation in degrees per second
    public float minRotationSpeed = 1f;
    public bool isMinMaxRotationSpeed;
    private float cooldownMinMaxRotationCheck = 2f;
    public bool isStayOnLastRotation;
    public bool isStopRotating;
    public bool isSwitchOnOffAxes;
    public bool isRotatingAllAxes;
    public bool isRotatingXAxes;
    public bool isRotatingYAxes;
    public bool isRotatingZAxes;

    void Update()
    {
        if (isStopRotating)
        {
            return;
        }

        if (isMinMaxRotationSpeed)
        {
            cooldownMinMaxRotationCheck -= Time.deltaTime;
            if(cooldownMinMaxRotationCheck <= 0)
            {
                ChangeRotationSpeed();
                ChangeOnOffRotationAxes();
                cooldownMinMaxRotationCheck = 2f;
            }
        }

        if(isRotatingAllAxes == false)
        {
            if(isRotatingXAxes)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                transform.Rotate(rotationStep, 0f, 0f);
            }
            if (isRotatingYAxes)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, rotationStep, 0f);
            }
            if (isRotatingZAxes)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, 0f, rotationStep);
            }
        }
        else
        {
            float rotationStep = rotationSpeed * Time.deltaTime;
            transform.Rotate(rotationStep, rotationStep, rotationStep);
        }
    }

    private void ChangeRotationSpeed()
    {
        rotationSpeed = Random.Range(minRotationSpeed, 360f);
    }

    private void ChangeOnOffRotationAxes()
    {
        if (isRotatingAllAxes) return;
        if (isSwitchOnOffAxes == false) return;

        if (isRotatingXAxes)
        {
            isRotatingXAxes = false;
            isRotatingYAxes = true;            
        } 
        else if (isRotatingYAxes)
        {
            isRotatingXAxes = true;
            isRotatingYAxes = false;
        }
    }

    private void OnDisable()
    {
        // Reset rotation when the object is disabled
        if (isStayOnLastRotation == false)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        isStopRotating = false;
    }
}
