/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.EventListener;
namespace SharpEarth.events{


/**
 * Interface for listening for bulk-download events.
 *
 * @author tag
 * @version $Id: BulkRetrievalListener.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface BulkRetrievalListener extends EventListener
{
    /**
     * A bulk-download event occurred, either a succes, a failure or an extended event.
     *
     * @param event the event that occurred.
     * @see SharpEarth.retrieve.BulkRetrievable 
     */
    void eventOccurred(BulkRetrievalEvent event);
}
}
