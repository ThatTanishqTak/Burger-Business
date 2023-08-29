using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] private float rotateSpeed = 12.0f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private float playerRadius = 0.7f;
    private float playerHeight = 2.0f;
    private float interactDistance = 2.0f;
    private float moveDistance;
    private bool isWalking;
    private bool canMove;
    private Vector3 lastInteraction;

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() { return isWalking; }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0.0f, inputVector.y);

        if (moveDirection != Vector3.zero) { lastInteraction = moveDirection; }

        if (Physics.Raycast(transform.position, lastInteraction, out RaycastHit rayCastHit, interactDistance, countersLayerMask)) 
            if (rayCastHit.transform.TryGetComponent(out ClearCounter clearCounter)) { clearCounter.Interact(); }
    }

    private void HandleMovement()
    {
        moveDistance = moveSpeed * Time.deltaTime;

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0.0f, inputVector.y);
        canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirection, moveDistance);

        if (!canMove)
        {
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0.0f, 0.0f).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionX, moveDistance);

            if (canMove) { moveDirection = moveDirectionX; }
            else
            {
                Vector3 moveDirectionZ = new Vector3(0.0f, 0.0f, moveDirection.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionZ, moveDistance);

                if (canMove) { moveDirection = moveDirectionZ; }
            }
        }

        if (canMove) { transform.position += moveDirection * moveDistance; }
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

        isWalking = moveDirection != Vector3.zero;
    }
}