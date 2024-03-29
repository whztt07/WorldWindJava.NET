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
using SharpEarth.events.Message;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>StyleMap</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLStyleMap.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLStyleMap : KMLAbstractStyleSelector
{
    protected List<KMLPair> pairs = new ArrayList<KMLPair>();

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLStyleMap(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLPair)
            this.addPair((KMLPair) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    public List<KMLPair> getPairs()
    {
        return this.pairs;
    }

    protected void addPair(KMLPair pair)
    {
        this.pairs.add(pair);
    }

    /**
     * Returns a specified style from the style map.
     *
     * @param styleState the style key, either {@link KMLConstants#NORMAL} or {@link KMLConstants#HIGHLIGHT}. If null,
     *                   {@link KMLConstants#NORMAL} is used.
     *
     * @return the requested style, or null if it does not exist in the map.
     */
    public KMLAbstractStyleSelector getStyleFromMap(String styleState)
    {
        if (styleState == null)
            styleState = KMLConstants.NORMAL;

        foreach (KMLPair pair in this.pairs)
        {
            if (pair.getKey().Equals(styleState))
                return pair.getStyleSelector();
        }

        return null;
    }

    /**
     * Returns a specified style URL from the style map.
     *
     * @param styleState the style key, either {@link KMLConstants#NORMAL} or {@link KMLConstants#HIGHLIGHT}. If null,
     *                   {@link KMLConstants#NORMAL} is used.
     *
     * @return the requested style URL, or null if it does not exist in the map.
     */
    public KMLStyleUrl getStyleUrlFromMap(String styleState)
    {
        if (styleState == null)
            styleState = KMLConstants.NORMAL;

        foreach (KMLPair pair in this.pairs)
        {
            if (pair.getKey().Equals(styleState))
                return pair.getStyleUrl();
        }

        return null;
    }

    /**
     * Obtains the map's effective style for a specified style type (<i>IconStyle</i>, <i>ListStyle</i>, etc.) and state
     * (<i>normal</i> or <i>highlight</i>). The returned style is the result of merging values from the map's style
     * selectors and style URL for the indicated sub-style type, with precedence given to style selectors.
     * <p/>
     * Remote <i>styleUrls</i> that have not yet been resolved are not included in the result. In this case the returned
     * sub-style is marked with the value {@link SharpEarth.avlist.AVKey#UNRESOLVED}.
     *
     * @param styleState the style mode, either \"normal\" or \"highlight\".
     * @param subStyle   an instance of the {@link SharpEarth.ogc.kml.KMLAbstractSubStyle} class desired, such
     *                   as {@link SharpEarth.ogc.kml.KMLIconStyle}. The effective style values are accumulated
     *                   and merged into this instance. The instance should not be one from within the KML document
     *                   because its values may be overridden and augmented. The instance specified is the return value
     *                   of this method.
     *
     * @return the sub-style values for the specified type and state. The reference returned is the same one passed in
     *         as the <code>subStyle</code> argument.
     */
    public KMLAbstractSubStyle mergeSubStyles(KMLAbstractSubStyle subStyle, String styleState)
    {
        KMLStyleUrl styleUrl = this.getStyleUrlFromMap(styleState);
        KMLAbstractStyleSelector selector = this.getStyleFromMap(styleState);
        if (selector == null && styleUrl == null)
            return subStyle;
        else
            subStyle.setField(KMLConstants.STYLE_STATE, styleState); // identify which style state it is

        return KMLAbstractStyleSelector.mergeSubStyles(styleUrl, selector, styleState, subStyle);
    }

    @Override
    public void applyChange(KMLAbstractObject sourceValues)
    {
        if (!(sourceValues is KMLStyleMap))
        {
            String message = Logging.getMessage("nullValue.SourceIsNull");
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        super.applyChange(sourceValues);

        KMLStyleMap sourceMap = (KMLStyleMap) sourceValues;

        if (sourceMap.getPairs() != null && sourceMap.getPairs().size() > 0)
            this.pairs = sourceMap.getPairs();

        this.onChange(new Message(KMLAbstractObject.MSG_STYLE_CHANGED, this));
    }

    /**
     * Merge a list of incoming pairs with the current list. If an incoming pair has the same ID as an
     * existing one, replace the existing one, otherwise just add the incoming one.
     *
     * @param sourceMap the incoming pairs.
     */
    protected void mergePairs(KMLStyleMap sourceMap)
    {
        // Make a copy of the existing list so we can modify it as we traverse the copy.
        List<KMLPair> pairsCopy = new ArrayList<KMLPair>(this.getPairs().size());
        Collections.copy(pairsCopy, this.getPairs());

        foreach (KMLPair sourcePair in sourceMap.getPairs())
        {
            String id = sourcePair.getId();
            if (!WWUtil.isEmpty(id))
            {
                foreach (KMLPair existingPair in pairsCopy)
                {
                    String currentId = existingPair.getId();
                    if (!WWUtil.isEmpty(currentId) && currentId.Equals(id))
                    {
                        this.getPairs().remove(existingPair);
                    }
                }
            }

            this.getPairs().add(sourcePair);
        }
    }
}
}
