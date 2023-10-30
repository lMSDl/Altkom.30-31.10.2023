

using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Security.Cryptography.X509Certificates;

var contextOptions = new DbContextOptionsBuilder<Context>()
                        .UseSqlServer(@"Server=(local)\SQLEXPRESS;Database=EFCore;Integrated Security=true;TrustServerCertificate=True") //Encrypt=True
                        //.UseChangeTrackingProxies()
                        .LogTo(Console.WriteLine)
                        .Options;


var context = new Context(contextOptions);

context.Database.EnsureDeleted();
context.Database.EnsureCreated();

    for (int i = 0; i < 17; i++)
    {
        var o = new Order() { };
        o.DateTime = DateTime.Now;
        var orderProduct = new Product() { Name = "Marchewka", Price = 15 };
        o.Products.Add(orderProduct);

        context.Add(o);
    }

    context.SaveChanges();

context.ChangeTracker.Clear();

var product = context.Set<Product>().Skip(5).First();

//odczytywanie wartości ShadowProperty
var orderId = context.Entry(product).Property<int>("OrderId").CurrentValue;
orderId = context.Set<Product>().Skip(4).Select(x => EF.Property<int>(x, "OrderId")).First();
orderId = context.Set<Product>().Skip(4).Select(x => x.Order.Id).First();
Console.WriteLine(orderId);

context.Entry(product).Property("OrderId").CurrentValue = 1;
context.SaveChanges();



context.Entry(product).Property("IsDeleted").CurrentValue = true;
//product.IsDeleted = true;
context.SaveChanges();

context.ChangeTracker.Clear();

var products = context.Set<Product>()/*.Where(x => !x.IsDeleted)*/.ToList();

context.ChangeTracker.Clear();

var order = context.Set<Order>().Find(1);
products = context.Set<Product>()/*.Where(x => !x.IsDeleted)*/.Where(x => EF.Property<int>(x, "OrderId") == 1).ToList();


context.Entry(order).Property("IsDeleted").CurrentValue = true;
//order.IsDeleted = true;
context.SaveChanges();

context.ChangeTracker.Clear();

product = context.Set<Product>()/*.Where(x => !x.IsDeleted)*/.First();


product = context.Set<Product>().IgnoreQueryFilters().First();
context.ChangeTracker.Clear();




