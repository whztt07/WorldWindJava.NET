/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.logging.Level;
using SharpEarth.util.xml.AbstractXMLEventParser;
using SharpEarth.util;
namespace SharpEarth.ogc.wcs.wcs100{



/**
 * @author tag
 * @version $Id: WCS100SingleValue.java 2066 2014-06-20 20:41:46Z tgaskins $
 */
public class WCS100SingleValue : AbstractXMLEventParser
{
    public WCS100SingleValue(String namespaceURI)
    {
        super(namespaceURI);
    }

    public String getType()
    {
        return (String) this.getField("type");
    }

    public String getSemantic()
    {
        return (String) this.getField("semantic");
    }

    public String getSingleValueString()
    {
        return (String) this.getField("CharactersContent");
    }

    public Double getSingleValue()
    {
        if (this.getSingleValueString() == null)
            return null;

        try
        {
            return Double.parseDouble(this.getSingleValueString());
        }
        catch (NumberFormatException e)
        {
            String message = Logging.getMessage("generic.NumberFormatException");
            Logging.logger().log(Level.WARNING, message, e);
            return null;
        }
    }
}
}
