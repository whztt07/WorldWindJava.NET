/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.ogc.collada{

/**
 * Parser class for COLLADA elements that are not used by World Wind.
 *
 * @author pabercrombie
 * @version $Id: ColladaUnsupported.java 642 2012-06-14 17:31:29Z pabercrombie $
 */
public class ColladaUnsupported : ColladaAbstractObject
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public ColladaUnsupported(String namespaceURI)
    {
        super(namespaceURI);
    }
}
}
