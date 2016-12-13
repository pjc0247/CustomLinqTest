CustomLinqTest
=====

LINQ EXPR -> QUERY

```cs
class Player : Entity
{
    public string Name;
    public int Level;
}

class Program
{
    static void Main(string[] args)
    {
        QueryBuilder<Player>
            .Query
            .Where(x => x.Name == "park")
            .Where(x => x.Level > 10)
            
            .DumpQuery();
    }
}
```
```
[CustomLINQTest.Player]
   - x.Name Eq "park"
   - x.Level Gt 10
```
