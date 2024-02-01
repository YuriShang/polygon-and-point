using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Media;

namespace TestApp
{
    public class PolygonModel : OnPropertyChangedClass
    {
        private string model;
        private PointCollection polygonPoints;
        private ObservableCollection<Point> points;
        private Polygon polygon;
        private double height;
        private double width;
        private double minX;
        private double maxX;
        private double minY;
        private double maxY;
        private double scale;
        private double shiftX;
        private double shiftY;

        public PolygonModel(ObservableCollection<Point> p) 
        {
            minX = minY = double.MaxValue;
            maxX = maxY = double.MinValue;
            for (int i = 0; i < p.Count; i++)
            {
                minX = Math.Min(minX, p[i].X);
                maxX = Math.Max(maxX, p[i].X);
                minY = Math.Min(minY, p[i].Y);
                maxY = Math.Max(maxY, p[i].Y);
            }
            width = maxX - minX;
            height = maxY - minY;
            scale = Math.Min(450 / width, 450 / height) / 2.3; // коэффициент масштабирования 
            polygonPoints = new PointCollection();
            double k = width * scale;
            shiftX = width * scale / 2;
            shiftY = height * scale / 2;
            scalePoints(p);
            Points = p;
            polygon = new Polygon
            {
                Stroke = Brushes.Black,
                Fill = Brushes.LightBlue,
                StrokeThickness = 1.5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Points = polygonPoints,
                Height = height * scale+1.1,
                Width = width * scale+1.1,
            };
        }
        public string Model
        {
            get { return model; }
            set
            {
                model = value;
                OnPropertyChanged("Model");
            }
        }
        public ObservableCollection<Point> Points
        {
            get { return points; }
            set
            {
                points = value;
                OnPropertyChanged("Points");
            }
        }
        public Polygon Polygon_
        {
            get { return polygon; }
            set
            {
                polygon = value;
                OnPropertyChanged("Polygon");
            }
        }
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged("Width");
            }
        }
        public double MinX
        {
            get { return minX; }
            set
            {
                minX = value;
                OnPropertyChanged("MinX");
            }
        }
        public double MaxX
        {
            get { return maxX; }
            set
            {
                maxX = value;
                OnPropertyChanged("MaxX");
            }
        }
        public double MinY
        {
            get { return minY; }
            set
            {
                minY = value;
                OnPropertyChanged("MinY");
            }
        }
        public double MaxY
        {
            get { return maxY; }
            set
            {
                maxY = value;
                OnPropertyChanged("MaxY");
            }
        }
        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                OnPropertyChanged("Scale");
            }
        }
        public double ShiftX
        {
            get { return shiftX; }
            set
            {
                shiftX = value;
                OnPropertyChanged("ShiftX");
            }
        }
        public double ShiftY
        {
            get { return shiftY; }
            set
            {
                shiftY = value;
                OnPropertyChanged("ShiftY");
            }
        }
        public enum PointInPolygon { INSIDE, OUTSIDE, BOUNDARY } //положение точки в многоугольнике

        private enum EdgeType { TOUCHING, CROSSING, INESSENTIAL } //положение ребра

        private enum PointOverEdge { LEFT, RIGHT, BETWEEN, OUTSIDE } //положение точки относительно отрезка
        public void scalePoints(ObservableCollection<Point> points_)
        {
            for (int i = 0; i < points_.Count; i++)
            {
                double x = points_[i].X * scale + ShiftX;
                double y = -points_[i].Y * scale + ShiftY;
                Point p = new Point(x, y);
                polygonPoints.Add(p);
            }
            //path.Reset();
            //points = points.Append(points[0]).ToArray();
            //float lastX = (points[0].X) * scale + translateX;
            //float lastY = (minY - points[0].Y) * scale + translateY + minY * scale * -1; // направление оси Y инвертировано

            //for (int i = 0; i < points.Length; i++)
            //{
            //    float x = (points[i].X) * scale + translateX;
            //    float y = (minY - points[i].Y) * scale + translateY + minY * scale * -1; // направление оси Y инвертировано

            //    path.AddLine(lastX, lastY, x, y);

            //    lastX = x;
            //    lastY = y;
            //}
            //g.DrawPath(p, path);
            //g.FillPath(brush, path);
        }
        public bool Touch(Point a) //проверка попадания точки в Polygon
        {
            PointInPolygon touch = pointInPolygon(a);
            return touch == PointInPolygon.INSIDE || touch == PointInPolygon.BOUNDARY;
        }
        private PointOverEdge classify(Point p, Point v, Point w) //положение точки p относительно отрезка vw
        {
            //коэффициенты уравнения прямой
            double a = v.Y - w.Y;
            double b = w.X - v.X;
            double c = v.X * w.Y - w.X * v.Y;

            //подставим точку в уравнение прямой
            double f = a * p.X + b * p.Y + c;
            if (f > 0)
                return PointOverEdge.RIGHT; //точка лежит справа от отрезка
            if (f < 0)
                return PointOverEdge.LEFT; //слева от отрезка

            double minX = Math.Min(v.X, w.X);
            double maxX = Math.Max(v.X, w.X);
            double minY = Math.Min(v.Y, w.Y);
            double maxY = Math.Max(v.Y, w.Y);

            if (minX <= p.X && p.X <= maxX && minY <= p.Y && p.Y <= maxY)
                return PointOverEdge.BETWEEN; //точка лежит на отрезке
            return PointOverEdge.OUTSIDE; //точка лежит на прямой, но не на отрезке
        }

        private EdgeType edgeType(Point a, Point v, Point w) //тип ребра vw для точки a
        {
            switch (classify(a, v, w))
            {
                case PointOverEdge.LEFT:
                    return ((v.Y < a.Y) && (a.Y <= w.Y)) ? EdgeType.CROSSING : EdgeType.INESSENTIAL;
                case PointOverEdge.RIGHT:
                    return ((w.Y < a.Y) && (a.Y <= v.Y)) ? EdgeType.CROSSING : EdgeType.INESSENTIAL;
                case PointOverEdge.BETWEEN:
                    return EdgeType.TOUCHING;
                default:
                    return EdgeType.INESSENTIAL;
            }
        }

        public PointInPolygon pointInPolygon(Point a) //положение точки в многоугольнике
        {
            bool parity = true;
            ObservableCollection<Point> points = Points;
            for (int i = 0; i < points.Count; i++)
            {
                Point v = points[i];
                Point w = points[(i + 1) % points.Count];

                switch (edgeType(a, v, w))
                {
                    case EdgeType.TOUCHING:
                        return PointInPolygon.BOUNDARY;
                    case EdgeType.CROSSING:
                        parity = !parity;
                        break;
                }
            }
            return parity ? PointInPolygon.OUTSIDE : PointInPolygon.INSIDE;
        }
    }
}
