using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class WrongwayTracker : MonoBehaviour
    {
        Competitor _competitor;
        bool _isGoingWrongway;
        float _totalDistance;
        float _lastDistance;
        float _currentDistance;
        float _furthestDistance;
        float _waitTimer;
        float _lastDistanceOverrideTime;

        public void Setup(Competitor c)
        {
            _competitor = c;
        }

        void LateUpdate()
        {
            if(!RGSKCore.Instance.RaceSettings.wrongwayTracking)
                return;

            if (_competitor == null || Time.time < _lastDistanceOverrideTime)
                return;

            _totalDistance = _competitor.DistanceTravelled;

            if (!_isGoingWrongway)
            {
                if (_totalDistance > _furthestDistance)
                {
                    _furthestDistance = _totalDistance;
                }

                if (_totalDistance < _furthestDistance - 10 && _competitor.IsRacing())
                {
                    _waitTimer += Time.deltaTime;

                    if (_waitTimer > 2f)
                    {
                        _waitTimer = 0f;
                        _isGoingWrongway = true;
                        RGSKEvents.OnWrongwayStart.Invoke(_competitor);
                    }
                }
                else
                {
                    _waitTimer = 0f;
                }
            }
            else
            {
                if (_totalDistance < _furthestDistance)
                {
                    _furthestDistance = _totalDistance;
                }

                if (_totalDistance > _furthestDistance + 0.5f || _totalDistance <= 10 || !_competitor.IsRacing())
                {
                    _isGoingWrongway = false;
                    RGSKEvents.OnWrongwayStop.Invoke(_competitor);
                }
            }
        }

        public void SetFurthestDistance(float distance)
        {
            _furthestDistance = distance;
            _lastDistanceOverrideTime = Time.time + 1f;
        }

        public void ResetFurthestDistance()
        {
            _furthestDistance = -1;
        }
    }
}