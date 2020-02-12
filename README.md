# DBInline 1.0.0-pre-release 

![.NET Core develop](https://github.com/NicoZweifel/DBInline/workflows/.NET%20Core%20develop/badge.svg)
![.NET Core master](https://github.com/NicoZweifel/DBInline/workflows/.NET%20Core%20master/badge.svg)

- WIP
- Currently all Tests pass.
- Before using this in production, more extensive tests should be written, since there might still be bugs/unintended behaviour.

# Summary

- Collection of Classes/Interfaces/Extensions to quickly get transactions,
  commands and also pool different Databases together in the code.
  
- Contains various wrappers/static methods/overloads for different Database use-cases.

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
  it all comes down to builder interfaces and an IConnectionSource Interface.

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
       .Select("id, name")   // Column,Value Parameters are always param string[], alternatively the returned Builders can add fields/columns with Add()
       .From("Customers")
       .Where("name","John Doe") //Parameters can be generated like this.
       .Or("name","Peter Brown") // Or condition will always be added to the last where condition like so: WHERE (name=@generatedParam_1 OR name=@generatedParam2)
       .Limit(5) // Setting Result count limit
       .AddRollback(() =>
       {
           Console.WriteLine("I am a rollback lambda!"); //Add C# Rollback
       })
       .Select(r => (string) r[0]) //Create the objects
       .ToList(); //Connection will close if iterator is returned immediately, call ToList() or create another collection.
});
```
- Or:
```cs
return await TransactionAsync(t =>
            {
               return t.Query<string>()
                    .Select()
                    .Add("id") //Columns can also be added like this.
                    .Add("name") 
                    .From("Customers")
                    .Param("name","John Doe") //Adding parameter with name and value.
                    .Param(some IDbParameter Implementation) //Adding IDbParameter.
                    .Param(("@name","Peter Brown") //Adding parameter as ValueTuple.
                    .Where("name like @name")
                    .Limit(5) // Setting Result count limit
                    .Select(r => (string) r[0]) //Create the objects
                    .ToList(); //Connection will close if iterator is returned immediately, call ToList() or create another collection.
            });
```

# Pool / various Sql Wrapper Examples

```cs
using var p = Pool(); 

//SELECT

var customers = !p.Query()
    .Select()
    .From("Customers")
    .limit(5)
    .Select(r => (string)r[0]) //create desired object.
    .ToList();

//UPDATE

var i = p.Query()
    .Update("Customers")
    .Where("DBID", 10)
    .AddRollback(() =>
    {
       Console.WriteLine("I am a rollback lambda!"); //Add C# Rollback
    })
    .Run(); //ExecuteNonQuery

//DELETE

p.Query()
    .Delete("Customers")
    .Where("DBID", 5)
    .AddRollback(() =>
    {
        Console.WriteLine("I am a rollback lambda!"); //Add C# Rollback
    })
    .Select() //Chaining Delete + Select query together
    From("Customers")
    .Get(x => new Customer((int) x[0], (string) x[1]));

//INSERT INTO ... VALUES

var insertCount1 = p.Query() 
    .Insert(Customers) //InsertBuilder
    .Add("id")
    .Add("name")
    .Values()
    .AddRow()
    .AddValue(1).AddValue("John Doe")
    .AddRow()
    .AddValue(2).AddValue("James Smith")
    .AddRow()
    .AddValue(3).AddValue("Jack Williams")
    .AddRow()
    .AddValue(4).AddValue("Peter Brown")
    .AddRow()
    .AddValue(5).AddValue("Hans Mueller")
    .Run();


//INSERT INTO ... SELECT

insertCount2 = p.Query()
            .Insert(Employees)
            .Add("id")
            .Add("name")
            .Select()
            .Add("id")
            .Add("name")
            .From(Customers)
            .Run();

//Query another Database, in this example a table containing Jsons.
//Additional databases have to be registered by name in the ContextController class.

var res2 = p.Query<long>(Database2)
    .Select()
    .From("Documents")
    .Where("customerID",5)
    .Table() 
    .ToJson(); //Convert DataTable to Json skipping Serialization/Deserialization of objects.

p.Commit();  //With the using statement in place, if this is not called everything will be rollbacked.
```
# Pool lambda Examples:
```cs
return Pool(p =>
    {
        var res1 = p.Query<DataTable>()
            .Select()
            .From("Customers")
            .Where("DBID", 10)
            .Or("DBID",11)
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
        .Select("name")
        .From("Customers")
        .Where("name", "John Doe")
        .Or("name", "James Smith")
        .SelectAsync(x => (string) x[1]) //Selecting Task<List<string>>
        .ConfigureAwait(false));
    
    var customers = await p.Query<int>()
        .Select("id","name")
        .From("Customers")
        .SelectAsync(x => new Customer(x)) //Create some object.
        .ConfigureAwait(false);

    p.Query<string>()  //UPDATE + SELECT
        .Update(TableName)
        .Set("name", "John Doe2")
        .Set("id",6)
        .Where("id", 1)
        .Select()
        .Add("name")
        .From(TableName)
        .Where("id",6)
        .Scalar();
        
}).ConfigureAwait(false);
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
