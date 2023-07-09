using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;

namespace HW4.Pages;

public class IndexModel : PageModel
{
    public List<feedItem> items4page { get; set; } = new();
    public List<feedItem> outlines { get; set; } = new();

    private readonly IHttpClientFactory _httpClientFactory;
    public IndexModel(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;
    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 12)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://blue.feedland.org/opml?screenname=dave");

        string OPMLcontent = await response.Content.ReadAsStringAsync();

        XmlDocument opmlDocument = new XmlDocument();
        opmlDocument.LoadXml(OPMLcontent);
        XmlElement? root = opmlDocument.DocumentElement;
        XmlNodeList nodes = root.GetElementsByTagName("outline");
        List<feedItem> itemsList = new List<feedItem>();

        int totalItems = nodes?.Count ?? 0;
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        int startIndex = (pageNumber - 1) * pageSize;
        int endIndex = Math.Min(startIndex + pageSize, nodes.Count);


        foreach (XmlNode node in nodes)
        {
            string text = node.Attributes["text"].Value ?? "";
            string link = node.Attributes["xmlUrl"].Value ?? "";

            feedItem newItem = new feedItem()
            {
                text = text,
                xmlLink = link
            };

            itemsList.Add(newItem);
        }

        outlines = itemsList;

        // Assuming 'allItems' is the complete list of items
        var itemsForPage = outlines.Skip(startIndex).Take(endIndex - startIndex).ToList();

        items4page = itemsForPage;

        ViewData["PageNumber"] = pageNumber;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalItems"] = totalItems;
        ViewData["TotalPages"] = totalPages;

    }
}

public class feedItem
{
    public string? text { get; set; }
    public string? xmlLink { get; set; }
}

