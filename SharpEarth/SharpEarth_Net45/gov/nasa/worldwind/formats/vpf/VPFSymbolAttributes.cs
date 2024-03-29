/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util.Arrays;
using java.awt;
using SharpEarth.util;
using SharpEarth.render;
using SharpEarth.geom.Angle;
namespace SharpEarth.formats.vpf{



/**
 * @author Patrick Murris
 * @version $Id: VPFSymbolAttributes.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class VPFSymbolAttributes : BasicShapeAttributes
{
    public static class LabelAttributes
    {
        private Font font;
        private Color color;
        private Color backgroundColor;
        private double offset;
        private Angle offsetAngle;
        private String prepend;
        private String append;
        private String attributeName;
        private int abbreviationTableId;

        public LabelAttributes()
        {
            this.font = defaultFont;
            this.color = defaultColor;
            this.backgroundColor = defaultBackgroundColor;
        }

        public LabelAttributes(LabelAttributes attributes)
        {
            if (attributes == null)
            {
                String message = Logging.getMessage("nullValue.AttributesIsNull");
                Logging.logger().severe(message);
                throw new ArgumentException(message);
            }

            this.font = attributes.getFont();
            this.color = attributes.getColor();
            this.backgroundColor = attributes.getBackgroundColor();
            this.offset = attributes.getOffset();
            this.offsetAngle = attributes.getOffsetAngle();
            this.prepend = attributes.getPrepend();
            this.append = attributes.getAppend();
            this.attributeName = attributes.getAttributeName();
            this.abbreviationTableId = attributes.getAbbreviationTableId();
        }

        public LabelAttributes copy()
        {
            return new LabelAttributes(this);
        }

        public Font getFont()
        {
            return this.font;
        }

        public void setFont(Font font)
        {
            this.font = font;
        }

        public Color getColor()
        {
            return this.color;
        }

        public void setColor(Color color)
        {
            this.color = color;
        }

        public Color getBackgroundColor()
        {
            return this.backgroundColor;
        }

        public void setBackgroundColor(Color color)
        {
            this.backgroundColor = color;
        }

        public double getOffset()
        {
            return offset;
        }

        public void setOffset(double offset)
        {
            this.offset = offset;
        }

        public Angle getOffsetAngle()
        {
            return this.offsetAngle;
        }

        public void setOffsetAngle(Angle angle)
        {
            this.offsetAngle = angle;
        }

        public String getPrepend()
        {
            return this.prepend;
        }

        public void setPrepend(String text)
        {
            this.prepend = text;
        }

        public String getAppend()
        {
            return this.append;
        }

        public void setAppend(String text)
        {
            this.append = text;
        }

        public String getAttributeName()
        {
            return this.attributeName;
        }

        public void setAttributeName(String name)
        {
            this.attributeName = name;
        }

        public int getAbbreviationTableId()
        {
            return this.abbreviationTableId;
        }

        public void setAbbreviationTableId(int tableId)
        {
            this.abbreviationTableId = tableId;
        }

        @SuppressWarnings({"RedundantIfStatement"})
        @Override
        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            LabelAttributes that = (LabelAttributes) o;

            if (this.abbreviationTableId != that.abbreviationTableId)
                return false;
            if (Double.compare(this.offset, that.offset) != 0)
                return false;
            if (this.append != null ? !this.append.Equals(that.append) : that.append != null)
                return false;
            if (this.attributeName != null ? !this.attributeName.Equals(that.attributeName)
                : that.attributeName != null)
                return false;
            if (this.backgroundColor != null ? !this.backgroundColor.Equals(that.backgroundColor)
                : that.backgroundColor != null)
                return false;
            if (this.color != null ? !this.color.Equals(that.color) : that.color != null)
                return false;
            if (this.font != null ? !this.font.Equals(that.font) : that.font != null)
                return false;
            if (this.offsetAngle != null ? !this.offsetAngle.Equals(that.offsetAngle) : that.offsetAngle != null)
                return false;
            if (this.prepend != null ? !this.prepend.Equals(that.prepend) : that.prepend != null)
                return false;

            return true;
        }

        @Override
        public override int GetHashCode()
        {
            int result;
            long temp;
            result = this.font != null ? this.font.GetHashCode() : 0;
            result = 31 * result + (this.color != null ? this.color.GetHashCode() : 0);
            result = 31 * result + (this.backgroundColor != null ? this.backgroundColor.GetHashCode() : 0);
            temp = this.offset != +0.0d ? BitConverter.DoubleToInt64Bits(this.offset) : 0L;
            result = 31 * result + (int) (temp ^ (temp >>> 32));
            result = 31 * result + (this.offsetAngle != null ? this.offsetAngle.GetHashCode() : 0);
            result = 31 * result + (this.prepend != null ? this.prepend.GetHashCode() : 0);
            result = 31 * result + (this.append != null ? this.append.GetHashCode() : 0);
            result = 31 * result + (this.attributeName != null ? this.attributeName.GetHashCode() : 0);
            result = 31 * result + this.abbreviationTableId;
            return result;
        }
    }

    private static final Font defaultFont = Font.decode("Arial-PLAIN-12");
    private static final Color defaultColor = Color.WHITE;
    private static final Color defaultBackgroundColor = Color.BLACK;

    private VPFFeatureType featureType;
    private VPFSymbolKey symbolKey;
    private Object iconImageSource;
    private double iconImageScale;
    private bool mipMapIconImage;
    private LabelAttributes[] labelAttributes;
    private double displayPriority;
    private String orientationAttributeName;
    private String description;

    public VPFSymbolAttributes()
    {
    }

    public VPFSymbolAttributes(VPFFeatureType featureType, VPFSymbolKey symbolKey)
    {
        this.featureType = featureType;
        this.symbolKey = symbolKey;
        this.iconImageSource = null;
        this.iconImageScale = 1d;
        this.mipMapIconImage = true;
        this.labelAttributes = null;
        this.displayPriority = 0;
        this.orientationAttributeName = null;
        this.description = null;
    }

    public VPFSymbolAttributes(VPFSymbolAttributes attributes)
    {
        super(attributes);
        this.featureType = attributes.getFeatureType();
        this.symbolKey = attributes.getSymbolKey();
        this.iconImageSource = attributes.getIconImageSource();
        this.iconImageScale = attributes.getIconImageScale();
        this.mipMapIconImage = attributes.isMipMapIconImage();
        this.displayPriority = attributes.getDisplayPriority();
        this.orientationAttributeName = attributes.getOrientationAttributeName();
        this.description = attributes.getDescription();

        if (attributes.getLabelAttributes() != null)
        {
            LabelAttributes[] array = attributes.getLabelAttributes();
            int numLabelAttributes = array.length;
            this.labelAttributes = new LabelAttributes[numLabelAttributes];

            for (int i = 0; i < numLabelAttributes; i++)
            {
                this.labelAttributes[i] = (array[i] != null) ? array[i].copy() : null;
            }
        }
    }

    /** {@inheritDoc} */
    public ShapeAttributes copy()
    {
        return new VPFSymbolAttributes(this);
    }

