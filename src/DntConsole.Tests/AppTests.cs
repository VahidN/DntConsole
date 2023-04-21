using DntConsole.Services.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DntFeeds.Tests;

[TestClass]
public class AppTests
{
    [TestMethod]
    public async Task TestSiteUrlIsAdded()
    {
        var blogPostsService = TestsAppFactory.GetRequiredService<IBlogPostsService>();
        var isAdded = await blogPostsService.IsUrlAddedAsync("https://www.dntips.ir");
        Assert.IsFalse(isAdded);
    }
}