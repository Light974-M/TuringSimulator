using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods
{
    public enum Axis
    {
        X,
        Y,
        Z,
        W,
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }

    public enum PlayerAction
    {
        Idle,
        Move,
        Jump,
        Attack,
        Defend,
        Interact
    }

    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard,
        Expert
    }

    public enum UIState
    {
        MainMenu,
        OptionsMenu,
        InGameHUD,
        PauseMenu,
        GameOverScreen,
        VictoryScreen
    }

    public enum MovementType
    {
        Walking,
        Running,
        Flying,
        Swimming,
        Crawling
    }

    public enum AnimationState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        Attacking,
        Dying
    }

    public enum HealthState
    {
        Healthy,
        Injured,
        Critical,
        Dead
    }

    public enum WeatherType
    {
        Sunny,
        Rainy,
        Snowy,
        Windy,
        Stormy,
        Foggy
    }

    public enum ConstructionState
    {
        NotStarted,
        InProgress,
        Completed,
        Damaged,
        Destroyed
    }

    public enum SaveState
    {
        NotSaved,
        AutoSave,
        ManualSave,
        QuickSave,
        Checkpoint
    }

    public enum PositionRendredType
    {
        Centered,
        IntCentered,
        Edged,
    }
}
