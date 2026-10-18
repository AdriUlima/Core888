using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour_UL, ILateUpdatable
{
    [SerializeField] private InputActionReference[] buttonsFrame, buttonsHold, floats, vector2s;
    private bool[] buttonsF, buttonsH;
    private float[] floatsV;
    private Vector2[] vector2sV;

    void Start()
    {
        Init(ref buttonsF, buttonsFrame, EnableButtonFrame);
        Init(ref buttonsH, buttonsHold, EnableButtonHold);
        Init(ref floatsV, floats, EnableFloat);
        Init(ref vector2sV, vector2s, EnableVector2);
    }
    private void Init<T>(ref T[] array, InputActionReference[] actions, Action<InputActionReference,int> binder)
    {
        array = new T[actions.Length];
        for (int i = 0; i < actions.Length; i++) binder(actions[i], i);
    }

    private void EnableButtonFrame(InputActionReference input, int i) => EnableButton(input, i, buttonsF, false);
    private void EnableButtonHold(InputActionReference input, int i) => EnableButton(input, i, buttonsH, true);
    private void EnableButton(InputActionReference input, int i, bool[] array, bool isHold)
    {
        InputAction action = input.action;
        action.performed += _ =>
        {
            array[i] = true;
        };
        if (isHold)
        {
            action.canceled += _ =>
            {
                array[i] = false;
            };
        }
        action.Enable();
    }
    private void EnableFloat(InputActionReference input, int i)
    {
        InputAction action = input.action;
        action.performed += ctx =>
        {
            floatsV[i] = ctx.ReadValue<float>();
        };
        action.canceled += ctx =>
        {
            floatsV[i] = 0f;
        };
        action.Enable();
    }
    private void EnableVector2(InputActionReference input, int i)
    {
        InputAction action = input.action;
        action.performed += ctx =>
        {
            vector2sV[i] = ctx.ReadValue<Vector2>();
        };
        action.canceled += ctx =>
        {
            vector2sV[i] = Vector2.zero;
        };
        action.Enable();
    }

    public bool GetButtonFrame(int i) => buttonsF[i];
    public bool GetButtonFrame<T>(T e) where T : Enum => GetButtonFrame(Convert.ToInt32(e));
    public bool GetButtonHold(int i) => buttonsH[i];
    public bool GetButtonHold<T>(T e) where T : Enum => GetButtonHold(Convert.ToInt32(e));
    public float GetFloat(int i) => floatsV[i];
    public float GetFloat<T>(T e) where T : Enum => GetFloat(Convert.ToInt32(e));
    public Vector2 GetVector2(int i) => vector2sV[i];
    public Vector2 GetVector2<T>(T e) where T : Enum => GetVector2(Convert.ToInt32(e));

    public void OnLateUpdate()
    {
        Array.Clear(buttonsF, 0, buttonsF.Length);
    }
}