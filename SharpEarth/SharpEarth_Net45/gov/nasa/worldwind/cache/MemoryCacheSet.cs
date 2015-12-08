/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using java.util;
using SharpEarth.util;
namespace SharpEarth.cache{



/**
 * @author tag
 * @version $Id: MemoryCacheSet.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface MemoryCacheSet
{
    bool containsCache(string key);

    MemoryCache getCache(string cacheKey);

    MemoryCache addCache(string key, MemoryCache cache);

    IEnumerable<PerformanceStatistic> getPerformanceStatistics();

    void clear();

    IDictionary<string, MemoryCache> getAllCaches();
}
}
