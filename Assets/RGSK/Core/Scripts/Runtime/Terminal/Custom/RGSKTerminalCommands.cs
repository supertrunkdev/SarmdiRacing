using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandTerminal;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class RGSKTerminalCommands
    {
        #region RACE
        [RegisterCommand(Name = "list", Help = "Lists all the entities in the scene")]
        static void CommandListEntities(CommandArg[] args)
        {
            for (int i = 0; i < RGSKCore.Instance.GeneralSettings.entitySet.Items.Count; i++)
            {
                var e = RGSKCore.Instance.GeneralSettings.entitySet.Items[i];
                Terminal.print($"[{i}] {UIHelper.FormatNameText(e?.ProfileDefiner?.definition)} [{e.ID}]");
            }
        }

        [RegisterCommand(Name = "ini", Help = "Initializes the race")]
        static void CommandInitRace(CommandArg[] args)
        {
            var ini = GameObject.FindObjectOfType<RaceInitializer>();
            ini?.Initialize();
        }

        [RegisterCommand(Name = "dini", Help = "Deinitializes the race")]
        static void CommandDeInitRace(CommandArg[] args)
        {
            RaceManager.Instance?.DeInitializeRace();
        }

        [RegisterCommand(Name = "fin", Help = "Finishes the race for a competitor")]
        static void CommandFinishRace(CommandArg[] args)
        {
            var entity = GeneralHelper.GetFocusedEntity();

            if (entity != null)
            {
                PauseManager.Instance?.Unpause();
                RaceManager.Instance?.FinishRace(entity.Competitor);
            }
        }

        [RegisterCommand(Name = "dq", Help = "Disqualify the competitor")]
        static void CommandDQ(CommandArg[] args)
        {
            var entity = GeneralHelper.GetFocusedEntity();

            if (entity != null)
            {
                PauseManager.Instance?.Unpause();
                RaceManager.Instance?.Disqualify(entity.Competitor);
            }
        }

        [RegisterCommand(Name = "finall", Help = "Finish the race for all")]
        static void CommandFinishAll(CommandArg[] args)
        {
            PauseManager.Instance?.Unpause();
            RaceManager.Instance?.ForceFinishRace(false);
        }

        [RegisterCommand(Name = "dqall", Help = "Disqualify all")]
        static void CommandDQAll(CommandArg[] args)
        {
            PauseManager.Instance?.Unpause();
            RaceManager.Instance?.ForceFinishRace(true);
        }

        [RegisterCommand(Name = "restartrace", Help = "Restart the race")]
        static void CommandRestartRace(CommandArg[] args)
        {
            PauseManager.Instance?.Unpause();
            RaceManager.Instance?.RestartRace();
        }

        [RegisterCommand(Name = "nextcp", Help = "Teleport to the next checkpoint")]
        static void CommandNextCheckpoint(CommandArg[] args)
        {
            var c = GeneralHelper.GetFocusedEntity().Competitor;

            if (c != null && c.IsRacing())
            {
                var cp = c.NextCheckpoint;
                if (cp != null)
                {
                    c.transform.SetPositionAndRotation(cp.transform.position, cp.transform.rotation);
                }

                if (c.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.ResetVelocity();
                }
            }
        }
        #endregion

        #region MISC
        [RegisterCommand(Name = "setai", Help = "Toggles AI control", MinArgCount = 1)]
        static void CommandSetAI(CommandArg[] args)
        {
            int index = args[0].Int;

            if (Terminal.IssuedError)
                return;

            var e = GeneralHelper.GetFocusedEntity();
            if (e != null)
            {
                GeneralHelper.ToggleAIInput(e.gameObject, index == 0 ? false : true);
                GeneralHelper.TogglePlayerInput(e.gameObject, index == 1 ? false : true);
                GeneralHelper.ToggleInputControl(e.gameObject, true);
            }
        }

        [RegisterCommand(Name = "target", Help = "Changes the camera target by a direction", MinArgCount = 1)]
        static void CommandCameraTarget(CommandArg[] args)
        {
            int index = args[0].Int;

            if (Terminal.IssuedError)
                return;

            CameraManager.Instance?.ChangeTarget(index);
        }

        [RegisterCommand(Name = "settrans", Help = "Sets vehicle transmission", MinArgCount = 1)]
        static void CommandTransmission(CommandArg[] args)
        {
            int index = args[0].Int;

            if (Terminal.IssuedError)
                return;

            var e = GeneralHelper.GetFocusedEntity();
            if (e != null)
            {
                GeneralHelper.SetTransmission(e.gameObject, index == 0 ? TransmissionType.Automatic : TransmissionType.Manual);
            }
        }

        [RegisterCommand(Name = "repair", Help = "Repairs the vehicle")]
        static void CommandRepair(CommandArg[] args)
        {
            var e = GeneralHelper.GetFocusedEntity();
            if (e != null && e.Vehicle != null)
            {
                e.Vehicle.Repair();
            }
        }

        [RegisterCommand(Name = "ghost", Help = "Sets as ghost", MinArgCount = 1)]
        static void CommandGhostMesh(CommandArg[] args)
        {
            int index = args[0].Int;

            if (Terminal.IssuedError)
                return;

            var e = GeneralHelper.GetFocusedEntity();
            if (e != null)
            {
                GeneralHelper.ToggleGhostedMesh(e.gameObject, index == 1);
            }
        }

        [RegisterCommand(Name = "cinematic", Help = "Enables cinematic camera mode", MinArgCount = 1)]
        static void CommandCinematicCamera(CommandArg[] args)
        {
            int index = args[0].Int;

            if (Terminal.IssuedError)
                return;

            CameraManager.Instance?.ToggleRouteCameras(index == 1);
        }

        [RegisterCommand(Name = "randcol", Help = "Randomizes vehicle colors")]
        static void CommandRandomColors(CommandArg[] args)
        {
            for (int i = 0; i < RGSKCore.Instance.GeneralSettings.entitySet.Items.Count; i++)
            {
                var e = RGSKCore.Instance.GeneralSettings.entitySet.Items[i];
                GeneralHelper.SetColor(e.gameObject, GeneralHelper.GetRandomVehicleColor(), 0);
            }
        }

        [RegisterCommand(Name = "hideui", Help = "Hides the current active screen")]
        static void CommandHideUI(CommandArg[] args)
        {
            var screen = UIManager.Instance.ActiveScreen;
            if (screen != null)
            {
                var canvasGroup = screen.GetComponent<CanvasGroup>();
                canvasGroup.SetAlpha(canvasGroup.alpha == 0 ? 1 : 0);
            }
        }

        [RegisterCommand(Name = "nextsong", Help = "Play the next song")]
        static void CommandNextSong(CommandArg[] args)
        {
            MusicManager.Instance?.PlayNext();
        }
        #endregion
    }
}