    /** {@inheritDoc} */
    public void copy(ShapeAttributes attributes)
    {
        super.copy(attributes);

        if (attributes is VPFSymbolAttributes)
        {
            VPFSymbolAttributes vpfAttrs = (VPFSymbolAttributes) attributes;
            this.featureType = vpfAttrs.getFeatureType();
            this.symbolKey = vpfAttrs.getSymbolKey();
            this.iconImageSource = vpfAttrs.getIconImageSource();
            this.iconImageScale = vpfAttrs.getIconImageScale();
            this.mipMapIconImage = vpfAttrs.isMipMapIconImage();
            this.displayPriority = vpfAttrs.getDisplayPriority();
            this.orientationAttributeName = vpfAttrs.getOrientationAttributeName();
            this.description = vpfAttrs.getDescription();

            if (vpfAttrs.getLabelAttributes() != null)
            {
                LabelAttributes[] array = vpfAttrs.getLabelAttributes();
                int numLabelAttributes = array.length;
                this.labelAttributes = new LabelAttributes[numLabelAttributes];

                for (int i = 0; i < numLabelAttributes; i++)
                {
                    this.labelAttributes[i] = (array[i] != null) ? array[i].copy() : null;
                }
            }
        }
    }

    public VPFFeatureType getFeatureType()
    {
        return this.featureType;
    }

