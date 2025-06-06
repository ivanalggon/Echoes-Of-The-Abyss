using UnityEngine;

public class LookAtCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    public Texture2D defaultCursor;
    public Texture2D enemyCursor;
    public Texture2D itemCursor;         // Cursor para items
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;

    [Header("References")]
    public Transform playerTransform;
    public LayerMask aimLayerMask;

    private Quaternion targetRotation;
    private bool shouldRotate = false;

    private enum CursorState { Default, Enemy, Item }
    private CursorState currentCursor = CursorState.Default;

    void Start()
    {
        SetCursor(defaultCursor);
    }

    void Update()
    {
        HandleCursor();
        HandleRotation();
    }

    void HandleCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        CursorState newCursorState = CursorState.Default;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, aimLayerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                newCursorState = CursorState.Enemy;
            }
            else if (hit.collider.CompareTag("Boss"))
            {
                newCursorState = CursorState.Enemy;
            }
            else if (hit.collider.CompareTag("Item"))
            {
                newCursorState = CursorState.Item;
            }
            else if (hit.collider.CompareTag("Ally"))
            {
                newCursorState = CursorState.Item;
            }
        }

        if (newCursorState != currentCursor)
        {
            currentCursor = newCursorState;
            switch (currentCursor)
            {
                case CursorState.Default:
                    SetCursor(defaultCursor);
                    break;
                case CursorState.Enemy:
                    SetCursor(enemyCursor);
                    break;
                case CursorState.Item:
                    SetCursor(itemCursor);
                    break;
            }
        }
    }

    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, aimLayerMask))
            {
                Vector3 lookPoint = hit.point;
                Vector3 direction = lookPoint - playerTransform.position;
                direction.y = 0;

                if (direction.sqrMagnitude > 0.001f)
                {
                    targetRotation = Quaternion.LookRotation(direction);
                    shouldRotate = true;
                }
            }
        }

        if (shouldRotate)
        {
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(playerTransform.rotation, targetRotation) < 0.1f)
            {
                playerTransform.rotation = targetRotation;
                shouldRotate = false;
            }
        }
    }

    void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
    }
}
