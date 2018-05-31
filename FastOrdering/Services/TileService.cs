using FastOrdering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FastOrdering.Services
{
    class TileService
    {
        public static Windows.Data.Xml.Dom.XmlDocument CreateTiles(SampleOrder item)
        {
            XDocument xDoc = new XDocument(
                new XElement("tile", new XAttribute("version", 3),
                        new XElement("visual",
                           // Small Tile
                            new XElement("binding", new XAttribute("branding", "name"), new XAttribute("displayName", "FastOrdering"), new XAttribute("template", "TileSmall"),
                                new XElement("group",
                                    new XElement("subgroup",
                                        new XElement("text", item.OrderName, new XAttribute("hint-style", "caption")),
                                        new XElement("text", item.Sold, new XAttribute("hint-style", "captionsubtle")),
                                        new XAttribute("hint-wrap", true),
                                        new XAttribute("hint-maxLines", 3)
                                    )
                                )
                            ),
                           // Medium Tile
                            new XElement("binding", new XAttribute("branding", "name"), new XAttribute("displayName", "FastOrding"), new XAttribute("template", "TileMedium"), new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", item.imgPath)),
                                new XElement("group",
                                    new XElement("subgroup",
                                        new XElement("text", item.OrderName, new XAttribute("hint-style", "title")),
                                        new XElement("text", item.Sold, new XAttribute("hint-style", "base"),
                                        new XAttribute("hint-wrap", true),
                                        new XAttribute("hint-maxLines", 3))
                                    )
                                )
                            ),
                            //Wide Tile
                            new XElement("binding", new XAttribute("branding", "name"), new XAttribute("displayName", "FastOrding"), new XAttribute("template", "TileWide"), new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", item.imgPath)),
                                new XElement("group",
                                    new XElement("subgroup",
                                        new XElement("text", item.OrderName, new XAttribute("hint-style", "title")),
                                        new XElement("text", item.Sold, new XAttribute("hint-style", "base"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3)),
                                        new XElement("text", item.Visited, new XAttribute("hint-style", "base"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                        )
                                )
                            ),
                           // Large Tile
                            new XElement("binding", new XAttribute("branding", "name"), new XAttribute("displayName", "FastOrding"), new XAttribute("template", "TileLarge"), new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", item.imgPath)),
                                new XElement("group",
                                    new XElement("subgroup",
                                        new XElement("text", item.OrderName, new XAttribute("hint-style", "title")),
                                        new XElement("text", item.Sold, new XAttribute("hint-style", "base"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3)),
                                        new XElement("text", item.Visited, new XAttribute("hint-style", "base"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                        )
                                )
                            )
                        )
                    )
            );
            Windows.Data.Xml.Dom.XmlDocument xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());
            return xmlDoc;
        }
    }
}
