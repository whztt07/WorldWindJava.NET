/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.retrieve;
namespace SharpEarth.events{


/**
 * Notifies of bulk retrieval events.
 *
 * @author tag
 * @version $Id: BulkRetrievalEvent.java 1171 2013-02-11 21:45:02Z dcollins $
 * @see SharpEarth.retrieve.BulkRetrievable
 */
public class BulkRetrievalEvent : WWEvent
{
    /** Constant indicating retrieval failure. */
    public static readonly string RETRIEVAL_FAILED = "gov.nasa.worldwind.retrieve.BulkRetrievable.RetrievalFailed";

    /** Constant indicating retrieval success. */
    public static readonly string RETRIEVAL_SUCCEEDED = "gov.nasa.worldwind.retrieve.BulkRetrievable.RetrievalSucceeded";

    protected string eventType;
    protected string item;

    /**
     * Creates a new event.
     *
     * @param source    the event source, typically either a tiled image layer, elevation model or placename layer.
     * @param eventType indicates success or failure. One of {@link #RETRIEVAL_SUCCEEDED} or {@link #RETRIEVAL_FAILED}.
     * @param item      the cache location of the item whose retrieval succeeded or failed.
     *
     * @see SharpEarth.retrieve.BulkRetrievable
     */
    public BulkRetrievalEvent(BulkRetrievable source, string eventType, string item ) : base(source)
    {

        this.eventType = eventType;
        this.item = item;
    }

    /**
     * Returns the event source.
     *
     * @return the event source, typically either a tiled image layer, elevation model or placename layer.
     *
     * @see SharpEarth.retrieve.BulkRetrievable
     */
    public BulkRetrievable getSource()
    {
      return base.getSource() as BulkRetrievable;
    }

    /**
     * Returns the event type, one of {@link #RETRIEVAL_SUCCEEDED} or {@link #RETRIEVAL_FAILED}.
     *
     * @return the event type.
     */
    public string getEventType()
    {
        return eventType;
    }

    /**
     * Returns the filestore location of the item whose retrieval succeeded or failed.
     *
     * @return the filestore location of the item.
     */
    public string getItem()
    {
        return item;
    }
}
}
