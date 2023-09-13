using System.Xml;

namespace Utils.Extensions.xml {
public static class XmlElementExtensions {
    public static int getIntAttribute(this XmlElement element, string name) {
        return int.Parse(element.GetAttribute(name));
    }
}
}