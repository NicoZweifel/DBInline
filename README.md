# DBInline 1.0.0-alpha

- WIP
- Currently all Tests pass.
- Alpha Release Version. Should not be used in Production.

# Summary

- Collection of Classes/Interfaces/Extensions to quickly get transactions,
  commands and also pool different Databases in one Connectionpool.
  
- Rollbacks/Disposing is handled in the background.

- Commits, Rollbacks all transactions/connections of multiple databases are created and pooled together.

- Will rollback if an exception occurs or if you are using the Pool class and forget to call Commit().

- The Lambda version does not need the Commit call, but you have to pass a lambda instead.

- There is also extensions to quickly get a transaction or command going.

- The Transactions rollback in the Pool.Dispose() method if an unhandled exception occurs.

- The Lambda version handles the transactions and connections after the lambda execution fails/completes.

# DatabaseContext

- The DBcontext is identified with a name and you can register a new context in the ContextController class.

- If no name is passed, a defaultContext is used.

- Postgres and MsSql are tested, MySql isn't but should work as well if a connectionstring is defined in the DatabaseContext class.

- Sqllite might be removed in the future, since it does not support everything anyway.

# Async

- Contains async versions for every method also async Lambda Pool versions,
  so you can query different Databases at the same time and everything will be rollbacked if an exception occurs.

# Usage

- It is not forbidden to nest pools, some tests passed, but it is not intended/supported and may yield unexpected results.

- There is a lot of options on how to select objects (Datatset,Datatable,Reader etc...),
  which makes the api a little unintuitive in some places, since there is too many options at the moment.

- Some of them might be removed for clearer Usage and improved generic type inference.

- Since everything is connected through interfaces, all these methods can be mixed,
  it all comes down to a IQueryBuilder and IConnectionSource Interfaces.

# Using Pool

- Don't forget to use using ```cs var p = new Pool()``` on this variant, or ur pool will not be rollbacked/disposed/handled properly.
- If using lambdas is preferred, there are static functions that take a lambda as parameter.

- These have to be statically imported.
  E.g.: 
 ```cs
 using static DBInline.Extensions;
 ```

# Pool Examples

```cs
using var p = Pool(); 
var i = p.Query()
    .Set('Some update/delete query')
    .Param('Some parameter') //Multiple options to add parameters
    .AddRollback(() => 'some C# rollback action')
    .Run(); //ExecuteNonQuery

var res1 = !p.Query()
    .Set('Some select query')
    .Select(r => (string)r[0]) //create desired object.
    .ToList();

for (var counter = 1; counter< 10;counter++)
{
    p.Query()
        .Set('Update/delete query')
        .AddRollback(() => 'some C# rollback action')
        .Run(); //ExecuteNonQuery
}

var res2 = p.Query<long>(Database2) //Query another Database
    .Set('Some query')
    .Param('param Name', 'param value') //Add parameter with name and value
    .Scalar(); //ExecuteScalar

p.Commit();  //With the using statement in place, if this is not called everything will be rollbacked.
```
# Pool lambda Examples:
```cs
return Pool(p =>
    {
        var res1 = p.Query<DataTable>()
            .Set('Some select query')
            .Param('Some parameter') //param with SimpleParameter class can also be called with (name,value).
            .Table(); //Select as Datatable
        return res1;
    });
}          
```
- Or:
```cs
var t =PoolAsync(p => ...            
```        
- Or if an async lambda is necessary (for querying multiple different Databases at once.):            
```cs 
var t = PoolAsync(async p =>
            {
                var asyncIe =  p.Query<string>() 
                    .Set('Some query')
                    .SelectAsync(r=> (string)r[0]); //Select AsyncIenumerable.

                var res1 = "";
                    await foreach(var obj in asyncIe) //AsyncIenumerable
                    {
                        res1 += obj;
                    }

                var res2 = p.Query<string>()
                    .Set('Some query')
                    .Param('Some parameter')
                    .AddRollback(() => { })
                    .ScalarAsync();

                var table = (await p.Query<string>()
                        .Set('Some select query')
                        .TableAsync()) //Select as DataTable
                        .ToJson(); //Create Json from Table

                return res1 + await res2 + table;
            });
            t.Wait();
            return t.Result;
```

# CMD/QueryRun Examples:
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

# Transaction Examples:
```cs
var t = Transaction(t => ...
```
- Or:
```cs
return await TransactionAsync(t =>
            {
               return t.Query<string>()
                    .Set('Some query')
                    .Select(r => (string) r[0]) //Create the objects
                    .ToList();
            });
```

# Tests

- On seperate Branch cause they are a mess.
- DB Instance dependent and chaotic, see:[Improve Test Project](https://github.com/NicoZweifel/DBInline/issues/1)
- Eventually there will be Tables generated and used in the tests.
