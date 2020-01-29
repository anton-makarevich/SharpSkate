// ==========================================================================
// Copyright (c) 2011-2016, dlg.krakow.pl
// All Rights Reserved
//
// NOTICE: dlg.krakow.pl permits you to use, modify, and distribute this file
// in accordance with the terms of the license agreement accompanying it.
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using Sanet.SmartSkating.Tools.GpxComposer.Models.Gpx;

namespace GpxTools.Models.Gpx
{
    public static class GpxNamespaces
    {
        public const string GPX_NAMESPACE = "http://www.topografix.com/GPX/1/1";
        public const string GARMIN_EXTENSIONS_NAMESPACE = "http://www.garmin.com/xmlschemas/GpxExtensions/v3";
        public const string GARMIN_TRACKPOINT_EXTENSIONS_V1_NAMESPACE = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1";
        public const string GARMIN_TRACKPOINT_EXTENSIONS_V2_NAMESPACE = "http://www.garmin.com/xmlschemas/TrackPointExtension/v2";
        public const string GARMIN_WAYPOINT_EXTENSIONS_NAMESPACE = "http://www.garmin.com/xmlschemas/WaypointExtension/v1";
        public const string DLG_EXTENSIONS_NAMESPACE = "http://dlg.krakow.pl/gpx/extensions/v1";
    }
    
    public class GpxAttributes
    {
        public string Version { get; set; }
        public string Creator { get; set; }
    }