static void ChangeTracker(DbContextOptions<Context> contextOptions)
{
    var order = new Order() { };
    var product = new Product() { Name = "Marchewka", Price = 15 };

    using (var context = new Context(contextOptions))
    {
        //wyłączenie automatycznego wykrywania zmian
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        /*order = context.CreateProxy<Order>() ;
        product = context.CreateProxy<Product>(x => { x.Name = product.Name; x.Price = product.Price; }) ;*/
        order.Products.Add(product);

        Console.WriteLine("Order przed dodaniem do kontekstu: " + context.Entry(order).State);
        Console.WriteLine("Product przed dodaniem do kontekstu: " + context.Entry(product).State);

        //context.Attach(order);
        context.Add(order);
        Console.WriteLine("Order po dodaniu do kontekstu: " + context.Entry(order).State);
        Console.WriteLine("Product po dodaniu do kontekstu: " + context.Entry(product).State);

        context.SaveChanges();

        Console.WriteLine("Order po zapisie do bazy: " + context.Entry(order).State);
        Console.WriteLine("Product po zapisie do bazy: " + context.Entry(product).State);

        order.DateTime = DateTime.Now;
        order.Name = "alamakota";
        Console.WriteLine("Order po zmianie daty: " + context.Entry(order).State);
        Console.WriteLine("Order DateTime zmodyfikowany? " + context.Entry(order).Property(x => x.DateTime).IsModified);
        Console.WriteLine("Order Products zmodyfikowany? " + context.Entry(order).Collection(x => x.Products).IsModified);

        context.Remove(product);
        Console.WriteLine("Product po usunięciu: " + context.Entry(product).State);

        context.SaveChanges();
        Console.WriteLine("Order po zapisie: " + context.Entry(order).State);
        Console.WriteLine("Product po zapisie: " + context.Entry(product).State);

        product.Id = 0;
        context.Attach(product);
        Console.WriteLine("Product: " + context.Entry(product).State);
        context.SaveChanges();
    }


    order.DateTime = order.DateTime.AddMinutes(-60);

    using (var context = new Context(contextOptions))
    {
        context.Attach(order);
        Console.WriteLine("Order: " + context.Entry(order).State);
        //context.Entry(order).State = EntityState.Modified;
        context.Entry(order).Property(x => x.DateTime).IsModified = true;
        Console.WriteLine("Order: " + context.Entry(order).State);
        Console.WriteLine("Order DateTime zmodyfikowany? " + context.Entry(order).Property(x => x.DateTime).IsModified);
        Console.WriteLine("Order Name zmodyfikowany? " + context.Entry(order).Property(x => x.Name).IsModified);
        Console.WriteLine("Order Products zmodyfikowany? " + context.Entry(order).Collection(x => x.Products).IsModified);
        context.SaveChanges();
        /*}

        using (var context = new Context(contextOptions))
        {*/
        context.ChangeTracker.Clear(); //wyczyszczenie kontekstu - alternatywa do tworzenia nowego kontekstu

        //AutoDetectChanges działa w przypadku wywołania: SaveChanges, Local, Entry
        //context.ChangeTracker.AutoDetectChangesEnabled = true;


        order = new Order() { };
        product = new Product() { Name = "Kapusta", Price = 10 };
        order.Products.Add(product);
        product = new Product() { Name = "Ogórki", Price = 7 };
        order.Products.Add(product);

        Console.WriteLine("Przed dodaniem do kontekstu: ");
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

        context.Add(order);
        Console.WriteLine("Po dodaniu do kontekstu: ");
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();
        Console.WriteLine("Po zapisie: ");
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

        order.DateTime = DateTime.Now;
        product.Price = 4.5f;
        Console.WriteLine("Po zmianie: ");
        context.ChangeTracker.DetectChanges(); //ręczne wykrywanie zmian
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);


        context.ChangeTracker.Clear();

        var products = new List<Product>() {
        new Product() {Name = "P1", Order = new Order {Id = 60, DateTime = DateTime.Now}},
        new Product() {Name = "P2", Order = new Order()}
    };
        //context.AddRange(products);

        foreach (var p in products)
        {

            context.ChangeTracker.TrackGraph(p.Order, entityEntry =>
            {
                if (entityEntry.Entry.IsKeySet)
                {
                    entityEntry.Entry.State = EntityState.Unchanged;
                }
                else
                {
                    entityEntry.Entry.State = EntityState.Added;
                }
            });
            context.ChangeTracker.TrackGraph(p, entityEntry =>
            {
                entityEntry.Entry.State = EntityState.Added;
            });

        }

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        //context.SaveChanges();



    }
}

static Context ConcurrencyToken(DbContextOptions<Context> contextOptions)
{
    var context = new Context(contextOptions);

    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    var order = new Order() { };
    order.DateTime = DateTime.Now;

    context.Add(order);
    context.SaveChanges();


    order.DateTime = DateTime.Now.AddMinutes(100);
    //order.Name = "alamakota";

    context.SaveChanges();

    var product = new Product() { Order = order, Name = "Marchewka", Price = 15 };

    context.Add(product);
    context.SaveChanges();


    product.Price = product.Price * 1.1f;

    var saved = false;
    do
    {
        try
        {
            context.SaveChanges();
            saved = true;
        }
        catch (DbUpdateConcurrencyException e)
        {

            foreach (var entry in e.Entries)
            {

                //wartości jakie chcmy wprowadzić do bazy
                var currentValues = entry.CurrentValues;
                //wartości jakie pobraliśmy z bazy (historyczne)
                var originalValues = entry.OriginalValues;
                //wartości jakie są aktualnie w bazie danych
                var databaseValues = entry.GetDatabaseValues();

                switch (entry.Entity)
                {
                    case Product:

                        var property = currentValues.Properties.Single(x => x.Name == nameof(Product.Price));
                        var currentPrice = (float)currentValues[property];
                        var originalPrice = (float)originalValues[property];
                        var databasePrice = (float)databaseValues[property];

                        currentPrice = databasePrice + (currentPrice - originalPrice);

                        currentValues[property] = currentPrice;

                        break;
                }
                entry.OriginalValues.SetValues(databaseValues);
            }

        }
    } while (!saved);
    return context;
}