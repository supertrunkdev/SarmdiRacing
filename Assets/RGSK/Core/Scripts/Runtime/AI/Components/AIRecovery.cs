using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [System.Serializable]
    public class AIRecovery : AIComponent
    {
        public bool IsStuck { get; private set; }

        float _stuckTimer;
        float _reverseTimer;
        float _lastReverseAttemptTime;
        int _reverseAttempts;

        public override void Update()
        {
            if (Controller.Behaviour == null || !Controller.Entity.Vehicle.HasControl || !Controller.Behaviour.canRecover)
            {
                _stuckTimer = 0;
                _reverseTimer = 0;
                _reverseAttempts = 0;
                IsStuck = false;
                return;
            }

            if (Controller.InputController.ThrottleInput > 0.1f &&
                Controller.Entity.Vehicle.CurrentSpeed <= Controller.Behaviour.commonSettings.stuckSpeed)
            {
                _stuckTimer += Time.deltaTime;
            }

            if (_stuckTimer > Controller.Behaviour.commonSettings.stuckReverseTime)
            {
                IsStuck = true;
                _reverseTimer += Time.deltaTime;

                if (_reverseTimer > Controller.Behaviour.commonSettings.reverseDuration)
                {
                    _reverseAttempts++;
                    _lastReverseAttemptTime = Time.time;
                    _stuckTimer = 0;
                    _reverseTimer = 0;
                    IsStuck = false;
                }

                if (_reverseAttempts > Controller.Behaviour.commonSettings.maxReverseAttempts)
                {
                    _reverseAttempts = 0;
                    Controller.Entity?.Repositioner?.Reposition();
                }
            }

            if (_reverseAttempts > 0 && Time.time > _lastReverseAttemptTime + 10)
            {
                _reverseAttempts = 0;
                _stuckTimer = 0;
            }
        }
    }
}