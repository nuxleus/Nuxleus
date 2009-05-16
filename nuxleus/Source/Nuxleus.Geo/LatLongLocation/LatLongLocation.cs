using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Nuxleus.Geo
{
    public struct LatLongLocation : ILatLongLocation
    {

        string m_city;
        string m_country;
        string m_countryCode;
        string m_lat;
        string m_long;
        string[] m_locationArray;

        public string City
        {
            get { return m_city; }
            set { m_city = value; }
        }
        public string Country
        {
            get { return m_country; }
            set { m_country = value; }
        }
        public string CountryCode
        {
            get { return m_countryCode; }
            set { m_countryCode = value; }
        }
        public string Lat
        {
            get { return m_lat; }
            set { m_lat = value; }
        }
        public string Long
        {
            get { return m_long; }
            set { m_long = value; }
        }
        public string[] LocationArray
        {
            get { return m_locationArray; }
            set { m_locationArray = value; }
        }
        public LatLongLocation (string[] geoInfo)
        {
            m_city = geoInfo[0];
            m_country = geoInfo[1];
            m_countryCode = geoInfo[2];
            m_lat = geoInfo[3];
            m_long = geoInfo[4];
            m_locationArray = geoInfo;
        }

        public static string ToDelimitedString (string delimiter, LatLongLocation location)
        {
            return String.Join(delimiter, location.LocationArray);
        }
    }
}
