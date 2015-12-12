/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util.xml;
using SharpEarth.util;
using SharpEarth.ogc.collada.impl.ColladaTraversalContext;
using SharpEarth.geom.Box;
namespace SharpEarth.ogc.collada{


/**
 * Base class for COLLADA parser classes.
 *
 * @author pabercrombie
 * @version $Id: ColladaAbstractObject.java 1696 2013-10-31 18:46:55Z tgaskins $
 */
public abstract class ColladaAbstractObject : AbstractXMLEventParser
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    protected ColladaAbstractObject(String namespaceURI)
    {
        super(namespaceURI);
    }

    /** {@inheritDoc} Overridden to return ColladaRoot instead of a XMLEventParser. */
    @Override
    public ColladaRoot getRoot()
    {
        XMLEventParser root = super.getRoot();
        return root is ColladaRoot ? (ColladaRoot) root : null;
    }

    /**
     * Returns this renderable's model coordinate extent.
     *
     * @param tc The traversal context to use when determining the extent.
     * @return The model coordinate extent.
     *
     * @throws ArgumentException if either the traversal context is null.
     */
    public Box getLocalExtent(ColladaTraversalContext tc)
    {
        if (tc == null)
        {
            String message = Logging.getMessage("nullValue.TraversalContextIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return null;
    }
}
}
