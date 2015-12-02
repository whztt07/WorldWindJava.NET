/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using System.Linq;

namespace SharpEarth.pick{


/**
 * @author tag
 * @version $Id: PickedObjectList.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class PickedObjectList : List<PickedObject>
{
    public PickedObjectList()
    {

    }

    public PickedObjectList(PickedObjectList list) : base(list)// clone a shallow copy
    {
    }

    public PickedObject getTopPickedObject()
    {
      if (Count == 1)
        return this[0];
      return this.FirstOrDefault( po => po.isOnTop() );
    }

    public object getTopObject()
    {
        PickedObject po = this.getTopPickedObject();
        return po != null ? po.getObject() : null;
    }

    public PickedObject getTerrainObject()
    {
      return this.FirstOrDefault( po => po.isTerrain() );
    }

    public PickedObject getMostRecentPickedObject()
    {
      return this.LastOrDefault();
    }

    /**
     * Returns a list of all picked objects in this list who's onTop flag is set to true. This returns <code>null</code>
     * if this list is empty, or does not contain any picked objects marked as on top.
     *
     * @return a new list of the picked objects marked as on top, or <code>null</code> if nothing is marked as on top.
     */
    public List<PickedObject> getAllTopPickedObjects()
    {
        List<PickedObject> list = null; // Lazily create the list to avoid unnecessary allocations.



        foreach (PickedObject po in this)
        {
            if (po.isOnTop())
            {
                if (list == null)
                    list = new List<PickedObject>();
                list.Add(po);
            }
        }

        return list;
    }

    /**
     * Returns a list of all objects associated with a picked object in this list who's onTop flag is set to true. This
     * returns <code>null</code> if this list is empty, or does not contain any picked objects marked as on top.
     *
     * @return a new list of the objects associated with a picked object marked as on top, or <code>null</code> if
     *         nothing is marked as on top.
     */
    public List<object> getAllTopObjects()
    {
        List<object> list = null; // Lazily create the list to avoid unnecessary allocations.

        foreach (PickedObject po in this)
        {
            if (po.isOnTop())
            {
                if (list == null)
                    list = new List<object>();
                list.Add(po.getObject());
            }
        }

        return list;
    }

    public bool hasNonTerrainObjects()
    {
        return this.Count > 1 || (this.Count == 1 && this.getTerrainObject() == null);
    }
}
}
