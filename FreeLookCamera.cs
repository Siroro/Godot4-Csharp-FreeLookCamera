using System;
using Godot;

public partial class FreeLookCamera : Camera3D
{
    private float _velocity;

    [Export(PropertyHint.Range, "0f,100f,0.01f")]
    public float BoostSpeedMultiplier = 3.0f;

    [Export(PropertyHint.Range, "0f,1000f,0.01f")]
    public float DefaultVelocity = 5f;

    [Export(PropertyHint.TypeString, "Maximum speed of camera movement.")]
    public float MaxSpeed = 1000f;

    [Export(PropertyHint.TypeString, "Minimum speed of camera movement.")]
    public float MinSpeed = 0.2f;

    [Export(PropertyHint.Range, "0f,10f,0.01f")]
    public float Sensitivity = 3f;

    [Export(PropertyHint.Range, "0f,10f,0.001f")]
    public float SpeedScale = 1.17f;

    public override void _Ready()
    {
        base._Ready();
        _velocity = DefaultVelocity;
    }

    public override void _Input(InputEvent @event)
    {
        if (!Current)
            return;

        base._Input(@event);

        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            if (@event is InputEventMouseMotion mouseMotionEvent)
            {
                Vector3 tempRot = Rotation;
                tempRot.Y -= mouseMotionEvent.Relative.X / 1000 * Sensitivity;
                tempRot.X -= mouseMotionEvent.Relative.Y / 1000 * Sensitivity;
                tempRot.X = Mathf.Clamp(tempRot.X, Mathf.Pi / -2, Mathf.Pi / 2);
                Rotation = tempRot;
            }
        }

        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            switch (mouseButtonEvent.ButtonIndex)
            {
                case MouseButton.Right:
                    Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
                    break;
                case MouseButton.WheelUp:
                    _velocity = Mathf.Clamp(_velocity * SpeedScale, MinSpeed, MaxSpeed);
                    break;
                case MouseButton.WheelDown:
                    _velocity = Mathf.Clamp(_velocity / SpeedScale, MinSpeed, MaxSpeed);
                    break;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (!Current)
            return;
        base._Process(delta);
        var direction = new Vector3(
                Convert.ToSingle(Input.IsKeyPressed(Key.D)) - Convert.ToSingle(Input.IsKeyPressed(Key.A)),
                Convert.ToSingle(Input.IsKeyPressed(Key.E)) - Convert.ToSingle(Input.IsKeyPressed(Key.Q)),
                Convert.ToSingle(Input.IsKeyPressed(Key.S)) - Convert.ToSingle(Input.IsKeyPressed(Key.W)))
            .Normalized();

        if (Input.IsKeyPressed(Key.Shift))
        {
            Translate(direction * (float)(_velocity * delta * BoostSpeedMultiplier));
        }
        else
        {
            Translate(direction * (float)(_velocity * delta));
        }
    }
}
