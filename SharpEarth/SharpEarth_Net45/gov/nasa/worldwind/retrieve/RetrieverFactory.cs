/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.avlist.AVList;
namespace SharpEarth.retrieve{


/**
 * @author tag
 * @version $Id: RetrieverFactory.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface RetrieverFactory
{
    Retriever createRetriever(AVList parameters, RetrievalPostProcessor postProcessor);
}
}
