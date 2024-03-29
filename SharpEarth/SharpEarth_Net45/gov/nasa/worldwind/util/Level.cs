/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.geom;
using SharpEarth.avlist;
namespace SharpEarth.util{


  /**
   * @author tag
   * @version $Id: Level.java 1171 2013-02-11 21:45:02Z dcollins $
   */
  public class Level
  {
    public static object SEVERE { get; internal set; }
  }
  extends AVListImpl implements Comparable<Level>
{
    protected AVList parameters;
    protected int levelNumber;
    protected String levelName; // null or empty level name signifies no data resources associated with this level
    protected LatLon tileDelta;
    protected int tileWidth;
    protected int tileHeight;
    protected String cacheName;
    protected String service;
    protected String dataset;
    protected String formatSuffix;
    protected double texelSize;
    protected String path;
    protected TileUrlBuilder urlBuilder;
    protected long expiryTime = 0;
    protected bool active = true;

    // Absent tiles: A tile is deemed absent if a specified maximum number of attempts have been made to retrieve it.
    // Retrieval attempts are governed by a minimum time interval between successive attempts. If an attempt is made
    // within this interval, the tile is still deemed to be absent until the interval expires.
    protected AbsentResourceList absentTiles;
    int DEFAULT_MAX_ABSENT_TILE_ATTEMPTS = 2;
    int DEFAULT_MIN_ABSENT_TILE_CHECK_INTERVAL = 10000; // milliseconds

    public Level(AVList parameters)
    {
        if (params == null)
        {
            String message = Logging.getMessage("nullValue.LevelConfigParams");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.params = parameters.copy(); // Private copy to insulate from subsequent changes by the app
        String message = this.validate(params);
        if (message != null)
        {
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        String ln = this.params.getStringValue(AVKey.LEVEL_NAME);
        this.levelName = ln != null ? ln : "";

        this.levelNumber = (Integer) this.params.getValue(AVKey.LEVEL_NUMBER);
        this.tileDelta = (LatLon) this.params.getValue(AVKey.TILE_DELTA);
        this.tileWidth = (Integer) this.params.getValue(AVKey.TILE_WIDTH);
        this.tileHeight = (Integer) this.params.getValue(AVKey.TILE_HEIGHT);
        this.cacheName = this.params.getStringValue(AVKey.DATA_CACHE_NAME);
        this.service = this.params.getStringValue(AVKey.SERVICE);
        this.dataset = this.params.getStringValue(AVKey.DATASET_NAME);
        this.formatSuffix = this.params.getStringValue(AVKey.FORMAT_SUFFIX);
        this.urlBuilder = (TileUrlBuilder) this.params.getValue(AVKey.TILE_URL_BUILDER);
        this.expiryTime = AVListImpl.getLongValue(params, AVKey.EXPIRY_TIME, 0L);

//        double averageTileSize = 0.5 * (this.tileWidth + this.tileHeight);
//        double averageTileDelta =
//            0.5 * (this.tileDelta.getLatitude().getRadians() + this.tileDelta.getLongitude().getRadians());
        this.texelSize = this.tileDelta.getLatitude().getRadians() / this.tileHeight;

        this.path = this.cacheName + "/" + this.levelName;

        Integer maxAbsentTileAttempts = (Integer) this.params.getValue(AVKey.MAX_ABSENT_TILE_ATTEMPTS);
        if (maxAbsentTileAttempts == null)
            maxAbsentTileAttempts = DEFAULT_MAX_ABSENT_TILE_ATTEMPTS;

        Integer minAbsentTileCheckInterval = (Integer) this.params.getValue(AVKey.MIN_ABSENT_TILE_CHECK_INTERVAL);
        if (minAbsentTileCheckInterval == null)
            minAbsentTileCheckInterval = DEFAULT_MIN_ABSENT_TILE_CHECK_INTERVAL;

        this.absentTiles = new AbsentResourceList(maxAbsentTileAttempts, minAbsentTileCheckInterval);
    }

    /**
     * Determines whether the constructor arguments are valid.
     *
     * @param parameters the list of parameters to validate.
     *
     * @return null if valid, otherwise a <code>String</code> containing a description of why it's invalid.
     */
    protected String validate(AVList parameters)
    {
        StringBuffer sb = new StringBuffer();

        Object o = parameters.getValue(AVKey.LEVEL_NUMBER);
        if (o == null || !(o is Integer) || ((Integer) o) < 0)
            sb.append(Logging.getMessage("term.levelNumber")).append(" ");

        o = parameters.getValue(AVKey.LEVEL_NAME);
        if (o == null || !(o is String))
            sb.append(Logging.getMessage("term.levelName")).append(" ");

        o = parameters.getValue(AVKey.TILE_WIDTH);
        if (o == null || !(o is Integer) || ((Integer) o) < 0)
            sb.append(Logging.getMessage("term.tileWidth")).append(" ");

        o = parameters.getValue(AVKey.TILE_HEIGHT);
        if (o == null || !(o is Integer) || ((Integer) o) < 0)
            sb.append(Logging.getMessage("term.tileHeight")).append(" ");

        o = parameters.getValue(AVKey.TILE_DELTA);
        if (o == null || !(o is LatLon))
            sb.append(Logging.getMessage("term.tileDelta")).append(" ");

        o = parameters.getValue(AVKey.DATA_CACHE_NAME);
        if (o == null || !(o is String) || ((String) o).length() < 1)
            sb.append(Logging.getMessage("term.fileStoreFolder")).append(" ");

        o = parameters.getValue(AVKey.TILE_URL_BUILDER);
        if (o == null || !(o is TileUrlBuilder))
            sb.append(Logging.getMessage("term.tileURLBuilder")).append(" ");

        o = parameters.getValue(AVKey.EXPIRY_TIME);
        if (o != null && (!(o is Long) || ((Long) o) < 1))
            sb.append(Logging.getMessage("term.expiryTime")).append(" ");

        if (params.getStringValue(AVKey.LEVEL_NAME).length() > 0)
        {
            o = parameters.getValue(AVKey.DATASET_NAME);
            if (o == null || !(o is String) || ((String) o).length() < 1)
                sb.append(Logging.getMessage("term.datasetName")).append(" ");

            o = parameters.getValue(AVKey.FORMAT_SUFFIX);
            if (o == null || !(o is String) || ((String) o).length() < 1)
                sb.append(Logging.getMessage("term.formatSuffix")).append(" ");
        }

        if (sb.length() == 0)
            return null;

        return Logging.getMessage("layers.LevelSet.InvalidLevelDescriptorFields", sb.ToString());
    }

    public AVList getParams()
    {
        return parameters;
    }

    public String getPath()
    {
        return this.path;
    }

    public int getLevelNumber()
    {
        return this.levelNumber;
    }

    public String getLevelName()
    {
        return this.levelName;
    }

    public LatLon getTileDelta()
    {
        return this.tileDelta;
    }

    public int getTileWidth()
    {
        return this.tileWidth;
    }

    public int getTileHeight()
    {
        return this.tileHeight;
    }

    public String getFormatSuffix()
    {
        return this.formatSuffix;
    }

    public String getService()
    {
        return this.service;
    }

    public String getDataset()
    {
        return this.dataset;
    }

    public String getCacheName()
    {
        return this.cacheName;
    }

    public double getTexelSize()
    {
        return this.texelSize;
    }

    public bool isEmpty()
    {
        return this.levelName == null || this.levelName.Equals("") || !this.active;
    }

    public void markResourceAbsent(long tileNumber)
    {
        if (tileNumber >= 0)
            this.absentTiles.markResourceAbsent(tileNumber);
    }

    public bool isResourceAbsent(long tileNumber)
    {
        return this.absentTiles.isResourceAbsent(tileNumber);
    }

    public void unmarkResourceAbsent(long tileNumber)
    {
        if (tileNumber >= 0)
            this.absentTiles.unmarkResourceAbsent(tileNumber);
    }

    public long getExpiryTime()
    {
        return this.expiryTime;
    }

    public void setExpiryTime(long expTime)
    {
        this.expiryTime = expTime;
    }

    public bool isActive()
    {
        return this.active;
    }

    public void setActive(boolean active)
    {
        this.active = active;
    }

    public AbsentResourceList getAbsentTiles()
    {
        return absentTiles;
    }

    @Override
    public Object setValue(String key, Object value)
    {
        if (key != null && key.Equals(AVKey.MAX_ABSENT_TILE_ATTEMPTS) && value is Integer)
            this.absentTiles.setMaxTries((Integer) value);
        else if (key != null && key.Equals(AVKey.MIN_ABSENT_TILE_CHECK_INTERVAL) && value is Integer)
            this.absentTiles.setMinCheckInterval((Integer) value);

        return super.setValue(key, value);
    }

    @Override
    public Object getValue(String key)
    {
        if (key != null && key.Equals(AVKey.MAX_ABSENT_TILE_ATTEMPTS))
            return this.absentTiles.getMaxTries();
        else if (key != null && key.Equals(AVKey.MIN_ABSENT_TILE_CHECK_INTERVAL))
            return this.absentTiles.getMinCheckInterval();

        return super.getValue(key);
    }

    /**
     * Returns the URL necessary to retrieve the specified tile.
     *
     * @param tile        the tile who's resources will be retrieved.
     * @param imageFormat a string identifying the mime type of the desired image format
     *
     * @return the resource URL.
     *
     * @throws java.net.MalformedURLException if the URL cannot be formed from the tile's parameters.
     * @throws ArgumentException       if <code>tile</code> is null.
     */
    public java.net.URL getTileResourceURL(Tile tile, String imageFormat) throws java.net.MalformedURLException
    {
        if (tile == null)
        {
            String msg = Logging.getMessage("nullValue.TileIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return this.urlBuilder.getURL(tile, imageFormat);
    }

    public Sector computeSectorForPosition(Angle latitude, Angle longitude, LatLon tileOrigin)
    {
        if (latitude == null || longitude == null)
        {
            String message = Logging.getMessage("nullValue.LatLonIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        if (tileOrigin == null)
        {
            String message = Logging.getMessage("nullValue.TileOriginIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        // Compute the tile's SW lat/lon based on its row/col in the level's data set.
        Angle dLat = this.getTileDelta().getLatitude();
        Angle dLon = this.getTileDelta().getLongitude();
        Angle latOrigin = tileOrigin.getLatitude();
        Angle lonOrigin = tileOrigin.getLongitude();

        int row = Tile.computeRow(dLat, latitude, latOrigin);
        int col = Tile.computeColumn(dLon, longitude, lonOrigin);
        Angle minLatitude = Tile.computeRowLatitude(row, dLat, latOrigin);
        Angle minLongitude = Tile.computeColumnLongitude(col, dLon, lonOrigin);

        return new Sector(minLatitude, minLatitude.add(dLat), minLongitude, minLongitude.add(dLon));
    }

    public int compareTo(Level that)
    {
        if (that == null)
        {
            String msg = Logging.getMessage("nullValue.LevelIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        return this.levelNumber < that.levelNumber ? -1 : this.levelNumber == that.levelNumber ? 0 : 1;
    }

    public override bool Equals(Object o)
    {
        if (this == o)
            return true;
        if (o == null || GetType() != o.GetType())
            return false;

        final Level level = (Level) o;

        if (levelNumber != level.levelNumber)
            return false;
        if (tileHeight != level.tileHeight)
            return false;
        if (tileWidth != level.tileWidth)
            return false;
        if (cacheName != null ? !cacheName.Equals(level.cacheName) : level.cacheName != null)
            return false;
        if (dataset != null ? !dataset.Equals(level.dataset) : level.dataset != null)
            return false;
        if (formatSuffix != null ? !formatSuffix.Equals(level.formatSuffix) : level.formatSuffix != null)
            return false;
        if (levelName != null ? !levelName.Equals(level.levelName) : level.levelName != null)
            return false;
        if (service != null ? !service.Equals(level.service) : level.service != null)
            return false;
        //noinspection RedundantIfStatement
        if (tileDelta != null ? !tileDelta.Equals(level.tileDelta) : level.tileDelta != null)
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        int result;
        result = levelNumber;
        result = 29 * result + (levelName != null ? levelName.GetHashCode() : 0);
        result = 29 * result + (tileDelta != null ? tileDelta.GetHashCode() : 0);
        result = 29 * result + tileWidth;
        result = 29 * result + tileHeight;
        result = 29 * result + (formatSuffix != null ? formatSuffix.GetHashCode() : 0);
        result = 29 * result + (service != null ? service.GetHashCode() : 0);
        result = 29 * result + (dataset != null ? dataset.GetHashCode() : 0);
        result = 29 * result + (cacheName != null ? cacheName.GetHashCode() : 0);
        return result;
    }

    @Override
    public override string ToString()
    {
        return this.path;
    }
}
}
