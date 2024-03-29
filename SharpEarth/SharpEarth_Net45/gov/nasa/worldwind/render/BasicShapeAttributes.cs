/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using static SharpEarth.ogc.kml.impl.KMLExportUtil.kmlBoolean;
using java.io;
using java.awt;
using javax.xml.stream;
using SharpEarth.util;
using SharpEarth.ogc.kml.impl.KMLExportUtil;
using SharpEarth.ogc.kml.KMLConstants;
using SharpEarth.Exportable;
namespace SharpEarth.render{




/**
 * Basic implementation of the {@link SharpEarth.render.ShapeAttributes} interface.
 *
 * @author dcollins
 * @version $Id: BasicShapeAttributes.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class BasicShapeAttributes : ShapeAttributes
{
    /** Indicates whether or not some of the shape's attributes are unresolved. Initially <code>false</code>. */
    protected bool unresolved;
    /** Indicates whether or not the shape's interior is drawn. Initially <code>false</code>. */
    protected bool drawInterior;
    /** Indicates whether or not the shape's outline is drawn. Initially <code>false</code>. */
    protected bool drawOutline;
    /** Indicates whether or not the shape should be rendered with smooth lines and edges. Initially <code>false</code>. */
    protected bool enableAntialiasing;
    /** Indicates whether lighting is applied to the shape. Initially <code>false</code>. */
    protected bool enableLighting;
    /** Indicates the material properties of the shape's interior. Initially <code>null</code>. */
    protected Material interiorMaterial;
    /** Indicates the material properties of the shape's outline. Initially <code>null</code>. */
    protected Material outlineMaterial;
    /** Indicates the opacity of the shape's interior as a floating-point value in the range 0.0 to 1.0. Initially 0.0. */
    protected double interiorOpacity;
    /** Indicates the opacity of the shape's outline as a floating-point value in the range 0.0 to 1.0. Initially 0.0. */
    protected double outlineOpacity;
    /** Indicates the line width (in pixels) used when rendering the shape's outline. Initially 0.0. */
    protected double outlineWidth;
    /** Indicates the number of times each bit in the outline stipple pattern is repeated. Initially 0. */
    protected int outlineStippleFactor;
    /** Indicates the 16-bit integer that defines which pixels are rendered in the shape's outline. Initially 0. */
    protected short outlineStipplePattern;
    /** Indicates the image source that is applied as a texture to the shape's interior. Initially <code>null</code>. */
    protected Object imageSource;
    /** Indicates the amount the balloon's texture is scaled by as a floating-point value. Initially 0.0. */
    protected double imageScale;

    /**
     * Creates a new <code>BasicShapeAttributes</code> with the default attributes. The default attributes are as
     * follows:
     * <p/>
     * <table> <tr><th>Attribute</th><th>Default Value</th></tr> <tr><td>unresolved</td><td><code>true</code></td></tr>
     * <tr><td>drawInterior</td><td><code>true</code></td></tr> <tr><td>drawOutline</td><td><code>true</code></td></tr>
     * <tr><td>enableAntialiasing</td><td><code>true</code></td></tr> <tr><td>enableLighting</td><td><code>false</code></td></tr>
     * <tr><td>interiorMaterial</td><td>{@link SharpEarth.render.Material#WHITE}</td></tr>
     * <tr><td>outlineMaterial</td><td>{@link SharpEarth.render.Material#BLACK}</td></tr>
     * <tr><td>interiorOpacity</td><td>1.0</td></tr> <tr><td>outlineOpacity</td><td>1.0</td></tr>
     * <tr><td>outlineWidth</td><td>1.0</td></tr> <tr><td>outlineStippleFactor</td><td>0</td></tr>
     * <tr><td>outlineStipplePattern</td><td>0xF0F0</td></tr> <tr><td>imageSource</td><td><code>null</code></td></tr>
     * <tr><td>imageScale</td><td>1.0</td></tr> </table>
     */
    public BasicShapeAttributes()
    {
        // Note: update the above constructor comment if these defaults change.

        this.drawInterior = true;
        this.drawOutline = true;
        this.enableAntialiasing = true;
        this.enableLighting = false;
        this.interiorMaterial = Material.WHITE;
        this.outlineMaterial = Material.BLACK;
        this.interiorOpacity = 1;
        this.outlineOpacity = 1;
        this.outlineWidth = 1;
        this.outlineStippleFactor = 0;
        this.outlineStipplePattern = (short) 0xF0F0;
        this.imageSource = null;
        this.imageScale = 1;
    }

