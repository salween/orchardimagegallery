using Mello.ImageGallery.Models.Plugins.LightBox;
using Mello.ImageGallery.Models.Plugins.PrettyPhoto;

namespace Mello.ImageGallery.Models.Plugins {
    public abstract class PluginFactory {
        public static PluginFactory GetFactory(Plugin plugin) {
            if (plugin == Plugins.Plugin.PrettyPhoto) {
                return new PrettyPhotoFactory();
            }

            return new LightBoxFactory();
        }

        public abstract ImageGalleryPlugin Plugin { get; }

        public abstract PluginResourceDescriptor PluginResourceDescriptor { get; }
    }
}