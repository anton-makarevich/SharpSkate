// ==========================================================================
// Copyright (c) 2011-2016, dlg.krakow.pl
// All Rights Reserved
//
// NOTICE: dlg.krakow.pl permits you to use, modify, and distribute this file
// in accordance with the terms of the license agreement accompanying it.
// ==========================================================================

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using GpxTools.Models.Gpx;

namespace Sanet.SmartSkating.Tools.GpxComposer.Models.Gpx
{
    public class GpxWriter : IDisposable
    {
        private const string GpxVersion = "1.1";
        private const string GpxCreator = "http://dlg.krakow.pl/gpx";
        private const string GarminExtensionsPrefix = "gpxx";
        private const string GarminWaypointExtensionsPrefix = "gpxwpx";
        private const string GarminTrackpointExtensionsV2Prefix = "gpxtpx";
        private const string DlgExtensionsPrefix = "dlg";

        private readonly XmlWriter _writer;

        public GpxWriter(Stream stream)
        {
            _writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = true, Indent = true });
            _writer.WriteStartDocument(false);
            _writer.WriteStartElement("gpx", GpxNamespaces.GPX_NAMESPACE);
            _writer.WriteAttributeString("version", GpxVersion);
            _writer.WriteAttributeString("creator", GpxCreator);
            _writer.WriteAttributeString("xmlns", GarminExtensionsPrefix, null, GpxNamespaces.GARMIN_EXTENSIONS_NAMESPACE);
            _writer.WriteAttributeString("xmlns", GarminWaypointExtensionsPrefix, null, GpxNamespaces.GARMIN_WAYPOINT_EXTENSIONS_NAMESPACE);
            _writer.WriteAttributeString("xmlns", GarminTrackpointExtensionsV2Prefix, null, GpxNamespaces.GARMIN_TRACKPOINT_EXTENSIONS_V2_NAMESPACE);
            _writer.WriteAttributeString("xmlns", DlgExtensionsPrefix, null, GpxNamespaces.DLG_EXTENSIONS_NAMESPACE);
        }

        public void Dispose()
        {
            _writer.WriteEndElement();
            _writer.Close();
        }

        public void WriteMetadata(GpxMetadata metadata)
        {
            _writer.WriteStartElement("metadata");

            if (metadata.Name != null) _writer.WriteElementString("name", metadata.Name);
            if (metadata.Description != null) _writer.WriteElementString("desc", metadata.Description);
            if (metadata.Author != null) WritePerson("author", metadata.Author);
            if (metadata.Copyright != null) WriteCopyright("copyright", metadata.Copyright);
            if (metadata.Link != null) WriteLink("link", metadata.Link);
            if (metadata.Time != null) _writer.WriteElementString("time", ToGpxDateString(metadata.Time.Value));
            if (metadata.Keywords != null) _writer.WriteElementString("keywords", metadata.Keywords);
            if (metadata.Bounds != null) WriteBounds("bounds", metadata.Bounds);

            _writer.WriteEndElement();
        }

        public void WriteRoute(GpxRoute route)
        {
            _writer.WriteStartElement("rte");
            WriteTrackOrRoute(route);

            foreach (GpxRoutePoint routePoint in route.RoutePoints)
            {
                WriteRoutePoint("rtept", routePoint);
            }

            _writer.WriteEndElement();
        }

        private void WriteTrackOrRoute(GpxTrackOrRoute trackOrRoute)
        {
            if (trackOrRoute.Name != null) _writer.WriteElementString("name", trackOrRoute.Name);
            if (trackOrRoute.Comment != null) _writer.WriteElementString("cmt", trackOrRoute.Comment);
            if (trackOrRoute.Description != null) _writer.WriteElementString("desc", trackOrRoute.Description);
            if (trackOrRoute.Source != null) _writer.WriteElementString("src", trackOrRoute.Source);

            foreach (GpxLink link in trackOrRoute.Links)
            {
                WriteLink("link", link);
            }

            if (trackOrRoute.Number != null) _writer.WriteElementString("number", trackOrRoute.Number.Value.ToString(CultureInfo.InvariantCulture));
            if (trackOrRoute.Type != null) _writer.WriteElementString("type", trackOrRoute.Type);

            if (trackOrRoute.HasExtensions)
            {
                _writer.WriteStartElement("extensions");
                _writer.WriteStartElement(trackOrRoute is GpxTrack ? "TrackExtension" : "RouteExtension", GpxNamespaces.GARMIN_EXTENSIONS_NAMESPACE);

                _writer.WriteElementString("DisplayColor", GpxNamespaces.GARMIN_EXTENSIONS_NAMESPACE, trackOrRoute.DisplayColor.ToString());

                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }
        }

        private void WriteRoutePoint(string elementName, GpxRoutePoint routePoint)
        {
            _writer.WriteStartElement(elementName);
            WritePoint(routePoint);

            if (routePoint.HasExtensions)
            {
                _writer.WriteStartElement("extensions");
                _writer.WriteStartElement("RoutePointExtension", GpxNamespaces.GARMIN_EXTENSIONS_NAMESPACE);

                foreach (GpxPoint point in routePoint.RoutePoints)
                {
                    WriteSubPoint(point);
                }

                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        private void WritePoint(GpxPoint point)
        {
            _writer.WriteAttributeString("lat", point.Latitude.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("lon", point.Longitude.ToString(CultureInfo.InvariantCulture));
            if (point.Elevation != null) _writer.WriteElementString("ele", point.Elevation.Value.ToString(CultureInfo.InvariantCulture));
            if (point.Time != null) _writer.WriteElementString("time", ToGpxDateString(point.Time.Value));
            if (point.MagneticVar != null) _writer.WriteElementString("magvar", point.MagneticVar.Value.ToString(CultureInfo.InvariantCulture));
            if (point.GeoidHeight != null) _writer.WriteElementString("geoidheight", point.GeoidHeight.Value.ToString(CultureInfo.InvariantCulture));
            if (point.Name != null) _writer.WriteElementString("name", point.Name);
            if (point.Comment != null) _writer.WriteElementString("cmt", point.Comment);
            if (point.Description != null) _writer.WriteElementString("desc", point.Description);
            if (point.Source != null) _writer.WriteElementString("src", point.Source);

            foreach (GpxLink link in point.Links)
            {
                WriteLink("link", link);
            }

            if (point.Symbol != null) _writer.WriteElementString("sym", point.Symbol);
            if (point.Type != null) _writer.WriteElementString("type", point.Type);
            if (point.FixType != null) _writer.WriteElementString("fix", point.FixType);
            if (point.Satelites != null) _writer.WriteElementString("sat", point.Satelites.Value.ToString(CultureInfo.InvariantCulture));
            if (point.Hdop != null) _writer.WriteElementString("hdop", point.Hdop.Value.ToString(CultureInfo.InvariantCulture));
            if (point.Vdop != null) _writer.WriteElementString("vdop", point.Vdop.Value.ToString(CultureInfo.InvariantCulture));
            if (point.Pdop != null) _writer.WriteElementString("pdop", point.Pdop.Value.ToString(CultureInfo.InvariantCulture));
            if (point.AgeOfData != null) _writer.WriteElementString("ageofdgpsdata", point.AgeOfData.Value.ToString(CultureInfo.InvariantCulture));
            if (point.DgpsId != null) _writer.WriteElementString("dgpsid", point.DgpsId.Value.ToString(CultureInfo.InvariantCulture));
        }

        private void WriteSubPoint(GpxPoint point)
        {
            _writer.WriteStartElement("rpt", GpxNamespaces.GARMIN_EXTENSIONS_NAMESPACE);

            _writer.WriteAttributeString("lat", point.Latitude.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("lon", point.Longitude.ToString(CultureInfo.InvariantCulture));

            _writer.WriteEndElement();
        }

        private void WritePerson(string elementName, GpxPerson person)
        {
            _writer.WriteStartElement(elementName);

            if (person.Name != null) _writer.WriteElementString("name", person.Name);
            if (person.Email != null) WriteEmail("email", person.Email);

            _writer.WriteEndElement();
        }

        private void WriteEmail(string elementName, GpxEmail email)
        {
            _writer.WriteStartElement(elementName);
            _writer.WriteAttributeString("id", email.Id);
            _writer.WriteAttributeString("domain", email.Domain);
            _writer.WriteEndElement();
        }

        private void WriteLink(string elementName, GpxLink link)
        {
            _writer.WriteStartElement(elementName);
            _writer.WriteAttributeString("href", link.Href);
            if (link.Text != null) _writer.WriteElementString("text", link.Text);
            if (link.MimeType != null) _writer.WriteElementString("type", link.MimeType);
            _writer.WriteEndElement();
        }

        private void WriteCopyright(string elementName, GpxCopyright copyright)
        {
            _writer.WriteStartElement(elementName);
            _writer.WriteAttributeString("author", copyright.Author);
            if (copyright.Year != null) _writer.WriteElementString("year", copyright.Year.Value.ToString());
            if (copyright.Licence != null) _writer.WriteElementString("licence", copyright.Licence);
            _writer.WriteEndElement();
        }

        private void WriteBounds(string elementName, GpxBounds bounds)
        {
            _writer.WriteStartElement(elementName);
            _writer.WriteAttributeString("minlat", bounds.MinLatitude.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("minlon", bounds.MinLongitude.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("maxlat", bounds.MaxLatitude.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("maxlon", bounds.MaxLongitude.ToString(CultureInfo.InvariantCulture));
            _writer.WriteEndElement();
        }

        private static string ToGpxDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH':'mm':'ss.FFFZ");
            //return string.Format("{0:D4}-{1:D2}-{2:D2}T{3:D2}:{4:D2}:{5:D2}", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }
    }
}