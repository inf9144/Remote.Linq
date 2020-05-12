# Remote.Linq

| branch   | AppVeyor                             | Travis CI                          | Codecov.io         | Codacy                  | CodeFactor             | License                     |
| ---      | ---                                  | ---                                | ---                | ---                     | ---                    | ---                         |
| `master` | [![AppVeyor Build Status]([1])]([2]) | [![Travis build Status]([3])]([4]) | [![codecov][5]][6] | [![Codacy Badge][7]][8] | [![CodeFactor][9]][10] | [![GitHub license][11]][12] |

| package                           | nuget                        | myget                              |
| ---                               | ---                          | ---                                |
| `Remote.Linq`                     | [![NuGet Badge]([13])]([14]) | [![MyGet Pre Release]([15])]([16]) |
| `Remote.Linq.EntityFramework`     | [![NuGet Badge]([17])]([18]) | [![MyGet Pre Release]([19])]([20]) |
| `Remote.Linq.EntityFrameworkCore` | [![NuGet Badge]([21])]([22]) | [![MyGet Pre Release]([23])]([24]) |
| `Remote.Linq.Newtonsoft.Json`     | [![NuGet Badge]([25])]([26]) | [![MyGet Pre Release]([27])]([28]) |


### Description
Remote Linq is a small and easy to use - yet very powerful - library to translate LINQ expression trees to strongly typed, serializable expression trees and vice versa. It provides functionality to send arbitrary LINQ queries to a remote service to be applied and executed against any enumerable or queryable data collection.

Building a LINQ interface for custom services is made a breeze by using Remote Linq.


### Features
* Translate LINQ expressions into serializable expression trees (remote LINQ expression) and vice versa. 
* Build remote single-type query services (paging, sorting, filtering)
* Build remote complex LINQ query services (arbitrary LINQ query including joins, groupings, aggregations, projections, etc.)

