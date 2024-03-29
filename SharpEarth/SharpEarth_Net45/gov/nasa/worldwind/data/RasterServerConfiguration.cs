/*
 * Copyright (C) 2015 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream;
using SharpEarth.util.xml;
using SharpEarth.util;
using SharpEarth.geom.Sector;
namespace SharpEarth.data{



/**
 * Parses a raster server configuration document.
 *
 * @author tag
 * @version $Id: RasterServerConfiguration.java 2813 2015-02-18 23:35:24Z tgaskins $
 */
public class RasterServerConfiguration : AbstractXMLEventParser
{
    protected static class Property : AbstractXMLEventParser
    {
        public Property(String namespaceURI)
        {
            base(namespaceURI);
        }

        public String getName()
        {
            return (String) this.getField("name");
        }

        public String getValue()
        {
            return (String) this.getField("value");
        }
    }

    protected static class RasterSector : AbstractXMLEventParser
    {
        public RasterSector(String namespaceURI)
        {
            base(namespaceURI);
        }

        public Sector getSector()
        {
            AbstractXMLEventParser corner = (AbstractXMLEventParser) this.getField("SouthWest");
            AbstractXMLEventParser latLon = (AbstractXMLEventParser) corner.getField("LatLon");
            Double minLat = Double.valueOf((String) latLon.getField("latitude"));
            Double minLon = Double.valueOf((String) latLon.getField("longitude"));
            String units = (String) latLon.getField("units");

            corner = (AbstractXMLEventParser) this.getField("NorthEast");
            latLon = (AbstractXMLEventParser) corner.getField("LatLon");
            Double maxLat = Double.valueOf((String) latLon.getField("latitude"));
            Double maxLon = Double.valueOf((String) latLon.getField("longitude"));

            if (units.Equals("radians"))
                return Sector.fromRadians(minLat, maxLat, minLon, maxLon);
            else
                return Sector.fromDegrees(minLat, maxLat, minLon, maxLon);
        }
    }

    protected static class Corner : AbstractXMLEventParser
    {
        public Corner(String namespaceURI)
        {
            base(namespaceURI);
        }
    }

    public static class Source : AbstractXMLEventParser
    {
        public Source(String namespaceURI)
        {
            base(namespaceURI);
        }

        public String getPath()
        {
            return (String) this.getField("path");
        }

        public String getType()
        {
            return (String) this.getField("type");
        }

        public Sector getSector()
        {
            return ((RasterSector) this.getField("Sector")).getSector();
        }
    }

    protected static class Sources : AbstractXMLEventParser
    {
        protected ArrayList<Source> sources = new ArrayList<Source>();

        public Sources(String namespaceURI)
        {
            base(namespaceURI);
        }

        public ArrayList<Source> getSources()
        {
            return this.sources;
        }

        protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
            throws XMLStreamException
        {
            if (ctx.isStartElement(event, "Source"))
            {
                Source s = (Source) ctx.getParser(event).parse(ctx, event);
                this.sources.add(s);
            }
            else
            {
                base.doParseEventContent(ctx, event, args);
            }
        }
    }

    protected static String namespaceURI;

    protected XMLEventReader eventReader;
    protected XMLEventParserContext parserContext;

    protected HashMap<String, String> properties = new HashMap<String, String>();

    public RasterServerConfiguration(Object docSource)
    {
        base(namespaceURI);

        this.eventReader = this.createReader(docSource);

        this.initialize();
    }

    protected void initialize()
    {
        this.parserContext = this.createParserContext(this.eventReader);
    }

    protected XMLEventReader createReader(Object docSource)
    {
        return WWXML.openEventReader(docSource);
    }

    protected XMLEventParserContext createParserContext(XMLEventReader reader)
    {
        this.parserContext = new BasicXMLEventParserContext(reader);
        this.parserContext.setDefaultNamespaceURI(this.getNamespaceURI());

        return this.parserContext;
    }

    public XMLEventParserContext getParserContext()
    {
        return this.parserContext;
    }

    public String getVersion()
    {
        return (String) this.getField("version");
    }

    public Sector getSector()
    {
        RasterSector sector = (RasterSector) this.getField("Sector");

        return sector != null ? sector.getSector() : null;
    }

    public HashMap<String, String> getProperties()
    {
        return this.properties;
    }

    public ArrayList<Source> getSources()
    {
        return ((Sources) this.getField("Sources")).getSources();
    }

    public RasterServerConfiguration parse(Object... args) throws XMLStreamException
    {
        XMLEventParserContext ctx = this.parserContext;
        QName capsName = new QName(this.getNamespaceURI(), "RasterServer");

        for (XMLEvent event = ctx.nextEvent(); ctx.hasNext(); event = ctx.nextEvent())
        {
            if (event == null)
                continue;

            if (event.isStartElement() && event.asStartElement().getName().Equals(capsName))
            {
                // Parse the attributes in order to get the version number in order to determine the namespaces.
                this.doParseEventAttributes(ctx, event);
                ctx.setDefaultNamespaceURI(this.getNamespaceURI());

                // Now register the parsers.
                this.registerParsers(ctx);

                base.parse(ctx, event, args);
                return this;
            }
        }

        return null;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "Property"))
        {
            Property p = (Property) ctx.getParser(event).parse(ctx, event);
            this.properties.put(p.getName(), p.getValue());
        }
        else
        {
            base.doParseEventContent(ctx, event, args);
        }
    }

    protected void registerParsers(XMLEventParserContext ctx)
    {
        ctx.registerParser(new QName(this.getNamespaceURI(), "Property"),
            new Property(this.getNamespaceURI()));

        ctx.registerParser(new QName(this.getNamespaceURI(), "LatLon"),
            new AttributesOnlyXMLEventParser(this.getNamespaceURI()));

        ctx.registerParser(new QName(this.getNamespaceURI(), "SouthWest"),
            new Corner(this.getNamespaceURI()));

        ctx.registerParser(new QName(this.getNamespaceURI(), "NorthEast"),
            new Corner(this.getNamespaceURI()));

        ctx.registerParser(new QName(this.getNamespaceURI(), "Sector"),
            new RasterSector(this.getNamespaceURI()));

        ctx.registerParser(new QName(this.getNamespaceURI(), "Source"),
            new Source(this.getNamespaceURI()));

        ctx.registerParser(new QName(this.getNamespaceURI(), "Sources"),
            new Sources(this.getNamespaceURI()));
    }
}
}
