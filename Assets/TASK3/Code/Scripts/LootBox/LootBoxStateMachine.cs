using AxGrid;
using AxGrid.Base;
using AxGrid.FSM;
using LootBox.States;
using UnityEngine;
using UnityEngine.UI;

namespace LootBox
{
    public sealed class LootBoxStateMachine : MonoBehaviourExtBind
    {
        [Header("References")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button stopButton;
        
        public const string IdleStateName = "IdleState";
        public const string SpinningStateName = "SpinningState";
        
        [OnAwake]
        private void OnAwake()
        {
            Settings.Fsm = new FSM();
            
            Settings.Fsm.Add(new LootBoxIdleState(startButton), new LootBoxSpinningState(stopButton));
            Settings.Fsm.Start(IdleStateName);
        }

        [OnUpdate]
        private void OnUpdate()
        {
            Settings.Fsm.Update(Time.deltaTime);
        }
    }
}