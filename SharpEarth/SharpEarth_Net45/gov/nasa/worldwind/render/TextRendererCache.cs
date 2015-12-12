/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using SharpEarth.Disposable;
namespace SharpEarth.render{


/**
 * http://java.sun.com/products/java-media/2D/reference/faqs/index.html#Q_What_are_fractional_metrics_Wh
 *
 * @author tag
 * @version $Id: TextRendererCache.java 2053 2014-06-10 20:16:57Z tgaskins $
 */
public class TextRendererCache : Disposable
{
    public static class CacheKey
    {
        private final java.awt.Font font;
        private final bool antialiased;
        private final bool useFractionalMetrics;
        private final bool mipmap;

        public CacheKey(java.awt.Font font, bool antialiased, bool useFractionalMetrics, bool mipmap)
        {
            if (font == null)
            {
                String message = Logging.getMessage("nullValue.FontIsNull");
                Logging.logger().severe(message);
                throw new ArgumentException(message);
            }

            this.font = font;
            this.antialiased = antialiased;
            this.useFractionalMetrics = useFractionalMetrics;
            this.mipmap = mipmap;
        }

        public final java.awt.Font getFont()
        {
            return this.font;
        }

        public final bool isAntialiased()
        {
            return this.antialiased;
        }

        public final bool isUseFractionalMetrics()
        {
            return this.useFractionalMetrics;
        }

        public final bool isMipmap()
        {
            return this.mipmap;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || this.GetType() != o.GetType())
                return false;

            CacheKey that = (CacheKey) o;

            return (this.antialiased == that.antialiased)
                && (this.useFractionalMetrics == that.useFractionalMetrics)
                && (this.mipmap == that.mipmap)
                && (this.font.Equals(that.font));
        }

        public override int GetHashCode()
        {
            int result = this.font.GetHashCode();
            result = 31 * result + (this.antialiased ? 1 : 0);
            result = 31 * result + (this.useFractionalMetrics ? 1 : 0);
            result = 31 * result + (this.mipmap ? 1 : 0);
            return result;
        }
    }

    protected java.util.concurrent.ConcurrentHashMap<Object, TextRenderer> textRendererMap;

    public TextRendererCache()
    {
        this.textRendererMap = new java.util.concurrent.ConcurrentHashMap<Object, TextRenderer>();
    }

    public void dispose()
    {
        this.disposeAll();
        this.textRendererMap.clear();
    }

    public int getNumObjects()
    {
        return this.textRendererMap.size();
    }

    public TextRenderer get(Object key)
    {
        if (key == null)
        {
            String message = Logging.getMessage("nullValue.KeyIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return this.textRendererMap.get(key);
    }

    public void put(Object key, TextRenderer textRenderer)
    {
        if (key == null)
        {
            String message = Logging.getMessage("nullValue.KeyIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        TextRenderer oldTextRenderer = this.textRendererMap.put(key, textRenderer);

        if (oldTextRenderer != null)
        {
            this.dispose(oldTextRenderer);
        }
    }

    public void remove(Object key)
    {
        if (key == null)
        {
            String message = Logging.getMessage("nullValue.KeyIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        TextRenderer textRenderer = this.textRendererMap.remove(key);

        if (textRenderer != null)
        {
            this.dispose(textRenderer);
        }
    }

    public bool contains(Object key)
    {
        if (key == null)
        {
            String message = Logging.getMessage("nullValue.KeyIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return this.textRendererMap.containsKey(key);
    }

    public void clear()
    {
        this.disposeAll();
        this.textRendererMap.clear();
    }

    protected void dispose(TextRenderer textRenderer)
    {
        if (textRenderer != null)
        {
            textRenderer.dispose();
        }
    }

    protected void disposeAll()
    {
        for (java.util.Map.Entry<Object, TextRenderer> e : this.textRendererMap.entrySet())
        {
            if (e.getValue() != null)
            {
                this.dispose(e.getValue());
            }
        }
    }
}
}
