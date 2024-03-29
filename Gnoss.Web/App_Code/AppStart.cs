﻿using Microsoft.Extensions.Hosting.Internal;
using System.Reflection;

public static class AppStart
{
    public static void AppInitialize()
    {
        // we create a new instance of our own VirtualPathProvider.

        //TODO Javier MyVirtualPathProvider providerInstance = new MyVirtualPathProvider();
        // any settings about your VirtualPathProvider may go here.

        // we get the current instance of HostingEnvironment class. We can't create a new one
        // because it is not allowed to do so. An AppDomain can only have one HostingEnvironment
        // instance.
        HostingEnvironment hostingEnvironmentInstance = (HostingEnvironment)typeof(HostingEnvironment).InvokeMember("_theHostingEnvironment", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
        if (hostingEnvironmentInstance == null)
            return;

        // we get the MethodInfo for RegisterVirtualPathProviderInternal method which is internal
        // and also static.
        MethodInfo mi = typeof(HostingEnvironment).GetMethod("RegisterVirtualPathProviderInternal", BindingFlags.NonPublic | BindingFlags.Static);
        if (mi == null)
            return;

        // finally we invoke RegisterVirtualPathProviderInternal method with one argument which
        // is the instance of our own VirtualPathProvider.
        //TODO Javier mi.Invoke(hostingEnvironmentInstance, new object[] { (VirtualPathProvider)providerInstance });
    }
}
