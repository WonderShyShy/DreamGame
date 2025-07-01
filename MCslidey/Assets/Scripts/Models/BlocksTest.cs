using System.Collections.Generic;
using Other;

namespace Models
{
    public class BlocksTest
    {
        private List<List<int[]>> _readyBlocks;

        private int _enabledAudoClearCount = 7;
        private int _curGoldEffTypeIndex = -1;
        private int[] _goldSpecial = new[]
        {
//            (int) Blocks.SpecialGoldEffType.Split43,
            (int) Blocks.SpecialGoldEffType.Color,
        };
        
        public List<int[]> GetNextBlocksData()
        {
            //v9
            if (_readyBlocks == null)
            {
                _readyBlocks = new List<List<int[]>>
                {
                    new List<int[]>
                    {
                        new[] {1, (int) Blocks.Color.Red, 7, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {1, (int) Blocks.Color.Blue, 2, 0, 0, 0},
                        new[] {3, (int) Blocks.Color.Green, 3, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Yellow, 7, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {2, (int) Blocks.Color.Red, 0, 0, 0, 0},
                        new[] {2, (int) Blocks.Color.Pink, 2, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Yellow, 6, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Yellow, 7, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {2, (int) Blocks.Color.Blue, 0, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Yellow, 2, 0, 0, 0},
                        new[] {2, (int) Blocks.Color.Pink, 6, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {3, (int) Blocks.Color.Blue, 0, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Yellow, 3, 0, 0, 0},
                        new[] {2, (int) Blocks.Color.Red, 6, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {3, (int) Blocks.Color.Pink, 0, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Red, 3, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Blue, 4, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Yellow, 6, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {1, (int) Blocks.Color.Blue, 0, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Blue, 1, 0, 0, 0},
                        new[] {3, (int) Blocks.Color.Yellow, 2, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Red, 5, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {1, (int) Blocks.Color.Yellow, 1, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Green, 3, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Blue, 4, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Pink, 5, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Pink, 6, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {2, (int) Blocks.Color.Red, 1, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Blue, 3, 0, 0, 0},
                        new[] {2, (int) Blocks.Color.Yellow, 5, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Green, 7, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {1, (int) Blocks.Color.Pink, 0, 0, 0, 0},
                        new[] {2, (int) Blocks.Color.Pink, 1, 0, 0, 0},
                        new[] {2, (int) Blocks.Color.Yellow, 4, (int) Blocks.Special.Gold, 0, 0},
                        new[] {1, (int) Blocks.Color.Green, 7, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {2, (int) Blocks.Color.Green, 0, 0, 0, 0},
                        new[] {1, (int) Blocks.Color.Blue, 2, 0, 0, 0},
                        new[] {3, (int) Blocks.Color.Red, 5, 0, 0, 0},
                    },
                    
                    new List<int[]>
                    {
                        new[] {1, (int) Blocks.Color.Red, 6, 0, 0, 0},
                    },

            //v8
//            if (_readyBlocks == null)
//            {
//                 _readyBlocks = new List<List<int[]>>
//                {
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 4, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Yellow, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Yellow, 2, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 2, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 2, 0, 0, 0},
//                    },
//                    
//                    //后全消除
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Green, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Yellow, 3, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Pink, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Pink, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Pink, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Green, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                    }
                
                //v7
//                _readyBlocks = new List<List<int[]>>
//                {
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 4, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Yellow, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Yellow, 2, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 2, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 2, 0, 0, 0},
//                    },
//                    
//                    //蛋糕
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 3, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 3, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 6, 0, 0, 0},
//                    },
//                    
//                    //消除蛋糕
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Green, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Green, 5, 0, 0, 0},
//                    }
//                };

                //v6
//                _readyBlocks = new List<List<int[]>>
//                {
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 4, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Yellow, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Yellow, 2, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 2, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 2, 0, 0, 0},
//                    },
//                    
//                    //红心
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 3, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Green, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Green, 5, 0, 0, 0},
//                    },
//                    
//                    //消除红心
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 4, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Green, 3, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Yellow, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Yellow, 3, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Yellow, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 2, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Pink, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Pink, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Red, 3, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Red, 0, 0, 0, 0},
//                    },
//                };
                
//                v5
//                _readyBlocks = new List<List<int[]>>
//                {
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Blue, 0, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int) Blocks.Color.Red, 0, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Yellow, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Green, 3, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Pink, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int) Blocks.Color.Green, 1, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Blue, 3, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Red, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Red, 4, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Blue, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Blue, 3, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int) Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Red, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int) Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Green, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Blue, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Green, 1, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Blue, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Yellow, 3, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Yellow, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Pink, 2, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Pink, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int) Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Green, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int) Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Pink, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int) Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Blue, 2, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Red, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int) Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Blue, 3, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Pink, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Yellow, 2, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Red, 3, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Pink, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{4, (int) Blocks.Color.Green, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int) Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 2, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Green, 3, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 5, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Red, 3, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Blue, 5, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Pink, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int) Blocks.Color.Yellow, 1, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Red, 3, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Green, 1, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Pink, 3, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Yellow, 6, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Red, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int) Blocks.Color.Blue, 1, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Red, 4, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Yellow, 5, 0, 0, 0},
//                    },
//                };

