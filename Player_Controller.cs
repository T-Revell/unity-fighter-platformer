using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{

    //Variables

    ////Input

    public Vector2 dInput;
    public bool special;

    //bools that are true while button is held

    public bool jumpInput;
    public bool dHeld;
    public bool dashHeld;
    public bool specialHeld;

    //buffers

    public int specialBuffer;



    //Directional Input

    public void DirectionInput(InputAction.CallbackContext context)
    {
        dInput = context.ReadValue<Vector2>(); //reads input as a vector 2, right and up being positive in their respective axis, max value 1f

        dHeld = context.action.triggered; //bool that is true if any direction is held and false if not
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        jumpInput = context.action.triggered; //bool that is true if jump button is held and false if not
    }

    //Dash Input

    public void DashInput(InputAction.CallbackContext context)
    {
        dashHeld = context.action.triggered; //bool that is true if dash button is held and false if not
    }

    //Special Input

    public void SpecialInput(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            special = true; //sets bool to true on the frame it was pressed
        }

        specialHeld = context.action.triggered; //bool that is true if special button is held and false if not

    }

    private void Update()
    {
        if (special) //if special is true, a buffer counter is added to each frame
        {
            specialBuffer += 1;
        }

        if(specialBuffer == 5) //once this counter equals five, special is set to false and the counter is reset
        {
            special = false;
            specialBuffer = 0; //this counter is used to add an input buffer
        }


    }
}
