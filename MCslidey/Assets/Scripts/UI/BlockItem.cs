using BFF;
using Manager;
using Models;
using Other;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class BlockItem : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject blockImgNode;
        public Sprite[] blockSpriteFrameColor1;
        public Sprite[] blockSpriteFrameColor2;
        public Sprite[] blockSpriteFrameColor3;
        public Sprite[] blockSpriteFrameColor4;
        public Sprite[] blockSpriteFrameColor5;

        private Sprite _tmpSprite;
        private Vector2 _startPos;
        private Vector2 _originalPos;
        private int[] _data;
        private int[] _tmpEdgePos;
        private int _deltaX = 0;
        //当前位置
        private Vector2 _dstPos;

        // 智能拖动相关变量
        private float _currentDragSpeed = 0f;        // 当前拖动速度
        private float _averageDragSpeed = 0f;        // 平均拖动速度
        private Vector2 _lastDragPos = Vector2.zero; // 上一帧拖动位置
        private float _lastDragTime = 0f;            // 上一帧时间
        private bool _isSlowDragMode = false;        // 当前是否为慢速拖动模式
        private int _lockedGridOffset = int.MaxValue; // 当前锁定的格子偏移（用于避免频繁跳跃）
        private bool _isGridLocked = false;          // 是否已锁定到某个格子

        private GameObject _blockBgLightEff;
        private GameObject _blockBgTip;
        private GameObject _blockLightTip;

        public bool IsMovedBlockItem { get; set; } = false;
        public bool WillBeHangRemove { get; set; } = false;
        public int OriHang { get; set; } = 0;
        public int LastDownOffsetY { get; set; } = 0;

        // Start is called before the first frame update
        void Start()
        {
            Input.multiTouchEnabled = false;
        }

        public int[] GetData()
        {
            return _data;
        }

        public int GetColor()
        {
            return _data[(int)Blocks.Key.Color];
        }

        public int GetLength()
        {
            return _data[(int)Blocks.Key.Length];
        }

        public int GetPosIndex()
        {
            return _data[(int)Blocks.Key.Pos];
        }

        public bool IsSpecial()
        {
            return _data[(int)Blocks.Key.Special] != 0;
        }

        public int GetSpecial()
        {
            return _data[(int)Blocks.Key.Special];
        }

        public bool HaveIce()
        {
            return Blocks.HaveIce(GetPosIndex());
        }

        public bool IsIce()
        {
            return Blocks.IsIce(GetPosIndex());
        }

        public Sprite GetSprite()
        {
            return _tmpSprite;
        }

        public void UpdateUi(int[] data, GameObject bgLightEff = null, GameObject bgTip = null, GameObject lightTip = null)
        {
            if (gameObject.GetComponent<Touchable>() == null)
            {
                gameObject.AddComponent<Touchable>();
            }
            GetComponent<Touchable>().objectID = data;

            _data = data;
            _blockBgLightEff = bgLightEff;
            _blockBgTip = bgTip;
            _blockLightTip = lightTip;

            var blockLength = data[(int)Blocks.Key.Length];
            switch (data[(int)Blocks.Key.Color])
            {
                case (int)Blocks.Color.Blue:
                    _tmpSprite = blockSpriteFrameColor1[blockLength - 1];
                    break;
                case (int)Blocks.Color.Green:
                    _tmpSprite = blockSpriteFrameColor2[blockLength - 1];
                    break;
                case (int)Blocks.Color.Pink:
                    _tmpSprite = blockSpriteFrameColor3[blockLength - 1];
                    break;
                case (int)Blocks.Color.Red:
                    _tmpSprite = blockSpriteFrameColor4[blockLength - 1];
                    break;
                case (int)Blocks.Color.Yellow:
                    _tmpSprite = blockSpriteFrameColor5[blockLength - 1];
                    break;
            }

            blockImgNode.GetComponent<Image>().sprite = _tmpSprite;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth * blockLength, Constant.BlockHeight);
        }

        public void OnPointerDown()
        {
            if (!Player.UserCanMove) return;

            if (IsSpecial() && GetSpecial() == (int)Blocks.Special.Stone)
            {
                Constant.EffCtrlScript.ShowBlockShakeEff(gameObject);
                return;
            }

            if (Player.IsBlockMoving) return;

            if (Constant.SceneVersion == "3")
            {
                _blockLightTip.GetComponent<Image>().sprite = blockImgNode.GetComponent<Image>().sprite;
                _blockLightTip.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth * GetLength(), Constant.BlockHeight);
                _blockLightTip.SetActive(true);

                _blockLightTip.transform.localPosition = transform.localPosition;
                _blockLightTip.transform.SetAsLastSibling();
            }
        }

        public void OnPointerUp()
        {
            if (Constant.SceneVersion == "3")
            {
                HideBlockBgLightEff();
            }
        }

        public void OnBeginDrag(Vector2 pos)
        {
            Constant.GamePlayScript.ResetClearTipTime();
            transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + Blocks.GetLieByPos(GetPosIndex()) * Constant.BlockWidth, transform.localPosition.y);

            if (!Player.UserCanMove) return;

            if (IsSpecial() && GetSpecial() == (int)Blocks.Special.Stone)
            {
                Constant.EffCtrlScript.ShowBlockShakeEff(gameObject);
                return;
            }

            if (Player.IsBlockMoving) return;
            Player.IsBlockMoving = true;

            _startPos = pos * 1;
            _originalPos = transform.localPosition;
            var tmpEdgeIndex = Blocks.GetEdgeIndex(_data);
            _tmpEdgePos = new[] { Constant.BlockGroupEdgeLeft + Constant.BlockWidth * tmpEdgeIndex[0], Constant.BlockGroupEdgeLeft + Constant.BlockWidth * tmpEdgeIndex[1] };

            _deltaX = 0;
            
            // 初始化智能拖动状态（不影响现有逻辑）
            ResetDragState();
            
            ShowBlockBgLightEff();
        }

        /// <summary>
        /// pos 相对于按下时的偏移量
        /// </summary>
        /// <param name="pos"></param>
        public void OnDrag(Vector2 pos)
        {
            if (IsSpecial() && GetSpecial() == (int)Blocks.Special.Stone)
            {
                return;
            }
            Constant.GamePlayScript.ResetClearTipTime();

            if (!Player.UserCanMove) return;
            if (!Player.IsBlockMoving) return;

            if (_tmpEdgePos != null)
            {
                // 原始位置计算（保持不变）
                Vector2 rawPos = _originalPos + pos;
                rawPos.y = _originalPos.y;
                
                // 智能拖动逻辑（新增，可通过开关控制）
                if (Constant.SmartDragAttractionSwitch)
                {
                    // 计算拖动速度
                    float dragSpeed = CalculateDragSpeed(rawPos);
                    
                    // 判断拖动模式
                    _isSlowDragMode = dragSpeed < Constant.SmartDragSpeedThreshold;
                    
                    // 应用吸附效果
                    _dstPos = ApplyGridAttraction(rawPos, _isSlowDragMode);
                    
                    // 调试模式输出
                    if (Constant.SmartDragDebugMode)
                    {
                        Debug.Log($"拖动速度: {dragSpeed:F1}, 模式: {(_isSlowDragMode ? "慢速吸附" : "快速跟随")}");
                    }
                }
                else
                {
                    // 原始行为（功能关闭时）
                    _dstPos = rawPos;
                }

                // 边界检查（保持原逻辑不变）
                if (_dstPos.x <= _tmpEdgePos[0])
                {
                    _dstPos.x = _tmpEdgePos[0];
                }
                else if (_dstPos.x >= _tmpEdgePos[1])
                {
                    _dstPos.x = _tmpEdgePos[1];
                }
                
                // 应用位置（保持不变）
                transform.localPosition = _dstPos;
                _deltaX = Tools.ChinaRound((transform.localPosition.x - _originalPos.x) / Constant.BlockWidth);
                ShowBlockBgLightEff();
            }
        }

        public void OnEndDrag()
        {
            if (IsSpecial() && GetSpecial() == (int)Blocks.Special.Stone)
            {
                return;
            }

            Constant.GamePlayScript.ResetClearTipTime();

            if (!Player.UserCanMove) return;
            if (!Player.IsBlockMoving) return;
            Player.IsBlockMoving = false;

            HideBlockBgLightEff();

            // 清理拖动状态
            if (Constant.SmartDragAttractionSwitch)
            {
                ResetDragState();
            }

            if (Player.IsInGuide())
            {
                var guideData = Player.GetGuideStepData(Player.GetGuideStep());
                if (guideData != null)
                {
                    var startIndex = guideData[0];
                    var endIndex = guideData[1];
                    var moveX = Blocks.GetLieByPos(endIndex) - Blocks.GetLieByPos(startIndex);
                    if (startIndex == GetPosIndex() && moveX == _deltaX)
                    {
                        Player.CompleteGuide();
                        Constant.EffCtrlScript.RemoveGuideStepEff();
                    }
                    else
                    {
                        _deltaX = 0;
                    }
                }
            }

            if (_tmpEdgePos != null)
            {
                transform.localPosition = new Vector2(_originalPos.x + Constant.BlockWidth * _deltaX, _originalPos.y);
                if (_deltaX != 0)
                {
                    Constant.GamePlayScript.MoveEnd(new[] { _deltaX, 0, _data[(int)Blocks.Key.Pos] });
                }
                else
                {
                    // 方块回到原位，播放Minecraft蜡烛音效
                    ManagerAudio.PlaySound("add_candle1");
                }
                _tmpEdgePos = null;
            }
        }

        private void ShowBlockBgLightEff()
        {
            if (!_blockBgTip.activeInHierarchy && !_blockBgLightEff.activeInHierarchy)
            {
                _blockBgTip.transform.localPosition = _originalPos + new Vector2(Constant.BlockWidth * GetLength() / 2f, Constant.BlockHeight / 2f);
                _blockBgTip.GetComponent<Image>().sprite = blockImgNode.GetComponent<Image>().sprite;
                _blockBgTip.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth * GetLength(), Constant.BlockHeight);
                _blockBgTip.SetActive(true);

                _blockBgLightEff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth * GetLength(), Constant.BlockHeight * Constant.Hang);
                _blockBgLightEff.transform.localPosition = new Vector2(_originalPos.x, _blockBgLightEff.transform.localPosition.y);
                _blockBgLightEff.SetActive(true);
            }

            // 修改：让长条虚影实时跟随方块位置，而不是跳跃式移动
            _blockBgLightEff.transform.localPosition = new Vector2(transform.localPosition.x, _blockBgLightEff.transform.localPosition.y);

            if (Constant.SceneVersion == "3")
            {
                _blockLightTip.transform.localPosition = transform.localPosition;
                _blockLightTip.transform.SetAsLastSibling();
            }
        }

        private void HideBlockBgLightEff()
        {
            _blockBgTip.SetActive(false);
            _blockBgLightEff.SetActive(false);

            if (Constant.SceneVersion == "3")
            {
                _blockLightTip.SetActive(false);
            }
        }

        /// <summary>
        /// 检查是否应该禁用智能拖动（特殊情况）
        /// </summary>
        private bool ShouldDisableSmartDrag()
        {
            // 新手引导期间禁用智能拖动，避免干扰
            if (Player.IsInGuide())
            {
                return true;
            }
            
            // 石头方块禁用
            if (IsSpecial() && GetSpecial() == (int)Blocks.Special.Stone)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// 计算拖动速度（像素/秒）
        /// </summary>
        private float CalculateDragSpeed(Vector2 currentPos)
        {
            if (!Constant.SmartDragAttractionSwitch || ShouldDisableSmartDrag()) 
                return 0f;
            
            float currentTime = Time.unscaledTime;
            
            // 防止除零错误
            if (_lastDragTime > 0)
            {
                float deltaTime = currentTime - _lastDragTime;
                if (deltaTime > 0.001f) // 最小时间间隔，避免极小的deltaTime
                {
                    Vector2 deltaPos = currentPos - _lastDragPos;
                    _currentDragSpeed = deltaPos.magnitude / deltaTime;
                    
                    // 限制最大速度，避免异常值
                    _currentDragSpeed = Mathf.Clamp(_currentDragSpeed, 0f, 5000f);
                    
                    // 使用指数移动平均平滑速度
                    _averageDragSpeed = Mathf.Lerp(_averageDragSpeed, _currentDragSpeed, 
                        Constant.SmartDragSpeedSmoothingFactor);
                }
            }
            
            _lastDragPos = currentPos;
            _lastDragTime = currentTime;
            
            return _averageDragSpeed;
        }

        /// <summary>
        /// 应用格子吸附效果 - 跳跃式吸附
        /// </summary>
        private Vector2 ApplyGridAttraction(Vector2 rawPos, bool isSlowMode)
        {
            if (!Constant.SmartDragAttractionSwitch || !isSlowMode || ShouldDisableSmartDrag())
            {
                // 功能关闭或快速模式，清除锁定状态
                _isGridLocked = false;
                _lockedGridOffset = int.MaxValue;
                return rawPos;
            }
            
            Vector2 result = rawPos;
            
            // 计算当前格子偏移（相对于原始位置）
            float gridOffset = (rawPos.x - _originalPos.x) / Constant.BlockWidth;
            int nearestGridOffset = Mathf.RoundToInt(gridOffset);
            
            // 计算到最近格子中心的距离比例（相对于格子宽度）
            float distanceRatio = Mathf.Abs(gridOffset - nearestGridOffset);
            
            // 定义阈值
            float attractionThreshold = Constant.SmartDragAttractionThreshold; // 0.3f = 格子宽度的30%
            float deadZone = Constant.SmartDragDeadZone; // 0.15f = 格子宽度的15%
            
            if (_isGridLocked)
            {
                // 已锁定状态：只有离开死区才能解锁
                if (_lockedGridOffset != nearestGridOffset)
                {
                    // 移动到了不同的格子区域
                    float distanceToLocked = Mathf.Abs(gridOffset - _lockedGridOffset);
                    if (distanceToLocked > deadZone)
                    {
                        // 离开死区，解除锁定
                        _isGridLocked = false;
                        _lockedGridOffset = int.MaxValue;
                    }
                    else
                    {
                        // 仍在死区内，保持锁定
                        result.x = _originalPos.x + _lockedGridOffset * Constant.BlockWidth;
                        return result;
                    }
                }
                else
                {
                    // 在同一格子内，保持锁定
                    result.x = _originalPos.x + _lockedGridOffset * Constant.BlockWidth;
                    return result;
                }
            }
            
            // 未锁定状态：检查是否应该吸附
            if (distanceRatio < attractionThreshold)
            {
                // 进入吸附区域，直接跳到格子中心
                _isGridLocked = true;
                _lockedGridOffset = nearestGridOffset;
                result.x = _originalPos.x + nearestGridOffset * Constant.BlockWidth;
                
                if (Constant.SmartDragDebugMode)
                {
                    Debug.Log($"🧲 吸附到格子: {nearestGridOffset}, 距离比例: {distanceRatio:F2}");
                }
            }
            else
            {
                // 在自由区域，完全跟随手指
                result.x = rawPos.x;
            }
            
            return result;
        }

        /// <summary>
        /// 重置拖动状态
        /// </summary>
        private void ResetDragState()
        {
            _currentDragSpeed = 0f;
            _averageDragSpeed = 0f;
            _lastDragPos = Vector2.zero;
            _lastDragTime = 0f;
            _isSlowDragMode = false;
            _isGridLocked = false;
            _lockedGridOffset = int.MaxValue;
        }
    }
}
