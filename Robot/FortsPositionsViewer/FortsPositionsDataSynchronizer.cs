using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using FileHelpers;

namespace FortsPositionsViewer
{
    public class FortsPositionsDataSynchronizer
    {
        public Task Synchronize(DeriativeNames deriativeName)
        {
            return Task.Run(() => SynchronizeData(deriativeName));
        }

        private void SynchronizeData(DeriativeNames deriativeName)
        {
            var dataProvider = new FortsPositionsDataProvider(deriativeName);

            var data = dataProvider.Read();

            var syncStartDate =  GetSyncStartDate(data);

            for (var date = syncStartDate; date < DateTime.Now.Date; date = date.AddDays(1))
            {
                var rawDataFilePath = Config.GetFortsOpenPositionsRawDataFilePath(date);

                if (File.Exists(rawDataFilePath) == false)
                {
                    LoadDataFromServerIntoFile(date, rawDataFilePath);
                }

                AddRecordsFromFileIntoData(rawDataFilePath, deriativeName, data);
            }       
            
            dataProvider.Write(data);
        }

        private void AddRecordsFromFileIntoData(string filePath, DeriativeNames deriativeName,  OpenInterestData data)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var date = DateTime.ParseExact(fileName, "yy-MM-dd", null);

            var engine = new FileHelperEngine<RawOpenPositionsDataItem>();
            var records = engine.ReadFile(filePath);

            if (records.Any() == false) return; //weekends days contains only header

            var deriativeRecords = 
                records.Where(x => x.ShortName.ToLower() == deriativeName.ToString().ToLower() && x.ContractType == "F").ToList();

            var individualsRecord = deriativeRecords.First(x => x.IsIndividuals);
            var legalsRecord = deriativeRecords.First(x => x.IsIndividuals == false);

            data.Individuals.Add(date, new OpenInterestDataItem { ClientsInLong = individualsRecord.NumberOfClientsInLong,
                                                                  ClientsInShort = individualsRecord.NumberOfClientsInShort,
                                                                  LongPosition = individualsRecord.LongPosition,
                                                                  ShortPosition = individualsRecord.ShortPosition});

            data.Legals.Add(date, new OpenInterestDataItem
            {
                ClientsInLong = legalsRecord.NumberOfClientsInLong,
                ClientsInShort = legalsRecord.NumberOfClientsInShort,
                LongPosition = legalsRecord.LongPosition,
                ShortPosition = legalsRecord.ShortPosition
            });
        }

        private static DateTime GetSyncStartDate(OpenInterestData data)
        {
            if (data.Individuals.Keys.Any())
            {
                return data.Individuals.Keys.OrderByDescending(x => x).First().AddDays(1);
            }
            else
            {
                return Config.GetSyncDefaultStartDate();
            }
        }

        void LoadDataFromServerIntoFile(DateTime date, string filePath)
        {
            try
            {
                var request = WebRequest.Create(Config.GetFortsOpenPositionsUrl(date));
                var responseStream = request.GetResponse().GetResponseStream();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        string responseText = reader.ReadToEnd();

                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            using (StreamWriter writer = new StreamWriter(fileStream))
                            {
                                writer.WriteLine(responseText);
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Analize(DeriativeNames deriativeName)
        {
            

        }
    }
}