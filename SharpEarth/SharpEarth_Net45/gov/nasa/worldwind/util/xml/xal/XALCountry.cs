/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.XMLStreamException;
using javax.xml.stream.events.XMLEvent;
using SharpEarth.util.xml.XMLEventParserContext;
namespace SharpEarth.util.xml.xal{



/**
 * @author tag
 * @version $Id: XALCountry.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class XALCountry : XALAbstractObject
{
    protected List<XALAddressLine> addressLines;
    protected List<XALCountryNameCode> countryNameCodes;
    protected List<XALCountryName> countryNames;

    public XALCountry(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is XALAddressLine)
            this.addAddressLine((XALAddressLine) o);
        else if (o is XALCountryNameCode)
            this.addCountryNameCode((XALCountryNameCode) o);
        else if (o is XALCountryName)
            this.addCountryName((XALCountryName) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    public List<XALAddressLine> getAddressLines()
    {
        return this.addressLines;
    }

    protected void addAddressLine(XALAddressLine o)
    {
        if (this.addressLines == null)
            this.addressLines = new ArrayList<XALAddressLine>();

        this.addressLines.add(o);
    }

    public List<XALCountryNameCode> getCountryNameCodes()
    {
        return this.countryNameCodes;
    }

    protected void addCountryNameCode(XALCountryNameCode o)
    {
        if (this.countryNameCodes == null)
            this.countryNameCodes = new ArrayList<XALCountryNameCode>();

        this.countryNameCodes.add(o);
    }

    public List<XALCountryName> getCountryNames()
    {
        return this.countryNames;
    }

    protected void addCountryName(XALCountryName o)
    {
        if (this.countryNames == null)
            this.countryNames = new ArrayList<XALCountryName>();

        this.countryNames.add(o);
    }

    public XALAdministrativeArea getAdministrativeArea()
    {
        return (XALAdministrativeArea) this.getField("AdministrativeArea");
    }

    public XALLocality getLocality()
    {
        return (XALLocality) this.getField("Locality");
    }

    public XALThoroughfare getThoroughfare()
    {
        return (XALThoroughfare) this.getField("Thoroughfare");
    }
}
}
