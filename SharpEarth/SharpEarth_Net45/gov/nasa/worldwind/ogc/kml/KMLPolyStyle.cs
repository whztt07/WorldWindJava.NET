/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.ogc.kml{

/**
 * Represents the KML <i>PolyStyle</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLPolyStyle.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLPolyStyle : KMLAbstractColorStyle
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLPolyStyle(String namespaceURI)
    {
        super(namespaceURI);
    }

    public Boolean getFill()
    {
        return (Boolean) this.getField("fill");
    }

    public bool isFill()
    {
        return this.getFill() == null || this.getFill();
    }

    public Boolean getOutline()
    {
        return (Boolean) this.getField("outline");
    }

    public bool isOutline()
    {
        return this.getOutline() == null || this.getOutline();
    }
}
}
