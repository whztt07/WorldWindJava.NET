/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml.XMLEventParserContext;
using SharpEarth.util;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>ListStyle</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLListStyle.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLListStyle : KMLAbstractSubStyle
{
    protected List<KMLItemIcon> itemIcons = new ArrayList<KMLItemIcon>();

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLListStyle(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLItemIcon)
            this.addItemIcon((KMLItemIcon) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    public String getListItemType()
    {
        return (String) this.getField("listItemType");
    }

    public String getBgColor()
    {
        return (String) this.getField("bgColor");
    }

    protected void addItemIcon(KMLItemIcon o)
    {
        this.itemIcons.add(o);
    }

    public List<KMLItemIcon> getItemIcons()
    {
        return this.itemIcons;
    }

    public Integer getMaxSnippetLines()
    {
        return (Integer) this.getField("maxSnippetLines");
    }

    @Override
    public void applyChange(KMLAbstractObject sourceValues)
    {
        if (!(sourceValues is KMLListStyle))
        {
            String message = Logging.getMessage("KML.InvalidElementType", sourceValues.GetType().Name);
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        KMLListStyle sourceStyle = (KMLListStyle) sourceValues;

        if (sourceStyle.getItemIcons() != null && sourceStyle.getItemIcons().size() > 0)
            this.mergeItemIcons(sourceStyle);

        super.applyChange(sourceValues);
    }

    /**
     * Merge a list of incoming item icons with the current list. If an incoming item icon has the same ID as an
     * existing one, replace the existing one, otherwise just add the incoming one.
     *
     * @param sourceStyle the incoming item icons.
     */
    protected void mergeItemIcons(KMLListStyle sourceStyle)
    {
        // Make a copy of the existing list so we can modify it as we traverse the copy.
        List<KMLItemIcon> itemIconsCopy = new ArrayList<KMLItemIcon>(this.getItemIcons().size());
        Collections.copy(itemIconsCopy, this.getItemIcons());

        foreach (KMLItemIcon sourceItemIcon in sourceStyle.getItemIcons())
        {
            String id = sourceItemIcon.getId();
            if (!WWUtil.isEmpty(id))
            {
                foreach (KMLItemIcon existingItemIcon in itemIconsCopy)
                {
                    String currentId = existingItemIcon.getId();
                    if (!WWUtil.isEmpty(currentId) && currentId.Equals(id))
                    {
                        this.getItemIcons().remove(existingItemIcon);
                    }
                }
            }

            this.getItemIcons().add(sourceItemIcon);
        }
    }
}
}
