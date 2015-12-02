/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util;
using SharpEarth.util.PerformanceStatistic;
namespace SharpEarth.cache{



/**
 * @author tag
 * @version $Id: MemoryCacheSet.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface MemoryCacheSet
{
    bool containsCache(String key);

    MemoryCache getCache(String cacheKey);

    MemoryCache addCache(String key, MemoryCache cache);

    Collection<PerformanceStatistic> getPerformanceStatistics();

    void clear();

    Map<String, MemoryCache> getAllCaches();
}
}
