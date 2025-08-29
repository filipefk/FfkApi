using FfkApi.Communication.Requests;

namespace TestUtil.Requests;

public class RequestCadastrarFeedEmLoteBuilder
{
    public static RequestCadastrarFeedEmLote Build(int? quantos = 3)
    {
        var feeds = new List<RequestCadastrarFeed>();

        for (var i = 0; i < quantos; i++)
        {
            feeds.Add(RequestCadastrarFeedBuilder.Build());
        }

        return new RequestCadastrarFeedEmLote
        {
            Feeds = feeds
        };
    }

    public static RequestCadastrarFeedEmLote Build(FfkApi.Domain.Entities.Feed[] feeds)
    {
        return new RequestCadastrarFeedEmLote
        {
            Feeds = feeds.Select(RequestCadastrarFeedBuilder.Build).ToList()
        };
    }
}
