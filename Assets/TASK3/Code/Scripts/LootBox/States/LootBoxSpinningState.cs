using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using LootBox.View;
using UnityEngine;
using UnityEngine.UI;

namespace LootBox.States
{
    [State(LootBoxStateMachine.SpinningStateName)]
    public sealed class LootBoxSpinningState : FSMState
    {
        private readonly Button _stopButton;
        
        private bool _isStopRequested;
        
        private const string StopOpeningLootBoxButtonName = "Stop";
        private const float UnlockStopButtonTime = 3;

        public LootBoxSpinningState(Button stopButton)
        {
            _stopButton = stopButton;
        }
        
        [Enter]
        private void OnEnter()
        {
            _isStopRequested = false;
            _stopButton.interactable = false;
            
            Settings.Model.EventManager.Invoke(LootBoxSpinView.StartSpinEvent);
        }

        [Exit]
        private void OnExit()
        {
            _stopButton.interactable = false;
        }

        [One(UnlockStopButtonTime)]
        private void UnlockStopButton()
        {
            Debug.Log("Delay is finished");
            _stopButton.interactable = true;
        }
        
        [Bind("OnBtn")]
        private void OnStopButtonClicked(string buttonName)
        {
            if (_isStopRequested || !string.Equals(buttonName, StopOpeningLootBoxButtonName))
                return;
            
            _isStopRequested = true;
            _stopButton.interactable = false;
            
            Settings.Model.EventManager.Invoke(LootBoxSpinView.StopSpinEvent);
        }

        [Bind(LootBoxSpinView.OnFinishSpinEvent)]
        private void OnFinishSpin()
        {
            Settings.Model.EventManager.Invoke(LootBoxSpinView.PlayEffectEvent);
            Parent.Change(LootBoxStateMachine.IdleStateName);
        }
    }
}