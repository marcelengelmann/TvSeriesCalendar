using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using TvSeriesCalendar.Models;

namespace TvSeriesCalendar.Services
{
    public class SeriesLocalDataService
    {
        private ObservableCollection<TvSeries> _tvSeries;

        public SeriesLocalDataService()
        {
            _tvSeries = new ObservableCollection<TvSeries>();
            string filePath = Directory.GetCurrentDirectory() + "\\savedData\\series.json";
            if (File.Exists(filePath) == false) return;
            //download missing images
            string content = File.ReadAllText(filePath);
            _tvSeries = JsonConvert.DeserializeObject<ObservableCollection<TvSeries>>(content);
        }

        public ObservableCollection<TvSeries> GetTvSeries()
        {
            return _tvSeries;
        }

        public void Save(ObservableCollection<TvSeries> tvSeries)
        {
            //todo: save to file
            _tvSeries = tvSeries;
            string json = JsonConvert.SerializeObject(_tvSeries, Formatting.Indented);
            string filePath = Directory.GetCurrentDirectory() + "\\savedData\\series.json";
            File.WriteAllText(filePath, json);
        }
    }
}