### Scope
In contrast to [re-linq](https://github.com/re-motion/Relinq), this project enables serialization and deserialization of expression trees and applying LINQ expressions to other LINQ providers e.g. linq-to-object, linq-to-entity, etc. 

Remote.Linq makes it super easy to implement a service allowing LINQ queries defined on a client to be executed on a remote server. 

Write operations (insert/update/delete) have to be implemented by other means if needed. [InfoCarrier.Core](https://github.com/azabluda/InfoCarrier.Core) might be interesting for such scenarios.

### Sample

#### Client

Implement a repository class to set-up server connection and expose the queryable data sets (`IQueryable<>`)
```C#
public class ClientDataRepository
{
    private readonly Func<Expression, IEnumerable<DynamicObject>> _dataProvider;

    public RemoteRepository(string uri)
    {
        _dataProvider = expression =>
            {
                // setup service connectivity
                IQueryService service = CreateServerConnection(uri);
                // send expression to service and get back results
                IEnumerable<DynamicObject> result = service.ExecuteQuery(expression);
                return result;
            };
    }

    public IQueryable<Blog> Blogs => RemoteQueryable.Factory.CreateQueryable<Blog>(_dataProvider);
   
    public IQueryable<Post> Posts => RemoteQueryable.Factory.CreateQueryable<Post>(_dataProvider);
   
    public IQueryable<User> Users => RemoteQueryable.Factory.CreateQueryable<User>(_dataProvider);
}
```

Use your repository to compose LINQ query and let the data be retrieved from the backend service
```C#
var repository = new ClientDataRepository();

var myBlogPosts = (
    from blog in repository.Blogs
    from post in blog.Posts
    join owner in repository.Users on blog.OwnerId equals owner.Id
    where owner.login == "hi-its-me"
    select new 
    {
        post.Title,
        post.Date,
        Preview = post.Text.Substring(0, 50)
    }).ToList();
```

#### Server

Implement the backend service to handle the client's query expression by applying it to a data source e.g. an ORM

```C#
public interface IQueryService
{
    IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression);
}

public class QueryService : IQueryService, IDisposable
{
    // any linq provider e.g. entity framework, nhibernate, ...
    private IDataProvider _datastore = new ObjectRelationalMapper();

    // you need to be able to retrieve an IQueryable by type
    private Func<Type, IQueryable> _queryableProvider = type => _datastore.GetQueryableByType(type);

    public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
    {
        // `Execute` is an extension method provided by Remote.Linq
        // it applies an expression to a data source and returns the result
        return queryExpression.Execute(queryableProvider: _queryableProvider);
    }

    public void Dispose()
    {
        _datastore.Dispose();
    }
}
```

## Remote.Linq.EntityFramework / Remote.Linq.EntityFrameworkCore

Remote linq extensions for entity framework and entity framework core. 

Use this package when using features specific to EF6 and EF Core:
- Apply eager-loading (`Include`-expressions)
- Make use of DB functions 
  e.g. `queryable.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Name, "%fruit%"))`

### Sample

#### Client

Query blogs including posts and owner

```C#
using var repository = new RemoteRepository();
var blogs = repository.Blogs
    .Include("Posts")
    .Include("Owner")
    .ToList();
```

#### Server

Execute query on database via EF Core

```C#
public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
{
    using var dbContext = new DbContext();
    return queryExpression.ExecuteWithEntityFrameworkCore(dbContext);
}
```

## Remote.Linq.Newtonsoft.Json

Provides [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) serialization settings for Remote.Linq types.

### Sample

```C#
public TExpression Demo<TExpression>(TExpression expression)
    where TExpression : Remote.Linq.Expressions.Expression
{
    JsonSerializerSettings serializerSettings = new JsonSerializerSettings().ConfigureRemoteLinq();
 
    string json = JsonConvert.SerializeObject(expression, serializerSettings);
 
    TExpression result = JsonConvert.DeserializeObject<TExpression>(json, serializerSettings);
 
    return result;
}
```

[1]: https://ci.appveyor.com/api/projects/status/64kw6dsuvfwyrdtl/branch/master?svg=true
[2]: https://ci.appveyor.com/project/6bee/remote-linq/branch/master
[3]: https://travis-ci.org/6bee/Remote.Linq.svg?branch=master
[4]: https://travis-ci.org/6bee/Remote.Linq?branch=master
[5]: https://codecov.io/gh/6bee/Remote.Linq/branch/master/graph/badge.svg
[6]: https://codecov.io/gh/6bee/Remote.Linq
[7]: https://api.codacy.com/project/badge/Grade/c00e5959612843a78e23d10b4bad44da
[8]: https://www.codacy.com/manual/6bee/Remote.Linq
[9]: https://www.codefactor.io/repository/github/6bee/Remote.Linq/badge
[10]: https://www.codefactor.io/repository/github/6bee/Remote.Linq
[11]: https://img.shields.io/github/license/6bee/Remote.Linq.svg
[12]: https://github.com/6bee/Remote.Linq/blob/master/license.txt
[13]: https://buildstats.info/nuget/Remote.Linq?includePreReleases=true
[14]: https://www.nuget.org/packages/Remote.Linq
[15]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.svg?style=flat-square&label=myget
[16]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq
[17]: https://buildstats.info/nuget/Remote.Linq.EntityFramework?includePreReleases=true
[18]: https://www.nuget.org/packages/Remote.Linq.EntityFramework
[19]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.EntityFramework.svg?style=flat-square&label=myget
[20]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.EntityFramework
[21]: https://buildstats.info/nuget/Remote.Linq.EntityFrameworkCore?includePreReleases=true
[22]: https://www.nuget.org/packages/Remote.Linq.EntityFrameworkCore
[23]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.EntityFrameworkCore.svg?style=flat-square&label=myget
[24]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.EntityFrameworkCore
[25]: https://buildstats.info/nuget/Remote.Linq.Newtonsoft.Json?includePreReleases=true
[26]: https://www.nuget.org/packages/Remote.Linq.Newtonsoft.Json
[27]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.Newtonsoft.Json.svg?style=flat-square&label=myget
[28]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.Newtonsoft.Json
