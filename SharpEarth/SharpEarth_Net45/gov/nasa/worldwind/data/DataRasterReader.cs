/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.avlist.AVList;
namespace SharpEarth.data{


// DataRasterReader is the interface class for all data raster readers implementations
/**
 * @author dcollins
 * @version $Id: DataRasterReader.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface DataRasterReader : AVList
{
    String getDescription(); // TODO: remove

    String[] getSuffixes(); // TODO: remove

    /**
     * Indicates whether this reader can read a specified data source.
     * The source may be one of the following:
     * <ul>
     *     <li>{@link java.io.File}</li>
     *     <li>{@link String}</li> </ul>
     *     <li>{@link java.io.InputStream}</li>
     *     <li>{@link java.net.URL}</li>
     * <ul/>
     *
     * @param source the source to examine.
     * @param parameters parameters required by certain reader implementations. May be null for most readers.
     *
     * @return true if this reader can read the data source, otherwise false.
     */
    bool canRead(Object source, AVList parameters);

    /**
     * Reads and returns the DataRaster instances from a data source.
     * The source may be one of the following:
     * The source may be one of the following:
     * <ul>
     *     <li>{@link java.io.File}</li>
     *     <li>{@link String}</li> </ul>
     *     <li>{@link java.io.InputStream}</li>
     *     <li>{@link java.net.URL}</li>
     * <ul/>
     *
     * @param source the source to read.
     * @param parameters parameters required by certain reader implementations. May be null for most readers. If non-null,
     *               the metadata is added to this list, and the list reference is the return value of this method.
     *
     * @return the list of metadata read from the data source. The list is empty if the data source has no metadata.
     *
     * @throws java.io.IOException if an IO error occurs.
     */
    DataRaster[] read(Object source, AVList parameters) throws java.io.IOException;

    /**
     * Reads and returns the metadata from a data source.
     * The source may be one of the following:
     * <ul>
     *     <li>{@link java.io.File}</li>
     *     <li>{@link String}</li> </ul>
     *     <li>{@link java.io.InputStream}</li>
     *     <li>{@link java.net.URL}</li>
     * <ul/>
     *
     * TODO: Why would the caller specify parameters to this method?
     *
     * @param source the source to examine.
     * @param parameters parameters required by certain reader implementations. May be null for most readers. If non-null,
     *               the metadata is added to this list, and the list reference is the return value of this method.
     *
     * @return the list of metadata read from the data source. The list is empty if the data source has no metadata.
     *
     * @throws java.io.IOException if an IO error occurs.
     */
    AVList readMetadata(Object source, AVList parameters) throws java.io.IOException;

    /**
     * Indicates whether a data source is imagery.
     * TODO: Identify when parameters must be passed.
     * The source may be one of the following:
     * <ul>
     *     <li>{@link java.io.File}</li>
     *     <li>{@link String}</li> </ul>
     *     <li>{@link java.io.InputStream}</li>
     *     <li>{@link java.net.URL}</li>
     * <ul/>
     *
     * @param source the source to examine.
     * @param parameters parameters required by certain reader implementations. May be null for most readers.
     *
     * @return true if the source is imagery, otherwise false.
     */
    bool isImageryRaster(Object source, AVList parameters);

    /**
     * Indicates whether a data source is elevation data.
     * TODO: Identify when parameters must be passed.
     *
     * The source may be one of the following:
     * <ul>
     *     <li>{@link java.io.File}</li>
     *     <li>{@link String}</li> </ul>
     *     <li>{@link java.io.InputStream}</li>
     *     <li>{@link java.net.URL}</li>
     * <ul/>
     *
     * @param source the source to examine.
     * @param parameters parameters required by certain reader implementations. May be null for most readers.
     * TODO: Identify when parameters must be passed.
     *
     * @return true if the source is elevation data, otherwise false.
     */
    bool isElevationsRaster(Object source, AVList parameters);
}
}
