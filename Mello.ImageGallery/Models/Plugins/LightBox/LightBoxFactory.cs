using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mello.ImageGallery.Models.Plugins.LightBox
{
  public sealed class LightBoxFactory : PluginFactory
  {
    public LightBoxFactory()
    {
      _pluginResourceDescriptor = new LightBoxResourceDescriptor();
      _plugin = new LightBox(new LightBoxSettings(PluginResourceDescriptor.PluginResourcePath));
    }

    private readonly ImageGalleryPlugin _plugin;

    private readonly PluginResourceDescriptor _pluginResourceDescriptor;

    public override ImageGalleryPlugin Plugin
    {
      get { return _plugin; }
    }

    public override PluginResourceDescriptor PluginResourceDescriptor
    {
      get { return _pluginResourceDescriptor; }
    }
  }
}
