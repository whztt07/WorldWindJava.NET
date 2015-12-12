/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.geom;

namespace SharpEarth.tracks
{
  /**
   * @author tag
   * @version $Id: TrackPoint.java 1171 2013-02-11 21:45:02Z dcollins $
   */
  public interface TrackPoint
  {
    double getLatitude();

    void setLatitude( double latitude );

    double getLongitude();

    void setLongitude( double longitude );

    double getElevation();

    void setElevation( double elevation );

    string getTime();

    void setTime( string time );

    Position getPosition();

    void setPosition( Position position );
  }
}
