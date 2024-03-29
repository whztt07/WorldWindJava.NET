/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.logging.Level;
using SharpEarth.util.xml.AbstractXMLEventParser;
using SharpEarth.util;
namespace SharpEarth.ogc.gml{



/**
 * @author tag
 * @version $Id: GMLPos.java 2066 2014-06-20 20:41:46Z tgaskins $
 */
public class GMLPos : AbstractXMLEventParser
{
    public GMLPos(String namespaceURI)
    {
        super(namespaceURI);
    }

    public String getDimension()
    {
        return (String) this.getField("dimension");
    }

    public String getPosString()
    {
        return (String) this.getField("CharactersContent");
    }

    public double[] getPos2()
    {
        String[] strings = this.getPosString().split(" ");

        if (strings.length < 2)
            return null;

        try
        {
            return new double[] {Double.parseDouble(strings[0]), Double.parseDouble(strings[1])};
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
