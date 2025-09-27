public class AdPlatformRepositoryTests
{
    private readonly string _testData = @"
        Яндекс.Директ:/ru
        Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
        Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl
        Крутая реклама:/ru/svrd
        ";

    [Fact]
    public void LoadFromText_ShouldLoadPlatforms_ForRoot()
    {
        var repo = new AdPlatformRepository();
        repo.LoadFromText(_testData);

        var result = repo.FindPlatforms("/ru");

        Assert.Contains("Яндекс.Директ", result);
        Assert.DoesNotContain("Ревдинский рабочий", result);
    }

    [Fact]
    public void Search_ShouldReturnPlatforms_ForNestedLocation()
    {
        var repo = new AdPlatformRepository();
        repo.LoadFromText(_testData);

        var result = repo.FindPlatforms("/ru/svrd/revda");

        Assert.Contains("Яндекс.Директ", result);
        Assert.Contains("Ревдинский рабочий", result);
        Assert.Contains("Крутая реклама", result);
    }

    [Fact]
    public void Search_ShouldReturnPlatforms_ForRegion()
    {
        var repo = new AdPlatformRepository();
        repo.LoadFromText(_testData);

        var result = repo.FindPlatforms("/ru/svrd");

        Assert.Contains("Яндекс.Директ", result);
        Assert.Contains("Крутая реклама", result);
        Assert.DoesNotContain("Ревдинский рабочий", result);
    }

    [Fact]
    public void Reload_ShouldReplaceData()
    {
        var repo = new AdPlatformRepository();
        repo.LoadFromText(_testData);

        var newData = "NewAd:/ru/ekb";
        repo.LoadFromText(newData);

        var resultOld = repo.FindPlatforms("/ru");
        var resultNew = repo.FindPlatforms("/ru/ekb");

        Assert.DoesNotContain("Яндекс.Директ", resultOld);
        Assert.Contains("NewAd", resultNew);
    }
}