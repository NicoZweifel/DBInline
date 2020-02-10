# DBInline 1.0.0

- WIP
- Currently all Tests pass.
- Before using this in production, more testing should be done.

# Summary

- Collection of Classes/Interfaces/Extensions to quickly get transactions,
  commands and also pool different Databases together in the code.
  Contains various wrappers/static methods/overloads for different Database use-cases.
  
- Rollbacks/Disposing is handled in the background.

- Commits, Rollbacks all transactions/connections of multiple databases are created and handled together.

- Will rollback if an exception occurs or if you are using the Pool class and forget to call Commit().

- The Lambda version does not need the Commit call, but you have to pass a lambda instead.

- There is also extensions to quickly get a transaction or command going.

- The Transactions rollback in the Pool.Dispose() method if an unhandled exception occurs.

- The Lambda version handles the transactions and connections after the lambda execution fails/completes.

# DatabaseContext

- The DbContext is identified with a name and you can register a new context in the ContextController class.

- If no name is passed, a defaultContext is used.

- Postgres and MsSql are tested, MySql isn't but should work as well if a ConnectionString is defined in the DatabaseContext class.

- SqlLite might be removed in the future, since it does not support everything anyway.

# Async

- Contains async versions for every method also async Lambda Pool versions,
  in case it's necessary to query different Databases at the same time, rollbacks for both and also rollbacks in the application.

# Usage

- It is not forbidden to nest pools, some tests passed, but it is not intended/supported and may yield unexpected results.

- There is a lot of options on how to select objects (DataSet,DataTable,Reader etc...),
  which makes the api a little unintuitive in some places, since there is too many options at the moment.

- Some of them might be removed for clearer Usage and improved generic type inference.

- Since everything is connected through interfaces, all these methods can be mixed,
  it all comes down to a IQueryBuilder and IConnectionSource Interfaces.

# Using Pool

- Don't forget to use using ```cs var p = new Pool()``` on this variant, or ur pool will not be rollbacked/disposed/handled properly.
- If using lambdas is preferred, there are static functions that take a lambda as parameter.

- These have to be imported.
  E.g.: 
 ```cs
 using static DBInline.Extensions;
 ```


# Transaction Examples:
```cs
var t = Transaction(t =>
{
   return t.Query<string>()
       .Set('Some query')
       .Where("name","John Doe") //Parameters can be generated like this.
       .Or("name","Peter Brown") // Or condition will always be added to the last where condition like so: WHERE (name=@generatedParam_1 OR name=@generatedParam2)
       .Limit(5) // Setting Result count limit
       .AddRollback(() =>
       {
           Console.WriteLine("I am a rollback lambda!"); //Add C# Rollback
       })
       .Select(r => (string) r[0]) //Create the objects
       .ToList();
});
```
- Or:
```cs
return await TransactionAsync(t =>
            {
               return t.Query<string>()
                    .Set('Some query')
                    .Param("name","John Doe") //Adding parameter with name and value.
                    .Param(some IDbParameter Implementation) //Adding IDbParameter.
                    .Param(("@name","Peter Brown") //Adding parameter as ValueTuple.
                    .Where("name like @name")
                    .Limit(5) // Setting Result count limit
                    .Select(r => (string) r[0]) //Create the objects
                    .ToList();
            });
```

# CMD/QueryRun Examples (these will probably be removed):
```cs
return QueryRun<List<string>> ...
```
- Or:
```cs
return await QueryAsync<List<string>>('Some query', cmd =>
            {
                {
                    return cmd
                        .Select(r=>(string)r[0]) //Create the objects
                        .ToList();
                }
            });

```

# Pool Examples

```cs
using var p = Pool(); 
var i = p.Query()
    .Set('Some update/delete query')
    .Param("DBID",10)
    .Where("DBID = @DBID")
    .AddRollback(() =>
    {
       Console.WriteLine("I am a rollback lambda!"); //Add C# Rollback
    })
    .Run(); //ExecuteNonQuery

var res1 = !p.Query()
    .Set('Some select query')
    .limit(10)
    .Select(r => (string)r[0]) //create desired object.
    .ToList();

for (var counter = 1; counter< 10;counter++)
{
    p.Query()
        .Set('Update/delete query')
        .Param("DBID",counter)
        .Where("DBID = @DBID")
        .AddRollback(() =>
        {
            Console.WriteLine("I am a rollback lambda!"); //Add C# Rollback
        })
        .Run(); //ExecuteNonQuery
}

var res2 = p.Query<long>(Database2) //Query another Database
    .Set('Some query')
    .Param("param", "value") //Add parameter with name and value
    .Where("name = @param")
    .Scalar(); //ExecuteScalar

p.Commit();  //With the using statement in place, if this is not called everything will be rollbacked.
```
# Pool lambda Examples:
```cs
return Pool(p =>
    {
        var res1 = p.Query<DataTable>()
            .Set('Some select query')
            .Param("DBID",10)
            .Where("DBID = @DBID")
            .Table(); //Select as Datatable
        return res1.ToJson() //Extension to immediately create Json out of a Datatable;
    });
}          
```
- Or:
```cs
var t =PoolAsync(p => ...            
```        
- async lambdas are only handled in the Pool-Extensions:         
```cs 
return await PoolAsync(async p =>
                   {
                       var list = new List<string>();
       
                       await foreach (var x in p.Query()
                           .Set(SelectQuery)
                           .SelectAsyncEnumerable(x => (string) x[1])) //Selecting IAsyncIEnumerable<T>
                       {
                           list.Add(x);
                       }
       
                       var johnJames = (await p.Query()
                           .Set(SelectQuery)
                           .Where("name", "John Doe")
                           .Or("name", "James Smith")
                           .SelectAsync(x => (string) x[1]) //Selecting Task<List<T>
                           .ConfigureAwait(false));
       
                       var peter = await p.Query<int>()
                           .Set(SelectQuery)
                           .ScalarAsync()
                           .ConfigureAwait(false);
       
                   }).ConfigureAwait(false);
```

# Tests

- Contains TestProject that will create a Test-Table named dbinline_generated_table
- Put Credentials @ DBInline.Test/bin/debug||release/netcoreapp3.1/credentials
```cs            
if (ContextController.Connected) return;
var path = Path.Combine(Environment.CurrentDirectory, "credentials\\data.json");
if (!File.Exists(path))
{
    File.WriteAllText(path,JsonSerializer.Serialize(new List<DatabaseCredentials> {new DatabaseCredentials()}));
    throw new Exception($"No Database Credentials found, file has been created at {path}");
}
var credentials = JsonSerializer.Deserialize<DatabaseCredentials[]>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "credentials\\data.json")));
```