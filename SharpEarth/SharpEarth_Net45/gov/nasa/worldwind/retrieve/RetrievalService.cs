/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.WWObject;
namespace SharpEarth.retrieve{


/**
 * @author Tom Gaskins
 * @version $Id: RetrievalService.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface RetrievalService : WWObject
{
    RetrievalFuture runRetriever(Retriever retriever);

    RetrievalFuture runRetriever(Retriever retriever, double priority);

    void setRetrieverPoolSize(int poolSize);

    int getRetrieverPoolSize();

    bool hasActiveTasks();

    bool isAvailable();

    bool contains(Retriever retriever);

    int getNumRetrieversPending();

    void shutdown(boolean immediately);

    public interface SSLExceptionListener
    {
        void onException(Throwable e, String path);
    }

    /**
     * Specifies the listener called when a {@link javax.net.ssl.SSLHandshakeException} is thrown during resource
     * retrieval.
     *
     * @param listener to listener to invoke, or null if no listener is to be invoked.
     */
    void setSSLExceptionListener(SSLExceptionListener listener);

    /**
     * Indicates the listener to be called when {@link javax.net.ssl.SSLHandshakeException}s are thrown during resource
     * retrieval.
     *
     * @return the exception listener, or null if no listener has been specified.
     */
    SSLExceptionListener getSSLExceptionListener();
}
}
