using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TvSeriesCalendar.Models;

namespace TvSeriesCalendar.Services
{
    public class LocalDataService
    {
        private ObservableCollection<TvSeries> _tvSeries;

        public ObservableCollection<TvSeries> GetTvSeries()
        {
            return _tvSeries;
        }

        public void Save(ObservableCollection<TvSeries> tvSeries)
        {
            //todo: save to file
            _tvSeries = tvSeries;
            string json = JsonConvert.SerializeObject(_tvSeries, Formatting.Indented);
            string FilePath = Directory.GetCurrentDirectory() + "\\savedData\\series.json";
            File.WriteAllText(FilePath, json);
        }

        public LocalDataService()
        {
            _tvSeries = new ObservableCollection<TvSeries>();
            string FilePath = Directory.GetCurrentDirectory() + "\\savedData\\series.json";
            if (File.Exists(FilePath))
            {
                //download missing images
                string content = File.ReadAllText(FilePath);
                _tvSeries = JsonConvert.DeserializeObject<ObservableCollection<TvSeries>>(content);
            }
        }
    }
}