//                v4
//                _readyBlocks = new List<List<int[]>>
//                {
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Blue, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int) Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Blue, 6, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int) Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Green, 2, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 5, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 6, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Yellow, 1, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Blue, 4, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Yellow, 5, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Blue, 1, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Blue, 2, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 3, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Green, 4, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int) Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Blue, 4, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Green, 1, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Blue, 2, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 4, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Blue, 5, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Blue, 1, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Green, 3, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Blue, 5, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Blue, 1, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Yellow, 4, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 7, 0, 0, 0}
//                    },
//                    
//                    //顶行
//                    new List<int[]>
//                    {
//                        new []{3, (int) Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Yellow, 4, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 7, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int) Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Green, 1, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Pink, 4, 0, 0, 0},
//                        new []{3, (int) Blocks.Color.Blue, 5, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int) Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{2, (int) Blocks.Color.Green, 4, 0, 0, 0},
//                        new []{1, (int) Blocks.Color.Green, 6, 0, 0, 0}
//                    },
//                };    

//                v3
//                _readyBlocks = new List<List<int[]>>
//                {
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 4, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Yellow, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Yellow, 2, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 2, (int) Blocks.Special.Gold, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 2, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 4, 0, 0, 0},
//                    },
//                    
//                    //红心
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Red, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Red, 3, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Green, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Green, 5, 0, 0, 0},
//                    },
//                    
//                    //消除红心
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 4, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    //最后的闪电特效
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 3, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 2, (int)Blocks.Special.Gold, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Pink, 1, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Yellow, 6, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Pink, 1, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Yellow, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Pink, 4, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Pink, 7, 0, 0, 0},
//                    },
//                };
                
//                v2
                //6连消
//                _readyBlocks = new List<List<int[]>>
//                {
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 4, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Red, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Yellow, 2, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Pink, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 2, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 4, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Pink, 2, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Red, 3, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 4, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Pink, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Yellow, 1, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Yellow, 3, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{1, (int)Blocks.Color.Red, 1, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Yellow, 2, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Pink, 5, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Pink, 6, 0, 0, 0},
//                    },
//                    
//                    //金色切割
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Pink, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{4, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 2, (int)Blocks.Special.Gold, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Pink, 1, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Yellow, 6, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Pink, 1, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Blue, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Green, 0, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Yellow, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Pink, 4, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Pink, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Green, 2, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Pink, 5, 0, 0, 0},
//                    },
//                    
//                    //再来一个颜色消
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Green, 4, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 0, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Green, 4, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
////                        new []{3, (int)Blocks.Color.Red, 3, (int)Blocks.Special.Gold, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 3, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Yellow, 6, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Blue, 1, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Yellow, 4, 0, 0, 0},
//                        new []{2, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Pink, 7, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Yellow, 3, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                    },
//                    
//                    
//                    new List<int[]>
//                    {
//                        new []{2, (int)Blocks.Color.Pink, 2, 0, 0, 0},
//                        new []{4, (int)Blocks.Color.Green, 4, 0, 0, 0},
//                    },
//                    
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Blue, 5, 0, 0, 0},
//                    },
//                    
//                    
//                    new List<int[]>
//                    {
//                        new []{3, (int)Blocks.Color.Yellow, 0, 0, 0, 0},
//                        new []{3, (int)Blocks.Color.Red, 3, 0, 0, 0},
//                        new []{1, (int)Blocks.Color.Green, 6, 0, 0, 0},
//                    },
//                };                

