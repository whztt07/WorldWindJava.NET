/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util.xml.AbstractXMLEventParser;
using SharpEarth.ogc.ows.OWSContactInfo;
namespace SharpEarth.ogc.wcs.wcs100{


/**
 * @author tag
 * @version $Id: WCS100ResponsibleParty.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100ResponsibleParty : AbstractXMLEventParser
{
    public WCS100ResponsibleParty(String namespaceURI)
    {
        super(namespaceURI);
    }

    public String getIndividualName()
    {
        return (String) this.getField("individualName");
    }

    public String getOrganisationName()
    {
        return (String) this.getField("organisationName");
    }

    public String getPositionName()
    {
        return (String) this.getField("positionName");
    }

    public OWSContactInfo getContactInfo()
    {
        return (OWSContactInfo) this.getField("contactInfo");
    }
}
}
