/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using SharpEarth.render;
using SharpEarth.avlist;
using SharpEarth.cache;
using java.beans;
using SharpEarth.java.lang;
using SharpEarth.geom;
using System.Drawing;
using SharpEarth.java.org.w3c.dom;
using System;
using SharpEarth.javax.xml.xpath;

namespace SharpEarth.layers{



/**
 * @author tag
 * @version $Id: AbstractLayer.java 2254 2014-08-22 17:02:46Z tgaskins $
 */
public abstract class AbstractLayer : WWObjectImpl, Layer
{
    private bool enabled = true;
    private bool pickable = true;
    private double opacity = 1d;
    private double minActiveAltitude = -double.MaxValue;
    private double maxActiveAltitude = double.MaxValue;
    private bool networkDownloadEnabled = true;
    private long expiryTime = 0;
    private ScreenCredit screenCredit = null;
    private FileStore dataFileStore = WorldWind.getDataFileStore();

    public bool isEnabled()
    {
        return this.enabled;
    }

    public bool isPickEnabled()
    {
        return pickable;
    }

    public void setPickEnabled(bool pickable)
    {
        this.pickable = pickable;
    }

    public void setEnabled(bool enabled)
    {
        bool oldEnabled = this.enabled;
        this.enabled = enabled;
        this.propertyChange(new PropertyChangeEvent(this, "Enabled", oldEnabled, this.enabled));
    }

    public string getName()
    {
        object n = this.getValue(AVKey.DISPLAY_NAME);

        return n != null ? n.ToString() : this.ToString();
    }

    public void setName(string name)
    {
        this.setValue(AVKey.DISPLAY_NAME, name);
    }

    public override string ToString()
    {
        object n = this.getValue(AVKey.DISPLAY_NAME);

        return n != null ? n.ToString() : base.ToString();
    }

    public double getOpacity()
    {
        return opacity;
    }

    public void setOpacity(double opacity)
    {
        this.opacity = opacity;
    }

    public double getMinActiveAltitude()
    {
        return minActiveAltitude;
    }

    public void setMinActiveAltitude(double minActiveAltitude)
    {
        this.minActiveAltitude = minActiveAltitude;
    }

    public double getMaxActiveAltitude()
    {
        return maxActiveAltitude;
    }

    public void setMaxActiveAltitude(double maxActiveAltitude)
    {
        this.maxActiveAltitude = maxActiveAltitude;
    }

    public double? getMinEffectiveAltitude(double radius)
    {
        return null;
    }

    public double? getMaxEffectiveAltitude(double radius)
    {
        return null;
    }

    public double getScale()
    {
        object o = this.getValue(AVKey.MAP_SCALE);
        return o != null && o is double ? (double) o : 1;
    }

    public bool isNetworkRetrievalEnabled()
    {
        return networkDownloadEnabled;
    }

    public void setNetworkRetrievalEnabled(bool networkDownloadEnabled)
    {
        this.networkDownloadEnabled = networkDownloadEnabled;
    }

    public FileStore getDataFileStore()
    {
        return this.dataFileStore;
    }

