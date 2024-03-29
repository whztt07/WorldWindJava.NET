/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util;
using SharpEarth.events.Message;
namespace SharpEarth.ogc.kml{


/**
 * Represents the KML <i>SubStyle</i> element.
 *
 * @author tag
 * @version $Id: KMLAbstractSubStyle.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public abstract class KMLAbstractSubStyle : KMLAbstractObject
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    protected KMLAbstractSubStyle(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    public void applyChange(KMLAbstractObject sourceValues)
    {
        if (!(sourceValues is KMLAbstractSubStyle))
        {
            String message = Logging.getMessage("KML.InvalidElementType", sourceValues.GetType().Name);
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        super.applyChange(sourceValues);

        this.onChange(new Message(KMLAbstractObject.MSG_STYLE_CHANGED, this));
    }
}
}
