/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util.Arrays;
using java.io.IOException;
using SharpEarth.util;
using SharpEarth.geom.Sector;
using SharpEarth.avlist;
namespace SharpEarth.data{



/**
 * Abstract base class for most {@link DataRasterReader} implementations.
 *
 * @author dcollins
 * @version $Id: AbstractDataRasterReader.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public abstract class AbstractDataRasterReader extends AVListImpl implements DataRasterReader
{
    protected abstract bool doCanRead(Object source, AVList parameters);

    protected abstract DataRaster[] doRead(Object source, AVList parameters) throws java.io.IOException;

    protected abstract void doReadMetadata(Object source, AVList parameters) throws java.io.IOException;

    protected final String description;
    protected final String[] mimeTypes;
    protected final String[] suffixes;

    public AbstractDataRasterReader(String description, String[] mimeTypes, String[] suffixes)
    {
        this.description = description;
        this.mimeTypes = Arrays.copyOf(mimeTypes, mimeTypes.length);
        this.suffixes = Arrays.copyOf(suffixes, suffixes.length);

        this.setValue(AVKey.SERVICE_NAME, AVKey.SERVICE_NAME_OFFLINE);
    }

    public AbstractDataRasterReader(String[] mimeTypes, String[] suffixes)
    {
        this(descriptionFromSuffixes(suffixes), mimeTypes, suffixes);
    }

    protected AbstractDataRasterReader(String description)
    {
        this(description, new String[0], new String[0]);
    }

    /** {@inheritDoc} */
    public String getDescription()
    {
        return this.description;
    }

    public String[] getMimeTypes()
    {
        String[] copy = new String[mimeTypes.length];
        System.arraycopy(mimeTypes, 0, copy, 0, mimeTypes.length);
        return copy;
    }

    /** {@inheritDoc} */
    public String[] getSuffixes()
    {
        String[] copy = new String[suffixes.length];
        System.arraycopy(suffixes, 0, copy, 0, suffixes.length);
        return copy;
    }

    /** {@inheritDoc} */
    public bool canRead(Object source, AVList parameters)
    {
        if (source == null)
            return false;

        //noinspection SimplifiableIfStatement
        if (!this.canReadSuffix(source))
            return false;

        return this.doCanRead(source, parameters);
    }

    protected bool canReadSuffix(Object source)
    {
        // If the source has no path, we cannot return failure, so return that the test passed.
        String path = WWIO.getSourcePath(source);
        if (path == null)
            return true;

        // If the source has a suffix, then we return success if this reader supports the suffix.
        String pathSuffix = WWIO.getSuffix(path);
        bool matchesAny = false;
        foreach (String suffix  in  suffixes)
        {
            if (suffix.equalsIgnoreCase(pathSuffix))
            {
                matchesAny = true;
                break;
            }
        }
        return matchesAny;
    }

    /** {@inheritDoc} */
    public DataRaster[] read(Object source, AVList parameters) throws java.io.IOException
    {
        if (!this.canRead(source, parameters))
        {
            String message = Logging.getMessage("DataRaster.CannotRead", source);
            Logging.logger().severe(message);
            throw new java.io.IOException(message);
        }

        return this.doRead(source, parameters);
    }

    /** {@inheritDoc} */
    public AVList readMetadata(Object source, AVList parameters) throws java.io.IOException
    {
        if (!this.canRead(source, parameters))
        {
            String message = Logging.getMessage("DataRaster.CannotRead", source);
            throw new ArgumentException(message);
        }

        if (params == null)
            parameters = new AVListImpl();

        this.doReadMetadata(source, parameters);

        String message = this.validateMetadata(source, parameters);
        if (message != null)
            throw new java.io.IOException(message);

        return parameters;
    }

    protected String validateMetadata(Object source, AVList parameters)
    {
        StringBuilder sb = new StringBuilder();

        Object o = (params != null) ? parameters.getValue(AVKey.WIDTH) : null;
        if (o == null || !(o is Integer))
            sb.append(sb.length() > 0 ? ", " : "").append(Logging.getMessage("WorldFile.NoSizeSpecified", source));

        o = (params != null) ? parameters.getValue(AVKey.HEIGHT) : null;
        if (o == null || !(o is Integer))
            sb.append(sb.length() > 0 ? ", " : "").append(Logging.getMessage("WorldFile.NoSizeSpecified", source));

        o = (params != null) ? parameters.getValue(AVKey.SECTOR) : null;
        if (o == null || !(o is Sector))
            sb.append(sb.length() > 0 ? ", " : "").append(Logging.getMessage("WorldFile.NoSectorSpecified", source));

        if (sb.length() == 0)
            return null;

        return sb.ToString();
    }

    /** {@inheritDoc} */
    public bool isImageryRaster(Object source, AVList parameters)
    {
        if (params != null && AVKey.IMAGE.Equals(params.getStringValue(AVKey.PIXEL_FORMAT)))
            return true;

        try
        {
            AVList metadata = this.readMetadata(source, parameters);
            return metadata != null && AVKey.IMAGE.Equals(metadata.getStringValue(AVKey.PIXEL_FORMAT));
        }
        catch (IOException e)
        {
            return false;
        }
    }

    /** {@inheritDoc} */
    public bool isElevationsRaster(Object source, AVList parameters)
    {
        if (params != null && AVKey.ELEVATION.Equals(params.getStringValue(AVKey.PIXEL_FORMAT)))
            return true;

        try
        {
            AVList metadata = this.readMetadata(source, parameters);
            return metadata != null && AVKey.ELEVATION.Equals(metadata.getStringValue(AVKey.PIXEL_FORMAT));
        }
        catch (IOException e)
        {
            return false;
        }
    }

    //**************************************************************//
    //********************  Utilities  *****************************//
    //**************************************************************//

    private static String descriptionFromSuffixes(String[] suffixes)
    {
        StringBuilder sb = new StringBuilder();
        foreach (String suffix  in  suffixes)
        {
            if (sb.length() > 0)
                sb.append(", ");
            sb.append("*.").append(suffix.toLowerCase());
        }
        return sb.ToString();
    }
}
}
