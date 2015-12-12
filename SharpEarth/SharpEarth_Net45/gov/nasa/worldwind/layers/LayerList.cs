/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.concurrent.CopyOnWriteArrayList;
using java.util;
using java.beans;
using SharpEarth.util;
using SharpEarth.events.Message;
using SharpEarth.avlist;
using SharpEarth;
namespace SharpEarth.layers{



/**
 * @author Tom Gaskins
 * @version $Id: LayerList.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class LayerList : CopyOnWriteArrayList<Layer>, WWObject
{
    private WWObjectImpl wwo = new WWObjectImpl(this);

    public LayerList()
    {
    }

    public LayerList(Layer[] layers)
    {
        if (layers == null)
        {
            String message = Logging.getMessage("nullValue.LayersIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.addAll(Arrays.asList(layers));
    }

    public LayerList(LayerList layerList)
    {
        super(layerList);
    }

    public String getDisplayName()
    {
        return this.getStringValue(AVKey.DISPLAY_NAME);
    }

    public void setDisplayName(String displayName)
    {
        this.setValue(AVKey.DISPLAY_NAME, displayName);
    }

    protected LayerList makeShallowCopy(LayerList sourceList)
    {
        return new LayerList(sourceList);
    }

    public static List<Layer> getListDifference(LayerList oldList, LayerList newList)
    {
        ArrayList<Layer> deltaList = new ArrayList<Layer>();

        foreach (Layer layer  in  newList)
        {
            if (!oldList.contains(layer))
                deltaList.add(layer);
        }

        return deltaList;
    }

    /**
     * Aggregate the contents of a group of layer lists into a single one. All layers are placed in the first designated
     * list and removed from the subsequent lists.
     *
     * @param lists an array containing the lists to aggregate. All members of the second and subsequent lists in the
     *              array are added to the first list in the array.
     *
     * @return the aggregated list.
     *
     * @throws ArgumentException if the layer-lists array is null or empty.
     */
    public static LayerList collapseLists(LayerList[] lists)
    {
        if (lists == null || lists.length == 0)
        {
            String message = Logging.getMessage("nullValue.LayersListArrayIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        LayerList list = lists[0];

        for (int i = 1; i < lists.length; i++)
        {
            LayerList ll = lists[i];
            foreach (Layer layer  in  ll)
            {
                list.add(layer);
            }

            foreach (Layer layer  in  ll)
            {
                ll.remove(layer);
            }
        }

        return list;
    }

    public static List<Layer> getLayersAdded(LayerList oldList, LayerList newList)
    {
        return getListDifference(oldList, newList);
    }

    public static List<Layer> getLayersRemoved(LayerList oldList, LayerList newList)
    {
        return getListDifference(newList, oldList);
    }

    public bool add(Layer layer)
    {
        if (layer == null)
        {
            String message = Logging.getMessage("nullValue.LayerIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        LayerList copy = makeShallowCopy(this);
        super.add(layer);
        layer.addPropertyChangeListener(this);
        this.firePropertyChange(AVKey.LAYERS, copy, this);

        return true;
    }

    public void add(int index, Layer layer)
    {
        if (layer == null)
        {
            String message = Logging.getMessage("nullValue.LayerIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        LayerList copy = makeShallowCopy(this);
        super.add(index, layer);
        layer.addPropertyChangeListener(this);
        this.firePropertyChange(AVKey.LAYERS, copy, this);
    }

    public void remove(Layer layer)
    {
        if (layer == null)
        {
            String msg = Logging.getMessage("nullValue.LayerIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (!this.contains(layer))
            return;

        LayerList copy = makeShallowCopy(this);
        layer.removePropertyChangeListener(this);
        super.remove(layer);
        this.firePropertyChange(AVKey.LAYERS, copy, this);
    }

    public Layer remove(int index)
    {
        Layer layer = get(index);
        if (layer == null)
            return null;

        LayerList copy = makeShallowCopy(this);
        layer.removePropertyChangeListener(this);
        super.remove(index);
        this.firePropertyChange(AVKey.LAYERS, copy, this);

        return layer;
    }

    public bool moveLower(Layer targetLayer)
    {
        int index = this.indexOf(targetLayer);
        if (index <= 0)
            return false;

        this.remove(index);
        this.add(index - 1, targetLayer);

        return true;
    }

    public bool moveHigher(Layer targetLayer)
    {
        int index = this.indexOf(targetLayer);
        if (index < 0)
            return false;

        this.remove(index);
        this.add(index + 1, targetLayer);

        return true;
    }

    public Layer set(int index, Layer layer)
    {
        if (layer == null)
        {
            String message = Logging.getMessage("nullValue.LayerIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        Layer oldLayer = this.get(index);
        if (oldLayer != null)
            oldLayer.removePropertyChangeListener(this);

        LayerList copy = makeShallowCopy(this);
        super.set(index, layer);
        layer.addPropertyChangeListener(this);
        this.firePropertyChange(AVKey.LAYERS, copy, this);

        return oldLayer;
    }

    public bool remove(Object o)
    {
        foreach (Layer layer  in  this)
        {
            if (layer.Equals(o))
                layer.removePropertyChangeListener(this);
        }

        LayerList copy = makeShallowCopy(this);
        bool removed = super.remove(o);
        if (removed)
            this.firePropertyChange(AVKey.LAYERS, copy, this);

        return removed;
    }

    public bool addIfAbsent(Layer layer)
    {
        foreach (Layer l  in  this)
        {
            if (l.Equals(layer))
                return false;
        }

        layer.addPropertyChangeListener(this);

        LayerList copy = makeShallowCopy(this);
        bool added = super.addIfAbsent(layer);
        if (added)
            this.firePropertyChange(AVKey.LAYERS, copy, this);

        return added;
    }

    public bool removeAll(Collection<?> objects)
    {
        foreach (Layer layer  in  this)
        {
            layer.removePropertyChangeListener(this);
        }

        LayerList copy = makeShallowCopy(this);
        bool removed = super.removeAll(objects);
        if (removed)
            this.firePropertyChange(AVKey.LAYERS, copy, this);

        foreach (Layer layer  in  this)
        {
            layer.addPropertyChangeListener(this);
        }

        return removed;
    }

    public bool removeAll()
    {
        foreach (Layer layer  in  this)
        {
            layer.removePropertyChangeListener(this);
        }

        LayerList copy = makeShallowCopy(this);
        bool removed = super.retainAll(new ArrayList<Layer>()); // retain no layers
        if (removed)
            this.firePropertyChange(AVKey.LAYERS, copy, this);

        return removed;
    }

    public int addAllAbsent(Collection<? extends Layer> layers)
    {
        foreach (Layer layer  in  layers)
        {
            if (!this.contains(layer))
                layer.addPropertyChangeListener(this);
        }

        LayerList copy = makeShallowCopy(this);
        int numAdded = super.addAllAbsent(layers);
        if (numAdded > 0)
            this.firePropertyChange(AVKey.LAYERS, copy, this);

        return numAdded;
    }

    public bool addAll(Collection<? extends Layer> layers)
    {
        foreach (Layer layer  in  layers)
        {
            layer.addPropertyChangeListener(this);
        }

        LayerList copy = makeShallowCopy(this);
        bool added = super.addAll(layers);
        if (added)
            this.firePropertyChange(AVKey.LAYERS, copy, this);

        return added;
    }

    public bool addAll(int i, Collection<? extends Layer> layers)
    {
        foreach (Layer layer  in  layers)
        {
            layer.addPropertyChangeListener(this);
        }

        LayerList copy = makeShallowCopy(this);
        bool added = super.addAll(i, layers);
        if (added)
            this.firePropertyChange(AVKey.LAYERS, copy, this);

        return added;
    }

    @SuppressWarnings( {"SuspiciousMethodCalls"})
    public bool retainAll(Collection<?> objects)
    {
        foreach (Layer layer  in  this)
        {
            if (!objects.contains(layer))
                layer.removePropertyChangeListener(this);
        }

        LayerList copy = makeShallowCopy(this);
        bool added = super.retainAll(objects);
        if (added)
            this.firePropertyChange(AVKey.LAYERS, copy, this);

        return added;
    }

    public void replaceAll(Collection<? extends Layer> layers)
    {
        ArrayList<Layer> toDelete = new ArrayList<Layer>();
        ArrayList<Layer> toKeep = new ArrayList<Layer>();

        foreach (Layer layer  in  layers)
        {
            if (!this.contains(layer))
                toDelete.add(layer);
            else
                toKeep.add(layer);
        }

        foreach (Layer layer  in  toDelete)
        {
            this.remove(layer);
        }

        super.clear();

        foreach (Layer layer  in  layers)
        {
            if (!toKeep.contains(layer))
                layer.addPropertyChangeListener(this);

            super.add(layer);
        }
    }

    public Layer getLayerByName(String name)
    {
        if (name == null)
        {
            String message = Logging.getMessage("nullValue.NameIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        foreach (Layer l  in  this)
        {
            if (l.getName().Equals(name))
                return l;
        }

        return null;
    }

    public List<Layer> getLayersByClass(Class classToFind)
    {
        if (classToFind == null)
        {
            String message = Logging.getMessage("nullValue.ClassIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        ArrayList<Layer> layers = new ArrayList<Layer>();

        foreach (Layer l  in  this)
        {
            if (l.GetType().Equals(classToFind))
                layers.add(l);
        }

        return layers;
    }

    public Object getValue(String key)
    {
        return wwo.getValue(key);
    }

    public Collection<Object> getValues()
    {
        return wwo.getValues();
    }

    public Set<Map.Entry<String, Object>> getEntries()
    {
        return wwo.getEntries();
    }

    public String getStringValue(String key)
    {
        return wwo.getStringValue(key);
    }

    public Object setValue(String key, Object value)
    {
        return wwo.setValue(key, value);
    }

    public AVList setValues(AVList avList)
    {
        return wwo.setValues(avList);
    }

    public bool hasKey(String key)
    {
        return wwo.hasKey(key);
    }

    public Object removeKey(String key)
    {
        return wwo.removeKey(key);
    }

    public AVList copy()
    {
        return wwo.copy();
    }

    public AVList clearList()
    {
        return this.wwo.clearList();
    }

    public LayerList sort()
    {
        if (this.size() <= 0)
            return this;

        Layer[] array = new Layer[this.size()];
        this.toArray(array);
        Arrays.sort(array, new Comparator<Layer>()
        {
            public int compare(Layer layer, Layer layer1)
            {
                return layer.getName().compareTo(layer1.getName());
            }
        });

        this.clear();
        super.addAll(Arrays.asList(array));

        return this;
    }

    public void addPropertyChangeListener(String propertyName, PropertyChangeListener listener)
    {
        wwo.addPropertyChangeListener(propertyName, listener);
    }

    public void removePropertyChangeListener(String propertyName, PropertyChangeListener listener)
    {
        wwo.removePropertyChangeListener(propertyName, listener);
    }

    public void addPropertyChangeListener(PropertyChangeListener listener)
    {
        wwo.addPropertyChangeListener(listener);
    }

    public void removePropertyChangeListener(PropertyChangeListener listener)
    {
        wwo.removePropertyChangeListener(listener);
    }

    public void firePropertyChange(PropertyChangeEvent propertyChangeEvent)
    {
        wwo.firePropertyChange(propertyChangeEvent);
    }

    public void firePropertyChange(String propertyName, Object oldValue, Object newValue)
    {
        wwo.firePropertyChange(propertyName, oldValue, newValue);
    }

    public void propertyChange(PropertyChangeEvent propertyChangeEvent)
    {
        wwo.propertyChange(propertyChangeEvent);
    }

    public void onMessage(Message message)
    {
        wwo.onMessage(message);
    }

    @Override
    public override string ToString()
    {
        String r = "";
        foreach (Layer l  in  this)
        {
            r += l.ToString() + ", ";
        }
        return r;
    }
}
}
