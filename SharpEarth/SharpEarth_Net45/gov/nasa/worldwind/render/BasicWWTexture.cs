/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.net.URL;
using java.io.InputStream;
using java.awt.image;
using javax.media.opengl;
using SharpEarth.util;
using com.jogamp.opengl.util.texture.awt.AWTTextureIO;
using com.jogamp.opengl.util.texture;
namespace SharpEarth.render{



/**
 * Basic implementation of a texture derived from an image source such as an image file or a {@link
 * java.awt.image.BufferedImage}.
 * <p/>
 * The interface contains a method, {@link #isTextureInitializationFailed()} to determine whether the instance failed to
 * convert an image source to a texture. If such a failure occurs, the method returns true and no further attempts are
 * made to create the texture.
 * <p/>
 * This class retrieves its image source immediately during a call to {@link #bind(DrawContext)} or {@link
 * #applyInternalTransform(DrawContext)}.
 *
 * @author tag
 * @version $Id: BasicWWTexture.java 1171 2013-02-11 21:45:02Z dcollins $
 * @see LazilyLoadedTexture
 */
public class BasicWWTexture : WWTexture
{
    private Object imageSource;
    private bool useMipMaps;
    private bool useAnisotropy = true;

    protected Integer width;
    protected Integer height;
    protected TextureCoords texCoords;
    protected bool textureInitializationFailed = false;

    /**
     * Constructs a texture object from an image source.
     * <p/>
     * The texture's image source is opened, if a file, only when the texture is displayed. If the texture is not
     * displayed the image source is not read.
     *
     * @param imageSource the source of the image, either a file path {@link String} or a {@link BufferedImage}.
     * @param useMipMaps  Indicates whether to generate and use mipmaps for the image.
     *
     * @throws ArgumentException if the <code>imageSource</code> is null.
     */
    public BasicWWTexture(Object imageSource, bool useMipMaps)
    {
        initialize(imageSource, useMipMaps);
    }

    /**
     * Constructs a texture object.
     * <p/>
     * The texture's image source is opened, if a file, only when the texture is displayed. If the texture is not
     * displayed the image source is not read.
     *
     * @param imageSource the source of the image, either a file path {@link String} or a {@link BufferedImage}.
     *
     * @throws ArgumentException if the <code>imageSource</code> is null.
     */
    public BasicWWTexture(Object imageSource)
    {
        this(imageSource, false);
    }