    public class GpxMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GpxPerson Author { get; set; }
        public GpxCopyright Copyright { get; set; }
        public GpxLink Link { get; set; }
        public DateTime? Time { get; set; }
        public string Keywords { get; set; }
        public GpxBounds Bounds { get; set; }
    }

    public class GpxPoint
    {
        private const double EarthRadius = 6371; // [km]
        private const double Radian = Math.PI / 180;

        protected readonly GpxProperties Properties = new GpxProperties();

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
        public DateTime? Time { get; set; }

        public double? MagneticVar
        {
            get => Properties.GetValueProperty<double>("MagneticVar");
            set => Properties.SetValueProperty("MagneticVar", value);
        }

        public double? GeoidHeight
        {
            get => Properties.GetValueProperty<double>("GeoidHeight");
            set => Properties.SetValueProperty("GeoidHeight", value);
        }

        public string Name
        {
            get => Properties.GetObjectProperty<string>("Name");
            set => Properties.SetObjectProperty("Name", value);
        }

        public string Comment
        {
            get => Properties.GetObjectProperty<string>("Comment");
            set => Properties.SetObjectProperty("Comment", value);
        }

        public string Description
        {
            get => Properties.GetObjectProperty<string>("Description");
            set => Properties.SetObjectProperty("Description", value);
        }

        public string Source
        {
            get => Properties.GetObjectProperty<string>("Source");
            set => Properties.SetObjectProperty("Source", value);
        }

        public IList<GpxLink> Links => Properties.GetListProperty<GpxLink>("Links");

        public string Symbol
        {
            get => Properties.GetObjectProperty<string>("Symbol");
            set => Properties.SetObjectProperty("Symbol", value);
        }

        public string Type
        {
            get => Properties.GetObjectProperty<string>("Type");
            set => Properties.SetObjectProperty("Type", value);
        }

        public string FixType
        {
            get => Properties.GetObjectProperty<string>("FixType");
            set => Properties.SetObjectProperty("FixType", value);
        }

        public int? Satelites
        {
            get => Properties.GetValueProperty<int>("Satelites");
            set => Properties.SetValueProperty("Satelites", value);
        }

        public double? Hdop
        {
            get => Properties.GetValueProperty<double>("Hdop");
            set => Properties.SetValueProperty("Hdop", value);
        }

        public double? Vdop
        {
            get => Properties.GetValueProperty<double>("Vdop");
            set => Properties.SetValueProperty("Vdop", value);
        }

        public double? Pdop
        {
            get => Properties.GetValueProperty<double>("Pdop");
            set => Properties.SetValueProperty("Pdop", value);
        }

        public double? AgeOfData
        {
            get => Properties.GetValueProperty<double>("AgeOfData");
            set => Properties.SetValueProperty("AgeOfData", value);
        }

        public int? DgpsId
        {
            get => Properties.GetValueProperty<int>("DgpsId");
            set => Properties.SetValueProperty("DgpsId", value);
        }

        public double GetDistanceFrom(GpxPoint other)
        {
            double thisLatitude = Latitude;
            double otherLatitude = other.Latitude;
            double thisLongitude = Longitude;
            double otherLongitude = other.Longitude;

            double deltaLatitude = Math.Abs(Latitude - other.Latitude);
            double deltaLongitude = Math.Abs(Longitude - other.Longitude);

            thisLatitude *= Radian;
            otherLatitude *= Radian;
            deltaLongitude *= Radian;

            double cos = Math.Cos(deltaLongitude) * Math.Cos(thisLatitude) * Math.Cos(otherLatitude) +
                Math.Sin(thisLatitude) * Math.Sin(otherLatitude);

            return EarthRadius * Math.Acos(cos);
        }
    }
    
    public class GpxWayPoint : GpxPoint
    {
        // GARMIN_EXTENSIONS, GARMIN_WAYPOINT_EXTENSIONS

        public double? Proximity
        {
            get => Properties.GetValueProperty<double>("Proximity");
            set => Properties.SetValueProperty<double>("Proximity", value);
        }

        public double? Temperature
        {
            get => Properties.GetValueProperty<double>("Temperature");
            set => Properties.SetValueProperty<double>("Temperature", value);
        }

        public double? Depth
        {
            get => Properties.GetValueProperty<double>("Depth");
            set => Properties.SetValueProperty<double>("Depth", value);
        }

        public string DisplayMode
        {
            get => Properties.GetObjectProperty<string>("DisplayMode");
            set => Properties.SetObjectProperty<string>("DisplayMode", value);
        }

        public IList<string> Categories => Properties.GetListProperty<string>("Categories");

        public GpxAddress Address
        {
            get => Properties.GetObjectProperty<GpxAddress>("Address");
            set => Properties.SetObjectProperty<GpxAddress>("Address", value);
        }

        public IList<GpxPhone> Phones => Properties.GetListProperty<GpxPhone>("Phones");

        // GARMIN_WAYPOINT_EXTENSIONS

        public int? Samples
        {
            get => Properties.GetValueProperty<int>("Samples");
            set => Properties.SetValueProperty<int>("Samples", value);
        }

        public DateTime? Expiration
        {
            get => Properties.GetValueProperty<DateTime>("Expiration");
            set => Properties.SetValueProperty<DateTime>("Expiration", value);
        }

        // DLG_EXTENSIONS

        public int? Level
        {
            get => Properties.GetValueProperty<int>("Level");
            set => Properties.SetValueProperty<int>("Level", value);
        }

        public IList<string> Aliases => Properties.GetListProperty<string>("Aliases");

        public bool HasGarminExtensions =>
            Proximity != null || Temperature != null || Depth != null ||
            DisplayMode != null || Address != null ||
            Categories.Count != 0 || Phones.Count != 0;

        public bool HasGarminWaypointExtensions => Samples != null || Expiration != null;

        public bool HasDlgExtensions => Level != null || Aliases.Count != 0;

        public bool HasExtensions => HasGarminExtensions || HasGarminWaypointExtensions || HasDlgExtensions;
    }

    public class GpxTrackPoint : GpxPoint
    {
        // GARMIN_EXTENSIONS, GARMIN_TRACKPOINT_EXTENSIONS_V1, GARMIN_TRACKPOINT_EXTENSIONS_V2

        public double? Temperature
        {
            get => Properties.GetValueProperty<double>("Temperature");
            set => Properties.SetValueProperty("Temperature", value);
        }

        public double? Depth
        {
            get => Properties.GetValueProperty<double>("Depth");
            set => Properties.SetValueProperty("Depth", value);
        }

        // GARMIN_TRACKPOINT_EXTENSIONS_V1, GARMIN_TRACKPOINT_EXTENSIONS_V2

        public double? WaterTemperature
        {
            get => Properties.GetValueProperty<double>("WaterTemperature");
            set => Properties.SetValueProperty("WaterTemperature", value);
        }

        public int? HeartRate
        {
            get => Properties.GetValueProperty<int>("HeartRate");
            set => Properties.SetValueProperty("HeartRate", value);
        }

        public int? Cadence
        {
            get => Properties.GetValueProperty<int>("Cadence");
            set => Properties.SetValueProperty("Cadence", value);
        }

        // GARMIN_TRACKPOINT_EXTENSIONS_V2

        public double? Speed
        {
            get => Properties.GetValueProperty<double>("Speed");
            set => Properties.SetValueProperty("Speed", value);
        }

        public double? Course
        {
            get => Properties.GetValueProperty<double>("Course");
            set => Properties.SetValueProperty("Course", value);
        }

        public double? Bearing
        {
            get => Properties.GetValueProperty<double>("Bearing");
            set => Properties.SetValueProperty("Bearing", value);
        }

        public bool HasGarminExtensions => Temperature != null || Depth != null;

        public bool HasGarminTrackpointExtensionsV1 => WaterTemperature != null || HeartRate != null || Cadence != null;

        public bool HasGarminTrackpointExtensionsV2 => Speed != null || Course != null || Bearing != null;

        public bool HasExtensions => HasGarminExtensions || HasGarminTrackpointExtensionsV1 || HasGarminTrackpointExtensionsV2;
    }

    public class GpxRoutePoint : GpxPoint
    {
        // GARMIN_EXTENSIONS

        public IList<GpxPoint> RoutePoints => Properties.GetListProperty<GpxPoint>("RoutePoints");

        public bool HasExtensions => RoutePoints.Count != 0;
    }

    public class GpxPointCollection<T> : IList<T> where T : GpxPoint
    {
        private readonly List<T> _points = new List<T>();

        public GpxPoint AddPoint(T point)
        {
            _points.Add(point);
            return point;
        }

        public T StartPoint => (_points.Count == 0) ? null : _points[0];

        public T EndPoint => (_points.Count == 0) ? null : _points[_points.Count - 1];

        public double GetLength()
        {
            double result = 0;

            for (int i = 1; i < _points.Count; i++)
            {
                double dist = _points[i].GetDistanceFrom(_points[i - 1]);
                result += dist;
            }

            return result;
        }

        public GpxPointCollection<GpxPoint> ToGpxPoints()
        {
            GpxPointCollection<GpxPoint> points = new GpxPointCollection<GpxPoint>();

            foreach (T gpxPoint in _points)
            {
                GpxPoint point = new GpxPoint
                {
                    Longitude = gpxPoint.Longitude,
                    Latitude = gpxPoint.Latitude,
                    Elevation = gpxPoint.Elevation,
                    Time = gpxPoint.Time
                };

                points.Add(point);
            }

            return points;
        }

        public int Count => _points.Count;

        public int IndexOf(T item)
        {
            return _points.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _points.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _points.RemoveAt(index);
        }

        public T this[int index]
        {
            get => _points[index];
            set => _points[index] = value;
        }

        public void Add(T item)
        {
            _points.Add(item);
        }

        public void Clear()
        {
            _points.Clear();
        }

        public bool Contains(T item)
        {
            return _points.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _points.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly => false;

        public bool Remove(T item)
        {
            return _points.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class GpxTrackOrRoute
    {
        private readonly List<GpxLink> _links = new List<GpxLink>(0);

        public string Name { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public int? Number { get; set; }
        public string Type { get; set; }

        public IList<GpxLink> Links => _links;

        // GARMIN_EXTENSIONS

        public GpxColor? DisplayColor { get; set; }

        public bool HasExtensions => DisplayColor != null;

        public abstract GpxPointCollection<GpxPoint> ToGpxPoints();
    }

    public class GpxRoute : GpxTrackOrRoute
    {
        private readonly GpxPointCollection<GpxRoutePoint> _routePoints = new GpxPointCollection<GpxRoutePoint>();

        public GpxPointCollection<GpxRoutePoint> RoutePoints => _routePoints;

        public override GpxPointCollection<GpxPoint> ToGpxPoints()
        {
            GpxPointCollection<GpxPoint> points = new GpxPointCollection<GpxPoint>();

            foreach (GpxRoutePoint routePoint in _routePoints)
            {
                points.Add(routePoint);

                foreach (GpxPoint gpxPoint in routePoint.RoutePoints)
                {
                    points.Add(gpxPoint);
                }
            }

            return points;
        }
    }

    public class GpxTrack : GpxTrackOrRoute
    {
        private readonly List<GpxTrackSegment> _segments = new List<GpxTrackSegment>(1);

        public IList<GpxTrackSegment> Segments => _segments;

        public override GpxPointCollection<GpxPoint> ToGpxPoints()
        {
            GpxPointCollection<GpxPoint> points = new GpxPointCollection<GpxPoint>();

            foreach (GpxTrackSegment segment in _segments)
            {
                GpxPointCollection<GpxPoint> segmentPoints = segment.TrackPoints.ToGpxPoints();

                foreach (GpxPoint point in segmentPoints)
                {
                    points.Add(point);
                }
            }

            return points;
        }
    }

    public class GpxTrackSegment
    {
        readonly GpxPointCollection<GpxTrackPoint> _trackPoints = new GpxPointCollection<GpxTrackPoint>();

        public GpxPointCollection<GpxTrackPoint> TrackPoints => _trackPoints;
    }

    public class GpxLink
    {
        public string Href { get; set; }
        public string Text { get; set; }
        public string MimeType { get; set; }

        public Uri Uri
        {
            get
            {
                Uri result;
                return Uri.TryCreate(Href, UriKind.Absolute, out result) ? result : null;
            }
        }
    }

    public class GpxEmail
    {
        public string Id { get; set; }
        public string Domain { get; set; }
    }

    public class GpxAddress
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }

    public class GpxPhone
    {
        public string Number { get; set; }
        public string Category { get; set; }
    }

    public class GpxPerson
    {
        public string Name { get; set; }
        public GpxEmail Email { get; set; }
        public GpxLink Link { get; set; }
    }

    public class GpxCopyright
    {
        public string Author { get; set; }
        public int? Year { get; set; }
        public string Licence { get; set; }
    }

    public class GpxBounds
    {
        public double MinLatitude { get; set; }
        public double MinLongitude { get; set; }
        public double MaxLatitude { get; set; }
        public double MaxLongitude { get; set; }
    }

    public enum GpxColor : uint
    {
        Black = 0xff000000,
        DarkRed = 0xff8b0000,
        DarkGreen = 0xff008b00,
        DarkYellow = 0x8b8b0000,
        DarkBlue = 0Xff00008b,
        DarkMagenta = 0xff8b008b,
        DarkCyan = 0xff008b8b,
        LightGray = 0xffd3d3d3,
        DarkGray = 0xffa9a9a9,
        Red = 0xffff0000,
        Green = 0xff00b000,
        Yellow = 0xffffff00,
        Blue = 0xff0000ff,
        Magenta = 0xffff00ff,
        Cyan = 0xff00ffff,
        White = 0xffffffff,
        Transparent = 0x00ffffff
    }
    
    
}