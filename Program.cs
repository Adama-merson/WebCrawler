using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebCrawler
{
    class Program
    {
        // Define the maximum depth for crawling
        const int MaxDepth = 3;

        static async Task Main(string[] args)
        {
            // Initialize the priority queue and visited URLs set
            var priorityQueue = new SortedSet<(int priority, string url, int depth)>(new PriorityComparer());
            var visitedUrls = new HashSet<string>();

            // Starting URL
            string startUrl = "https://edition.cnn.com/";
            // Add the starting URL to the queue with an initial priority of 10 and depth of 1
            AddUrlToQueue(priorityQueue, startUrl, 10, 1);

            // Main crawling loop
            while (priorityQueue.Count > 0)
            {
                // Get the next URL with the highest priority (lowest number)
                var nextUrlTuple = GetNextUrl(priorityQueue);
                if (nextUrlTuple != null)
                {
                    var (nextUrl, depth) = nextUrlTuple.Value;
                    // Crawl the page if it is within the maximum depth
                    if (depth <= MaxDepth)
                    {
                        await CrawlPage(nextUrl, priorityQueue, visitedUrls, depth);
                    }
                }
            }
        }

        // Method to add a URL to the priority queue
        static void AddUrlToQueue(SortedSet<(int priority, string url, int depth)> queue, string url, int priority, int depth)
        {
            queue.Add((priority, url, depth));
        }

        // Method to get the next URL from the priority queue
        static (string url, int depth)? GetNextUrl(SortedSet<(int priority, string url, int depth)> queue)
        {
            if (queue.Count == 0) return null;
            var item = queue.Min; // Get the element with the lowest priority
            queue.Remove(item); // Remove it from the queue
            return (item.url, item.depth); // Return the URL and depth
        }

        // Method to crawl a page
        static async Task CrawlPage(string url, SortedSet<(int priority, string url, int depth)> queue, HashSet<string> visitedUrls, int depth)
        {
            visitedUrls.Add(url); // Mark the URL as visited

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            // Process all <a> tags to find new links
            foreach (var link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                var href = link.GetAttributeValue("href", string.Empty);
                var fullUrl = new Uri(new Uri(url), href).ToString();

                // If the URL hasn't been visited, add it to the queue
                if (!visitedUrls.Contains(fullUrl))
                {
                    // Determine the priority based on whether it's a pagination link or not
                    int priority = fullUrl.Contains("page=") ? 1 : 10;
                    // Add the new URL to the queue with the incremented depth
                    AddUrlToQueue(queue, fullUrl, priority, depth + 1);
                }
            }
        }
    }

    // Custom comparer to sort the priority queue
   
}
