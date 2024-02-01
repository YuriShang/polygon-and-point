using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TestApp
{
    public interface IDialogService
    {
        void ShowMessage(string message);   // показ сообщения
        string FilePath { get; set; }   // путь к выбранному файлу
        bool OpenFileDialog();  // открытие файла
    }
    public class DefaultDialogService : IDialogService
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
    public interface IFileService
    {
        List<PointModel> Open(string filename);
    }
    public class CsvFileService : IFileService
    {

        public List<PointModel> Open(string filename)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8, // Our file uses UTF-8 encoding.
                Delimiter = "," // The delimiter is a comma.
            };

            List<PointModel> points = new List<PointModel>();
            
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                using (var t = new StreamReader(fs))
                using (var csv = new CsvReader(t, configuration))
                {
                    var data = csv.GetRecords<PointModel>();
                    points.AddRange(data);
                };
            }
            return points;
        }
    }
}
