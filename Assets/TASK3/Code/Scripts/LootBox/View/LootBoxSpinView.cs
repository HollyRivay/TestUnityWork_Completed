using System.Collections;
using AxGrid;
using AxGrid.Base;
using AxGrid.Model;
using UnityEngine;

namespace LootBox.View
{
    public sealed class LootBoxSpinView : MonoBehaviourExtBind
    {
        [Header("References")]
        [SerializeField] private RectTransform rouletteRect;
        [SerializeField] private Transform itemsParent;
        [Space]
        [SerializeField] private ParticleSystem winParticles;
        
        [Header("Settings")]
        [SerializeField] private float itemHeight = 150;
        [Space]
        [SerializeField] private float spinningSpeed = 100;
        [SerializeField] private float startSpinTime = 1.5f;
        [SerializeField] private float smoothTime = 0.25f;
        [Space]
        [SerializeField] private float finishSpinTime = 1.5f;
        [SerializeField] private float snapSmoothTime = 1;
        [SerializeField] private float snapDistanceThreshold = 5;
        
        private Transform[] _items;
        private Vector3[] _itemsInitialPositions;
        private Vector3[] _itemsDestinationPositions;

        public const string StartSpinEvent = "StartSpin";
        public const string StopSpinEvent = "StopSpin";
        public const string PlayEffectEvent = "PlayEffect";
        
        public const string OnFinishSpinEvent = "OnFinishSpin";

        [OnStart]
        private void OnStart()
        {
            int itemsAmount = itemsParent.childCount;

            _items = new Transform[itemsAmount];
            for (int i = 0; i < itemsAmount; i++)
            {
                _items[i] = itemsParent.GetChild(i);
            }
            
            _itemsInitialPositions = new Vector3[itemsAmount];
            for (int i = 0; i < itemsAmount; i++)
            {
                _itemsInitialPositions[i] = _items[i].position;
            }
            
            _itemsDestinationPositions = new Vector3[itemsAmount];
        }

        [Bind(StartSpinEvent)]
        private void StartSpin()
        {
            StopAllCoroutines();
            
            StartCoroutine(SpinningRoutine());
        }

        [Bind(StopSpinEvent)]
        private void StopSpin()
        {
            StopAllCoroutines();
            
            StartCoroutine(FinishSpinningRoutine());
        }

        [Bind(PlayEffectEvent)]
        private void PlayEffect()
        {
            winParticles.gameObject.SetActive(true);
            winParticles.Play();
        }
        
        private IEnumerator SpinningRoutine()
        {
            Vector3 smoothVelocity = Vector3.zero;
            float speedTimer = 0;
            
            while (true)
            {
                speedTimer += Time.deltaTime;
                
                float speed = Mathf.Lerp(0, spinningSpeed, Mathf.InverseLerp(0, startSpinTime, speedTimer));
                
                for (int i = 0; i < _items.Length; i++)
                {
                    _items[i].position = Vector3.SmoothDamp(_items[i].position, 
                        _items[i].position - Vector3.up * speed, ref smoothVelocity, smoothTime);

                    float heightDelta = rouletteRect.rect.height * 0.5f + itemHeight * 0.5f;
                    if (_items[i].position.y <= rouletteRect.position.y - heightDelta)
                    {
                        _items[i].position = _itemsInitialPositions[0];
                    }
                }
                
                yield return null;
            }
        }

        private IEnumerator FinishSpinningRoutine()
        {
            Vector3 smoothVelocity = Vector3.zero;
            
            float stopTimer = 0;
            
            while (stopTimer < finishSpinTime)
            {
                stopTimer += Time.deltaTime;
                
                float speed = Mathf.Lerp(spinningSpeed, 0, Mathf.InverseLerp(0, finishSpinTime, stopTimer));
                
                for (int i = 0; i < _items.Length; i++)
                {
                    _items[i].position = Vector3.SmoothDamp(_items[i].position, 
                        _items[i].position - Vector3.up * speed, ref smoothVelocity, smoothTime);

                    float heightDelta = rouletteRect.rect.height * 0.5f + itemHeight * 0.5f;
                    if (_items[i].position.y <= rouletteRect.position.y - heightDelta)
                    {
                        _items[i].position = _itemsInitialPositions[0];
                    }
                }
                
                yield return null;
            }

            float minDistance = float.MaxValue;
            int minDistanceItemIndex = 0;
            
            for (int i = 0; i < _items.Length; i++)
            {
                float distance = (_items[i].position - rouletteRect.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minDistanceItemIndex = i;
                }
            }

            Transform closestToCenterItem = _items[minDistanceItemIndex];
            
            Vector3 offsetToCenter = rouletteRect.position - closestToCenterItem.position;
            
            for (int i = 0; i < _itemsDestinationPositions.Length; i++)
            {
                _itemsDestinationPositions[i] = _items[i].position + offsetToCenter;
            }

            bool snapNotFinished = true;
            
            while (snapNotFinished)
            {
                for (int i = 0; i < _items.Length; i++)
                {
                    _items[i].position = Vector3.SmoothDamp(_items[i].position, 
                        _itemsDestinationPositions[i], ref smoothVelocity, snapSmoothTime);
                    
                    float distanceToDestination = (_itemsDestinationPositions[i] - _items[i].position).sqrMagnitude;

                    if (distanceToDestination <= snapDistanceThreshold * snapDistanceThreshold)
                    {
                        for (int j = 0; j < _items.Length; j++)
                        {
                            _items[j].position = _itemsDestinationPositions[j];
                        }

                        snapNotFinished = false;
                        break;
                    }
                }
                
                yield return null;
            }
            
            Settings.Fsm.Invoke(OnFinishSpinEvent);
        }
    }
}