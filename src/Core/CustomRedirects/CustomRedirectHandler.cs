using System;
using BVNetwork.NotFound.Core.Data;
using BVNetwork.NotFound.Core.Upgrade;
using EPiServer.Logging;

namespace BVNetwork.NotFound.Core.CustomRedirects
{
    /// <summary>
    /// Handler for custom redirects. Loads and caches the list of custom redirects
    /// to ensure performance. 
    /// </summary>
    public class CustomRedirectHandler
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        // ReSharper disable once InconsistentNaming
        private const string CACHE_KEY_CUSTOM_REDIRECT_HANDLER_INSTANCE = "BvnCustomMultiSiteRedirectHandler";
        private CustomRedirectCollection _customRedirects;

        #region constructors...
        // Should only be instanciated by the static Current method
        protected CustomRedirectHandler()
        {
        }
        #endregion

        /// <summary>
        /// The collection of custom redirects
        /// </summary>
        public CustomRedirectCollection CustomRedirects
        {
            get
            {
                return _customRedirects;
            }
        }

        /// <summary>
        /// Save a collection of redirects, and call method to raise an event in order to clear cache on all servers.
        /// </summary>
        /// <param name="redirects"></param>
        public void SaveCustomRedirects(CustomRedirectCollection redirects)
        {
            DataStoreHandler dynamicHandler = new DataStoreHandler();
            foreach (CustomRedirect redirect in redirects)
            {
                // Add redirect 
                dynamicHandler.SaveCustomRedirect(redirect);
            }
            DataStoreEventHandlerHook.DataStoreUpdated();
        }

        /// <summary>
        /// Read the custom redirects from the dynamic data store, and 
        /// stores them in the CustomRedirect property
        /// </summary>
        protected void LoadCustomRedirects()
        {
            DataStoreHandler dynamicHandler = new DataStoreHandler();
            _customRedirects = new CustomRedirectCollection();

            foreach (CustomRedirect redirect in dynamicHandler.GetCustomRedirects(false))
                _customRedirects.Add(redirect);
        }

        /// <summary>
        /// Static method for getting the current custom redirects handler. Will store
        /// the handler in cache after it has been used.
        /// </summary>
        /// <returns>An instanciated CustomRedirectHandler</returns>
        public static CustomRedirectHandler Current
        {
            get
            {
                Logger.Debug("Begin: Get Current CustomRedirectHandler");
                // First check if there is a cached version of
                // this object
                var handler = GetHandlerFromCache();
                if (handler != null)
                {
                    Logger.Debug("Returning cached handler.");
                    Logger.Debug("End: Get Current CustomRedirectHandler");
                    // Got the cached version, return it
                    return handler;
                }

                // Not cached, we need to create it
                handler = new CustomRedirectHandler();
                // Load redirects with standard settings
                Logger.Debug("Begin: Load custom redirects from dynamic data store");
                try
                {
                    handler.LoadCustomRedirects();
                }
                catch (Exception ex)
                {
                    Logger.Error("An error occured while loading redirects from dds. CustomRedirectHandlerException has now been set. Exception:" + ex);
                    CustomRedirectHandlerException = ex.ToString();
                    Upgrader.Valid = false;
                }
                Logger.Debug("End: Load custom redirects from dynamic data store");

                // Store in cache
                StoreHandlerInCache(handler);

                Logger.Debug("End: Get Current CustomRedirectHandler");
                return handler;
            }
        }

        /// <summary>
        /// Clears the redirect cache.
        /// </summary>
        public static void ClearCache()
        {
            EPiServer.CacheManager.Remove(CACHE_KEY_CUSTOM_REDIRECT_HANDLER_INSTANCE);
        }

        /// <summary>
        /// Gets the handler from the cache, if it has been stored there.
        /// </summary>
        /// <returns>An instanciated CustomRedirectHandler if found in the cache, null if not found</returns>
        private static CustomRedirectHandler GetHandlerFromCache()
        {
            var handler = EPiServer.CacheManager.Get(CACHE_KEY_CUSTOM_REDIRECT_HANDLER_INSTANCE) as CustomRedirectHandler;
            return handler;
        }


        /// <summary>
        /// Stores the redirect handler in the cache
        /// </summary>
        private static void StoreHandlerInCache(CustomRedirectHandler handler)
        {
            EPiServer.CacheManager.Insert(CACHE_KEY_CUSTOM_REDIRECT_HANDLER_INSTANCE,
                                                handler);
        }

        public static string CustomRedirectHandlerException { get; set; }


    }
}
