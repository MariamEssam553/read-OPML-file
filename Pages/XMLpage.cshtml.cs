using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;

namespace HW4.Pages;

public class XMLpageModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public XMLpageModel(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;

    public List<Item> Items { get; set; } = new();
    public async Task<IActionResult> OnGetAsync()
    {
        string feed = Request.Query["message"];
        await renderXml(feed);
        return Page();
    }

    public async Task renderXml(string url)
    {
        //string url = "http://scripting.com/rss.xml";

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url);
        string XMLcontent = await response.Content.ReadAsStringAsync();

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(XMLcontent);
        XmlElement? root = xmlDocument.DocumentElement;
        XmlNodeList nodes = root.GetElementsByTagName("item");
        List<Item> itemsList = new List<Item>();

        foreach (XmlNode node in nodes)
        {
            XmlNode? titleTag = node.SelectSingleNode("title");
            string title = (titleTag != null) ? titleTag.InnerText : "";

            XmlNode? descriptionTag = node.SelectSingleNode("description");
            string htmlCode = (descriptionTag != null) ? descriptionTag.InnerText : "";
            HtmlString description = new HtmlString(htmlCode);

            XmlNode? pubDateTag = node.SelectSingleNode("pubDate");
            string pubDate = (pubDateTag != null) ? pubDateTag.InnerText : "";
            var date = DateTime.Parse(pubDate);

            XmlNode? linkTag = node.SelectSingleNode("link");
            string link = (linkTag != null) ? linkTag.InnerText : "";

            Item newItem = new Item()
            {
                title = title,
                description = description,
                pubdate = date,
                link = link
            };

            itemsList.Add(newItem);
        }

        Items = itemsList;


    }
}
public class Item
{
    public string? title { get; set; }
    public HtmlString? description { get; set; }
    public DateTime pubdate { get; set; }
    public string? link { get; set; }
}
