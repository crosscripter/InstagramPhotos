using System;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

public class InstagramImage
{
    public string Url { get; set; }
    public string Link { get; set; }
    public string Caption { get; set; }

    public InstagramImage(string url, string link, string caption)
    {
        Url = url;
        Link = link;
        Caption = caption;
    }

    public override string ToString()
    {
    	return $@"<a href=""{Link}"" title=""{Caption}""><img src=""{Url}""/></a>";
    }
}

class InstagramController
{
	static dynamic GetJson(string url)
    {
        try
        {
            using (var web = new WebClient())
            {
                var data = web.DownloadString(url);
                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<dynamic>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting JSON, error was: {ex}");
            return null;
        }
    }

    static InstagramImage GetImage(ref dynamic json, int item)
    {
        try
        {
            var items = json["items"];
            if (item >= items.Length) return null;
            var url = items[item]["images"]["standard_resolution"]["url"];
            var link = items[item]["link"];
            var caption = items[item]["caption"]?["text"] ?? string.Empty;
            return new InstagramImage(url, link, caption);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting image, error was {ex}");
            return null;
        }
    }

    static InstagramImage[] GetLatestImages(string userid, int count)
    {
        var url = $"https://www.instagram.com/{userid}/media/";
        var images = new InstagramImage[count];
        var json = GetJson(url);
        if (json == null) return images;

        for (var i = 0; i < count; i++)
        {
            images[i] = GetImage(ref json, i) ?? default(InstagramImage);
        }

        return images;
    }

	public static void Main()
	{				
        var images = GetLatestImages("jasonhalbig", 4);

		foreach (var image in images)
		{
			Console.WriteLine(image);
		}
	}
}
