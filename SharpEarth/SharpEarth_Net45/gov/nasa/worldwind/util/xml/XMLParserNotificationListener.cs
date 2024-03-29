/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.util.xml {

/**
 * The interface that receives {@link SharpEarth.util.xml.XMLEventParserContext} notifications.
 *
 * @author tag
 * @version $Id: XMLParserNotificationListener.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface XMLParserNotificationListener
{
    /**
     * Receives notification events from the parser context.
     *
     * @param notification the notification object containing the notificaton type and data.
     */
    void notify(XMLParserNotification notification);
  }
}
