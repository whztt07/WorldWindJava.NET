/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.Iterator;
using javax.xml.stream.events;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml;
using SharpEarth.util.WWUtil;
namespace SharpEarth.ogc.wms{



/**
 * Parses a WMS layer Dimension element.
 *
 * @author tag
 * @version $Id: WMSLayerDimension.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class WMSLayerDimension : AbstractXMLEventParser
{
    protected StringBuilder dimension;
    protected String name;
    protected String units;
    protected String unitSymbol;
    protected String defaultValue;
    protected Boolean multipleValues;
    protected Boolean nearestValue;
    protected Boolean current;

    public WMSLayerDimension(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (event.isCharacters())
        {
            String s = ctx.getCharacters(event);
            if (!WWUtil.isEmpty(s))
            {
                if (this.dimension == null)
                    this.dimension = new StringBuilder();

                this.dimension.append(s);
            }
        }
    }

    @Override
    protected void doParseEventAttributes(XMLEventParserContext ctx, XMLEvent event, Object... args)
    {
        Iterator iter = event.asStartElement().getAttributes();
        if (iter == null)
            return;

        while (iter.hasNext())
        {
            Attribute attr = (Attribute) iter.next();
            if (attr.getName().getLocalPart().Equals("name") && attr.getValue() != null)
                this.setName(attr.getValue());

            else if (attr.getName().getLocalPart().Equals("units") && attr.getValue() != null)
                this.setUnits(attr.getValue());

            else if (attr.getName().getLocalPart().Equals("unitSymbol") && attr.getValue() != null)
                this.setUnitSymbol(attr.getValue());

            else if (attr.getName().getLocalPart().Equals("default") && attr.getValue() != null)
                this.setDefaultValue(attr.getValue());

            else if (attr.getName().getLocalPart().Equals("multipleValues") && attr.getValue() != null)
            {
                Boolean d = WWUtil.convertStringToBoolean(attr.getValue());
                if (d != null)
                    this.setMultipleValues(d);
            }
            else if (attr.getName().getLocalPart().Equals("nearestValue") && attr.getValue() != null)
            {
                Boolean d = WWUtil.convertStringToBoolean(attr.getValue());
                if (d != null)
                    this.setNearestValue(d);
            }
            else if (attr.getName().getLocalPart().Equals("current") && attr.getValue() != null)
            {
                Boolean d = WWUtil.convertStringToBoolean(attr.getValue());
                if (d != null)
                    this.setCurrent(d);
            }
        }
    }

    public String getDimension()
    {
        if (this.dimension == null)
            this.dimension = new StringBuilder();

        return dimension.ToString();
    }

    public String getName()
    {
        return name;
    }

    protected void setName(String name)
    {
        this.name = name;
    }

    public String getUnits()
    {
        return units;
    }

    protected void setUnits(String units)
    {
        this.units = units;
    }

    public String getUnitSymbol()
    {
        return unitSymbol;
    }

    protected void setUnitSymbol(String unitSymbol)
    {
        this.unitSymbol = unitSymbol;
    }

    public String getDefaultValue()
    {
        return defaultValue;
    }

    protected void setDefaultValue(String defaultValue)
    {
        this.defaultValue = defaultValue;
    }

    public Boolean isMultipleValues()
    {
        return multipleValues;
    }

    protected void setMultipleValues(Boolean multipleValues)
    {
        this.multipleValues = multipleValues;
    }

    public Boolean isNearestValue()
    {
        return nearestValue;
    }

    protected void setNearestValue(Boolean nearestValue)
    {
        this.nearestValue = nearestValue;
    }

    public Boolean isCurrent()
    {
        return current;
    }

    protected void setCurrent(Boolean current)
    {
        this.current = current;
    }
}
}
