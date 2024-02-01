using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestApp
{
    class PolygonViewModel : OnPropertyChangedClass
    {
        private PolygonModel polygon;
        private Polygon p;
        private double polygonWidth;
        private double polygonHeight;
        private Thickness shift = new Thickness(0,0,0,0);
        private Ellipse ellipse;
        private RelayCommand pointAddCommand;
        private RelayCommand pointsClearCommand;
        private RelayCommand calculateCommand;
        private RelayCommand csvOpenCommand;
        private ObservableCollection<Point> points;
        private double x;
        private double y;
        private Point point;
        private double pointX;
        private double pointY;
        private string pointPosition = "не определен";
        IFileService fileService;
        IDialogService dialogService;
        public PolygonViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;
            points = new ObservableCollection<Point>();
        }
        public RelayCommand CsvOpenCommand
        {
            get
            {
                return csvOpenCommand ??
                  (csvOpenCommand = new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.OpenFileDialog() == true)
                          {
                              Points.Clear();
                              List<PointModel> points = fileService.Open(dialogService.FilePath);
                              foreach (var p in points)
                              {
                                  Point point = new Point(p.x, p.y);
                                  Points.Add(point);
                              }
                              dialogService.ShowMessage("Файл открыт");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }
        public RelayCommand AddCommand
        {
            get
            {
                return pointAddCommand??
                  (pointAddCommand = new RelayCommand(AddPoint, CanAdd));
            }
        }
        private bool CanAdd(object parameter)
        {
            bool r = parameter is bool error && !error;
            return true; // исправить
        }
        private void AddPoint(object parametr)
        {
            Point point = new Point(X, Y);
            Points.Add(point);
        }
        public RelayCommand ClearCommand
        {
            get
            {
                return pointsClearCommand ??
                  (pointsClearCommand = new RelayCommand(obj =>
                  {
                      Points.Clear();
                  }));
            }
        }
        public RelayCommand CalculateCommand
        {
            get
            {
                return calculateCommand ??
                  (calculateCommand = new RelayCommand(obj =>
                  {
                      polygon = new PolygonModel(points);
                      Polygon_ = polygon.Polygon_;
                      Point_= new Point(pointX, pointY);
                      if (points.Count > 0)
                      {
                          PolygonWidth = polygon.Width * polygon.Scale;
                          PolygonHeight = polygon.Height * polygon.Scale;
                          bool pointPosition = polygon.Touch(Point_); // определяем положение точки
                          DrawPoint(PointX, PointY);
                          PointPosition = pointPosition ? "точка внутри" : "точка снаружи";
                      }
                      else PointPosition = "не определен";
                  }));
            }
        }
        public double X
        {
            get { return x; }
            set 
            { 
                x = value;
                OnPropertyChanged("X");
            }
        }
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("X");
            }
        }
        public Polygon Polygon_
        {
            get { return p; }
            set
            {
                p = value;
                OnPropertyChanged("Polygon_");
            }
        }
        public double PolygonWidth
        {
            get { return polygonWidth; }
            set
            {
                polygonWidth = value;
                OnPropertyChanged("PolygonWidth");
            }
        }
        public double PolygonHeight
        {
            get { return polygonHeight; }
            set
            {
                polygonHeight = value;
                OnPropertyChanged("PolygonHeight");
            }
        }
        public Thickness Shift 
        {
            get { return shift; }
            set
            {
                shift = value;
                OnPropertyChanged("Shift");
            }
        }
        public Ellipse Ellipse_
        {
            get { return ellipse; }
            set
            {
                ellipse = value;
                OnPropertyChanged("Ellipse_");
            }
        }
        public ObservableCollection<Point> Points
        {
            get { return points; }
        }
        public double PointX
        {
            get { return pointX; }
            set
            {
                pointX = value;
                OnPropertyChanged("PointX");
            }
        }
        public double PointY
        {
            get { return pointY; }
            set
            {
                pointY = value;
                OnPropertyChanged("PointPosition");
            }
        }
        public string PointPosition
        {
            get { return pointPosition; }
            set
            {
                pointPosition = value;
                OnPropertyChanged("PointPosition");
            }
        }
        public Point Point_
        {
            get { return point; }
            set
            {
                point = value;
                OnPropertyChanged("Point_");
            }
        }
        public void DrawPoint(double x, double y)
        {
            ellipse = new Ellipse();
            ellipse.RenderTransform = new TranslateTransform { X = x*polygon.Scale, Y = y*-polygon.Scale };
            ellipse.Stroke = Brushes.Black;
            ellipse.Width = 5;
            ellipse.Height = 5;
            ellipse.Fill = Brushes.Red;
            Ellipse_ = ellipse;
        }
    }
}