//                v1
//                _readyBlocks = new List<List<int[]>>
//                {
//                    //起步6行展示4连消
//                    new List<int[]>
//                    {
//                        new []{1, 4, 3, 0, 0, 0},
//                    },
//                            
//                    new List<int[]>
//                    {
//                        new []{2, 4, 0, 0, 0, 0},
//                        new []{1, 3, 2, 0, 0, 0},
//                        new []{2, 1, 3, 0, 0, 0}
//                    },
//                            
//                    new List<int[]>
//                    {
//                        new []{3, 2, 0, 0, 0, 0},
//                        new []{4, 3, 4, 0, 0, 0}
//                    },
//                            
//                    new List<int[]>
//                    {
//                        new []{3, 4, 0, 0, 0, 0},
//                        new []{3, 5, 3, 0, 0, 0}
//                    },
//                            
//                    new List<int[]>
//                    {
//                        new []{4, 1, 2, 0, 0, 0},
//                        new []{2, 2, 6, 0, 0, 0}
//                    },
//                
//                    new List<int[]>
//                    {
//                        new []{2, 3, 0, 0, 0, 0},
//                        new []{3, 4, 3, 0, 0, 0},
//                        new []{2, 5, 6, 0, 0, 0},
//                    },
//                    
//                    //中间6行展示消青铜
//                    new List<int[]>
//                    {
//                        new []{3, 2, 1, 0, 0, 0},
//                        new []{3, 3, 5, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, 1, 1, 0, 0, 0},
//                        new []{3, 2, 5, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, 2, 0, 0, 0, 0},
//                        new []{2, 4, 2, 0, 0, 0},
//                        new []{2, 5, 5, 0, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, 3, 0, 0, 0, 0},
//                        new []{4, 1, 2, 2, 0, 0},
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{2, 4, 1, 0, 0, 0},
//                        new []{2, 2, 3, 0, 0, 0},
//                        new []{3, 2, 5, 0, 0, 0},
//                    },
//                    
//                    
//                    new List<int[]>
//                    {
//                        new []{3, 4, 0, 0, 0, 0},
//                        new []{2, 3, 3, 0, 0, 0},
//                        new []{2, 5, 6, 0, 0, 0}
//                    },
//                    
//                    //生成金色块的行
//                    new List<int[]>
//                    {
//                        new []{2, 1, 0, 0, 0, 0},
//                        new []{2, 3, 3, 3, 0, 0},
//                        new []{2, 2, 6, 0, 0, 0}
//                    },
//                    
//                    new List<int[]>
//                    {
//                        new []{3, 2, 0, 0, 0, 0},
//                        new []{3, 2, 3, 0, 0, 0},
//                    },
                };
            }

            if (_readyBlocks.Count <= 0) return null;
            var data = _readyBlocks[0];
            _readyBlocks.RemoveAt(0);
            return data;
        }

        public bool ShouldAddBlockItems()
        {
            //v8
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 6:
//                    return false;
//            }
            
            //v7
//            if (_enabledAudoClearCount <= -7)
//            {
//                return false;
//            }

//            v6
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 6:
//                    return false;
//            }
            
//            v5
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 2:
//                case 4:
//                case 7:
//                    return false;
//            }

//            v4
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 1:
//                case 3:
//                    return false;
//            }

            return true;
        }

        public int GetMaxHangNum()
        {
            //v9
            switch (Constant.GameStatusData.SlideNumber)
            {
                case 0:
                    return 6;
                case 1:
                case 2:
                case 3:
                    return 3;
                case 4:
                    return 4;
                default:
                    return 6;
            }
            
            //v8
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 0:
//                case 1:
//                case 2:
//                case 3:
//                case 4:
//                    return 4;
//                case 5:
//                    return 7;
//                case 6:
//                    return 0;
//                default:
//                    return 6;
//            }
            
            //v7
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 0:
//                    return 4;
//                case 1:
//                    return 6;
//                case 2:
//                    return 4;
//                case 3:
//                    return 9;
//            }
            
            //v6
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 0:
//                    return 7;
//                case 1:
//                    return 6;
//                case 2:
//                    return 4;
//                case 3:
//                    return 8;
//                case 4:
//                    return 7;
//                case 5:
//                    return 4;
//                case 6:
//                    return 0;
//                default:
//                    return 4;
//            }
            
//            v5
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 0:
//                    return 7;
//                case 1:
//                    return 6;
//                case 3:
//                    return 4;
//                case 5:
//                case 6:
//                    return 9;
//            }

//            v4
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 0:
//                    return 9;
//                case 1:
//                    return 5;
//                case 2:
//                    return 4;
//                case 3:
//                    return 1;
//            }
//
//            v3
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 0:
//                    return 7;
//                case 1:
//                    return 2;
//                case 2:
//                    return 6;
//                case 3:
//                    return 8;
//                case 4:
//                    return 9;
//                default:
//                    return 6;
//            }

//            v2
//            return 9;
//            v1
//            return 6;
        }

        public int GetCurrentScore()
        {
            return 10000;
        }

        public int GetGoldEffType()
        {
//            ++_curGoldEffTypeIndex;
//            return _goldSpecial[_curGoldEffTypeIndex];
            return _goldSpecial[0];
        }

        public bool EnableIceBlock()
        {
            return true;
        }

        public List<int> GetIceBlocks()
        {
            if (Constant.GameStatusData.SlideNumber == 2)
            {
                return new List<int>
                {
                    1,
                    5
                };
            }

            return null;
        }

        public bool EnableAutoClear()
        {
            //v7
//            switch (Constant.GameStatusData.SlideNumber)
//            {
//                case 3:
//                    DebugEx.Log("EnableAutoClear", _enabledAudoClearCount);
//                    --_enabledAudoClearCount;
//                    if (_enabledAudoClearCount >= 0)
//                    {
//                        return false;
//                    }
//
//                    break;
//            }

            return true;
        }

        public bool OnlyClearOne()
        {
            //v7
//            if (Constant.GameStatusData.SlideNumber == 3)
//            {
//                return true;
//            }

            return false;
        }

        public bool ShouldShowClearHangEff()
        {
            //v7
//            if (Constant.GameStatusData.SlideNumber == 3 && _enabledAudoClearCount < 7)
//            {
//                return true;
//            }

            return false;
        }
    }
}
