/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.events.Message;
namespace SharpEarth.ogc.kml{


/**
 * Represents the KML <i>IconStyle</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLIconStyle.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLIconStyle : KMLAbstractColorStyle
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLIconStyle(String namespaceURI)
    {
        super(namespaceURI);
    }

    public Double getScale()
    {
        return (Double) this.getField("scale");
    }

    public Double getHeading()
    {
        return (Double) this.getField("heading");
    }

    public KMLVec2 getHotSpot()
    {
        return (KMLVec2) this.getField("hotSpot");
    }

    public KMLIcon getIcon()
    {
        return (KMLIcon) this.getField("Icon");
    }

    @Override
    public void onChange(Message msg)
    {
        if (KMLAbstractObject.MSG_LINK_CHANGED.Equals(msg.getName()))
            this.onChange(new Message(KMLAbstractObject.MSG_STYLE_CHANGED, this));

        super.onChange(msg);
    }
}
}
