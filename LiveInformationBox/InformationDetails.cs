/*
 * LiveInformationBox 2.0
 * by Jayson Ragasa aka Nullstring
 * Baguio City, Philippines
 * -
 *
*/

using System.Xml;

namespace LiveInformationBox
{
    public class InformationDetails
    {
        public string Information { set; get; }
        public string ShortDescription { set; get; }
        public string Title { set; get; }
    }

    public class XMLInformationReader
    {
        private string _xmlInfoFile = string.Empty;

        public XMLInformationReader(string XMLInfoFile)
        {
            _xmlInfoFile = XMLInfoFile;
        }

        public InformationDetails Read(string ControlName)
        {
            InformationDetails infoDet = null;

            XmlDocument d = new XmlDocument();

            d.Load(_xmlInfoFile);

            string xpath = "/ControlInformations/control[@name=\"{0}\"]";
            xpath = string.Format(xpath, ControlName);

            XmlNode thisNode = d.SelectSingleNode(xpath);

            if (thisNode != null)
            {
                infoDet = new InformationDetails();

                XmlNode node;

                node = thisNode.SelectSingleNode("./title/text()");
                if (node != null)
                {
                    infoDet.Title = node.Value;
                }

                node = thisNode.SelectSingleNode("./short_description/text()");
                if (node != null)
                {
                    infoDet.ShortDescription = node.Value;
                }

                node = thisNode.SelectSingleNode("./infos/text()");
                if (node != null)
                {
                    infoDet.Information = node.Value;
                }
            }

            d = null;

            return infoDet;
        }
    }
}