using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    [System.Serializable]
    public class GamepadIcons
    {
        [SerializeField] string name;
        public InputManager.InputController type;

        [Header("Sprites")]
        public Sprite buttonSouth;
        public Sprite buttonNorth;
        public Sprite buttonEast;
        public Sprite buttonWest;
        public Sprite startButton;
        public Sprite selectButton;
        public Sprite leftTrigger;
        public Sprite rightTrigger;
        public Sprite leftShoulder;
        public Sprite rightShoulder;
        public Sprite dpad;
        public Sprite dpadUp;
        public Sprite dpadDown;
        public Sprite dpadLeft;
        public Sprite dpadRight;
        public Sprite leftStick;
        public Sprite rightStick;
        public Sprite leftStickPress;
        public Sprite rightStickPress;

        public Sprite GetSprite(string controlPath)
        {
            if (string.IsNullOrWhiteSpace(controlPath))
                return null;

            switch (controlPath.ToLower().Replace(" ", ""))
            {
                case "buttonsouth": return buttonSouth;
                case "buttonnorth": return buttonNorth;
                case "buttoneast": return buttonEast;
                case "buttonwest": return buttonWest;
                case "start": return startButton;
                case "select": return selectButton;
                case "lefttrigger": return leftTrigger;
                case "righttrigger": return rightTrigger;
                case "leftshoulder": return leftShoulder;
                case "rightshoulder": return rightShoulder;
                case "d-pad": case "dpad": return dpad;
                case "d-pad/up": case "dpad/up": return dpadUp;
                case "d-pad/down": case "dpad/down": return dpadDown;
                case "d-pad/left": case "dpad/left": return dpadLeft;
                case "d-pad/right": case "dpad/right": return dpadRight;
                case "leftstick": case "leftstick/x": case "leftstick/y": return leftStick;
                case "rightstick": case "rightstick/x": case "rightstick/y": return rightStick;
                case "leftstickpress": return leftStickPress;
                case "rightstickpress": return rightStickPress;
            }

            return null;
        }
    }

    [CreateAssetMenu(menuName = "RGSK/Misc/Gamepad Icons")]
    public class GamepadIconDefinition : ScriptableObject
    {
        [SerializeField] List<GamepadIcons> gamepadIcons = new List<GamepadIcons>();

        public GamepadIcons GetIconSet(InputManager.InputController controller)
        {
            return gamepadIcons.FirstOrDefault(x => x.type == controller);
        }
    }
}