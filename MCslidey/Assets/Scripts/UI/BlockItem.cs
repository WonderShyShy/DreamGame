using BFF;
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
                
                _dstPos = _originalPos + pos;
                _dstPos.y = _originalPos.y;

                if (_dstPos.x <= _tmpEdgePos[0])
                {
                    _dstPos.x = _tmpEdgePos[0];
                }
                else if (_dstPos.x >= _tmpEdgePos[1])
                {
                    _dstPos.x = _tmpEdgePos[1];
                }
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

            _blockBgLightEff.transform.localPosition = new Vector2(_originalPos.x + _deltaX * Constant.BlockWidth, _blockBgLightEff.transform.localPosition.y);

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
    }
}
