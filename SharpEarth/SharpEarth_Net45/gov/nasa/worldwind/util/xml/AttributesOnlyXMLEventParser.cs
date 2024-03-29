/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.util.xml{

/**
 * @author tag
 * @version $Id: AttributesOnlyXMLEventParser.java 1981 2014-05-08 03:59:04Z tgaskins $
 */
public class AttributesOnlyXMLEventParser : AbstractXMLEventParser
{
    public AttributesOnlyXMLEventParser()
    {
    }

    public AttributesOnlyXMLEventParser(string namespaceURI)
      : base (namespaceURI)
    {
    }
  }
}
