/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util;
namespace SharpEarth.util{


public class PerformanceStatistic : Comparable<PerformanceStatistic>
{
    public static final String ALL = "gov.nasa.worldwind.perfstat.All";
    /** @deprecated Airspace geometry count is no longer logged during airspace rendering. */
    public static final String AIRSPACE_GEOMETRY_COUNT = "gov.nasa.worldwind.perfstat.AirspaceGeometryCount";
    /** @deprecated Airspace vertex count is no longer logged during airspace rendering. */
    public static final String AIRSPACE_VERTEX_COUNT = "gov.nasa.worldwind.perfstat.AirspaceVertexCount";
    public static final String FRAME_RATE = "gov.nasa.worldwind.perfstat.FrameRate";
    public static final String FRAME_TIME = "gov.nasa.worldwind.perfstat.FrameTime";
    public static final String IMAGE_TILE_COUNT = "gov.nasa.worldwind.perfstat.ImageTileCount";
    public static final String TERRAIN_TILE_COUNT = "gov.nasa.worldwind.perfstat.TerrainTileCount";
    public static final String MEMORY_CACHE = "gov.nasa.worldwind.perfstat.MemoryCache";
    public static final String PICK_TIME = "gov.nasa.worldwind.perfstat.PickTime";
    public static final String JVM_HEAP = "gov.nasa.worldwind.perfstat.JvmHeap";
    public static final String JVM_HEAP_USED = "gov.nasa.worldwind.perfstat.JvmHeapUsed";
    public static final String TEXTURE_CACHE = "gov.nasa.worldwind.perfstat.TextureCache";

    public static final Set<String> ALL_STATISTICS_SET = new HashSet<String>(1);
    static
    {
        ALL_STATISTICS_SET.add(PerformanceStatistic.ALL);
    }

    private final String key;
    private final String displayString;
    private final Object value;

    public PerformanceStatistic(String key, String displayString, Object value)
    {
        this.key = key;
        this.displayString = displayString;
        this.value = value;
    }

    public String getKey()
    {
        return key;
    }

    public String getDisplayString()
    {
        return displayString;
    }

    public Object getValue()
    {
        return value;
    }

    public int compareTo(PerformanceStatistic that)
    {
        //noinspection StringEquality
        if (this.displayString == that.displayString)
            return 0;

        if (this.displayString != null && that.displayString != null)
            return this.displayString.compareTo(that.displayString);

        return this.displayString == null ? -1 : 1;
    }

    public override bool Equals(Object o)
    {
        if (this == o)
            return true;
        if (o == null || GetType() != o.GetType())
            return false;

        PerformanceStatistic that = (PerformanceStatistic) o;

        if (displayString != null ? !displayString.Equals(that.displayString) : that.displayString != null)
            return false;
        if (key != null ? !key.Equals(that.key) : that.key != null)
            return false;
        //noinspection RedundantIfStatement
        if (value != null ? !value.Equals(that.value) : that.value != null)
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        int result;
        result = (key != null ? key.GetHashCode() : 0);
        result = 31 * result + (displayString != null ? displayString.GetHashCode() : 0);
        result = 31 * result + (value != null ? value.GetHashCode() : 0);
        return result;
    }

    public override string ToString()
    {
        return this.displayString + " " + this.value.ToString();
    }
}
}
