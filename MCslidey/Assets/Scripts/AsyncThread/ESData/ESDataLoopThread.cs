//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading.Tasks;
//using BFF.Lib;
//using Newtonsoft.Json;
//
//namespace BFF
//{
//    public class ESDataLoopThread : BFFLoopThread
//    {
//        private string TAG = "ESDataLoopThread----------";
//        private Dictionary<string, FileRecordQueue> _recFileDict = new Dictionary<string, FileRecordQueue>();
//        private static ESDataLoopThread _instance;
//        private ESDataSender _eventSender;
//        private Dictionary<string, int> _alreadyDelCountDict = new Dictionary<string, int>
//        {
//            {ESDataConfig.esTypeGame, 0},
//            {ESDataConfig.esTypeAd, 0}
//        };
//        private static readonly object LockInstance = new object();
//        
//        private int _totalWriteCount = 0;
//        private int _sendSuccessCount = 0;
//        private int _totalCount = 0;
//        private int _failCount = 0;
//        
//        public static ESDataLoopThread Instance
//        {
//            get
//            {
//                lock (LockInstance)
//                {
//                    if (_instance == null)
//                    {
//                        _instance = new ESDataLoopThread();
//                        _instance.Start("ESDataLoopThread");
//                        Helper.Log("ESDataLoopThread----------", "start");
//                    }
//                    return _instance;
//                }
//            }
//        }
//
//        public void WriteGameRecord(string savePath, string h5Action, Hashtable content)
//        {
//            Handler.Post(() =>
//            {
//                Helper.Log(TAG, ESDataConfig.esTypeGame, h5Action, "GenerateData start");
//                var data = ESDataBase.GenerateData(h5Action, content);
//                Helper.Log(TAG, ESDataConfig.esTypeGame, h5Action, "GenerateData ok");
//                Helper.Log(TAG, JsonConvert.SerializeObject(data));
//                WriteRecord(savePath, data, ESDataConfig.esTypeGame);
//            });
//        }
//
//        public void WriteAdRecord(string savePath, string content, string adChance, bool isBannerReturn)
//        {
//            Handler.Post(() =>
//            {
//                Helper.Log(TAG, ESDataConfig.esTypeAd, adChance, "GenerateAdData start");
//                var data = ESDataBase.GenerateAdData(content, adChance, isBannerReturn);
//                Helper.Log(TAG, ESDataConfig.esTypeAd, adChance, "GenerateAdData ok");
//                Helper.Log(TAG, JsonConvert.SerializeObject(data));
//                WriteRecord(savePath, data, ESDataConfig.esTypeAd);
//            });
//        }
//        
//        public void WriteBaseData(string savePath)
//        {
//            Helper.Log(TAG, ESDataConfig.esTypeAd, "GenerateBaseData start");
//            var data = ESDataBase.GenerateBaseData();
//            Helper.Log(TAG, ESDataConfig.esTypeAd, "GenerateBaseData ok");
//            Helper.Log(TAG, JsonConvert.SerializeObject(data));
//            WriteRecord(savePath, data, ESDataConfig.esTypeAd);
//        }
//
//        private void WriteRecord(string path, Hashtable content, string esType = "game")
//        {
//            ++_totalWriteCount;
//            Handler.Post(async () =>
//            {
//                if (!_recFileDict.ContainsKey(esType))
//                {
//                    var fileName = Path.Combine(path, ESDataConfig.GetESFileName(esType));
//                    _recFileDict.Add(esType, new FileRecordQueue(fileName, ESDataConfig.GetESRecordSize(esType), ESDataConfig.GetESRecordMaxCount(esType)));
//                    Helper.Log(TAG, esType, "创建或读取文件");
//                    await Task.Delay(100);
//                }
//                
//                var contentString = JsonConvert.SerializeObject(content);
//                
//                if (_eventSender == null)
//                {
//                    _eventSender = new ESDataSender();
//                }
//
//                if (_eventSender.IsSending())
//                {
//                    Helper.Log(TAG, esType, "sender is sending, wait and write");
//                    if (_recFileDict[esType].RecordCount() >= ESDataConfig.GetESRecordMaxCount(esType))
//                    {
//                        _alreadyDelCountDict[esType] += _recFileDict[esType].RecordCount() - ESDataConfig.GetESRecordMaxCount(esType) + 1;
//                    }
//                    _recFileDict[esType].WriteRecord(contentString);
//                    return;
//                }
//                
//                Helper.Log(TAG, esType, "record count before write", _recFileDict[esType].RecordCount());
//                _recFileDict[esType].WriteRecord(contentString);
//                await Task.Delay(100);
//                Helper.Log(TAG, esType, "record count after write", _recFileDict[esType].RecordCount());
//                
//                if (_recFileDict[esType].RecordCount() >= ESDataConfig.GetESMinSendCount(esType))
//                {
//                    SendESData(esType);
//                }
//            });
//        }
//
//        private void SendESData(string esType)
//        {
//            Handler.Post(() =>
//            {
//                if (_eventSender.IsSending())
//                {
//                    return;
//                }
//                
//                _alreadyDelCountDict[esType] = 0;
//                var contentArr = new List<Hashtable>();
//                if (_recFileDict[esType].RecordCount() > 0)
//                {
//                    var oldDataList = _recFileDict[esType].FetchRecord(ESDataConfig.GetESMaxSendCount(esType));
//                    if (oldDataList != null && oldDataList.Count > 0)
//                    {
//                        foreach (var data in oldDataList)
//                        {
//                            if (data != null)
//                            {
//                                contentArr.Add(JsonConvert.DeserializeObject<Hashtable>(data));
//                            }
//                        }
//                    }
//                }
//
//                if (contentArr.Count > 0)
//                {
//                    var contentStr = JsonConvert.SerializeObject(new Hashtable {{"data", contentArr}});
//                    Helper.Log(TAG, esType, "send count:", contentArr.Count, "send data:", contentStr);
//
//                    var postData = new List<KeyValuePair<string, string>>();
//                    postData.Add(new KeyValuePair<string, string>("sig_kv", ESDataConfig.GetESSigKey()));
//                    postData.Add(new KeyValuePair<string, string>("signature",
//                        ESDataHelper.GetESSignature(contentStr, ESDataConfig.GetESHashKey())));
//                    postData.Add(new KeyValuePair<string, string>("content", contentStr));
//                    postData.Add(new KeyValuePair<string, string>("cten", "p"));
//
//                    _totalCount += contentArr.Count;
//                    _eventSender.SendESData(esType, postData, esTypeCallback =>
//                    {
//                        _sendSuccessCount += contentArr.Count;
//                        Helper.Log(TAG, esTypeCallback, "log es ok and del data");
//                        Handler.Post(() =>
//                        {
//                            if (contentArr.Count > _alreadyDelCountDict[esTypeCallback])
//                            {
//                                _recFileDict[esTypeCallback].RemoveRecord(contentArr.Count - _alreadyDelCountDict[esTypeCallback]);
//                            }
//                        });
//                    }, esTypeCallback =>
//                    {
//                        _failCount += contentArr.Count;
//                        Helper.Log(TAG, esTypeCallback, "log es fail and rewrite data to the end of file");
//                        Handler.Post(() =>
//                        {
//                            if (contentArr.Count > _alreadyDelCountDict[esTypeCallback])
//                            {
//                                var oldDataList = _recFileDict[esTypeCallback].FetchRecord(contentArr.Count - _alreadyDelCountDict[esTypeCallback], true);
//                                if (oldDataList.Count > 0)
//                                {
//                                    foreach (var data in oldDataList)
//                                    {
//                                        _recFileDict[esTypeCallback].WriteRecord(data);
//                                    }
//                                }
//                            }
//                        });
//                    });
//                }
//            });
//        }
//
//        public void LogTestResult()
//        {
//            Helper.Log("writeCount", _totalWriteCount);
//            Helper.Log("total", _totalCount);
//            Helper.Log("success", _sendSuccessCount);
//            Helper.Log("fail", _failCount);
//
//            var totalCount = 0;
//            foreach (var item in _recFileDict)
//            {
//                totalCount += item.Value.RecordCount();
//            }
//            Helper.Log("remain", totalCount);
//        }
//    }
//}
//
