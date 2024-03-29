/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using org.w3c.dom;
using SharpEarth.cache;
namespace SharpEarth.util{


/**
 * An implementation of {@link FileStoreFilter} which accepts XML configuration documents. Accepted document types are:
 * <ul> <li>Layer configuration documents</li> <li>ElevationModel configuration documents</li> <li>Installed data
 * configuration documents</li> <li>World Wind .NET LevelSet documents</li> </ul>
 *
 * @author dcollins
 * @version $Id: DataConfigurationFilter.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class DataConfigurationFilter : java.io.FileFilter, FileStoreFilter
{
    /** Creates a DataConfigurationFilter, but otherwise does nothing. */
    public DataConfigurationFilter()
    {
    }

    /**
     * Returns true if the specified file can be opened as an XML document, and calling {@link
     * #accept(org.w3c.dom.Document)} returns true.
     *
     * @param file the file in question.
     *
     * @return true if the file should be accepted; false otherwise.
     *
     * @throws ArgumentException if the file is null.
     */
    public bool accept(java.io.File file)
    {
        if (file == null)
        {
            String msg = Logging.getMessage("nullValue.FileIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        // First check the file path, optionally returning false if the path cannot be accepted for any reason.
        if (!this.acceptFilePath(file.getPath()))
            return false;

        Document doc = null;
        try
        {
            doc = WWXML.openDocumentFile(file.getPath(), this.GetType());
        }
        catch (Exception e)
        {
            // Not interested in logging the exception. We just want to return false, indicating that the File cannot
            // be opened as an XML document.
        }

        return (doc != null) && (doc.getDocumentElement() != null) && this.accept(doc);
    }

    /**
     * Returns true if the specified {@link java.net.URL} can be opened as an XML document, and calling {@link
     * #accept(org.w3c.dom.Document)} returns true.
     *
     * @param url the URL in question.
     *
     * @return true if the URL should be accepted; false otherwise.
     *
     * @throws ArgumentException if the url is null.
     */
    public bool accept(java.net.URL url)
    {
        if (url == null)
        {
            String msg = Logging.getMessage("nullValue.URLIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        Document doc = null;
        try
        {
            doc = WWXML.openDocumentURL(url);
        }
        catch (Exception e)
        {
            // Not interested in logging the exception. We just want to return false, indicating that the URL cannot
            // be opened as an XML document.
        }

        return (doc != null) && (doc.getDocumentElement() != null) && this.accept(doc);
    }

    /**
     * Returns true if the specified {@link java.io.InputStream} can be opened as an XML document, and calling {@link
     * #accept(org.w3c.dom.Document)} returns true.
     *
     * @param inputStream the input stream in question.
     *
     * @return true if the input stream should be accepted; false otherwise.
     *
     * @throws ArgumentException if the input stream is null.
     */
    public bool accept(java.io.InputStream inputStream)
    {
        if (inputStream == null)
        {
            String msg = Logging.getMessage("nullValue.InputStreamIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        Document doc = null;
        try
        {
            doc = WWXML.openDocumentStream(inputStream);
        }
        catch (Exception e)
        {
            // Not interested in logging the exception. We just want to return false, indicating that the InputStream
            // cannot be opened as an XML document.
        }

        return (doc != null) && (doc.getDocumentElement() != null) && this.accept(doc);
    }

    /**
     * Returns true if the specified file store path can be opened as an XML document, and calling {@link
     * #accept(org.w3c.dom.Document)} returns true.
     *
     * @param fileStore the file store containing the named file path.
     * @param fileName  the named file path in question.
     *
     * @return true if the file name should be accepted; false otherwise.
     *
     * @throws ArgumentException if either the file store or the file name are null.
     */
    public bool accept(FileStore fileStore, String fileName)
    {
        if (fileStore == null)
        {
            String msg = Logging.getMessage("nullValue.FileStoreIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (fileName == null)
        {
            String message = Logging.getMessage("nullValue.FilePathIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        // Attempt to locate the named path in the FileStore, optionally checking the class path. If a file with that
        // name cannot be located, then return false.
        java.net.URL url = fileStore.findFile(fileName, true);
        if (url == null)
            return false;

        // Attempt to convert the URL to a local file path. If that succeeds, then continue treating the URL as if
        // it were a File.
        java.io.File file = WWIO.convertURLToFile(url);
        if (file != null)
            return this.accept(file);

        return this.accept(url);
    }

    /**
     * Returns true if the specified DOM Document should be accepted as a configuration document.
     *
     * @param doc the Document in question.
     *
     * @return true if the Document should be accepted; false otherwise.
     *
     * @throws ArgumentException if the document is null.
     */
    public bool accept(Document doc)
    {
        if (doc == null)
        {
            String message = Logging.getMessage("nullValue.DocumentIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (doc.getDocumentElement() == null)
        {
            String message = Logging.getMessage("nullValue.DocumentElementIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return this.accept(doc.getDocumentElement());
    }

    /**
     * Returns true if the specified DOM {@link org.w3c.dom.Element} should be accepted as a configuration document.
     *
     * @param domElement the Document in question.
     *
     * @return true if the Document should be accepted; false otherwise.
     *
     * @throws ArgumentException if the document is null.
     */
    public bool accept(Element domElement)
    {
        if (domElement == null)
        {
            String message = Logging.getMessage("nullValue.DocumentIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return DataConfigurationUtils.isDataConfig(domElement);
    }

    protected bool acceptFilePath(String filePath)
    {
        if (filePath == null)
        {
            String message = Logging.getMessage("nullValue.FilePathIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return filePath.toLowerCase().endsWith(".xml");
    }
}
}
