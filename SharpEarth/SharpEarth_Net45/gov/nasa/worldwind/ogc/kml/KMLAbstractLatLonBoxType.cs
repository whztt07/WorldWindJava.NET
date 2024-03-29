/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util;
using SharpEarth.events.Message;
namespace SharpEarth.ogc.kml{


/**
 * Represents the KML <i>LatLonBox</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLAbstractLatLonBoxType.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public abstract class KMLAbstractLatLonBoxType : KMLAbstractObject
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    protected KMLAbstractLatLonBoxType(String namespaceURI)
    {
        super(namespaceURI);
    }

    public Double getNorth()
    {
        return (Double) this.getField("north");
    }

    public Double getSouth()
    {
        return (Double) this.getField("south");
    }

    public Double getEast()
    {
        return (Double) this.getField("east");
    }

    public Double getWest()
    {
        return (Double) this.getField("west");
    }

    @Override
    public void applyChange(KMLAbstractObject sourceValues)
    {
        if (!(sourceValues is KMLAbstractLatLonBoxType))
        {
            String message = Logging.getMessage("nullValue.SourceIsNull");
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        super.applyChange(sourceValues);

        this.onChange(new Message(KMLAbstractObject.MSG_BOX_CHANGED, this));
    }
}
}
