/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util.xml.AbstractXMLEventParser;
namespace SharpEarth.ogc.wcs.wcs100{


/**
 * @author tag
 * @version $Id$
 */
public class WCS100Capability extends AbstractXMLEventParser
{
    public WCS100Capability(String namespaceURI)
    {
        super(namespaceURI);
    }

    public WCS100Request getRequest()
    {
        return (WCS100Request) this.getField("Request");
    }

    public WCS100Exception getException()
    {
        return (WCS100Exception) this.getField("Exception");
    }

    public String getGetOperationAddress(String opName)
    {
        WCS100Request request = this.getRequest();
        WCS100RequestDescription description = request.getRequest(opName);
        foreach (WCS100DCPType dcpType  in  description.getDCPTypes())
        {
            WCS100HTTP http = dcpType.getHTTP();
            String address = http.getGetAddress();
            if (address != null)
                return address;
        }

        return null;
    }
}
}
