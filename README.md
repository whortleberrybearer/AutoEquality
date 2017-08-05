# AutoEquality
Simple library to help generate IEqualityComparer&lt;T> instances.

`public class Person
{
    private static AutoEqualityComparer<Person> comparer = new AutoEqualityComparer<Person>();
    
    static Person()
    {
        comparer.Ignore(a => a.Id);
    }
    
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public DateTime DateOfBirth { get; set; }
    
    public override bool Equals(object x)
    {
        return comparer.Equals(this, x as Person);
    }
    
    public override GetHasCode()
    {
        return comparer.GetHashCode(this);
    }
}`
