using AxGrid.FSM;
using AxGrid.Model;
using UnityEngine.UI;

namespace LootBox.States
{
    [State(LootBoxStateMachine.IdleStateName)]
    public sealed class LootBoxIdleState : FSMState
    {
        private readonly Button _startButton;
        
        private const string StartOpeningLootBoxButtonName = "Start";

        public LootBoxIdleState(Button startButton)
        {
            _startButton = startButton;
        }
        
        [Enter]
        private void OnEnter()
        {
            _startButton.interactable = true;
        }

        [Exit]
        private void OnExit()
        {
            _startButton.interactable = false;
        }
        
        [Bind("OnBtn")]
        private void OnStartButtonClicked(string buttonName)
        {
            if (!string.Equals(buttonName, StartOpeningLootBoxButtonName))
                return;
            
            Parent.Change(LootBoxStateMachine.SpinningStateName);
        }
    }
}