/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.net.URL;
using java.lang.reflect.Method;
using SharpEarth.Configuration;
namespace SharpEarth.util{



/**
 * @author dcollins
 * @version $Id: BrowserOpener.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class BrowserOpener
{
    public static void browse(URL url) throws Exception
    {
        if (url == null)
        {
            String message = Logging.getMessage("nullValue.URLIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        try
        {
            String urlString = url.ToString();
            if (Configuration.isMacOS())
                browseMacOS(urlString);
            else if (Configuration.isWindowsOS())
                browseWindows(urlString);
            else if (Configuration.isUnixOS() || Configuration.isLinuxOS())
                browseUnix(urlString);
        }
        catch (Exception e)
        {
            throw new Exception(String.Format("Cannot browse URL %s", url), e);
        }
    }

    private static void browseMacOS(String urlString) throws Exception
    {
        Class<?> fileManager = Class.forName("com.apple.eio.FileManager");
        Method openURL = fileManager.getDeclaredMethod("openURL", String.class);
        openURL.invoke(null, urlString);
    }

    private static void browseWindows(String urlString) throws Exception
    {
        Runtime.getRuntime().exec("rundll32 url.dll,FileProtocolHandler " + urlString);
    }

    private static void browseUnix(String urlString) throws Exception
    {
        String browser = null;

        String[] browsers = {"firefox", "opera", "konqueror", "epiphany", "mozilla", "netscape"};
        for (String curBrowser : browsers)
            if (Runtime.getRuntime().exec(new String[] {"which", curBrowser}).waitFor() == 0)
                browser = curBrowser;

        if (browser == null)
            throw new Exception("Cannot find browser");

        Runtime.getRuntime().exec(new String[] {browser, urlString});
    }
}
}