    protected void initialize(Object imageSource, bool useMipMaps)
    {
        if (imageSource == null)
        {
            String message = Logging.getMessage("nullValue.ImageSource");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.imageSource = imageSource;
        this.useMipMaps = useMipMaps;
    }

    public Object getImageSource()
    {
        return imageSource;
    }

    public int getWidth(DrawContext dc)
    {
        if (this.width != null)
            return this.width;

        Texture t = this.getTexture(dc, true);

        return t != null ? t.getWidth() : 0;
    }

    public int getHeight(DrawContext dc)
    {
        if (this.height != null)
            return this.height;

        Texture t = this.getTexture(dc, true);

        return t != null ? t.getHeight() : 0;
    }

    /**
     * Indicates whether the texture creates and uses mipmaps.
     *
     * @return true if mipmaps are used, false if  not.
     */
    public bool isUseMipMaps()
    {
        return useMipMaps;
    }

    public TextureCoords getTexCoords()
    {
        return texCoords;
    }

    public bool isTextureCurrent(DrawContext dc)
    {
        return true;
    }

    /**
     * Indicates whether texture anisotropy is applied to the texture when rendered.
     *
     * @return useAnisotropy true if anisotropy is to be applied, otherwise false.
     */
    public bool isUseAnisotropy()
    {
        return useAnisotropy;
    }

    /**
     * Specifies whether texture anisotropy is applied to the texture when rendered.
     *
     * @param useAnisotropy true if anisotropy is to be applied, otherwise false.
     */
    public void setUseAnisotropy(boolean useAnisotropy)
    {
        this.useAnisotropy = useAnisotropy;
    }

    public bool isTextureInitializationFailed()
    {
        return textureInitializationFailed;
    }

    protected Texture getTexture(DrawContext dc, bool initialize)
    {
        Texture t = this.getTextureFromCache(dc);

        if (t == null && initialize)
            t = this.initializeTexture(dc, this.imageSource);

        return t;
    }

    protected Texture getTextureFromCache(DrawContext dc)
    {
        Texture t = dc.getTextureCache().getTexture(this.imageSource);
        if (t != null && this.width == null)
        {
            this.width = t.getWidth();
            this.height = t.getHeight();
            this.texCoords = t.getImageTexCoords();
        }

        return t;
    }

    public bool bind(DrawContext dc)
    {
        if (dc == null)
        {
            String message = Logging.getMessage("nullValue.DrawContextIsNull");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        Texture t = this.getTextureFromCache(dc);
        if (t == null)
        {
            t = this.initializeTexture(dc, this.imageSource);
            if (t != null)
                return true; // texture was bound during initialization.
        }

        if (t != null)
            t.bind(dc.getGL());

        if (t != null && this.width == 0 && this.height == 0)
        {
            this.width = t.getWidth();
            this.height = t.getHeight();
            this.texCoords = t.getImageTexCoords();
        }

        return t != null;
    }

    public void applyInternalTransform(DrawContext dc)
    {
        if (dc == null)
        {
            String message = Logging.getMessage("nullValue.DrawContextIsNull");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        // Use the tile's texture if available.
        Texture t = this.getTextureFromCache(dc);
        if (t == null)
            t = this.initializeTexture(dc, this.imageSource);

        if (t != null)
        {
            if (t.getMustFlipVertically())
            {
                GL2 gl = dc.getGL().getGL2(); // GL initialization checks for GL2 compatibility.
                gl.glMatrixMode(GL2.GL_TEXTURE);
                gl.glLoadIdentity();
                gl.glScaled(1, -1, 1);
                gl.glTranslated(0, -1, 0);
            }
        }
    }

    protected Texture initializeTexture(DrawContext dc, Object imageSource)
    {
        if (dc == null)
        {
            String message = Logging.getMessage("nullValue.DrawContextIsNull");
            Logging.logger().severe(message);
            throw new IllegalStateException(message);
        }

        if (this.textureInitializationFailed)
            return null;

        Texture t;
        bool haveMipMapData;
        GL gl = dc.getGL();

        if (imageSource is String)
        {
            String path = (String) imageSource;

            Object streamOrException = WWIO.getFileOrResourceAsStream(path, this.GetType());
            if (streamOrException == null || streamOrException is Exception)
            {
                Logging.logger().log(java.util.logging.Level.SEVERE, "generic.ExceptionAttemptingToReadImageFile",
                    streamOrException != null ? streamOrException : path);
                this.textureInitializationFailed = true;
                return null;
            }

            try
            {
                TextureData td = OGLUtil.newTextureData(gl.getGLProfile(), (InputStream) streamOrException,
                    this.useMipMaps);
                t = TextureIO.newTexture(td);
                haveMipMapData = td.getMipmapData() != null;
            }
            catch (Exception e)
            {
                String msg = Logging.getMessage("layers.TextureLayer.ExceptionAttemptingToReadTextureFile",
                    imageSource);
                Logging.logger().log(java.util.logging.Level.SEVERE, msg, e);
                this.textureInitializationFailed = true;
                return null;
            }
        }
        else if (imageSource is BufferedImage)
        {
            try
            {
                TextureData td = AWTTextureIO.newTextureData(gl.getGLProfile(), (BufferedImage) imageSource,
                    this.useMipMaps);
                t = TextureIO.newTexture(td);
                haveMipMapData = td.getMipmapData() != null;
            }
            catch (Exception e)
            {
                String msg = Logging.getMessage("generic.IOExceptionDuringTextureInitialization");
                Logging.logger().log(java.util.logging.Level.SEVERE, msg, e);
                this.textureInitializationFailed = true;
                return null;
            }
        }
        else if (imageSource is URL)
        {
            try
            {
                InputStream stream = ((URL) imageSource).openStream();
                if (stream == null)
                {
                    Logging.logger().log(java.util.logging.Level.SEVERE, "generic.ExceptionAttemptingToReadImageFile",
                        imageSource);
                    this.textureInitializationFailed = true;
                    return null;
                }

                TextureData td = OGLUtil.newTextureData(gl.getGLProfile(), stream, this.useMipMaps);
                t = TextureIO.newTexture(td);
                haveMipMapData = td.getMipmapData() != null;
            }
            catch (Exception e)
            {
                String msg = Logging.getMessage("layers.TextureLayer.ExceptionAttemptingToReadTextureFile",
                    imageSource);
                Logging.logger().log(java.util.logging.Level.SEVERE, msg, e);
                this.textureInitializationFailed = true;
                return null;
            }
        }
        else
        {
            Logging.logger().log(java.util.logging.Level.SEVERE, "generic.UnrecognizedImageSourceType",
                imageSource.GetType().Name);
            this.textureInitializationFailed = true;
            return null;
        }

        if (t == null) // In case JOGL TextureIO returned null
        {
            Logging.logger().log(java.util.logging.Level.SEVERE, "generic.TextureUnreadable",
                imageSource is String ? imageSource : imageSource.GetType().Name);
            this.textureInitializationFailed = true;
            return null;
        }

        // Textures with the same path are assumed to be identical textures, so key the texture id off the
        // image source.
        dc.getTextureCache().put(imageSource, t);
        t.bind(gl);

        // Enable the appropriate mip-mapping texture filters if the caller has specified that mip-mapping should be
        // enabled, and the texture itself supports mip-mapping.
        bool useMipMapFilter = this.useMipMaps && (haveMipMapData || t.isUsingAutoMipmapGeneration());
        gl.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER,
            useMipMapFilter ? GL.GL_LINEAR_MIPMAP_LINEAR : GL.GL_LINEAR);
        gl.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
        gl.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_CLAMP_TO_EDGE);
        gl.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_CLAMP_TO_EDGE);

        if (this.isUseAnisotropy() && useMipMapFilter)
        {
            double maxAnisotropy = dc.getGLRuntimeCapabilities().getMaxTextureAnisotropy();
            if (dc.getGLRuntimeCapabilities().isUseAnisotropicTextureFilter() && maxAnisotropy >= 2.0)
            {
                gl.glTexParameterf(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAX_ANISOTROPY_EXT, (float) maxAnisotropy);
            }
        }

        this.width = t.getWidth();
        this.height = t.getHeight();
        this.texCoords = t.getImageTexCoords();

        return t;
    }
}
}