    public void setDataFileStore(FileStore fileStore)
    {
        if (fileStore == null)
        {
            string message = Logging.getMessage("nullValue.FileStoreIsNull");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        this.dataFileStore = fileStore;
    }

    public bool isLayerInView(DrawContext dc)
    {
        if (dc == null)
        {
            string message = Logging.getMessage("nullValue.DrawContextIsNull");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        return true;
    }

    public bool isLayerActive(DrawContext dc)
    {
        if (dc == null)
        {
            string message = Logging.getMessage("nullValue.DrawContextIsNull");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (null == dc.getView())
        {
        string message = Logging.getMessage("layers.AbstractLayer.NoViewSpecifiedInDrawingContext");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        Position eyePos = dc.getView().getEyePosition();
        if (eyePos == null)
            return false;

        double altitude = eyePos.getElevation();
        return altitude >= this.minActiveAltitude && altitude <= this.maxActiveAltitude;
    }

    public void preRender(DrawContext dc)
    {
        if (!this.enabled)
            return; // Don't check for arg errors if we're disabled

        if (null == dc)
        {
            string message = Logging.getMessage("nullValue.DrawContextIsNull");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (null == dc.getGlobe())
        {
            string message = Logging.getMessage("layers.AbstractLayer.NoGlobeSpecifiedInDrawingContext");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (null == dc.getView())
        {
            string message = Logging.getMessage("layers.AbstractLayer.NoViewSpecifiedInDrawingContext");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (!this.isLayerActive(dc))
            return;

        if (!this.isLayerInView(dc))
            return;

        this.doPreRender(dc);
    }

    /**
     * @param dc the current draw context
     *
     * @throws ArgumentException if <code>dc</code> is null, or <code>dc</code>'s <code>Globe</code> or
     *                                  <code>View</code> is null
     */
    public void render(DrawContext dc)
    {
        if (!this.enabled)
            return; // Don't check for arg errors if we're disabled

        if (null == dc)
        {
            string message = Logging.getMessage("nullValue.DrawContextIsNull");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (null == dc.getGlobe())
        {
            string message = Logging.getMessage("layers.AbstractLayer.NoGlobeSpecifiedInDrawingContext");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (null == dc.getView())
        {
            string message = Logging.getMessage("layers.AbstractLayer.NoViewSpecifiedInDrawingContext");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (!this.isLayerActive(dc))
            return;

        if (!this.isLayerInView(dc))
            return;

        this.doRender(dc);
    }

    public void pick(DrawContext dc, Point point)
    {
        if (!this.enabled)
            return; // Don't check for arg errors if we're disabled

        if (null == dc)
        {
            string message = Logging.getMessage("nullValue.DrawContextIsNull");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (null == dc.getGlobe())
        {
            string message = Logging.getMessage("layers.AbstractLayer.NoGlobeSpecifiedInDrawingContext");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (null == dc.getView())
        {
            string message = Logging.getMessage("layers.AbstractLayer.NoViewSpecifiedInDrawingContext");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (!this.isLayerActive(dc))
            return;

        if (!this.isLayerInView(dc))
            return;

        this.doPick(dc, point);
    }

    protected void doPick(DrawContext dc, Point point)
    {
        // any state that could change the color needs to be disabled, such as GL_TEXTURE, GL_LIGHTING or GL_FOG.
        // re-draw with unique colors
        // store the object info in the selectable objects table
        // read the color under the cursor
        // use the color code as a key to retrieve a selected object from the selectable objects table
        // create an instance of the PickedObject and add to the dc via the dc.addPickedObject() method
    }

    public void dispose() // override if disposal is a supported operation
    {
    }

    protected void doPreRender(DrawContext dc)
    {
    }

    protected abstract void doRender(DrawContext dc);

    public bool isAtMaxResolution()
    {
        return !this.isMultiResolution();
    }

    public bool isMultiResolution()
    {
        return false;
    }

    public string getRestorableState()
    {
        return null;
    }

    public void restoreState(string stateInXml)
    {
        string message = Logging.getMessage("RestorableSupport.RestoreNotSupported");
        Logging.logger().severe(message);
        throw new UnsupportedOperationException(message);
    }

    public void setExpiryTime(long expiryTime)
    {
        this.expiryTime = expiryTime;
    }

    public long getExpiryTime()
    {
        return this.expiryTime;
    }

    protected ScreenCredit getScreenCredit()
    {
        return screenCredit;
    }

    protected void setScreenCredit(ScreenCredit screenCredit)
    {
        this.screenCredit = screenCredit;
    }

    //**************************************************************//
    //********************  Configuration  *************************//
    //**************************************************************//

    /**
     * Returns true if a specified DOM document is a Layer configuration document, and false otherwise.
     *
     * @param domElement the DOM document in question.
     *
     * @return true if the document is a Layer configuration document; false otherwise.
     *
     * @throws ArgumentException if document is null.
     */
    public static bool isLayerConfigDocument(Element domElement)
    {
        if (domElement == null)
        {
            string message = Logging.getMessage("nullValue.DocumentIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        XPath xpath = WWXML.makeXPath();
        Element[] elements = WWXML.getElements(domElement, "//Layer", xpath);

        return elements != null && elements.Length > 0;
    }

    /**
     * Appends layer configuration parameters as elements to the specified context. This appends elements for the
     * following parameters: <table> <tr><th>Parameter</th><th>Element Path</th><th>Type</th></tr> <tr><td>{@link
     * AVKey#DISPLAY_NAME}</td><td>DisplayName</td><td>string</td></tr> <tr><td>{@link
     * AVKey#OPACITY}</td><td>Opacity</td><td>Double</td></tr> <tr><td>{@link AVKey#MAX_ACTIVE_ALTITUDE}</td><td>ActiveAltitudes/@max</td><td>Double</td></tr>
     * <tr><td>{@link AVKey#MIN_ACTIVE_ALTITUDE}</td><td>ActiveAltitudes/@min</td><td>Double</td></tr> <tr><td>{@link
     * AVKey#NETWORK_RETRIEVAL_ENABLED}</td><td>NetworkRetrievalEnabled</td><td>Boolean</td></tr> <tr><td>{@link
     * AVKey#MAP_SCALE}</td><td>MapScale</td><td>Double</td></tr> <tr><td>{@link AVKey#SCREEN_CREDIT}</td><td>ScreenCredit</td><td>ScreenCredit</td></tr>
     * </table>
     *
     * @param parameters  the key-value pairs which define the layer configuration parameters.
     * @param context the XML document root on which to append layer configuration elements.
     *
     * @return a reference to context.
     *
     * @throws ArgumentException if either the parameters or the context are null.
     */
    public static Element createLayerConfigElements(AVList parameters, Element context)
    {
        if (parameters == null)
        {
            string message = Logging.getMessage("nullValue.ParametersIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (context == null)
        {
            string message = Logging.getMessage("nullValue.ContextIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        WWXML.checkAndAppendTextElement(parameters, AVKey.DISPLAY_NAME, context, "DisplayName");
        WWXML.checkAndAppendDoubleElement(parameters, AVKey.OPACITY, context, "Opacity");

        double maxAlt = AVListImpl.getDoubleValue(parameters, AVKey.MAX_ACTIVE_ALTITUDE);
      double minAlt = AVListImpl.getDoubleValue(parameters, AVKey.MIN_ACTIVE_ALTITUDE);
        if (maxAlt != null || minAlt != null)
        {
            Element el = WWXML.appendElementPath(context, "ActiveAltitudes");
            if (maxAlt != null)
                WWXML.setDoubleAttribute(el, "max", maxAlt);
            if (minAlt != null)
                WWXML.setDoubleAttribute(el, "min", minAlt);
        }

        WWXML.checkAndAppendBooleanElement(parameters, AVKey.NETWORK_RETRIEVAL_ENABLED, context, "NetworkRetrievalEnabled");
        WWXML.checkAndAppendDoubleElement(parameters, AVKey.MAP_SCALE, context, "MapScale");
        WWXML.checkAndAppendScreenCreditElement(parameters, AVKey.SCREEN_CREDIT, context, "ScreenCredit");
        WWXML.checkAndAppendBooleanElement(parameters, AVKey.PICK_ENABLED, context, "PickEnabled");

        return context;
    }

    /**
     * Parses layer configuration parameters from the specified DOM document. This writes output as key-value pairs to
     * parameters. If a parameter from the XML document already exists in parameters, that parameter is ignored. Supported key
     * and parameter names are: <table> <tr><th>Parameter</th><th>Element Path</th><th>Type</th></tr> <tr><td>{@link
     * AVKey#DISPLAY_NAME}</td><td>DisplayName</td><td>string</td></tr> <tr><td>{@link
     * AVKey#OPACITY}</td><td>Opacity</td><td>Double</td></tr> <tr><td>{@link AVKey#MAX_ACTIVE_ALTITUDE}</td><td>ActiveAltitudes/@max</td><td>Double</td></tr>
     * <tr><td>{@link AVKey#MIN_ACTIVE_ALTITUDE}</td><td>ActiveAltitudes/@min</td><td>Double</td></tr> <tr><td>{@link
     * AVKey#NETWORK_RETRIEVAL_ENABLED}</td><td>NetworkRetrievalEnabled</td><td>Boolean</td></tr> <tr><td>{@link
     * AVKey#MAP_SCALE}</td><td>MapScale</td><td>Double</td></tr> <tr><td>{@link AVKey#SCREEN_CREDIT}</td><td>ScreenCredit</td><td>{@link
     * ScreenCredit}</td></tr> </table>
     *
     * @param domElement the XML document root to parse for layer configuration elements.
     * @param parameters     the output key-value pairs which recieve the layer configuration parameters. A null reference
     *                   is permitted.
     *
     * @return a reference to parameters, or a new AVList if parameters is null.
     *
     * @throws ArgumentException if the document is null.
     */
    public static AVList getLayerConfigParams(Element domElement, AVList parameters)
    {
        if (domElement == null)
        {
            string message = Logging.getMessage("nullValue.DocumentIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (parameters == null)
            parameters = new AVListImpl();

        XPath xpath = WWXML.makeXPath();

        WWXML.checkAndSetStringParam(domElement, parameters, AVKey.DISPLAY_NAME, "DisplayName", xpath);
        WWXML.checkAndSetDoubleParam(domElement, parameters, AVKey.OPACITY, "Opacity", xpath);
        WWXML.checkAndSetDoubleParam(domElement, parameters, AVKey.MAX_ACTIVE_ALTITUDE, "ActiveAltitudes/@max", xpath);
        WWXML.checkAndSetDoubleParam(domElement, parameters, AVKey.MIN_ACTIVE_ALTITUDE, "ActiveAltitudes/@min", xpath);
        WWXML.checkAndSetBooleanParam(domElement, parameters, AVKey.NETWORK_RETRIEVAL_ENABLED, "NetworkRetrievalEnabled",
            xpath);
        WWXML.checkAndSetDoubleParam(domElement, parameters, AVKey.MAP_SCALE, "MapScale", xpath);
        WWXML.checkAndSetScreenCreditParam(domElement, parameters, AVKey.SCREEN_CREDIT, "ScreenCredit", xpath);
        WWXML.checkAndSetIntegerParam(domElement, parameters, AVKey.MAX_ABSENT_TILE_ATTEMPTS, "MaxAbsentTileAttempts",
            xpath);
        WWXML.checkAndSetIntegerParam(domElement, parameters, AVKey.MIN_ABSENT_TILE_CHECK_INTERVAL,
            "MinAbsentTileCheckInterval", xpath);
        WWXML.checkAndSetBooleanParam(domElement, parameters, AVKey.PICK_ENABLED, "PickEnabled", xpath);

        return parameters;
    }
}
}
