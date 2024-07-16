namespace RGSK
{
    #region AI

    public enum TargetApproachAction
    {
        None,
        SlowDown,
        SlowDownAndStop
    }

    #endregion

    #region AUDIO

    public enum AudioGroup
    {
        Master = 0,
        Music = 1,
        SFX = 2,
        Vehicle = 3,
        UI = 4,
    }

    #endregion

    #region MISC

    public enum SortOrder
    {
        Descending = 0,
        Ascending = 1
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }

    public enum DirectionAxis
    {
        Vertical,
        Horizontal
    }

    #endregion

    #region RACE

    public enum RaceState
    {
        PreRace = 0,
        Countdown = 1,
        Racing = 2,
        PostRace = 3,
        RollingStart = 4,
    }

    public enum RaceDurationMode
    {
        LapBased = 0,
        TimeBased = 1,
        LapAndTime = 2
    }

    public enum RaceStartMode
    {
        StandingStart = 0,
        RollingStart = 1,
    }

    public enum RacePositioningMode
    {
        Distance = 0,
        DriftPoints = 1,
        TotalTime = 2,
        BestLapTime = 3,
        TotalSpeed = 4,
        AverageSpeed = 5,
        Score = 6,
    }

    public enum RaceTimerMode
    {
        Global = 0,
        PerCompetitor = 1
    }

    public enum GlobalTimerElapsedAction
    {
        Finish = 0,
        DisqualifyLastPlace = 1,
        FinalLap = 2,
    }

    public enum DnfTimerStartMode
    {
        Off = 0,
        AfterFirstFinish = 1,
        AfterHalfFinish = 2
    }

    public enum CheckpointType
    {
        Sector = 0,
        TimeExtend = 1,
        Speedtrap = 2
    }

    public enum TrackLayoutType
    {
        Circuit = 0,
        PointToPoint = 1,
    }

    public enum RaceSessionType
    {
        Race = 0,
        TargetScore = 1
    }

    public enum SelectionMode
    {
        Random,
        Selected
    }

    public enum ColorSelectionMode
    {
        Random,
        Color,
        Livery
    }

    public enum OpponentClassOptions
    {
        All = 0,
        Selected = 1,
        SameAsPlayer = 2,
    }

    #endregion

    #region UI

    public enum TimeFormat
    {
        MM_SS_FFF = 0,
        MM_SS = 1,
        SS = 2,
        S_FFF = 3,
        HH_MM_SS = 4
    }

    public enum NumberDisplayMode
    {
        Default = 0,
        Single = 1,
        Ordinal = 2
    }

    public enum NameDisplayMode
    {
        FullName = 0,
        FirstName = 1,
        LastName = 2,
        Initials = 3,
        ThreeLetterAbbreviation = 4,
    }

    public enum VehicleNameDisplayMode
    {
        FullName,
        ModelName
    }

    public enum ButtonType
    {
        Restart,
        BackToMenu,
        QuitApplication,
        WatchReplay,
    }

    public enum MobileControlType
    {
        Touch = 0,
        Wheel = 1,
        Tilt = 2,
    }

    public enum BoardSize
    {
        Full,
        Mini
    }

    public enum RaceSettingUIType
    {
        RaceType = 0,
        StartMode = 1,
        LapCount = 2,
        OpponentCount = 3,
        StartingPosition = 4,
        SessionTimeLimit = 5,
        AIDifficulty = 6,
        Collision = 7,
        Slipstream = 9,
        Ghost = 10,
    }

    public enum GameplaySettingType
    {
        SpeedUnit,
        Transmission,
        Nameplate,
        MobileInput,
        Vibration,
        DistanceUnit,
        FPS,
    }
    #endregion

    #region UNITS

    public enum SpeedUnit
    {
        KMH,
        MPH
    }

    public enum DistanceUnit
    {
        Meters,
        Feet,
        Yards,
        Kilometers,
        Miles
    }

    #endregion

    #region VEHICLE

    public enum WheelAxle
    {
        Front,
        Rear
    }

    public enum Drivetrain
    {
        RWD,
        FWD,
        AWD
    }

    public enum VehicleHandlingMode
    {
        Grip,
        Drift
    }

    public enum TransmissionType
    {
        Automatic = 0,
        Manual = 1
    }

    public enum SurfaceEmissionType
    {
        Slip,
        Velocity
    }

    public enum VehicleLightType
    {
        HeadLight = 0,
        TailLight = 1,
        ReverseLight = 2
    }

    public enum ExhaustEffectType
    {
        Smoke = 0,
        Backfire = 1,
        Nitrous = 2
    }

    public enum IKTarget
    {
        LeftHand,
        RightHand,
        LeftFoot,
        RightFoot,
        HeadLook
    }
    #endregion
}