using Core.EventBus;
using Public.SO;
using UnityEngine;

namespace Public.Core.Events
{
    public class CharacterEvent
    {
        public struct SwapCharactor: IEvent
        {
            public CharacterSO character;
            
            public SwapCharactor(CharacterSO charactor)
            {
                this.character = charactor;
            }
        }
        
        public struct TameEvent: IEvent
        {
            public CharacterSO character;
                
            public TameEvent(CharacterSO charactor)
            {
                this.character = charactor;
            }
        }
    }
}