/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.Map;
using SharpEarth.util;
using SharpEarth.events.Message;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>Style</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLStyle.java 1528 2013-07-31 01:00:32Z pabercrombie $
 */
public class KMLStyle : KMLAbstractStyleSelector
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLStyle(String namespaceURI)
    {
        super(namespaceURI);
    }

    public KMLIconStyle getIconStyle()
    {
        return (KMLIconStyle) this.getField(KMLConstants.ICON_STYLE_FIELD);
    }

    public KMLLabelStyle getLabelStyle()
    {
        return (KMLLabelStyle) this.getField(KMLConstants.LABEL_STYLE_FIELD);
    }

    public KMLLineStyle getLineStyle()
    {
        return (KMLLineStyle) this.getField(KMLConstants.LINE_STYLE_FIELD);
    }

    public KMLPolyStyle getPolyStyle()
    {
        return (KMLPolyStyle) this.getField(KMLConstants.POLY_STYLE_FIELD);
    }

    public KMLBalloonStyle getBaloonStyle()
    {
        return (KMLBalloonStyle) this.getField(KMLConstants.BALOON_STYLE_FIELD);
    }

    public KMLListStyle getListStyle()
    {
        return (KMLListStyle) this.getField(KMLConstants.LIST_STYLE_FIELD);
    }

    /**
     * {@inheritDoc} Overridden to handle deprecated {@code labelColor} field. The {@code labelColor} field is
     * deprecated, and has been replaced by {@code LabelStyle}. If {@code labelColor} is set this method will apply the
     * color to the {@code LabelStyle}, creating a new {@code LabelStyle} if necessary.
     */
    @Override
    public void setField(String keyName, Object value)
    {
        if ("labelColor".Equals(keyName))
        {
            KMLLabelStyle labelStyle = this.getLabelStyle();
            if (labelStyle == null)
            {
                labelStyle = new KMLLabelStyle(this.getNamespaceURI());
                this.setField(KMLConstants.LABEL_STYLE_FIELD, labelStyle);
            }
            labelStyle.setField("color", value);
        }
        else
        {
            super.setField(keyName, value);
        }
    }

    /**
     * Adds the sub-style fields of a specified sub-style to this one's fields if they don't already exist.
     *
     * @param subStyle the sub-style to merge with this one.
     *
     * @return the substyle passed in as the parameter.
     *
     * @throws ArgumentException if the sub-style parameter is null.
     */
    public KMLAbstractSubStyle mergeSubStyle(KMLAbstractSubStyle subStyle)
    {
        if (subStyle == null)
        {
            String message = Logging.getMessage("nullValue.SymbolIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (!this.hasFields())
            return subStyle;

        Class subStyleClass = subStyle.GetType();
        foreach (Map.Entry<String, Object> field in this.getFields().getEntries())
        {
            if (field.getValue() != null && field.getValue().GetType().Equals(subStyleClass))
            {
                this.overrideFields(subStyle, (KMLAbstractSubStyle) field.getValue());
            }
        }

        return subStyle;
    }

    @Override
    public void applyChange(KMLAbstractObject sourceValues)
    {
        if (!(sourceValues is KMLStyle))
        {
            String message = Logging.getMessage("KML.InvalidElementType", sourceValues.GetType().Name);
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        super.applyChange(sourceValues);

        this.onChange(new Message(KMLAbstractObject.MSG_STYLE_CHANGED, this));
    }
}
}