    /**
     * Creates a new <code>BasicShapeAttributes</code> configured with the specified <code>attributes</code>.
     *
     * @param attributes the attributes to configure the new <code>BasicShapeAttributes</code> with.
     *
     * @throws ArgumentException if <code>attributes</code> is <code>null</code>.
     */
    public BasicShapeAttributes(ShapeAttributes attributes)
    {
        if (attributes == null)
        {
            String message = Logging.getMessage("nullValue.AttributesIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.drawInterior = attributes.isDrawInterior();
        this.drawOutline = attributes.isDrawOutline();
        this.enableAntialiasing = attributes.isEnableAntialiasing();
        this.enableLighting = attributes.isEnableLighting();
        this.interiorMaterial = attributes.getInteriorMaterial();
        this.outlineMaterial = attributes.getOutlineMaterial();
        this.interiorOpacity = attributes.getInteriorOpacity();
        this.outlineOpacity = attributes.getOutlineOpacity();
        this.outlineWidth = attributes.getOutlineWidth();
        this.outlineStippleFactor = attributes.getOutlineStippleFactor();
        this.outlineStipplePattern = attributes.getOutlineStipplePattern();
        this.imageSource = attributes.getImageSource();
        this.imageScale = attributes.getImageScale();
    }

    /** {@inheritDoc} */
    public ShapeAttributes copy()
    {
        return new BasicShapeAttributes(this);
    }

    /** {@inheritDoc} */
    public void copy(ShapeAttributes attributes)
    {
        if (attributes != null)
        {
            this.drawInterior = attributes.isDrawInterior();
            this.drawOutline = attributes.isDrawOutline();
            this.enableAntialiasing = attributes.isEnableAntialiasing();
            this.enableLighting = attributes.isEnableLighting();
            this.interiorMaterial = attributes.getInteriorMaterial();
            this.outlineMaterial = attributes.getOutlineMaterial();
            this.interiorOpacity = attributes.getInteriorOpacity();
            this.outlineOpacity = attributes.getOutlineOpacity();
            this.outlineWidth = attributes.getOutlineWidth();
            this.outlineStippleFactor = attributes.getOutlineStippleFactor();
            this.outlineStipplePattern = attributes.getOutlineStipplePattern();
            this.imageSource = attributes.getImageSource();
            this.imageScale = attributes.getImageScale();
        }
    }

    /** {@inheritDoc} */
    public bool isUnresolved()
    {
        return unresolved;
    }

    /** {@inheritDoc} */
    public void setUnresolved(boolean unresolved)
    {
        this.unresolved = unresolved;
    }

    /** {@inheritDoc} */
    public bool isDrawInterior()
    {
        return this.drawInterior;
    }

    /** {@inheritDoc} */
    public void setDrawInterior(boolean draw)
    {
        this.drawInterior = draw;
    }

    /** {@inheritDoc} */
    public bool isDrawOutline()
    {
        return this.drawOutline;
    }

    /** {@inheritDoc} */
    public void setDrawOutline(boolean draw)
    {
        this.drawOutline = draw;
    }

    /** {@inheritDoc} */
    public bool isEnableAntialiasing()
    {
        return this.enableAntialiasing;
    }

    /** {@inheritDoc} */
    public void setEnableAntialiasing(boolean enable)
    {
        this.enableAntialiasing = enable;
    }

    /** {@inheritDoc} */
    public bool isEnableLighting()
    {
        return enableLighting;
    }

    /** {@inheritDoc} */
    public void setEnableLighting(boolean enableLighting)
    {
        this.enableLighting = enableLighting;
    }

    /** {@inheritDoc} */
    public Material getInteriorMaterial()
    {
        return this.interiorMaterial;
    }

    /** {@inheritDoc} */
    public void setInteriorMaterial(Material material)
    {
        if (material == null)
        {
            String message = Logging.getMessage("nullValue.MaterialIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.interiorMaterial = material;
    }

    /** {@inheritDoc} */
    public Material getOutlineMaterial()
    {
        return this.outlineMaterial;
    }

    /** {@inheritDoc} */
    public void setOutlineMaterial(Material material)
    {
        if (material == null)
        {
            String message = Logging.getMessage("nullValue.MaterialIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.outlineMaterial = material;
    }

    /** {@inheritDoc} */
    public double getInteriorOpacity()
    {
        return this.interiorOpacity;
    }

    /** {@inheritDoc} */
    public void setInteriorOpacity(double opacity)
    {
        if (opacity < 0 || opacity > 1)
        {
            String message = Logging.getMessage("generic.OpacityOutOfRange", opacity);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.interiorOpacity = opacity;
    }

    /** {@inheritDoc} */
    public double getOutlineOpacity()
    {
        return this.outlineOpacity;
    }

    /** {@inheritDoc} */
    public void setOutlineOpacity(double opacity)
    {
        if (opacity < 0 || opacity > 1)
        {
            String message = Logging.getMessage("generic.OpacityOutOfRange", opacity);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.outlineOpacity = opacity;
    }

    /** {@inheritDoc} */
    public double getOutlineWidth()
    {
        return this.outlineWidth;
    }

    /** {@inheritDoc} */
    public void setOutlineWidth(double width)
    {
        if (width < 0)
        {
            String message = Logging.getMessage("Geom.WidthIsNegative", width);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.outlineWidth = width;
    }

    /** {@inheritDoc} */
    public int getOutlineStippleFactor()
    {
        return this.outlineStippleFactor;
    }

    /** {@inheritDoc} */
    public void setOutlineStippleFactor(int factor)
    {
        if (factor < 0)
        {
            String message = Logging.getMessage("generic.ArgumentOutOfRange", "factor < 0");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.outlineStippleFactor = factor;
    }

    /** {@inheritDoc} */
    public short getOutlineStipplePattern()
    {
        return this.outlineStipplePattern;
    }

    /** {@inheritDoc} */
    public void setOutlineStipplePattern(short pattern)
    {
        this.outlineStipplePattern = pattern;
    }

    /** {@inheritDoc} */
    public Object getImageSource()
    {
        return this.imageSource;
    }

    /** {@inheritDoc} */
    public void setImageSource(Object imageSource)
    {
        // Can be null
        this.imageSource = imageSource;
    }

    /** {@inheritDoc} */
    public double getImageScale()
    {
        return this.imageScale;
    }

    /** {@inheritDoc} */
    public void setImageScale(double scale)
    {
        if (scale <= 0)
        {
            String message = Logging.getMessage("generic.ScaleOutOfRange", scale);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.imageScale = scale;
    }

    /** {@inheritDoc} */
    public void getRestorableState(RestorableSupport rs, RestorableSupport.StateObject so)
    {
        if (rs == null)
        {
            String message = Logging.getMessage("nullValue.RestorableSupportIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        rs.addStateValueAsBoolean(so, "drawInterior", this.isDrawInterior());

        rs.addStateValueAsBoolean(so, "drawOutline", this.isDrawOutline());

        rs.addStateValueAsBoolean(so, "enableAntialiasing", this.isEnableAntialiasing());

        rs.addStateValueAsBoolean(so, "enableLighting", this.isEnableLighting());

        this.getInteriorMaterial().getRestorableState(rs, rs.addStateObject(so, "interiorMaterial"));

        this.getOutlineMaterial().getRestorableState(rs, rs.addStateObject(so, "outlineMaterial"));

        rs.addStateValueAsDouble(so, "interiorOpacity", this.getInteriorOpacity());

        rs.addStateValueAsDouble(so, "outlineOpacity", this.getOutlineOpacity());

        rs.addStateValueAsDouble(so, "outlineWidth", this.getOutlineWidth());

        rs.addStateValueAsInteger(so, "outlineStippleFactor", this.getOutlineStippleFactor());

        rs.addStateValueAsInteger(so, "outlineStipplePattern", this.getOutlineStipplePattern());

        if (this.getImageSource() != null && this.getImageSource() is String)
            rs.addStateValueAsString(so, "interiorImagePath", (String) this.getImageSource());

        rs.addStateValueAsDouble(so, "interiorImageScale", this.getImageScale());
    }

    /** {@inheritDoc} */
    public void restoreState(RestorableSupport rs, RestorableSupport.StateObject so)
    {
        if (rs == null)
        {
            String message = Logging.getMessage("nullValue.RestorableSupportIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        Boolean b = rs.getStateValueAsBoolean(so, "drawInterior");
        if (b != null)
            this.setDrawInterior(b);

        b = rs.getStateValueAsBoolean(so, "drawOutline");
        if (b != null)
            this.setDrawOutline(b);

        b = rs.getStateValueAsBoolean(so, "enableAntialiasing");
        if (b != null)
            this.setEnableAntialiasing(b);

        b = rs.getStateValueAsBoolean(so, "enableLighting");
        if (b != null)
            this.setEnableLighting(b);

        RestorableSupport.StateObject mo = rs.getStateObject(so, "interiorMaterial");
        if (mo != null)
            this.setInteriorMaterial(this.getInteriorMaterial().restoreState(rs, mo));

        mo = rs.getStateObject(so, "outlineMaterial");
        if (mo != null)
            this.setOutlineMaterial(this.getOutlineMaterial().restoreState(rs, mo));

        Double d = rs.getStateValueAsDouble(so, "interiorOpacity");
        if (d != null)
            this.setInteriorOpacity(d);

        d = rs.getStateValueAsDouble(so, "outlineOpacity");
        if (d != null)
            this.setOutlineOpacity(d);

        d = rs.getStateValueAsDouble(so, "outlineWidth");
        if (d != null)
            this.setOutlineWidth(d);

        Integer i = rs.getStateValueAsInteger(so, "outlineStippleFactor");
        if (i != null)
            this.setOutlineStippleFactor(i);

        i = rs.getStateValueAsInteger(so, "outlineStipplePattern");
        if (i != null)
            this.setOutlineStipplePattern(i.shortValue());

        String s = rs.getStateValueAsString(so, "interiorImagePath");
        if (s != null)
            this.setImageSource(s);

        d = rs.getStateValueAsDouble(so, "interiorImageScale");
        if (d != null)
            this.setImageScale(d);
    }

    @Override
    public override bool Equals(Object o)
    {
        if (this == o)
            return true;
        if (o == null || this.GetType() != o.GetType())
            return false;

        BasicShapeAttributes that = (BasicShapeAttributes) o;

        if (this.unresolved != that.unresolved)
            return false;
        if (this.drawInterior != that.drawInterior)
            return false;
        if (this.drawOutline != that.drawOutline)
            return false;
        if (this.enableAntialiasing != that.enableAntialiasing)
            return false;
        if (this.enableLighting != that.enableLighting)
            return false;
        if (this.interiorMaterial != null ? !this.interiorMaterial.Equals(that.interiorMaterial)
            : that.interiorMaterial != null)
            return false;
        if (this.outlineMaterial != null ? !this.outlineMaterial.Equals(that.outlineMaterial)
            : that.outlineMaterial != null)
            return false;
        if (Double.compare(this.interiorOpacity, that.interiorOpacity) != 0)
            return false;
        if (Double.compare(this.outlineOpacity, that.outlineOpacity) != 0)
            return false;
        if (Double.compare(this.outlineWidth, that.outlineWidth) != 0)
            return false;
        if (this.outlineStippleFactor != that.outlineStippleFactor)
            return false;
        if (this.outlineStipplePattern != that.outlineStipplePattern)
            return false;
        if (this.imageSource != null ? !this.imageSource.Equals(that.imageSource) : that.imageSource != null)
            return false;
        //noinspection RedundantIfStatement
        if (Double.compare(this.imageScale, that.imageScale) != 0)
            return false;

        return true;
    }

    @Override
    public override int GetHashCode()
    {
        int result;
        long temp;

        result = (this.unresolved ? 1 : 0);
        result = 31 * result + (this.drawInterior ? 1 : 0);
        result = 31 * result + (this.drawOutline ? 1 : 0);
        result = 31 * result + (this.enableAntialiasing ? 1 : 0);
        result = 31 * result + (this.enableLighting ? 1 : 0);
        result = 31 * result + (this.interiorMaterial != null ? this.interiorMaterial.GetHashCode() : 0);
        result = 31 * result + (this.outlineMaterial != null ? this.outlineMaterial.GetHashCode() : 0);
        temp = this.interiorOpacity != +0.0d ? BitConverter.DoubleToInt64Bits(this.interiorOpacity) : 0L;
        result = 31 * result + (int) (temp ^ (temp >>> 32));
        temp = this.outlineOpacity != +0.0d ? BitConverter.DoubleToInt64Bits(this.outlineOpacity) : 0L;
        result = 31 * result + (int) (temp ^ (temp >>> 32));
        temp = this.outlineWidth != +0.0d ? BitConverter.DoubleToInt64Bits(this.outlineWidth) : 0L;
        result = 31 * result + (int) (temp ^ (temp >>> 32));
        result = 31 * result + this.outlineStippleFactor;
        result = 31 * result + (int) this.outlineStipplePattern;
        result = 31 * result + (this.imageSource != null ? this.imageSource.GetHashCode() : 0);
        temp = this.imageScale != +0.0d ? BitConverter.DoubleToInt64Bits(this.imageScale) : 0L;
        result = 31 * result + (int) (temp ^ (temp >>> 32));

        return result;
    }

    /** {@inheritDoc} */
    public String isExportFormatSupported(String mimeType)
    {
        if (KMLConstants.KML_MIME_TYPE.equalsIgnoreCase(mimeType))
            return Exportable.FORMAT_SUPPORTED;
        else
            return Exportable.FORMAT_NOT_SUPPORTED;
    }

    /** {@inheritDoc} */
    public void export(String mimeType, Object output) throws IOException, NotSupportedException
    {
        if (mimeType == null)
        {
            String message = Logging.getMessage("nullValue.Format");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (output == null)
        {
            String message = Logging.getMessage("nullValue.OutputBufferIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (KMLConstants.KML_MIME_TYPE.equalsIgnoreCase(mimeType))
        {
            try
            {
                exportAsKML(output);
            }
            catch (XMLStreamException e)
            {
                Logging.logger().throwing(GetType().Name, "export", e);
                throw new IOException(e);
            }
        }
        else
        {
            String message = Logging.getMessage("Export.UnsupportedFormat", mimeType);
            Logging.logger().warning(message);
            throw new NotSupportedException(message);
        }
    }

    /**
     * Export the placemark attributes to KML as a {@code <Style>} element. The {@code output} object will receive the
     * data. This object must be one of: java.io.Writer<br/> java.io.OutputStream<br/> javax.xml.stream.XMLStreamWriter
     *
     * @param output Object to receive the generated KML.
     *
     * @throws XMLStreamException If an exception occurs while writing the KML
     * @see #export(String, Object)
     */
    protected void exportAsKML(Object output) throws XMLStreamException
    {
        XMLStreamWriter xmlWriter = null;
        XMLOutputFactory factory = XMLOutputFactory.newInstance();
        bool closeWriterWhenFinished = true;

        if (output is XMLStreamWriter)
        {
            xmlWriter = (XMLStreamWriter) output;
            closeWriterWhenFinished = false;
        }
        else if (output is Writer)
        {
            xmlWriter = factory.createXMLStreamWriter((Writer) output);
        }
        else if (output is OutputStream)
        {
            xmlWriter = factory.createXMLStreamWriter((OutputStream) output);
        }

        if (xmlWriter == null)
        {
            String message = Logging.getMessage("Export.UnsupportedOutputObject");
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        xmlWriter.writeStartElement("Style");

        // Line style
        xmlWriter.writeStartElement("LineStyle");

        final Color lineColor = this.getOutlineMaterial().getDiffuse();
        if (lineColor != null)
        {
            xmlWriter.writeStartElement("color");
            xmlWriter.writeCharacters(KMLExportUtil.stripHexPrefix(WWUtil.encodeColorABGR(lineColor)));
            xmlWriter.writeEndElement();

            xmlWriter.writeStartElement("colorMode");
            xmlWriter.writeCharacters("normal");
            xmlWriter.writeEndElement();
        }

        final Double lineWidth = this.getOutlineWidth();
        if (lineWidth != null)
        {
            xmlWriter.writeStartElement("width");
            xmlWriter.writeCharacters(Double.ToString(lineWidth));
            xmlWriter.writeEndElement();
        }

        xmlWriter.writeEndElement(); // LineStyle

        // Poly style
        xmlWriter.writeStartElement("PolyStyle");

        final Color fillColor = this.getInteriorMaterial().getDiffuse();
        if (fillColor != null)
        {
            xmlWriter.writeStartElement("color");
            xmlWriter.writeCharacters(KMLExportUtil.stripHexPrefix(WWUtil.encodeColorABGR(fillColor)));
            xmlWriter.writeEndElement();

            xmlWriter.writeStartElement("colorMode");
            xmlWriter.writeCharacters("normal");
            xmlWriter.writeEndElement();
        }

        xmlWriter.writeStartElement("fill");
        xmlWriter.writeCharacters(kmlBoolean(isDrawInterior()));
        xmlWriter.writeEndElement();

        xmlWriter.writeStartElement("outline");
        xmlWriter.writeCharacters(kmlBoolean(isDrawOutline()));
        xmlWriter.writeEndElement();

        xmlWriter.writeEndElement(); // PolyStyle
        xmlWriter.writeEndElement(); // Style

        xmlWriter.flush();
        if (closeWriterWhenFinished)
            xmlWriter.close();
    }
}
}
