using DG.Tweening;
using Models;
using Other;
using UnityEngine;

namespace UI
{
    public class MeteorGroup : MonoBehaviour
    {
        public GameObject meteorPrefab;

        private readonly ObjectPool _meteorPool = new ObjectPool();
        private int _sx;
        private int _sy;
        private int _ex;
        private int _ey;
        // Start is called before the first frame update
        void Start()
        {
            _meteorPool.CreateObject(meteorPrefab, 1);
            _meteorPool.Put(meteorPrefab);

            var size = gameObject.GetComponent<RectTransform>().sizeDelta;
            _sx = (int) -size.x / 2;
            _sy = (int) -size.y / 2;
            _ex = (int) size.x / 2;
            _ey = (int) size.y / 2;
            
            AddMeteor();
        }

        private void AddMeteor()
        {
            var meteor = _meteorPool.Get();
            meteor.transform.SetParent(gameObject.transform);
            var meteorScale = 2 * Tools.GetNumFromRange(75, 100) / 100f;
            meteor.transform.localScale = new Vector2(meteorScale, meteorScale);
            meteor.transform.localPosition = new Vector2(Tools.GetNumFromRange(_sx, _ex), Tools.GetNumFromRange(_sy, _ey));
            meteor.transform.DOBlendableLocalMoveBy(
                new Vector3(Constant.ScreenWidth * 2, -Constant.ScreenWidth * 2, 0), Tools.GetNumFromRange(35, 50)).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    _meteorPool.Put(meteor);
                    AddMeteor();
                });

            if (Tools.GetNumFromRange(0, 10) >= 8)
            {
                var meteor2 = _meteorPool.Get();
                meteor2.transform.SetParent(gameObject.transform);
                var meteorScale2 = 2 * Tools.GetNumFromRange(50, 100) / 100f;
                meteor2.transform.localScale = new Vector2(meteorScale2, meteorScale2);
                meteor2.transform.localPosition = new Vector2(Tools.GetNumFromRange(_sx, _ex), Tools.GetNumFromRange(_sy, _ey));
                meteor2.transform.DOBlendableLocalMoveBy(
                    new Vector3(Constant.ScreenWidth * 2, -Constant.ScreenWidth * 2, 0), Tools.GetNumFromRange(20, 30)).SetEase(Ease.Linear).OnComplete(
                    () =>
                    {
                        _meteorPool.Put(meteor2);
                    });
            }
        }
    }
}
