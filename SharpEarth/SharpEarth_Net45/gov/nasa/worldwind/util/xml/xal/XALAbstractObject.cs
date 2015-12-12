/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util.xml.AbstractXMLEventParser;
namespace SharpEarth.util.xml.xal{


/**
 * @author tag
 * @version $Id: XALAbstractObject.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class XALAbstractObject : AbstractXMLEventParser
{
    public XALAbstractObject(String namespaceURI)
    {
        super(namespaceURI);
    }

    public String getType()
    {
        return (String) this.getField("Type");
    }

    public String getCode()
    {
        return (String) this.getField("Code");
    }
}
}
