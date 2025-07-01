using BFF;
using Other;
using UI;
using UnityEngine;

namespace Models
{
    public class InputModel : MonoBehaviour
    {
        private GameObject _currentBlockItem;
        
        // Start is called before the first frame update
        void Start()
        {
            TouchManager.OnTouchDown += OnTouchDown;
            TouchManager.OnTouchMove += OnTouchMove;
            TouchManager.OnTouchUp += OnTouchUp;
        }

        void OnTouchDown(TouchFinger touchFinger)
        {
            if (!Player.UserCanMove) return;
            if (Player.IsBlockMoving) return;
            
            if (touchFinger.touchable == null) return;

            var posIndex = ((int[]) touchFinger.touchable.objectID)[(int) Blocks.Key.Pos];
            _currentBlockItem = Constant.GamePlayScript.GetBlockItemByPosIndex(posIndex);

            if (_currentBlockItem == null) return;
            
            _currentBlockItem.GetComponent<BlockItem>().OnPointerDown();
            _currentBlockItem.GetComponent<BlockItem>().OnBeginDrag(touchFinger.touchPosition);
        }
        
        void OnTouchMove(TouchFinger touchFinger)
        {
            if (touchFinger.touchable == null) return;

            if (_currentBlockItem == null)
            {
                return;
            }
            
            _currentBlockItem.GetComponent<BlockItem>().OnDrag(touchFinger.startDragDelta);
        }
        
        void OnTouchUp(TouchFinger touchFinger)
        {
            if (touchFinger.touchable == null) return;

            if (_currentBlockItem == null)
            {
                return;
            }
            
            _currentBlockItem.GetComponent<BlockItem>().OnEndDrag();
            _currentBlockItem.GetComponent<BlockItem>().OnPointerUp();
        }

        void OnDestroy()
        {
            TouchManager.OnTouchDown -= OnTouchDown;
            TouchManager.OnTouchMove -= OnTouchMove;
            TouchManager.OnTouchUp -= OnTouchUp;
        }

        private void Update()
        {
            TouchManager.InputManagerLoop();
        }
    }
}
