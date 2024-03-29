/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.concurrent.ConcurrentHashMap;
using java.util.Map;

using SharpEarth.util.xml;
using SharpEarth.ogc.kml;
namespace SharpEarth.ogc.kml.gx{



/**
 * @author tag
 * @version $Id: GXParserContext.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class GXParserContext : BasicXMLEventParserContext
{
    protected static final String[] StringFields = new String[]
        {
            "altitudeMode",
            "description",
            "flyToMode",
            "playMode",
        };

    protected static final String[] DoubleFields = new String[]
        {
            "duration",
        };

    protected static final String[] BooleanFields = new String[]
        {
            "balloonVisibility",
        };

    public static Map<QName, XMLEventParser> getDefaultParsers()
    {
        ConcurrentHashMap<QName, XMLEventParser> parsers = new ConcurrentHashMap<QName, XMLEventParser>();

        String ns = GXConstants.GX_NAMESPACE;
        parsers.put(new QName(ns, "AnimatedUpdate"), new GXAnimatedUpdate(ns));
        parsers.put(new QName(ns, "FlyTo"), new GXFlyTo(ns));
        parsers.put(new QName(ns, "LatLonQuad"), new GXLatLongQuad(ns));
        parsers.put(new QName(ns, "Playlist"), new GXPlaylist(ns));
        parsers.put(new QName(ns, "SoundCue"), new GXSoundCue(ns));
        parsers.put(new QName(ns, "TimeSpan"), new KMLTimeSpan(ns));
        parsers.put(new QName(ns, "TimeStamp"), new KMLTimeStamp(ns));
        parsers.put(new QName(ns, "Tour"), new GXTour(ns));
        parsers.put(new QName(ns, "TourControl"), new GXTourControl(ns));
        parsers.put(new QName(ns, "Wait"), new GXWait(ns));

        StringXMLEventParser stringParser = new StringXMLEventParser();
        foreach (String s in StringFields)
        {
            parsers.put(new QName(ns, s), stringParser);
        }

        DoubleXMLEventParser doubleParser = new DoubleXMLEventParser();
        foreach (String s in DoubleFields)
        {
            parsers.put(new QName(ns, s), doubleParser);
        }

        BooleanXMLEventParser booleanParser = new BooleanXMLEventParser();
        foreach (String s in BooleanFields)
        {
            parsers.put(new QName(ns, s), booleanParser);
        }

        return parsers;
    }
}
}