    public VPFSymbolKey getSymbolKey()
    {
        return this.symbolKey;
    }

    public Object getIconImageSource()
    {
        return this.iconImageSource;
    }

    public void setIconImageSource(Object imageSource)
    {
        this.iconImageSource = imageSource;
    }

    public double getIconImageScale()
    {
        return this.iconImageScale;
    }

    public void setIconImageScale(double scale)
    {
        this.iconImageScale = scale;
    }

    public bool isMipMapIconImage()
    {
        return this.mipMapIconImage;
    }

    public void setMipMapIconImage(boolean mipMap)
    {
        this.mipMapIconImage = mipMap;
    }

    public LabelAttributes[] getLabelAttributes()
    {
        return this.labelAttributes;
    }

    public void setLabelAttributes(LabelAttributes[] attributes)
    {
        this.labelAttributes = attributes;
    }

    public double getDisplayPriority()
    {
        return this.displayPriority;
    }

    public void setDisplayPriority(double displayPriority)
    {
        this.displayPriority = displayPriority;
    }

    public String getOrientationAttributeName()
    {
        return this.orientationAttributeName;
    }

    public void setOrientationAttributeName(String name)
    {
        this.orientationAttributeName = name;
    }

    public String getDescription()
    {
        return this.description;
    }

    public void setDescription(String description)
    {
        this.description = description;
    }

    @SuppressWarnings({"RedundantIfStatement"})
    @Override
    public override bool Equals(Object o)
    {
        if (this == o)
            return true;
        if (o == null || GetType() != o.GetType())
            return false;
        if (!super.Equals(o))
            return false;

        VPFSymbolAttributes that = (VPFSymbolAttributes) o;

        if (Double.compare(this.displayPriority, that.displayPriority) != 0)
            return false;
        if (Double.compare(this.iconImageScale, that.iconImageScale) != 0)
            return false;
        if (this.mipMapIconImage != that.mipMapIconImage)
            return false;
        if (this.description != null ? !this.description.Equals(that.description) : that.description != null)
            return false;
        if (this.featureType != that.featureType)
            return false;
        if (this.iconImageSource != null ? !this.iconImageSource.Equals(that.iconImageSource)
            : that.iconImageSource != null)
            return false;
        if (!Arrays.Equals(this.labelAttributes, that.labelAttributes))
            return false;
        if (this.orientationAttributeName != null ? !this.orientationAttributeName.Equals(that.orientationAttributeName)
            : that.orientationAttributeName != null)
            return false;
        if (this.symbolKey != null ? !this.symbolKey.Equals(that.symbolKey) : that.symbolKey != null)
            return false;

        return true;
    }

    @Override
    public override int GetHashCode()
    {
        int result = super.GetHashCode();
        long temp;
        result = 31 * result + (this.featureType != null ? this.featureType.GetHashCode() : 0);
        result = 31 * result + (this.symbolKey != null ? this.symbolKey.GetHashCode() : 0);
        result = 31 * result + (this.iconImageSource != null ? this.iconImageSource.GetHashCode() : 0);
        temp = this.iconImageScale != +0.0d ? BitConverter.DoubleToInt64Bits(this.iconImageScale) : 0L;
        result = 31 * result + (int) (temp ^ (temp >>> 32));
        result = 31 * result + (this.mipMapIconImage ? 1 : 0);
        result = 31 * result + (this.labelAttributes != null ? Arrays.hashCode(this.labelAttributes) : 0);
        temp = this.displayPriority != +0.0d ? BitConverter.DoubleToInt64Bits(this.displayPriority) : 0L;
        result = 31 * result + (int) (temp ^ (temp >>> 32));
        result = 31 * result + (this.orientationAttributeName != null ? this.orientationAttributeName.GetHashCode() : 0);
        result = 31 * result + (this.description != null ? this.description.GetHashCode() : 0);
        return result;
    }
}
}
